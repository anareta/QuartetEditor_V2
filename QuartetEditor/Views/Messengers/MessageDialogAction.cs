using MahApps.Metro.Controls;
using Prism.Interactivity.InteractionRequest;
using QuartetEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using MahApps.Metro.Controls.Dialogs;

namespace QuartetEditor.Views.Messengers
{
    /// <summary>
    /// メッセージダイアログを表示する
    /// </summary>
    public class MessageDialogAction : TriggerAction<MetroWindow>
    {
        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="parameter"></param>
        protected override async void Invoke(object parameter)
        {
            // イベント引数とContextを取得する
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;
            var entity = ctx.Content as DialogArg;

            if (entity != null)
            {
                var result = await this.AssociatedObject.ShowMessageAsync(entity.Title,
                                        entity.Message,
                                        entity.Style,
                                        entity.Settings);

                ctx.Confirmed = result == MessageDialogResult.Affirmative;

                entity.Result = result;
            }

            // コールバックを呼び出す
            args.Callback();
        }
    }
}
