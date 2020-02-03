using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Drawing;

namespace SkiaTextRenderer
{
    public class TextRenderer
    {
        private static readonly string[] NewLineCharacters = new[] { Environment.NewLine, UnicodeCharacters.NewLine.ToString(), UnicodeCharacters.CarriageReturn.ToString() };

        private static FontCache FontCache;
        private static readonly SKPaint TextPaint = new SKPaint();
        private static float LineHeight { get => TextPaint.TextSize; }
        private static FontStyle TextStyle;
        private static string Text;
        private static TextFormatFlags Flags;
        private static int? _CursorPosition;
        private static int? CursorPosition
        {
            get => _CursorPosition;
            set
            {
                if (value == null)
                {
                    _CursorPosition = null;
                    return;
                }

                if (value <= -1)
                    _CursorPosition = -1;
                else if (value >= Text.Length - 1)
                    _CursorPosition = Text.Length - 1;
                else
                    _CursorPosition = value;
            }
        }
        private static float MaxLineWidth;
        private static Rectangle Bounds = Rectangle.Empty;

        private static Size ContentSize = Size.Empty;
        private static float LeftPadding
        {
            get
            {
                if (Flags.HasFlag(TextFormatFlags.NoPadding))
                    return 0;

                if (Flags.HasFlag(TextFormatFlags.LeftAndRightPadding))
                    return (float)Math.Ceiling((TextPaint.FontSpacing / 6.0) * 2.0);

                if (Flags.HasFlag(TextFormatFlags.GlyphOverhangPadding))
                    return (float)Math.Ceiling(TextPaint.FontSpacing / 6.0);

                return 0;
            }
        }
        private static float RightPadding
        {
            get
            {
                if (Flags.HasFlag(TextFormatFlags.NoPadding))
                    return 0;

                if (Flags.HasFlag(TextFormatFlags.LeftAndRightPadding))
                    return (float)Math.Ceiling((TextPaint.FontSpacing / 6.0) * 2.5);

                if (Flags.HasFlag(TextFormatFlags.GlyphOverhangPadding))
                    return (float)Math.Ceiling((TextPaint.FontSpacing / 6.0) * 1.5);

                return 0;
            }
        }

        private static bool EnableWrap { get => (Flags & (TextFormatFlags.NoClipping | TextFormatFlags.SingleLine)) == 0; }
        private static bool LineBreakWithoutSpaces { get => (Flags & TextFormatFlags.WordBreak) == 0; }

        private static int NumberOfLines;
        private static int TextDesiredHeight;
        private static List<float> LinesWidth = new List<float>();
        private static List<float> LinesOffsetX = new List<float>();
        private static int LetterOffsetY;

        class LetterInfo
        {
            public char Character;
            public bool Valid;
            public float PositionX;
            public float PositionY;
            public int LineIndex;
        }
        private static List<LetterInfo> LettersInfo = new List<LetterInfo>();

        private delegate int GetFirstCharOrWordLength(string textLine, int startIndex);

        private static void PrepareTextPaint(Font font)
        {
            FontCache = FontCache.GetCache(font.Typeface, font.Size);

            TextPaint.IsStroke = false;
            TextPaint.HintingLevel = SKPaintHinting.Normal;
            TextPaint.IsAutohinted = true; // Only for freetype
            TextPaint.IsEmbeddedBitmapText = true;
            TextPaint.DeviceKerningEnabled = true;

            TextPaint.Typeface = font.Typeface;
            TextPaint.TextSize = font.Size;

            if (font.Style == FontStyle.Italic)
                TextPaint.TextSkewX = -0.4f;
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

                if (character == UnicodeCharacters.NewLine || character == UnicodeCharacters.CarriageReturn
                    || (!Utils.IsUnicodeNonBreaking(character) && (Utils.IsUnicodeSpace(character) || Utils.IsCJKUnicode(character))))
                {
                    break;
                }

                if (!FontCache.GetLetterDefinitionForChar(character, out var letterDef))
                {
                    break;
                }

                if (MaxLineWidth > 0)
                {
                    if (nextLetterX + letterDef.AdvanceX > MaxLineWidth)
                        break;
                }

                nextLetterX += letterDef.AdvanceX;

                length++;
            }

