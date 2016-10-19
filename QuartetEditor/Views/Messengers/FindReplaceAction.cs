using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;
using Prism.Interactivity.InteractionRequest;
using QuartetEditor.Entities;
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
    public class FindReplaceAction : TriggerAction<FindReplaceDialog>
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
            var entity = ctx.Content as FindReplaceEntity;

            if (entity != null)
            {
                switch (entity.Kind)
                {
                    case FindReplaceEntity.Action.Find:
                        {
                            if (!this.FindNext(entity.Find))
                            {
                                SystemSounds.Beep.Play();
                            }
                        }
                        break;
                    case FindReplaceEntity.Action.Replace:
                        {
                            string input = this.AssociatedObject.Editor.Text.Substring(this.AssociatedObject.Editor.SelectionStart, this.AssociatedObject.Editor.SelectionLength);
                            Match match = entity.Find.Match(input);
                            bool replaced = false;
                            if (match.Success && match.Index == 0 && match.Length == input.Length)
                            {
                                this.AssociatedObject.Editor.Document.Replace(this.AssociatedObject.Editor.SelectionStart, this.AssociatedObject.Editor.SelectionLength, entity.Replace);
                                replaced = true;
                            }

                            if (!FindNext(entity.Find) && !replaced)
                            {
                                SystemSounds.Beep.Play();
                            }
                        }
                        break;
                    case FindReplaceEntity.Action.ReplaceAll:
                        {
                            var result = await this.AssociatedObject.ShowMessageAsync("確認",
                                                                                      "本当にすべて置換しますか？",
                                                                                      MessageDialogStyle.AffirmativeAndNegative);
                            if (result == MessageDialogResult.Affirmative)
                            {
                                int offset = 0;
                                this.AssociatedObject.Editor.BeginChange();
                                foreach (Match match in entity.Find.Matches(this.AssociatedObject.Editor.Text))
                                {
                                    this.AssociatedObject.Editor.Document.Replace(offset + match.Index, match.Length, entity.Replace);
                                    offset += entity.Replace.Length - match.Length;
                                }
                                this.AssociatedObject.Editor.EndChange();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            // コールバックを呼び出す
            args.Callback();
        }

        /// <summary>
        /// 次のテキストを検索
        /// </summary>
        /// <param name="textToFind"></param>
        /// <returns></returns>
        private bool FindNext(Regex regex)
        {
            int start = regex.Options.HasFlag(RegexOptions.RightToLeft) ?
            this.AssociatedObject.Editor.SelectionStart : this.AssociatedObject.Editor.SelectionStart + this.AssociatedObject.Editor.SelectionLength;
            Match match = regex.Match(this.AssociatedObject.Editor.Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                if (regex.Options.HasFlag(RegexOptions.RightToLeft))
                    match = regex.Match(this.AssociatedObject.Editor.Text, this.AssociatedObject.Editor.Text.Length);
                else
                    match = regex.Match(this.AssociatedObject.Editor.Text, 0);
            }

            if (match.Success)
            {
                this.AssociatedObject.Editor.Select(match.Index, match.Length);
                TextLocation loc = this.AssociatedObject.Editor.Document.GetLocation(match.Index);
                this.AssociatedObject.Editor.ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }
    }
}
