using QuartetEditor.Extensions;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Reactive.Subjects;
using ICSharpCode.AvalonEdit.Document;
using System.Reactive.Disposables;
using QuartetEditor.Models.Undo;
using QuartetEditor.Enums;
using System.IO;
using QuartetEditor.Entities;
using QuartetEditor.Utilities;
using System.Text.RegularExpressions;

namespace QuartetEditor.Models
{
    /// <summary>
    ///データモデルクラス
    /// </summary>
    public class NodeManager : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// ノードの変更状態を通知
        /// </summary>
        public IObservable<bool> NodeEdited => _NodeEdited;

        private Subject<bool> _NodeEdited = new Subject<bool>();

        /// <summary>
        /// 内部データクラス
        /// </summary>
        private ObservableCollection<Node> TreeSource { get; } = new ObservableCollection<Node>();

        /// <summary>
        /// 読み取り専用データツリー
        /// </summary>
        public ReadOnlyObservableCollection<Node> Tree { get; }

        #region ViewState

        /// <summary>
        /// 選択されているノード
        /// </summary>
        private Node _SelectedNode;

        public Node SelectedNode
        {
            get { return this._SelectedNode; }
            set { this.SetProperty(ref this._SelectedNode, value); }
        }

        /// <summary>
        /// 編集パネルのコンテンツ
        /// </summary>
        public TextDocument TextContent
        {
            get
            {
                return this.SelectedNode?.Content;
            }
        }

        /// <summary>
        /// 左参照パネルのノード
        /// </summary>
        private Node _ParentNode;

        private Node ParentNode
        {
            get { return this._ParentNode; }
            set
            {
                this._ParentNode = value;
                this.OnPropertyChanged(nameof(this.ParentTextContent));
            }
        }

        /// <summary>
        /// 左参照パネルのコンテンツ
        /// </summary>
        public string ParentTextContent
        {
            get
            {
                return this.ParentNode?.Content?.Text;
            }
        }

        /// <summary>
        /// 上参照パネルのノード
        /// </summary>
        private Node _PrevNode;

        private Node PrevNode
        {
            get { return this._PrevNode; }
            set
            {
                this._PrevNode = value;
                this.OnPropertyChanged(nameof(this.PrevTextContent));
            }
        }

        /// <summary>
        /// 上参照パネルのコンテンツ
        /// </summary>
        public string PrevTextContent
        {
            get
            {
                return this.PrevNode?.Content?.Text;
            }
        }

        /// <summary>
        /// 下参照パネルのノード
        /// </summary>
        private Node _NextNode;

        private Node NextNode
        {
            get { return this._NextNode; }
            set
            {
                this._NextNode = value;
                this.OnPropertyChanged(nameof(this.NextTextContent));
            }
        }

        /// <summary>
        /// 下参照パネルのコンテンツ
        /// </summary>
        public string NextTextContent
        {
            get
            {
                return this.NextNode?.Content?.Text;
            }
        }

        /// <summary>
        /// 参照されているノードの参照フラグを再設定
        /// </summary>
        public void UpdatePanelReffer()
        {
            if (this.ParentNode != null)
            {
                this.ParentNode.IsReferred = ConfigManager.Current.Config.LeftPanelOpen;
            }
            if (this.PrevNode != null)
            {
                this.PrevNode.IsReferred = ConfigManager.Current.Config.TopPanelOpen;
            }
            if (this.NextNode != null)
            {
                this.NextNode.IsReferred = ConfigManager.Current.Config.BottomPanelOpen;
            }
        }

        /// <summary>
        /// ノード名変更モードを呼び出す
        /// </summary>
        public void CallNameEditMode()
        {
            this.SelectedNode.IsNameEditMode = true;
        }

        #endregion ViewState

