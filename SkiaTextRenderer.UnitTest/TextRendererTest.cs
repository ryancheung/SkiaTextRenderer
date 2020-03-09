using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System.IO;
using System.Drawing;

namespace SkiaTextRenderer.UnitTest
{
    [TestClass]
    public class TextRendererTest
    {
        static SKTypeface Typeface;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Typeface = SKTypeface.FromFile(Path.Combine("simsun.ttf"));
        }

        [TestMethod]
        public void TestGetCursorFromPoint()
        {
            var font = new Font(Typeface, 20);
            var flags = TextFormatFlags.Default;
            var text = "Hello 你好 world!";
            var size = TextRenderer.MeasureText(text, font, 0, flags);

            var point = new SKPoint(0, 0);
            var cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(0, cursor);

            point = new SKPoint(6, 10);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(0, cursor);

            point = new SKPoint(12, 17);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(1, cursor);

            point = new SKPoint(23, 17);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(2, cursor);

            point = new SKPoint(81, 9);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(7, cursor);

            point = new SKPoint(136, 14);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(11, cursor);

            point = new SKPoint(141, 11);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(12, cursor);

            point = new SKPoint(170, 16);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(15, cursor);

            // Click on the right of last character
            point = new SKPoint(178, 13);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(15, cursor);

            point = new SKPoint(11, -1);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(1, cursor);

            point = new SKPoint(181, 12);
            cursor = TextRenderer.GetCursorFromPoint(text, font, SKRect.Create(0, 0, size.Width, size.Height), flags, point);
            Assert.AreEqual(15, cursor);
        }
    }
}
