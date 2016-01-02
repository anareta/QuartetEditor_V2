using ICSharpCode.AvalonEdit.Document;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using QuartetEditor.Entities;
using QuartetEditor.Extensions;
using QuartetEditor.Models;
using QuartetEditor.Views.DraggableTreeView.Description;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuartetEditor.ViewModels
{
    /// <summary>
    /// メインウィンドウViewModel
    /// </summary>
    class MainWindowViewModel : BindableBase, IDisposable
    {
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
        /// 現在選択中のノード
        /// </summary>
        public ReactiveProperty<NodeViewModel> SelectedNode { get; } = new ReactiveProperty<NodeViewModel>();

        /// <summary>
        /// 設定情報
        /// </summary>
        private ConfigManager ConfigModel { get; } = ConfigManager.Current;

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

        /// <summary>
        /// ノードのドラッグドロップ処理の媒介
        /// </summary>
        public DragAcceptDescription DragAcceptDescription { get; } = new DragAcceptDescription();

        #region Content

        /// <summary>
        /// 選択中のノード
        /// </summary>
        public NodeViewModel _SelectedItem;

        public NodeViewModel SelectedItem
        {
            get
            {
                return this._SelectedItem;
            }
            set
            {
                this.SetProperty(ref this._SelectedItem, value);
                this.OnPropertyChanged(() => this.TextContent);
            }
        }

        /// <summary>
        /// 編集中のコンテンツ
        /// </summary>
        public TextDocument TextContent
        {
            get
            {
                return this.SelectedItem == null ? new TextDocument() : this.SelectedItem.Content.Value;
            }
            set
            {
                if (this.SelectedItem != null)
                {
                    this.SelectedItem.Content.Value = value;
                    this.OnPropertyChanged(() => this.TextContent);
                }
            }
        }

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
            panelOpenRequest.Raise(new Confirmation { Content = state});
        }

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
        public ReactiveCommand OpenAboutCommand { get; private set; }　= new ReactiveCommand();

        #endregion AboutFlyout

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.Tree = this.Model
                .Tree
                .ToReadOnlyReactiveCollection(x => new NodeViewModel(x));

            #region Panel

            this.LeftPanelOpen = this.ConfigModel.Config
            .ToReactivePropertyAsSynchronized(x => x.LeftPanelOpen)
            .AddTo(this.Disposable);
            this.LeftPanelOpen.PropertyChangedAsObservable().Subscribe(_ => this.RisePanelState());

            this.TopPanelOpen = this.ConfigModel.Config
            .ToReactivePropertyAsSynchronized(x => x.TopPanelOpen)
            .AddTo(this.Disposable);
            this.TopPanelOpen.PropertyChangedAsObservable().Subscribe(_ => this.RisePanelState());

            this.BottomPanelOpen = this.ConfigModel.Config
            .ToReactivePropertyAsSynchronized(x => x.BottomPanelOpen)
            .AddTo(this.Disposable);
            this.BottomPanelOpen.PropertyChangedAsObservable().Subscribe(_ => this.RisePanelState());

            #endregion Panel

            #region DragDrop

            this.DragAcceptDescription.DragOverAction += (System.Windows.DragEventArgs args) => 
            {
                if (args.AllowedEffects.HasFlag(System.Windows.DragDropEffects.Move) &&
                args.Data.GetDataPresent(typeof(string)))
                {
                    args.Effects = System.Windows.DragDropEffects.Move;
                }

                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.Model.DragOverAction(target?.Model);
            };

            this.DragAcceptDescription.DragEnterAction += (System.Windows.DragEventArgs args) =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.Model.DragEnterAction(target?.Model);
            };

            this.DragAcceptDescription.DragLeaveAction += (System.Windows.DragEventArgs args) =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.Model.DragLeaveAction(target?.Model);
            };

            this.DragAcceptDescription.DragDropAction += (System.Windows.DragEventArgs args) =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = args.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                this.Model.DragDropAction(target?.Model, data?.Model);

            };

            #endregion DragDrop

            #region AboutFlyout

            // AboutCommand
            this.OpenAboutCommand.Subscribe( _ => this.IsAboutOpen = true );

            #endregion AboutFlyout

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
