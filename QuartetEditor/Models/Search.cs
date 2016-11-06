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
            this.FindAll();

            var regex = this.GetRegEx(this.TextToFind, findPrev);
            if (regex == null)
            {
                return;
            }

            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart : selectionStart + selectionLength;

            var result = this.WholeAllNode ? 
                            this.FindFromDocument(regex, startIndex, this.Document.SelectedNode) :
                            this.Find(regex, startIndex, this.Document.SelectedNode, false, true);

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
        /// すべて検索する
        /// </summary>
        public void FindAll()
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

            var list = new List<SearchResult>();
            var find = this.Find(regex, 0, this.Document.SelectedNode, false, false);

            if (find != null)
            {
                do
                {
                    list.Add(find);
                    find = this.Find(regex, find.Index + find.Length, this.Document.SelectedNode, false, false);

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
        public void Replace(int selectionStart, int selectionLength, bool editorSelected)
        {
            var regex = this.GetRegEx(this.TextToFind, false);
            if (regex == null)
            {
                return;
            }

            int startIndex = regex.Options.HasFlag(RegexOptions.RightToLeft) ? selectionStart + selectionLength : selectionStart;

            var result = this.WholeAllNode ?
                            this.FindFromDocument(regex, startIndex, this.Document.SelectedNode, null, !editorSelected) :
                            this.Find(regex, startIndex, this.Document.SelectedNode, false, true);


            if (result != null)
            {
                if (result.Node.ID != this.Document.SelectedNode.ID)
                {
                    result.Node.IsSelected = true;
                }

                switch (result.Type)
                {
                    case SearchResult.TargetType.Content:
                        result.Node.Content.Replace(result.Index, result.Length, this.TextToReplace);
                        result.Length = this.TextToReplace.Length;
                        break;
                    case SearchResult.TargetType.Title:
                        result.Node.Name = result.Node.Name.Substring(0, result.Index) + this.TextToReplace + result.Node.Name.Substring(result.Index + result.Length);
                        break;
                    default:
                        break;
                }
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
                    this.Document.ReplaceAllText(regex, this.TextToReplace, this.Document.SelectedNode);
                }

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

            return null;
        }

        /// <summary>
        /// ドキュメント全体から文字を検索します
        /// </summary>
        private SearchResult FindFromDocument(Regex regex, int start, Node node, Node startNode = null, bool titleFirst = false)
        {
            if (titleFirst)
            {
                var titlematch = FindFromNodeName(regex, node);

                if (titlematch != null)
                {
                    return titlematch;
                }
            }

            var result = this.Find(regex, start, node, false, false);

            if (result != null)
            {
                return result;
            }
            else
            {
                var next = regex.Options.HasFlag(RegexOptions.RightToLeft) ?
                    this.Document.GetUp(node) ?? this.Document.FollowLastChild(node, -1) :
                    this.Document.GetDown(node) ?? this.Document.Tree.First();

                if (next != null)
                {
                    var titlematch = FindFromNodeName(regex, next);

                    if (titlematch != null)
                    {
                        return titlematch;
                    }

                    if (node.ID == startNode?.ID)
                    {
                        return null;
                    }

                    return this.FindFromDocument(
                        regex,
                        regex.Options.HasFlag(RegexOptions.RightToLeft) ? next.Content.Text.Length : 0,
                        next,
                        startNode ?? node);
                }
            }

            return null;
        }

        /// <summary>
        /// ノードが正規表現に一致するか判定する
        /// </summary>
        private static SearchResult FindFromNodeName(Regex regex, Node next)
        {
            Match titleMatch;
            if (regex.Options.HasFlag(RegexOptions.RightToLeft))
            {
                titleMatch = regex.Match(next.Name, next.Name.Length);
            }
            else
            {
                titleMatch = regex.Match(next.Name, 0);
            }

            if (titleMatch.Success)
            {
                var loc = new SearchResult() { Type = SearchResult.TargetType.Title };
                loc.Node = next;
                loc.Index = titleMatch.Index;
                loc.Length = titleMatch.Length;
                return loc;
            }

            return null;
        }
    }
}
