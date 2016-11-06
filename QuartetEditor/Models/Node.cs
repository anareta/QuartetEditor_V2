﻿using ICSharpCode.AvalonEdit.Document;
using Prism.Mvvm;
using QuartetEditor.Attributes;
using QuartetEditor.Entities;
using QuartetEditor.Enums;
using QuartetEditor.Extensions;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    public class Node : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// 識別番号
        /// </summary>
        private string _ID = Guid.NewGuid().ToString();

        /// <summary>
        /// 識別番号
        /// </summary>
        [Unique]
        public string ID
        {
            get { return this._ID; }
            set { this.SetProperty(ref this._ID, value); }
        }

        /// <summary>
        /// ノード名
        /// </summary>
        private string _Name = "新しいノード";

        /// <summary>
        /// ノード名
        /// </summary>
        public string Name
        {
            get { return this._Name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.SetProperty(ref this._Name, "新しいノード");
                    return;
                }
                this.SetProperty(ref this._Name, value.Replace("\n", "").Replace("\r", ""));
            }
        }

        /// <summary>
        /// ノードのコンテンツ
        /// </summary>

        [Unique]
        public TextDocument Content { get; } = new TextDocument();

        /// <summary>
        /// 内部データクラス
        /// </summary>
        private ObservableCollection<Node> _ChildrenSource = new ObservableCollection<Node>();

        [Unique]
        public ObservableCollection<Node> ChildrenSource
        {
            get { return this._ChildrenSource; }
            set { this.SetProperty(ref this._ChildrenSource, value); }
        }

        /// <summary>
        /// 読み取り専用データツリー
        /// </summary>
        public ReadOnlyObservableCollection<Node> _Children;

        [Unique]
        public ReadOnlyObservableCollection<Node> Children
        {
            get { return this._Children; }
            set { this.SetProperty(ref this._Children, value); }
        }

        /// <summary>
        /// 展開状態
        /// </summary>
        public bool _IsExpanded = true;

        public bool IsExpanded
        {
            get { return this._IsExpanded; }
            set { this.SetProperty(ref this._IsExpanded, value); }
        }

        /// <summary>
        /// 選択状態
        /// </summary>
        public bool _IsSelected;

        [Unique]
        public bool IsSelected
        {
            get { return this._IsSelected; }
            set { this.SetProperty(ref this._IsSelected, value); }
        }

        /// <summary>
        /// ノード名編集モード
        /// </summary>
        public bool _IsNameEditMode;

        [Unique]
        public bool IsNameEditMode
        {
            get { return this._IsNameEditMode; }
            set { this.SetProperty(ref this._IsNameEditMode, value); }
        }

        /// <summary>
        /// 参照状態
        /// </summary>
        public bool _IsReferred;

        [Unique]
        public bool IsReferred
        {
            get { return this._IsReferred; }
            set { this.SetProperty(ref this._IsReferred, value); }
        }

        /// <summary>
        /// ドラッグオーバーしているか
        /// </summary>
        public bool _IsDragOver;

        [Unique]
        public bool IsDragOver
        {
            get { return this._IsDragOver; }
            set { this.SetProperty(ref this._IsDragOver, value); }
        }

        /// <summary>
        /// ドロップする位置
        /// </summary>
        public DropPositionEnum _DropPosition;

        [Unique]
        public DropPositionEnum DropPosition
        {
            get { return this._DropPosition; }
            set
            {
                this._DropPosition = value;
                this.OnPropertyChanged(() => this.DropPosition);
            }
        }

        /// <summary>
        /// 編集されたか
        /// </summary>
        public bool _IsEdited = false;

        [Unique]
        public bool IsEdited
        {
            get { return this._IsEdited; }
            set { this.SetProperty(ref this._IsEdited, value); }
        }

        /// <summary>
        /// 子ノードが編集された
        /// </summary>
        public bool _ChildrenEdited = false;

        [Unique]
        public bool ChildrenEdited
        {
            get { return this._ChildrenEdited; }
            set { this.SetProperty(ref this._ChildrenEdited, value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Node()
        {
            // 接続
            this.Children = new ReadOnlyObservableCollection<Node>(this.ChildrenSource);

            // プロパティの変更を監視する
            this.ObserveProperty(x => x.Name)
                .Subscribe(x => this.IsEdited = true)
                .AddTo(this.Disposable);

            Observable.FromEventPattern(h => this.Content.TextChanged += h,
                                        h => this.Content.TextChanged -= h)
                      .Subscribe(_ => this.IsEdited = true).AddTo(this.Disposable);

            // 子ノードの変更検出
            this.ChildrenSource.ObserveElementProperty(x => x.IsEdited).Where(x => x.Value)
                .Merge(this.ChildrenSource.ObserveElementProperty(x => x.ChildrenEdited).Where(x => x.Value))
                .Subscribe(x =>
                {
                    this.ChildrenEdited = true;
                })
                .AddTo(this.Disposable);

            this.ChildrenSource.CollectionChangedAsObservable()
                .Subscribe(x => this.ChildrenEdited = true)
                .AddTo(this.Disposable);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">ノードの名前</param>
        public Node(string name) : this()
        {
            this._Name = name;
#if DEBUG
            this.Content.Text = name;
#endif
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Node(QuartetEditorDescriptionItem QEDItem) : this()
        {
            this.Name = QEDItem.Name;
            this.Content.Text = QEDItem.Content;
            this.Content.UndoStack.ClearAll();
            foreach (var item in QEDItem.Children)
            {
                this.ChildrenSource.Add(new Node(item));
            }
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="item"></param>
        public Node(Node item) : this()
        {
            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(UniqueAttribute), false);
                if (!(attribute.Count() > 0))
                {
                    property.SetValue(this, property.GetValue(item, null));
                }
            }

            this.Content.Text = item.Content.Text;
            this.Content.UndoStack.ClearAll();

            foreach (var child in item.ChildrenSource)
            {
                this.ChildrenSource.Add(new Node(child));
            }
        }


        /// <summary>
        /// 子ノードを追加する
        /// </summary>
        /// <returns>追加した子ノードへの参照</returns>
        public Node AddChild()
        {
            var item = new Node();
            this.ChildrenSource.Add(item);
            return item;
        }

        /// <summary>
        /// すべてのノードに対しての操作を提供します
        /// </summary>
        /// <param name="act"></param>
        public void WorkAllNode(Action<Node> act)
        {
            act(this);
            this.Children.ForEach(node => node.WorkAllNode(act));
        }

        /// <summary>
        /// 条件を満たすノードが存在するか判定します
        /// </summary>
        public bool HasAnyNode(Predicate<Node> predicate)
        {
            if (predicate(this))
            {
                return true;
            }
            return this.Children.Any(node => node.HasAnyNode(predicate));
        }

        #region FindAndReplace

        /// <summary>
        /// 次のテキストを検索
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="start"></param>
        /// <param name="node"></param>
        /// <param name="findPrev"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public SearchResult Find(Regex regex, int start, bool findPrev, bool loop)
        {
            string text = this.Content.Text;
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
                loc.Node = this;

                return loc;
            }

            return null;
        }

        /// <summary>
        /// すべて検索する
        /// </summary>
        public IEnumerable<SearchResult> FindAll(Regex regex)
        {
            var list = new List<SearchResult>();
            var find = this.Find(regex, 0, false, false);

            if (find != null)
            {
                do
                {
                    list.Add(find);
                    find = this.Find(regex, find.Index + find.Length, false, false);

                } while (find != null);

            }
            return list;
        }

        /// <summary>
        /// ノード名が正規表現に一致するか判定する
        /// </summary>
        public SearchResult FindFromNodeName(Regex regex)
        {
            Match titleMatch;
            if (regex.Options.HasFlag(RegexOptions.RightToLeft))
            {
                titleMatch = regex.Match(this.Name, this.Name.Length);
            }
            else
            {
                titleMatch = regex.Match(this.Name, 0);
            }

            if (titleMatch.Success)
            {
                var loc = new SearchResult() { Type = SearchResult.TargetType.Title };
                loc.Node = this;
                loc.Index = titleMatch.Index;
                loc.Length = titleMatch.Length;
                return loc;
            }

            return null;
        }

        /// <summary>
        /// １つ置換
        /// </summary>
        public SearchResult Replace(SearchResult find, string textToReplace)
        {
            if (find != null)
            {
                switch (find.Type)
                {
                    case SearchResult.TargetType.Content:
                        find.Node.Content.Replace(find.Index, find.Length, textToReplace);
                        find.Length = textToReplace.Length;
                        break;
                    case SearchResult.TargetType.Title:
                        find.Node.Name = find.Node.Name.Substring(0, find.Index) + textToReplace + find.Node.Name.Substring(find.Index + find.Length);
                        break;
                    default:
                        break;
                }
            }

            return find;
        }

        /// <summary>
        /// ノードのテキストを全文置換します
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="textToReplace"></param>
        /// <param name="node"></param>
        public void ReplaceAll(Regex regex, string textToReplace)
        {
            int offset = 0;
            this.Content.BeginUpdate();
            foreach (Match match in regex.Matches(this.Content.Text))
            {
                this.Content.Replace(offset + match.Index, match.Length, textToReplace);
                offset += textToReplace.Length - match.Length;
            }
            this.Content.EndUpdate();
        }

        /// <summary>
        /// ノード名のテキストを置換します
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="textToReplace"></param>
        /// <param name="node"></param>
        public void ReplaceNodeName(Regex regex, string textToReplace)
        {
            int offset = 0;
            foreach (Match match in regex.Matches(this.Name))
            {
                this.Name = this.Name.Substring(0, match.Index + offset) + textToReplace + this.Name.Substring(match.Index + match.Length + offset);
                offset += textToReplace.Length - match.Length;
            }
        }
        #endregion FindAndReplace

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            if (QEDocument.Current.Content.HasAnyNode(node => node.ID == this.ID))
            {
                // NodeManagerに登録されている場合は破棄処理を実行しない
                return;
            }

            this.Disposable.Dispose();
            foreach (var item in this.ChildrenSource)
            {
                item.Dispose();
            }
        }
    }
}
