namespace Rendering.App;

using Rendering.API;
using Raylib_cs;
using System.Numerics;

class Glyph {
    private GlyphData data;
    private float spacing;

    public Glyph(GlyphData data, int spacing, int unitsPerEm) {
        float xCoordMin = int.MaxValue;
        float xCoordMax = int.MinValue;

        for (int i = 0; i < data.Contours.Count; i++) {
            for (int j = 0; j < data.Contours[i].Count; j++) {
                data.Contours[i][j] = new Vector2(1.0f * data.Contours[i][j].X / unitsPerEm, 1.0f * data.Contours[i][j].Y / unitsPerEm);
                xCoordMin = Math.Min(xCoordMin, data.Contours[i][j].X);
                xCoordMax = Math.Max(xCoordMax, data.Contours[i][j].X);
            }
        }

        this.spacing = 1.0f * spacing / unitsPerEm;
        this.data = data;
    }

    public float GetSpacing(int fontSize) {
        return spacing * fontSize;
    }

    public Rectangle GetBoundingBox(int fontSize) {
        int xMin = int.MaxValue;
        int xMax = int.MinValue;
        int yMin = int.MaxValue;
        int yMax = int.MinValue;

        foreach (var contour in data.Contours) {
            foreach (var point in contour) {
                xMin = Math.Min(xMin, (int) (point.X * fontSize));
                xMax = Math.Max(xMax, (int) (point.X * fontSize));
                yMin = Math.Min(yMin, (int) (-point.Y * fontSize));
                yMax = Math.Max(yMax, (int) (-point.Y * fontSize));
            }
        }

        return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
    }


    public float Render(int fontSize, int offsetX, int offsetY) {
        // Every adjacent pair of points are different types (onCurve, offCurve)
        // First point is always onCurve
        // We can get three adjacent points and create a bezier curve from them (two endpoints and one control point)
        foreach (var contour in data.Contours) {
            for (int i = 0; i < contour.Count; i += 2) {
                Vector2 v1 = contour[i];
                Vector2 v2 = contour[(i + 1) % contour.Count];
                Vector2 v3 = contour[(i + 2) % contour.Count];

                v1 = new Vector2(v1.X * fontSize + offsetX, -v1.Y * fontSize + offsetY);
                v2 = new Vector2(v2.X * fontSize + offsetX, -v2.Y * fontSize + offsetY);
                v3 = new Vector2(v3.X * fontSize + offsetX, -v3.Y * fontSize + offsetY);

                new Bezier(new BezierData(v1, v2, v3)).Render();
            }
        }

        return spacing * fontSize;
    }
}