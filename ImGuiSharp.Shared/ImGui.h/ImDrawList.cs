
namespace ImGui
{
    using System;

    // A single draw command list (generally one per window)
    // Draw command list
    // This is the low-level list of polygons that ImGui functions are filling. At the end of the frame, all command lists are passed to your ImGuiIO::RenderDrawListFn function for rendering.
    // At the moment, each ImGui window contains its own ImDrawList but they could potentially be merged in the future.
    // If you want to add custom rendering within a window, you can use ImGui::GetWindowDrawList() to access the current draw list and add your own primitives.
    // You can interleave normal ImGui:: calls and adding primitives to the current draw list.
    // All positions are in screen coordinates (0,0=top-left, 1 pixel per unit). Primitives are always added to the list and not culled (culling is done at render time and at a higher-level by ImGui:: functions).
    public class ImDrawList : IDisposable
    {
        internal static ImVec4 GNullClipRect = new ImVec4(-8192.0f, -8192.0f, +8192.0f, +8192.0f);

        internal static void PathBezierToCasteljau(ImVector<ImVec2> path, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float tess_tol, int level)
        {
            float dx = x4 - x1;
            float dy = y4 - y1;
            float d2 = ((x2 - x4) * dy - (y2 - y4) * dx);
            float d3 = ((x3 - x4) * dy - (y3 - y4) * dx);
            d2 = (d2 >= 0) ? d2 : -d2;
            d3 = (d3 >= 0) ? d3 : -d3;
            if ((d2 + d3) * (d2 + d3) < tess_tol * (dx * dx + dy * dy))
            {
                path.push_back(new ImVec2(x4, y4));
            }
            else if (level < 10)
            {
                float x12 = (x1 + x2) * 0.5f, y12 = (y1 + y2) * 0.5f;
                float x23 = (x2 + x3) * 0.5f, y23 = (y2 + y3) * 0.5f;
                float x34 = (x3 + x4) * 0.5f, y34 = (y3 + y4) * 0.5f;
                float x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                float x234 = (x23 + x34) * 0.5f, y234 = (y23 + y34) * 0.5f;
                float x1234 = (x123 + x234) * 0.5f, y1234 = (y123 + y234) * 0.5f;

                PathBezierToCasteljau(path, x1, y1, x12, y12, x123, y123, x1234, y1234, tess_tol, level + 1);
                PathBezierToCasteljau(path, x1234, y1234, x234, y234, x34, y34, x4, y4, tess_tol, level + 1);
            }
        }

        // This is what you have to render
        internal ImVector<ImDrawCmd> CmdBuffer;              // Commands. Typically 1 command = 1 gpu draw call.

        internal ImVector<ImDrawIdx> IdxBuffer;              // Index buffer. Each command consume ImDrawCmd::ElemCount of those
        internal ImVector<ImDrawVert> VtxBuffer;             // Vertex buffer.

        // [Internal, used while building lists]
        internal string _OwnerName;                          // Pointer to owner window's name (if any) for debugging

        internal uint _VtxCurrentIdx;                        // [Internal] == VtxBuffer.Size
        internal int _VtxWritePtr;                         // [Internal] point within VtxBuffer.Data after each add command (to avoid using the ImVector<> operators too much)
        internal int _IdxWritePtr;                         // [Internal] point within IdxBuffer.Data after each add command (to avoid using the ImVector<> operators too much)
        internal ImVector<ImVec4> _ClipRectStack;            // [Internal]
        internal ImVector<ImTextureID> _TextureIdStack;      // [Internal]
        internal ImVector<ImVec2> _Path;                     // [Internal] current path building
        internal int _ChannelsCurrent;                       // [Internal] current channel number (0)
        internal int _ChannelsCount;                         // [Internal] number of active channels (1+)
        internal ImVector<ImDrawChannel> _Channels;          // [Internal] draw channels for columns API (not resized down so _ChannelsCount may be smaller than _Channels.Size)

        public ImDrawList(string owner = null)
        {
            _OwnerName = owner;
            CmdBuffer = new ImVector<ImDrawCmd>();
            IdxBuffer = new ImVector<ImDrawIdx>();
            VtxBuffer = new ImVector<ImDrawVert>();
            _ClipRectStack = new ImVector<ImVec4>();
            _TextureIdStack = new ImVector<ImTextureID>();
            _Path = new ImVector<ImVec2>();
            _Channels = new ImVector<ImDrawChannel>();

            Clear();
        }

        internal ImDrawCmd? GetCurrentDrawCmd()
        {
            return CmdBuffer.Size > 0 ? (ImDrawCmd?)CmdBuffer[CmdBuffer.Size - 1] : null;
        }

        internal void SetCurrentDrawCmd(ImDrawCmd cmd)
        {
            System.Diagnostics.Debug.Assert(CmdBuffer.Size > 0);
            CmdBuffer[CmdBuffer.Size - 1] = cmd;
        }

        internal ImDrawCmd? GetPreviousDrawCmd()
        {
            return CmdBuffer.Size > 1 ? (ImDrawCmd?)CmdBuffer[CmdBuffer.Size - 2] : null;
        }

        public void Dispose()
        {
            ClearFreeMemory();
        }
        //~ImDrawList() { ClearFreeMemory(); }

        //// Internal helpers
        //// NB: all primitives needs to be reserved via PrimReserve() beforehand!
        internal void Clear()
        {
            CmdBuffer.resize(0);
            IdxBuffer.resize(0);
            VtxBuffer.resize(0);
            _VtxCurrentIdx = 0;
            _VtxWritePtr = -1;
            _IdxWritePtr = -1;
            _ClipRectStack.resize(0);
            _TextureIdStack.resize(0);
            _Path.resize(0);
            _ChannelsCurrent = 0;
            _ChannelsCount = 1;
            // NB: Do not clear channels so our allocations are re-used after the first frame.
        }

        internal void ClearFreeMemory()
        {
            CmdBuffer.clear();
            IdxBuffer.clear();
            VtxBuffer.clear();
            _VtxCurrentIdx = 0;
            _VtxWritePtr = -1;
            _IdxWritePtr = -1;
            _ClipRectStack.clear();
            _TextureIdStack.clear();
            _Path.clear();
            _ChannelsCurrent = 0;
            _ChannelsCount = 1;
            for (int i = 0; i < _Channels.Size; i++)
            {
                //if (i == 0) memset(&_Channels[0], 0, sizeof(_Channels[0]));  // channel 0 is a copy of CmdBuffer/IdxBuffer, don't destruct again
                _Channels[i].CmdBuffer.clear();
                _Channels[i].IdxBuffer.clear();
            }
            _Channels.clear();
        }

