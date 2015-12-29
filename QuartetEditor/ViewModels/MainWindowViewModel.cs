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
    class MainWindowViewModel
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
            foreach (var node in this.Tree)
            {
                this.ResetDragFlag(node);
            }
        }

        private void ResetDragFlag(NodeViewModel node)
        {
            node.IsDragOver = false;
            foreach (var item in node.Children)
            {
                item.IsDragOver = false;
                if (item.Children.Count > 0)
                {
                    this.ResetDragFlag(item);
                }
            }
        }

    }
}
