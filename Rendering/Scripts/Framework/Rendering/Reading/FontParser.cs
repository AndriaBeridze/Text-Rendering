namespace Rendering.Reading;

using System.Numerics;
using Rendering.API;
using Rendering.App;
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
            UInt32 offset = reader.ReadUInt32(); // Actual location of the table in the file
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
        if (numContours < 0) return ReadCompoundGlyph(index);
        
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

    private GlyphData ReadCompoundGlyph(uint index) {
        GlyphData glyph = new GlyphData([], [], [], []);
        reader.GoTo(tableLocation["glyf"] + glyphOffsets[index]);

        reader.SkipBytes(2); // Skip number of contours 
        reader.SkipBytes(2 * 4); // Skip bounding box

        while (true) {
            (GlyphData subGlyph, bool isLast) = ReadComponentGlyph();

            // Add component contours to the main glyph
            foreach (List<Vector2> contours in subGlyph.Contours) {
                glyph.Contours.Add(contours);
            }

            if (isLast) break;
        }

        return glyph;
    }

    private (GlyphData, bool) ReadComponentGlyph() {
        uint flags = reader.ReadUInt16();
        uint glyphIndex = reader.ReadUInt16();

        int prevLocation = reader.Position;
        GlyphData glyph = ReadGlyph(glyphIndex);
        reader.GoTo((uint) prevLocation);

        // Read offsets
        // If the flag bit 0 is set, the values are 16 bit signed integers
        // Otherwise, they are 8 bit signed integers
        double offsetX = BitHelper.IsBitSet(flags, 0) ? reader.ReadInt16() : reader.ReadSByte();
        double offsetY = BitHelper.IsBitSet(flags, 0) ? reader.ReadInt16() : reader.ReadSByte();
        double scaleX = 1, scaleY = 1;

        if (BitHelper.IsBitSet(flags, 3)) {
            scaleX = scaleY = reader.ReadF2Dot14(); // Scales values are the same
        } else if (BitHelper.IsBitSet(flags, 6)) {
            // Scale values are different
            scaleX = reader.ReadF2Dot14();
            scaleY = reader.ReadF2Dot14();
        }

        for (int i = 0; i < glyph.Contours.Count; i++) {
            for (int j = 0; j < glyph.Contours[i].Count; j++) {
                Vector2 point = glyph.Contours[i][j];

                // Apply transformation
                point.X = (float) (point.X * scaleX + offsetX);
                point.Y = (float) (point.Y * scaleY + offsetY);

                glyph.Contours[i][j] = point;
            }
        }

        // If bit 5 is set, the glyph is the last component
        bool isLast = !BitHelper.IsBitSet(flags, 5);
        return (glyph, isLast);
    }

    private (uint, int)[] GetUnicodeMapping() {
        List<(uint, int)> mapping = new();
        reader.GoTo(tableLocation["cmap"]);

        reader.SkipBytes(2); // Skip version
        uint numSubtables = reader.ReadUInt16(); 

        uint subtableOffset = 0;
        int versionID = -1;

        for (int i = 0; i < numSubtables; i++) {
            int platformID = reader.ReadUInt16();
            int specificID = reader.ReadUInt16();
            uint offset = reader.ReadUInt32();

            if (platformID == 0) {
                if (specificID >= 0 && specificID <= 4 && specificID > versionID) {
                    subtableOffset = offset;
                    versionID = specificID;
                }
            } else if (platformID == 3 && versionID == -1) {
                if (specificID == 1 || specificID == 10) {
                    subtableOffset = offset;
                }
            }
        }

        reader.GoTo(tableLocation["cmap"] + subtableOffset);
        int format = reader.ReadUInt16();
        bool missingChar = false;

        
        if (format == 4) { // Format 4
            reader.SkipBytes(2); // Skip length
            reader.SkipBytes(2); // Skip language
            
            int numSegments = reader.ReadUInt16() / 2;
            reader.SkipBytes(6); 

            int[] endCodes = new int[numSegments];
            for (int i = 0; i < numSegments; i++) endCodes[i] = reader.ReadUInt16();
            
            reader.SkipBytes(2); 

            int[] startCodes = new int[numSegments];
            for (int i = 0; i < numSegments; i++) startCodes[i] = reader.ReadUInt16();
            
            int[] idDeltas = new int[numSegments];
            for (int i = 0; i < numSegments; i++) idDeltas[i] = reader.ReadUInt16();

            (int offset, int readLoc)[] idRangeOffsets = new (int, int)[numSegments];
            for (int i = 0; i < numSegments; i++) {
                int readLoc = reader.Position;
                int offset = reader.ReadUInt16();
                idRangeOffsets[i] = (offset, readLoc);
            }

            for (int i = 0; i < startCodes.Length; i++) {
                int endCode = endCodes[i];
                int currCode = startCodes[i];

                if (currCode == 65535) break;

                while (currCode <= endCode) {
                    int glyphIndex;

                    if (idRangeOffsets[i].offset == 0) {
                        glyphIndex = (currCode + idDeltas[i]) % 65536;
                    } else {
                        uint prevLocation = (uint) reader.Position;
                        int rangeOffsetLocation = idRangeOffsets[i].readLoc + idRangeOffsets[i].offset;
                        int glyphIndexArrayLocation = 2 * (currCode - startCodes[i]) + rangeOffsetLocation;

                        reader.GoTo((uint) glyphIndexArrayLocation);
                        glyphIndex = reader.ReadUInt16();

                        if (glyphIndex != 0) {
                            glyphIndex = (glyphIndex + idDeltas[i]) % 65536;
                        }

                        reader.GoTo(prevLocation);
                    }

                    mapping.Add(new((uint)glyphIndex, currCode));
                    missingChar |= glyphIndex == 0;
                    currCode++;
                }
            }
        } else if (format == 12) { // Format 12
            reader.SkipBytes(10); 
            uint numGroups = reader.ReadUInt32();

            for (int i = 0; i < numGroups; i++) {
                uint startCode = reader.ReadUInt32();
                uint endCode = reader.ReadUInt32();
                uint startGlyphIndex = reader.ReadUInt32();

                uint numChars = endCode - startCode + 1;
                for (int charCodeOffset = 0; charCodeOffset < numChars; charCodeOffset++) {
                    uint charCode = (uint)(startCode + charCodeOffset);
                    uint glyphIndex = (uint)(startGlyphIndex + charCodeOffset);

                    mapping.Add(new(glyphIndex, (int) charCode));
                    missingChar |= glyphIndex == 0;
                }
            }
        } else throw new Exception($"Not supported cmap format: { format }");

        if (!missingChar) mapping.Add(new(0, 65535));

        return mapping.ToArray();
    }

    public GlyphData ReadGlyphByUnicode(uint unicode) {
        if  (unicode == 32) return new GlyphData([], [], [], []); // Space character

        (uint, int)[] mappings = GetUnicodeMapping();
        uint glyphIndex = 0;

        for (int i = 0; i < mappings.Length; i++) {
            if (mappings[i].Item2 == unicode) {
                glyphIndex = mappings[i].Item1;
                break;
            }
        }

        return ReadGlyph(glyphIndex);
    }
}