using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Drawing;

namespace SkiaTextRenderer
{
    public class TextRenderer
    {
        private static readonly string[] NewLineCharacters = new[] { Environment.NewLine, UnicodeCharacters.NewLine.ToString(), UnicodeCharacters.CarriageReturn.ToString() };

        private static readonly SKPaint TextPaint = new SKPaint();
        private static SKTypeface Typeface;
        private static FontStyle TextStyle;
        private static string Text;
        private static TextFormatFlags Flags;
        private static float MaxLineWidth;
        private static Rectangle Bounds = Rectangle.Empty;

        private static Size ContentSize = Size.Empty;
        private static float LeftPadding
        {
            get
            {
                switch (Flags)
                {
                    case TextFormatFlags.NoPadding:
                        return 0;
                    case TextFormatFlags.GlyphOverhangPadding:
                        return (float)Math.Ceiling(TextPaint.FontSpacing / 6.0);
                    case TextFormatFlags.LeftAndRightPadding:
                        return (float)Math.Ceiling((TextPaint.FontSpacing / 6.0) * 2.0);
                    default:
                        return 0;
                }
            }
        }
        private static float RightPadding
        {
            get
            {
                switch (Flags)
                {
                    case TextFormatFlags.NoPadding:
                        return 0;
                    case TextFormatFlags.GlyphOverhangPadding:
                        return (float)Math.Ceiling((TextPaint.FontSpacing / 6.0) * 1.5);
                    case TextFormatFlags.LeftAndRightPadding:
                        return (float)Math.Ceiling((TextPaint.FontSpacing / 6.0) * 2.5);
                    default:
                        return 0;
                }
            }
        }

        private static bool EnableWrap { get => (Flags & TextFormatFlags.NoClipping) == 0; }
        private static bool LineBreakWithoutSpaces { get => (Flags & TextFormatFlags.WordBreak) == 0; }

        class TextLine
        {
            public TextLine(string text, int width)
            {
                Text = text;
                Width = width;
                OffsetX = 0;
            }
            public string Text;
            public int Width;
            public int OffsetX;
        }

        private static int TextDesiredHeight;
        private static float LongestLineWidth;
        private static List<TextLine> TextLines = new List<TextLine>();
        private static int LetterOffsetY;

        private delegate int GetFirstCharOrWordLength(string textLine, int startIndex);

        private static void PrepareTextPaint(Font font)
        {
            TextPaint.IsStroke = false;
            TextPaint.HintingLevel = SKPaintHinting.Normal;
            TextPaint.IsAutohinted = true; // Only for freetype
            TextPaint.IsEmbeddedBitmapText = true;
            TextPaint.DeviceKerningEnabled = true;

            Typeface = TextPaint.Typeface = font.Typeface;
            TextPaint.TextSize = font.Size;

            if (font.Style == FontStyle.Italic)
                TextPaint.TextSkewX = -0.5f;
            else
                TextPaint.TextSkewX = 0;

            TextStyle = font.Style;
        }
        private static int GetFirstCharLength(string textLine, int startIndex)
        {
            return 1;
        }

        private static int GetFirstWordLength(string textLine, int startIndex)
        {
            int length = 0;
            float nextLetterX = 0;

            for (int index = startIndex; index < textLine.Length; ++index)
            {
                var character = textLine[index];

                length++;

                if (character == UnicodeCharacters.NewLine || character == UnicodeCharacters.CarriageReturn
                    || (!Utils.IsUnicodeNonBreaking(character) && (Utils.IsUnicodeSpace(character) || Utils.IsCJKUnicode(character))))
                {
                    break;
                }

                if (!TextPaint.ContainsGlyphs(character.ToString()))
                {
                    break;
                }

                var letterRect = new SKRect();
                TextPaint.MeasureText(character.ToString(), ref letterRect);
                var letterWidth = letterRect.Width;

                if (MaxLineWidth > 0)
                {
                    if ((nextLetterX + letterWidth) > MaxLineWidth)
                        break;
                }

                nextLetterX += letterWidth;
            }

            return length;
        }
        private static void MultilineTextWrapByWord()
        {
            MultilineTextWrap(GetFirstWordLength);
        }
        private static void MultilineTextWrapByChar()
        {
            MultilineTextWrap(GetFirstCharLength);
        }
        private static void MultilineTextWrap(GetFirstCharOrWordLength nextTokenLength)
        {
            var lines = Text.Split(NewLineCharacters, StringSplitOptions.None);

            var rect = new SKRect();

            if (MaxLineWidth <= 0)
            {
                foreach (var line in lines)
                {
                    TextPaint.MeasureText(line, ref rect);
                    TextLines.Add(new TextLine(line, (int)rect.Width));
                    if (LongestLineWidth < rect.Width)
                        LongestLineWidth = rect.Width;
                }

                TextDesiredHeight = (int)(TextLines.Count * TextPaint.FontSpacing);

                ContentSize.Width = (int)(LongestLineWidth + LeftPadding + RightPadding);
                ContentSize.Height = TextDesiredHeight;

                return;
            }

            List<string> newLines;

            if (EnableWrap)
            {
                newLines = new List<string>();
                for (int i = 0; i < lines.Length; i++)
                {
                    var textLine = lines[i];
                    int lineBreakIndex = 0;
                    int nextTokenStartIndex = 0;
                    int currentLineLength = 0;
                    int measuredLength = 0;

                    while (true)
                    {
                        if (string.IsNullOrEmpty(textLine))
                        {
                            // Draw a blank space for empty newline
                            newLines.Add(" ");
                            break;
                        }

                        currentLineLength += nextTokenLength(textLine, nextTokenStartIndex);
                        var tempNewLineText = textLine.Substring(lineBreakIndex, currentLineLength);
                        measuredLength = (int)TextPaint.BreakText(tempNewLineText, MaxLineWidth);

                        if (measuredLength == currentLineLength)
                        {
                            nextTokenStartIndex = lineBreakIndex + currentLineLength;

                            if (nextTokenStartIndex < textLine.Length)
                                continue;
                            else
                            {
                                newLines.Add(tempNewLineText);
                                break;
                            }
                        }
                        else
                        {
                            if (lineBreakIndex == 0 && nextTokenStartIndex == 0) // The first token length is greater than MaxLineWidth
                            {
                                if (measuredLength == 0) // The first token length is greater than MaxLineWidth
                                    measuredLength = 1;

                                newLines.Add(textLine.Substring(lineBreakIndex, measuredLength));

                                if (textLine.Length == measuredLength)
                                    break;

                                textLine = textLine.Substring(measuredLength);
                                lineBreakIndex = 0;
                                nextTokenStartIndex = 0;
                                currentLineLength = 0;
                            }
                            else
                            {
                                var previousNewLineText = textLine.Substring(lineBreakIndex, nextTokenStartIndex - lineBreakIndex);
                                newLines.Add(previousNewLineText);
                                textLine = textLine.Substring(nextTokenStartIndex);
                                lineBreakIndex = 0;
                                nextTokenStartIndex = 0;
                                currentLineLength = 0;
                            }
                        }
                    }
                }
            }
            else
                newLines = new List<string>(lines);

            foreach (var line in newLines)
            {
                TextPaint.MeasureText(line, ref rect);
                TextLines.Add(new TextLine(line, (int)rect.Width));
                if (LongestLineWidth < rect.Width)
                    LongestLineWidth = rect.Width;
            }

            TextDesiredHeight = (int)(TextLines.Count * TextPaint.FontSpacing);

            ContentSize.Width = (int)(LongestLineWidth + LeftPadding + RightPadding);
            ContentSize.Height = TextDesiredHeight;
        }

