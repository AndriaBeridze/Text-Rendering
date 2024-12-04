namespace Rendering.App;

using Rendering.API;
using Raylib_cs;

class Program {
    static void Main() {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Theme.ScreenWidth, Theme.ScreenHeight, "Text Rendering");
        Raylib.SetTargetFPS(60);

        App app = new App();

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Theme.BackgroundColor);

            app.Update();
            app.Render();

            Raylib.EndDrawing();
        }
    }
}