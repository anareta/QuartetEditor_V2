﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Reactive.Linq;

namespace QuartetEditor.Views.DraggableTreeView
{
    /// <summary>
    /// DragDropの開始処理
    /// </summary>
    public class DragStartBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Dragスタート位置
        /// </summary>
        private Point _origin;

        /// <summary>
        /// ボタン押下フラグ
        /// </summary>
        private bool _isButtonDown;

        public DragDropEffects AllowedEffects
        {
            get { return (DragDropEffects)GetValue(AllowedEffectsProperty); }
            set { SetValue(AllowedEffectsProperty, value); }
        }

        public static readonly DependencyProperty AllowedEffectsProperty =
            DependencyProperty.Register("AllowedEffects", typeof(DragDropEffects),
                    typeof(DragStartBehavior), new UIPropertyMetadata(DragDropEffects.All));

        public object DragDropData
        {
            get { return GetValue(DragDropDataProperty); }
            set { SetValue(DragDropDataProperty, value); }
        }

        public static readonly DependencyProperty DragDropDataProperty =
            DependencyProperty.Register("DragDropData", typeof(object),
                    typeof(DragStartBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.MouseDown += h,
                h => this.AssociatedObject.MouseDown -= h)
                .Subscribe(e => this.AssociatedObject_MouseDown(null, e));

            Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.MouseMove += h,
                h => this.AssociatedObject.MouseMove -= h)
                .Subscribe(e => this.AssociatedObject_MouseMove(null, e));

            Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.MouseUp += h,
                h => this.AssociatedObject.MouseUp -= h)
                .Subscribe(e => this.AssociatedObject_MouseUp(null, e));

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._origin = e.GetPosition(this.AssociatedObject);
            this._isButtonDown = true;
        }

        void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !this._isButtonDown)
            {
                return;
            }

            var point = e.GetPosition(this.AssociatedObject);

            if (this.CheckDistance(point, this._origin))
            {
                DragDrop.DoDragDrop(this.AssociatedObject, this.DragDropData, this.AllowedEffects);
                this._isButtonDown = false;
                //e.Handled = true;
            }
        }

        void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this._isButtonDown = false;
        }

        private bool CheckDistance(Point x, Point y)
        {
            return Math.Abs(x.X - y.X) >= SystemParameters.MinimumHorizontalDragDistance / 2 ||
                Math.Abs(x.Y - y.Y) >= SystemParameters.MinimumVerticalDragDistance / 2;
        }
    }
}
