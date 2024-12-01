namespace Rendering.Helper;

class ByteHelper {
    public static bool IsBitSet(byte b, int pos) => (b & (1 << pos)) != 0;
}

