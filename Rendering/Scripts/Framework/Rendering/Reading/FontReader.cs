namespace Rendering.Reading;

class FontReader {
    private Stream stream;
    private BinaryReader reader;

    private const string defaultPath = "Rendering/Fonts/";
    private const string defaultExtension = ".ttf";

    public FontReader(string path) {
        stream = new FileStream(defaultPath + path + defaultExtension, FileMode.Open);
        reader = new BinaryReader(stream);
    }

    public void SkipBytes(uint count) {
        stream.Position += count;
    }

    public byte ReadByte() {
        return reader.ReadByte();
    }

    public sbyte ReadSByte() {
        return reader.ReadSByte();
    }

    public int Position => (int) stream.Position;

    public UInt16 ReadUInt16() {
        UInt16 value = reader.ReadUInt16();

        if (BitConverter.IsLittleEndian) {
            value = (UInt16) (value << 8 | value >> 8);
        }

        return value;
    }

    public UInt32 ReadUInt32() {
        UInt32 value = reader.ReadUInt32();

        if (BitConverter.IsLittleEndian) {
            value = (UInt32) (
                (value & 0x000000FF) << 24 |
                (value & 0x0000FF00) <<  8 |
                (value & 0x00FF0000) >>  8 |
                (value & 0xFF000000) >> 24
            );
        }

        return value;
    }

    public Int16 ReadInt16() => (Int16) ReadUInt16();
    public Int32 ReadInt32() => (Int32) ReadUInt32();

    public void GoTo(uint position) => stream.Position = position;

    public double ReadF2Dot14() {
        short value = reader.ReadInt16();
        return value / 16384.0f;
    }
    
    public string ReadTag() {
        string tag = "";
        for (int i = 0; i < 4; i++) {
            tag += (char) reader.ReadByte();
        }

        return tag;
    }
}