        internal void PushClipRect(ImVec4 clip_rect)  // Scissoring. Note that the values are (x1,y1,x2,y2) and NOT (x1,y1,w,h). This is passed down to your render function but not used for CPU-side clipping. Prefer using higher-level ImGui::PushClipRect() to affect logic (hit-testing and widget culling)
        {
            _ClipRectStack.push_back(clip_rect);
            UpdateClipRect();
        }

        internal void PushClipRectFullScreen()
        {
            PushClipRect(GNullClipRect);

            // FIXME-OPT: This would be more correct but we're not supposed to access ImGuiState from here?
            //ImGuiState& g = *GImGui;
            //PushClipRect(GetVisibleRect());
        }

        internal void PopClipRect()
        {
            System.Diagnostics.Debug.Assert(_ClipRectStack.Size > 0);
            _ClipRectStack.pop_back();
            UpdateClipRect();
        }

        internal void PushTextureID(ImTextureID texture_id)
        {
            _TextureIdStack.push_back(texture_id);
            UpdateTextureID();
        }

        internal void PopTextureID()
        {
            System.Diagnostics.Debug.Assert(_TextureIdStack.Size > 0);
            _TextureIdStack.pop_back();
            UpdateTextureID();
        }

        //    // Primitives
        internal void AddLine(ImVec2 a, ImVec2 b, uint col, float thickness = 1.0f)
        {
            if ((col >> 24) == 0)
                return;
            PathLineTo(a + new ImVec2(0.5f, 0.5f));
            PathLineTo(b + new ImVec2(0.5f, 0.5f));
            PathStroke(col, false, thickness);
        }

        // a: upper-left, b: lower-right
        internal void AddRect(ImVec2 a, ImVec2 b, uint col, float rounding = 0.0f, int rounding_corners = 0x0F, float thickness = 1.0f)
        {
            if ((col >> 24) == 0)
                return;
            PathRect(a + new ImVec2(0.5f, 0.5f), b - new ImVec2(0.5f, 0.5f), rounding, rounding_corners);
            PathStroke(col, true, thickness);
        }

        // a: upper-left, b: lower-right
        internal void AddRectFilled(ImVec2 a, ImVec2 b, uint col, float rounding = 0.0f, int rounding_corners = 0x0F)
        {
            if ((col >> 24) == 0)
                return;
            if (rounding > 0.0f)
            {
                PathRect(a, b, rounding, rounding_corners);
                PathFill(col);
            }
            else
            {
                PrimReserve(6, 4);
                PrimRect(a, b, col);
            }
        }

        internal void AddRectFilledMultiColor(ImVec2 a, ImVec2 c, uint col_upr_left, uint col_upr_right, uint col_bot_right, uint col_bot_left)
        {
            if (((col_upr_left | col_upr_right | col_bot_right | col_bot_left) >> 24) == 0)
                return;

            ImVec2 uv = ImGui.Instance.FontTexUvWhitePixel;
            PrimReserve(6, 4);
            PrimWriteIdx((ImDrawIdx)(_VtxCurrentIdx)); PrimWriteIdx((ImDrawIdx)(_VtxCurrentIdx + 1)); PrimWriteIdx((ImDrawIdx)(_VtxCurrentIdx + 2));
            PrimWriteIdx((ImDrawIdx)(_VtxCurrentIdx)); PrimWriteIdx((ImDrawIdx)(_VtxCurrentIdx + 2)); PrimWriteIdx((ImDrawIdx)(_VtxCurrentIdx + 3));
            PrimWriteVtx(a, uv, col_upr_left);
            PrimWriteVtx(new ImVec2(c.x, a.y), uv, col_upr_right);
            PrimWriteVtx(c, uv, col_bot_right);
            PrimWriteVtx(new ImVec2(a.x, c.y), uv, col_bot_left);
        }

        internal void AddTriangle(ImVec2 a, ImVec2 b, ImVec2 c, uint col, float thickness = 1.0f)
        {
            if ((col >> 24) == 0)
                return;

            PathLineTo(a);
            PathLineTo(b);
            PathLineTo(c);
            PathStroke(col, true, thickness);
        }

        internal void AddTriangleFilled(ImVec2 a, ImVec2 b, ImVec2 c, uint col)
        {
            if ((col >> 24) == 0)
                return;

            PathLineTo(a);
            PathLineTo(b);
            PathLineTo(c);
            PathFill(col);
        }

        internal void AddCircle(ImVec2 centre, float radius, uint col, int num_segments = 12, float thickness = 1.0f)
        {
            if ((col >> 24) == 0)
                return;

            float a_max = ImGui.PI * 2.0f * (num_segments - 1.0f) / num_segments;
            PathArcTo(centre, radius - 0.5f, 0.0f, a_max, num_segments);
            PathStroke(col, true, thickness);
        }

        internal void AddCircleFilled(ImVec2 centre, float radius, uint col, int num_segments = 12)
        {
            if ((col >> 24) == 0)
                return;

            float a_max = ImGui.PI * 2.0f * ((num_segments - 1.0f) / num_segments);
            PathArcTo(centre, radius, 0.0f, a_max, num_segments);
            PathFill(col);
        }

        internal void AddText(ImVec2 pos, uint col, string text, int text_begin = 0, int text_end = -1)
        {
            AddText(ImGui.Instance.Font, ImGui.Instance.FontSize, pos, col, text, text_begin, text_end);
        }

        internal void AddText(ImVec2 pos, uint col, char[] text, int text_begin = 0, int text_end = -1)
        {
            AddText(ImGui.Instance.Font, ImGui.Instance.FontSize, pos, col, text, text_begin, text_end);
        }

