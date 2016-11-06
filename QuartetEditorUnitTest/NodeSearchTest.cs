using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using QuartetEditor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuartetEditor.Entities;

namespace QuartetEditorUnitTest
{
    /// <summary>
    /// Node検索系機能のテスト
    /// </summary>
    [TestClass]
    public class NodeSearchTest
    {
        public NodeSearchTest()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// テスト後のノード掃除
        /// </summary>
        [TestCleanup()]
        public void NodeCleanup()
        {

        }


        private List<QuartetEditorDescriptionItem> CreateItemList(params QuartetEditorDescriptionItem[] nodes)
        {
            return new List<QuartetEditorDescriptionItem>(nodes);
        }

        private QuartetEditorDescriptionItem CreateItem(string name, string content = "")
        {
            return new QuartetEditorDescriptionItem { Name = name, Content = content };
        }

        private void SetQED(ref NodeManager target, QuartetEditorDescription qed)
        {
            var pbObj = new PrivateObject(target);

            // private void Load(QuartetEditorDescription model)
            pbObj.Invoke("Load", qed);
        }

        private Node FindNode(NodeManager target, string nodeName)
        {
            var pbObj = new PrivateObject(target);

            // private Node Find(IList<Node> list, Predicate<Node> predicate)
            Predicate<Node> search = (node) => node.Name == nodeName;
            return pbObj.Invoke("Find", target.Tree, search) as Node;
        }

        private Node GetUp(NodeManager target, Node node)
        {
            var pbObj = new PrivateObject(target);

            return pbObj.Invoke("GetUp", node) as Node;
        }

        private Node GetDown(NodeManager target, Node node)
        {
            var pbObj = new PrivateObject(target);

            return pbObj.Invoke("GetDown", node) as Node;
        }

        #region GetPrev

        [TestMethod]
        public void GetPrev_同階層()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");

            var pbObj = new PrivateObject(model);

            // private Node GetPrev(Node item)
            var find = pbObj.Invoke("GetPrev", selected) as Node;

            Assert.AreSame("ノード１", find.Name);
        }

        [TestMethod]
        public void GetPrev_ない()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");

            var pbObj = new PrivateObject(model);

            // private Node GetPrev(Node item)
            var find = pbObj.Invoke("GetPrev", selected) as Node;

