namespace Rendering.App;

using System.Numerics;
using Rendering.API;
using Raylib_cs;

class Bezier {
    private BezierData data;
    private int resolution = 50;

    public Bezier(BezierData data) {
        this.data = data;
    }
    
    private Vector2 Lerp(Vector2 s, Vector2 e, float t) {
        return s + (e - s) * t;
    }

    public void Render() {
        float delta = 1.0f / resolution;
        Vector2 last = data.Start;

        for (int i = 1; i <= resolution; i++) {
            float t = i * delta;
            Vector2 IntermediateA = Lerp(data.Start, data.Control, t);
            Vector2 IntermediateB = Lerp(data.Control, data.End, t);
            Vector2 Point = Lerp(IntermediateA, IntermediateB, t);

            Raylib.DrawLineEx(last, Point, 3, Color.Blue);

            last = Point;
        }
    }
}