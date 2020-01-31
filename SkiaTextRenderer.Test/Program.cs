using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace SkiaTextRenderer.Test
{
    class Program
    {
        static SKTypeface Typeface;

        static Program()
        {
            Typeface = SKTypeface.FromFile(Path.Combine("Fonts", "simsun.ttf"));
        }

        static void TestDraw(string text, float fontSize, TextFormatFlags flags)
        {
            var font = new Font(Typeface, fontSize);

            var fileName = $"{text}-{fontSize}-{flags}.png";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            var size = TextRenderer.MeasureText(text, font, 0, flags);
            var BackColour = SKColors.Black;

            using (SKBitmap bitmap = new SKBitmap(size.Width, size.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(BackColour);

                TextRenderer.DrawText(canvas, text, font, new Rectangle(0, 0, size.Width, size.Height), SKColors.White, flags);

                using (Stream s = File.Open(fileName, FileMode.Create))
                {
                    SKData d = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                    d.SaveTo(s);
                }
            }

            Console.WriteLine("Drawing {0} (fontSize {1}, flags {2}), measured size: {3}", text, fontSize, flags, size);
        }

        static void Main(string[] args)
        {

            // Drawing A with Default
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.Default);
            // Drawing A with Horizontal Center
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.HorizontalCenter);
            // Drawing A with Vertical Center
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.VerticalCenter);
            // Drawing A without padding
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.NoPadding);
            // Drawing A with LeftAndRightPadding
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.LeftAndRightPadding);
        }
    }
}
