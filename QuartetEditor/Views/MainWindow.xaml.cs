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
using QuartetEditor.Views.Controls;

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

        #region ExportDialog

        /// <summary>
        /// エクスポートのリクエスト
        /// </summary>
        private Action<ExportSettingModel> Export { set; get; }

        /// <summary>
        /// エクスポートダイアログ
        /// </summary>
        private ExportDialog ExportDialog { set; get; }

        #endregion ExportDialog

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
                if (model != null)
                {
                    model.SaveDialogViewAction = this.SaveDialog;
                    model.OpenDialogViewAction = this.OpenDialog;
                    model.ExportDialogViewAction = this.ShowExportDialog;
                    model.MessageDialogViewAction = this.ShowDialogEventListener;
                    this.Export = model.Export;
                }
            };

            #region ExportDialog

            this.ExportDialog = new ExportDialog();
            this.ExportDialog.DataContext = new ExportDialogViewModel(() => this.HideMetroDialogAsync(this.ExportDialog));
            this.ExportDialog.Excute.Subscribe(m =>
            {
                this.MouseLeftButtonDown -= this.MouseLeftButtonDownWindowMove;
                if (m != null)
                {
                    this.Export(m);
                }
            });
            #endregion ExportDialog

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
        /// エクスポートダイアログを開く
        /// </summary>
        /// <returns></returns>
        private async Task ShowExportDialog()
        {
            this.MouseLeftButtonDown += this.MouseLeftButtonDownWindowMove;
            await this.ShowMetroDialogAsync(this.ExportDialog);
            return;
        }

        /// <summary>
        /// ウィンドウを閉じていいか
        /// </summary>
        private bool canShutdown = false;

        /// <summary>
        /// ウィンドウを閉じるときの処理
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (!NodeManager.Current.IsEdited || this.canShutdown)
            {
                ConfigManager.Current.SaveConfig();

                // ウィンドウ閉じる
                e.Cancel = false;
                base.OnClosing(e);
                return;
            }

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "保存して閉じる(_S)",
                NegativeButtonText = "保存せずに閉じる(_E)",
                FirstAuxiliaryButtonText = "閉じるのをキャンセル(_C)",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("終了します",
                                                     "ファイルが保存されていません。保存しますか？",
                                                     MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                                                     mySettings);
            canShutdown = (result != MessageDialogResult.FirstAuxiliary);

            if (this.canShutdown)
            {
                // 閉じる
                if (result == MessageDialogResult.Affirmative)
                {
                    // 保存する
                    try
                    {
                        if (!NodeManager.Current.SaveOverwrite())
                        {
                            this.canShutdown = false;
                        }
                    }
                    catch
                    {
                        this.canShutdown = false;
                    }
                }
            }

            if (this.canShutdown)
            {
                // Closingイベント中にCloseすると例外が出るので一旦Closingイベントを確実に抜けて続きをやる
                await this.Dispatcher.InvokeAsync( () => this.Close() );
            }
        }
    }
}