            if (length == 0 && textLine.Length > 0)
                length = 1;

            return length;
        }
        private static void RecordLetterInfo(SKPoint point, char character, int letterIndex, int lineIndex)
        {
            if (letterIndex >= LettersInfo.Count)
            {
                LettersInfo.Add(new LetterInfo());
            }

            LettersInfo[letterIndex].LineIndex = lineIndex;
            LettersInfo[letterIndex].Character = character;
            LettersInfo[letterIndex].Valid = FontCache.GetLetterDefinitionForChar(character, out var letterDef) && letterDef.ValidDefinition;
            LettersInfo[letterIndex].PositionX = point.X;
            LettersInfo[letterIndex].PositionY = point.Y;
        }
        private static void RecordPlaceholderInfo(int letterIndex, char character)
        {
            if (letterIndex >= LettersInfo.Count)
            {
                LettersInfo.Add(new LetterInfo());
            }

            LettersInfo[letterIndex].Character = character;
            LettersInfo[letterIndex].Valid = false;
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
            int textLength = Text.Length;
            int lineIndex = 0;
            float nextTokenX = 0;
            float nextTokenY = 0;
            float longestLine = 0;
            float letterRight = 0;
            float nextWhitespaceWidth = 0;
            FontLetterDefinition letterDef;
            SKPoint letterPosition = new SKPoint();
            bool nextChangeSize = true;

            for (int index = 0; index < textLength;)
            {
                char character = Text[index];
                if (character == UnicodeCharacters.NewLine)
                {
                    if (!Flags.HasFlag(TextFormatFlags.SingleLine))
                    {
                        LinesWidth.Add(letterRight);
                        letterRight = 0;
                        lineIndex++;
                        nextTokenX = 0;
                        nextTokenY += LineHeight;
                    }

                    RecordPlaceholderInfo(index, character);
                    index++;
                    continue;
                }

                var tokenLen = nextTokenLength(Text, index);
                float tokenRight = letterRight;
                float nextLetterX = nextTokenX;
                float whitespaceWidth = nextWhitespaceWidth;
                bool newLine = false;
                for (int tmp = 0; tmp < tokenLen; ++tmp)
                {
                    int letterIndex = index + tmp;
                    character = Text[letterIndex];
                    if (character == UnicodeCharacters.CarriageReturn)
                    {
                        RecordPlaceholderInfo(letterIndex, character);
                        continue;
                    }

                    // \b - Next char not change x position
                    if (character == UnicodeCharacters.NextCharNoChangeX)
                    {
                        nextChangeSize = false;
                        RecordPlaceholderInfo(letterIndex, character);
                        continue;
                    }

                    if (!FontCache.GetLetterDefinitionForChar(character, out letterDef))
                    {
                        RecordPlaceholderInfo(letterIndex, character);
                        Console.WriteLine($"TextRenderer.MultilineTextWrap error: can't find letter definition in font file for letter: {character}");
                        continue;
                    }

                    if (EnableWrap && MaxLineWidth > 0 && nextTokenX > 0 && nextLetterX + letterDef.AdvanceX > MaxLineWidth
                        && !Utils.IsUnicodeSpace(character) && nextChangeSize)
                    {
                        LinesWidth.Add(letterRight - whitespaceWidth);
                        nextWhitespaceWidth = 0f;
                        letterRight = 0f;
                        lineIndex++;
                        nextTokenX = 0f;
                        nextTokenY += LineHeight;
                        newLine = true;
                        break;
                    }
                    else
                    {
                        letterPosition.X = nextLetterX;
                    }

                    letterPosition.Y = nextTokenY;
                    RecordLetterInfo(letterPosition, character, letterIndex, lineIndex);

                    if (nextChangeSize)
                    {
                        var newLetterWidth = letterDef.AdvanceX;

                        nextLetterX += newLetterWidth;
                        tokenRight = nextLetterX;

                        if (Utils.IsUnicodeSpace(character))
                        {
                            nextWhitespaceWidth += newLetterWidth;
                        }
                        else
                        {
                            nextWhitespaceWidth = 0;
                        }
                    }

                    nextChangeSize = true;
                }

                if (newLine)
                {
                    continue;
                }

                nextTokenX = nextLetterX;
                letterRight = tokenRight;

                index += tokenLen;
            }

            if (LinesWidth.Count == 0)
            {
                LinesWidth.Add(letterRight);
                longestLine = letterRight;
            }
            else
            {
                LinesWidth.Add(letterRight - nextWhitespaceWidth);
                foreach (var lineWidth in LinesWidth)
                {
                    if (longestLine < lineWidth)
                        longestLine = lineWidth;
                }
            }

            NumberOfLines = lineIndex + 1;
            TextDesiredHeight = (int)(NumberOfLines * LineHeight);

            ContentSize.Width = (int)(longestLine + LeftPadding + RightPadding);
            ContentSize.Height = TextDesiredHeight;
        }

