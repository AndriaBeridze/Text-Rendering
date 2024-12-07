namespace Rendering.App;

using Rendering.Loader;

class App {
    private Text textRender;   

    public App() {
        FontData font = new FontParser("JetBrainsMonoNL-Regular").GetFontData();
        textRender = new Text("Hello, World!", font);
    }

    public void Update() {
        int? key = Keyboard.RegisterKey();
        if (key == -1) {
            textRender.RemoveChar();
        } else if (key != null) {
            textRender.AddChar((uint) key);
        }
    }

    public void Render() {
        textRender.Render();
    }
}