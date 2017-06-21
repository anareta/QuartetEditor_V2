using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuartetEditor.Utilities;
using QuartetEditor.Entities;

namespace QuartetEditorUnitTest
{
    [TestClass]
    public class NodeConverterUtilityTest
    {
        [TestMethod]
        public void FromTreeText_一段階の構造()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + "ノードの構造とは無関係の文章" + lineFeed + 
                       ".ノード１のタイトル" + lineFeed + 
                       "ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed + lineFeed +
                       ".ノード２のタイトル" + lineFeed +
                       "ノード２の本文" + lineFeed + "ノード２の本文" + lineFeed + lineFeed;
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed, result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);

            Assert.AreEqual("ノード２のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード２の本文" + lineFeed + "ノード２の本文" + lineFeed + lineFeed, result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_行頭に改行なし()
        {
            string lineFeed = Environment.NewLine;
            var text = ".ノード１のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed + lineFeed +
                       ".ノード２のタイトル" + lineFeed +
                       "ノード２の本文" + lineFeed + "ノード２の本文" + lineFeed + lineFeed;
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed, result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);

            Assert.AreEqual("ノード２のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード２の本文" + lineFeed + "ノード２の本文" + lineFeed + lineFeed, result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_末尾に改行なし()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed +
                       ".ノード２のタイトル" + lineFeed +
                       "ノード２の本文" + lineFeed + "ノード２の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + "ノード１の本文", result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);

            Assert.AreEqual("ノード２のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード２の本文" + lineFeed + "ノード２の本文", result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_ノード１つだけ()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed;
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed, result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_タイトルだけで終わる()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed;
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("", result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_タイトルだけで終わり改行がない()
        {
            string lineFeed = Environment.NewLine;
            var text = ".ノード１のタイトル";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("", result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_二段階の構造()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed + lineFeed +
                       "..ノード１aのタイトル" + lineFeed +
                       "ノード1aの本文" + lineFeed + "ノード１aの本文" + lineFeed + lineFeed +
                       "..ノード１bのタイトル" + lineFeed +
                       "ノード1bの本文" + lineFeed + "ノード１bの本文" + lineFeed + lineFeed +
                       ".ノード２のタイトル" + lineFeed +
                       "ノード２の本文" + lineFeed + "ノード２の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed, result.Node[0].Content);
            Assert.AreEqual(2, result.Node[0].Children.Count);

            Assert.AreEqual("ノード１aのタイトル", result.Node[0].Children[0].Name);
            Assert.AreEqual("ノード1aの本文" + lineFeed + "ノード１aの本文" + lineFeed, 
                            result.Node[0].Children[0].Content);
            Assert.AreEqual(0, result.Node[0].Children[0].Children.Count);

            Assert.AreEqual("ノード１bのタイトル", result.Node[0].Children[1].Name);
            Assert.AreEqual("ノード1bの本文" + lineFeed + "ノード１bの本文" + lineFeed, 
                            result.Node[0].Children[1].Content);
            Assert.AreEqual(0, result.Node[0].Children[1].Children.Count);

            Assert.AreEqual("ノード２のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード２の本文" + lineFeed + "ノード２の本文", result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_三段階の構造()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード1のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + "ノード1の本文" + lineFeed +
                       "..ノード1-1のタイトル" + lineFeed +
                       "ノード1-1の本文" + lineFeed + "ノード1-1の本文" + lineFeed +
                       "...ノード1-1-1のタイトル" + lineFeed +
                       "ノード1-1-1の本文" + lineFeed + "ノード1-1-1の本文";

            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);

            Assert.AreEqual("ノード1のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + "ノード1の本文", result.Node[0].Content);
            Assert.AreEqual(1, result.Node[0].Children.Count);

            Assert.AreEqual("ノード1-1のタイトル", result.Node[0].Children[0].Name);
            Assert.AreEqual("ノード1-1の本文" + lineFeed + "ノード1-1の本文",
                            result.Node[0].Children[0].Content);
            Assert.AreEqual(1, result.Node[0].Children[0].Children.Count);

            Assert.AreEqual("ノード1-1-1のタイトル", result.Node[0].Children[0].Children[0].Name);
            Assert.AreEqual("ノード1-1-1の本文" + lineFeed + "ノード1-1-1の本文",
                            result.Node[0].Children[0].Children[0].Content);
            Assert.AreEqual(0, result.Node[0].Children[0].Children[0].Children.Count);

        }

        [TestMethod]
        public void FromTreeText_子階層のタイトルの前に同階層のタイトル()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード1のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed +
                       "..ノード1-1のタイトル" + lineFeed +
                       "ノード1-1の本文" + lineFeed +
                       "..ノード1-2のタイトル" + lineFeed +
                       "ノード1-2の本文" + lineFeed +
                       "...ノード1-2-1のタイトル" + lineFeed +
                       "ノード1-2-1の本文" + lineFeed +
                       ".ノード2のタイトル" + lineFeed +
                       "ノード2の本文" + lineFeed + 
                       "..ノード2-1のタイトル" + lineFeed +
                       "ノード2-1の本文";

            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);

            Assert.AreEqual("ノード1のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文", result.Node[0].Content);
            Assert.AreEqual(2, result.Node[0].Children.Count);

            Assert.AreEqual("ノード1-1のタイトル", result.Node[0].Children[0].Name);
            Assert.AreEqual("ノード1-1の本文",
                            result.Node[0].Children[0].Content);
            Assert.AreEqual(0, result.Node[0].Children[0].Children.Count);

            Assert.AreEqual("ノード1-2のタイトル", result.Node[0].Children[1].Name);
            Assert.AreEqual("ノード1-2の本文",
                            result.Node[0].Children[1].Content);
            Assert.AreEqual(1, result.Node[0].Children[1].Children.Count);

            Assert.AreEqual("ノード1-2-1のタイトル", result.Node[0].Children[1].Children[0].Name);
            Assert.AreEqual("ノード1-2-1の本文",
                            result.Node[0].Children[1].Children[0].Content);
            Assert.AreEqual(0, result.Node[0].Children[1].Children[0].Children.Count);

            Assert.AreEqual("ノード2のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード2の本文", result.Node[1].Content);
            Assert.AreEqual(1, result.Node[1].Children.Count);

            Assert.AreEqual("ノード2-1のタイトル", result.Node[1].Children[0].Name);
            Assert.AreEqual("ノード2-1の本文",
                            result.Node[1].Children[0].Content);
            Assert.AreEqual(0, result.Node[1].Children[0].Children.Count);

        }

        [TestMethod]
        public void FromTreeText_改行コードがLF()
        {
            string lineFeed = "\n";
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + "ノード１の本文" + lineFeed + lineFeed +
                       ".ノード２のタイトル" + lineFeed +
                       "ノード２の本文" + lineFeed + "ノード２の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + Environment.NewLine + "ノード１の本文" + Environment.NewLine, result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);

            Assert.AreEqual("ノード２のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード２の本文" + Environment.NewLine + "ノード２の本文", result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_タイトルだけで次のノードが現れる()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed +
                       ".ノード２のタイトル" + lineFeed +
                       "ノード２の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("", result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);

            Assert.AreEqual("ノード２のタイトル", result.Node[1].Name);
            Assert.AreEqual("ノード２の本文", result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_行頭がピリオド()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ". .ノード1のタイトル" + lineFeed +
                       " .ノード1の本文" + lineFeed +
                       "ノード1の本文" + lineFeed +
                       " .ノード1の本文" + lineFeed +
                       ".. .ノード1-1のタイトル" + lineFeed +
                       " .ノード1-1の本文" + lineFeed +
                       ". ..ノード2のタイトル" + lineFeed +
                       " ..ノード2の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(2, result.Node.Count);

            Assert.AreEqual(".ノード1のタイトル", result.Node[0].Name);
            Assert.AreEqual(
                ".ノード1の本文" + lineFeed +
                "ノード1の本文" + lineFeed +
                ".ノード1の本文", 
                result.Node[0].Content);
            Assert.AreEqual(1, result.Node[0].Children.Count);

            Assert.AreEqual(".ノード1-1のタイトル", result.Node[0].Children[0].Name);
            Assert.AreEqual(".ノード1-1の本文", result.Node[0].Children[0].Content);
            Assert.AreEqual(0, result.Node[0].Children[0].Children.Count);

            Assert.AreEqual("..ノード2のタイトル", result.Node[1].Name);
            Assert.AreEqual("..ノード2の本文", result.Node[1].Content);
            Assert.AreEqual(0, result.Node[1].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_本文の文中にピリオド()
        {
            string lineFeed = Environment.NewLine;
            var text = ". .ノード1のタイトル" + lineFeed +
                       "ノード1の本文 .ノード1の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);

            Assert.AreEqual(".ノード1のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文 .ノード1の本文", result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_本文に空行あり()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ".ノード１のタイトル" + lineFeed +
                       "ノード1の本文" + lineFeed + lineFeed + lineFeed + "ノード１の本文" + lineFeed;
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);
            Assert.AreEqual("ノード１のタイトル", result.Node[0].Name);
            Assert.AreEqual("ノード1の本文" + lineFeed + lineFeed + lineFeed + "ノード１の本文" + lineFeed, result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);
        }

        [TestMethod]
        public void FromTreeText_行頭が空白()
        {
            string lineFeed = Environment.NewLine;
            var text = lineFeed + lineFeed +
                       ". ノード1のタイトル" + lineFeed +
                       " ノード1の本文";
            QuartetEditorDescription result;
            NodeConverterUtility.FromTreeText(text, '.', out result);

            Assert.AreEqual(1, result.Node.Count);

            Assert.AreEqual(" ノード1のタイトル", result.Node[0].Name);
            Assert.AreEqual(" ノード1の本文", result.Node[0].Content);
            Assert.AreEqual(0, result.Node[0].Children.Count);
        }
    }
}
