using QuartetEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Entities
{
    /// <summary>
    /// QEDエンティティクラス
    /// </summary>
    class QuartetEditorDescription
    {
        /// <summary>
        /// ノード
        /// </summary>
        public List<QuartetEditorDescriptionItem> Node { set; get; } = new List<QuartetEditorDescriptionItem>();

        /// <summary>
        /// バージョン
        /// </summary>
        public double Version { set; get; } = 1.0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node"></param>
        public QuartetEditorDescription(IList<Node> tree)
        {
            foreach (var node in tree)
            {
                this.Node.Add(new QuartetEditorDescriptionItem(node));
            }
        }
    }

    /// <summary>
    /// QEDエンティティクラス
    /// </summary>
    class QuartetEditorDescriptionItem
    {
        /// <summary>
        /// ノード名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        ///  コンテンツ
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// 子
        /// </summary>
        public List<QuartetEditorDescriptionItem> Children { set; get; } = new List<QuartetEditorDescriptionItem>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node"></param>
        public QuartetEditorDescriptionItem(Node node)
        {
            this.Name = node.Name;
            this.Content = node.Content.Text;
            foreach (var child in node.ChildrenSource)
            {
                this.Children.Add(new QuartetEditorDescriptionItem(child));
            }
        }
    }
}
