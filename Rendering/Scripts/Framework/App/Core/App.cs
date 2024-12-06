namespace Rendering.App;

using Rendering.Reading;

class App {
    private FontParser parser;
    private Glyph glyph;

    uint unicode = 'i';

    public App() {
        parser = new FontParser("JetBrainsMonoNL-Regular"); // Example font
        glyph = new Glyph(parser.ReadGlyphByUnicode(unicode));
    }

    public void Update() {
        
    }

    public void Render() {
        glyph.Render();
    }
}