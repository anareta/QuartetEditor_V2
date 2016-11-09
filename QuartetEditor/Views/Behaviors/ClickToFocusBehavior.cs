using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Behaviors
{
    /// <summary>
    /// クリックされたときにフォーカスを設定します
    /// </summary>
    public sealed class ClickToFocusBehavior : Behavior<Control>
    {
        /// <summary>
        /// アタッチ
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject == null)
            {
                return;
            }

            Observable.FromEvent<MouseButtonEventHandler, MouseEventArgs>(
            h => (s, e) => h(e),
            h => this.AssociatedObject.MouseLeftButtonDown += h,
            h => this.AssociatedObject.MouseLeftButtonDown -= h)
            .Subscribe(this.OnMouseLeftButtonDown);
        }

        /// <summary>
        /// マウス押下時の動作
        /// </summary>
        /// <param name="e"></param>
        void OnMouseLeftButtonDown(MouseEventArgs e)
        {
            this.AssociatedObject.Focus();
        }


        /// <summary>
        /// デタッチ
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
