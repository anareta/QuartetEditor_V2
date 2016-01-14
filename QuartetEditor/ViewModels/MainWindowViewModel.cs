using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using QuartetEditor.Entities;
using QuartetEditor.Enums;
using QuartetEditor.Extensions;
using QuartetEditor.Models;
using QuartetEditor.Views.DraggableTreeView.Description;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace QuartetEditor.ViewModels
{
    /// <summary>
    /// メインウィンドウViewModel
    /// </summary>
    class MainWindowViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// メッセージダイアログ表示要求
        /// </summary>
        public Func<DialogArg, Task<MessageDialogResult>> MessageDialogViewAction { get; set; }

        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// モデルクラス
        /// </summary>
        private NodeManager Model { get; } = NodeManager.Current;

        /// <summary>
        /// 木構造
        /// </summary>
        public ReadOnlyReactiveCollection<NodeViewModel> Tree { get; }

        /// <summary>
        /// ウィンドウタイトル
        /// </summary>
        public ReactiveProperty<string> WindowTitle { get; }

        /// <summary>
        /// 現在選択中のノード
        /// </summary>
        public ReactiveProperty<NodeViewModel> SelectedNode { get; }

        /// <summary>
        /// 設定情報
        /// </summary>
        private Config _Config = ConfigManager.Current.Config;

        public Config Config
        {
            get
            {
                return this._Config;
            }
            set
            {
                this.SetProperty(ref this._Config, value);
            }
        }

        /// <summary>
        /// 設定FlyoutViewModel
        /// </summary>
        public ConfigFlyoutViewModel _ConfigFlyoutViewModel = new ConfigFlyoutViewModel();

        public ConfigFlyoutViewModel ConfigFlyoutViewModel
        {
            get
            {
                return this._ConfigFlyoutViewModel;
            }
            set
            {
                this.SetProperty(ref this._ConfigFlyoutViewModel, value);
            }
        }

        #region DragDrop

        /// <summary>
        /// ノードのドラッグドロップ処理の媒介
        /// </summary>
        public DragAcceptDescription DragAcceptDescription { get; } = new DragAcceptDescription();

        #endregion DragDrop

        #region Content

        /// <summary>
        /// 編集中のコンテンツ
        /// </summary>
        public ReactiveProperty<TextDocument> TextContent { get; }

        /// <summary>
        /// 左参照パネルのコンテンツ
        /// </summary>
        public ReactiveProperty<string> ParentTextContent { get; }


        /// <summary>
        /// 上参照パネルのコンテンツ
        /// </summary>
        public ReactiveProperty<string> PrevTextContent { get; }

        /// <summary>
        /// 下参照パネルのコンテンツ
        /// </summary>
        public ReactiveProperty<string> NextTextContent { get; }

        #endregion Content

        #region PanelOpen

        /// <summary>
        /// パネルの開閉要求
        /// </summary>
        private InteractionRequest<Confirmation> panelOpenRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest PanelOpenRequest { get { return this.panelOpenRequest; } }

        /// <summary>
        /// 左参照パネルの開閉状態
        /// </summary>
        public ReactiveProperty<bool> LeftPanelOpen { get; }

        /// <summary>
        /// 上参照パネルの開閉状態
        /// </summary>
        public ReactiveProperty<bool> TopPanelOpen { get; }

        /// <summary>
        /// 下参照パネルの開閉状態
        /// </summary>
        public ReactiveProperty<bool> BottomPanelOpen { get; }

        /// <summary>
        /// パネルの開閉リクエストをViewに投げる
        /// </summary>
        private void RisePanelState()
        {
            var state = new PanelStateEntity()
            {
                LeftPanelOpen = this.LeftPanelOpen.Value,
                TopPanelOpen = this.TopPanelOpen.Value,
                BottomPanelOpen = this.BottomPanelOpen.Value,
            };
            // Viewにリクエストを投げる
            panelOpenRequest.Raise(new Confirmation { Content = state });
        }

        /// <summary>
        /// Panel開閉コマンド
        /// </summary>
        public ReactiveCommand<PanelKindEnum> PanelChangeCommand { get; private set; } = new ReactiveCommand<PanelKindEnum>();

        #endregion PanelOpen

        #region AboutFlyout

        /// <summary>
        /// AboutFlyoutの開閉状態
        /// </summary>
        private bool _IsAboutOpen = false;

        public bool IsAboutOpen
        {
            get { return this._IsAboutOpen; }
            set
            {
                this.SetProperty(ref this._IsAboutOpen, value);
            }
        }

        /// <summary>
        /// AboutFlyout開閉コマンド
        /// </summary>
        public ReactiveCommand OpenAboutCommand { get; private set; } = new ReactiveCommand();

        #endregion AboutFlyout

        #region Focus
        /// <summary>
        /// フォーカスの設定要求
        /// </summary>
        private InteractionRequest<Confirmation> setFocusRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest SetFocusRequest { get { return this.setFocusRequest; } }

        /// <summary>
        /// フォーカス設定コマンド
        /// </summary>
        public ReactiveCommand<string> SetFocusCommand { get; private set; } = new ReactiveCommand<string>();

        #endregion Focus

        #region NodeManipulation

        /// <summary>
        /// ノードの検索
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private NodeViewModel Find(IList<NodeViewModel> list, Predicate<NodeViewModel> predicate)
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
        /// 名前変更コマンド
        /// </summary>
        public ReactiveCommand NameEditCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// 元に戻すコマンド
        /// </summary>
        public ReactiveCommand UndoCommand { get; private set; }

        /// <summary>
        /// やり直すコマンド
        /// </summary>
        public ReactiveCommand RedoCommand { get; private set; }

        /// <summary>
        /// 削除コマンド
        /// </summary>
        public ReactiveCommand DeleteNodeCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// 同じ階層に追加コマンド
        /// </summary>
        public ReactiveCommand AddNodeSameCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// 下の階層に追加コマンド
        /// </summary>
        public ReactiveCommand AddNodeLowerCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// ノードを上に移動する
        /// </summary>
        public ReactiveCommand MoveUpCommand { get; private set; }

        /// <summary>
        /// ノードを下に移動する
        /// </summary>
        public ReactiveCommand MoveDownCommand { get; private set; }

        #endregion NodeManipulation

        #region File

        /// <summary>
        /// ファイルのドロップ処理の媒介
        /// </summary>
        public DragAcceptDescription DraggedFileAcceptDescription { get; } = new DragAcceptDescription();

        /// <summary>
        /// ViewへのSaveFileDialog処理要求
        /// </summary>
        public Func<SaveFileDialog, string> SaveDialogViewAction { get; set; }

        /// <summary>
        /// ViewへのOpenFileDialog処理要求
        /// </summary>
        public Func<OpenFileDialog, string> OpenDialogViewAction { get; set; }

        /// <summary>
        /// ファイル保存要求
        /// </summary>
        public ReactiveCommand SaveCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// ファイル変名保存要求
        /// </summary>
        public ReactiveCommand RenameSaveCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// ファイル読み込み要求
        /// </summary>
        public ReactiveCommand OpenCommand { get; private set; } = new ReactiveCommand();

        #endregion File

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.Tree = this.Model
                .Tree
                .ToReadOnlyReactiveCollection(x => new NodeViewModel(x));

            this.WindowTitle = this.Model.ObserveProperty(x => x.FileName)
                                   .Select(x => string.IsNullOrWhiteSpace(x) ? "" : System.IO.Path.GetFileName(x))
                                   .Select(x => "Quartet Editor" + (string.IsNullOrWhiteSpace(x) ? "" : $" - {x}"))
                                   .CombineLatest(this.Model.ObserveProperty(x => x.IsEdited), (title, flg) => title + (flg ? "（変更あり）" : ""))
                                   .ToReactiveProperty()
                                   .AddTo(this.Disposable);

            // エラーメッセージ表示要求の処理
            this.Model.ShowErrorMessageRequest.Subscribe(message =>
            {
                if (this.MessageDialogViewAction == null)
                {
                    return;
                }

                var arg = new DialogArg
                {
                    Title = "エラー",
                    Message = message,
                    Style = MessageDialogStyle.Affirmative
                };
                var task = this.MessageDialogViewAction(arg);

                return;
            });

            #region Content

            // VM -> M 一方向バインド
            this.SelectedNode = ReactiveProperty.FromObject(
                this.Model,
                x => x.SelectedNode,
                convert: x => (NodeViewModel)null, // M -> VMの変換処理
                convertBack: x => x?.Model); // VM -> Mの変換処理

            // M -> VM 一方向バインド
            this.TextContent = this.Model
                .ObserveProperty(x => x.TextContent)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.ParentTextContent = this.Model
                .ObserveProperty(x => x.ParentTextContent)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.PrevTextContent = this.Model
                .ObserveProperty(x => x.PrevTextContent)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.NextTextContent = this.Model
                .ObserveProperty(x => x.NextTextContent)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            #endregion Content

            #region Panel

            this.LeftPanelOpen = this.Config
            .ToReactivePropertyAsSynchronized(x => x.LeftPanelOpen)
            .AddTo(this.Disposable);

            this.TopPanelOpen = this.Config
            .ToReactivePropertyAsSynchronized(x => x.TopPanelOpen)
            .AddTo(this.Disposable);

            this.BottomPanelOpen = this.Config
            .ToReactivePropertyAsSynchronized(x => x.BottomPanelOpen)
            .AddTo(this.Disposable);

            // パネルの開閉状態が変わったときはRisePanelStateを呼び出す
            new[] { this.LeftPanelOpen, this.TopPanelOpen, this.BottomPanelOpen }
            .Select(x => INotifyPropertyChangedExtensions.PropertyChangedAsObservable(x))
            .Merge()
            .Subscribe(_ => this.RisePanelState())
            .AddTo(this.Disposable);

            this.PanelChangeCommand.Subscribe(kind =>
            {
                switch (kind)
                {
                    case PanelKindEnum.Left:
                        this.LeftPanelOpen.Value = !this.LeftPanelOpen.Value;
                        return;
                    case PanelKindEnum.Top:
                        this.TopPanelOpen.Value = !this.TopPanelOpen.Value;
                        return;
                    case PanelKindEnum.Bottom:
                        this.BottomPanelOpen.Value = !this.BottomPanelOpen.Value;
                        return;
                    default:
                        return;
                }
            });

            #endregion Panel

            #region DragDrop

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragOverAction += h,
            h => this.DragAcceptDescription.DragOverAction -= h).Subscribe(arg =>
            {
                var fe = arg.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = arg.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                if (data == null || data.IsNameEditMode.Value)
                {
                    return;
                }

                if (arg.AllowedEffects.HasFlag(System.Windows.DragDropEffects.Move))
                {
                    arg.Effects = System.Windows.DragDropEffects.Move;
                }

                this.Model.DragOverAction(target?.Model, data?.Model);
            }).AddTo(this.Disposable);

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragEnterAction += h,
            h => this.DragAcceptDescription.DragEnterAction -= h).Subscribe(arg =>
            {
                var fe = arg.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = arg.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                this.Model.DragEnterAction(target?.Model, data?.Model);
            }).AddTo(this.Disposable);

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragLeaveAction += h,
            h => this.DragAcceptDescription.DragLeaveAction -= h).Subscribe(arg =>
            {
                var fe = arg.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.Model.DragLeaveAction(target?.Model);
            }).AddTo(this.Disposable);

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragDropAction += h,
            h => this.DragAcceptDescription.DragDropAction -= h).Subscribe(arg =>
            {
                var fe = arg.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = arg.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                this.Model.DragDropAction(target?.Model, data?.Model);
            }).AddTo(this.Disposable);

            #endregion DragDrop

            #region AboutFlyout

            // AboutCommand
            this.OpenAboutCommand.Subscribe(_ => this.IsAboutOpen = true).AddTo(this.Disposable);

            #endregion AboutFlyout

            #region Focus
            // SetFocusCommand
            this.SetFocusCommand.Subscribe(param =>
            {
                // Viewにリクエストを投げる
                this.setFocusRequest.Raise(new Confirmation { Content = param });
            }).AddTo(this.Disposable);
            #endregion Focus

            #region NodeManipulation

            this.NameEditCommand.Subscribe(_ => this.Model.CallNameEditMode()).AddTo(this.Disposable);

            this.UndoCommand = new ReactiveCommand(this.Model.CanUndo, false);
            this.UndoCommand.Subscribe(_ =>
            {
                this.Model.Undo();
            }).AddTo(this.Disposable);

            this.RedoCommand = new ReactiveCommand(this.Model.CanRedo, false);
            this.RedoCommand.Subscribe(_ => this.Model.Redo()).AddTo(this.Disposable);

            this.DeleteNodeCommand.Subscribe(_ => this.Model.DeleteNode()).AddTo(this.Disposable);

            this.AddNodeSameCommand.Subscribe(_ => this.Model.AddNodeSame()).AddTo(this.Disposable);

            this.AddNodeLowerCommand.Subscribe(_ => this.Model.AddNodeLower()).AddTo(this.Disposable);

            this.MoveUpCommand = new ReactiveCommand(this.Model.CanMoveUp, false);
            this.MoveUpCommand.Subscribe(_ => this.Model.MoveUp()).AddTo(this.Disposable);

            this.MoveDownCommand = new ReactiveCommand(this.Model.CanMoveDown, false);
            this.MoveDownCommand.Subscribe(_ => this.Model.MoveDown()).AddTo(this.Disposable);
            #endregion NodeManipulation

            #region File

            // ファイルのドロップイベント処理
            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DraggedFileAcceptDescription.DragOverAction += h,
            h => this.DraggedFileAcceptDescription.DragOverAction -= h).Subscribe(arg =>
            {
                if (arg.Data.GetDataPresent(DataFormats.FileDrop, true))
                {
                    arg.Effects = DragDropEffects.Copy;
                    arg.Handled = true;
                }
            }).AddTo(this.Disposable); ;

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DraggedFileAcceptDescription.DragDropAction += h,
            h => this.DraggedFileAcceptDescription.DragDropAction -= h).Subscribe(arg =>
            {
                string[] files = arg.Data.GetData(DataFormats.FileDrop) as string[];

                if (files != null && files.Count() == 1)
                {
                    this.Model.Load(files[0]);
                }
            }).AddTo(this.Disposable);

            this.SaveCommand.Subscribe(_ => this.Model.SaveOverwrite()).AddTo(this.Disposable);
            this.RenameSaveCommand.Subscribe(_ => this.Model.RenameSave()).AddTo(this.Disposable);
            this.Model.SavePathRequest.Subscribe(act =>
            {
                if (this.SaveDialogViewAction == null)
                {
                    return;
                }

                var dialog = new SaveFileDialog();
                dialog.Title = "QEDファイルを保存";
                dialog.Filter = "QEDファイル(*.qed)|*.qed|全てのファイル(*.*)|*.*";
                dialog.AddExtension = true;
                dialog.DefaultExt = "qed";
                dialog.FileName = "新規";
                string path = this.SaveDialogViewAction(dialog);
                act(path);
            }).AddTo(this.Disposable);
            this.OpenCommand.Subscribe(_ => this.Model.OpenQED()).AddTo(this.Disposable);
            this.Model.OpenPathRequest.Subscribe(act =>
            {
                if (this.OpenDialogViewAction == null)
                {
                    return;
                }

                var dialog = new OpenFileDialog();
                dialog.Title = "QEDファイルを読み込み";
                dialog.Filter = "QEDファイル(*.qed)|*.qed|全てのファイル(*.*)|*.*";
                string path = this.OpenDialogViewAction(dialog);
                act(path);
            }).AddTo(this.Disposable);

            #endregion File

        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
        }

    }
}
