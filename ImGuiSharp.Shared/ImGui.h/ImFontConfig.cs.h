struct ImFontConfig
{
	void*           FontData;                   //          // TTF data
	int             FontDataSize;               //          // TTF data size
	bool            FontDataOwnedByAtlas;       // true     // TTF data ownership taken by the container ImFontAtlas (will delete memory itself). Set to true
	int             FontNo;                     // 0        // Index of font within TTF file
	float           SizePixels;                 //          // Size in pixels for rasterizer
	int             OversampleH, OversampleV;   // 3, 1     // Rasterize at higher quality for sub-pixel positioning. We don't use sub-pixel positions on the Y axis.
	bool            PixelSnapH;                 // false    // Align every character to pixel boundary (if enabled, set OversampleH/V to 1)
	ImVec2          GlyphExtraSpacing;          // 0, 0     // Extra spacing (in pixels) between glyphs
	const ImWchar*  GlyphRanges;                //          // List of Unicode range (2 value per range, values are inclusive, zero-terminated list)
	bool            MergeMode;                  // false    // Merge into previous ImFont, so you can combine multiple inputs font into one ImFont (e.g. ASCII font + icons + Japanese glyphs).
	bool            MergeGlyphCenterV;          // false    // When merging (multiple ImFontInput for one ImFont), vertically center new glyphs instead of aligning their baseline

												// [Internal]
	char            Name[32];                               // Name (strictly for debugging)
	ImFont*         DstFont;

	IMGUI_API ImFontConfig();
};