        private static void ComputeAlignmentOffset()
        {
            switch (Flags)
            {
                case TextFormatFlags.HorizontalCenter:
                    foreach (var line in TextLines)
                        line.OffsetX = (Bounds.Width - line.Width) / 2;
                    break;
                case TextFormatFlags.Right:
                    foreach (var line in TextLines)
                    {
                        line.OffsetX = Bounds.Width - line.Width;
                    }
                    break;
                case TextFormatFlags.Left:
                default:
                    foreach (var line in TextLines)
                        line.OffsetX = 0;
                    break;
            }

            switch (Flags)
            {
                case TextFormatFlags.VerticalCenter:
                    LetterOffsetY = (Bounds.Height - TextDesiredHeight) / 2;
                    break;
                case TextFormatFlags.Bottom:
                    LetterOffsetY = Bounds.Height - TextDesiredHeight;
                    break;
                case TextFormatFlags.Top:
                default:
                    LetterOffsetY = 0;
                    break;
            }
        }
        private static void AlignText()
        {
            if (string.IsNullOrEmpty(Text))
            {
                ContentSize = Size.Empty;
                return;
            }

            TextDesiredHeight = 0;
            TextLines.Clear();
            LongestLineWidth = 0;

            if (MaxLineWidth > 0 && !LineBreakWithoutSpaces)
                MultilineTextWrapByWord();
            else
                MultilineTextWrapByChar();
        }

        public static Size MeasureText(string text, Font font)
        {
            return MeasureText(text, font, 0, TextFormatFlags.Default);
        }
        public static Size MeasureText(string text, Font font, int maxLineWidth)
        {
            return MeasureText(text, font, maxLineWidth, TextFormatFlags.Default);
        }

        public static Size MeasureText(string text, Font font, float maxLineWidth, TextFormatFlags flags)
        {
            Text = text;
            Flags = flags;
            MaxLineWidth = maxLineWidth;

            PrepareTextPaint(font);

            AlignText();

            return ContentSize;
        }

        public static void DrawText(SKCanvas canvas, string text, Font font, Rectangle bounds, SKColor foreColor, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Bounds = bounds;
            Text = text;
            Flags = flags;
            MaxLineWidth = bounds.Width;

            PrepareTextPaint(font);

            AlignText();
            ComputeAlignmentOffset();

            TextPaint.Color = foreColor;

            var pos = new SKPoint();
            int lineIndex = 0;
            foreach (var line in TextLines)
            {
                pos.X = line.OffsetX + LeftPadding + bounds.X;
                // The X and Y coordinates passed to the DrawText method specify the left side of the text at the baseline.
                pos.Y = bounds.Y;
                pos.Y -= TextPaint.FontMetrics.Top;
                pos.Y += LetterOffsetY + lineIndex * TextPaint.FontSpacing;

                canvas.DrawText(line.Text, pos, TextPaint);

                if (TextStyle == FontStyle.Underline)
                {
                    pos.Y += TextPaint.FontMetrics.UnderlinePosition ?? 0;
                    canvas.DrawLine(new SKPoint(pos.X, pos.Y), new SKPoint(pos.X + line.Width, pos.Y), TextPaint);
                }

                lineIndex++;
            }
        }
    }
}