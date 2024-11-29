namespace Rendering.App;

using Raylib_cs;

class UIHelper {
    public static void HighlightPixel(int x, int y, Color color) {
        Raylib.DrawRectangle(x, y, 1, 1, color);
    }
}