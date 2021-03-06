﻿using QuartetEditor.Views.DraggableTreeView.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.DraggableTreeView
{
    /// <summary>
    /// DragDropの受付処理
    /// </summary>
    public sealed class DragAcceptBehavior : Behavior<FrameworkElement>
    {
        public DragAcceptDescription Description
        {
            get { return (DragAcceptDescription)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(DragAcceptDescription),
            typeof(DragAcceptBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            Observable.FromEvent<DragEventHandler, DragEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.DragOver += h,
                h => this.AssociatedObject.DragOver -= h)
                .Subscribe(arg => this.AssociatedObject_DragOver(null, arg));

            Observable.FromEvent<DragEventHandler, DragEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.Drop += h,
                h => this.AssociatedObject.Drop -= h)
                .Subscribe(arg => this.AssociatedObject_Drop(null, arg));

            Observable.FromEvent<DragEventHandler, DragEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.DragEnter += h,
                h => this.AssociatedObject.DragEnter -= h)
                .Subscribe(this.AssociatedObject_Enter);

            Observable.FromEvent<DragEventHandler, DragEventArgs>(
                h => (s, e) => h(e),
                h => this.AssociatedObject.DragLeave += h,
                h => this.AssociatedObject.DragLeave -= h)
                .Subscribe(this.AssociatedObject_Leave);

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            var desc = this.Description;
            if (desc == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }
            desc.OnOver(e);
            //e.Handled = true;
        }

        void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            var desc = this.Description;
            if (desc == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }
            desc.OnDrop(e);
            //e.Handled = true;
        }

        void AssociatedObject_Enter(DragEventArgs e)
        {
            var desc = this.Description;
            if (desc == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }
            desc.OnEnter(e);
            //e.Handled = true;
        }

        void AssociatedObject_Leave(DragEventArgs e)
        {
            var desc = this.Description;
            if (desc == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }
            desc.OnLeave(e);
            //e.Handled = true;
        }
    }
}
