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

    public void SkipBytes(int count) {
        stream.Position += count;
    }

    public byte ReadByte() {
        return reader.ReadByte();
    }

    public int Position => (int) stream.Position;

    public UInt16 ReadUIint16() {
        UInt16 value = reader.ReadUInt16();

        if (BitConverter.IsLittleEndian) {
            value = (UInt16) (value << 8 | value >> 8);
        }

        return value;
    }

    public UInt32 ReadUIint32() {
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

    public Int16 ReadInt16() => (Int16) ReadUIint16();
    public Int32 ReadInt32() => (Int32) ReadUIint32();

    public void GoTo(int position) => stream.Position = position;
    
    public string ReadTag() {
        string tag = "";
        for (int i = 0; i < 4; i++) {
            tag += (char) reader.ReadByte();
        }

        return tag;
    }
}