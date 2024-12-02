namespace Rendering.App;

using Rendering.API;
using Rendering.Reading;
using Raylib_cs;

class App {
    private FontParser parser;
    private Glyph glyph;

    public App() {
        parser = new FontParser("MapleMono-Bold"); // Example font

        glyph = new Glyph(parser.ReadGlyph(6));
    }

    public void Update() {

    }

    public void Render() {
        glyph.Render();
    }
}