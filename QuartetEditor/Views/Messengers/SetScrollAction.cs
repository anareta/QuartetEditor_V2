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

            // { [編集パネル], [左パネル], [上パネル], [下パネル] }
            var target = ctx.Content as int?[];

            if (target[0] is int val1)
            {
                this.AssociatedObject._CenterTextEditor.ScrollTo(val1);
            }

            args.Callback();
        }
    }
}