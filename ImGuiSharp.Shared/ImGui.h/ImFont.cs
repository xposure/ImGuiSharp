namespace ImGui
{
    // Runtime data for a single font within a parent ImFontAtlas
    // Font runtime data and rendering
    // ImFontAtlas automatically loads a default embedded font for you when you call GetTexDataAsAlpha8() or GetTexDataAsRGBA32().
    public class ImFont
    {
        // Members: Settings
        internal float FontSize;                 // <user set>      // Height of characters, set during loading (don't change after loading)
        internal float Scale;                    // = 1.0f          // Base font scale, multiplied by the per-window font scale which you can adjust with SetFontScale()
        internal ImVec2 DisplayOffset;           // = (0.0f,1.0f)   // Offset font rendering by xx pixels
        internal char FallbackChar;              // = '?'           // Replacement glyph if one isn't found. Only set via SetFallbackChar()
        internal ImFontConfig ConfigData;        //                 // Pointer within ImFontAtlas->ConfigData
        internal int ConfigDataCount;            //

        // Members: Runtime data
        internal struct Glyph
        {
            internal uint Codepoint;
            internal float XAdvance;
            internal float X0, Y0, X1, Y1;
            internal float U0, V0, U1, V1;     // Texture coordinates
        }

        internal float Ascent, Descent;    // Ascent: distance from top to bottom of e.g. 'A' [0..FontSize]
        internal ImFontAtlas ContainerAtlas;     // What we has been loaded into
        internal ImVector<Glyph> Glyphs;
        internal Glyph? FallbackGlyph;      // == FindGlyph(FontFallbackChar)
        internal float FallbackXAdvance;   //
        internal ImVector<float> IndexXAdvance;      // Sparse. Glyphs->XAdvance directly indexable (more cache-friendly that reading from Glyphs, for CalcTextSize functions which are often bottleneck in large UI)
        internal ImVector<int> IndexLookup;        // Sparse. Index glyphs by Unicode code-point.

        // Methods
        public ImFont()
        {
            Glyphs = new ImVector<Glyph>();
            IndexXAdvance = new ImVector<float>();
            IndexLookup = new ImVector<int>();
            Scale = 1.0f;
            FallbackChar = '?';
            Clear();
        }

        //~ImFont();

        void Clear()
        {
            FontSize = 0.0f;
            DisplayOffset = new ImVec2(0.0f, 1.0f);
            ConfigData = null;
            ConfigDataCount = 0;
            Ascent = Descent = 0.0f;
            ContainerAtlas = null;
            Glyphs.clear();
            FallbackGlyph = null;
            FallbackXAdvance = 0.0f;
            IndexXAdvance.clear();
            IndexLookup.clear();
        }

        internal void BuildLookupTable()
        {
            int max_codepoint = 0;
            for (int i = 0; i != Glyphs.Size; i++)
                max_codepoint = ImGui.Max(max_codepoint, (int)Glyphs[i].Codepoint);

            IndexXAdvance.clear();
            IndexXAdvance.resize(max_codepoint + 1);
            IndexLookup.clear();
            IndexLookup.resize(max_codepoint + 1);
            for (int i = 0; i < max_codepoint + 1; i++)
            {
                IndexXAdvance[i] = -1.0f;
                IndexLookup[i] = -1;
            }
            for (int i = 0; i < Glyphs.Size; i++)
            {
                int codepoint = (int)Glyphs[i].Codepoint;
                IndexXAdvance[codepoint] = Glyphs[i].XAdvance;
                IndexLookup[codepoint] = i;
            }

            // Create a glyph to handle TAB
            // FIXME: Needs proper TAB handling but it needs to be contextualized (or we could arbitrary say that each string starts at "column 0" ?)
            Glyph glyph;
            if (FindGlyph(' ', out glyph))
            {
                var size = Glyphs.Size;
                if (Glyphs[size - 1].Codepoint != '\t')
                    Glyphs.resize(++size);

                //if (Glyphs.back().Codepoint != '\t')   // So we can call this function multiple times
                //    Glyphs.resize(Glyphs.Size + 1);

                Glyph tab_glyph;
                FindGlyph(' ', out tab_glyph); //what if we don't find it?

                tab_glyph.Codepoint = '\t';
                tab_glyph.XAdvance *= 4;

                Glyphs[size - 1] = tab_glyph; //assign it back since we aren't working with refs

                IndexXAdvance[(int)tab_glyph.Codepoint] = (float)tab_glyph.XAdvance;
                IndexLookup[(int)tab_glyph.Codepoint] = (int)(Glyphs.Size - 1);
            }

            FallbackGlyph = null;
            Glyph findFallback;
            if (FindGlyph(FallbackChar, out findFallback))
            {
                FallbackGlyph = findFallback;
                FallbackXAdvance = findFallback.XAdvance;
            }

            for (int i = 0; i < max_codepoint + 1; i++)
                if (IndexXAdvance[i] < 0.0f)
                    IndexXAdvance[i] = FallbackXAdvance;
        }

        internal bool HasGlyph(char c)
        {
            if (c < IndexLookup.Size)
            {
                int i = IndexLookup[c];
                if (i != -1)
                    return true;
            }
            return false;
        }

        internal bool FindGlyph(char c, out Glyph glyph)
        {
            if (c < IndexLookup.Size)
            {
                int i = IndexLookup[c];
                if (i != -1)
                {
                    glyph = Glyphs[i];
                    return true;
                }
            }

            if (FallbackGlyph.HasValue)
                glyph = FallbackGlyph.Value;
            else
                glyph = new Glyph();

            return false;
        }

        void SetFallbackChar(char c)
        {
            FallbackChar = c;
            BuildLookupTable();
        }

        internal float GetCharAdvance(ushort c) { return (c < IndexXAdvance.Size) ? IndexXAdvance[c] : FallbackXAdvance; }
        internal bool IsLoaded() { return ContainerAtlas != null; }

        // 'max_width' stops rendering after a certain width (could be turned into a 2d size). FLT_MAX to disable.
        // 'wrap_width' enable automatic word-wrapping across multiple lines to fit into given width. 0.0f to disable.
        //ImVec2 CalcTextSizeA(float size, float max_width, float wrap_width, const char* text_begin, const char* text_end = NULL, const char** remaining = NULL){
        internal int CalcTextSizeA(out ImVec2 textSize, float size, float max_width, float wrap_width, string text, int text_begin, int text_end = -1)
        {
            if (text_end == -1)
                text_end = text.Length; // FIXME-OPT: Need to avoid this.

            float line_height = size;
            float scale = size / FontSize;

            ImVec2 text_size = new ImVec2(0, 0);
            float line_width = 0.0f;

            bool word_wrap_enabled = (wrap_width > 0.0f);
            int word_wrap_eol = -1;

            int s = text_begin;
            while (s < text_end)
            {
                if (word_wrap_enabled)
                {
                    // Calculate how far we can render. Requires two passes on the string data but keeps the code simple and not intrusive for what's essentially an uncommon feature.
                    if (word_wrap_eol == -1)
                    {
                        word_wrap_eol = CalcWordWrapPositionA(scale, text, s, text_end, wrap_width - line_width);
                        if (word_wrap_eol == s) // Wrap_width is too small to fit anything. Force displaying 1 character to minimize the height discontinuity.
                            word_wrap_eol++;    // +1 may not be a character start point in UTF-8 but it's ok because we use s >= word_wrap_eol below
                    }

                    if (s >= word_wrap_eol)
                    {
                        if (text_size.x < line_width)
                            text_size.x = line_width;

                        text_size.y += line_height;
                        line_width = 0.0f;
                        word_wrap_eol = -1;

                        // Wrapping skips upcoming blanks
                        while (s < text_end)
                        {
                            char wc = text[s];
                            if (char.IsSeparator(wc) ) { s++; }
                            //else if (wc == '\t') { s++; break; }
                            else if (wc == '\n') { s++; break; }
                            else { break; }
                        }
                        continue;
                    }
                }

                // Decode and advance source
                int prev_s = s;
                char c = text[s++];
                //if (c < 0x80)
                //{
                //    s += 1;
                //}
                //else
                //{
                //    s += ImTextCharFromUtf8(&c, s, text_end);
                //    if (c == 0)
                //        break;
                //}

                if (c < 32)
                {
                    if (c == '\n')
                    {
                        text_size.x = ImGui.Max(text_size.x, line_width);
                        text_size.y += line_height;
                        line_width = 0.0f;
                        continue;
                    }
                    if (c == '\r')
                        continue;
                }

                float char_width = (c < IndexXAdvance.Size ? IndexXAdvance[c] : FallbackXAdvance) * scale;
                if (line_width + char_width >= max_width)
                {
                    s = prev_s;
                    break;
                }

                line_width += char_width;
            }

            if (text_size.x < line_width)

                text_size.x = line_width;

            if (line_width > 0 || text_size.y == 0.0f)
                text_size.y += line_height;

            textSize = text_size;
            return s; //return the position we stopped at
        }
        int CalcWordWrapPositionA(float scale, string text, int text_begin, int text_end, float wrap_width)
        {
            // Simple word-wrapping for English, not full-featured. Please submit failing cases!
            // FIXME: Much possible improvements (don't cut things like "word !", "word!!!" but cut within "word,,,,", more sensible support for punctuations, support for Unicode punctuations, etc.)

            // For references, possible wrap point marked with ^
            //  "aaa bbb, ccc,ddd. eee   fff. ggg!"
            //      ^    ^    ^   ^   ^__    ^    ^

            // List of hardcoded separators: .,;!?'"

            // Skip extra blanks after a line returns (that includes not counting them in width computation)
            // e.g. "Hello    world" --> "Hello" "World"

            // Cut words that cannot possibly fit within one line.
            // e.g.: "The tropical fish" with ~5 characters worth of width --> "The tr" "opical" "fish"

            float line_width = 0.0f;
            float word_width = 0.0f;
            float blank_width = 0.0f;

            int word_end = text_begin;
            int prev_word_end = -1;
            bool inside_word = true;

            int s = text_begin;
            while (s < text_end)
            {
                char c = text[s];
                int next_s = s + 1;
                //if (c < 0x80)
                //    next_s = s + 1;
                //else
                //    next_s = s + ImTextCharFromUtf8(&c, s, text_end);
                if (c == 0)
                    break;

                if (c < 32)
                {
                    if (c == '\n')
                    {
                        line_width = word_width = blank_width = 0.0f;
                        inside_word = true;
                        s = next_s;
                        continue;
                    }
                    if (c == '\r')
                    {
                        s = next_s;
                        continue;
                    }
                }

                float char_width = ((int)c < IndexXAdvance.Size) ? IndexXAdvance[(int)c] * scale : FallbackXAdvance;
                if (char.IsSeparator(c))
                {
                    if (inside_word)
                    {
                        line_width += blank_width;
                        blank_width = 0.0f;
                    }
                    blank_width += char_width;
                    inside_word = false;
                }
                else
                {
                    word_width += char_width;
                    if (inside_word)
                    {
                        word_end = next_s;
                    }
                    else
                    {
                        prev_word_end = word_end;
                        line_width += word_width + blank_width;
                        word_width = blank_width = 0.0f;
                    }

                    // Allow wrapping after punctuation.
                    inside_word = !(c == '.' || c == ',' || c == ';' || c == '!' || c == '?' || c == '\"');
                }

                // We ignore blank width at the end of the line (they can be skipped)
                if (line_width + word_width >= wrap_width)
                {
                    // Words that cannot possibly fit within an entire line will be cut anywhere.
                    if (word_width < wrap_width)
                        s = prev_word_end > -1 ? prev_word_end : word_end;
                    break;
                }

                s = next_s;
            }

            return s;
        }

        int CalcWordWrapPositionA(float scale, char[] text, int text_begin, int text_end, float wrap_width)
        {
            // Simple word-wrapping for English, not full-featured. Please submit failing cases!
            // FIXME: Much possible improvements (don't cut things like "word !", "word!!!" but cut within "word,,,,", more sensible support for punctuations, support for Unicode punctuations, etc.)

            // For references, possible wrap point marked with ^
            //  "aaa bbb, ccc,ddd. eee   fff. ggg!"
            //      ^    ^    ^   ^   ^__    ^    ^

            // List of hardcoded separators: .,;!?'"

            // Skip extra blanks after a line returns (that includes not counting them in width computation)
            // e.g. "Hello    world" --> "Hello" "World"

            // Cut words that cannot possibly fit within one line.
            // e.g.: "The tropical fish" with ~5 characters worth of width --> "The tr" "opical" "fish"

            float line_width = 0.0f;
            float word_width = 0.0f;
            float blank_width = 0.0f;

            int word_end = text_begin;
            int prev_word_end = -1;
            bool inside_word = true;

            int s = text_begin;
            while (s < text_end)
            {
                char c = text[s];
                int next_s = s + 1;
                //if (c < 0x80)
                //    next_s = s + 1;
                //else
                //    next_s = s + ImTextCharFromUtf8(&c, s, text_end);
                if (c == 0)
                    break;

                if (c < 32)
                {
                    if (c == '\n')
                    {
                        line_width = word_width = blank_width = 0.0f;
                        inside_word = true;
                        s = next_s;
                        continue;
                    }
                    if (c == '\r')
                    {
                        s = next_s;
                        continue;
                    }
                }

                float char_width = ((int)c < IndexXAdvance.Size) ? IndexXAdvance[(int)c] * scale : FallbackXAdvance;
                if (char.IsSeparator(c))
                {
                    if (inside_word)
                    {
                        line_width += blank_width;
                        blank_width = 0.0f;
                    }
                    blank_width += char_width;
                    inside_word = false;
                }
                else
                {
                    word_width += char_width;
                    if (inside_word)
                    {
                        word_end = next_s;
                    }
                    else
                    {
                        prev_word_end = word_end;
                        line_width += word_width + blank_width;
                        word_width = blank_width = 0.0f;
                    }

                    // Allow wrapping after punctuation.
                    inside_word = !(c == '.' || c == ',' || c == ';' || c == '!' || c == '?' || c == '\"');
                }

                // We ignore blank width at the end of the line (they can be skipped)
                if (line_width + word_width >= wrap_width)
                {
                    // Words that cannot possibly fit within an entire line will be cut anywhere.
                    if (word_width < wrap_width)
                        s = prev_word_end > -1 ? prev_word_end : word_end;
                    break;
                }

                s = next_s;
            }

            return s;
        }

        internal ImRect RenderText(float size, ImVec2 pos, uint col, ImVec4 clip_rect, string text, int text_begin, int text_end, ImDrawList draw_list, float wrap_width = 0.0f, bool cpu_fine_clip = false)
        {
            if (text_end == -1)
                text_end = text.Length; // FIXME-OPT: Need to avoid this.

            // Align to be pixel perfect
            pos.x = (int)pos.x + DisplayOffset.x;
            pos.y = (int)pos.y + DisplayOffset.y;
            float x = pos.x;
            float y = pos.y;
            if (y > clip_rect.w)
                return new ImRect();

            float scale = size / FontSize;
            float line_height = FontSize * scale;
            bool word_wrap_enabled = (wrap_width > 0.0f);
            int word_wrap_eol = -1;

            int vtx_write = draw_list._VtxWritePtr;
            int idx_write = draw_list._IdxWritePtr;
            uint vtx_current_idx = draw_list._VtxCurrentIdx;

            int s = text_begin;
            if (!word_wrap_enabled && y + line_height < clip_rect.y)
                while (s < text_end && text[s] != '\n')  // Fast-forward to next line
                    s++;

            while (s < text_end)
            {
                if (word_wrap_enabled)
                {
                    // Calculate how far we can render. Requires two passes on the string data but keeps the code simple and not intrusive for what's essentially an uncommon feature.
                    if (word_wrap_eol == -1)
                    {
                        word_wrap_eol = CalcWordWrapPositionA(scale, text, s, text_end, wrap_width - (x - pos.x));
                        if (word_wrap_eol == s) // Wrap_width is too small to fit anything. Force displaying 1 character to minimize the height discontinuity.
                            word_wrap_eol++;    // +1 may not be a character start point in UTF-8 but it's ok because we use s >= word_wrap_eol below
                    }

                    if (s >= word_wrap_eol)
                    {
                        x = pos.x;
                        y += line_height;
                        word_wrap_eol = -1;

                        // Wrapping skips upcoming blanks
                        while (s < text_end)
                        {
                            char wc = text[s];
                            if (char.IsSeparator(wc)) { s++; }
                            else if (wc == '\n') { s++; break; }
                            else { break; }
                        }
                        continue;
                    }
                }

                // Decode and advance source
                char c = text[s++];
                //if (c< 0x80)
                //{
                // s += 1;
                //}
                //else
                //{
                // s += ImTextCharFromUtf8(&c, s, text_end);
                // if (c == 0)
                //  break;
                //}

                if (c < 32)
                {
                    if (c == '\n')
                    {
                        x = pos.x;
                        y += line_height;

                        if (y > clip_rect.w)
                            break;
                        if (!word_wrap_enabled && y + line_height < clip_rect.y)
                            while (s < text_end && text[s] != '\n')  // Fast-forward to next line
                                s++;

                        continue;
                    }
                    if (c == '\r')
                        continue;
                }

                float char_width = 0.0f;
                Glyph glyph;
                if (FindGlyph(c, out glyph))
                {
                    char_width = glyph.XAdvance * scale;

                    // Arbitrarily assume that both space and tabs are empty glyphs as an optimization
                    if (c != ' ' && c != '\t')
                    {
                        // We don't do a second finer clipping test on the Y axis as we've already skipped anything before clip_rect.y and exit once we pass clip_rect.w
                        float y1 = (float)(y + glyph.Y0 * scale);
                        float y2 = (float)(y + glyph.Y1 * scale);

                        float x1 = (float)(x + glyph.X0 * scale);
                        float x2 = (float)(x + glyph.X1 * scale);
                        if (x1 <= clip_rect.z && x2 >= clip_rect.x)
                        {
                            // Render a character
                            float u1 = glyph.U0;
                            float v1 = glyph.V0;
                            float u2 = glyph.U1;
                            float v2 = glyph.V1;

                            // CPU side clipping used to fit text in their frame when the frame is too small. Only does clipping for axis aligned quads.
                            if (cpu_fine_clip)
                            {
                                if (x1 < clip_rect.x)
                                {
                                    u1 = u1 + (1.0f - (x2 - clip_rect.x) / (x2 - x1)) * (u2 - u1);
                                    x1 = clip_rect.x;
                                }
                                if (y1 < clip_rect.y)
                                {
                                    v1 = v1 + (1.0f - (y2 - clip_rect.y) / (y2 - y1)) * (v2 - v1);
                                    y1 = clip_rect.y;
                                }
                                if (x2 > clip_rect.z)
                                {
                                    u2 = u1 + ((clip_rect.z - x1) / (x2 - x1)) * (u2 - u1);
                                    x2 = clip_rect.z;
                                }
                                if (y2 > clip_rect.w)
                                {
                                    v2 = v1 + ((clip_rect.w - y1) / (y2 - y1)) * (v2 - v1);
                                    y2 = clip_rect.w;
                                }
                                if (y1 >= y2)
                                {
                                    x += char_width;
                                    continue;
                                }
                            }

                            // We are NOT calling PrimRectUV() here because non-inlined causes too much overhead in a debug build.
                            // Inlined here:
                            {
                                draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 1); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 2);
                                draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 2); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 3);
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x1, y1), uv = new ImVec2(u1, v1), col = col };
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x2, y1), uv = new ImVec2(u2, v1), col = col };
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x2, y2), uv = new ImVec2(u2, v2), col = col };
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x1, y2), uv = new ImVec2(u1, v2), col = col };
                                vtx_current_idx += 4;
                            }
                        }
                    }
                }

                x += char_width;
            }

            draw_list._VtxWritePtr = vtx_write;
            draw_list._VtxCurrentIdx = vtx_current_idx;
            draw_list._IdxWritePtr = idx_write;
            return new ImRect(pos, new ImVec2(x, y + line_height));
        }

        internal ImRect RenderText(float size, ImVec2 pos, uint col, ImVec4 clip_rect, char[] text, int text_begin, int text_end, ImDrawList draw_list, float wrap_width = 0.0f, bool cpu_fine_clip = false)
        {
            if (text_end == -1)
                text_end = text.Length; // FIXME-OPT: Need to avoid this.

            // Align to be pixel perfect
            pos.x = (int)pos.x + DisplayOffset.x;
            pos.y = (int)pos.y + DisplayOffset.y;
            float x = pos.x;
            float y = pos.y;
            if (y > clip_rect.w)
                return new ImRect();

            float scale = size / FontSize;
            float line_height = FontSize * scale;
            bool word_wrap_enabled = (wrap_width > 0.0f);
            int word_wrap_eol = -1;

            int vtx_write = draw_list._VtxWritePtr;
            int idx_write = draw_list._IdxWritePtr;
            uint vtx_current_idx = draw_list._VtxCurrentIdx;

            int s = text_begin;
            if (!word_wrap_enabled && y + line_height < clip_rect.y)
                while (s < text_end && text[s] != '\n')  // Fast-forward to next line
                    s++;

            while (s < text_end)
            {
                if (word_wrap_enabled)
                {
                    // Calculate how far we can render. Requires two passes on the string data but keeps the code simple and not intrusive for what's essentially an uncommon feature.
                    if (word_wrap_eol != -1)
                    {
                        word_wrap_eol = CalcWordWrapPositionA(scale, text, s, text_end, wrap_width - (x - pos.x));
                        if (word_wrap_eol == s) // Wrap_width is too small to fit anything. Force displaying 1 character to minimize the height discontinuity.
                            word_wrap_eol++;    // +1 may not be a character start point in UTF-8 but it's ok because we use s >= word_wrap_eol below
                    }

                    if (s >= word_wrap_eol)
                    {
                        x = pos.x;
                        y += line_height;
                        word_wrap_eol = -1;

                        // Wrapping skips upcoming blanks
                        while (s < text_end)
                        {
                            char wc = text[s];
                            if (char.IsSeparator(wc)) { s++; }
                            else if (wc == '\n') { s++; break; }
                            else { break; }
                        }
                        continue;
                    }
                }

                // Decode and advance source
                char c = text[s++];
                //if (c< 0x80)
                //{
                // s += 1;
                //}
                //else
                //{
                // s += ImTextCharFromUtf8(&c, s, text_end);
                // if (c == 0)
                //  break;
                //}

                if (c < 32)
                {
                    if (c == '\n')
                    {
                        x = pos.x;
                        y += line_height;

                        if (y > clip_rect.w)
                            break;
                        if (!word_wrap_enabled && y + line_height < clip_rect.y)
                            while (s < text_end && text[s] != '\n')  // Fast-forward to next line
                                s++;

                        continue;
                    }
                    if (c == '\r')
                        continue;
                }

                float char_width = 0.0f;
                Glyph glyph;
                if (FindGlyph(c, out glyph))
                {
                    char_width = glyph.XAdvance * scale;

                    // Arbitrarily assume that both space and tabs are empty glyphs as an optimization
                    if (c != ' ' && c != '\t')
                    {
                        // We don't do a second finer clipping test on the Y axis as we've already skipped anything before clip_rect.y and exit once we pass clip_rect.w
                        float y1 = (float)(y + glyph.Y0 * scale);
                        float y2 = (float)(y + glyph.Y1 * scale);

                        float x1 = (float)(x + glyph.X0 * scale);
                        float x2 = (float)(x + glyph.X1 * scale);
                        if (x1 <= clip_rect.z && x2 >= clip_rect.x)
                        {
                            // Render a character
                            float u1 = glyph.U0;
                            float v1 = glyph.V0;
                            float u2 = glyph.U1;
                            float v2 = glyph.V1;

                            // CPU side clipping used to fit text in their frame when the frame is too small. Only does clipping for axis aligned quads.
                            if (cpu_fine_clip)
                            {
                                if (x1 < clip_rect.x)
                                {
                                    u1 = u1 + (1.0f - (x2 - clip_rect.x) / (x2 - x1)) * (u2 - u1);
                                    x1 = clip_rect.x;
                                }
                                if (y1 < clip_rect.y)
                                {
                                    v1 = v1 + (1.0f - (y2 - clip_rect.y) / (y2 - y1)) * (v2 - v1);
                                    y1 = clip_rect.y;
                                }
                                if (x2 > clip_rect.z)
                                {
                                    u2 = u1 + ((clip_rect.z - x1) / (x2 - x1)) * (u2 - u1);
                                    x2 = clip_rect.z;
                                }
                                if (y2 > clip_rect.w)
                                {
                                    v2 = v1 + ((clip_rect.w - y1) / (y2 - y1)) * (v2 - v1);
                                    y2 = clip_rect.w;
                                }
                                if (y1 >= y2)
                                {
                                    x += char_width;
                                    continue;
                                }
                            }

                            // We are NOT calling PrimRectUV() here because non-inlined causes too much overhead in a debug build.
                            // Inlined here:
                            {
                                draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 1); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 2);
                                draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 2); draw_list.IdxBuffer[idx_write++] = (ImDrawIdx)(vtx_current_idx + 3);
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x1, y1), uv = new ImVec2(u1, v1), col = col };
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x2, y1), uv = new ImVec2(u2, v1), col = col };
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x2, y2), uv = new ImVec2(u2, v2), col = col };
                                draw_list.VtxBuffer[vtx_write++] = new ImDrawVert() { pos = new ImVec2(x1, y2), uv = new ImVec2(u1, v2), col = col };
                                vtx_current_idx += 4;
                            }
                        }
                    }
                }

                x += char_width;
            }

            draw_list._VtxWritePtr = vtx_write;
            draw_list._VtxCurrentIdx = vtx_current_idx;
            draw_list._IdxWritePtr = idx_write;
            return new ImRect(pos, new ImVec2(x, y + line_height));
        }

    }
}
