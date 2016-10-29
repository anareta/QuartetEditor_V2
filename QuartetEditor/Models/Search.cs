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
        /// 検索結果
        /// </summary>
        public Subject<SearchResult> Found { get; } = new Subject<SearchResult>();

        /// <summary>
        /// 検索結果
        /// </summary>
        public Subject<IEnumerable<SearchResult>> FoundAll { get; } = new Subject<IEnumerable<SearchResult>>();

        /// <summary>
        /// 確認要求
        /// </summary>
        public Subject<Tuple<string, Action<bool>>> Confirmation { get; } = new Subject<Tuple<string, Action<bool>>>();

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
                return new Regex(textToFind, options);
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
        public void FindNext(int selectionStart, int selectionLength)
        {
            this.FindAll();

            var regex = this.GetRegEx(this.TextToFind, false);
            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart : selectionStart + selectionLength;

            var result = this.Find(regex, startIndex, NodeManager.Current.SelectedNode, false, true);

            if (result != null)
            {
                if (result.Node.ID != NodeManager.Current.SelectedNode.ID)
                {
                    NodeManager.Current.SelectedNode = result.Node;
                }
            }
            this.Found.OnNext(result);
        }

        /// <summary>
        /// 前へ検索
        /// </summary>
        public void FindPrev(int selectionStart, int selectionLength)
        {
            this.FindAll();

            var regex = this.GetRegEx(this.TextToFind, true);
            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart : selectionStart + selectionLength;

            var result = this.Find(regex, startIndex, NodeManager.Current.SelectedNode, false, true);

            if (result != null)
            {
                if (result.Node.ID != NodeManager.Current.SelectedNode.ID)
                {
                    NodeManager.Current.SelectedNode = result.Node;
                }
            }
            this.Found.OnNext(result);
        }

        /// <summary>
        /// すべて検索する
        /// </summary>
        public void FindAll()
        {
            var regex = this.GetRegEx(this.TextToFind, false);

            var list = new List<SearchResult>();
            var find = this.Find(regex, 0, NodeManager.Current.SelectedNode, false, false);

            if (find != null)
            {
                do
                {
                    list.Add(find);
                    find = this.Find(regex, find.Index + find.Length, NodeManager.Current.SelectedNode, false, false);

                } while (find != null);

            }
            this.FoundAll.OnNext(list);

            return;
        }

        /// <summary>
        /// 置換
        /// </summary>
        /// <param name="selectionStart"></param>
        /// <param name="selectionLength"></param>
        public void Replace(int selectionStart, int selectionLength)
        {
            var regex = this.GetRegEx(this.TextToFind, false);
            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart + selectionLength : selectionStart;

            var result = this.Find(regex, startIndex, NodeManager.Current.SelectedNode, false, true);

            if (result != null)
            {
                if (result.Node.ID != NodeManager.Current.SelectedNode.ID)
                {
                    NodeManager.Current.SelectedNode = result.Node;
                }

                result.Node.Content.Replace(result.Index, result.Length, this.TextToReplace);
                result.Length = this.TextToReplace.Length;
            }

            this.Found.OnNext(result);
        }

        /// <summary>
        /// すべて置換
        /// </summary>
        public void ReplaceAll()
        {
            this.Confirmation.OnNext(new Tuple<string, Action<bool>>("本当にすべて置換しますか？", (OK) =>
            {
                if (!OK)
                {
                    return;
                }

                var regex = this.GetRegEx(this.TextToFind, false, true);
                var content = NodeManager.Current.SelectedNode.Content;

                int offset = 0;
                content.BeginUpdate();
                foreach (Match match in regex.Matches(content.Text))
                {
                    content.Replace(offset + match.Index, match.Length, this.TextToReplace);
                    offset += this.TextToReplace.Length - match.Length;
                }
                content.EndUpdate();

            }));
        }

        /// <summary>
        /// 次のテキストを検索
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="start"></param>
        /// <param name="node"></param>
        /// <param name="findPrev"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        private SearchResult Find(Regex regex, int start, Node node, bool findPrev, bool loop)
        {
            string text = node.Content.Text;
            Match match = regex.Match(text, start);

            if (loop && !match.Success)  // 見つからなかった場合先頭に戻って探索
            {
                if (regex.Options.HasFlag(RegexOptions.RightToLeft))
                {
                    match = regex.Match(text, text.Length);
                }
                else
                {
                    match = regex.Match(text, 0);
                }
            }

            if (match.Success)
            {
                var loc = new SearchResult() { Type = Entities.SearchResult.TargetType.Content };
                loc.Index = match.Index;
                loc.Length = match.Length;
                loc.Node = node;

                return loc;
            }
            else
            {
                //var next = NodeManager.Current.GetCousinLast(node);
                //if (next != null)
                //{
                //    Match titleMatch;
                //    if (regex.Options.HasFlag(RegexOptions.RightToLeft))
                //        titleMatch = regex.Match(next.Name, next.Name.Length);
                //    else
                //        titleMatch = regex.Match(next.Name, 0);

                //    if (titleMatch.Success)
                //    {
                //        var loc = new Location() { Type = Location.TargetType.Node };
                //        loc.Index = titleMatch.Index;
                //        loc.Length = titleMatch.Length;
                //        loc.Node = next;

                //        return new Tuple<bool, Location>(true, loc);
                //    }

                //    return Find(regex, start, next);
                //}

            }

            return null;
        }

    }
}
