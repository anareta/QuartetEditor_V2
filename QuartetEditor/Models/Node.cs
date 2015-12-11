using ICSharpCode.AvalonEdit.Document;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    class Node : BindableBase
    {
        /// <summary>
        /// 識別番号
        /// </summary>
        private string _ID = Guid.NewGuid().ToString();

        /// <summary>
        /// 識別番号
        /// </summary>
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

        public TextDocument Content
        {
            get { return this._Content; }
            set { this.SetProperty(ref this._Content, value); }
        }

        /// <summary>
        /// 内部データクラス
        /// </summary>
        private ObservableCollection<Node> _ChildrenSource = new ObservableCollection<Node>();

        public ObservableCollection<Node> ChildrenSource
        {
            get { return this._ChildrenSource; }
            set { this.SetProperty(ref this._ChildrenSource, value); }
        }

        /// <summary>
        /// 読み取り専用データツリー
        /// </summary>
        public ReadOnlyObservableCollection<Node> _Children;

        public ReadOnlyObservableCollection<Node> Children
        {
            get { return this._Children; }
            set { this.SetProperty(ref this._Children, value); }
        }

        /// <summary>
        /// 展開状態
        /// </summary>
        public bool IsExpanded { set; get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Node()
        {
            // 接続
            this.Children = new ReadOnlyObservableCollection<Node>(this.ChildrenSource);
        }
    }
}
