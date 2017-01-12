using System;
using System.Collections.Generic;
using SharpFont;

namespace ImGui
{
    // Runtime data for multiple fonts, bake multiple fonts into a single texture, TTF font loader
    // Load and rasterize multiple TTF fonts into a same texture.
    // Sharing a texture for multiple fonts allows us to reduce the number of draw calls during rendering.
    // We also add custom graphic data into the texture that serves for ImGui.
    //  1. (Optional) Call AddFont*** functions. If you don't call any, the default font will be loaded for you.
    //  2. Call GetTexDataAsAlpha8() or GetTexDataAsRGBA32() to build and retrieve pixels data.
    //  3. Upload the pixels data into a texture within your graphics system.
    //  4. Call SetTexID(my_tex_id); and pass the pointer/identifier to your texture. This value will be passed back to you during rendering to identify the texture.
    //  5. Call ClearTexData() to free textures memory on the heap.
    public class ImFontAtlas
    {
        static char[] GetGlyphRangesDefault()
        {
            return new char[]
            {
                '\x0020', '\x00FF', // Basic Latin + Latin Supplement
                '\x0000',
            };
        }

        static uint Decode85Byte(char c) { return (c >= '\\') ? c - 36u : c - 35u; }
        static unsafe void Decode85(string src, byte[] dst)
        {
            var srcidx = 0;
            var dstidx = 0;
            while (srcidx < src.Length)
            {
                uint tmp = Decode85Byte(src[srcidx + 0]) + 85u * (Decode85Byte(src[srcidx + 1]) + 85u * (Decode85Byte(src[srcidx + 2]) + 85u * (Decode85Byte(src[srcidx + 3]) + 85u * Decode85Byte(src[srcidx + 4]))));
                dst[dstidx + 0] = (byte)((tmp >> 0) & 0xFF);
                dst[dstidx + 1] = (byte)((tmp >> 8) & 0xFF);
                dst[dstidx + 2] = (byte)((tmp >> 16) & 0xFF);
                dst[dstidx + 3] = (byte)((tmp >> 24) & 0xFF);   // We can't assume little-endianess.
                srcidx += 5;
                dstidx += 4;
            }
        }

        // Members
        // (Access texture data via GetTexData*() calls which will setup a default font for you.)
        internal ImTextureID TexID;              // User data to refer to the texture once it has been uploaded to user's graphic systems. It ia passed back to you during rendering.
        byte[] TexPixelsAlpha8;    // 1 component per pixel, each component is unsigned 8-bit. Total size = TexWidth * TexHeight
        byte[] TexPixelsRGBA32;    // 4 component per pixel, each component is unsigned 8-bit. Total size = TexWidth * TexHeight * 4
        internal int TexWidth;           // Texture width calculated during Build().
        internal int TexHeight;          // Texture height calculated during Build().
        internal int TexDesiredWidth;    // Texture width desired by user before Build(). Must be a power-of-two. If have many glyphs your graphics API have texture size restrictions you may want to increase texture width to decrease height.
        internal ImVec2 TexUvWhitePixel;    // Texture coordinates to a white pixel
        internal ImVector<ImFont> Fonts;              // Hold all the fonts returned by AddFont*. Fonts[0] is the default font upon calling ImGui::NewFrame(), use ImGui::PushFont()/PopFont() to change the current font.

        // Private
        internal ImVector<ImFontConfig> ConfigData;         // Internal data

        //// Build pixels data. This is automatically for you by the GetTexData*** functions.
        //struct ImFontTempBuildData
        //{
        //    internal stbtt_fontinfo FontInfo;
        //    internal stbrp_rect Rects;
        //    internal stbtt_pack_range Ranges;
        //    internal int RangesCount;
        //};

        struct ImFontPackingRect
        {
            internal int id;

            //input
            internal int w, h;

            //output
            internal int x, y;
            internal bool was_packed;

            internal bool pack(MaxRectsBinPack spc)
            {
                was_packed = false;
                if (w == 0 || h == 0)
                    return false;

                var r = spc.Insert(w, h, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit);
                if (r.width == 0 || r.height == 0)
                    return false;

                x = (int)r.x;
                y = (int)r.y;

                was_packed = true;
                return true;
            }

        }
        //public ImRect[] PackFont(MaxRectsBinPack packer, Texture2D texture, Texture2D[] textures, int width, int height, int maxSize)
        //{
        //    if (width > maxSize && height > maxSize) return null;
        //    if (width > maxSize || height > maxSize) { int temp = width; width = height; height = temp; }

