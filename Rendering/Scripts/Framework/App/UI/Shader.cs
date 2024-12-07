using Raylib_cs;

namespace Rendering.App;

class Shader {
    private int fontSize;
    private Rectangle boundingBox;
    private Glyph glyph;

    private int offsetX, offsetY;

    public Shader(Rectangle boundingBox, Glyph glyph, int fontSize, int offsetX, int offsetY) {
        this.boundingBox = boundingBox;

        this.glyph = glyph;

        this.fontSize = fontSize;

        this.offsetX = offsetX; 
        this.offsetY = offsetY;

        SetOffset(offsetX, offsetY);
    }

    public Shader(Rectangle boundingBox, Glyph glyph, int fontSize) {
        this.boundingBox = boundingBox;
        this.glyph = glyph;

        this.fontSize = fontSize;

        this.offsetX = 0;
        this.offsetY = 0;
    }

    public void SetOffset(int offsetX, int offsetY) {
        boundingBox.X += offsetX;
        boundingBox.Y += offsetY;
    }

    public float GetSpacing(int fontSize) {
        return glyph.GetSpacing(fontSize);
    }

    public float Render() {
        Raylib.DrawRectangleRec(boundingBox, Color.White);
        return glyph.Render(fontSize, offsetX, offsetY);
    }
}