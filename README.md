![](https://github.com/ryancheung/SkiaTextRenderer/workflows/.NET%20Core/badge.svg)

## A Winform-liked TextRenderer implemented with SkiaSharp

SkiaTextRenderer is a text renderer that simulate the [`System.Windows.Forms.TextRenderer`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.textrenderer.drawtext?view=netframework-4.8).

Its goal is to provider a cross-platform **TextRenderer** that can be easly used in game or app.

The APIs are just like the Winform version. Even [`TextFormatFlags`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.textformatflags?view=netframework-4.8) was directly copied and used. Most of the flags are implemented for now, left flags could be implemented later.

### Examples

#### TextFormatFlags.Default

![TextFormatFlags.Default](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/Hello-你好-world!-20-Default.png)

#### TextFormatFlags.HorizontalCenter

![TextFormatFlags.HorizontalCenter](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/Hello-你好-world!-20-HorizontalCenter.png)

#### TextFormatFlags.LeftAndRightPadding

![TextFormatFlags.LeftAndRightPadding](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/Hello-你好-world!-20-LeftAndRightPadding.png)

#### TextFormatFlags.NoPadding

![TextFormatFlags.NoPadding](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/Hello-你好-world!-20-NoPadding.png)

#### Italic & TextFormatFlags.Default

![Italic & TextFormatFlags.Default](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/italic-Hello-你好-world!-20-Default.png)

#### Underline & TextFormatFlags.Default

![Underline & TextFormatFlags.Default](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/underline-Hello-你好-world!-20-Default.png)

#### StrikeThrough & TextFormatFlags.Default

![StrikeThrough & TextFormatFlags.Default](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/strikethrough-Hello-你好-world!-20-Default.png)

#### Sized & TextFormatFlags.Default

![Sized & TextFormatFlags.Default](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/sized-Hello-你好-world!-12-Default.png)

#### Sized & TextFormatFlags.VertialCenter

![Sized & TextFormatFlags.VertialCenter & TextFormatFlags.WordBreak](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/sized-Hello-你好-world!-12-VerticalCenter.png)

#### Sized & TextFormatFlags.VertialCenter & TextFormatFlags.WordBreak

![Sized & TextFormatFlags.VertialCenter & TextFormatFlags.WordBreak](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/sized-Hello-你好-world!-12-GlyphOverhangPadding--Left--Top--VerticalCenter--WordBreak.png)

#### Sized & TextFormatFlags.HorizontalCenter & TextFormatFlags.VerticalCenter & TextFormatFlags.WordBreak

![Sized & TextFormatFlags.HorizontalCenter & TextFormatFlags.VerticalCenter & TextFormatFlags.WordBreak](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/sized-Hello-你好-world!-12-GlyphOverhangPadding--Left--Top--HorizontalCenter--VerticalCenter--WordBreak.png)

#### Auto break line with newline character '\n' or '\r' or "\r\n"

![Auto break line with newline character '\n' or '\r' or "\r\n"](https://github.com/ryancheung/SkiaTextRenderer/raw/master/examples/multiline-Hello-你-好-world!-20-Default.png)

### Install

```
nuget install SkiaTextRenderer
```

### Usage

```csharp
static void TestDraw(string text, Font font, TextFormatFlags flags)
{
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
}
```

### TODO

- Implement `TextFormatFlags.WordEllipsis`
- Implement `TextFormatFlags.SingleLine`
- Feature to return the cursor position of given click position for TextBox support
- Feature to draw text selection for TextBox support
