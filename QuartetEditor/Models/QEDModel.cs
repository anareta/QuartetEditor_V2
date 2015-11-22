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
    class QEDModel
    {
        /// <summary>
        /// システムのデータ
        /// </summary>
        public static QEDModel Current { get; } = new QEDModel();

        /// <summary>
        /// 内部データクラス
        /// </summary>
        private ObservableCollection<NodeModel> TreeSource { get; } = new ObservableCollection<NodeModel>();

        /// <summary>
        /// 読み取り専用データツリー
        /// </summary>
        public ReadOnlyObservableCollection<NodeModel> Tree { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private QEDModel()
        {
            // 接続
            this.Tree = new ReadOnlyObservableCollection<NodeModel>(this.TreeSource);
        }
    }
}
