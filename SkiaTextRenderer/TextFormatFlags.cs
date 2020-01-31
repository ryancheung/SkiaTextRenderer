using System;

namespace SkiaTextRenderer
{
    [Flags]
    public enum TextFormatFlags
    {
        // Applies the default formatting, which is left-aligned.
        Default = 0,
        // Adds padding to the bounding rectangle to accommodate overhanging glyphs.
        GlyphOverhangPadding = 0,
        // Aligns the text on the left side of the clipping area.
        Left = 0,
        // Aligns the text on the top of the bounding rectangle.
        Top = 0,
        // Centers the text horizontally within the bounding rectangle.
        HorizontalCenter = 1,
        // Aligns the text on the right side of the clipping area.
        Right = 2,
        // Centers the text vertically, within the bounding rectangle.
        VerticalCenter = 4,
        // Aligns the text on the bottom of the bounding rectangle. Applied only when the text is a single line.
        Bottom = 8,
        // Breaks the text at the end of a word.
        WordBreak = 16,
        // Displays the text in a single line.
        SingleLine = 32,
        // Height of the bounding rectangle is changed automatically.
        ResizeHeight = 64,
        // Allows the overhanging parts of glyphs and unwrapped text reaching outside the formatting rectangle to show.
        NoClipping = 256,
        // Includes the font external leading in line height. Typically, external leading is not included in the height of a line of text.
        ExternalLeading = 512,
        // Trims the line to the nearest word and an ellipsis is placed at the end of a trimmed line.
        WordEllipsis = 262144,
        // Does not add padding to the bounding rectangle.
        NoPadding = 268435456,
        // Adds padding to both sides of the bounding rectangle.
        LeftAndRightPadding = 536870912
    }
}