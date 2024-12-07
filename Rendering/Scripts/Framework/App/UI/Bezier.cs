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
    
    // Point travels from start to end with the constant speed
    // This function tells where the point is at time t
    // t = 0 -> start
    // t = 1 -> end
    private Vector2 Lerp(Vector2 start, Vector2 end, float t) {
        return start + (end - start) * t;
    }

    public void Render() {
        float delta = 1.0f / resolution;
        Vector2 last = data.Start;

        for (int i = 1; i <= resolution; i++) {
            float t = i * delta;
            Vector2 IntermediateA = Lerp(data.Start, data.Control, t);
            Vector2 IntermediateB = Lerp(data.Control, data.End, t);
            Vector2 Point = Lerp(IntermediateA, IntermediateB, t);

            Raylib.DrawLineEx(last, Point, 1, Color.Red);

            last = Point;
        }
    }
}