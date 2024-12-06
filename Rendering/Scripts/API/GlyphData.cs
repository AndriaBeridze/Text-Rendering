namespace Rendering.API;

using System.Numerics;

class GlyphData {
    // Each contour is kept separate
    public List<List<Vector2>> Contours; 

    public GlyphData(int[] xCoords, int[] yCoords, bool[] onCurve, int[] endPts) {
        Contours = [[]];
        for (int i = 0; i < xCoords.Length; i++) {
            Vector2 point = new Vector2(xCoords[i], yCoords[i]);
            Contours[Contours.Count - 1].Add(point);

            if (endPts.Contains(i)) Contours.Add([]); // Endpoint reached, start a new contour
        }

        Contours.RemoveAt(Contours.Count - 1);

        // Data contains straight lines and third order bezier curves
        // Instead of considering the special cases, we can just add the midpoints between the same type of points (onCurve or offCurve)
        // That way, both straight lines and bezier curves will become second order bezier curves
        for (int i = 0; i < endPts.Length; i++) {
            int start = i == 0 ? 0 : endPts[i - 1] + 1;
            int end = endPts[i];

            List<Vector2> contour = new List<Vector2>();

            for (int j = start; j <= end; j++) {
                int next = j == end ? start : j + 1;
                contour.Add(Contours[i][j - start]);
                if (onCurve[j] == onCurve[next]) {
                    // Add midpoint
                    contour.Add((Contours[i][j - start] + Contours[i][next - start]) / 2);
                }
            }

            // First point needs to be on the curve, because drawing cannot start from a control point
            // If it's not, move it to the end
            if (!onCurve[start]) {
                contour.Add(contour[0]);
                contour.RemoveAt(0);
            }

            Contours[i] = contour;
        }
    }
}