        private static void ComputeAlignmentOffset()
        {
            LinesOffsetX.Clear();

            if (Flags.HasFlag(TextFormatFlags.HorizontalCenter))
            {
                foreach (var lineWidth in LinesWidth)
                    LinesOffsetX.Add((Bounds.Width - lineWidth) / 2f);
            }
            else if (Flags.HasFlag(TextFormatFlags.Right))
            {
                foreach (var lineWidth in LinesWidth)
                    LinesOffsetX.Add(Bounds.Width - lineWidth);
            }
            else
            {
                for (int i = 0; i < NumberOfLines; i++)
                    LinesOffsetX.Add(0);
            }

            if (Flags.HasFlag(TextFormatFlags.VerticalCenter))
            {
                LetterOffsetY = (Bounds.Height - TextDesiredHeight) / 2;
            }
            else if (Flags.HasFlag(TextFormatFlags.Bottom))
            {
                LetterOffsetY = Bounds.Height - TextDesiredHeight;
            }
            else
            {
                LetterOffsetY = 0;
            }
        }

        private static void ComputeLetterPositionInBounds(ref Rectangle bounds)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                var letterInfo = LettersInfo[i];

                if (!letterInfo.Valid)
                    continue;

                var posX = letterInfo.PositionX + LinesOffsetX[letterInfo.LineIndex] + bounds.X;
                if (!Flags.HasFlag(TextFormatFlags.HorizontalCenter))
                    posX += LeftPadding;

                var posY = letterInfo.PositionY + LetterOffsetY + bounds.Y;
                if (Flags.HasFlag(TextFormatFlags.ExternalLeading))
                    posY += TextPaint.FontMetrics.Leading;

