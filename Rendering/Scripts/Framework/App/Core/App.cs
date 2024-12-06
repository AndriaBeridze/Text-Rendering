namespace Rendering.App;

using Rendering.Reading;

class App {
    private FontParser parser;
    private Glyph glyph;

    uint unicode = 'i';

    public App() {
        parser = new FontParser("MapleMono-Bold"); // Example font
        glyph = new Glyph(parser.ReadGlyphByUnicode(unicode));
    }

    public void Update() {
        int? key = Keyboard.RegisterKey();
        if (key != null) {
            unicode = (uint) key;
            glyph = new Glyph(parser.ReadGlyphByUnicode(unicode));
        }
    }

    public void Render() {
        glyph.Render();
    }
}