        //    MaxRectsBinPack bp = new MaxRectsBinPack(width, height);
        //    ImRect[] rects = new ImRect[textures.Length];

        //    for (int i = 0; i < textures.Length; i++)
        //    {
        //        Texture2D tex = textures[i];
        //        ImRect rect = bp.Insert(tex.width, tex.height, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit);
        //        if (rect.width == 0 || rect.height == 0)
        //        {
        //            return PackTextures(texture, textures, width * (width <= height ? 2 : 1), height * (height < width ? 2 : 1), maxSize);
        //        }
        //        rects[i] = rect;
        //    }
        //    texture.Resize(width, height);
        //    texture.SetPixels(new ImColor[width * height]);
        //    for (int i = 0; i < textures.Length; i++)
        //    {
        //        Texture2D tex = textures[i];
        //        ImRect rect = rects[i];
        //        ImColor[] colors = tex.GetPixels();

        //        if (rect.width != tex.width)
        //        {
        //            ImColor[] newColors = tex.GetPixels();

        //            for (int x = 0; x < rect.width; x++)
        //            {
        //                for (int y = 0; y < rect.height; y++)
        //                {
        //                    int prevIndex = ((int)rect.height - (y + 1)) + x * (int)tex.width;
        //                    newColors[x + y * (int)rect.width] = colors[prevIndex];
        //                }
        //            }

        //            colors = newColors;
        //        }

        //        texture.SetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, colors);
        //        rect.x /= width;
        //        rect.y /= height;
        //        rect.width /= width;
        //        rect.height /= height;
        //        rects[i] = rect;
        //    }

        //    return rects;

        //}


        internal bool Build()
        {
            System.Diagnostics.Debug.Assert(ConfigData.Size > 0);

            TexID = null;
            TexWidth = TexHeight = 0;
            TexUvWhitePixel = new ImVec2(0, 0);
            ClearTexData();

            int total_glyph_count = 0;
            int total_glyph_range_count = 0;
            for (int input_i = 0; input_i < ConfigData.Size; input_i++)
            {
                ImFontConfig cfg = ConfigData[input_i];
                System.Diagnostics.Debug.Assert(cfg.DstFont != null && (!cfg.DstFont.IsLoaded() || cfg.DstFont.ContainerAtlas == this));

                System.Diagnostics.Debug.Assert(cfg.FontData != null);

                // Count glyphs
                if (cfg.GlyphRanges == null)
                    cfg.GlyphRanges = GetGlyphRangesDefault();

                for (int in_range = 0; cfg.GlyphRanges[in_range] > 0 && cfg.GlyphRanges[in_range + 1] > 0; in_range += 2)
                {
                    total_glyph_count += (cfg.GlyphRanges[in_range + 1] - cfg.GlyphRanges[in_range]) + 1;
                    total_glyph_range_count++;
                }
            }

            // Start packing. We need a known width for the skyline algorithm. Using a cheap heuristic here to decide of width. User can override TexDesiredWidth if they wish.
            // After packing is done, width shouldn't matter much, but some API/GPU have texture size limitations and increasing width can decrease height.
            TexWidth = (TexDesiredWidth > 0) ? TexDesiredWidth : (total_glyph_count > 4000) ? 4096 : (total_glyph_count > 2000) ? 2048 : (total_glyph_count > 1000) ? 1024 : 512;
            TexHeight = 0;
            int max_tex_height = 1024 * 32;
            var spc = new MaxRectsBinPack(TexWidth, max_tex_height, false);

            ImVector<ImFontPackingRect> rects = new ImVector<ImFontPackingRect>();
            RenderCustomTexData(spc, 0, rects);

            // First font pass: pack all glyphs (no rendering at this point, we are working with rectangles in an infinitely tall texture at this point)
            for (int input_i = 0; input_i < ConfigData.Size; input_i++)
            {
                ImFontConfig cfg = ConfigData[input_i];
                cfg.Face.SetPixelSizes((uint)(cfg.SizePixels * cfg.OversampleH), (uint)(cfg.SizePixels * cfg.OversampleV));
                for (int in_range = 0; cfg.GlyphRanges[in_range] > 0 && cfg.GlyphRanges[in_range + 1] > 0; in_range += 2)
                {
                    var glyphs = new List<ImRect>((cfg.GlyphRanges[in_range + 1] - cfg.GlyphRanges[in_range]) + 1);
                    var packedGlyphs = new List<ImRect>((cfg.GlyphRanges[in_range + 1] - cfg.GlyphRanges[in_range]) + 1);
                    for (var range = cfg.GlyphRanges[in_range]; range <= cfg.GlyphRanges[in_range + 1]; range++)
                    {
                        char c = (char)range;

                        uint glyphIndex = cfg.Face.GetCharIndex(c);
                        cfg.Face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);

                        //added padding to keep from bleeding
                        glyphs.Add(new ImRect(0, 0, (int)cfg.Face.Glyph.Metrics.Width + 2, (int)cfg.Face.Glyph.Metrics.Height + 2));
                    }
                    spc.Insert(glyphs, packedGlyphs, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit);
                    System.Diagnostics.Debug.Assert(glyphs.Count == packedGlyphs.Count);

                    for (var i = 0; i < glyphs.Count; i++)
                    {
                        var c = (cfg.GlyphRanges[in_range] + i);
                        var g = glyphs[i];
                        var pg = packedGlyphs[i];

                        var was_packed = pg.width > 0 && pg.height > 0;
                        var r = new ImFontPackingRect()
                        {
                            id = c,
                            x = (int)pg.x + 1,
                            y = (int)pg.y + 1,
                            w = (int)pg.width,
                            h = (int)pg.height,
                            was_packed = was_packed
                        };

                        if (was_packed)
                            TexHeight = ImGui.Max(TexHeight, r.y + r.h);

                        rects.push_back(r);
                    }
                }
            }

