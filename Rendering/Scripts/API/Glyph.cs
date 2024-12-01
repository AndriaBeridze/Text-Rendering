namespace Rendering.API;

using Raylib_cs;

class Glyph {
    int[] xCoords;
    int[] yCoords;
    int[] endPts;

    public Glyph(int[] xCoords, int[] yCoords, int[] endPts) {
        this.xCoords = xCoords;
        this.yCoords = yCoords;
        this.endPts = endPts;
        
        for (int i = 0; i < endPts.Length; i++) Console.WriteLine($"End point {i}: {endPts[i]}");
        for (int i = 0; i < xCoords.Length; i++) Console.WriteLine($"Point {i}: ({ xCoords[i] }, { yCoords[i] })");
    }  

    public void Render() {
    } 
}