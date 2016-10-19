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
        /// Viewへの検索置換要求
        /// </summary>
        public Subject<FindReplaceEntity> FindReplaceRequest { get; } = new Subject<FindReplaceEntity>();

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
        /// 次へ
        /// </summary>
        public void FindNext()
        {
            var entity = new FindReplaceEntity
            {
                Kind = FindReplaceEntity.Action.Find,
                Find = this.GetRegEx(this.TextToFind, false)
            };
            this.FindReplaceRequest.OnNext(entity);
        }

        /// <summary>
        /// 前へ
        /// </summary>
        public void FindPrev()
        {
            var entity = new FindReplaceEntity
            {
                Kind = FindReplaceEntity.Action.Find,
                Find = this.GetRegEx(this.TextToFind, true)
            };
            this.FindReplaceRequest.OnNext(entity);
        }

        /// <summary>
        /// 置換
        /// </summary>
        public void Replace()
        {
            var entity = new FindReplaceEntity
            {
                Kind = FindReplaceEntity.Action.Replace,
                Find = this.GetRegEx(this.TextToFind, false),
                Replace = this.TextToReplace
            };
            this.FindReplaceRequest.OnNext(entity);
        }

        /// <summary>
        /// すべて置換
        /// </summary>
        public void ReplaceAll()
        {
            var entity = new FindReplaceEntity
            {
                Kind = FindReplaceEntity.Action.ReplaceAll,
                Find = this.GetRegEx(this.TextToFind, false, true),
                Replace = this.TextToReplace
            };
            this.FindReplaceRequest.OnNext(entity);
        }
    }
}
