using SharpFont;

namespace ImGui
{
    // Configuration data when adding a font or merging fonts
    internal class ImFontConfig
    {
        internal Face            Face;
	    internal byte[]          FontData;                   //          // TTF data
	    internal int             FontDataSize;               //          // TTF data size
	    internal bool            FontDataOwnedByAtlas;       // true     // TTF data ownership taken by the container ImFontAtlas (will delete memory itself). Set to true
	    internal int             FontNo;                     // 0        // Index of font within TTF file
	    internal float           SizePixels;                 //          // Size in pixels for rasterizer
	    internal int             OversampleH, OversampleV;   // 3, 1     // Rasterize at higher quality for sub-pixel positioning. We don't use sub-pixel positions on the Y axis.
	    internal bool            PixelSnapH;                 // false    // Align every character to pixel boundary (if enabled, set OversampleH/V to 1)
	    internal ImVec2          GlyphExtraSpacing;          // 0, 0     // Extra spacing (in pixels) between glyphs
	    internal char[]          GlyphRanges;                //          // List of Unicode range (2 value per range, values are inclusive, zero-terminated list)
	    internal bool            MergeMode;                  // false    // Merge into previous ImFont, so you can combine multiple inputs font into one ImFont (e.g. ASCII font + icons + Japanese glyphs).
	    internal bool            MergeGlyphCenterV;          // false    // When merging (multiple ImFontInput for one ImFont), vertically center new glyphs instead of aligning their baseline

												    // [Internal]
	    internal string          Name;                       // Name (strictly for debugging)
	    internal ImFont          DstFont;

	    public ImFontConfig()
        {
            Face = null;
            FontData = null;
            FontDataSize = 0;
            FontDataOwnedByAtlas = true;
            FontNo = 0;
            SizePixels = 0.0f;
            OversampleH = 3;
            OversampleV = 1;
            PixelSnapH = false;
            GlyphExtraSpacing = new ImVec2(0.0f, 0.0f);
            GlyphRanges = null;
            MergeMode = false;
            MergeGlyphCenterV = false;
            DstFont = null;
            //memset(Name, 0, sizeof(Name));
            Name = null;
        }
    }
}
