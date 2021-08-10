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

            if (target[1] is int val2)
            {
                this.AssociatedObject._LeftTextBox.ScrollTo(val2);
            }

            if (target[2] is int val3)
            {
                this.AssociatedObject._TopTextBox.ScrollTo(val3);
            }

            if (target[3] is int val4)
            {
                this.AssociatedObject._BottomTextBox.ScrollTo(val4);
            }

            args.Callback();
        }
    }
}