        internal void AddText(ImFont font, float font_size, ImVec2 pos, uint col, string text, int text_begin = 0, int text_end = -1, float wrap_width = 0.0f, ImVec4? cpu_fine_clip_rect = null)
        {
            if ((col >> 24) == 0)
                return;

            if (text_end == -1)
                text_end = text.Length;
            if (text_begin == text_end)
                return;

            // Note: This is one of the few instance of breaking the encapsulation of ImDrawList, as we pull this from ImGui state, but it is just SO useful.
            // Might just move Font/FontSize to ImDrawList?
            if (font == null)
                font = ImGui.Instance.Font;
            if (font_size == 0.0f)
                font_size = ImGui.Instance.FontSize;

            System.Diagnostics.Debug.Assert(font.ContainerAtlas.TexID == _TextureIdStack[_TextureIdStack.Size - 1]);  // Use high-level ImGui::PushFont() or low-level ImDrawList::PushTextureId() to change font.

            // reserve vertices for worse case (over-reserving is useful and easily amortized)
            int char_count = (text_end - text_begin);
            int vtx_count_max = char_count * 4;
            int idx_count_max = char_count * 6;
            int vtx_begin = VtxBuffer.Size;
            int idx_begin = IdxBuffer.Size;
            PrimReserve(idx_count_max, vtx_count_max);

            ImVec4 clip_rect = _ClipRectStack[_ClipRectStack.Size - 1];
            if (cpu_fine_clip_rect.HasValue)
            {
                var cfcr = cpu_fine_clip_rect.Value;
                clip_rect.x = ImGui.Max(clip_rect.x, cfcr.x);
                clip_rect.y = ImGui.Max(clip_rect.y, cfcr.y);
                clip_rect.z = ImGui.Min(clip_rect.z, cfcr.z);
                clip_rect.w = ImGui.Min(clip_rect.w, cfcr.w);
            }
            var rect = font.RenderText(font_size, pos, col, clip_rect, text, text_begin, text_end, this, wrap_width, cpu_fine_clip_rect.HasValue);

            // give back unused vertices
            // FIXME-OPT: clean this up
            VtxBuffer.resize(_VtxWritePtr);
            IdxBuffer.resize(_IdxWritePtr);
            int vtx_unused = vtx_count_max - (VtxBuffer.Size - vtx_begin);
            int idx_unused = idx_count_max - (IdxBuffer.Size - idx_begin);
            var curr_cmd = CmdBuffer[CmdBuffer.Size - 1];
            curr_cmd.ElemCount -= (uint)idx_unused;
            CmdBuffer[CmdBuffer.Size - 1] = curr_cmd;

            //_VtxWritePtr -= vtx_unused; //this doesn't seem right, vtx/idx are already pointing to the unused spot
            //_IdxWritePtr -= idx_unused;
            _VtxCurrentIdx = (uint)VtxBuffer.Size;

            //AddRect(rect.Min, rect.Max, 0xff0000ff);
        }

        internal void AddText(ImFont font, float font_size, ImVec2 pos, uint col, char[] text, int text_begin = 0, int text_end = -1, float wrap_width = 0.0f, ImVec4? cpu_fine_clip_rect = null)
        {
            if ((col >> 24) == 0)
                return;

            if (text_end == -1)
                text_end = text.Length;
            if (text_begin == text_end)
                return;

            // Note: This is one of the few instance of breaking the encapsulation of ImDrawList, as we pull this from ImGui state, but it is just SO useful.
            // Might just move Font/FontSize to ImDrawList?
            if (font == null)
                font = ImGui.Instance.Font;
            if (font_size == 0.0f)
                font_size = ImGui.Instance.FontSize;

            System.Diagnostics.Debug.Assert(font.ContainerAtlas.TexID == _TextureIdStack[_TextureIdStack.Size - 1]);  // Use high-level ImGui::PushFont() or low-level ImDrawList::PushTextureId() to change font.

            // reserve vertices for worse case (over-reserving is useful and easily amortized)
            int char_count = (text_end - text_begin);
            int vtx_count_max = char_count * 4;
            int idx_count_max = char_count * 6;
            int vtx_begin = VtxBuffer.Size;
            int idx_begin = IdxBuffer.Size;
            PrimReserve(idx_count_max, vtx_count_max);

            ImVec4 clip_rect = _ClipRectStack[_ClipRectStack.Size - 1];
            if (cpu_fine_clip_rect.HasValue)
            {
                var cfcr = cpu_fine_clip_rect.Value;
                clip_rect.x = ImGui.Max(clip_rect.x, cfcr.x);
                clip_rect.y = ImGui.Max(clip_rect.y, cfcr.y);
                clip_rect.z = ImGui.Min(clip_rect.z, cfcr.z);
                clip_rect.w = ImGui.Min(clip_rect.w, cfcr.w);
            }
            var rect = font.RenderText(font_size, pos, col, clip_rect, text, text_begin, text_end, this, wrap_width, cpu_fine_clip_rect.HasValue);

            // give back unused vertices
            // FIXME-OPT: clean this up
            VtxBuffer.resize(_VtxWritePtr);
            IdxBuffer.resize(_IdxWritePtr);
            int vtx_unused = vtx_count_max - (VtxBuffer.Size - vtx_begin);
            int idx_unused = idx_count_max - (IdxBuffer.Size - idx_begin);
            var curr_cmd = CmdBuffer[CmdBuffer.Size - 1];
            curr_cmd.ElemCount -= (uint)idx_unused;
            CmdBuffer[CmdBuffer.Size - 1] = curr_cmd;

            //_VtxWritePtr -= vtx_unused; //this doesn't seem right, vtx/idx are already pointing to the unused spot
            //_IdxWritePtr -= idx_unused;
            _VtxCurrentIdx = (uint)VtxBuffer.Size;

            //AddRect(rect.Min, rect.Max, 0xff0000ff);
        }

        internal void AddImage(ImTextureID user_texture_id, ImVec2 a, ImVec2 b, ImVec2? _uv0 = null, ImVec2? _uv1 = null, uint? _col = null)
        {
            var uv0 = _uv0.HasValue ? _uv0.Value : new ImVec2(0, 0);
            var uv1 = _uv1.HasValue ? _uv1.Value : new ImVec2(1, 1);
            var col = _col.HasValue ? _col.Value : 0xFFFFFFFFu;

            if ((col >> 24) == 0)
                return;

            // FIXME-OPT: This is wasting draw calls.
            bool push_texture_id = _TextureIdStack.empty() || user_texture_id != _TextureIdStack[_TextureIdStack.Size - 1];
            if (push_texture_id)
                PushTextureID(user_texture_id);

            PrimReserve(6, 4);
            PrimRectUV(a, b, uv0, uv1, col);

            if (push_texture_id)
                PopTextureID();
        }

