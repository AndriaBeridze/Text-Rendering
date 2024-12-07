namespace Rendering.API;

using Raylib_cs;

class Theme {
    public static char[] SupportedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+-=[]{}|;':,.<>?/`~".ToArray();

    public static int ScreenWidth = 2560;
    public static int ScreenHeight = 1440;

    public static Color BackgroundColor = new Color(0, 0, 0, 255);
}