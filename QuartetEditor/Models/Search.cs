using ICSharpCode.AvalonEdit.Document;
using Prism.Mvvm;
using QuartetEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    /// <summary>
    /// 検索
    /// </summary>
    public class Search : BindableBase
    {
        /// <summary>
        /// 検索結果の通知
        /// </summary>
        public Subject<SearchResult> Found { get; } = new Subject<SearchResult>();

        /// <summary>
        /// 検索結果（すべて）の通知
        /// </summary>
        public Subject<IEnumerable<SearchResult>> FoundAll { get; } = new Subject<IEnumerable<SearchResult>>();

        /// <summary>
        /// 確認要求
        /// </summary>
        public Subject<Tuple<string, Action<bool>>> Confirmation { get; } = new Subject<Tuple<string, Action<bool>>>();

        /// <summary>
        /// 検索対象のドキュメント
        /// </summary>
        public NodeManager Document { get; private set; } = QEDocument.Current.Content;

        /// <summary>
        /// 検索文字
        /// </summary>
        public string TextToFind
        {
            get
            {
                return this._TextToFind;
            }
            set
            {
                this.SetProperty(ref this._TextToFind, value);
            }
        }
        private string _TextToFind = "";

        /// <summary>
        /// 置換文字
        /// </summary>
        public string TextToReplace
        {
            get
            {
                return this._TextToReplace;
            }
            set
            {
                this.SetProperty(ref this._TextToReplace, value);
            }
        }
        private string _TextToReplace = "";

        /// <summary>
        /// 大文字小文字を区別する
        /// </summary>
        public bool CaseSensitive
        {
            get
            {
                return this._CaseSensitive;
            }
            set
            {
                this.SetProperty(ref this._CaseSensitive, value);
            }
        }
        private bool _CaseSensitive = true;

        /// <summary>
        /// 単語として検索
        /// </summary>
        public bool WholeWord
        {
            get
            {
                return this._WholeWord;
            }
            set
            {
                this.SetProperty(ref this._WholeWord, value);

                if (value)
                {
                    // 正規表現の使用と排他関係
                    this.UseRegex = false;
                }
            }
        }
        private bool _WholeWord = false;

        /// <summary>
        /// 正規表現を使用
        /// </summary>
        public bool UseRegex
        {
            get
            {
                return this._UseRegex;
            }
            set
            {
                this.SetProperty(ref this._UseRegex, value);

                if (value)
                {
                    // ワイルドカード、単語検索と排他関係
                    this.WholeWord = false;
                    this.UseWildcards = false;
                }
            }
        }
        private bool _UseRegex = false;

        /// <summary>
        /// ワイルドカードを使用
        /// </summary>
        public bool UseWildcards
        {
            get
            {
                return this._UseWildcards;
            }
            set
            {
                this.SetProperty(ref this._UseWildcards, value);

                if (value)
                {
                    // 正規表現の使用と排他関係
                    this.UseRegex = false;
                }
            }
        }
        private bool _UseWildcards = false;

        /// <summary>
        /// 全ノードを対象に検索
        /// </summary>
        public bool WholeAllNode
        {
            get
            {
                return this._WholeAllNode;
            }
            set
            {
                this.SetProperty(ref this._WholeAllNode, value);
            }
        }
        private bool _WholeAllNode = false;

        /// <summary>
        /// 全ノードを対象に検索
        /// </summary>
        public bool HilightText
        {
            get
            {
                return this._HilightText;
            }
            set
            {
                this.SetProperty(ref this._HilightText, value);
            }
        }
        private bool _HilightText = false;

        /// <summary>
        /// 検索用の正規表現を生成する
        /// </summary>
        /// <param name="textToFind"></param>
        /// <param name="searchUp"></param>
        /// <param name="leftToRight"></param>
        /// <returns></returns>
        private Regex GetRegEx(string textToFind, bool searchUp, bool leftToRight = false)
        {
            RegexOptions options = RegexOptions.None;
            if (searchUp && !leftToRight)
            {
                options |= RegexOptions.RightToLeft;
            }

            if (!this.CaseSensitive)
            {
                options |= RegexOptions.IgnoreCase;
            }

            if (this.UseRegex)
            {
                try
                {
                    return new Regex(textToFind, options);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                string pattern = Regex.Escape(textToFind);
                if (this.UseWildcards)
                {
                    pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                }

                if (this.WholeWord)
                {
                    pattern = "\\b" + pattern + "\\b";
                }
                return new Regex(pattern, options);
            }
        }

        /// <summary>
        /// 次へ検索
        /// </summary>
        public void FindNext(int selectionStart, int selectionLength, bool findPrev)
        {
            this.HilightAll();

            var regex = this.GetRegEx(this.TextToFind, findPrev);
            if (regex == null)
            {
                return;
            }

            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart : selectionStart + selectionLength;

            var result = this.WholeAllNode ? 
                            this.Document.Find(regex, startIndex, this.Document.SelectedNode) :
                            this.Document.SelectedNode.Find(regex, startIndex, false, true);

            if (result != null)
            {
                if (result.Node.ID != this.Document.SelectedNode.ID)
                {
                    result.Node.IsSelected = true;
                }
            }
            this.Found.OnNext(result);
        }

        /// <summary>
        /// すべて検索してハイライト表示する
        /// </summary>
        public void HilightAll()
        {
            if (!this.HilightText)
            {
                // ハイライトを消去
                this.FoundAll.OnNext(new List<SearchResult>());
                return;
            }

            var regex = this.GetRegEx(this.TextToFind, false);
            if (regex == null)
            {
                return;
            }

            var find = this.Document.SelectedNode.FindAll(regex);

            this.FoundAll.OnNext(find);

            return;
        }

        /// <summary>
        /// 置換
        /// </summary>
        public void Replace(int selectionStart, int selectionLength, bool editorSelected)
        {
            var regex = this.GetRegEx(this.TextToFind, false);
            if (regex == null)
            {
                return;
            }

            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart + selectionLength : selectionStart;

            var result = this.WholeAllNode ?
                            this.Document.Find(regex, startIndex, this.Document.SelectedNode, null, !editorSelected) :
                            this.Document.SelectedNode.Find(regex, startIndex, false, true);

            if (result != null)
            {
                if (result.Node.ID != this.Document.SelectedNode.ID)
                {
                    result.Node.IsSelected = true;
                }

                result.Node.Replace(result, this.TextToReplace);
            }

            this.Found.OnNext(result);
        }

        /// <summary>
        /// すべて置換
        /// </summary>
        public void ReplaceAll()
        {
            this.Confirmation.OnNext(new Tuple<string, Action<bool>>("この操作は元に戻せません。本当にすべて置換しますか？", (OK) =>
            {
                if (!OK)
                {
                    return;
                }

                var regex = this.GetRegEx(this.TextToFind, false, true);
                if (regex == null)
                {
                    return;
                }

                if (this.WholeAllNode)
                {
                    this.Document.ReplaceAllTextWholeAllNode(regex, this.TextToReplace);
                }
                else
                {
                this.Document.SelectedNode.ReplaceAll(regex, this.TextToReplace);
                }

            }));
        }
    }
}