            // Create texture
            TexHeight = ImGui.UpperPowerOfTwo(TexHeight);
            TexPixelsAlpha8 = new byte[TexWidth * TexHeight];

            for (int input_i = 0; input_i < ConfigData.Size; input_i++)
            {
                ImFontConfig cfg = ConfigData[input_i];
                ImFont dst_font = cfg.DstFont;

                int unscaled_ascent = cfg.Face.Ascender,
                    unscaled_descent = cfg.Face.Descender;

                float font_scale = cfg.SizePixels / (unscaled_ascent - unscaled_descent); //taken from stbtt_ScaleForPixelHeight
                var max_height = cfg.Face.Height * font_scale;

                float ascent = unscaled_ascent * font_scale;
                float descent = unscaled_descent * font_scale;
                if (!cfg.MergeMode)
                {
                    dst_font.ContainerAtlas = this;
                    dst_font.ConfigData = cfg;
                    dst_font.ConfigDataCount = 0;
                    dst_font.FontSize = cfg.SizePixels;
                    dst_font.Ascent = ascent;
                    dst_font.Descent = descent;
                    dst_font.Glyphs.resize(0);
                }

                dst_font.ConfigDataCount++;
                float off_y = (cfg.MergeMode && cfg.MergeGlyphCenterV) ? (ascent - dst_font.Ascent) * 0.5f : 0.0f;

                //render
                for (var i = 0; i < rects.Size; i++)
                {
                    var rect = rects[i];
                    //if (rect.id > 0 /*&& rect.was_packed*/)
                    {
                        var codepoint = (ushort)rect.id;
                        if (cfg.MergeMode && dst_font.HasGlyph((char)codepoint))
                            continue;

                        uint glyphIndex = cfg.Face.GetCharIndex(codepoint);
                        cfg.Face.LoadGlyph(glyphIndex, LoadFlags.Render, LoadTarget.Normal);
                        cfg.Face.Glyph.RenderGlyph(RenderMode.Normal);

                        var bmp = cfg.Face.Glyph.Bitmap;
                        for (var x = 0; x < bmp.Width; x++)
                            for (var y = 0; y < bmp.Rows; y++)
                                TexPixelsAlpha8[(rect.x + x) + ((rect.y + y) * TexWidth)] = bmp.BufferData[x + y * bmp.Pitch];
                    }
                }

                cfg.Face.SetPixelSizes(0, (uint)cfg.SizePixels);

                //dst_font.FallbackGlyph = null; // Always clear fallback so FindGlyph can return NULL. It will be set again in BuildLookupTable()
                for (var i = 0; i < rects.Size; i++)
                {
                    var rect = rects[i];
                    //if (rect.id > 0 /*&& rect.was_packed*/)
                    {
                        var codepoint = (ushort)rect.id;
                        if (cfg.MergeMode && dst_font.HasGlyph((char)codepoint))
                            continue;

                        uint glyphIndex = cfg.Face.GetCharIndex(codepoint);
                        cfg.Face.LoadGlyph(glyphIndex, LoadFlags.ComputeMetrics, LoadTarget.Normal);

                        //var bmp = cfg.Face.Glyph.Bitmap;
                        //for (var x = 0; x < bmp.Width; x++)
                        //    for (var y = 0; y < bmp.Rows; y++)
                        //        TexPixelsAlpha8[(rect.x + x) + ((rect.y + y) * TexWidth)] = bmp.BufferData[x + y * bmp.Pitch];

                        dst_font.Glyphs.resize(dst_font.Glyphs.Size + 1);
                        var glyph = dst_font.Glyphs[dst_font.Glyphs.Size - 1];
                        glyph.Codepoint = codepoint;

                        //glyph.X0 = 0;
                        //glyph.X1 = (int)cfg.Face.Glyph.Metrics.Width;
                        //glyph.Y0 = 0;
                        //glyph.Y1 = (int)cfg.Face.Glyph.Metrics.Height;

                        glyph.X0 = cfg.Face.Glyph.BitmapLeft + 1;
                        glyph.X1 = (int)cfg.Face.Glyph.Metrics.Width + glyph.X0;
                        glyph.Y0 = max_height - (float)cfg.Face.Glyph.Metrics.HorizontalBearingY;
                        glyph.Y0 -= 2;
                        //glyph.Y1 -= max_height / 2;
                        glyph.Y1 = (float)cfg.Face.Glyph.Metrics.Height + glyph.Y0;
                        //glyph.Y0 += (int)(dst_font.Ascent + off_y + 0.5f);
                        //glyph.Y1 += (int)(dst_font.Ascent + off_y + 0.5f);


                        glyph.U0 = rect.x / (float)TexWidth;
                        glyph.V0 = rect.y / (float)TexHeight;
                        glyph.U1 = (rect.x + rect.w) / (float)TexWidth;
                        glyph.V1 = (rect.y + rect.h) / (float)TexHeight;


                        glyph.XAdvance = ((int)cfg.Face.Glyph.Advance.X + cfg.GlyphExtraSpacing.x);  // Bake spacing into XAdvance
                        if (cfg.PixelSnapH)
                            glyph.XAdvance = (int)(glyph.XAdvance + 0.5f);
                        dst_font.Glyphs[dst_font.Glyphs.Size - 1] = glyph;
                    }
                }

                cfg.DstFont.BuildLookupTable();
            }