        internal void AddPolyline(ImVector<ImVec2> points, int num_points, uint col, bool closed, float thickness, bool anti_aliased)
        {
            var points_count = points.Size;
            if (points_count < 2)
                return;

            ImVec2 uv = ImGui.Instance.FontTexUvWhitePixel;
            anti_aliased &= ImGui.Instance.Style.AntiAliasedLines;
            //if (ImGui::GetIO().KeyCtrl) anti_aliased = false; // Debug

            int count = points_count;
            if (!closed)
                count = points_count - 1;

            bool thick_line = thickness > 1.0f;
            if (anti_aliased)
            {
                // Anti-aliased stroke
                float AA_SIZE = 1.0f;
                uint col_trans = col & 0x00ffffff;

                int idx_count = thick_line ? count * 18 : count * 12;
                int vtx_count = thick_line ? points_count * 4 : points_count * 3;
                PrimReserve(idx_count, vtx_count);

                // Temporary buffer
                //ImVec2* temp_normals = (ImVec2*)alloca(points_count * (thick_line ? 5 : 3) * sizeof(ImVec2));
                //ImVec2* temp_points = temp_normals + points_count;
                var temp_normals = new ImVec2[points_count * (thick_line ? 5 : 3)];
                var temp_points = new ImVec2[points_count * (thick_line ? 5 : 3)];

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = (i1 + 1) == points_count ? 0 : i1 + 1;
                    ImVec2 diff = points[i2] - points[i1];
                    diff *= ImGui.InvLength(diff, 1.0f);
                    temp_normals[i1].x = diff.y;
                    temp_normals[i1].y = -diff.x;
                }
                if (!closed)
                    temp_normals[points_count - 1] = temp_normals[points_count - 2];

                if (!thick_line)
                {
                    if (!closed)
                    {
                        temp_points[0] = points[0] + temp_normals[0] * AA_SIZE;
                        temp_points[1] = points[0] - temp_normals[0] * AA_SIZE;
                        temp_points[(points_count - 1) * 2 + 0] = points[points_count - 1] + temp_normals[points_count - 1] * AA_SIZE;
                        temp_points[(points_count - 1) * 2 + 1] = points[points_count - 1] - temp_normals[points_count - 1] * AA_SIZE;
                    }

                    // FIXME-OPT: Merge the different loops, possibly remove the temporary buffer.
                    uint idx1 = _VtxCurrentIdx;
                    for (int i1 = 0; i1 < count; i1++)
                    {
                        int i2 = (i1 + 1) == points_count ? 0 : i1 + 1;
                        uint idx2 = (i1 + 1) == points_count ? _VtxCurrentIdx : idx1 + 3;

                        // Average normals
                        ImVec2 dm = (temp_normals[i1] + temp_normals[i2]) * 0.5f;
                        float dmr2 = dm.x * dm.x + dm.y * dm.y;
                        if (dmr2 > 0.000001f)
                        {
                            float scale = 1.0f / dmr2;
                            if (scale > 100.0f) scale = 100.0f;
                            dm *= scale;
                        }
                        dm *= AA_SIZE;
                        temp_points[i2 * 2 + 0] = points[i2] + dm;
                        temp_points[i2 * 2 + 1] = points[i2] - dm;

                        // Add indexes

                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 0); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 0); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 2);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 0);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 0);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 0); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 0); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 1);
                        //_IdxWritePtr += 12;

                        idx1 = idx2;
                    }

                    // Add vertexes
                    for (int i = 0; i < points_count; i++)
                    {
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = points[i], uv = uv, col = col };
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = temp_points[i * 2 + 0], uv = uv, col = col_trans };
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = temp_points[i * 2 + 1], uv = uv, col = col_trans };
                    }
                }
                else
                {
                    float half_inner_thickness = (thickness - AA_SIZE) * 0.5f;
                    if (!closed)
                    {
                        temp_points[0] = points[0] + temp_normals[0] * (half_inner_thickness + AA_SIZE);
                        temp_points[1] = points[0] + temp_normals[0] * (half_inner_thickness);
                        temp_points[2] = points[0] - temp_normals[0] * (half_inner_thickness);
                        temp_points[3] = points[0] - temp_normals[0] * (half_inner_thickness + AA_SIZE);
                        temp_points[(points_count - 1) * 4 + 0] = points[points_count - 1] + temp_normals[points_count - 1] * (half_inner_thickness + AA_SIZE);
                        temp_points[(points_count - 1) * 4 + 1] = points[points_count - 1] + temp_normals[points_count - 1] * (half_inner_thickness);
                        temp_points[(points_count - 1) * 4 + 2] = points[points_count - 1] - temp_normals[points_count - 1] * (half_inner_thickness);
                        temp_points[(points_count - 1) * 4 + 3] = points[points_count - 1] - temp_normals[points_count - 1] * (half_inner_thickness + AA_SIZE);
                    }

                    // FIXME-OPT: Merge the different loops, possibly remove the temporary buffer.
                    uint idx1 = _VtxCurrentIdx;
                    for (int i1 = 0; i1 < count; i1++)
                    {
                        int i2 = (i1 + 1) == points_count ? 0 : i1 + 1;
                        uint idx2 = (i1 + 1) == points_count ? _VtxCurrentIdx : idx1 + 4;

                        // Average normals
                        ImVec2 dm = (temp_normals[i1] + temp_normals[i2]) * 0.5f;
                        float dmr2 = dm.x * dm.x + dm.y * dm.y;
                        if (dmr2 > 0.000001f)
                        {
                            float scale = 1.0f / dmr2;
                            if (scale > 100.0f) scale = 100.0f;
                            dm *= scale;
                        }
                        ImVec2 dm_out = dm * (half_inner_thickness + AA_SIZE);
                        ImVec2 dm_in = dm * half_inner_thickness;
                        temp_points[i2 * 4 + 0] = points[i2] + dm_out;
                        temp_points[i2 * 4 + 1] = points[i2] + dm_in;
                        temp_points[i2 * 4 + 2] = points[i2] - dm_in;
                        temp_points[i2 * 4 + 3] = points[i2] - dm_out;

                        // Add indexes
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 2);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 1);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 0);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 0); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 0); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 1);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 3);
                        IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx1 + 3); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 3); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(idx2 + 2);
                        //_IdxWritePtr += 18;

                        idx1 = idx2;
                    }

                    // Add vertexes
                    for (int i = 0; i < points_count; i++)
                    {
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = temp_points[i * 4 + 0], uv = uv, col = col_trans };
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = temp_points[i * 4 + 1], uv = uv, col = col };
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = temp_points[i * 4 + 2], uv = uv, col = col };
                        VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = temp_points[i * 4 + 3], uv = uv, col = col_trans };
                        //_VtxWritePtr += 4;
                    }
                }
                _VtxCurrentIdx += (ImDrawIdx)vtx_count;
            }
            else
            {
                // Non Anti-aliased Stroke
                int idx_count = count * 6;
                int vtx_count = count * 4;      // FIXME-OPT: Not sharing edges
                PrimReserve(idx_count, vtx_count);

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = (i1 + 1) == points_count ? 0 : i1 + 1;
                    ImVec2 p1 = points[i1];
                    ImVec2 p2 = points[i2];
                    ImVec2 diff = p2 - p1;
                    diff *= ImGui.InvLength(diff, 1.0f);

                    float dx = diff.x * (thickness * 0.5f);
                    float dy = diff.y * (thickness * 0.5f);
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = new ImVec2(p1.x + dy, p1.y - dx), uv = uv, col = col };
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = new ImVec2(p2.x + dy, p2.y - dx), uv = uv, col = col };
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = new ImVec2(p2.x - dy, p2.y + dx), uv = uv, col = col };
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = new ImVec2(p1.x - dy, p1.y + dx), uv = uv, col = col };
                    //_VtxWritePtr += 4;

                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx + 1); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx + 2);
                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx + 2); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx + 3);
                    //_IdxWritePtr += 6;
                    _VtxCurrentIdx += 4;
                }
            }
        }

        internal void AddConvexPolyFilled(ImVector<ImVec2> points, int num_points, uint col, bool anti_aliased)
        {
            var points_count = points.Size;
            ImVec2 uv = ImGui.Instance.FontTexUvWhitePixel;
            anti_aliased &= ImGui.Instance.Style.AntiAliasedShapes;
            //if (ImGui::GetIO().KeyCtrl) anti_aliased = false; // Debug

            if (anti_aliased)
            {
                // Anti-aliased Fill
                float AA_SIZE = 1.0f;
                uint col_trans = col & 0x00ffffff;
                int idx_count = (points_count - 2) * 3 + points_count * 6;
                int vtx_count = (points_count * 2);
                PrimReserve(idx_count, vtx_count);

                // Add indexes for fill
                uint vtx_inner_idx = _VtxCurrentIdx;
                uint vtx_outer_idx = _VtxCurrentIdx + 1;
                for (int i = 2; i < points_count; i++)
                {
                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_inner_idx);
                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_inner_idx + ((i - 1) << 1));
                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_inner_idx + (i << 1));
                }

                // Compute normals
                ImVec2[] temp_normals = new ImVec2[points_count];
                //ImVec2* temp_normals = (ImVec2*)alloca(points_count * sizeof(ImVec2));

                for (int i0 = points_count - 1, i1 = 0; i1 < points_count; i0 = i1++)
                {
                    ImVec2 p0 = points[i0];
                    ImVec2 p1 = points[i1];
                    ImVec2 diff = p1 - p0;
                    diff *= ImGui.InvLength(diff, 1.0f);
                    temp_normals[i0].x = diff.y;
                    temp_normals[i0].y = -diff.x;
                }

                for (int i0 = points_count - 1, i1 = 0; i1 < points_count; i0 = i1++)
                {
                    // Average normals
                    ImVec2 n0 = temp_normals[i0];
                    ImVec2 n1 = temp_normals[i1];
                    ImVec2 dm = (n0 + n1) * 0.5f;
                    float dmr2 = dm.x * dm.x + dm.y * dm.y;
                    if (dmr2 > 0.000001f)
                    {
                        float scale = 1.0f / dmr2;
                        if (scale > 100.0f) scale = 100.0f;
                        dm *= scale;
                    }
                    dm *= AA_SIZE * 0.5f;

                    // Add vertices
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = (points[i1] - dm), uv = uv, col = col };
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = (points[i1] + dm), uv = uv, col = col_trans };

                    // Add indexes for fringes

                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_inner_idx + (i1 << 1)); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_inner_idx + (i0 << 1)); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_outer_idx + (i0 << 1));
                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_outer_idx + (i0 << 1)); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_outer_idx + (i1 << 1)); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(vtx_inner_idx + (i1 << 1));
                }
                _VtxCurrentIdx += (ImDrawIdx)vtx_count;
            }
            else
            {
                // Non Anti-aliased Fill
                int idx_count = (points_count - 2) * 3;
                int vtx_count = points_count;
                PrimReserve(idx_count, vtx_count);
                for (int i = 0; i < vtx_count; i++)
                    VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = points[i], uv = uv, col = col };

                for (uint i = 2u; i < points_count; i++)
                {
                    IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx + i - 1u); IdxBuffer[_IdxWritePtr++] = (ImDrawIdx)(_VtxCurrentIdx + i);
                }
                _VtxCurrentIdx += (ImDrawIdx)vtx_count;
            }
        }

        internal void AddBezierCurve(ImVec2 pos0, ImVec2 cp0, ImVec2 cp1, ImVec2 pos1, uint col, float thickness, int num_segments = 0)
        {
            if ((col >> 24) == 0)
                return;

            PathLineTo(pos0);
            PathBezierCurveTo(cp0, cp1, pos1, num_segments);
            PathStroke(col, false, thickness);
        }

        //    // Stateful path API, add points then finish with PathFill() or PathStroke()
        internal void PathClear()
        { _Path.resize(0); }

        internal void PathLineTo(ImVec2 pos)
        { _Path.push_back(pos); }

        internal void PathLineToMergeDuplicate(ImVec2 pos)
        { if (_Path.Size == 0 || _Path[_Path.Size - 1].x != pos.x || _Path[_Path.Size - 1].y != pos.y) _Path.push_back(pos); }

        internal void PathFill(uint col)
        { AddConvexPolyFilled(_Path, _Path.Size, col, true); PathClear(); }

        internal void PathStroke(uint col, bool closed, float thickness = 1.0f)
        { AddPolyline(_Path, _Path.Size, col, closed, thickness, true); PathClear(); }

        internal void PathArcTo(ImVec2 centre, float radius, float amin, float amax, int num_segments = 10)
        {
            if (radius == 0.0f)
                _Path.push_back(centre);
            _Path.reserve(_Path.Size + (num_segments + 1));
            for (int i = 0; i <= num_segments; i++)
            {
                float a = amin + ((float)i / (float)num_segments) * (amax - amin);
                _Path.push_back(new ImVec2(centre.x + ImGui.Cosf(a) * radius, centre.y + ImGui.Sinf(a) * radius));
            }
        }

        // Use precomputed angles for a 12 steps circle
        internal void PathArcToFast(ImVec2 centre, float radius, int amin, int amax)
        {
            ImVec2[] circle_vtx = new ImVec2[12];
            bool circle_vtx_builds = false;
            int circle_vtx_count = circle_vtx.Length;
            if (!circle_vtx_builds)
            {
                for (int i = 0; i < circle_vtx_count; i++)
                {
                    float a = ((float)i / (float)circle_vtx_count) * 2 * ImGui.PI;
                    circle_vtx[i].x = ImGui.Cosf(a);
                    circle_vtx[i].y = ImGui.Sinf(a);
                }
                circle_vtx_builds = true;
            }

            if (amin > amax) return;
            if (radius == 0.0f)
            {
                _Path.push_back(centre);
            }
            else
            {
                _Path.reserve(_Path.Size + (amax - amin + 1));
                for (int a = amin; a <= amax; a++)
                {
                    ImVec2 c = circle_vtx[a % circle_vtx_count];
                    _Path.push_back(new ImVec2(centre.x + c.x * radius, centre.y + c.y * radius));
                }
            }
        }

        internal void PathBezierCurveTo(ImVec2 p2, ImVec2 p3, ImVec2 p4, int num_segments = 0)
        {
            ImVec2 p1 = _Path[_Path.Size - 1];
            if (num_segments == 0)
            {
                // Auto-tessellated
                PathBezierToCasteljau(_Path, p1.x, p1.y, p2.x, p2.y, p3.x, p3.y, p4.x, p4.y, ImGui.Instance.Style.CurveTessellationTol, 0);
            }
            else
            {
                float t_step = 1.0f / (float)num_segments;
                for (int i_step = 1; i_step <= num_segments; i_step++)
                {
                    float t = t_step * i_step;
                    float u = 1.0f - t;
                    float w1 = u * u * u;
                    float w2 = 3 * u * u * t;
                    float w3 = 3 * u * t * t;
                    float w4 = t * t * t;
                    _Path.push_back(new ImVec2(w1 * p1.x + w2 * p2.x + w3 * p3.x + w4 * p4.x, w1 * p1.y + w2 * p2.y + w3 * p3.y + w4 * p4.y));
                }
            }
        }

        internal void PathRect(ImVec2 a, ImVec2 b, float rounding = 0.0f, int rounding_corners = 0x0F)
        {
            float r = rounding;
            r = ImGui.Min(r, ImGui.fabsf(b.x - a.x) * (((rounding_corners & (1 | 2)) == (1 | 2)) || ((rounding_corners & (4 | 8)) == (4 | 8)) ? 0.5f : 1.0f) - 1.0f);
            r = ImGui.Min(r, ImGui.fabsf(b.y - a.y) * (((rounding_corners & (1 | 8)) == (1 | 8)) || ((rounding_corners & (2 | 4)) == (2 | 4)) ? 0.5f : 1.0f) - 1.0f);

            if (r <= 0.0f || rounding_corners == 0)
            {
                PathLineTo(a);
                PathLineTo(new ImVec2(b.x, a.y));
                PathLineTo(b);
                PathLineTo(new ImVec2(a.x, b.y));
            }
            else
            {
                float r0 = (rounding_corners & 1) > 0 ? r : 0.0f;
                float r1 = (rounding_corners & 2) > 0 ? r : 0.0f;
                float r2 = (rounding_corners & 4) > 0 ? r : 0.0f;
                float r3 = (rounding_corners & 8) > 0 ? r : 0.0f;
                PathArcToFast(new ImVec2(a.x + r0, a.y + r0), r0, 6, 9);
                PathArcToFast(new ImVec2(b.x - r1, a.y + r1), r1, 9, 12);
                PathArcToFast(new ImVec2(b.x - r2, b.y - r2), r2, 0, 3);
                PathArcToFast(new ImVec2(a.x + r3, b.y - r3), r3, 3, 6);
            }
        }

        //// Channels
        //// - Use to simulate layers. By switching channels to can render out-of-order (e.g. submit foreground primitives before background primitives)
        //// - Use to minimize draw calls (e.g. if going back-and-forth between multiple non-overlapping clipping rectangles, prefer to append into separate channels then merge at the end)

        internal void ChannelsSplit(int channels_count)
        {
            System.Diagnostics.Debug.Assert(_ChannelsCurrent == 0 && _ChannelsCount == 1);
            int old_channels_count = _Channels.Size;
            if (old_channels_count < channels_count)
                _Channels.resize(channels_count);
            _ChannelsCount = channels_count;

            // _Channels[] (24 bytes each) hold storage that we'll swap with this->_CmdBuffer/_IdxBuffer
            // The content of _Channels[0] at this point doesn't matter. We clear it to make state tidy in a debugger but we don't strictly need to.
            // When we switch to the next channel, we'll copy _CmdBuffer/_IdxBuffer into _Channels[0] and then _Channels[1] into _CmdBuffer/_IdxBuffer
            //memset(&_Channels[0], 0, sizeof(ImDrawChannel));
            for (int i = 1; i < channels_count; i++)
            {
                if (i >= old_channels_count)
                {
                    //IM_PLACEMENT_NEW(&_Channels[i]) ImDrawChannel();
                    _Channels[i] = new ImDrawChannel();
                }
                else
                {
                    _Channels[i].CmdBuffer.resize(0);
                    _Channels[i].IdxBuffer.resize(0);
                }
                if (_Channels[i].CmdBuffer.Size == 0)
                {
                    ImDrawCmd draw_cmd = new ImDrawCmd();
                    draw_cmd.ClipRect = _ClipRectStack[_ClipRectStack.Size - 1];
                    draw_cmd.TextureId = _TextureIdStack[_TextureIdStack.Size - 1];
                    _Channels[i].CmdBuffer.push_back(draw_cmd);
                }
            }
        }

        internal void ChannelsMerge()
        {
            // Note that we never use or rely on channels.Size because it is merely a buffer that we never shrink back to 0 to keep all sub-buffers ready for use.
            if (_ChannelsCount <= 1)
                return;

            ChannelsSetCurrent(0);

            var curr_cmd = GetCurrentDrawCmd();
            if (curr_cmd.HasValue && curr_cmd.Value.ElemCount == 0)
                CmdBuffer.pop_back();

            int new_cmd_buffer_count = 0, new_idx_buffer_count = 0;
            for (int i = 1; i < _ChannelsCount; i++)
            {
                ImDrawChannel ch = _Channels[i];

                if (ch.CmdBuffer.Size > 0 && ch.CmdBuffer[ch.CmdBuffer.Size - 1].ElemCount == 0)
                    ch.CmdBuffer.pop_back();
                new_cmd_buffer_count += ch.CmdBuffer.Size;
                new_idx_buffer_count += ch.IdxBuffer.Size;
            }
            CmdBuffer.resize(CmdBuffer.Size + new_cmd_buffer_count);
            IdxBuffer.resize(IdxBuffer.Size + new_idx_buffer_count);

            int cmd_write = CmdBuffer.Size - new_cmd_buffer_count;
            _IdxWritePtr = IdxBuffer.Size - new_idx_buffer_count;
            for (int i = 1; i < _ChannelsCount; i++)
            {
                int sz;
                ImDrawChannel ch = _Channels[i];
                if ((sz = ch.CmdBuffer.Size) > 0)
                {
                    for (var k = cmd_write; k < sz; k++)
                        CmdBuffer[cmd_write + k] = ch.CmdBuffer[k];
                    //memcpy(cmd_write, ch.CmdBuffer.Data, sz * sizeof(ImDrawCmd));
                    cmd_write += sz;
                }
                if ((sz = ch.IdxBuffer.Size) > 0)
                {
                    for (var k = cmd_write; k < sz; k++)
                        IdxBuffer[_IdxWritePtr + k] = ch.IdxBuffer[k];
                    //memcpy(_IdxWritePtr, ch.IdxBuffer.Data, sz * sizeof(ImDrawIdx));
                    _IdxWritePtr += sz;
                }
            }

            AddDrawCmd();
            _ChannelsCount = 1;
        }

        internal void ChannelsSetCurrent(int idx)
        {
            System.Diagnostics.Debug.Assert(idx < _ChannelsCount);
            if (_ChannelsCurrent == idx)
                return;

            //memcpy(&_Channels.Data[_ChannelsCurrent].CmdBuffer, &CmdBuffer, sizeof(CmdBuffer)); // copy 12 bytes, four times
            //memcpy(&_Channels.Data[_ChannelsCurrent].IdxBuffer, &IdxBuffer, sizeof(IdxBuffer));

            _ChannelsCurrent = idx;

            //memcpy(&CmdBuffer, &_Channels.Data[_ChannelsCurrent].CmdBuffer, sizeof(CmdBuffer));
            CmdBuffer = _Channels.Data[_ChannelsCurrent].CmdBuffer;
            //memcpy(&IdxBuffer, &_Channels.Data[_ChannelsCurrent].IdxBuffer, sizeof(IdxBuffer));
            IdxBuffer = _Channels.Data[_ChannelsCurrent].IdxBuffer;

            _IdxWritePtr = IdxBuffer.Size;
        }

        //// Advanced
        internal void AddCallback(ImDrawCallback callback, object callback_data)  // Your rendering function must check for 'UserCallback' in ImDrawCmd and call the function instead of rendering triangles.
        {
            var size = CmdBuffer.Size;
            ImDrawCmd? current_cmd = GetCurrentDrawCmd();

            if (!current_cmd.HasValue || current_cmd.Value.ElemCount != 0u || current_cmd.Value.UserCallback != null)
            {
                AddDrawCmd();
                current_cmd = CmdBuffer[CmdBuffer.Size - 1];
            }

            var value = current_cmd.Value;
            value.UserCallback = callback;
            value.UserCallbackData = callback_data;
            SetCurrentDrawCmd(value);

            AddDrawCmd(); // Force a new command after us (see comment below)
        }

        internal ImVec4 GetCurrentClipRect()
        {
            return (_ClipRectStack.Size > 0 ? _ClipRectStack.Data[_ClipRectStack.Size - 1] : GNullClipRect);
        }

        internal ImTextureID GetCurrentTextureId()
        {
            return (_TextureIdStack.Size > 0 ? _TextureIdStack.Data[_TextureIdStack.Size - 1] : null);
        }

        internal void AddDrawCmd()
        {
            // This is useful if you need to forcefully create a new draw call (to allow for dependent rendering / blending). Otherwise primitives are merged into the same draw-call as much as possible
            ImDrawCmd draw_cmd = new ImDrawCmd();
            draw_cmd.ClipRect = GetCurrentClipRect();
            draw_cmd.TextureId = GetCurrentTextureId();

            System.Diagnostics.Debug.Assert(draw_cmd.ClipRect.x <= draw_cmd.ClipRect.z && draw_cmd.ClipRect.y <= draw_cmd.ClipRect.w);
            CmdBuffer.push_back(draw_cmd);
        }

        internal void UpdateClipRect()
        {
            // If current command is used with different settings we need to add a new command
            ImVec4 curr_clip_rect = GetCurrentClipRect();
            ImDrawCmd? curr_cmd = GetCurrentDrawCmd();
            if (!curr_cmd.HasValue || (curr_cmd.Value.ElemCount != 0 && curr_cmd.Value.ClipRect != curr_clip_rect) || curr_cmd.Value.UserCallback != null)
            {
                AddDrawCmd();
                return;
            }

            // Try to merge with previous command if it matches, else use current command
            ImDrawCmd? prev_cmd = GetPreviousDrawCmd();
            if (prev_cmd.HasValue && prev_cmd.Value.ClipRect == curr_clip_rect && prev_cmd.Value.TextureId == GetCurrentTextureId() && prev_cmd.Value.UserCallback == null)
                CmdBuffer.pop_back();
            else
            {
                var value = curr_cmd.Value;
                value.ClipRect = curr_clip_rect;
                SetCurrentDrawCmd(value);
            }
        }

        internal void PrimReserve(int idx_count, int vtx_count)
        {
            ImDrawCmd draw_cmd = CmdBuffer.Data[CmdBuffer.Size - 1];
            draw_cmd.ElemCount += (uint)idx_count;
            SetCurrentDrawCmd(draw_cmd);

            int vtx_buffer_size = VtxBuffer.Size;
            VtxBuffer.resize(vtx_buffer_size + vtx_count);
            _VtxWritePtr = vtx_buffer_size;

            int idx_buffer_size = IdxBuffer.Size;
            IdxBuffer.resize(idx_buffer_size + idx_count);
            _IdxWritePtr = idx_buffer_size;
        }

        // Axis aligned rectangle (composed of two triangles)
        internal void PrimRect(ImVec2 a, ImVec2 c, uint col)
        {
            var b = new ImVec2(c.x, a.y);
            var d = new ImVec2(a.x, c.y);
            var uv = new ImVec2(-1, -1);
            //ImVec2 b(c.x, a.y), d(a.x, c.y), uv(GImGui->FontTexUvWhitePixel);
            ImDrawIdx idx = (ImDrawIdx)_VtxCurrentIdx;
            IdxBuffer[_IdxWritePtr + 0] = idx; IdxBuffer[_IdxWritePtr + 1] = (ImDrawIdx)(idx + 1); IdxBuffer[_IdxWritePtr + 2] = (ImDrawIdx)(idx + 2);
            IdxBuffer[_IdxWritePtr + 3] = idx; IdxBuffer[_IdxWritePtr + 4] = (ImDrawIdx)(idx + 2); IdxBuffer[_IdxWritePtr + 5] = (ImDrawIdx)(idx + 3);
            VtxBuffer[_VtxWritePtr + 0] = new ImDrawVert() { pos = a, uv = uv, col = col };
            VtxBuffer[_VtxWritePtr + 1] = new ImDrawVert() { pos = b, uv = uv, col = col };
            VtxBuffer[_VtxWritePtr + 2] = new ImDrawVert() { pos = c, uv = uv, col = col };
            VtxBuffer[_VtxWritePtr + 3] = new ImDrawVert() { pos = d, uv = uv, col = col };
            _VtxWritePtr += 4;
            _VtxCurrentIdx += 4;
            _IdxWritePtr += 6;
        }

        internal void PrimRectUV(ImVec2 a, ImVec2 c, ImVec2 uv_a, ImVec2 uv_c, uint col)
        {
            var b = new ImVec2(c.x, a.y);
            var d = new ImVec2(a.x, c.y);
            var uv_b = new ImVec2(uv_c.x, uv_a.y);
            var uv_d = new ImVec2(uv_a.x, uv_c.y);

            ImDrawIdx idx = (ImDrawIdx)_VtxCurrentIdx;
            IdxBuffer[_IdxWritePtr + 0] = idx; IdxBuffer[_IdxWritePtr + 1] = (ImDrawIdx)(idx + 1); IdxBuffer[_IdxWritePtr + 2] = (ImDrawIdx)(idx + 2);
            IdxBuffer[_IdxWritePtr + 3] = idx; IdxBuffer[_IdxWritePtr + 4] = (ImDrawIdx)(idx + 2); IdxBuffer[_IdxWritePtr + 5] = (ImDrawIdx)(idx + 3);
            VtxBuffer[_VtxWritePtr + 0] = new ImDrawVert() { pos = a, uv = uv_a, col = col };
            VtxBuffer[_VtxWritePtr + 1] = new ImDrawVert() { pos = b, uv = uv_b, col = col };
            VtxBuffer[_VtxWritePtr + 2] = new ImDrawVert() { pos = c, uv = uv_c, col = col };
            VtxBuffer[_VtxWritePtr + 3] = new ImDrawVert() { pos = d, uv = uv_d, col = col };

            _VtxWritePtr += 4;
            _VtxCurrentIdx += 4;
            _IdxWritePtr += 6;
        }

        internal void PrimQuadUV(ImVec2 a, ImVec2 b, ImVec2 c, ImVec2 d, ImVec2 uv_a, ImVec2 uv_b, ImVec2 uv_c, ImVec2 uv_d, uint col)
        {
            ImDrawIdx idx = (ImDrawIdx)_VtxCurrentIdx;
            IdxBuffer[_IdxWritePtr + 0] = idx; IdxBuffer[_IdxWritePtr + 1] = (ImDrawIdx)(idx + 1); IdxBuffer[_IdxWritePtr + 2] = (ImDrawIdx)(idx + 2);
            IdxBuffer[_IdxWritePtr + 3] = idx; IdxBuffer[_IdxWritePtr + 4] = (ImDrawIdx)(idx + 2); IdxBuffer[_IdxWritePtr + 5] = (ImDrawIdx)(idx + 3);
            VtxBuffer[_VtxWritePtr + 0] = new ImDrawVert() { pos = a, uv = uv_a, col = col };
            VtxBuffer[_VtxWritePtr + 1] = new ImDrawVert() { pos = b, uv = uv_b, col = col };
            VtxBuffer[_VtxWritePtr + 2] = new ImDrawVert() { pos = c, uv = uv_c, col = col };
            VtxBuffer[_VtxWritePtr + 3] = new ImDrawVert() { pos = d, uv = uv_d, col = col };

            _VtxWritePtr += 4;
            _VtxCurrentIdx += 4;
            _IdxWritePtr += 6;
        }

        internal void PrimVtx(ImVec2 pos, ImVec2 uv, uint col)
        { PrimWriteIdx((ImDrawIdx)_VtxCurrentIdx); PrimWriteVtx(pos, uv, col); }

        internal void PrimWriteVtx(ImVec2 pos, ImVec2 uv, uint col)
        { VtxBuffer[_VtxWritePtr++] = new ImDrawVert() { pos = pos, uv = uv, col = col }; _VtxWritePtr++; _VtxCurrentIdx++; }

        internal void PrimWriteIdx(ImDrawIdx idx)
        { IdxBuffer[_IdxWritePtr++] = idx; }

        internal void UpdateTextureID()
        {
            // If current command is used with different settings we need to add a new command
            ImTextureID curr_texture_id = GetCurrentTextureId();
            ImDrawCmd? curr_cmd = GetCurrentDrawCmd();
            if (!curr_cmd.HasValue || (curr_cmd.Value.ElemCount != 0 && curr_cmd.Value.TextureId != curr_texture_id) || curr_cmd.Value.UserCallback != null)
            {
                AddDrawCmd();
                return;
            }

            // Try to merge with previous command if it matches, else use current command
            ImDrawCmd? prev_cmd = GetPreviousDrawCmd();
            if (prev_cmd.HasValue && prev_cmd.Value.TextureId == curr_texture_id && prev_cmd.Value.ClipRect == GetCurrentClipRect() && prev_cmd.Value.UserCallback == null)
                CmdBuffer.pop_back();
            else
            {
                var value = curr_cmd.Value;
                value.TextureId = curr_texture_id;
                SetCurrentDrawCmd(value);
            }
        }
    }
}