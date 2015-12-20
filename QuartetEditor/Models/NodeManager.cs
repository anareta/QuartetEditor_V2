using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    /// <summary>
    /// QEDファイルに記述可能なデータモデルクラス
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
            this.TreeSource.ObserveElementProperty(x => x.Name)
                .Subscribe(x => this.IsEdited = true);

            this.TreeSource.ObserveElementProperty(x => x.Content)
                .Subscribe(x => this.IsEdited = true);

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
            var item = new Node("ノード１");
            item.AddChild().Name = "ノード1-1";
            item.AddChild().Name = "ノード1-2";
            this.TreeSource.Add(item);
            this.TreeSource.Add(new Node("ノード２"));
            this.TreeSource.Add(new Node("ノード３"));
#endif
        }
    }
}
