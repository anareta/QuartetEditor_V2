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
            this.AssociatedObject.LeftPanelUpdate(state.LeftPanelOpen);
            this.AssociatedObject.TopPanelUpdate(state.TopPanelOpen);
            this.AssociatedObject.BottomPanelUpdate(state.BottomPanelOpen);

            // コールバックを呼び出す
            args.Callback();
        }
    }
}
