using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Entities
{
    public class PanelStateEntity
    {
        /// <summary>
        /// 左パネルの開閉状態
        /// </summary>
        public bool LeftPanelOpen { set; get; }

        /// <summary>
        /// 上パネルの開閉状態
        /// </summary>
        public bool TopPanelOpen { set; get; }

        /// <summary>
        /// 下パネルの開閉状態
        /// </summary>
        public bool BottomPanelOpen { set; get; }
    }
}
