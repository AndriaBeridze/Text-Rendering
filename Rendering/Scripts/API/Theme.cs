namespace Rendering.API;

using Raylib_cs;

class Theme {
    // All characters that is supported by every font
    public static char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+-=[]{}|;':,.<>?/`~\"\\ ".ToArray();

    public static int ScreenWidth = 2560;
    public static int ScreenHeight = 1440;

    public static int DefaultOffsetX = 50;
    public static int DefaultOffsetY = 100;

    public static Color BackgroundColor = new Color(0, 0, 0, 255);
}