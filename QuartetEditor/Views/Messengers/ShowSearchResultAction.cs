using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;
using Prism.Interactivity.InteractionRequest;
using QuartetEditor.Entities;
using QuartetEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Messengers
{
    /// <summary>
    /// 検索置換処理
    /// </summary>
    public class ShowSearchResultAction : TriggerAction<FindReplaceDialog>
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
            var entity = ctx.Content as SearchResult;

            if (entity != null)
            {

                this.AssociatedObject.Editor.Select(entity.Index, entity.Length);
                TextLocation loc = this.AssociatedObject.Editor.Document.GetLocation(entity.Index);
                this.AssociatedObject.Editor.ScrollTo(loc.Line, loc.Column);

            }
            else
            {
                SystemSounds.Beep.Play();
            }

            // コールバックを呼び出す
            args.Callback();
        }
    }
}
