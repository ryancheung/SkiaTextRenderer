namespace SkiaTextRenderer
{
    public class UnicodeCharacters
    {
        public const char NewLine = '\n'; // 10
        public const char CarriageReturn = '\r'; // 13
        public const char NextCharNoChangeX = '\b'; // 8
        public const char Space = ' '; // 32
        public const char NoBreakSpace = (char)0x00A0; // 160
    }

    public class Utils
    {
        /*
        * @ch is the unicode character whitespace?
        *
        * Reference: http://en.wikipedia.org/wiki/Whitespace_character#Unicode
        *
        * Return value: weather the character is a whitespace character.
        * */
        public static bool IsUnicodeSpace(char ch)
        {
            return (ch >= 0x0009 && ch <= 0x000D) || ch == 0x0020 || ch == 0x0085 || ch == 0x00A0 || ch == 0x1680
            || (ch >= 0x2000 && ch <= 0x200A) || ch == 0x2028 || ch == 0x2029 || ch == 0x202F
            || ch == 0x205F || ch == 0x3000;
        }

        public static bool IsCJKUnicode(char ch)
        {
            return (ch >= 0x4E00 && ch <= 0x9FBF)   // CJK Unified Ideographs
                || (ch >= 0x2E80 && ch <= 0x2FDF)   // CJK Radicals Supplement & Kangxi Radicals
                || (ch >= 0x2FF0 && ch <= 0x30FF)   // Ideographic Description Characters, CJK Symbols and Punctuation & Japanese
                || (ch >= 0x3100 && ch <= 0x31BF)   // Korean
                || (ch >= 0xAC00 && ch <= 0xD7AF)   // Hangul Syllables
                || (ch >= 0xF900 && ch <= 0xFAFF)   // CJK Compatibility Ideographs
                || (ch >= 0xFE30 && ch <= 0xFE4F)   // CJK Compatibility Forms
                || (ch >= 0x31C0 && ch <= 0x4DFF)   // Other extensions
                || (ch >= 0x1f004 && ch <= 0x1f682);// Emoji
        }

        public static bool IsUnicodeNonBreaking(char ch)
        {
            return ch == 0x00A0   // Non-Breaking Space
            || ch == 0x202F       // Narrow Non-Breaking Space
            || ch == 0x2007       // Figure Space
            || ch == 0x2060;      // Word Joiner
        }
    }
}