                letterInfo.PositionX = posX;
                letterInfo.PositionY = posY;
            }
        }

        private static void AlignText()
        {
            if (string.IsNullOrEmpty(Text))
            {
                ContentSize = Size.Empty;
                return;
            }

            FontCache.PrepareLetterDefinitions(Text);

            TextDesiredHeight = 0;
            LinesWidth.Clear();

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

        private static HashSet<int> LinesHadDrawedUnderlines = new HashSet<int>();

        private static void DrawCursorForEmptyString(SKCanvas canvas, Font font, ref SKColor foreColor)
        {
            TextPaint.TextSize = font.Size;
            TextPaint.Color = foreColor;
            canvas.DrawLine(new SKPoint(0, 0), new SKPoint(0, LineHeight), TextPaint);
        }

        private static void DrawCursor(SKCanvas canvas)
        {
            var pos1 = new SKPoint();
            var pos2 = new SKPoint();

            if (CursorPosition == Text.Length - 1)
            {
                var letterInfo = LettersInfo[CursorPosition.Value];
                FontLetterDefinition letterDef;
                FontCache.GetLetterDefinitionForChar(letterInfo.Character, out letterDef);

                pos1.X = letterInfo.PositionX + letterDef.AdvanceX;
                pos1.Y = letterInfo.PositionY;
                pos2.X = pos1.X;
                pos2.Y = letterInfo.PositionY + LineHeight;
            }
            else
            {
                var letterInfo = LettersInfo[CursorPosition.Value + 1];
                pos1.X = letterInfo.PositionX;
                pos1.Y = letterInfo.PositionY;
                pos2.X = pos1.X;
                pos2.Y = letterInfo.PositionY + LineHeight;
            }

            canvas.DrawLine(pos1, pos2, TextPaint);
        }

        private static void DrawToCanvas(SKCanvas canvas, ref SKColor foreColor)
        {
            TextPaint.Color = foreColor;

            SKPoint[] glyphPositions = new SKPoint[Text.Length];

            if (TextStyle == FontStyle.Underline || TextStyle == FontStyle.Strikeout)
                LinesHadDrawedUnderlines.Clear();

            for (int i = 0; i < Text.Length; i++)
            {
                var letterInfo = LettersInfo[i];

                if (!letterInfo.Valid)
                    continue;

                // X and Y coordinates passed to the DrawText method specify the left side of the text at the baseline.
                // So we need move Y with a ascender.
                var realPosY = letterInfo.PositionY + FontCache.FontAscender;
                glyphPositions[i] = new SKPoint(letterInfo.PositionX, realPosY);

                if (TextStyle == FontStyle.Underline || TextStyle == FontStyle.Strikeout)
                {
                    if (LinesHadDrawedUnderlines.Contains(letterInfo.LineIndex))
                        continue;

                    realPosY += TextStyle == FontStyle.Underline ? (TextPaint.FontMetrics.UnderlinePosition ?? 0) : (TextPaint.FontMetrics.StrikeoutPosition ?? 0);
                    canvas.DrawLine(new SKPoint(letterInfo.PositionX, realPosY), new SKPoint(letterInfo.PositionX + LinesWidth[letterInfo.LineIndex], realPosY), TextPaint);

                    LinesHadDrawedUnderlines.Add(letterInfo.LineIndex);
                }
            }

            canvas.DrawPositionedText(Text, glyphPositions, TextPaint);

            if (CursorPosition.HasValue)
                DrawCursor(canvas);
        }

        public static void DrawText(SKCanvas canvas, string text, Font font, Rectangle bounds, SKColor foreColor, TextFormatFlags flags, int? cursorPosition = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (cursorPosition != null)
                    DrawCursorForEmptyString(canvas, font, ref foreColor);

                return;
            }

            Text = text;
            Flags = flags;
            PrepareTextPaint(font);
            MaxLineWidth = bounds.Width - LeftPadding - RightPadding;
            Bounds = bounds;
            CursorPosition = cursorPosition;

            AlignText();

            ComputeAlignmentOffset();
            ComputeLetterPositionInBounds(ref bounds);

            DrawToCanvas(canvas, ref foreColor);
        }

        public static int GetCursorFromPoint(string text, Font font, Rectangle bounds, TextFormatFlags flags, SKPoint point)
        {
            if (string.IsNullOrEmpty(text))
                return -1;

            Text = text;
            Flags = flags;
            PrepareTextPaint(font);
            MaxLineWidth = bounds.Width - LeftPadding - RightPadding;
            Bounds = bounds;

            AlignText();

            ComputeAlignmentOffset();
            ComputeLetterPositionInBounds(ref bounds);

            int lineIndex = (int)(point.Y / LineHeight);

            for (int i = 0; i < Text.Length; i++)
            {
                var letterInfo = LettersInfo[i];
                if (letterInfo.LineIndex != lineIndex || !letterInfo.Valid)
                    continue;

                FontLetterDefinition letterDef;
                FontCache.GetLetterDefinitionForChar(letterInfo.Character, out letterDef);

                // Click the left side of the first character
                if (point.X <= letterInfo.PositionX)
                    return -1;

                if (point.X <= letterInfo.PositionX + letterDef.AdvanceX)
                {
                    if (point.X <= letterInfo.PositionX + letterDef.AdvanceX / 2)
                        return i - 1;
                    else
                        return i;
                }
                else
                {
                    if (i < Text.Length - 1)
                        continue;
                    else
                        return i;
                }
            }

            return -1;
        }
    }
}
