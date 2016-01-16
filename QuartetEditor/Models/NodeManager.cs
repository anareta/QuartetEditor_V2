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

namespace QuartetEditor.Models
{
    /// <summary>
    ///データモデルクラス
    /// </summary>
    class NodeManager : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// ユーザーへのエラー通知要求
        /// </summary>
        public Subject<string> ShowErrorMessageRequest { get; } = new Subject<string>();

        /// <summary>
        /// システムのデータ
        /// </summary>
        public static NodeManager Current { get; } = new NodeManager();

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
        private void UpdatePanelReffer()
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
        /// コンストラクタ
        /// </summary>
        private NodeManager()
        {
            // 接続
            this.Tree = new ReadOnlyObservableCollection<Node>(this.TreeSource);

            // プロパティの変更を監視する
            this.TreeSource.ObserveElementProperty(x => x.IsEdited).Where(x => x.Value)
                .Merge(this.TreeSource.ObserveElementProperty(x => x.ChildrenEdited).Where(x => x.Value))
                .Subscribe(x =>
                {
                    this.IsEdited = true;
                }).AddTo(this.Disposable);

            this.TreeSource.CollectionChangedAsObservable()
                .Subscribe(x => this.IsEdited = true)
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

                this.CanMoveUp.OnNext(this.GetOlder(this.SelectedNode) != null);
                this.CanMoveDown.OnNext(this.GetYounger(this.SelectedNode) != null);
            }).AddTo(this.Disposable);

            new[] { ConfigManager.Current.Config.ObserveProperty(c => c.LeftPanelOpen),
                    ConfigManager.Current.Config.ObserveProperty(c => c.TopPanelOpen),
                    ConfigManager.Current.Config.ObserveProperty(c => c.BottomPanelOpen)}
            .Merge()
            .Subscribe(_ => this.UpdatePanelReffer())
            .AddTo(this.Disposable);

            #endregion ViewState

            #region UndoRedo
            this.UndoRedoModel.CanRedoChange.Subscribe(b => this.CanRedo.OnNext(b));
            this.UndoRedoModel.CanUndoChange.Subscribe(b => this.CanUndo.OnNext(b));
            #endregion UndoRedo


#if DEBUG
            {
                var item = new Node("ノード1") { IsSelected = true };
                item.ChildrenSource.Add(new Node("ノード1-1"));
                item.ChildrenSource.Add(new Node("ノード1-2"));
                item.ChildrenSource.Last().ChildrenSource.Add(new Node("ノード1-2-1"));
                item.ChildrenSource.Last().ChildrenSource.Add(new Node("ノード1-2-2"));
                this.TreeSource.Add(item);
            }
            {
                var item = new Node("ノード2");
                item.ChildrenSource.Add(new Node("ノード2-1"));
                item.ChildrenSource.Last().ChildrenSource.Add(new Node("ノード2-1-1"));
                item.ChildrenSource.Last().ChildrenSource.Add(new Node("ノード2-1-2"));
                item.ChildrenSource.Add(new Node("ノード2-2"));
                item.ChildrenSource.Last().ChildrenSource.Add(new Node("ノード2-2-1"));
                item.ChildrenSource.Last().ChildrenSource.Add(new Node("ノード2-2-2"));
                this.TreeSource.Add(item);
            }

            this.TreeSource.Add(new Node("ノード3"));
