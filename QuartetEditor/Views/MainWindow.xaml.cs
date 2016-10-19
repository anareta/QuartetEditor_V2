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

            #region PanelSize

            var config = ConfigManager.Current.Config;
            this._EditorGrid.ColumnDefinitions[0].Width = new GridLength(config.LeftPanelWidth, GridUnitType.Star);
            this._EditorGrid.RowDefinitions[0].Height = new GridLength(config.TopPanelHeight, GridUnitType.Star);
            this._EditorGrid.RowDefinitions[4].Height = new GridLength(config.BottomPanelHeight, GridUnitType.Star);

            this._EditorGrid.ColumnDefinitions[2].Width = new GridLength(config.MainPanelWidth, GridUnitType.Star);
            this._EditorGrid.RowDefinitions[2].Height = new GridLength(config.MainPanelHeight, GridUnitType.Star);

            this._MainGrid.ColumnDefinitions[0].Width = new GridLength(config.NodePanelWidth, GridUnitType.Star);
            this._MainGrid.ColumnDefinitions[2].Width = new GridLength(config.LeftPanelWidth + config.MainPanelWidth, GridUnitType.Star);
            
            #endregion PanelSize
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
        /// 設定ファイルの保存中フラグ
        /// </summary>
        private bool configSaving = false;

        /// <summary>
        /// ウィンドウを閉じるときの処理
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (this.configSaving)
            {
                return;
            }

            if (this.canShutdown)
            {
                // ウィンドウ閉じる
                e.Cancel = false;
                base.OnClosing(e);
                return;
            }

            if (NodeManager.Current.IsEdited)
            {

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
            }
            else
            {
                this.canShutdown = true;
            }

            // 終了処理
            if (this.canShutdown)
            {
                this.configSaving = true;

                this.LeftPanelUpdate(true);
                this.TopPanelUpdate(true);
                this.BottomPanelUpdate(true);

                await this.Dispatcher.InvokeAsync( () =>
                {
                    {
                        var config = ConfigManager.Current.Config;
                        config.LeftPanelWidth = this._EditorGrid.ColumnDefinitions[0].ActualWidth;
                        config.TopPanelHeight = this._EditorGrid.RowDefinitions[0].ActualHeight;
                        config.BottomPanelHeight = this._EditorGrid.RowDefinitions[4].ActualHeight;

                        config.NodePanelWidth = this._MainGrid.ColumnDefinitions[0].ActualWidth;
                        config.MainPanelWidth = this._EditorGrid.ColumnDefinitions[2].ActualWidth;
                        config.MainPanelHeight = this._EditorGrid.RowDefinitions[2].ActualHeight;
                    }

                    ConfigManager.Current.SaveConfig();
                    this.configSaving = false;
                    this.Close();

                }, System.Windows.Threading.DispatcherPriority.SystemIdle);

            }
        }

        #region PanelUpdate

        /// <summary>
        /// 左パネルの開閉を更新
        /// </summary>
        /// <param name="state"></param>
        public void LeftPanelUpdate(bool state)
        {
            if (this.LeftPanelOpen != state)
            {
                if (state)
                {
                    // 開く
                    this._EditorGrid.ColumnDefinitions[0].Width = this.LeftPanelSize;
                    this._LeftSplitter.Visibility = System.Windows.Visibility.Visible;
                    this._LeftTextBox.Visibility = System.Windows.Visibility.Visible;
                    this._EditorGrid.ColumnDefinitions[0].MinWidth = 20;
                }
                else
                {
                    // 閉じる
                    this.LeftPanelSize = this._EditorGrid.ColumnDefinitions[0].Width;
                    this._EditorGrid.ColumnDefinitions[0].MinWidth = 0;
                    this._EditorGrid.ColumnDefinitions[0].Width = new GridLength(0);
                    this._LeftSplitter.Visibility = System.Windows.Visibility.Collapsed;
                    this._LeftTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 下パネルの開閉を更新
        /// </summary>
        /// <param name="state"></param>
        public void BottomPanelUpdate(bool state)
        {
            if (this.BottomPanelOpen != state)
            {
                if (state)
                {
                    // 開く
                    this._EditorGrid.RowDefinitions[4].Height = this.BottomPanelSize;
                    this._BottomSplitter.Visibility = System.Windows.Visibility.Visible;
                    this._BottomTextBox.Visibility = System.Windows.Visibility.Visible;
                    this._EditorGrid.RowDefinitions[4].MinHeight = 20;
                }
                else
                {
                    // 閉じる
                    this.BottomPanelSize = this._EditorGrid.RowDefinitions[4].Height;
                    this._EditorGrid.RowDefinitions[4].MinHeight = 0;
                    this._EditorGrid.RowDefinitions[4].Height = new GridLength(0);
                    this._BottomSplitter.Visibility = System.Windows.Visibility.Collapsed;
                    this._BottomTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 上パネルの開閉を更新
        /// </summary>
        /// <param name="state"></param>
        public void TopPanelUpdate(bool state)
        {
            if (this.TopPanelOpen != state)
            {
                if (state)
                {
                    // 開く
                    this._EditorGrid.RowDefinitions[0].Height = this.TopPanelSize;
                    this._TopSplitter.Visibility = System.Windows.Visibility.Visible;
                    this._TopTextBox.Visibility = System.Windows.Visibility.Visible;
                    this._EditorGrid.RowDefinitions[0].MinHeight = 20;

                }
                else
                {
                    // 閉じる
                    this.TopPanelSize = this._EditorGrid.RowDefinitions[0].Height;
                    this._EditorGrid.RowDefinitions[0].MinHeight = 0;
                    this._EditorGrid.RowDefinitions[0].Height = new GridLength(0);
                    this._TopSplitter.Visibility = System.Windows.Visibility.Collapsed;
                    this._TopTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion PanelUpdate

    }
}
