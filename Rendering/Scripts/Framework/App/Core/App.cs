namespace Rendering.App;

using Rendering.API;
using Rendering.Reading;
using Raylib_cs;

class App {
    private FontParser parser;
    private Glyph glyph;

    uint count = 0;

    public App() {
        parser = new FontParser("JetBrainsMonoNL-Regular"); // Example font

        glyph = new Glyph(parser.ReadGlyph(count));
    }

    public void Update() {
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) {
           glyph = new Glyph(parser.ReadGlyph(++count));
        }
    }

    public void Render() {
        glyph.Render();
    }
}