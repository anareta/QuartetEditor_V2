using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Behaviors
{
    /// <summary>
    /// WPFではカーソルキーに対してショートカットを割り当てられないため、このbehaviorを介してコマンドを実行する
    /// </summary>
    public class CursorKeyBindingBehavior : Behavior<UIElement>
    {
        /// <summary>
        /// ↑とCtrlキーの組み合わせで実行する操作
        /// </summary>
        public ICommand UpCtrlCommand
        {
            get { return (ICommand)GetValue(UpCtrlCommandProperty); }
            set { SetValue(UpCtrlCommandProperty, value); }
        }

        public static readonly DependencyProperty UpCtrlCommandProperty =
            DependencyProperty.Register("UpCtrlCommand", typeof(ICommand), typeof(CursorKeyBindingBehavior)
            , new PropertyMetadata(null));

        /// <summary>
        /// ↓とCtrlキーの組み合わせで実行する操作
        /// </summary>
        public ICommand DownCtrlCommand
        {
            get { return (ICommand)GetValue(DownCtrlCommandProperty); }
            set { SetValue(DownCtrlCommandProperty, value); }
        }

        public static readonly DependencyProperty DownCtrlCommandProperty =
            DependencyProperty.Register("DownCtrlCommand", typeof(ICommand), typeof(CursorKeyBindingBehavior)
            , new PropertyMetadata(null));

        /// <summary>
        /// →とCtrlキーの組み合わせで実行する操作
        /// </summary>
        public ICommand RightCtrlCommand
        {
            get { return (ICommand)GetValue(RightCtrlCommandProperty); }
            set { SetValue(RightCtrlCommandProperty, value); }
        }

        public static readonly DependencyProperty RightCtrlCommandProperty =
            DependencyProperty.Register("RightCtrlCommand", typeof(ICommand), typeof(CursorKeyBindingBehavior)
            , new PropertyMetadata(null));

        /// <summary>
        /// ←とCtrlキーの組み合わせで実行する操作
        /// </summary>
        public ICommand LeftCtrlCommand
        {
            get { return (ICommand)GetValue(LeftCtrlCommandProperty); }
            set { SetValue(LeftCtrlCommandProperty, value); }
        }

        public static readonly DependencyProperty LeftCtrlCommandProperty =
            DependencyProperty.Register("LeftCtrlCommand", typeof(ICommand), typeof(CursorKeyBindingBehavior)
            , new PropertyMetadata(null));

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

            Observable.FromEvent<KeyEventHandler, KeyEventArgs>(
            h => (s, e) => h(e),
            h => this.AssociatedObject.PreviewKeyDown += h,
            h => this.AssociatedObject.PreviewKeyDown -= h)
            .Subscribe(arg =>
            {
                if (arg.Key == Key.Up && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
                {
                    if (this.UpCtrlCommand != null && this.UpCtrlCommand.CanExecute(null))
                    {
                        this.UpCtrlCommand.Execute(null);
                    }
                }

                if (arg.Key == Key.Down && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
                {
                    if (this.DownCtrlCommand != null && this.DownCtrlCommand.CanExecute(null))
                    {
                        this.DownCtrlCommand.Execute(null);
                    }
                }

                if (arg.Key == Key.Right && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
                {
                    if (this.RightCtrlCommand != null && this.RightCtrlCommand.CanExecute(null))
                    {
                        this.RightCtrlCommand.Execute(null);
                    }
                }

                if (arg.Key == Key.Left && (Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
                {
                    if (this.LeftCtrlCommand != null && this.LeftCtrlCommand.CanExecute(null))
                    {
                        this.LeftCtrlCommand.Execute(null);
                    }
                }
            });
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