            // Cleanup temporaries
            //ImGui::MemFree(buf_packedchars);
            //ImGui::MemFree(buf_ranges);
            //ImGui::MemFree(tmp_array);

            // Render into our custom data block
            RenderCustomTexData(spc, 1, rects);

            return true;
        }

        void RenderCustomTexData(MaxRectsBinPack spc, int pass, ImVector<ImFontPackingRect> rects)
        {
            // A work of art lies ahead! (. = white layer, X = black layer, others are blank)

            // The white texels on the top left are the ones we'll use everywhere in ImGui to render filled shapes.
            const int TEX_DATA_W = 90;
            const int TEX_DATA_H = 27;
            const string texture_data =
                "..-         -XXXXXXX-    X    -           X           -XXXXXXX          -          XXXXXXX" +
                "..-         -X.....X-   X.X   -          X.X          -X.....X          -          X.....X" +
                "---         -XXX.XXX-  X...X  -         X...X         -X....X           -           X....X" +
                "X           -  X.X  - X.....X -        X.....X        -X...X            -            X...X" +
                "XX          -  X.X  -X.......X-       X.......X       -X..X.X           -           X.X..X" +
                "X.X         -  X.X  -XXXX.XXXX-       XXXX.XXXX       -X.X X.X          -          X.X X.X" +
                "X..X        -  X.X  -   X.X   -          X.X          -XX   X.X         -         X.X   XX" +
                "X...X       -  X.X  -   X.X   -    XX    X.X    XX    -      X.X        -        X.X      " +
                "X....X      -  X.X  -   X.X   -   X.X    X.X    X.X   -       X.X       -       X.X       " +
                "X.....X     -  X.X  -   X.X   -  X..X    X.X    X..X  -        X.X      -      X.X        " +
                "X......X    -  X.X  -   X.X   - X...XXXXXX.XXXXXX...X -         X.X   XX-XX   X.X         " +
                "X.......X   -  X.X  -   X.X   -X.....................X-          X.X X.X-X.X X.X          " +
                "X........X  -  X.X  -   X.X   - X...XXXXXX.XXXXXX...X -           X.X..X-X..X.X           " +
                "X.........X -XXX.XXX-   X.X   -  X..X    X.X    X..X  -            X...X-X...X            " +
                "X..........X-X.....X-   X.X   -   X.X    X.X    X.X   -           X....X-X....X           " +
                "X......XXXXX-XXXXXXX-   X.X   -    XX    X.X    XX    -          X.....X-X.....X          " +
                "X...X..X    ---------   X.X   -          X.X          -          XXXXXXX-XXXXXXX          " +
                "X..X X..X   -       -XXXX.XXXX-       XXXX.XXXX       ------------------------------------" +
                "X.X  X..X   -       -X.......X-       X.......X       -    XX           XX    -           " +
                "XX    X..X  -       - X.....X -        X.....X        -   X.X           X.X   -           " +
                "      X..X          -  X...X  -         X...X         -  X..X           X..X  -           " +
                "       XX           -   X.X   -          X.X          - X...XXXXXXXXXXXXX...X -           " +
                "------------        -    X    -           X           -X.....................X-           " +
                "                    ----------------------------------- X...XXXXXXXXXXXXX...X -           " +
                "                                                      -  X..X           X..X  -           " +
                "                                                      -   X.X           X.X   -           " +
                "                                                      -    XX           XX    -           ";

            //ImVector < stbrp_rect > &rects = *(ImVector<stbrp_rect>*)p_rects;
            if (pass == 0)
            {
                // Request rectangles
                var custom = new ImFontPackingRect();
                custom.w = TEX_DATA_W * 2 + 1;
                custom.h = TEX_DATA_H + 1;
                rects.push_back(custom);
                custom.pack(spc);
            }
            else if (pass == 1)
            {
                // Render/copy pixels
                //the first rect in rects will always be custom font data
                var r = rects[0];
                for (int y = 0, n = 0; y < TEX_DATA_H; y++)
                    for (int x = 0; x < TEX_DATA_W; x++, n++)
                    {
                        int offset0 = (r.x + x) + (r.y + y) * TexWidth;
                        int offset1 = offset0 + 1 + TEX_DATA_W;
                        TexPixelsAlpha8[offset0] = (byte)(texture_data[n] == '.' ? 0xFF : 0x00);
                        TexPixelsAlpha8[offset1] = (byte)(texture_data[n] == 'X' ? 0xFF : 0x00);
                    }

                ImVec2 tex_uv_scale = new ImVec2(1.0f / TexWidth, 1.0f / TexHeight);
                TexUvWhitePixel = new ImVec2((r.x + 0.5f) * tex_uv_scale.x, (r.y + 0.5f) * tex_uv_scale.y);

                //TODO: Finish render custom text
                //// Setup mouse cursors
                var cursor_datas = new ImVec2[,]
                {
                    // Pos ........ Size ......... Offset ......
                    { new ImVec2(0, 3),  new ImVec2(12, 19), new ImVec2(0, 0) }, // ImGuiMouseCursor_Arrow
                    { new ImVec2(13, 0), new ImVec2(7, 16),  new ImVec2(4, 8) }, // ImGuiMouseCursor_TextInput
                    { new ImVec2(31, 0), new ImVec2(23, 23), new ImVec2(11, 11) }, // ImGuiMouseCursor_Move
                    { new ImVec2(21, 0), new ImVec2(9, 23),  new ImVec2(5, 11) }, // ImGuiMouseCursor_ResizeNS
                    { new ImVec2(55, 18),new ImVec2(23, 9),  new ImVec2(11, 5) }, // ImGuiMouseCursor_ResizeEW
                    { new ImVec2(73, 0), new ImVec2(17, 17), new ImVec2(9, 9) }, // ImGuiMouseCursor_ResizeNESW
                    { new ImVec2(55, 0), new ImVec2(17, 17), new ImVec2(9, 9) }, // ImGuiMouseCursor_ResizeNWSE
                };

                for (int type = 0; type < 7; type++)
                {
                    ImGuiMouseCursorData cursor_data = ImGui.Instance.State.MouseCursorData[type];
                    ImVec2 pos = cursor_datas[type,0] + new ImVec2((float)r.x, (float)r.y);
                    ImVec2 size = cursor_datas[type,1];
                    cursor_data.Type = (ImGuiMouseCursor)type;
                    cursor_data.Size = size;
                    cursor_data.HotOffset = cursor_datas[type,2];
                    cursor_data.TexUvMin[0] = (pos) * tex_uv_scale;
                    cursor_data.TexUvMax[0] = (pos + size) * tex_uv_scale;
                    pos.x += TEX_DATA_W + 1;
                    cursor_data.TexUvMin[1] = (pos) * tex_uv_scale;
                    cursor_data.TexUvMax[1] = (pos + size) * tex_uv_scale;
                }
            }
        }


