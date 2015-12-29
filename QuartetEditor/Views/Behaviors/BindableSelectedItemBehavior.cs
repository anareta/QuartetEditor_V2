using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Behaviors
{
    /// <summary>
    /// SelectedItemをバインド可能にする
    /// </summary>
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        #region SelectedItemProperty

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null));

        //private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var item = e.NewValue as TreeViewItem;
        //    if (item != null)
        //    {
        //        item.SetValue(TreeViewItem.IsSelectedProperty, true);
        //    }
        //}

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            var selectedItemChanged = Observable.FromEvent<RoutedPropertyChangedEventHandler<object>, RoutedPropertyChangedEventArgs<object>>(
            h => (s, e) => h(e),
            h => this.AssociatedObject.SelectedItemChanged += h,
            h => this.AssociatedObject.SelectedItemChanged -= h);
            selectedItemChanged.Subscribe(e => this.SelectedItem = e.NewValue);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