            Assert.AreSame(null, find);
        }

        [TestMethod]
        public void GetPrev_１階層またぎ_ルート通る()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２-１");

            var pbObj = new PrivateObject(model);

            // private Node GetPrev(Node item)
            var find = pbObj.Invoke("GetPrev", selected) as Node;

            Assert.AreSame("ノード１-２", find.Name);
        }

        [TestMethod]
        public void GetPrev_１階層またぎ_ルート通らない()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[0].Children = CreateItemList(
                CreateItem("ノード１-１-１"),
                CreateItem("ノード１-１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-２-１");

            var pbObj = new PrivateObject(model);

            // private Node GetPrev(Node item)
            var find = pbObj.Invoke("GetPrev", selected) as Node;

            Assert.AreSame("ノード１-１-２", find.Name);
        }

        [TestMethod]
        public void GetPrev_３階層またぎ()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[0].Children[1].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-２-１"),
                CreateItem("ノード１-２-２-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );
            nodes.Node[1].Children[0].Children = CreateItemList(
                CreateItem("ノード２-１-１"),
                CreateItem("ノード２-１-２")
                );
            nodes.Node[1].Children[0].Children[0].Children = CreateItemList(
                CreateItem("ノード２-１-１-１"),
                CreateItem("ノード２-１-１-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２-１-１-１");

            var pbObj = new PrivateObject(model);

            // private Node GetPrev(Node item)
            var find = pbObj.Invoke("GetPrev", selected) as Node;

            Assert.AreSame("ノード１-２-２-２", find.Name);
        }

        #endregion GetPrev

        #region GetNext

        [TestMethod]
        public void GetNext_同階層()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");

            var pbObj = new PrivateObject(model);

            // private Node GetNext(Node item)
            var find = pbObj.Invoke("GetNext", selected) as Node;

            Assert.AreSame("ノード２", find.Name);
        }

        [TestMethod]
        public void GetNext_ない()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード３");

            var pbObj = new PrivateObject(model);

            // private Node GetNext(Node item)
            var find = pbObj.Invoke("GetNext", selected) as Node;

            Assert.AreSame(null, find);
        }

        [TestMethod]
        public void GetNext_１階層またぎ_ルート通る()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );
            nodes.Node[1].Children[1].Children = CreateItemList(
                CreateItem("ノード２-２-１"),
                CreateItem("ノード２-２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-２");

            var pbObj = new PrivateObject(model);

            // private Node GetNext(Node item)
            var find = pbObj.Invoke("GetNext", selected) as Node;

            Assert.AreSame("ノード２-１", find.Name);
        }

        [TestMethod]
        public void GetNext_１階層またぎ_ルート通らない()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２"),
                    CreateItem("ノード３")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[0].Children = CreateItemList(
                CreateItem("ノード１-１-１"),
                CreateItem("ノード１-１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-１-２");

            var pbObj = new PrivateObject(model);

            // private Node GetNext(Node item)
            var find = pbObj.Invoke("GetNext", selected) as Node;

            Assert.AreSame("ノード１-２-１", find.Name);
        }

        [TestMethod]
        public void GetNext_３階層またぎ()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[0].Children[1].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-２-１"),
                CreateItem("ノード１-２-２-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );
            nodes.Node[1].Children[0].Children = CreateItemList(
                CreateItem("ノード２-１-１"),
                CreateItem("ノード２-１-２")
                );
            nodes.Node[1].Children[0].Children[0].Children = CreateItemList(
                CreateItem("ノード２-１-１-１"),
                CreateItem("ノード２-１-１-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-２-２-２");

            var pbObj = new PrivateObject(model);

            // private Node GetNext(Node item)
            var find = pbObj.Invoke("GetNext", selected) as Node;

            Assert.AreSame("ノード２-１-１-１", find.Name);
        }

        #endregion GetNext

        #region GetOlder
        // private Node GetOlder(Node item)

        [TestMethod]
        public void GetOlder_ルート()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");

            var pbObj = new PrivateObject(model);
            
            var find = pbObj.Invoke("GetOlder", selected) as Node;

            Assert.AreSame("ノード１", find.Name);
        }

        [TestMethod]
        public void GetOlder_ない()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");

            var pbObj = new PrivateObject(model);

            var find = pbObj.Invoke("GetOlder", selected) as Node;

            Assert.AreSame(null, find);
        }

        [TestMethod]
        public void GetOlder_ルート以外()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-２");

            var pbObj = new PrivateObject(model);

            var find = pbObj.Invoke("GetOlder", selected) as Node;

            Assert.AreSame("ノード１-１", find.Name);
        }

        #endregion GetOlder

        #region GetYounger
        // private Node GetYounger(Node item)

        [TestMethod]
        public void GetYounger_ルート()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");

            var pbObj = new PrivateObject(model);

            var find = pbObj.Invoke("GetYounger", selected) as Node;

            Assert.AreSame("ノード２", find.Name);
        }

        [TestMethod]
        public void GetYounger_ない()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");

            var pbObj = new PrivateObject(model);

            var find = pbObj.Invoke("GetYounger", selected) as Node;

            Assert.AreSame(null, find);
        }

        [TestMethod]
        public void GetYounger_ルート以外()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-１");

            var pbObj = new PrivateObject(model);

            var find = pbObj.Invoke("GetYounger", selected) as Node;

            Assert.AreSame("ノード１-２", find.Name);
        }

        #endregion GetYounger

        #region GetUp

        [TestMethod]
        public void GetUp_同階層()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");
            var find = this.GetUp(model, selected);

            Assert.AreSame("ノード１", find.Name);
        }

        [TestMethod]
        public void GetUp_なし()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");

            var find = this.GetUp(model, selected);

            Assert.AreSame(null, find);
        }

        [TestMethod]
        public void GetUp_親()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２-１");

            var find = this.GetUp(model, selected);

            Assert.AreSame("ノード２", find.Name);
        }

        [TestMethod]
        public void GetUp_姉妹の子()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");

            var find = this.GetUp(model, selected);

            Assert.AreSame("ノード１-２-２", find.Name);
        }

        [TestMethod]
        public void GetUp_姉妹の子孫()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[0].Children[1].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-２-１"),
                CreateItem("ノード１-２-２-２")
                );
            nodes.Node[0].Children[1].Children[1].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-２-２-１"),
                CreateItem("ノード１-２-２-２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");

            var find = this.GetUp(model, selected);

            Assert.AreSame("ノード１-２-２-２-２", find.Name);
        }

        #endregion GetUp

        #region GetDown

        [TestMethod]
        public void GetDown_同階層()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");
            var find = this.GetDown(model, selected);

            Assert.AreSame("ノード２", find.Name);
        }

        [TestMethod]
        public void GetDown_なし()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード２");

            var find = this.GetDown(model, selected);

            Assert.AreSame(null, find);
        }

        [TestMethod]
        public void GetDown_親の妹()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[1].Children = CreateItemList(
                CreateItem("ノード２-１"),
                CreateItem("ノード２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-２");

            var find = this.GetDown(model, selected);

            Assert.AreSame("ノード２", find.Name);
        }

        [TestMethod]
        public void GetDown_子()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１");

            var find = this.GetDown(model, selected);

            Assert.AreSame("ノード１-１", find.Name);
        }

        [TestMethod]
        public void GetDown_祖先の妹()
        {
            var model = new NodeManager();

            var nodes = new QuartetEditorDescription()
            {
                Node = CreateItemList(
                    CreateItem("ノード１"),
                    CreateItem("ノード２")
                    )
            };
            nodes.Node[0].Children = CreateItemList(
                CreateItem("ノード１-１"),
                CreateItem("ノード１-２")
                );
            nodes.Node[0].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-１"),
                CreateItem("ノード１-２-２")
                );
            nodes.Node[0].Children[1].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-２-１"),
                CreateItem("ノード１-２-２-２")
                );
            nodes.Node[0].Children[1].Children[1].Children[1].Children = CreateItemList(
                CreateItem("ノード１-２-２-２-１"),
                CreateItem("ノード１-２-２-２-２")
                );

            SetQED(ref model, nodes);
            var selected = FindNode(model, "ノード１-２-２-２-２");

            var find = this.GetDown(model, selected);

            Assert.AreSame("ノード２", find.Name);
        }

        #endregion GetUp
    }
}
