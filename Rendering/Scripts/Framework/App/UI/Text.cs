using Rendering.API;
using Rendering.Loader;

namespace Rendering.App;

class Text {
    private List<Glyph> glyphs;
    private FontData font;
    private int fontSize = 60;

    public Text(string text, FontData font) {
        this.font = font;

        glyphs = new List<Glyph>();
        foreach (char c in text) {
            AddChar(c);
        }
    }

    public void AddChar(uint unicode) {
        glyphs.Add(font.GetGlyph(unicode));   
    }

    public void RemoveChar() {
        if (glyphs.Count > 0) {
            glyphs.RemoveAt(glyphs.Count - 1);
        }
    }

    public void Render() {
        int offsetX = Theme.DefaultOffsetX, offsetY = Theme.DefaultOffsetY;

        foreach (Glyph glyph in glyphs) {
            float spacing = glyph.GetSpacing(fontSize);
            if (offsetX + spacing > Theme.ScreenWidth - Theme.DefaultOffsetX) {
                offsetX = Theme.DefaultOffsetX;
                offsetY += fontSize;
            }
            offsetX += (int) glyph.Render(fontSize, offsetX, offsetY);
        }
    }
}