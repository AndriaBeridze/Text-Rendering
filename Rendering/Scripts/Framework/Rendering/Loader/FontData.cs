namespace Rendering.Loader;

using Rendering.App;
using Rendering.API;

class FontData {
    private char[] supportedCharacters = Theme.SupportedCharacters;

    private Glyph[] glyphs;
    private int unitsPerEm;

    public FontData(GlyphData[] glyphData, int unitsPerEm) {
        this.unitsPerEm = unitsPerEm;

        glyphs = new Glyph[glyphData.Length];
        for (int i = 0; i < glyphData.Length; i++) {
            glyphs[i] = new Glyph(glyphData[i]);
        }
    }

    public Glyph GetGlyph(uint unicode) {
        int glyphIndex = Array.IndexOf(supportedCharacters, (char) unicode);
        if (glyphIndex == -1) {
            return new Glyph(new GlyphData([], [], [], []));
        }

        return glyphs[glyphIndex];
    }
}