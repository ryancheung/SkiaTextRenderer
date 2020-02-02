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

        static string CleanFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            fileName = fileName.Replace(' ', '-');
            fileName = fileName.Replace(',', '-');

            return fileName;
        }

        static void TestDraw(string text, float fontSize, TextFormatFlags flags)
        {
            var font = new Font(Typeface, fontSize);

            var fileName = CleanFileName($"{text}-{fontSize}-{flags}.png");

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

            var fileName = CleanFileName($"italic-{text}-{fontSize}-{flags}.png");
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

        static void TestDrawUnderline(string text, float fontSize, TextFormatFlags flags)
        {
            var font = new Font(Typeface, fontSize, FontStyle.Underline);

            var fileName = CleanFileName($"underline-{text}-{fontSize}-{flags}.png");
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

                TextRenderer.DrawText(canvas, text, font, new Rectangle(0, -1, size.Width, size.Height), SKColors.White, flags);

                using (Stream s = File.Open(fileName, FileMode.Create))
                {
                    SKData d = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                    d.SaveTo(s);
                }
            }

            Console.WriteLine("Drawing Underline {0} (fontSize {1}, flags {2}), measured size: {3}", text, fontSize, flags, size);
        }

        static void TestDrawStrikeThrough(string text, float fontSize, TextFormatFlags flags)
        {
            var font = new Font(Typeface, fontSize, FontStyle.Strikeout);

            var fileName = CleanFileName($"strikethrough-{text}-{fontSize}-{flags}.png");
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

            Console.WriteLine("Drawing StrikeThrough {0} (fontSize {1}, flags {2}), measured size: {3}", text, fontSize, flags, size);
        }

        static void TestDrawWithSize(string text, float fontSize, TextFormatFlags flags, Size size)
        {
            var font = new Font(Typeface, fontSize);

            var fileName = CleanFileName($"sized-{text}-{fontSize}-{flags}.png");
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

        static void TestDrawMultiline(string text, float fontSize, TextFormatFlags flags)
        {
            var font = new Font(Typeface, fontSize);

            var fileName = CleanFileName($"multiline-{text}-{fontSize}-{flags}.png");

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

            Console.WriteLine("Drawing multiline {0} (fontSize {1}, flags {2}), measured size: {3}", text, fontSize, flags, size);
        }

        static void Main(string[] args)
        {

            TestDraw("Hello 你好 world!", 20, TextFormatFlags.Default);
            TestDraw("Hello 你好 world!", 20, TextFormatFlags.HorizontalCenter);
            TestDraw("Hello 你好 world!", 20, TextFormatFlags.VerticalCenter);
            TestDraw("Hello 你好 world!", 20, TextFormatFlags.NoPadding);
            TestDraw("Hello 你好 world!", 20, TextFormatFlags.LeftAndRightPadding);

            TestDrawItalic("Hello 你好 world!", 20, TextFormatFlags.Default);
            TestDrawUnderline("Hello 你好 world!", 20, TextFormatFlags.Default);
            TestDrawStrikeThrough("Hello 你好 world!", 20, TextFormatFlags.Default);

            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.Default, new Size(200, 80));
            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.VerticalCenter, new Size(100, 80));
            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak, new Size(80, 80));
            TestDrawWithSize("Hello 你好 world!", 12, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak, new Size(80, 80));
            TestDrawWithSize("Hel\nl\r\no 你\n好 world!", 12, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine, new Size(80, 20));

            TestDrawMultiline("Hello 你\n好 world!", 20, TextFormatFlags.Default);
        }
    }
}