        /// <summary>
        /// Node管理クラス
        /// </summary>
        public NodeManager()
        {
            // 接続
            this.Tree = new ReadOnlyObservableCollection<Node>(this.TreeSource);

            // プロパティの変更を監視する
            this.TreeSource.ObserveElementProperty(x => x.IsEdited).Where(x => x.Value)
                .Merge(this.TreeSource.ObserveElementProperty(x => x.ChildrenEdited).Where(x => x.Value))
                .Subscribe(x =>
                {
                    this._NodeEdited.OnNext(true);
                }).AddTo(this.Disposable);

            this.TreeSource.CollectionChangedAsObservable()
                .Subscribe(x => this._NodeEdited.OnNext(true) )
                .AddTo(this.Disposable);

            #region ViewState

            this.ObserveProperty(c => c.SelectedNode).Subscribe(_ =>
            {
                this.WorkAllNode(n => n.IsReferred = false);
                this.OnPropertyChanged(nameof(this.TextContent));
                this.PrevNode = this.GetPrev(this.SelectedNode);
                this.NextNode = this.GetNext(this.SelectedNode);
                this.ParentNode = this.GetParent(this.SelectedNode);
                this.UpdatePanelReffer();

                this._ChangedCanMoveUp.OnNext(this.GetOlder(this.SelectedNode) != null);
                this._ChangedCanMoveDown.OnNext(this.GetYounger(this.SelectedNode) != null);
                this._ChangedCanMoveChild.OnNext(this.GetOlder(this.SelectedNode) != null);
                this._ChangedCanMoveParent.OnNext(this.GetParent(this.SelectedNode) != null);
            }).AddTo(this.Disposable);

            #endregion ViewState

            #region UndoRedo
            this.UndoRedoModel.ChangedCanRedo.Subscribe(b => this._ChangedCanRedo.OnNext(b));
            this.UndoRedoModel.ChangedCanUndo.Subscribe(b => this._ChangedCanUndo.OnNext(b));
            #endregion UndoRedo

            // 空の場合は初期化
            if (this.TreeSource.Count == 0)
            {
                this.TreeSource.Add(new Node() { IsSelected = true });
            }

            this.OffEditFlag();
        }

        /// <summary>
        /// すべての編集フラグをオフにします
        /// </summary>
        public void OffEditFlag()
        {
            this.WorkAllNode(n => n.IsEdited = false);
            this.WorkAllNode(n => n.ChildrenEdited = false);
        }

        /// <summary>
        /// すべてのノードに対しての操作を提供します
        /// </summary>
        /// <param name="act"></param>
        private void WorkAllNode(Action<Node> act)
        {
            this.Tree.ForEach(node => node.WorkAllNode(act));
        }

        /// <summary>
        /// 条件を満たすノードが存在するか判定します
        /// </summary>
        /// <returns></returns>
        public bool HasAnyNode(Predicate<Node> predicate)
        {
            return this.Tree.Any(node => node.HasAnyNode(predicate));
        }

        #region NodeTransaction

        /// <summary>
        /// ノードを削除する
        /// </summary>
        public void DeleteNode()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> tree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (tree == null)
            {
                tree = this.TreeSource;
            }
            int index = tree.IndexOf(this.SelectedNode);

            // 削除後にすべてのノードが空になるか
            bool isLastItem = this.TreeSource == tree && this.TreeSource.Count() == 1;

            // 行う操作
            object[] doParam = new object[] { tree, index, isLastItem };
            var doAction = new Action<IList<Node>, int, bool>((_tree, _index, _isLastItem) =>
            {
                this.DeleteTransaction(_tree, _index);
                if (_isLastItem)
                {
                    // ノードが空になる場合は勝手に追加する
                    _tree.Add(new Node());
                }

                var nextSelected = _tree.ElementAtOrDefault(_index - 1 < 0 ? 0 : _index - 1);
                if (nextSelected != null)
                {
                    nextSelected.IsSelected = true;
                }
            });

            // 取り消す操作
            object[] undoParam = new object[] { tree, index, this.SelectedNode, isLastItem };
            var undoAction = new Action<IList<Node>, int, Node, bool>((_tree, _index, _item, _isLastItem) =>
            {
                if (_isLastItem)
                {
                    tree.RemoveAt(0);
                }

                this.AddTransaction(_tree, _index, _item);
            });

