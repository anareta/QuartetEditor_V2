using Prism.Interactivity.InteractionRequest;
using QuartetEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Messengers
{
    public class PanelOpenAction : TriggerAction<MainWindow>
    {
        protected override void Invoke(object parameter)
        {
            // イベント引数とContextを取得する
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;
            var state = ctx.Content as PanelStateEntity;

            // パネル
            this.LeftPanelUpdate(state.LeftPanelOpen);
            this.TopPanelUpdate(state.TopPanelOpen);
            this.BottomPanelUpdate(state.BottomPanelOpen);

            // コールバックを呼び出す
            args.Callback();
        }

        /// <summary>
        /// 左パネルの開閉を更新
        /// </summary>
        /// <param name="state"></param>
        private void LeftPanelUpdate(bool state)
        {
            if (this.AssociatedObject.LeftPanelOpen != state)
            {
                if (state)
                {
                    // 開く
                    this.AssociatedObject._EditorGrid.ColumnDefinitions[0].Width = this.AssociatedObject.LeftPanelSize;
                    this.AssociatedObject._LeftSplitter.Visibility = System.Windows.Visibility.Visible;
                    this.AssociatedObject._LeftTextBox.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    // 閉じる
                    this.AssociatedObject.LeftPanelSize = this.AssociatedObject._EditorGrid.ColumnDefinitions[0].Width;
                    this.AssociatedObject._EditorGrid.ColumnDefinitions[0].Width = new GridLength(0);
                    this.AssociatedObject._LeftSplitter.Visibility = System.Windows.Visibility.Collapsed;
                    this.AssociatedObject._LeftTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 下パネルの開閉を更新
        /// </summary>
        /// <param name="state"></param>
        private void BottomPanelUpdate(bool state)
        {
            if (this.AssociatedObject.BottomPanelOpen != state)
            {
                if (state)
                {
                    // 開く
                    this.AssociatedObject._EditorGrid.RowDefinitions[4].Height = this.AssociatedObject.BottomPanelSize;
                    this.AssociatedObject._BottomSplitter.Visibility = System.Windows.Visibility.Visible;
                    this.AssociatedObject._BottomTextBox.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    // 閉じる
                    this.AssociatedObject.BottomPanelSize = this.AssociatedObject._EditorGrid.RowDefinitions[4].Height;
                    this.AssociatedObject._EditorGrid.RowDefinitions[4].Height = new GridLength(0);
                    this.AssociatedObject._BottomSplitter.Visibility = System.Windows.Visibility.Collapsed;
                    this.AssociatedObject._BottomTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void TopPanelUpdate(bool state)
        {
            if (this.AssociatedObject.TopPanelOpen != state)
            {
                if (state)
                {
                    // 開く
                    this.AssociatedObject._EditorGrid.RowDefinitions[0].Height = this.AssociatedObject.TopPanelSize;
                    this.AssociatedObject._TopSplitter.Visibility = System.Windows.Visibility.Visible;
                    this.AssociatedObject._TopTextBox.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    // 閉じる
                    this.AssociatedObject.TopPanelSize = this.AssociatedObject._EditorGrid.RowDefinitions[0].Height;
                    this.AssociatedObject._EditorGrid.RowDefinitions[0].Height = new GridLength(0);
                    this.AssociatedObject._TopSplitter.Visibility = System.Windows.Visibility.Collapsed;
                    this.AssociatedObject._TopTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
