﻿using ICSharpCode.AvalonEdit.Document;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using QuartetEditor.Entities;
using QuartetEditor.Extensions;
using QuartetEditor.Models;
using QuartetEditor.Views.DraggableTreeView.Description;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuartetEditor.ViewModels
{
    /// <summary>
    /// メインウィンドウViewModel
    /// </summary>
    class MainWindowViewModel : BindableBase
    {
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
        private bool? _LeftPanelOpen = true;

        public bool? LeftPanelOpen
        {
            get { return this._LeftPanelOpen; }
            set
            {
                this.SetProperty(ref this._LeftPanelOpen, value);
                this.RisePanelState();
            }
        }

        /// <summary>
        /// 上参照パネルの開閉状態
        /// </summary>
        private bool? _TopPanelOpen = true;

        public bool? TopPanelOpen
        {
            get { return this._TopPanelOpen; }
            set
            {
                this.SetProperty(ref this._TopPanelOpen, value);
                this.RisePanelState();
            }
        }

        /// <summary>
        /// 下参照パネルの開閉状態
        /// </summary>
        private bool? _BottomPanelOpen = true;

        public bool? BottomPanelOpen
        {
            get { return this._BottomPanelOpen; }
            set
            {
                this.SetProperty(ref this._BottomPanelOpen, value);
                this.RisePanelState();
            }
        }

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
                this.RisePanelState();
            }
        }

        /// <summary>
        /// AboutFlyout開閉コマンド
        /// </summary>
        public ReactiveCommand OpenAboutCommand { get; private set; }　= new ReactiveCommand();

        #endregion AboutFlyout

        #region ConfigFlyout

        /// <summary>
        /// ConfigFlyoutの開閉状態
        /// </summary>
        private bool _IsConfigOpen = false;

        public bool IsConfigOpen
        {
            get { return this._IsConfigOpen; }
            set
            {
                this.SetProperty(ref this._IsConfigOpen, value);
                this.RisePanelState();
            }
        }

        /// <summary>
        /// ConfigFlyout開閉コマンド
        /// </summary>
        public ReactiveCommand OpenConfigCommand { get; private set; } = new ReactiveCommand();

        #endregion ConfigFlyout

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.Tree = this.Model
                .Tree
                .ToReadOnlyReactiveCollection(x => new NodeViewModel(x));

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

            #region ConfigFlyout

            // ConfigCommand
            this.OpenConfigCommand.Subscribe(_ => this.IsConfigOpen = true);

            #endregion ConfigFlyout
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            /* パネルの開閉初期状態などはここで設定する */
            this.LeftPanelOpen = this.ConfigModel.Config.LeftPanelOpen;
            this.TopPanelOpen = this.ConfigModel.Config.TopPanelOpen;
            this.BottomPanelOpen = this.ConfigModel.Config.BottomPanelOpen;

            this.SelectedItem = this.Tree.First();
        }

    }
}
