using Rendering.API;
using Rendering.Loader;
using Raylib_cs;

namespace Rendering.App;

class Text {
    private List<Shader> shaders;
    private FontData font;
    private int fontSize = 60;

    private List<(int, int)> offset = [(Theme.DefaultOffsetX, Theme.DefaultOffsetY)];

    public Text(string text, FontData font) {
        offset = [(Theme.DefaultOffsetX, Theme.DefaultOffsetY)];
        this.font = font;

        shaders = new List<Shader>();
        foreach (char c in text) {
            AddChar(c);
        }
    }

    public void AddChar(uint unicode) {
        Glyph glyph = font.GetGlyph(unicode);
        Rectangle boundingBox = glyph.GetBoundingBox(fontSize);

        int offsetX = offset[offset.Count - 1].Item1;
        int offsetY = offset[offset.Count - 1].Item2;

        shaders.Add(new Shader(boundingBox, glyph, fontSize, offsetX, offsetY));

        float spacing = glyph.GetSpacing(fontSize);
        if (offsetX + spacing > Theme.ScreenWidth - Theme.DefaultOffsetX) {
            offsetX = Theme.DefaultOffsetX;
            offsetY += fontSize;
        } else {
            offsetX += (int) spacing;
        }

        offset.Add((offsetX, offsetY));   
    }

    public void RemoveChar() {
        if (shaders.Count > 0) {
            shaders.RemoveAt(shaders.Count - 1);
            offset.RemoveAt(offset.Count - 1);
        }
    }

    public void Render() {
        foreach (Shader shader in shaders) {
            shader.Render();
        }
    }
}