// Load and rasterize multiple TTF fonts into a same texture.
// Sharing a texture for multiple fonts allows us to reduce the number of draw calls during rendering.
// We also add custom graphic data into the texture that serves for ImGui.
//  1. (Optional) Call AddFont*** functions. If you don't call any, the default font will be loaded for you.
//  2. Call GetTexDataAsAlpha8() or GetTexDataAsRGBA32() to build and retrieve pixels data.
//  3. Upload the pixels data into a texture within your graphics system.
//  4. Call SetTexID(my_tex_id); and pass the pointer/identifier to your texture. This value will be passed back to you during rendering to identify the texture.
//  5. Call ClearTexData() to free textures memory on the heap.
struct ImFontAtlas
{
	IMGUI_API ImFontAtlas();
	IMGUI_API ~ImFontAtlas();
	IMGUI_API ImFont*           AddFont(const ImFontConfig* font_cfg);
	IMGUI_API ImFont*           AddFontDefault(const ImFontConfig* font_cfg = NULL);
	IMGUI_API ImFont*           AddFontFromFileTTF(const char* filename, float size_pixels, const ImFontConfig* font_cfg = NULL, const ImWchar* glyph_ranges = NULL);
	IMGUI_API ImFont*           AddFontFromMemoryTTF(void* ttf_data, int ttf_size, float size_pixels, const ImFontConfig* font_cfg = NULL, const ImWchar* glyph_ranges = NULL);                                        // Transfer ownership of 'ttf_data' to ImFontAtlas, will be deleted after Build()
	IMGUI_API ImFont*           AddFontFromMemoryCompressedTTF(const void* compressed_ttf_data, int compressed_ttf_size, float size_pixels, const ImFontConfig* font_cfg = NULL, const ImWchar* glyph_ranges = NULL);  // 'compressed_ttf_data' still owned by caller. Compress with binary_to_compressed_c.cpp
	IMGUI_API ImFont*           AddFontFromMemoryCompressedBase85TTF(const char* compressed_ttf_data_base85, float size_pixels, const ImFontConfig* font_cfg = NULL, const ImWchar* glyph_ranges = NULL);              // 'compressed_ttf_data_base85' still owned by caller. Compress with binary_to_compressed_c.cpp with -base85 paramaeter
	IMGUI_API void              ClearTexData();             // Clear the CPU-side texture data. Saves RAM once the texture has been copied to graphics memory.
	IMGUI_API void              ClearInputData();           // Clear the input TTF data (inc sizes, glyph ranges)
	IMGUI_API void              ClearFonts();               // Clear the ImGui-side font data (glyphs storage, UV coordinates)
	IMGUI_API void              Clear();                    // Clear all

															// Retrieve texture data
															// User is in charge of copying the pixels into graphics memory, then call SetTextureUserID()
															// After loading the texture into your graphic system, store your texture handle in 'TexID' (ignore if you aren't using multiple fonts nor images)
															// RGBA32 format is provided for convenience and high compatibility, but note that all RGB pixels are white, so 75% of the memory is wasted.
															// Pitch = Width * BytesPerPixels
	IMGUI_API void              GetTexDataAsAlpha8(unsigned char** out_pixels, int* out_width, int* out_height, int* out_bytes_per_pixel = NULL);  // 1 byte per-pixel
	IMGUI_API void              GetTexDataAsRGBA32(unsigned char** out_pixels, int* out_width, int* out_height, int* out_bytes_per_pixel = NULL);  // 4 bytes-per-pixel
	void                        SetTexID(void* id) { TexID = id; }

	// Helpers to retrieve list of common Unicode ranges (2 value per range, values are inclusive, zero-terminated list)
	// (Those functions could be static but aren't so most users don't have to refer to the ImFontAtlas:: name ever if in their code; just using io.Fonts->)
	IMGUI_API const ImWchar*    GetGlyphRangesDefault();    // Basic Latin, Extended Latin
	IMGUI_API const ImWchar*    GetGlyphRangesKorean();     // Default + Korean characters
	IMGUI_API const ImWchar*    GetGlyphRangesJapanese();   // Default + Hiragana, Katakana, Half-Width, Selection of 1946 Ideographs
	IMGUI_API const ImWchar*    GetGlyphRangesChinese();    // Japanese + full set of about 21000 CJK Unified Ideographs
	IMGUI_API const ImWchar*    GetGlyphRangesCyrillic();   // Default + about 400 Cyrillic characters

															// Members
															// (Access texture data via GetTexData*() calls which will setup a default font for you.)
	void*                       TexID;              // User data to refer to the texture once it has been uploaded to user's graphic systems. It ia passed back to you during rendering.
	unsigned char*              TexPixelsAlpha8;    // 1 component per pixel, each component is unsigned 8-bit. Total size = TexWidth * TexHeight
	unsigned int*               TexPixelsRGBA32;    // 4 component per pixel, each component is unsigned 8-bit. Total size = TexWidth * TexHeight * 4
	int                         TexWidth;           // Texture width calculated during Build().
	int                         TexHeight;          // Texture height calculated during Build().
	int                         TexDesiredWidth;    // Texture width desired by user before Build(). Must be a power-of-two. If have many glyphs your graphics API have texture size restrictions you may want to increase texture width to decrease height.
	ImVec2                      TexUvWhitePixel;    // Texture coordinates to a white pixel
	ImVector<ImFont*>           Fonts;              // Hold all the fonts returned by AddFont*. Fonts[0] is the default font upon calling ImGui::NewFrame(), use ImGui::PushFont()/PopFont() to change the current font.

													// Private
	ImVector<ImFontConfig>      ConfigData;         // Internal data
	IMGUI_API bool              Build();            // Build pixels data. This is automatically for you by the GetTexData*** functions.
	IMGUI_API void              RenderCustomTexData(int pass, void* rects);
};
