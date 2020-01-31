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

        static void TestDrawItalic(string text, float fontSize, TextFormatFlags flags)
        {
            var font = new Font(Typeface, fontSize, FontStyle.Italic);

            var fileName = $"italic-{text}-{fontSize}-{flags}.png";
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

            Console.WriteLine("Drawing Italic {0} (fontSize {1}, flags {2}), measured size: {3}", text, fontSize, flags, size);
        }

        static void TestDrawWithSize(string text, float fontSize, TextFormatFlags flags, Size size)
        {
            var font = new Font(Typeface, fontSize);

            var fileName = $"sized-{text}-{fontSize}-{flags}.png";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

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

            TestDraw("Hello 你好 world!", 12, TextFormatFlags.Default);
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.HorizontalCenter);
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.VerticalCenter);
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.NoPadding);
            TestDraw("Hello 你好 world!", 12, TextFormatFlags.LeftAndRightPadding);

            TestDrawItalic("Hello 你好 world!", 12, TextFormatFlags.Default);

            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.Default, new Size(100, 80));
            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.VerticalCenter, new Size(100, 80));
            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak, new Size(80, 80));
            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak, new Size(80, 80));
        }
    }
}
