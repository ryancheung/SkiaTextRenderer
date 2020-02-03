using System.Collections.Generic;
using SkiaSharp;

namespace SkiaTextRenderer
{
    public class FontCache
    {
        private static Dictionary<int, FontCache> CacheStore = new Dictionary<int, FontCache>();

        private static int GetCacheKey(SKTypeface typeface, float fontSize)
        {
            return typeface.GetHashCode() + (int)(fontSize * 100f);
        }

        public static FontCache GetCache(SKTypeface typeface, float fontSize)
        {
            var cacheKey = GetCacheKey(typeface, fontSize);
            if (CacheStore.TryGetValue(cacheKey, out var fontCache))
                return fontCache;

            var newFontCache = new FontCache(typeface, fontSize);
            CacheStore[cacheKey] = newFontCache;

            return newFontCache;
        }

        public float FontAscender { get; }

        private Dictionary<char, FontLetterDefinition> _LetterDefinitions = new Dictionary<char, FontLetterDefinition>();
        private SKPaint _TextPaint = new SKPaint();

        public FontCache(SKTypeface typeface, float fontSize)
        {
            _TextPaint.Typeface = typeface;
            _TextPaint.TextSize = fontSize;

            FontAscender = -_TextPaint.FontMetrics.Top;
        }

        public bool GetLetterDefinitionForChar(char character, out FontLetterDefinition letterDefinition)
        {
            if (character == UnicodeCharacters.NoBreakSpace)
            {
                // change no-break space to regular space
                // reason: some fonts have issue with no-break space:
                //   * no letter definition
                //   * not normal big width
                character = UnicodeCharacters.Space;
            }

            if (_LetterDefinitions.TryGetValue(character, out letterDefinition))
                return letterDefinition.ValidDefinition;

            return false;
        }

        public void AddLetterDefinition(char character, FontLetterDefinition letterDefinition)
        {
            _LetterDefinitions[character] = letterDefinition;
        }

        public bool PrepareLetterDefinitions(string text)
        {
            List<char> newChars = new List<char>();
            FindNewCharacters(text, ref newChars);

            if (newChars.Count == 0)
                return false;

            var newCharString = new string(newChars.ToArray());

            var glyphs = _TextPaint.GetGlyphs(newCharString);
            var glyphWidths = _TextPaint.GetGlyphWidths(newCharString);

            for (int i = 0; i < glyphs.Length; i++)
            {
                FontLetterDefinition tempDef = new FontLetterDefinition();

                if (glyphs[i] == 0)
                {
                    _LetterDefinitions[newCharString[i]] = tempDef;
                }
                else
                {
                    tempDef.ValidDefinition = true;
                    tempDef.AdvanceX = glyphWidths[i];

                    _LetterDefinitions[newCharString[i]] = tempDef;
                }
            }

            return true;
        }

        private void FindNewCharacters(string text, ref List<char> chars)
        {
            if (_LetterDefinitions.Count == 0)
                chars.AddRange(text);
            else
            {
                foreach (var c in text)
                {
                    if (!_LetterDefinitions.ContainsKey(c))
                        chars.Add(c);
                }
            }
        }
    }
}