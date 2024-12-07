namespace Rendering.Loader;

using Rendering.App;
using Rendering.API;

class FontData {
    private char[] chars = Theme.Chars;

    private Glyph[] glyphs;

    public FontData(GlyphData[] glyphData, int[] spacing, int unitsPerEm) {
        glyphs = new Glyph[glyphData.Length];
        for (int i = 0; i < glyphData.Length; i++) {
            glyphs[i] = new Glyph(glyphData[i], spacing[i], unitsPerEm);
        }
    }

    public Glyph GetGlyph(uint unicode) {
        int glyphIndex = Array.IndexOf(chars, (char) unicode);
        if (glyphIndex == -1) {
            return glyphs[glyphs.Length - 1];
        }

        return glyphs[glyphIndex];
    }
}