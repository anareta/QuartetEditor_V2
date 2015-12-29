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
        /// ノードのドラッグドロップ処理の媒介
        /// </summary>
        public DragAcceptDescription DragAcceptDescription { get; } = new DragAcceptDescription();

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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.Tree = this.Model
                .Tree
                .ToReadOnlyReactiveCollection(x => new NodeViewModel(x));


            // 仮
#if DEBUG
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

                if (target != null && target.IsDragOver)
                {
                    
                }
                else if(target == null)
                {
                    this.Tree.Last().DropPosition = Enums.DropPositionEnum.Next;
                    this.Tree.Last().IsDragOver = true;
                }
            };

            this.DragAcceptDescription.DragEnterAction += (System.Windows.DragEventArgs args) =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.ResetDragFlag();
                if (target != null)
                {
                    target.IsDragOver = true;
                }
            };

            this.DragAcceptDescription.DragLeaveAction += (System.Windows.DragEventArgs args) =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;

                this.ResetDragFlag();

            };

            this.DragAcceptDescription.DragDropAction += (System.Windows.DragEventArgs args) =>
            {
                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null)
                {
                    return;
                }
                var target = fe.DataContext as NodeViewModel;
                this.ResetDragFlag();

            };
#endif
        }

        private void ResetDragFlag()
        {
            this.Tree.ForEach(node => this.ResetDragFlag(node));
        }

        private void ResetDragFlag(NodeViewModel node)
        {
            node.IsDragOver = false;
            node.Children.ForEach(item =>
            {
                item.IsDragOver = false;
                if (item.Children.Count > 0)
                {
                    this.ResetDragFlag(item);
                }
            });
        }

    }
}
