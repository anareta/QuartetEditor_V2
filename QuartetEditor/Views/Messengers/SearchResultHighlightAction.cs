using Prism.Interactivity.InteractionRequest;
using QuartetEditor.Entities;
using QuartetEditor.Views.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace QuartetEditor.Views.Messengers
{
    /// <summary>
    /// 検索結果のハイライト表示を行う
    /// </summary>
    public class SearchResultHighlightAction : TriggerAction<FindReplaceDialog>
    {
        /// <summary>
        /// ハイライト情報
        /// </summary>
        public List<OffsetHighlighter> Highlight { get; private set; }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            // イベント引数とContextを取得する
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;
            var entity = ctx.Content as IEnumerable<SearchResult>;

            if (entity != null)
            {
                this.ClearHighlight();

                var highlightList = new List<OffsetHighlighter>();
                foreach (var result in entity)
                {
                    switch (result.Type)
                    {
                        case SearchResult.TargetType.Content:
                            highlightList.Add(new OffsetHighlighter(result.Index, 
                                                                    result.Index + result.Length, 
                                                                    new SolidColorBrush(Color.FromArgb(0x60, 0xff, 0xff, 0x20))));
                            break;
                        case SearchResult.TargetType.Title:
                            continue;
                        default:
                            continue;
                    }
                }

                this.Highlight = highlightList;

                foreach (var item in highlightList)
                {
                    this.AssociatedObject.Editor.TextArea.TextView.LineTransformers.Add(item);
                }

                this.AssociatedObject.Editor.TextChanged += this.Editor_TextChanged;
                this.AssociatedObject.Closed += this.Dialog_Closed;
            }

            // コールバックを呼び出す
            args.Callback();
        }
        
        /// <summary>
        /// ダイアログを閉じるときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dialog_Closed(object sender, EventArgs e)
        {
            this.ClearHighlight();
        }

        /// <summary>
        /// テキスト変更時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            this.ClearHighlight();
        }

        /// <summary>
        /// ハイライト表示をオフに
        /// </summary>
        private void ClearHighlight()
        {
            this.AssociatedObject.Editor.TextChanged -= this.Editor_TextChanged;
            this.AssociatedObject.Closed -= this.Dialog_Closed;

            if (this.Highlight == null)
            {
                return;
            }

            foreach (var item in this.Highlight)
            {
                this.AssociatedObject.Editor.TextArea.TextView.LineTransformers.Remove(item);
            }

            this.Highlight.Clear();
            this.Highlight = null;
        }
    }
}