        public ImFontAtlas()
        {
            ConfigData = new ImVector<ImFontConfig>();
            Fonts = new ImVector<ImFont>();
        }
        //~ImFontAtlas();

        ImFont AddFont(ImFontConfig font_cfg)
        {
            System.Diagnostics.Debug.Assert(font_cfg.FontData != null && font_cfg.FontDataSize > 0);
            System.Diagnostics.Debug.Assert(font_cfg.SizePixels > 0.0f);

            // Create new font
            if (!font_cfg.MergeMode)
            {
                ImFont font = new ImFont();
                //IM_PLACEMENT_NEW(font) ImFont();
                Fonts.push_back(font);
            }

            ConfigData.push_back(font_cfg);
            ImFontConfig new_font_cfg = ConfigData[ConfigData.Size - 1];
            new_font_cfg.DstFont = Fonts[Fonts.Size - 1];
            if (!new_font_cfg.FontDataOwnedByAtlas)
            {
                //new_font_cfg.FontData = ImGui::MemAlloc(new_font_cfg.FontDataSize);
                new_font_cfg.FontDataOwnedByAtlas = true;
                var fontData = new byte[font_cfg.FontData.Length];
                Array.Copy(font_cfg.FontData, fontData, fontData.Length);
                new_font_cfg.FontData = fontData;

                //memcpy(new_font_cfg.FontData, font_cfg->FontData, (size_t)new_font_cfg.FontDataSize);
            }

            var library = new Library();
            new_font_cfg.Face = new Face(library, new_font_cfg.FontData, 0);
            //new_font_cfg.Face.SetCharSize(0, new_font_cfg.SizePixels, 0, 96);
            new_font_cfg.Face.SetPixelSizes(0, (uint)new_font_cfg.SizePixels);

            // Invalidate texture
            ClearTexData();
            return Fonts[Fonts.Size - 1];
        }
        internal ImFont AddFontDefault(ImFontConfig font_cfg_template = null)
        {
            ImFontConfig font_cfg = font_cfg_template ?? new ImFontConfig();// font_cfg_template != null ? *font_cfg_template : ImFontConfig();
            if (font_cfg_template == null)
            {
                font_cfg.OversampleH = font_cfg.OversampleV = 2;
                font_cfg.PixelSnapH = true;
            }
            if (font_cfg.Name == null)
                font_cfg.Name = "<default>";

            var ttf_compressed_base85 = STB.GetDefaultCompressedFontDataTTFBase85();
            ImFont font = AddFontFromMemoryCompressedBase85TTF(ttf_compressed_base85, 13.0f, font_cfg, GetGlyphRangesDefault());
            return font;
        }
        //TODO: AddFontFromFileTTF
        //ImFont AddFontFromFileTTF(string filename, float size_pixels, ImFontConfig font_cfg_template = null, uint[] glyph_ranges = null)
        //{
        //    int data_size = 0;
        //    byte[] data = ImLoadFileToMemory(filename, "rb", &data_size, 0);
        //    if (data != null)
        //    {
        //        System.Diagnostics.Debug.Assert(false);
        //        return null;
        //    }
        //    ImFontConfig font_cfg = font_cfg_template ?? new ImFontConfig();
        //    if (font_cfg.Name == null)
        //    {
        //        // Store a short copy of filename into into the font name for convenience
        //        font_cfg.Name = System.IO.Path.GetFileName(filename);
        //    }
        //    return AddFontFromMemoryTTF(data, data_size, size_pixels, font_cfg, glyph_ranges);
        //}
        // Transfer ownership of 'ttf_data' to ImFontAtlas, will be deleted after Build()
        ImFont AddFontFromMemoryTTF(byte[] ttf_data, int ttf_size, float size_pixels, ImFontConfig font_cfg_template = null, char[] glyph_ranges = null)
        {
            ImFontConfig font_cfg = font_cfg_template ?? new ImFontConfig();
            System.Diagnostics.Debug.Assert(font_cfg.FontData == null);
            font_cfg.FontData = ttf_data;
            font_cfg.FontDataSize = ttf_size;
            font_cfg.SizePixels = size_pixels;
            if (glyph_ranges != null)
                font_cfg.GlyphRanges = glyph_ranges;
            return AddFont(font_cfg);
        }
        // 'compressed_ttf_data' still owned by caller. Compress with binary_to_compressed_c.cpp
        unsafe ImFont AddFontFromMemoryCompressedTTF(byte* compressed_ttf_data, uint compressed_ttf_size, float size_pixels, ImFontConfig font_cfg_template = null, char[] glyph_ranges = null)
        {
            uint buf_decompressed_size = STB.stb_decompress_length(compressed_ttf_data);
            //unsigned char* buf_decompressed_data = (unsigned char*)ImGui::MemAlloc(buf_decompressed_size);
            byte[] _buf_decompressed_data = new byte[buf_decompressed_size];

            fixed (byte* buf_decompressed_data = _buf_decompressed_data)
            {
                STB.stb_decompress(buf_decompressed_data, compressed_ttf_data, compressed_ttf_size);

                ImFontConfig font_cfg = font_cfg_template ?? new ImFontConfig();

                System.Diagnostics.Debug.Assert(font_cfg.FontData == null);
                font_cfg.FontDataOwnedByAtlas = true;
                return AddFontFromMemoryTTF(_buf_decompressed_data, (int)buf_decompressed_size, size_pixels, font_cfg_template, glyph_ranges);
            }
        }
        // 'compressed_ttf_data_base85' still owned by caller. Compress with binary_to_compressed_c.cpp with -base85 paramaeter
        unsafe ImFont AddFontFromMemoryCompressedBase85TTF(string compressed_ttf_data_base85, float size_pixels, ImFontConfig font_cfg = null, char[] glyph_ranges = null)
        {
            uint compressed_ttf_size = (uint)(((compressed_ttf_data_base85.Length + 4) / 5) * 4);
            byte[] _compressed_ttf = new byte[compressed_ttf_size];
            //void* compressed_ttf = ImGui::MemAlloc((size_t)compressed_ttf_size);
            fixed (byte* compressed_ttf = _compressed_ttf)
            {
                Decode85(compressed_ttf_data_base85, _compressed_ttf);
                ImFont font = AddFontFromMemoryCompressedTTF(compressed_ttf, compressed_ttf_size, size_pixels, font_cfg, glyph_ranges);
                //ImGui::MemFree(compressed_ttf);
                return font;
            }
        }

