using System;
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

        static void Main(string[] args)
        {
            var font = new Font(Typeface, 12);

            var a = TextRenderer.MeasureText("A", font);

            Console.WriteLine("Measured text size: {0}", a);
        }
    }
}
