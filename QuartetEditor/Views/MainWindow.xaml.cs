using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuartetEditor.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region PanelState
        /// <summary>
        /// 最後にパネルが開いていたときの大きさ
        /// </summary>
        public GridLength LeftPanelSize { get; set; }

        /// <summary>
        /// パネルが開いているか
        /// </summary>
        public bool LeftPanelOpen
        {
            get
            {
                return this._LeftTextBox.Visibility == Visibility.Visible;
            }
        }

        /// <summary>
        /// 最後にパネルが開いていたときの大きさ
        /// </summary>
        public GridLength TopPanelSize { get; set; }

        /// <summary>
        /// パネルが開いているか
        /// </summary>
        public bool TopPanelOpen
        {
            get
            {
                return this._TopTextBox.Visibility == Visibility.Visible;
            }
        }

        /// <summary>
        /// 最後にパネルが開いていたときの大きさ
        /// </summary>
        public GridLength BottomPanelSize { get; set; }

        /// <summary>
        /// パネルが開いているか
        /// </summary>
        public bool BottomPanelOpen
        {
            get
            {
                return this._BottomTextBox.Visibility == Visibility.Visible;
            }
        }
        #endregion PanelState

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
