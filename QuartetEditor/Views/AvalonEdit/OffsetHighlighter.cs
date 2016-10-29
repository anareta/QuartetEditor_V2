using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QuartetEditor.Views.AvalonEdit
{
    /// <summary>
    /// TextEditorにハイライトを付ける
    /// </summary>
    public class OffsetHighlighter : DocumentColorizingTransformer
    {
        /// <summary>
        /// ハイライトをつける開始位置（先頭からの文字数）
        /// </summary>
        public int StartOffset { get; private set; }

        /// <summary>
        /// ハイライトをつける終了位置（先頭からの文字数）
        /// </summary>
        public int EndOffset { get; private set; }

        /// <summary>
        /// 着色する色
        /// </summary>
        public Brush HighlightColor { get; set; }

        /// <summary>
        /// TextEditorにハイライトを付ける
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public OffsetHighlighter(int startIndex, int endIndex, Brush color)
        {
            this.StartOffset = startIndex;
            this.EndOffset = endIndex;
            this.HighlightColor = color;
        }

        /// <summary>
        /// 行ごとの着色
        /// </summary>
        /// <param name="line"></param>
        protected override void ColorizeLine(DocumentLine line)
        {
            try
            {
                if (line.Length == 0)
                {
                    return;
                }

                int start = line.Offset > StartOffset ? line.Offset : StartOffset;
                int end = EndOffset > line.EndOffset ? line.EndOffset : EndOffset;

                var color = this.HighlightColor ?? new SolidColorBrush(Color.FromArgb(0xa0, 0xff, 0xff, 0x10));

                // 黄色にハイライト
                base.ChangeLinePart(
                    start, 
                    end, 
                    element => element.TextRunProperties.SetBackgroundBrush(color)
                    );
            }
            catch (Exception)
            {
            }
        }
    }
}