#endif
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
        private void OffEditFlag()
        {
            this.WorkAllNode(n => n.IsEdited = false);
            this.WorkAllNode(n => n.ChildrenEdited = false);
            this.IsEdited = false;
        }

        /// <summary>
        /// すべてのノードに対しての操作を提供します
        /// </summary>
        /// <param name="act"></param>
        private void WorkAllNode(Action<Node> act)
        {
            this.Tree.ForEach(node => node.WorkAllNode(act));
        }

        #region NodeTransaction

        /// <summary>
        /// ノードを削除する
        /// </summary>
        public void DeleteNode()
        {
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
                _tree.Insert(_index, _item);
                _tree.ElementAt(_index).IsSelected = true;
            });

            // 操作実行
            this.UndoRedoModel.Do(doAction, doParam, undoAction, undoParam);
        }

        /// <summary>
        /// ノードを同階層に追加する
        /// </summary>
        public void AddNodeSame()
        {
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
            object[] doParam = new object[] { tree, index, newItem };
            var doAction = new Action<IList<Node>, int, Node>((_tree, _index, _newItem) =>
            {
                this.AddTransaction(_tree, _index, _newItem);
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
        }

        /// <summary>
        /// ノードを削除する
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index"></param>
        private void DeleteTransaction(IList<Node> tree, int index)
        {
            var deleteItem = tree.ElementAt(index);
            tree.RemoveAt(index);
        }

        /// <summary>
        /// ノードを下に移動する
        /// </summary>
        public void MoveDown()
        {
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
        public Subject<bool> CanMoveDown { get; } = new Subject<bool>();

        /// <summary>
        /// ノードを上に移動する
        /// </summary>
        public void MoveUp()
        {
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
        public Subject<bool> CanMoveUp { get; } = new Subject<bool>();

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
            // 移動元から削除
            fromTree.RemoveAt(fromIndex);

            // 移動先に挿入
            toTree.Insert(toIndex, item);

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
            if (dropped == null || dropped.IsNameEditMode)
            {
                return;
            }

            // 移動先が自分自身の子の場合は移動しない
            if (this.Find(dropped.Children, c => c.ID == target?.ID) != null)
            {
                return;
            }

            if (target != null)
            {
                if (target.ID == dropped.ID)
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
        /// <param name="target"></param>
        /// <param name="dropped"></param>
        public void DragDropAction(Node target, Node dropped)
        {
            target = this.Find(this.TreeSource, c => c.IsDragOver);

            this.WorkAllNode(c => c.IsDragOver = false);

            if (dropped == null || dropped.IsNameEditMode || target == null)
            {
                return;
            }

            IList<Node> fromTree;
            int fromIndex;
            IList<Node> toTree;
            int toIndex;

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
                        return;
                }
            }

            this.Move(fromTree, fromIndex, toTree, toIndex, dropped, target);
            dropped.IsSelected = true;
        }

        #endregion DragDrop

        #region Search

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
                return this.GetCousinLast(item);
            }
            else
            {
                return list.ElementAtOrDefault(index - 1);
            }
        }

        /// <summary>
        /// 指定されたNodeの姉を取得します
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
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetCousinLast(Node item)
        {
            for (int i = 0; i < 3; i++)
            {
                var ancestor = this.GetYounger(this.FollowParent(item, i));

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
                return this.FollowLastChild(item.Children.Last(), --count);
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
                return this.GetCousinFirst(item);
            }
            else
            {
                return list.ElementAtOrDefault(index + 1);
            }
        }

        /// <summary>
        /// 指定されたNodeの妹を取得します
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
        /// <param name="item"></param>
        /// <returns></returns>
        private Node GetCousinFirst(Node item)
        {
            for (int i = 0; i < 3; i++)
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

        #endregion  Search

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
        public Subject<bool> CanUndo { get; } = new Subject<bool>();

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
        public Subject<bool> CanRedo { get; } = new Subject<bool>();

        #endregion UndoRedo

        #region File

        /// <summary>
        /// ViewへのSavePath処理要求
        /// </summary>
        public Subject<Action<string>> SavePathRequest { get; } = new Subject<Action<string>>();

        /// <summary>
        /// ViewへのOpenPath処理要求
        /// </summary>
        public Subject<Action<string>> OpenPathRequest { get; } = new Subject<Action<string>>();

        /// <summary>
        /// 編集されたか
        /// </summary>
        private bool _IsEdited = false;

        public bool IsEdited
        {
            get { return this._IsEdited; }
            set { this.SetProperty(ref this._IsEdited, value); }
        }

        /// <summary>
        /// ファイル名
        /// </summary>
        private string _FileName;

        public string FileName
        {
            get { return this._FileName; }
            set { this.SetProperty(ref this._FileName, value); }
        }

        /// <summary>
        /// ファイルを上書き保存する
        /// </summary>
        /// <returns></returns>
        public bool SaveOverwrite()
        {
            if (this.FileName == null ||
                string.IsNullOrWhiteSpace(this.FileName) ||
                !File.Exists(this.FileName))
            {
                // 有効なファイル名が存在しない場合は変名処理へ
                return this.RenameSave();
            }

            if (this.Save(this.FileName))
            {
                this.OffEditFlag();
                return true;
            }

            return false;
        }

        /// <summary>
        /// ファイルを変名保存する
        /// </summary>
        /// <returns></returns>
        public bool RenameSave()
        {
            bool result = false;
            this.SavePathRequest.OnNext(path =>
            {
                if (path != null && !string.IsNullOrWhiteSpace(path))
                {
                    if (this.Save(path))
                    {
                        this.FileName = path;
                        this.OffEditFlag();
                        result = true;
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// ファイルを保存する
        /// </summary>
        /// <param name="path"></param>
        private bool Save(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                var data = new QuartetEditorDescription(this.TreeSource);
                FileUtility.SaveJsonObject(path, data);
                return true;
            }
            catch
            {
                this.ShowErrorMessageRequest.OnNext("ファイルの保存に失敗しました。");
            }
            return false;
        }

        /// <summary>
        /// ファイルを開いて読み込む
        /// </summary>
        /// <returns></returns>
        public void OpenQED()
        {
            this.OpenPathRequest.OnNext(path =>
            {
                this.Load(path);
            });
        }

        /// <summary>
        /// ファイルの読み込みを行います
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (File.Exists(path))
            {
                if (this.IsEdited)
                {
                    this.OpenNewProcess(path);
                    return;
                }

                bool fail = false;
                try
                {
                    QuartetEditorDescription model;
                    if (FileUtility.LoadJsonObject(path, out model) == false)
                    {
                        fail = true;
                    }
                    else
                    {
                        this.TreeSource.Clear();
                        this.UndoRedoModel.Clear();
                        foreach (var item in model.Node)
                        {
                            this.TreeSource.Add(new Node(item));
                        }
                        this.FileName = path;
                        this.Tree.First().IsSelected = true;
                        this.OffEditFlag();
                    }
                }
                catch
                {
                    fail = true;
                }

                if (fail)
                {
                    this.ShowErrorMessageRequest.OnNext("ファイルの読み込みに失敗しました。");
                }
            }
        }

        /// <summary>
        /// 新しいプロセスでファイルを開く
        /// </summary>
        /// <param name="path"></param>
        private void OpenNewProcess(string path)
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location, "\"" + path + "\"");
        }

        #endregion File

        #region Export

        /// <summary>
        /// Viewへのエクスポート先SavePath処理要求
        /// </summary>
        public Subject<Action<string, ExportKindEnum>> ExportSavePathRequest { get; } = new Subject<Action<string, ExportKindEnum>>();

        /// <summary>
        /// エクスポート
        /// </summary>
        public void Export()
        {
            bool fail = false;
            this.ExportSavePathRequest.OnNext((path, kind) =>
            {
                if (path != null && !string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        switch (kind)
                        {
                            case ExportKindEnum.Text:
                                string exportstr = NodeConverterUtility.ToText(new QuartetEditorDescription(this.TreeSource));
                                fail = !FileUtility.SaveText(path, exportstr, Encoding.UTF8);
                                return;
                            case ExportKindEnum.HTML:
                                return;
                            case ExportKindEnum.Directory:
                                return;
                            default:
                                return;
                        }
                    }
                    catch (Exception)
                    {
                        fail = true;
                    }
                }
            });

            if (fail)
            {
                this.ShowErrorMessageRequest.OnNext("エクスポートに失敗しました…");
            }
        }

        #endregion Export

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
