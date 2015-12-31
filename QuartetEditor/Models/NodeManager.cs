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
            var item = new Node("ノード１") { IsSelected = true };
            item.AddChild().Name = "ノード1-1";
            item.AddChild().Name = "ノード1-2";
            this.TreeSource.Add(item);
            this.TreeSource.Add(new Node("ノード２"));
            this.TreeSource.Add(new Node("ノード３"));
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
        /// ノードのドラッグオーバー時の処理
        /// </summary>
        public void DragOverAction(Node target)
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
        public void DragEnterAction(Node target)
        {
            this.ResetDragOverFlag();
            if (target != null)
            {
                target.IsDragOver = true;
            }
            if (target == null)
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
            this.ResetDragOverFlag();
        }

        /// <summary>
        /// ノードのドロップ時の処理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dropped"></param>
        public void DragDropAction(Node target, Node dropped)
        {
            this.ResetDragOverFlag();

            if (dropped == null || dropped.IsNameEditMode)
            {
                return;
            }

            // TODO:ドロップ処理

        }

        /// <summary>
        /// IsDragOverフラグをすべてfalseにします
        /// </summary>
        public void ResetDragOverFlag()
        {
            this.Tree.ForEach(node => node.ResetDragOverFlag());
        }
    }
}
