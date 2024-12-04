namespace Rendering.Helper;

class BitHelper {
    public static bool IsBitSet(byte b, int pos) => (b & (1 << pos)) != 0;

    public static bool IsBitSet(UInt16 b, int pos) => (b & (1 << pos)) != 0;

    public static bool IsBitSet(UInt32 b, int pos) => (b & (1 << pos)) != 0;
}

