namespace Rendering.App;

using Rendering.Loader;

class App {
    private FontData font;
    private Glyph glyph;

    uint unicode = 'i';

    public App() {
        font = new FontParser("Poppins-Regular").GetFontData();
        glyph = font.GetGlyph(unicode);
    }

    public void Update() {
        int? key = Keyboard.RegisterKey();
        if (key != null) {
            unicode = (uint) key;
            glyph = font.GetGlyph(unicode);
        }
    }

    public void Render() {
        glyph.Render();
    }
}