using MahApps.Metro.Controls;
using QuartetEditor.Models;
using QuartetEditor.ViewModels;
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
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;
using QuartetEditor.Entities;

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
            // ダイアログ表示時、タイトルバーをドラッグしてウィンドウを動かせなくなるので、
            // ウィンドウ全体で左クリックで移動できるようにする
            DialogManager.DialogOpened += (s, e) =>
            {
                this.MouseLeftButtonDown += this.MouseLeftButtonDownWindowMove;
            };

            // ダイアログを閉じているときは移動させない
            DialogManager.DialogClosed += (s, e) =>
            {
                this.MouseLeftButtonDown -= this.MouseLeftButtonDownWindowMove;
            };

            this.DataContextChanged += (s, e) =>
            {
                var model = e.NewValue as MainWindowViewModel;
                model.SaveDialogViewAction = this.SaveDialog;
                model.OpenDialogViewAction = this.OpenDialog;
                model.MessageDialogViewAction = this.ShowDialogEventListener;
            };

            InitializeComponent();
        }

        /// <summary>
        /// マウス左ボタン押下時にウィンドウをドラッグ移動させます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftButtonDownWindowMove(object sender, MouseButtonEventArgs e)
        {
            //マウスボタン押下状態でなければ何もしない
            if (e.ButtonState != MouseButtonState.Pressed)
            {
                return;
            }
            this.DragMove();
        }

        /// <summary>
        /// ダイアログ表示要求を処理する
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task<MessageDialogResult> ShowDialogEventListener(DialogArg e)
        {
            var result = await this.ShowMessageAsync(e.Title,
                                                    e.Message,
                                                    e.Style,
                                                    e.Settings);
            this.DragMove();
            return result;
        }

        /// <summary>
        /// ファイル開くダイアログを開く
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string OpenDialog(OpenFileDialog arg)
        {
            if (arg.ShowDialog() == true)
            {
                return arg.FileName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ファイル保存ダイアログを開く
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string SaveDialog(SaveFileDialog arg)
        {
            if (arg.ShowDialog() == true)
            {
                return arg.FileName;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// ウィンドウを閉じるときの処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            /* ウィンドウを閉じるかここでダイアログを出す */

            ConfigManager.Current.SaveConfig();
        }
    }
}
