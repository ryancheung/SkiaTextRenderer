using SkiaSharp;

namespace SkiaTextRenderer
{
    public enum FontStyle
    {
        Regular,
        Bold,
        Italic,
        Underline,
        Strikeout
    }

    public class Font
    {
        public SKTypeface Typeface { get; }
        public float Size { get; }
        public FontStyle Style { get; }

        public Font(SKTypeface typeface, float size, FontStyle style = FontStyle.Regular)
        {
            Typeface = typeface;
            Size = size;
            Style = style;
        }
    }
}
