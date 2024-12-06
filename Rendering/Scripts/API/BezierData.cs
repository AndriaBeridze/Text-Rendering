namespace Rendering.API;

using System.Numerics;

class BezierData {
    public Vector2 Start;
    public Vector2 Control;
    public Vector2 End;

    // For more information on Bezier curves, see https://en.wikipedia.org/wiki/Bezier_curve
    public BezierData(Vector2 start, Vector2 control, Vector2 end) {
        Start = start;
        Control = control;
        End = end;
    }
}