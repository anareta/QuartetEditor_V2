using ICSharpCode.AvalonEdit.Document;
using Prism.Mvvm;
using QuartetEditor.Attributes;
using QuartetEditor.Enums;
using QuartetEditor.Extensions;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    class Node : BindableBase, IDisposable
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
            set { this.SetProperty(ref this._Name, value); }
        }

        /// <summary>
        /// ノードのコンテンツ
        /// </summary>
        private TextDocument _Content = new TextDocument();

        [Unique]
        public TextDocument Content
        {
            get { return this._Content; }
            set { this.SetProperty(ref this._Content, value); }
        }

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
        public bool _IsExpanded;

        public bool IsExpanded
        {
            get { return this._IsExpanded; }
            set { this.SetProperty(ref this._IsExpanded, value); }
        }

        /// <summary>
        /// 選択状態
        /// </summary>
        public bool _IsSelected;

        public bool IsSelected
        {
            get { return this._IsSelected; }
            set { this.SetProperty(ref this._IsSelected, value); }
        }

        /// <summary>
        /// ノード名編集モード
        /// </summary>
        public bool _IsNameEditMode;

        public bool IsNameEditMode
        {
            get { return this._IsNameEditMode; }
            set { this.SetProperty(ref this._IsNameEditMode, value); }
        }

        /// <summary>
        /// 参照状態
        /// </summary>
        public bool _IsReferred;

        public bool IsReferred
        {
            get { return this._IsReferred; }
            set { this.SetProperty(ref this._IsReferred, value); }
        }

        /// <summary>
        /// ドラッグオーバーしているか
        /// </summary>
        public bool _IsDragOver;

        public bool IsDragOver
        {
            get { return this._IsDragOver; }
            set { this.SetProperty(ref this._IsDragOver, value); }
        }

        /// <summary>
        /// ドロップする位置
        /// </summary>
        public DropPositionEnum _DropPosition;

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
            private set { this.SetProperty(ref this._IsEdited, value); }
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

            this.ObserveProperty(x => x.Content)
                .Subscribe(x => this.IsEdited = true)
                .AddTo(this.Disposable);

            this.ChildrenSource.ObserveElementProperty(x => x.IsEdited)
                .Subscribe(x =>
                {
                    if (x.Instance.IsEdited)
                    {
                        this.IsEdited = true;
                    }
                })
                .AddTo(this.Disposable);

            this.ChildrenSource.CollectionChangedAsObservable()
                .Subscribe(x => this.IsEdited = true)
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
            this._Content.Text = name;
#endif
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

            foreach (var child in item.ChildrenSource)
            {
                this.ChildrenSource.Add(new Node(child));
            }

            this.Content.Text = item.Content.Text;
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
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
            foreach (var item in this.ChildrenSource)
            {
                item.Dispose();
            }
        }
    }
}
