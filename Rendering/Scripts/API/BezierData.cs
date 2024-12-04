namespace Rendering.API;

using System.Numerics;

class BezierData {
    public Vector2 Start;
    public Vector2 Control;
    public Vector2 End;

    public BezierData(Vector2 start, Vector2 control, Vector2 end) {
        Start = start;
        Control = control;
        End = end;
    }
}