        // Clear the CPU-side texture data. Saves RAM once the texture has been copied to graphics memory.
        void ClearTexData()
        {
            TexPixelsAlpha8 = null;
            TexPixelsRGBA32 = null;
        }
        // Clear the input TTF data (inc sizes, glyph ranges)
        void ClearInputData()
        {
            for (int i = 0; i < ConfigData.Size; i++)
                if (ConfigData[i].FontData != null && ConfigData[i].FontDataOwnedByAtlas)
                {
                    //ImGui::MemFree(ConfigData[i].FontData);
                    ConfigData[i].FontData = null;
                }

            // When clearing this we lose access to the font name and other information used to build the font.
            for (int i = 0; i < Fonts.Size; i++)
                if (Fonts[i].ConfigData.FontData == null)
                {
                    Fonts[i].ConfigData = null;
                    Fonts[i].ConfigDataCount = 0;
                }
            ConfigData.clear();
        }

        // Clear the ImGui-side font data (glyphs storage, UV coordinates)
        void ClearFonts()
        {
            //for (int i = 0; i < Fonts.Size; i++)
            //{
            //    Fonts[i]->~ImFont();
            //    ImGui::MemFree(Fonts[i]);
            //}
            Fonts.clear();
        }

        // Clear all
        internal void Clear()
        {
            ClearInputData();
            ClearTexData();
            ClearFonts();
        }


