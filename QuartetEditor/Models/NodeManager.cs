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

namespace QuartetEditor.Models
{
    /// <summary>
    ///データモデルクラス
    /// </summary>
    class NodeManager
    {
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

        /// <summary>
        /// 編集されたか
        /// </summary>
        public bool IsEdited { get; private set; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private NodeManager()
        {
            // 接続
            this.Tree = new ReadOnlyObservableCollection<Node>(this.TreeSource);

            // プロパティの変更を監視する
            this.TreeSource.ObserveElementProperty(x => x.IsEdited)
                .Subscribe(x =>
                {
                    if (x.Instance.IsEdited)
                    {
                        this.IsEdited = true;
                    }
                });

            this.TreeSource.CollectionChangedAsObservable()
                .Subscribe(x => this.IsEdited = true);
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
                this.AddNode().IsSelected = true;
            }

            // ノードがゼロになったら新規ノードを追加する
            var collectionChange = Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            h => (s, e) => h(e),
            h => this.TreeSource.CollectionChanged += h,
            h => this.TreeSource.CollectionChanged -= h);
            collectionChange.Subscribe((_)=>
            {
                if (this.TreeSource.Count == 0)
                {
                    this.AddNode().IsSelected = true;
                }
            });
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
        /// ノードを末尾に追加する
        /// </summary>
        /// <returns>追加したノードへの参照</returns>
        public Node AddNode()
        {
            var item = new Node();
            this.TreeSource.Add(item);
            return item;
        }

        /// <summary>
        /// ノードの移動処理
        /// </summary>
        /// <param name="item"></param>
        /// <param name="target"></param>
        private void Move(Node item, Node target)
        {
            // 移動元から削除
            {
                IList<Node> parentTree = this.GetParent(item)?.ChildrenSource;
                if (parentTree == null)
                {
                    parentTree = this.TreeSource;
                }
                parentTree.Remove(item);
            }

            // 移動先に追加
            {
                switch (target.DropPosition)
                {
                    case Enums.DropPositionEnum.Prev:
                    case Enums.DropPositionEnum.Next:
                        IList<Node> parentTree = this.GetParent(target)?.ChildrenSource;
                        if (parentTree == null)
                        {
                            parentTree = this.TreeSource;
                        }
                        int targetIndex = parentTree.IndexOf(target);
                        if (target.DropPosition == Enums.DropPositionEnum.Next)
                        {
                            ++targetIndex;
                        }
                        parentTree.Insert(targetIndex, item);
                        break;
                    case Enums.DropPositionEnum.Child:
                        target.ChildrenSource.Add(item);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
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

            this.Move(dropped, target);
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
        public Node GetPrev(Node item)
        {
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
        public Node GetNext(Node item)
        {
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
        public Node GetParent(Node item)
        {
            return this.Find(this.Tree, c => c.Children.Any(child => child.ID == item.ID));
        }
    }
    #endregion  Search

}
