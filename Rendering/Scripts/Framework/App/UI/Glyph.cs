namespace Rendering.App;

using Rendering.API;
using System.Numerics;

class Glyph {
    private GlyphData data;

    public Glyph(GlyphData data) {
        this.data = data;
    }

    public void Render() {
        foreach (var contour in data.Contours) {
            for (int i = 0; i < contour.Count; i += 2) {
                Vector2 v1 = contour[i];
                Vector2 v2 = contour[(i + 1) % contour.Count];
                Vector2 v3 = contour[(i + 2) % contour.Count];

                v1 = new Vector2(v1.X / 2 + 100, -v1.Y / 2 + 1000);
                v2 = new Vector2(v2.X / 2 + 100, -v2.Y / 2 + 1000);
                v3 = new Vector2(v3.X / 2 + 100, -v3.Y / 2 + 1000);

                new Bezier(new BezierData(v1, v2, v3)).Render();
            }
        }
    }
}