namespace Rendering.Reading;

using System.Numerics;
using Rendering.API;
using Rendering.Helper;

class FontParser {
    private FontReader reader;

    private Dictionary<string, UInt32> tableLocation = new Dictionary<string, UInt32>();

    private int numGlyphs;
    private uint[] glyphOffsets;

    public FontParser(string path) {
        reader = new FontReader(path);

        reader.SkipBytes(4);
        int numTables = reader.ReadUInt16();
        reader.SkipBytes(3 * 2);

        for (int i = 0; i < numTables; i++) {
            string tag = reader.ReadTag();

            UInt32 checksum = reader.ReadUInt32();
            UInt32 offset = reader.ReadUInt32();
            UInt32 length = reader.ReadUInt32();

            tableLocation.Add(tag, offset);
        }

        // Get number of glyphs
        reader.GoTo(tableLocation["maxp"] + 4); // Skip version
        numGlyphs = reader.ReadUInt16();
        glyphOffsets = new uint[numGlyphs];

        // Get the ttf version
        // Important for the glyph offset data
        // Short version: data -> actual offset / 2
        // Long version:  data -> actual offset
        reader.GoTo(tableLocation["head"]);
        reader.SkipBytes(50); // Skip to indexToLocFormat
        bool isShort = reader.ReadUInt16() == 0;

        // Get the glyph offsets by reading numGlyphs values from the loca table
        reader.GoTo(tableLocation["loca"]);
        for (int i = 0; i < numGlyphs; i++) {
            glyphOffsets[i] = isShort ? reader.ReadUInt16() * 2u : reader.ReadUInt32();
        }
    }

    public GlyphData ReadGlyph(uint index) {
        reader.GoTo(tableLocation["glyf"] + glyphOffsets[index]);

        // Read number of contours
        int numContours = reader.ReadInt16();
        if (numContours < 0) return ReadGlyph(index + 1);
        
        int[] endPts = new int[numContours];
        reader.SkipBytes(4 * 2); // Skip bounding box

        for (int i = 0; i < endPts.Length; i++) endPts[i] = reader.ReadUInt16();

        // Flags
        int numPoints = endPts[endPts.Length - 1] + 1;
        byte[] flags = new byte[numPoints];
        reader.SkipBytes(reader.ReadUInt16()); // Instructions

        for (int i = 0; i < numPoints; i++) {
            byte flag = reader.ReadByte();
            flags[i] = flag;

            if (BitHelper.IsBitSet(flags[i], 3)) {
                // Repeat
                int repeat = reader.ReadByte();
                for (int j = 0; j < repeat; j++) {
                    flags[++i] = flag;
                }
            }
        }

        int[] xCoords = GetCoords(reader, flags, readingX : true);
        int[] yCoords = GetCoords(reader, flags, readingX : false);
        bool[] onCurve = GetOnCurvePoints(flags);

        return new GlyphData(xCoords, yCoords, onCurve, endPts);
    }

    private int[] GetCoords(FontReader reader, byte[] flags, bool readingX) {
        int[] coords = new int[flags.Length];
        bool[] onCurve = new bool[flags.Length];  

        int shortVectorFlag = readingX ? 1 : 2; // second bit is for x, third bit is for y
        int skipVectorFlag = readingX ? 4 : 5; // fifth bit is for x, sixth bit is for y

        for (int i = 0; i < flags.Length; i++) {
            byte flag = flags[i];
            coords[i] = i == 0 ? 0 : coords[i - 1];

            onCurve[i] = BitHelper.IsBitSet(flag, 0);

            
            int offset = 0;
            if (BitHelper.IsBitSet(flag, shortVectorFlag)) offset = reader.ReadByte() * (!BitHelper.IsBitSet(flag, skipVectorFlag) ? -1 : 1);
            else if (!BitHelper.IsBitSet(flag, skipVectorFlag)) offset += reader.ReadInt16();

            coords[i] += offset;
        }

        return coords;
    }

    private bool[] GetOnCurvePoints(byte[] flags) {
        bool[] onCurve = new bool[flags.Length];

        for (int i = 0; i < flags.Length; i++) {
            // First bit is set if the point is on the curve
            // If it is not set, the point is off the curve
            onCurve[i] = BitHelper.IsBitSet(flags[i], 0);
        }

        return onCurve;
    }
}