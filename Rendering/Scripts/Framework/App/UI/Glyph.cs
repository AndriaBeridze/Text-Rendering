namespace Rendering.App;

using Rendering.API;
using Raylib_cs;
using System.Numerics;

class Glyph {
    private GlyphData data;

    public Glyph(GlyphData data) {
        this.data = data;
    }

    public void Render() {
        foreach (var contour in data.Contours) {
            for (int i = 0; i < contour.Count; i++) 
                Raylib.DrawLineV(contour[i] / 2 + new Vector2(100, 100), contour[(i + 1) % contour.Count] / 2 + new Vector2(100, 100), Color.White);

            for (int i = 0; i < contour.Count; i++)
                Raylib.DrawCircle((int) contour[i].X / 2 + 100, (int) contour[i].Y / 2 + 100, 3, Color.Red);
        }
    }
}