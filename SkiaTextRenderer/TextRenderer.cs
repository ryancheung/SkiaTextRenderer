using System;
using System.Drawing;
using SkiaSharp;

namespace SkiaTextRenderer
{
    [Obsolete("Use " + nameof(TextRendererSk) + " instead")]
    public static class TextRenderer
    {
        public static Size MeasureText(string text, Font font)
        {
            return ToSize(TextRendererSk.MeasureText(text, font));
        }

        public static Size MeasureText(string text, Font font, int maxLineWidth)
        {
            return ToSize(TextRendererSk.MeasureText(text, font, maxLineWidth));
        }

        public static Size MeasureText(string text, Font font, float maxLineWidth, TextFormatFlags flags)
        {
            return ToSize(TextRendererSk.MeasureText(text, font, maxLineWidth, flags));
        }

        public static void DrawText(SKCanvas canvas, string text, Font font, Rectangle bounds, SKColor foreColor,
            TextFormatFlags flags, TextPaintOptions options)
        {
            TextRendererSk.DrawText(canvas, text, font, ToSkRect(bounds), foreColor, flags, options);
        }

        public static void DrawText(SKCanvas canvas, string text, Font font, Rectangle bounds, SKColor foreColor,
            TextFormatFlags flags)
        {
            TextRendererSk.DrawText(canvas, text, font, ToSkRect(bounds), foreColor, flags);
        }

        public static void DrawText(SKCanvas canvas, string text, Font font, Rectangle bounds, SKColor foreColor,
            TextFormatFlags flags, int cursorPosition)
        {
            TextRendererSk.DrawText(canvas, text, font, ToSkRect(bounds), foreColor, flags, cursorPosition);
        }

        public static SKPoint GetCursorDrawPosition(string text, Font font, Rectangle bounds, TextFormatFlags flags,
            int cursorPosition)
        {
            return TextRendererSk.GetCursorDrawPosition(text, font, ToSkRect(bounds), flags, cursorPosition);
        }

        public static int GetCursorFromPoint(string text, Font font, Rectangle bounds, TextFormatFlags flags,
            SKPoint point)
        {
            return TextRendererSk.GetCursorFromPoint(text, font, ToSkRect(bounds), flags, point);
        }

        public static int GetCursorFromPoint(string text, Font font, Rectangle bounds, TextFormatFlags flags,
            SKPoint point, out SKPoint cursorDrawPosition)
        {
            return TextRendererSk.GetCursorFromPoint(text, font, ToSkRect(bounds), flags, point,
                out cursorDrawPosition);
        }

        private static Size ToSize(SKSize skSize)
        {
            return new Size((int) skSize.Width, (int) skSize.Height);
        }

        private static SKRect ToSkRect(Rectangle bounds)
        {
            return SKRect.Create(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
    }
}
