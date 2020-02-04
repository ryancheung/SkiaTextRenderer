using SkiaSharp;

namespace SkiaTextRenderer
{
    public class TextPaintOptions
    {
        private int? _SelectionStart;
        public int? SelectionStart
        {
            get => _SelectionStart;
            set
            {
                if (_SelectionStart == value)
                    return;

                _SelectionStart = value;
                EnsureSafeOptionValue(ref _SelectionStart);
            }
        }

        private int? _SelectionEnd;
        public int? SelectionEnd
        {
            get => _SelectionEnd;
            set
            {
                if (_SelectionEnd == value)
                    return;

                _SelectionEnd = value;
                EnsureSafeOptionValue(ref _SelectionEnd);
            }
        }

        public SKColor SelectionColor;

        private int? _CursorPosition;
        public int? CursorPosition
        {
            get => _CursorPosition;
            set
            {
                if (_CursorPosition == value)
                    return;

                _CursorPosition = value;
                EnsureSafeOptionValue(ref _CursorPosition);
            }
        }

        public TextPaintOptions()
        {
            _SelectionStart = null;
            _SelectionEnd = null;
            SelectionColor = new SKColor(0, 120, 215); // Windows textbox selection color

            _CursorPosition = null;

            EnsureSafeOptionValuesForText();
        }

        private void EnsureSafeOptionValue(ref int? optionValue, string text = null)
        {
            if (optionValue == null) return;

            if (optionValue < -1)
                optionValue = -1;
            else if (text != null && optionValue > text.Length - 1)
                optionValue = text.Length - 1;
        }

        public void EnsureSafeOptionValuesForText(string text = null)
        {
            EnsureSafeOptionValue(ref _CursorPosition, text);
            EnsureSafeOptionValue(ref _SelectionStart, text);
            EnsureSafeOptionValue(ref _SelectionEnd, text);

            if (_SelectionStart != null && _SelectionEnd != null)
            {
                if (_SelectionStart > _SelectionEnd)
                {
                    var tmp = _SelectionStart;
                    _SelectionStart = _SelectionEnd;
                    _SelectionEnd = tmp;
                }
            }
        }

        public void Clear()
        {
            _SelectionStart = null;
            _SelectionEnd = null;
            _CursorPosition = null;
        }
    }
}