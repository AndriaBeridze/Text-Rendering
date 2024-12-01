namespace Rendering.Reading;

using Rendering.API;
using Rendering.Helper;

class FontParser {
    FontReader reader;

    Dictionary<string, UInt32> tableLocation = new Dictionary<string, UInt32>();

    public FontParser(string path) {
        reader = new FontReader(path);

        reader.SkipBytes(4);
        int numTables = reader.ReadUIint16();
        reader.SkipBytes(3 * 2);

        Console.WriteLine("Number of tables: " + numTables);
        for (int i = 0; i < numTables; i++) {
            string tag = reader.ReadTag();

            UInt32 checksum = reader.ReadUIint32();
            UInt32 offset = reader.ReadUIint32();
            UInt32 length = reader.ReadUIint32();

            tableLocation.Add(tag, offset);

            Console.WriteLine($"Table : {tag:D6} | Position : {offset:D6} | Length : {length:D6}");
        }

        reader.GoTo((int) tableLocation["glyf"]);
    }

    public Glyph ReadGlyph() {
        // Read number of contours
        int numContours = reader.ReadInt16();
        if (numContours <= 0) {
            return new Glyph(new int[0], new int[0], new int[0]);
        }
        int[] endPts = new int[numContours];
        reader.SkipBytes(4 * 2);

        for (int i = 0; i < endPts.Length; i++) {
            endPts[i] = reader.ReadUIint16();
        }

        // Flags
        int numPoints = endPts[endPts.Length - 1] + 1;
        byte[] flags = new byte[numPoints];
        reader.SkipBytes(reader.ReadUIint16()); // Instructions

        for (int i = 0; i < numPoints; i++) {
            byte flag = reader.ReadByte();
            flags[i] = flag;

            if (ByteHelper.IsBitSet(flags[i], 3)) {
                int repeat = reader.ReadByte();
                for (int j = 0; j < repeat; j++) {
                    Console.WriteLine($"{ numPoints } { i } { repeat }");
                    flags[i] = flag;
                }
            }
        }

        int[] xCoords = GetCoords(reader, flags, readingX : true);
        int[] yCoords = GetCoords(reader, flags, readingX : false);

        return new Glyph(xCoords, yCoords, endPts);
    }

    private int[] GetCoords(FontReader reader, byte[] flags, bool readingX) {
        int[] coords = new int[flags.Length];
        int shortVectorFlag = readingX ? 1 : 2;
        int skipVectorFlag = readingX ? 4 : 5;

        for (int i = 0; i < flags.Length; i++) {
            byte flag = flags[i];
            coords[i] = i == 0 ? 0 : coords[i - 1];

            bool isOnCurve = ByteHelper.IsBitSet(flag, 0);

            if (ByteHelper.IsBitSet(flag, shortVectorFlag)) {
                coords[i] += reader.ReadByte() * (!ByteHelper.IsBitSet(flag, skipVectorFlag) ? -1 : 1);
            } else if (!ByteHelper.IsBitSet(flag, skipVectorFlag)) {
                coords[i] += reader.ReadInt16();
            }
        }

        return coords;
    }
}