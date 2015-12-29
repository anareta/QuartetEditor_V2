using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows;
using QuartetEditor.Extensions;
using System.Reactive.Linq;

namespace QuartetEditor.Views.Behaviors
{
    public sealed class ContentBorderBehavior : Behavior<Border>
    {
        /// <summary>
        /// フォーカスがあるときの色
        /// </summary>
        public Brush FocusedBrush
        {
            get { return (Brush)GetValue(FocusedBrushProperty); }
            set { SetValue(FocusedBrushProperty, value); }
        }

        /// <summary>
        /// FocusedBrushプロパティ
        /// </summary>
        public static readonly DependencyProperty FocusedBrushProperty =
            DependencyProperty.Register("FocusedBrush",
            typeof(Brush),
            typeof(ContentBorderBehavior),
            new PropertyMetadata((Brush)Brushes.White));

        /// <summary>
        /// フォーカスがないときの色
        /// </summary>
        public Brush NotFocusedBrush
        {
            get { return (Brush)GetValue(NotFocusedBrushProperty); }
            set { SetValue(NotFocusedBrushProperty, value); }
        }

        /// <summary>
        /// NotFocusedBrushプロパティ
        /// </summary>
        public static readonly DependencyProperty NotFocusedBrushProperty =
            DependencyProperty.Register("NotFocusedBrush",
            typeof(Brush),
            typeof(ContentBorderBehavior),
            new PropertyMetadata((Brush)Brushes.Black));

        /// <summary>
        /// Borderのコンテンツ
        /// </summary>
        private Control control;

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
            this.AssociatedObject.BorderBrush = this.NotFocusedBrush;
            this.control = this.AssociatedObject.Descendants<Control>().FirstOrDefault();

            if (this.control != null)
            {
                var gotFocus = Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.GotFocus += h,
                h => this.AssociatedObject.GotFocus -= h);
                gotFocus.Subscribe(this.OnGotFocus);

                var lostFocus = Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.LostFocus += h,
                h => this.AssociatedObject.LostFocus -= h);
                lostFocus.Subscribe(this.OnLostFocus);
            }
        }

        /// <summary>
        /// デタッチ
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        /// <summary>
        /// ロストフォーカス時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLostFocus(System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.BorderBrush = this.NotFocusedBrush;
        }

        /// <summary>
        /// フォーカス取得時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnGotFocus(System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.BorderBrush = this.FocusedBrush;
        }

    }
}
