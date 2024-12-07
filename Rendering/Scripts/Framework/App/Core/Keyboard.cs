namespace Rendering.App;

using Raylib_cs;

class Keyboard {
    private static Dictionary<char, char> shift = new Dictionary<char, char> {
        {'1', '!'}, {'2', '@'}, {'3', '#'}, {'4', '$'}, {'5', '%'},
        {'6', '^'}, {'7', '&'}, {'8', '*'}, {'9', '('}, {'0', ')'},
        {'-', '_'}, {'=', '+'}, {'[', '{'}, {']', '}'}, {'\\', '|'},
        {';', ':'}, {'\'', '"'}, {',', '<'}, {'.', '>'}, {'/', '?'},
        {'a', 'A'}, {'b', 'B'}, {'c', 'C'}, {'d', 'D'}, {'e', 'E'},
        {'f', 'F'}, {'g', 'G'}, {'h', 'H'}, {'i', 'I'}, {'j', 'J'},
        {'k', 'K'}, {'l', 'L'}, {'m', 'M'}, {'n', 'N'}, {'o', 'O'},
        {'p', 'P'}, {'q', 'Q'}, {'r', 'R'}, {'s', 'S'}, {'t', 'T'},
        {'u', 'U'}, {'v', 'V'}, {'w', 'W'}, {'x', 'X'}, {'y', 'Y'},
        {'z', 'Z'}, {'A', 'a'}, {'B', 'b'}, {'C', 'c'}, {'D', 'd'},
        {'E', 'e'}, {'F', 'f'}, {'G', 'g'}, {'H', 'h'}, {'I', 'i'},
        {'J', 'j'}, {'K', 'k'}, {'L', 'l'}, {'M', 'm'}, {'N', 'n'},
        {'O', 'o'}, {'P', 'p'}, {'Q', 'q'}, {'R', 'r'}, {'S', 's'},
        {'T', 't'}, {'U', 'u'}, {'V', 'v'}, {'W', 'w'}, {'X', 'x'},
        {'Y', 'y'}, {'Z', 'z'}, {'`', '~'}
    };

    public static int? RegisterKey() {
        if (Raylib.IsKeyPressed(KeyboardKey.Backspace)) return -1;

        char? key = null;
        bool isShiftPressed = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);
        bool isCapsLockPressed = Raylib.IsKeyDown(KeyboardKey.CapsLock);

        for (int i = ' '; i < '~'; i++) {
            if (!Raylib.IsKeyPressed((KeyboardKey) i)) continue;

            key = char.ToLower((char) i);
            if (isShiftPressed && shift.ContainsKey(key.Value)) {
                key = shift[key.Value];
            }
            if (isCapsLockPressed && char.IsLetter(key.Value)) {
                key = shift[key.Value];
            }

            break;
        }

        return key;
    }
}
