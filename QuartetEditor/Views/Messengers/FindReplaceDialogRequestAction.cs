using Prism.Interactivity.InteractionRequest;
using QuartetEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Messengers
{
    public class FindReplaceDialogRequestAction : TriggerAction<MainWindow>
    {
        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            // イベント引数とContextを取得する
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;
            var entity = ctx.Content as FindReplaceDialogRequestEntity;

            if (entity != null)
            {
                switch (entity.RequestKind)
                {
                    case FindReplaceDialogRequestEntity.DialogRequest.OpenFind:
                        FindReplaceDialog.ShowFindReplaceDialog(this.AssociatedObject, this.AssociatedObject._CenterTextEditor, true);
                        break;
                    case FindReplaceDialogRequestEntity.DialogRequest.OpenReplace:
                        FindReplaceDialog.ShowFindReplaceDialog(this.AssociatedObject, this.AssociatedObject._CenterTextEditor, false);
                        break;
                    case FindReplaceDialogRequestEntity.DialogRequest.FindNext:
                        FindReplaceDialog.Find(this.AssociatedObject, this.AssociatedObject._CenterTextEditor, false);
                        break;
                    case FindReplaceDialogRequestEntity.DialogRequest.FindPrev:
                        FindReplaceDialog.Find(this.AssociatedObject, this.AssociatedObject._CenterTextEditor, true);
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