            // 操作実行
            this.UndoRedoModel.Do(doAction, doParam, undoAction, undoParam);
        }

        /// <summary>
        /// ノードを同階層に追加する
        /// </summary>
        public void AddNodeSame()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> tree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (tree == null)
            {
                tree = this.TreeSource;
            }
            int index = tree.IndexOf(this.SelectedNode) + 1;

            this.AddNode(tree, index);
        }

        /// <summary>
        /// ノードを下階層に追加する
        /// </summary>
        public void AddNodeLower()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> tree = this.SelectedNode.ChildrenSource;
            int index = tree.Count();

            this.AddNode(tree, index);
        }

        /// <summary>
        /// ノードを追加する
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index"></param>
        private void AddNode(IList<Node> tree, int index)
        {
            // 行う操作
            var newItem = new Node();
            this.AddNode(tree, index, newItem);
        }

        /// <summary>
        /// ノードを追加する
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index"></param>
        private void AddNode(IList<Node> tree, int index, Node addItem)
        {
            // 行う操作
            object[] doParam = new object[] { tree, index, addItem };
            var doAction = new Action<IList<Node>, int, Node>((_tree, _index, _addItem) =>
            {
                this.AddTransaction(_tree, _index, _addItem);
            });

            // 取り消す操作
            object[] undoParam = new object[] { tree, index };
            var undoAction = new Action<IList<Node>, int>((_tree, _index) =>
            {
                this.DeleteTransaction(_tree, _index);
            });

            // 操作実行
            this.UndoRedoModel.Do(doAction, doParam, undoAction, undoParam);
        }

        /// <summary>
        /// ノードを追加する
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        private void AddTransaction(IList<Node> tree, int index, Node item)
        {
            tree.Insert(index, item);

            var parent = this.GetParent(item);
            if (parent != null)
            {
                parent.IsExpanded = true;
            }
        }

        /// <summary>
        /// ノードの複製
        /// </summary>
        /// <returns></returns>
        public void Reproduce()
        {
            if (this.SelectedNode == null)
            {
                return;
            }

            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            var newItem = new Node(this.SelectedNode);
            IList<Node> tree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (tree == null)
            {
                tree = this.TreeSource;
            }

            int index = tree.IndexOf(this.SelectedNode) + 1;

            this.AddNode(tree, index, newItem);
        }

        /// <summary>
        /// ノードを削除する
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index"></param>
        private void DeleteTransaction(IList<Node> tree, int index)
        {
            var deleteItem = tree.ElementAt(index);

            var parent = this.GetParent(deleteItem);
            if (parent != null)
            {
                parent.IsExpanded = true;
            }

            tree.RemoveAt(index);
        }

        /// <summary>
        /// ノードを削除する
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="item"></param>
        private void DeleteTransaction(IList<Node> tree, Node item)
        {
            var parent = this.GetParent(item);
            if (parent != null)
            {
                parent.IsExpanded = true;
            }

            tree.Remove(item);
        }

        /// <summary>
        /// ノードを下に移動する
        /// </summary>
        public void MoveDown()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> fromTree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (fromTree == null)
            {
                fromTree = this.TreeSource;
            }

            int fromIndex = fromTree.IndexOf(this.SelectedNode);

            this.Move(fromTree, fromIndex, fromTree, fromIndex + 2, this.SelectedNode, null);

        }

        /// <summary>
        /// 「ノードを下に移動する」実行可否
        /// </summary>
        public IObservable<bool> ChangedCanMoveDown => _ChangedCanMoveDown;

        private Subject<bool> _ChangedCanMoveDown = new Subject<bool>();

        /// <summary>
        /// ノードを上に移動する
        /// </summary>
        public void MoveUp()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> fromTree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (fromTree == null)
            {
                fromTree = this.TreeSource;
            }

            int fromIndex = fromTree.IndexOf(this.SelectedNode);

            this.Move(fromTree, fromIndex, fromTree, fromIndex - 1, this.SelectedNode, null);
        }

        /// <summary>
        /// 「ノードを上に移動する」実行可否
        /// </summary>
        public IObservable<bool> ChangedCanMoveUp => _ChangedCanMoveUp;

        private Subject<bool> _ChangedCanMoveUp = new Subject<bool>();

        /// <summary>
        /// ノードを右に移動する
        /// </summary>
        public void MoveChild()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> fromTree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (fromTree == null)
            {
                fromTree = this.TreeSource;
            }

            int fromIndex = fromTree.IndexOf(this.SelectedNode);

            IList<Node> toTree = this.GetOlder(this.SelectedNode)?.ChildrenSource;
            if (toTree == null)
            {
                return;
            }

            this.Move(fromTree, fromIndex, toTree, toTree.Count, this.SelectedNode, null);
        }

        /// <summary>
        /// 「ノードを右に移動する」実行可否
        /// </summary>
        public IObservable<bool> ChangedCanMoveChild => _ChangedCanMoveChild;

        private Subject<bool> _ChangedCanMoveChild = new Subject<bool>();

        /// <summary>
        /// ノードを左に移動する
        /// </summary>
        public void MoveParent()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            IList<Node> fromTree = this.GetParent(this.SelectedNode)?.ChildrenSource;
            if (fromTree == null)
            {
                fromTree = this.TreeSource;
            }

            int fromIndex = fromTree.IndexOf(this.SelectedNode);

            var parent = this.GetParent(this.SelectedNode);
            if (parent == null)
            {
                return;
            }

            IList<Node> toTree = this.GetParent(parent)?.ChildrenSource;
            if (toTree == null)
            {
                toTree = this.TreeSource;
            }

            this.Move(fromTree, fromIndex, toTree, toTree.IndexOf(parent) + 1, this.SelectedNode, null);
        }

        /// <summary>
        /// 「ノードを左に移動する」実行可否
        /// </summary>
        public IObservable<bool> ChangedCanMoveParent => _ChangedCanMoveParent;

        private Subject<bool> _ChangedCanMoveParent = new Subject<bool>();

        /// <summary>
        /// ノードの移動処理
        /// </summary>
        private void Move(IList<Node> fromTree, int fromIndex,
                                     IList<Node> toTree, int toIndex,
                                     Node item, Node target)
        {
            // 行う操作
            object[] doParam = new object[] { fromTree, fromIndex, toTree, toIndex, item, target };
            var doAction = new Action<IList<Node>, int, IList<Node>, int, Node, Node>((_fromTree, _fromIndex, _toTree, _toIndex, _item, _target) =>
            {
                if (_fromTree == _toTree)
                {
                    if (_fromIndex < _toIndex)
                    {
                        // 同じTreeで下へ移動する場合
                        // 先に削除を行うため、移動先のインデックスを調整する
                        --_toIndex;
                    }
                }
                this.MoveTransaction(
                    _fromTree, _fromIndex,
                    _toTree, _toIndex,
                    _item);

                if (_target?.ChildrenSource == _toTree)
                {
                    _target.IsExpanded = true;
                }
            });

            // 取り消す操作
            object[] undoParam = new object[] { toTree, toIndex, fromTree, fromIndex, item };
            var undoAction = new Action<IList<Node>, int, IList<Node>, int, Node>((_fromTree, _fromIndex, _toTree, _toIndex, _item) =>
            {
                if (_fromTree == _toTree)
                {
                    if (_fromIndex > _toIndex)
                    {
                        --_fromIndex;
                    }
                }
                this.MoveTransaction(
                    _fromTree, _fromIndex,
                    _toTree, _toIndex,
                    _item);
            });

            // 操作実行
            this.UndoRedoModel.Do(doAction, doParam, undoAction, undoParam);
        }

        /// <summary>
        /// 移動を実行
        /// </summary>
        private void MoveTransaction(IList<Node> fromTree, int fromIndex,
                                     IList<Node> toTree, int toIndex,
                                     Node item)
        {
            var fromParent = this.GetParent(item);
            if (fromParent != null)
            {
                fromParent.IsExpanded = true;
            }

            // 移動元から削除
            fromTree.RemoveAt(fromIndex);

            // 移動先に挿入
            toTree.Insert(toIndex, item);

            var toParent = this.GetParent(item);
            if (toParent != null)
            {
                toParent.IsExpanded = true;
            }

        }

        /// <summary>
        /// 見出しからノードを自動生成
        /// </summary>
        public void AddNodeFromHeader()
        {
            if (this.SelectedNode.IsNameEditMode)
            {
                return;
            }

            if (this.SelectedNode == null)
            {
                return;
            }

            string text = this.SelectedNode.Content.Text;
            char[] headerChars = ConfigManager.Current.Config.HeaderCharactors.ToCharArray();
            if (text.IndexOfAny(headerChars) == -1)
            {
                // そもそも見出し文字がない場合は何もせず復帰
                return;
            }

            // 改行コードが２文字だと都合が悪いので一時的に置き換え
            text = text.Replace(Environment.NewLine, "\r");
            var addItems = new Stack<Node>();

            int index = 0;
            while (text.IndexOfAny(headerChars, index) != -1)
            {
                bool empty = false;

                int headerStart = text.IndexOfAny(headerChars, index);
                int headerEnd = text.IndexOf('\r', headerStart + 1);
                if (headerEnd == -1)
                {
                    empty = true;
                    headerEnd = text.Length;
                }
                string name = text.Substring(headerStart + 1, headerEnd - headerStart - 1);
                index = headerEnd + 1;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var addItem = new Node();
                addItem.Name = name;

                if (!empty)
                {
                    int contentEnd = text.IndexOfAny(headerChars, index);
                    if (contentEnd == -1)
                    {
                        contentEnd = text.Length;
                        empty = true;
                    }
                    string content = text.Substring(index, contentEnd - index);
                    content.Replace("\r", Environment.NewLine);
                    addItem.Content.Text = content;
                    addItem.Content.UndoStack.ClearAll();
                    index = contentEnd;
                }

                addItems.Push(addItem);

                if (empty)
                {
                    break;
                }
            }

            this.AddNodes(this.SelectedNode.ChildrenSource, this.SelectedNode.Children.Count, addItems);
        }

        /// <summary>
        /// ノードを複数追加する
        /// </summary>
        private void AddNodes(IList<Node> tree, int index, IEnumerable<Node> addItems)
        {
            // 行う操作
            object[] doParam = new object[] { tree, index, addItems };
            var doAction = new Action<IList<Node>, int, IEnumerable<Node>>((_tree, _index, _addItems) =>
            {
                foreach (var item in _addItems)
                {
                    this.AddTransaction(_tree, _index, item);
                }
            });

            // 取り消す操作
            object[] undoParam = new object[] { tree, index, addItems };
            var undoAction = new Action<IList<Node>, int, IEnumerable<Node>>((_tree, _index, _addItems) =>
            {
                foreach (var item in _addItems)
                {
                    this.DeleteTransaction(_tree, item);
                }

            });

            // 操作実行
            this.UndoRedoModel.Do(doAction, doParam, undoAction, undoParam);
        }

        #endregion NodeTransaction

        #region DragDrop

        /// <summary>
        /// ノードのドラッグオーバー時の処理
        /// </summary>
        public void DragOverAction(Node target, Node dropped)
        {
            if (target == null)
            {
                this.Tree.Last().DropPosition = Enums.DropPositionEnum.Next;
                this.Tree.Last().IsDragOver = true;
            }
        }

        /// <summary>
        /// ノードのドラッグエンター時の処理
        /// </summary>
        /// <param name="target"></param>
        public void DragEnterAction(Node target, Node dropped)
        {
            this.WorkAllNode(c => c.IsDragOver = false);

            // ノード名を編集中のときは移動しない
            if (dropped != null && dropped.IsNameEditMode)
            {
                return;
            }

            // 移動先が自分自身の子の場合は移動しない
            if (dropped != null && this.Find(dropped.Children, c => c.ID == target?.ID) != null)
            {
                return;
            }

            if (target != null)
            {
                if (target.ID == dropped?.ID)
                {
                    return;
                }
                else
                {
                    target.IsDragOver = true;
                    return;
                }
            }
            else
            {
                this.Tree.Last().DropPosition = Enums.DropPositionEnum.Next;
                this.Tree.Last().IsDragOver = true;
            }
        }

        /// <summary>
        /// ノードのドラッグ離脱時の処理
        /// </summary>
        /// <param name="target"></param>
        public void DragLeaveAction(Node target)
        {
            this.WorkAllNode(c => c.IsDragOver = false);
        }

        /// <summary>
        /// ノードのドロップ時の処理
        /// </summary>
        public void DragDropAction(System.Windows.DragEventArgs arg, Node target, Node dropped, bool isPressCtrlKey)
        {
            target = this.Find(this.TreeSource, c => c.IsDragOver);
            this.WorkAllNode(c => c.IsDragOver = false);

            if (dropped != null)
            {
                // ノードのドロップ
                if (isPressCtrlKey)
                {
                    this.NodeDropCopy(target, dropped);
                    return;
                }
                else
                {
                    this.NodeDropMove(target, dropped);
                    return;
                }
            }
            else
            {
                // ファイルのドロップ
                string[] files = arg.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

                if (files != null)
                {
                    this.FileDrop(target, files);
                    return;
                }
            }
        }

        /// <summary>
        /// ノードのドロップ時の処理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dropped"></param>
        private void NodeDropMove(Node target, Node dropped)
        {
            if (dropped == null || dropped.IsNameEditMode || target == null)
            {
                return;
            }

            IList<Node> fromTree;
            int fromIndex;
            IList<Node> toTree;
            int toIndex;

            if (!this.GetDragDropInfo(target, dropped, out fromTree, out fromIndex, out toTree, out toIndex))
            {
                return;
            }

            this.Move(fromTree, fromIndex, toTree, toIndex, dropped, target);
        }

        /// <summary>
        /// ノードのドロップコピー時の処理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dropped"></param>
        private void NodeDropCopy(Node target, Node dropped)
        {
            if (dropped == null || dropped.IsNameEditMode || target == null)
            {
                return;
            }

            IList<Node> fromTree;
            int fromIndex;
            IList<Node> toTree;
            int toIndex;

            if (!this.GetDragDropInfo(target, dropped, out fromTree, out fromIndex, out toTree, out toIndex))
            {
                return;
            }

            this.AddNode(toTree, toIndex, new Node(dropped));
        }

        /// <summary>
        /// ドラッグドロップの情報を取得します
        /// </summary>
        /// <param name="target">ドロップ先</param>
        /// <param name="dropped">ドロップしたNode</param>
        /// <param name="fromTree"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toTree"></param>
        /// <param name="toIndex"></param>
        /// <returns>移動に必要な情報の取得に成功したらtrue</returns>
        private bool GetDragDropInfo(Node target, Node dropped,
                                     out IList<Node> fromTree, out int fromIndex, out IList<Node> toTree, out int toIndex)
        {
            fromTree = default(IList<Node>);
            fromIndex = default(int);
            toTree = default(IList<Node>);
            toIndex = default(int);

            // 移動元
            {
                fromTree = this.GetParent(dropped)?.ChildrenSource;
                if (fromTree == null)
                {
                    fromTree = this.TreeSource;
                }
                fromIndex = fromTree.IndexOf(dropped);
            }

            // 移動先
            {
                switch (target.DropPosition)
                {
                    case Enums.DropPositionEnum.Prev:
                    case Enums.DropPositionEnum.Next:
                        toTree = this.GetParent(target)?.ChildrenSource;
                        if (toTree == null)
                        {
                            toTree = this.TreeSource;
                        }
                        toIndex = toTree.IndexOf(target);
                        if (target.DropPosition == Enums.DropPositionEnum.Next)
                        {
                            ++toIndex;
                        }
                        break;
                    case Enums.DropPositionEnum.Child:
                        toTree = target.ChildrenSource;
                        toIndex = target.ChildrenSource.Count();
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ファイルを読み込んでノードに展開します
        /// </summary>
        private void FileDrop(Node target, string[] files)
        {
            // 追加する先
            IList<Node> toTree = default(IList<Node>);
            int toIndex = default(int);
            {
                switch (target.DropPosition)
                {
                    case Enums.DropPositionEnum.Prev:
                    case Enums.DropPositionEnum.Next:
                        toTree = this.GetParent(target)?.ChildrenSource;
                        if (toTree == null)
                        {
                            toTree = this.TreeSource;
                        }
                        toIndex = toTree.IndexOf(target);
                        if (target.DropPosition == Enums.DropPositionEnum.Next)
                        {
                            ++toIndex;
                        }
                        break;
                    case Enums.DropPositionEnum.Child:
                        toTree = target.ChildrenSource;
                        toIndex = target.ChildrenSource.Count();
                        break;
                    default:
                        return;
                }
            }

            // 追加するノード
            var addItems = new Stack<Node>();
            {
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    QuartetEditorDescription model;
                    if (FileUtility.LoadJsonObject(file, out model) == true)
                    {
                        // ファイルはQuartetEditorのファイル
                        foreach (var node in model.Node)
                        {
                            var additem = new Node(node);
                            addItems.Push(additem);
                        }
                        continue;
                    }
                    else
                    {
                        // QuartetEditor以外のファイル
                        string content;
                        if (!FileUtility.LoadTextByAnyEncoding(file, out content))
                        {
                            continue;
                        }

                        // 改行コードの置き換え
                        if (content.IndexOf("\r\n") != -1)
                        {
                            content = content.Replace("\r\n", Environment.NewLine);
                        }
                        else if (content.IndexOf("\r") != -1)
                        {
                            content = content.Replace("\r", Environment.NewLine);
                        }
                        else if (content.IndexOf("\n") != -1)
                        {
                            content = content.Replace("\n", Environment.NewLine);
                        }


                        string fileName = Path.GetFileNameWithoutExtension(file);

                        var node = new Node();
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            node.Name = fileName;
                        }

                        node.Content.Text = content;
                        node.Content.UndoStack.ClearAll();

                        addItems.Push(node);
                        continue;
                    }
                }
            }

            this.AddNodes(toTree, toIndex, addItems);
        }

        #endregion DragDrop

        #region NodeSearch

        /// <summary>
        /// ノードの検索
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Node Find(IList<Node> list, Predicate<Node> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return item;
                }
                else
                {
                    if (item.Children.Count > 0)
                    {
                        var ret = this.Find(item.Children, predicate);
                        if (ret != null)
                        {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 指定されたNodeの姉を取得します
        /// 見つからない場合は親類を探します
        /// それでも見つからない場合はnullを返します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetPrev(Node item)
        {
            if (item == null)
            {
                return null;
            }

            bool hasParent = true;
            var list = this.GetParent(item)?.Children;
            if (list == null)
            {
                hasParent = false;
                list = this.Tree;
            }
            int index = list.IndexOf(item);
            if (index == 0 && hasParent)
            {
                // 姉妹の先頭の場合
                return this.GetCousinLast(item, 3);
            }
            else
            {
                return list.ElementAtOrDefault(index - 1);
            }
        }

        /// <summary>
        /// 指定されたNodeの姉（上の方にある）を取得します
        /// 見つからない場合はnullを返します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetOlder(Node item)
        {
            if (item == null)
            {
                return null;
            }

            var list = this.GetParent(item)?.Children;
            if (list == null)
            {
                list = this.Tree;
            }
            int index = list.IndexOf(item);

            return list.ElementAtOrDefault(index - 1);

        }

        /// <summary>
        /// 祖先をたどり、親類（上）のノードを取得します
        /// </summary>
        /// <param name="item">基準となるノード</param>
        /// <param name="maxDistance">何階層まで検索するか</param>
        /// <returns>見つかったノード（見つからなかったらnull）</returns>
        private Node GetCousinLast(Node item, int maxDistance)
        {
            for (int i = 0; i < maxDistance; i++)
            {
                var ancestor = this.GetOlder(this.FollowParent(item, i));

                if (ancestor != null)
                {
                    return this.FollowLastChild(ancestor, i);
                }
                else
                {
                    continue;
                }
            }
            return null;
        }

        /// <summary>
        /// 祖先をたどります
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Node FollowParent(Node item, int count)
        {
            if (item == null)
            {
                return null;
            }

            var parent = this.GetParent(item);
            if (parent == null)
            {
                return null;
            }

            if (count == 0)
            {
                return parent;
            }
            else
            {
                return this.FollowParent(parent, --count);
            }
        }

        /// <summary>
        /// 子孫をたどります（末っ子）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Node FollowLastChild(Node item, int count)
        {
            if (item == null)
            {
                return null;
            }

            if (item.Children.Count == 0)
            {
                return item;
            }

            if (count == 0)
            {
                return item.Children.Last();
            }
            else
            {
                return this.FollowLastChild(item.Children.Last(), count < 0 ? -1 : --count);
            }
        }

        /// <summary>
        /// 子孫をたどります（長女）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Node FollowFirstChild(Node item, int count)
        {
            if (item == null)
            {
                return null;
            }
            if (item.Children.Count == 0)
            {
                return item;
            }

            if (count == 0)
            {
                return item.Children.First();
            }
            else
            {
                return this.FollowFirstChild(item.Children.First(), --count);
            }
        }

        /// <summary>
        /// 指定されたNodeの妹を取得します
        /// 見つからない場合は親類を探します
        /// それでも見つからない場合はnullを返します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetNext(Node item)
        {
            if (item == null)
            {
                return null;
            }

            bool hasParent = true;
            var list = this.GetParent(item)?.Children;
            if (list == null)
            {
                hasParent = false;
                list = this.Tree;
            }
            int index = list.IndexOf(item);
            if (index + 1 == list.Count && hasParent)
            {
                // 姉妹の末尾の場合
                return this.GetCousinFirst(item, 3);
            }
            else
            {
                return list.ElementAtOrDefault(index + 1);
            }
        }

        /// <summary>
        /// 指定されたNodeの妹（下の方にある）を取得します
        /// 見つからない場合はnullを返します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetYounger(Node item)
        {
            if (item == null)
            {
                return null;
            }

            var list = this.GetParent(item)?.Children;
            if (list == null)
            {
                list = this.Tree;
            }
            int index = list.IndexOf(item);
            return list.ElementAtOrDefault(index + 1);
        }

        /// <summary>
        /// 祖先をたどり、親類（下）のノードを取得します
        /// </summary>
        /// <param name="item">基準となるノード</param>
        /// <param name="maxDistance">何階層まで検索するか</param>
        /// <returns>見つかったノード（見つからなかったらnull）</returns>
        private Node GetCousinFirst(Node item, int maxDistance)
        {
            for (int i = 0; i < maxDistance; i++)
            {
                var ancestor = this.GetYounger(this.FollowParent(item, i));

                if (ancestor != null)
                {
                    return this.FollowFirstChild(ancestor, i);
                }
                else
                {
                    continue;
                }
            }
            return null;
        }

        /// <summary>
        /// 指定されたNodeの親を取得します
        /// 見つからない場合はnullを返します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetParent(Node item)
        {
            if (item == null)
            {
                return null;
            }
            return this.Find(this.Tree, c => c.Children.Any(child => child.ID == item.ID));
        }

        /// <summary>
        /// １つ上のノードを取得します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetUp(Node item)
        {
            if (item == null)
            {
                return null;
            }

            var older = this.GetOlder(item);

            if (older != null)
            {
                return this.FollowLastChild(older, -1);
            }

            var parent = this.GetParent(item);

            if (parent == null)
            {
                return null;
            }

            return parent;
        }

        /// <summary>
        /// １つ下のノードを取得します
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetDown(Node item)
        {
            if (item == null)
            {
                return null;
            }

            var child = item.Children.FirstOrDefault();
            if (child != null)
            {
                return child;
            }

            var younger = this.GetYounger(item);

            if (younger != null)
            {
                return younger;
            }

            var parent = this.GetParent(item);

            while (parent != null)
            {
                var parentYounger = this.GetYounger(parent);
                if (parentYounger != null)
                {
                    return parentYounger;
                }
                parent = this.GetParent(parent);
            }

            return null;
        }


        #endregion  NodeSearch

        #region UndoRedo

        /// <summary>
        /// UndoRedo管理
        /// </summary>
        private UndoRedoManager UndoRedoModel { get; } = new UndoRedoManager();

        /// <summary>
        /// 元に戻す
        /// </summary>
        public void Undo()
        {
            this.UndoRedoModel.Undo();
        }

        /// <summary>
        /// 「元に戻す」実行可否
        /// </summary>
        public IObservable<bool> ChangedCanUndo => _ChangedCanUndo;

        private Subject<bool> _ChangedCanUndo = new Subject<bool>();

        /// <summary>
        /// やり直す
        /// </summary>
        public void Redo()
        {
            this.UndoRedoModel.Redo();
        }

        /// <summary>
        /// 「やり直す」実行可否
        /// </summary>
        public IObservable<bool> ChangedCanRedo => _ChangedCanRedo;

        private Subject<bool> _ChangedCanRedo = new Subject<bool>();

        #endregion UndoRedo

        #region Load

        /// <summary>
        /// QuartetEditorDescriptionを設定します
        /// </summary>
        /// <param name="model"></param>
        public void Load(QuartetEditorDescription model)
        {
            this.TreeSource.Clear();
            this.UndoRedoModel.Clear();
            foreach (var item in model.Node)
            {
                this.TreeSource.Add(new Node(item));
            }

            this.Tree.First().IsSelected = true;
            this.OffEditFlag();
        }

        #endregion Load

        #region FindAndReplace

        /// <summary>
        /// ドキュメント全体から文字を検索します
        /// </summary>
        public SearchResult Find(Regex regex, int start, Node node, Node startNode = null, bool titleFirst = false)
        {
            if (titleFirst)
            {
                var titlematch = node.FindFromNodeName(regex);

                if (titlematch != null)
                {
                    return titlematch;
                }
            }

            var result = node.Find(regex, start, false, false);

            if (result != null)
            {
                return result;
            }
            else
            {
                var next = regex.Options.HasFlag(RegexOptions.RightToLeft) ?
                    this.GetUp(node) ?? this.FollowLastChild(node, -1) :
                    this.GetDown(node) ?? this.Tree.First();

                if (next != null)
                {
                    var titlematch = next.FindFromNodeName(regex);

                    if (titlematch != null)
                    {
                        return titlematch;
                    }

                    if (node.ID == startNode?.ID)
                    {
                        return null;
                    }

                    return this.Find(
                        regex,
                        regex.Options.HasFlag(RegexOptions.RightToLeft) ? next.Content.Text.Length : 0,
                        next,
                        startNode ?? node);
                }
            }

            return null;
        }

        /// <summary>
        /// 全ノードのノード名、コンテンツすべてのテキストを置換します
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="textToReplace"></param>
        public void ReplaceAllTextWholeAllNode(Regex regex, string textToReplace)
        {
            this.WorkAllNode(node =>
            {
                node.ReplaceAll(regex, textToReplace);
                node.ReplaceNodeName(regex, textToReplace);

            });
        }

        #endregion FindAndReplace

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
            foreach (var item in this.TreeSource)
            {
                item.Dispose();
            }
        }
    }
}
