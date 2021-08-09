using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Messengers
{
    public class SetScrollAction : TriggerAction<MainWindow>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;
            var target = ctx.Content as int?;

            if (target is int line)
            {
                this.AssociatedObject._CenterTextEditor.ScrollTo(line);
            }

            args.Callback();
        }
    }
}