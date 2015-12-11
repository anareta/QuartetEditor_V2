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
        /// コンストラクタ
        /// </summary>
        private NodeManager()
        {
            // 接続
            this.Tree = new ReadOnlyObservableCollection<Node>(this.TreeSource);
        }
    }
}
