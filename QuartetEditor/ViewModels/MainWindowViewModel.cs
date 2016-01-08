using ICSharpCode.AvalonEdit.Document;
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

        /// <summary>
        /// ノードのドラッグドロップ処理の媒介
        /// </summary>
        public DragAcceptDescription DragAcceptDescription { get; } = new DragAcceptDescription();

        #region Content

        /// <summary>
        /// 編集中のコンテンツ
        /// </summary>
        public ReactiveProperty<TextDocument> TextContent { get; } = new ReactiveProperty<TextDocument>();

        /// <summary>
        /// 左参照パネルのコンテンツ
        /// </summary>
        public ReactiveProperty<string> ParentTextContent { get; } = new ReactiveProperty<string>();


        /// <summary>
        /// 上参照パネルのコンテンツ
        /// </summary>
        public ReactiveProperty<string> PrevTextContent { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// 下参照パネルのコンテンツ
        /// </summary>
        public ReactiveProperty<string> NextTextContent { get; } = new ReactiveProperty<string>();

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
        public ReactiveCommand OpenAboutCommand { get; private set; }　= new ReactiveCommand();

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
        public ReactiveCommand<string> NameEditCommand { get; private set; } = new ReactiveCommand<string>();

        #endregion NodeManipulation


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.Tree = this.Model
                .Tree
                .ToReadOnlyReactiveCollection(x => new NodeViewModel(x));

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
            this.LeftPanelOpen.PropertyChangedAsObservable().Subscribe(_ =>
            {
                this.RisePanelState();
            });

            this.TopPanelOpen = this.Config
            .ToReactivePropertyAsSynchronized(x => x.TopPanelOpen)
            .AddTo(this.Disposable);
            this.TopPanelOpen.PropertyChangedAsObservable().Subscribe(_ =>
            {
                this.RisePanelState();
            });

            this.BottomPanelOpen = this.Config
            .ToReactivePropertyAsSynchronized(x => x.BottomPanelOpen)
            .AddTo(this.Disposable);
            this.BottomPanelOpen.PropertyChangedAsObservable().Subscribe(_ =>
            {
                this.RisePanelState();
            });

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
            h => this.DragAcceptDescription.DragOverAction -= h).Subscribe(args =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = args.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                if (data == null || data.IsNameEditMode.Value)
                {
                    return;
                }

                if (args.AllowedEffects.HasFlag(System.Windows.DragDropEffects.Move))
                {
                    args.Effects = System.Windows.DragDropEffects.Move;
                }

                this.Model.DragOverAction(target?.Model, data?.Model);
            });

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragEnterAction += h,
            h => this.DragAcceptDescription.DragEnterAction -= h).Subscribe(args =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = args.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                this.Model.DragEnterAction(target?.Model, data?.Model);
            });

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragLeaveAction += h,
            h => this.DragAcceptDescription.DragLeaveAction -= h).Subscribe(args =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.Model.DragLeaveAction(target?.Model);
            });

            Observable.FromEvent<Action<DragEventArgs>, DragEventArgs>(
            h => (e) => h(e),
            h => this.DragAcceptDescription.DragDropAction += h,
            h => this.DragAcceptDescription.DragDropAction -= h).Subscribe(args =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                var data = args.Data.GetData(typeof(NodeViewModel)) as NodeViewModel;

                this.Model.DragDropAction(target?.Model, data?.Model);
            });

            #endregion DragDrop

            #region AboutFlyout

            // AboutCommand
            this.OpenAboutCommand.Subscribe( _ => this.IsAboutOpen = true );

            #endregion AboutFlyout

            #region Focus
            // SetFocusCommand
            this.SetFocusCommand.Subscribe(param =>
            {
                // Viewにリクエストを投げる
                this.setFocusRequest.Raise(new Confirmation { Content = param });
            });
            #endregion Focus

            #region NodeManipulation

            this.NameEditCommand.Subscribe(_ => this.Model.CallNameEditMode() );

            #endregion NodeManipulation

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