        // Retrieve texture data
        // User is in charge of copying the pixels into graphics memory, then call SetTextureUserID()
        // After loading the texture into your graphic system, store your texture handle in 'TexID' (ignore if you aren't using multiple fonts nor images)
        // RGBA32 format is provided for convenience and high compatibility, but note that all RGB pixels are white, so 75% of the memory is wasted.
        // Pitch = Width * BytesPerPixels
        // 1 byte per-pixel
        public byte[] GetTexDataAsAlpha8(out int out_width, out int out_height)
        {
            // Build atlas on demand
            if (TexPixelsAlpha8 == null)
            {
                if (ConfigData.empty())
                    AddFontDefault();
                Build();
            }

            out_width = TexWidth;
            out_height = TexHeight;

            //if (out_bytes_per_pixel) *out_bytes_per_pixel = 1;
            return TexPixelsAlpha8;
        }

        // 4 bytes-per-pixel
        public byte[] GetTexDataAsRGBA32(out int out_width, out int out_height)
        {
            out_width = TexWidth;
            out_height = TexHeight;

            // Convert to RGBA32 format on demand
            // Although it is likely to be the most commonly used format, our font rendering is 1 channel / 8 bpp
            if (TexPixelsRGBA32 == null)
            {
                //unsigned char* pixels;
                var pixels = GetTexDataAsAlpha8(out out_width, out out_height);
                TexPixelsRGBA32 = new byte[TexWidth * TexHeight * 4];

                //const unsigned char* src = pixels;
                //unsigned int* dst = TexPixelsRGBA32;
                var dst = 0;
                for (var n = 0; n < TexWidth * TexHeight; n++)
                {
                    TexPixelsRGBA32[dst++] = pixels[n];
                    TexPixelsRGBA32[dst++] = 0xff;
                    TexPixelsRGBA32[dst++] = 0xff;
                    TexPixelsRGBA32[dst++] = 0xff;
                    //*dst++ = ((unsigned int)(*src++) << 24) | 0x00FFFFFF;

                }
            }

            return TexPixelsRGBA32;
            //*out_pixels = (unsigned char*)TexPixelsRGBA32;
            //if (out_width) *out_width = TexWidth;
            //if (out_height) *out_height = TexHeight;
            //if (out_bytes_per_pixel) *out_bytes_per_pixel = 4;
        }

        // 4 bytes-per-pixel
        public byte[] GetTexDataAsARGB32(out int out_width, out int out_height)
        {
            out_width = TexWidth;
            out_height = TexHeight;

            // Convert to RGBA32 format on demand
            // Although it is likely to be the most commonly used format, our font rendering is 1 channel / 8 bpp
            if (TexPixelsRGBA32 == null)
            {
                //unsigned char* pixels;
                var pixels = GetTexDataAsAlpha8(out out_width, out out_height);
                TexPixelsRGBA32 = new byte[TexWidth * TexHeight * 4];

                //const unsigned char* src = pixels;
                //unsigned int* dst = TexPixelsRGBA32;
                var dst = 0;
                for (var n = 0; n < TexWidth * TexHeight; n++)
                {
                    TexPixelsRGBA32[dst++] = 0xff;
                    TexPixelsRGBA32[dst++] = 0xff;
                    TexPixelsRGBA32[dst++] = 0xff;
                    TexPixelsRGBA32[dst++] = pixels[n];
                    //*dst++ = ((unsigned int)(*src++) << 24) | 0x00FFFFFF;

                }
            }

            return TexPixelsRGBA32;
            //*out_pixels = (unsigned char*)TexPixelsRGBA32;
            //if (out_width) *out_width = TexWidth;
            //if (out_height) *out_height = TexHeight;
            //if (out_bytes_per_pixel) *out_bytes_per_pixel = 4;
        }

        //public void SetTexID(void* id) { TexID = id; }

        // Helpers to retrieve list of common Unicode ranges (2 value per range, values are inclusive, zero-terminated list)
        // (Those functions could be static but aren't so most users don't have to refer to the ImFontAtlas:: name ever if in their code; just using io.Fonts->)
        //public  ImWchar* GetGlyphRangesDefault();    // Basic Latin, Extended Latin
        //public  ImWchar* GetGlyphRangesKorean();     // Default + Korean characters
        //public  ImWchar* GetGlyphRangesJapanese();   // Default + Hiragana, Katakana, Half-Width, Selection of 1946 Ideographs
        //public  ImWchar* GetGlyphRangesChinese();    // Japanese + full set of about 21000 CJK Unified Ideographs
        //public  ImWchar* GetGlyphRangesCyrillic();   // Default + about 400 Cyrillic characters

    }
}
