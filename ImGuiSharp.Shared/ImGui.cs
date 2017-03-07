using System.IO;

namespace ImGui
{
    internal partial class ImGui
    {
        #region static helpers

        public const float PI = 3.14159265358979323846f;

        internal static int IM_F32_TO_INT8(float _VAL)
        {
            return (int)((_VAL) * 255.0f + 0.5f);
        }

        // FIXME-OPT: Replace with e.g. FNV1a hash? CRC32 pretty much randomly access 1KB. Need to do proper measurements.
        static uint[] crc32_lut = new uint[256];
        internal static uint Hash(uint seed, string data, int start = 0, int end = -1)
        {
            if (end == -1)
                end = data.Length;

            if (crc32_lut[1] == 0)
            {
                uint polynomial = 0xEDB88320;
                for (uint i = 0; i < 256; i++)
                {
                    uint crc = i;
                    for (uint j = 0; j < 8; j++)
                        crc = ((crc >> 1) ^ ((uint)-(crc & 1)) & polynomial);
                    crc32_lut[i] = crc;
                }
            }

            {
                seed = ~seed;
                uint crc = seed;
                int current = start;

                //if (data_size > 0)
                //{
                //    // Known size
                //    while (data_size-- > 0)
                //        crc = (crc >> 8) ^ crc32_lut[(crc & 0xFF) ^ current++];
                //}
                //else
                {
                    // Zero-terminated string
                    while (current < end)
                    {
                        char c = data[current++];
                        // We support a syntax of "label###id" where only "###id" is included in the hash, and only "label" gets displayed.
                        // Because this syntax is rarely used we are optimizing for the common case.
                        // - If we reach ### in the string we discard the hash so far and reset to the seed.
                        // - We don't do 'current += 2; continue;' after handling ### to keep the code smaller.
                        if (current < end - 2 && c == '#' && data[current] == '#' && data[current + 1] == '#')
                            crc = seed;

                        crc = (crc >> 8) ^ crc32_lut[(crc & 0xFF) ^ c];
                    }
                }
                return ~crc;
            }
        }

        internal static int UpperPowerOfTwo(int v)
        {
            v--; v |= v >> 1; v |= v >> 2; v |= v >> 4; v |= v >> 8; v |= v >> 16; v++; return v;
        }

        internal static float fmodf(float x, float y)
        {
            return x % y;
        }

        internal static float fabsf(float v)
        {
            return System.Math.Abs(v);
        }

        internal static float powf(float x, float y)
        {
            return (float)System.Math.Pow(x, y);
        }


        internal static float Cosf(float v)
        {
            return (float)System.Math.Cos(v);
        }

        internal static float Sinf(float v)
        {
            return (float)System.Math.Sin(v);
        }

        internal static int Min(int lhs, int rhs)
        {
            return lhs < rhs ? lhs : rhs;
        }

        internal static int Max(int lhs, int rhs)
        {
            return lhs >= rhs ? lhs : rhs;
        }

        internal static float Min(float lhs, float rhs)
        {
            return lhs < rhs ? lhs : rhs;
        }

        internal static float Max(float lhs, float rhs)
        {
            return lhs >= rhs ? lhs : rhs;
        }

        internal static ImVec2 Min(ImVec2 lhs, ImVec2 rhs)
        {
            return new ImVec2(Min(lhs.x, rhs.x), Min(lhs.y, rhs.y));
        }

        internal static ImVec2 Max(ImVec2 lhs, ImVec2 rhs)
        {
            return new ImVec2(Max(lhs.x, rhs.x), Max(lhs.y, rhs.y));
        }

        internal static int Clamp(int v, int mn, int mx)
        {
            return (v < mn) ? mn : (v > mx) ? mx : v;
        }

        internal static float Clamp(float v, float mn, float mx)
        {
            return (v < mn) ? mn : (v > mx) ? mx : v;
        }

        internal static ImVec2 Clamp(ImVec2 f, ImVec2 mn, ImVec2 mx)
        {
            return new ImVec2(Clamp(f.x, mn.x, mx.x), Clamp(f.y, mn.y, mx.y));
        }

        internal static float Saturate(float f)
        {
            return (f < 0.0f) ? 0.0f : (f > 1.0f) ? 1.0f : f;
        }

        internal static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        internal static ImVec2 Lerp(ImVec2 a, ImVec2 b, ImVec2 t)
        {
            return new ImVec2(a.x + (b.x - a.x) * t.x, a.y + (b.y - a.y) * t.y);
        }

        internal static float LengthSqr(ImVec2 lhs)
        {
            return lhs.x * lhs.x + lhs.y * lhs.y;
        }

        internal static float LengthSqr(ImVec4 lhs)
        {
            return lhs.x * lhs.x + lhs.y * lhs.y + lhs.z * lhs.z + lhs.w * lhs.w;
        }

        internal static float InvLength(ImVec2 lhs, float fail_value)
        {
            float d = lhs.x * lhs.x + lhs.y * lhs.y; if (d > 0.0f) return 1.0f / (float)System.Math.Sqrt(d); return fail_value;
        }

        internal static ImVec2 Round(ImVec2 v)
        {
            return new ImVec2((float)(int)v.x, (float)(int)v.y);
        }

        private static float[] u32FloatLookup;
        internal static ImVec4 ColorConvertU32ToFloat4(uint @in)
        {
            if(u32FloatLookup == null)
            {
                const float s = 1.0f / 255.0f;
                u32FloatLookup = new float[256];
                for(var i =0; i < 256; i++)
                {
                    u32FloatLookup[i] = i * s;
                }
            }

            var x = u32FloatLookup[@in & 0xff]; @in >>= 8;
            var y = u32FloatLookup[@in & 0xff]; @in >>= 8;
            var z = u32FloatLookup[@in & 0xff]; @in >>= 8;
            var w = u32FloatLookup[@in & 0xff];

            return new ImVec4(x, y, z, w);
            //return new ImVec4((@in & 0xFF), ((@in >> 8) & 0xFF), ((@in >> 16) & 0xFF), (@in >> 24)) * s;
        }

        internal static uint ColorConvertFloat4ToU32(ImVec4 @in)
        {
            uint @out;
            @out = (uint)((Saturate(@in.x)) * 255.0f + 0.5f);
            @out |= ((uint)((Saturate(@in.y)) * 255.0f + 0.5f) << 8);
            @out |= ((uint)((Saturate(@in.z)) * 255.0f + 0.5f) << 16);
            @out |= ((uint)((Saturate(@in.w)) * 255.0f + 0.5f) << 24);
            return @out;
        }

        // Convert hsv floats ([0-1],[0-1],[0-1]) to rgb floats ([0-1],[0-1],[0-1]), from Foley & van Dam p593
        // also http://en.wikipedia.org/wiki/HSL_and_HSV
        internal static void ColorConvertHSVtoRGB(float h, float s, float v, out float out_r, out float out_g, out float out_b)
        {
            if (s == 0.0f)
            {
                // gray
                out_r = out_g = out_b = v;
                return;
            }

            h = fmodf(h, 1.0f) / (60.0f / 360.0f);
            int i = (int)h;
            float f = h - (float)i;
            float p = v * (1.0f - s);
            float q = v * (1.0f - s * f);
            float t = v * (1.0f - s * (1.0f - f));

            switch (i)
            {
                case 0: out_r = v; out_g = t; out_b = p; break;
                case 1: out_r = q; out_g = v; out_b = p; break;
                case 2: out_r = p; out_g = v; out_b = t; break;
                case 3: out_r = p; out_g = q; out_b = v; break;
                case 4: out_r = t; out_g = p; out_b = v; break;
                case 5: default: out_r = v; out_g = p; out_b = q; break;
            }
        }



        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        #endregion static helpers

        private static ImGui _instance = null;

        public static ImGui Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ImGui();
                return _instance;
            }
        }

        internal ImGuiState State                       = new ImGuiState();
        internal static ImFontAtlas ImDefaultFontAtlas  = new ImFontAtlas();
        //ImGuiState* GImGui = &GImDefaultState;

        // Statically allocated default font atlas. This is merely a maneuver to keep ImFontAtlas definition at the bottom of the .h file (otherwise it'd be inside ImGuiIO)
        // Also we wouldn't be able to new() one at this point, before users may define IO.MemAllocFn.

        public ImVec2 FontTexUvWhitePixel { get { return Font.ContainerAtlas.TexUvWhitePixel; } }                // (Shortcut) == Font.TexUvWhitePixel
        public ImGuiStyle Style { get { return State.Style; } }
        public ImFont Font { get { return State.Font; } }
        public float FontSize { get { return Font.FontSize; } }
        //public ImFont Font { get; private set; }
        //public float FontSize { get; private set; }

        public ImGuiIO GetIO()
        {
            return State.IO;
        }

        public void NewFrame()
        {
            //ImGuiState & g = *GImGui;
            var g = State;

            // Check user data
            System.Diagnostics.Debug.Assert(g.IO.DeltaTime >= 0.0f);               // Need a positive DeltaTime (zero is tolerated but will cause some timing issues)
            System.Diagnostics.Debug.Assert(g.IO.DisplaySize.x >= 0.0f && g.IO.DisplaySize.y >= 0.0f);
            System.Diagnostics.Debug.Assert(g.IO.Fonts.Fonts.Size > 0);           // Font Atlas not created. Did you call io.Fonts.GetTexDataAsRGBA32 / GetTexDataAsAlpha8 ?
            System.Diagnostics.Debug.Assert(g.IO.Fonts.Fonts[0].IsLoaded());     // Font Atlas not created. Did you call io.Fonts.GetTexDataAsRGBA32 / GetTexDataAsAlpha8 ?
            System.Diagnostics.Debug.Assert(g.Style.CurveTessellationTol > 0.0f);  // Invalid style setting

            if (!g.Initialized)
            {
                // Initialize on first frame
                //g.LogClipboard = (ImGuiTextBuffer*)ImGui::MemAlloc(sizeof(ImGuiTextBuffer));
                //TODO: g.LogClipboard = new ImGuiTextBuffer();
                //IM_PLACEMENT_NEW(g.LogClipboard) ImGuiTextBuffer();

                //TODO: System.Diagnostics.Debug.Assert(g.Settings.empty());
                //TODO: 
                LoadSettings();
                g.Initialized = true;
            }

            SetCurrentFont(g.IO.Fonts.Fonts[0]);

            g.Time += g.IO.DeltaTime;
            g.FrameCount += 1;
            g.Tooltip = null;
            g.OverlayDrawList.Clear();
            g.OverlayDrawList.PushTextureID(g.IO.Fonts.TexID);
            g.OverlayDrawList.PushClipRectFullScreen();
            g.OverlayDrawList.AddDrawCmd();

            // Mark rendering data as invalid to prevent user who may have a handle on it to use it
            g.RenderDrawData.Valid = false;
            g.RenderDrawData.CmdLists = null;
            g.RenderDrawData.CmdListsCount = g.RenderDrawData.TotalVtxCount = g.RenderDrawData.TotalIdxCount = 0;

            // Update inputs state
            if (g.IO.MousePos.x < 0 && g.IO.MousePos.y < 0)
                g.IO.MousePos = new ImVec2(-9999.0f, -9999.0f);
            if ((g.IO.MousePos.x < 0 && g.IO.MousePos.y < 0) || (g.IO.MousePosPrev.x < 0 && g.IO.MousePosPrev.y < 0))   // if mouse just appeared or disappeared (negative coordinate) we cancel out movement in MouseDelta
                g.IO.MouseDelta = new ImVec2(0.0f, 0.0f);
            else
                g.IO.MouseDelta = g.IO.MousePos - g.IO.MousePosPrev;
            g.IO.MousePosPrev = g.IO.MousePos;
            for (int i = 0; i < g.IO.MouseDown.Length; i++)
            {
                g.IO.MouseClicked[i] = g.IO.MouseDown[i] && g.IO.MouseDownDuration[i] < 0.0f;
                g.IO.MouseReleased[i] = !g.IO.MouseDown[i] && g.IO.MouseDownDuration[i] >= 0.0f;
                g.IO.MouseDownDurationPrev[i] = g.IO.MouseDownDuration[i];
                g.IO.MouseDownDuration[i] = g.IO.MouseDown[i] ? (g.IO.MouseDownDuration[i] < 0.0f ? 0.0f : g.IO.MouseDownDuration[i] + g.IO.DeltaTime) : -1.0f;
                g.IO.MouseDoubleClicked[i] = false;
                if (g.IO.MouseClicked[i])
                {
                    if (g.Time - g.IO.MouseClickedTime[i] < g.IO.MouseDoubleClickTime)
                    {
                        if (ImGui.LengthSqr(g.IO.MousePos - g.IO.MouseClickedPos[i]) < g.IO.MouseDoubleClickMaxDist * g.IO.MouseDoubleClickMaxDist)
                            g.IO.MouseDoubleClicked[i] = true;
                        g.IO.MouseClickedTime[i] = float.MinValue;// -FLT_MAX;    // so the third click isn't turned into a double-click
                    }
                    else
                    {
                        g.IO.MouseClickedTime[i] = g.Time;
                    }
                    g.IO.MouseClickedPos[i] = g.IO.MousePos;
                    g.IO.MouseDragMaxDistanceSqr[i] = 0.0f;
                }
                else if (g.IO.MouseDown[i])
                {
                    g.IO.MouseDragMaxDistanceSqr[i] = ImGui.Max(g.IO.MouseDragMaxDistanceSqr[i], ImGui.LengthSqr(g.IO.MousePos - g.IO.MouseClickedPos[i]));
                }
            }
            //memcpy(g.IO.KeysDownDurationPrev, g.IO.KeysDownDuration, sizeof(g.IO.KeysDownDuration));
            for (int i = 0; i < g.IO.KeysDown.Length; i++)
                g.IO.KeysDownDuration[i] = g.IO.KeysDown[i] ? (g.IO.KeysDownDuration[i] < 0.0f ? 0.0f : g.IO.KeysDownDuration[i] + g.IO.DeltaTime) : -1.0f;

            // Calculate frame-rate for the user, as a purely luxurious feature
            g.FramerateSecPerFrameAccum += g.IO.DeltaTime - g.FramerateSecPerFrame[g.FramerateSecPerFrameIdx];
            g.FramerateSecPerFrame[g.FramerateSecPerFrameIdx] = g.IO.DeltaTime;
            g.FramerateSecPerFrameIdx = (g.FramerateSecPerFrameIdx + 1) % g.FramerateSecPerFrame.Length;
            g.IO.Framerate = 1.0f / (g.FramerateSecPerFrameAccum / (float)g.FramerateSecPerFrame.Length);

            // Clear reference to active widget if the widget isn't alive anymore
            g.HoveredIdPreviousFrame = g.HoveredId;
            g.HoveredId = 0;
            g.HoveredIdAllowOverlap = false;
            if (!g.ActiveIdIsAlive && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != 0)
                SetActiveID(0);
            g.ActiveIdPreviousFrame = g.ActiveId;
            g.ActiveIdIsAlive = false;
            g.ActiveIdIsJustActivated = false;
            if (g.ActiveId == 0)
                g.MovedWindow = null;

            // Delay saving settings so we don't spam disk too much
            if (g.SettingsDirtyTimer > 0.0f)
            {
                g.SettingsDirtyTimer -= g.IO.DeltaTime;
                //TODO: if (g.SettingsDirtyTimer <= 0.0f)
                //TODO:    SaveSettings();
            }

            // Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            g.HoveredWindow = FindHoveredWindow(g.IO.MousePos, false);
            if (g.HoveredWindow != null && ((g.HoveredWindow.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow))
                g.HoveredRootWindow = g.HoveredWindow.RootWindow;
            else
                g.HoveredRootWindow = FindHoveredWindow(g.IO.MousePos, true);

            ImGuiWindow modal_window = GetFrontMostModalRootWindow();
            if (modal_window != null)
            {
                g.ModalWindowDarkeningRatio = ImGui.Min(g.ModalWindowDarkeningRatio + g.IO.DeltaTime * 6.0f, 1.0f);
                if (g.HoveredRootWindow != modal_window)
                    g.HoveredRootWindow = g.HoveredWindow = null;
            }
            else
            {
                g.ModalWindowDarkeningRatio = 0.0f;
            }

            // Are we using inputs? Tell user so they can capture/discard the inputs away from the rest of their application.
            // When clicking outside of a window we assume the click is owned by the application and won't request capture. We need to track click ownership.
            int mouse_earliest_button_down = -1;
            bool mouse_any_down = false;
            for (int i = 0; i < g.IO.MouseDown.Length; i++)
            {
                if (g.IO.MouseClicked[i])
                    g.IO.MouseDownOwned[i] = (g.HoveredWindow != null) || (!g.OpenedPopupStack.empty());
                mouse_any_down |= g.IO.MouseDown[i];
                if (g.IO.MouseDown[i])
                    if (mouse_earliest_button_down == -1 || g.IO.MouseClickedTime[mouse_earliest_button_down] > g.IO.MouseClickedTime[i])
                        mouse_earliest_button_down = i;
            }
            bool mouse_avail_to_imgui = (mouse_earliest_button_down == -1) || g.IO.MouseDownOwned[mouse_earliest_button_down];
            if (g.CaptureMouseNextFrame != -1)
                g.IO.WantCaptureMouse = (g.CaptureMouseNextFrame != 0);
            else
                g.IO.WantCaptureMouse = (mouse_avail_to_imgui && (g.HoveredWindow != null || mouse_any_down)) || (g.ActiveId != 0) || (!g.OpenedPopupStack.empty());
            g.IO.WantCaptureKeyboard = (g.CaptureKeyboardNextFrame != -1) ? (g.CaptureKeyboardNextFrame != 0) : (g.ActiveId != 0);
            g.IO.WantTextInput = (g.ActiveId != 0 && g.InputTextState.Id == g.ActiveId);
            g.MouseCursor = ImGuiMouseCursor.ImGuiMouseCursor_Arrow;
            g.CaptureMouseNextFrame = g.CaptureKeyboardNextFrame = -1;
            g.OsImePosRequest = new ImVec2(1.0f, 1.0f); // OS Input Method Editor showing on top-left of our window by default

            // If mouse was first clicked outside of ImGui bounds we also cancel out hovering.
            if (!mouse_avail_to_imgui)
                g.HoveredWindow = g.HoveredRootWindow = null;

            // Scale & Scrolling
            if (g.HoveredWindow != null && g.IO.MouseWheel != 0.0f && !g.HoveredWindow.Collapsed)
            {
                ImGuiWindow window = g.HoveredWindow;
                if (g.IO.KeyCtrl)
                {
                    if (g.IO.FontAllowUserScaling)
                    {
                        // Zoom / Scale window
                        float new_font_scale = ImGui.Clamp(window.FontWindowScale + g.IO.MouseWheel * 0.10f, 0.50f, 2.50f);
                        float scale = new_font_scale / window.FontWindowScale;
                        window.FontWindowScale = new_font_scale;

                        ImVec2 offset = window.Size * (1.0f - scale) * (g.IO.MousePos - window.Pos) / window.Size;
                        window.Pos += offset;
                        window.PosFloat += offset;
                        window.Size *= scale;
                        window.SizeFull *= scale;
                    }
                }
                else
                {
                    // Scroll
                    if (!((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollWithMouse) == ImGuiWindowFlags.ImGuiWindowFlags_NoScrollWithMouse))
                    {
                        int scroll_lines = ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) == ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) ? 3 : 5;
                        SetWindowScrollY(window, window.Scroll.y - g.IO.MouseWheel * window.CalcFontSize() * scroll_lines);
                    }
                }
            }

            // Pressing TAB activate widget focus
            // NB: Don't discard FocusedWindow if it isn't active, so that a window that go on/off programatically won't lose its keyboard focus.
            if (g.ActiveId == 0 && g.FocusedWindow != null && g.FocusedWindow.Active && IsKeyPressedMap(ImGuiKey.ImGuiKey_Tab, false))
                g.FocusedWindow.FocusIdxTabRequestNext = 0;

            // Mark all windows as not visible
            for (int i = 0; i != g.Windows.Size; i++)
            {
                ImGuiWindow window = g.Windows[i];
                window.WasActive = window.Active;
                window.Active = false;
                window.Accessed = false;
            }

            // No window should be open at the beginning of the frame.
            // But in order to allow the user to call NewFrame() multiple times without calling Render(), we are doing an explicit clear.
            g.CurrentWindowStack.resize(0);
            g.CurrentPopupStack.resize(0);
            CloseInactivePopups();

            // Create implicit window - we will only render it if the user has added something to it.
            SetNextWindowSize(new ImVec2(400, 400), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
            Begin("Debug");
        }

        // Push a new ImGui window to add widgets to.
        // - A default window called "Debug" is automatically stacked at the beginning of every frame so you can use widgets without explicitly calling a Begin/End pair.
        // - Begin/End can be called multiple times during the frame with the same window name to append content.
        // - 'size_on_first_use' for a regular window denote the initial size for first-time creation (no saved data) and isn't that useful. Use SetNextWindowSize() prior to calling Begin() for more flexible window manipulation.
        // - The window name is used as a unique identifier to preserve window information across frames (and save rudimentary information to the .ini file).
        //   You can use the "##" or "###" markers to use the same label with different id, or same id with different label. See documentation at the top of this file.
        // - Return false when window is collapsed, so you can early out in your code. You always need to call ImGui::End() even if false is returned.
        // - Passing 'bool* p_opened' displays a Close button on the upper-right corner of the window, the pointed value will be set to false when the button is pressed.
        // - Passing non-zero 'size' is roughly equivalent to calling SetNextWindowSize(size, ImGuiSetCond_FirstUseEver) prior to calling Begin().        
        public bool Begin(string name)
        {
            bool throwAway = true;
            return BeginEx(name, ref throwAway, new ImVec2(0f, 0f), -1.0f, 0, false);
        }

        bool Begin(string name, ref bool p_opened, ImGuiWindowFlags flags = 0)
        {
            return BeginEx(name, ref p_opened, new ImVec2(0f, 0f), -1.0f, flags, true);
        }

        bool Begin(string name, ref bool p_opened, ImVec2 size_on_first_use, float bg_alpha, ImGuiWindowFlags flags)
        {
            return BeginEx(name, ref p_opened, size_on_first_use, bg_alpha, flags, true);
        }

        bool BeginEx(string name, ref bool p_opened, ImVec2 size_on_first_use, float bg_alpha, ImGuiWindowFlags flags, bool allowClose = false)
        {
            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            System.Diagnostics.Debug.Assert(name != null);                        // Window name required
            System.Diagnostics.Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            System.Diagnostics.Debug.Assert(g.FrameCountEnded != g.FrameCount);   // Called ImGui::Render() or ImGui::EndFrame() and haven't called ImGui::NewFrame() again yet

            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoInputs) > 0)
                flags |= ImGuiWindowFlags.ImGuiWindowFlags_NoMove | ImGuiWindowFlags.ImGuiWindowFlags_NoResize;

            // Find or create
            bool window_is_new = false;
            ImGuiWindow window = FindWindowByName(name);
            if (window == null)
            {
                window = CreateNewWindow(name, size_on_first_use, flags);
                window_is_new = true;
            }

            int current_frame = GetFrameCount();
            bool first_begin_of_the_frame = (window.LastFrameActive != current_frame);
            if (first_begin_of_the_frame)
                window.Flags = flags;
            else
                flags = window.Flags;

            // Add to stack
            ImGuiWindow parent_window = !g.CurrentWindowStack.empty() ? g.CurrentWindowStack.back() : null;
            g.CurrentWindowStack.push_back(window);
            SetCurrentWindow(window);
            CheckStacksSize(window, true);
            System.Diagnostics.Debug.Assert(parent_window != null || (flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == 0);

            bool window_was_active = (window.LastFrameActive == current_frame - 1);   // Not using !WasActive because the implicit "Debug" window would always toggle off.on
            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) > 0)
            {
                ImGuiPopupRef popup_ref = g.OpenedPopupStack[g.CurrentPopupStack.Size];
                window_was_active &= (window.PopupID == popup_ref.PopupID);
                window_was_active &= (window == popup_ref.Window);
                popup_ref.Window = window;
                g.CurrentPopupStack.push_back(popup_ref);
                window.PopupID = popup_ref.PopupID;
            }

            bool window_appearing_after_being_hidden = (window.HiddenFrames == 1);

            // Process SetNextWindow***() calls
            bool window_pos_set_by_api = false, window_size_set_by_api = false;
            if (g.SetNextWindowPosCond > 0)
            {
                ImVec2 backup_cursor_pos = window.DC.CursorPos;                  // FIXME: not sure of the exact reason of this anymore :( need to look into that.
                if (!window_was_active || window_appearing_after_being_hidden)
                    window.SetWindowPosAllowFlags |= (int)ImGuiSetCond.ImGuiSetCond_Appearing;

                window_pos_set_by_api = (window.SetWindowPosAllowFlags & (int)g.SetNextWindowPosCond) != 0;
                if (window_pos_set_by_api && LengthSqr(g.SetNextWindowPosVal - new ImVec2(-float.MaxValue, -float.MaxValue)) < 0.001f)
                {
                    window.SetWindowPosCenterWanted = true;                            // May be processed on the next frame if this is our first frame and we are measuring size
                    window.SetWindowPosAllowFlags &= ~(int)(ImGuiSetCond.ImGuiSetCond_Once | ImGuiSetCond.ImGuiSetCond_FirstUseEver | ImGuiSetCond.ImGuiSetCond_Appearing);
                }
                else
                {
                    SetWindowPos(window, g.SetNextWindowPosVal, g.SetNextWindowPosCond);
                }
                window.DC.CursorPos = backup_cursor_pos;
                g.SetNextWindowPosCond = 0;
            }
            if (g.SetNextWindowSizeCond > 0)
            {
                if (!window_was_active || window_appearing_after_being_hidden)
                    window.SetWindowSizeAllowFlags |= (int)ImGuiSetCond.ImGuiSetCond_Appearing;
                window_size_set_by_api = (window.SetWindowSizeAllowFlags & (int)g.SetNextWindowSizeCond) != 0;
                SetWindowSize(window, g.SetNextWindowSizeVal, g.SetNextWindowSizeCond);
                g.SetNextWindowSizeCond = 0;
            }
            if (g.SetNextWindowContentSizeCond > 0)
            {
                window.SizeContentsExplicit = g.SetNextWindowContentSizeVal;
                g.SetNextWindowContentSizeCond = 0;
            }
            else if (first_begin_of_the_frame)
            {
                window.SizeContentsExplicit = new ImVec2(0.0f, 0.0f);
            }
            if (g.SetNextWindowCollapsedCond > 0)
            {
                if (!window_was_active || window_appearing_after_being_hidden)
                    window.SetWindowCollapsedAllowFlags |= (int)ImGuiSetCond.ImGuiSetCond_Appearing;
                SetWindowCollapsed(window, g.SetNextWindowCollapsedVal, g.SetNextWindowCollapsedCond);
                g.SetNextWindowCollapsedCond = 0;
            }
            if (g.SetNextWindowFocus)
            {
                SetWindowFocus();
                g.SetNextWindowFocus = false;
            }

            // Update known root window (if we are a child window, otherwise window == window.RootWindow)
            int root_idx, root_non_popup_idx;
            for (root_idx = g.CurrentWindowStack.Size - 1; root_idx > 0; root_idx--)
                if ((g.CurrentWindowStack[root_idx].Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == 0)
                    break;
            for (root_non_popup_idx = root_idx; root_non_popup_idx > 0; root_non_popup_idx--)
                if ((g.CurrentWindowStack[root_non_popup_idx].Flags & (ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow | ImGuiWindowFlags.ImGuiWindowFlags_Popup)) == 0)
                    break;
            window.RootWindow = g.CurrentWindowStack[root_idx];
            window.RootNonPopupWindow = g.CurrentWindowStack[root_non_popup_idx];      // This is merely for displaying the TitleBgActive color.

            // Default alpha
            if (bg_alpha < 0.0f)
                bg_alpha = style.WindowFillAlphaDefault;

            // When reusing window again multiple times a frame, just append content (don't need to setup again)
            if (first_begin_of_the_frame)
            {
                window.Active = true;
                window.BeginCount = 0;
                window.DrawList.Clear();
                window.ClipRect = new ImRect(-float.MaxValue, -float.MaxValue, +float.MaxValue, +float.MaxValue);
                window.LastFrameActive = current_frame;
                window.IDStack.resize(1);

                // Setup texture, outer clipping rectangle
                window.DrawList.PushTextureID(g.Font.ContainerAtlas.TexID);
                ImRect fullscreen_rect = GetVisibleRect();
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) > 0 && (flags & (ImGuiWindowFlags.ImGuiWindowFlags_ComboBox | ImGuiWindowFlags.ImGuiWindowFlags_Popup)) == 0)
                    PushClipRect(parent_window.ClipRect.Min, parent_window.ClipRect.Max, true);
                else
                    PushClipRect(fullscreen_rect.Min, fullscreen_rect.Max, true);

                // New windows appears in front
                if (!window_was_active)
                {
                    window.AutoPosLastDirection = -1;

                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoFocusOnAppearing) == 0)
                        if ((flags & (ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow | ImGuiWindowFlags.ImGuiWindowFlags_Tooltip)) == 0 || (flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) > 0)
                            FocusWindow(window);

                    // Popup first latch mouse position, will position itself when it appears next frame
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0 && !window_pos_set_by_api)
                        window.PosFloat = g.IO.MousePos;
                }

                // Collapse window by double-clicking on title bar
                // At this point we don't have a clipping rectangle setup yet, so we can use the title bar area for hit detection and drawing
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) == 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoCollapse) == 0)
                {
                    ImRect title_bar_rect2 = window.TitleBarRect();
                    if (g.HoveredWindow == window && IsMouseHoveringRect(title_bar_rect2.Min, title_bar_rect2.Max) && g.IO.MouseDoubleClicked[0])
                    {
                        window.Collapsed = !window.Collapsed;
                        if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings) == 0)
                            MarkSettingsDirty();
                        FocusWindow(window);
                    }
                }
                else
                {
                    window.Collapsed = false;
                }

                // SIZE

                // Save contents size from last frame for auto-fitting (unless explicitly specified)
                window.SizeContents.x = (float)(int)((window.SizeContentsExplicit.x != 0.0f) ? window.SizeContentsExplicit.x : ((window_is_new ? 0.0f : window.DC.CursorMaxPos.x - window.Pos.x) + window.Scroll.x));
                window.SizeContents.y = (float)(int)((window.SizeContentsExplicit.y != 0.0f) ? window.SizeContentsExplicit.y : ((window_is_new ? 0.0f : window.DC.CursorMaxPos.y - window.Pos.y) + window.Scroll.y));

                // Hide popup/tooltip window when first appearing while we measure size (because we recycle them)
                if (window.HiddenFrames > 0)
                    window.HiddenFrames--;
                if ((flags & (ImGuiWindowFlags.ImGuiWindowFlags_Popup | ImGuiWindowFlags.ImGuiWindowFlags_Tooltip)) != 0 && !window_was_active)
                {
                    window.HiddenFrames = 1;
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize) > 0)
                    {
                        if (!window_size_set_by_api)
                            window.Size = window.SizeFull = new ImVec2(0, 0);
                        window.SizeContents = new ImVec2(0, 0);
                    }
                }

                // Lock window padding so that altering the ShowBorders flag for children doesn't have side-effects.
                window.WindowPadding = ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) > 0 && (flags & (ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders | ImGuiWindowFlags.ImGuiWindowFlags_ComboBox | ImGuiWindowFlags.ImGuiWindowFlags_Popup)) == 0) ? new ImVec2(0, 0) : style.WindowPadding;

                // Calculate auto-fit size
                ImVec2 size_auto_fit;
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) != 0)
                {
                    // Tooltip always resize. We keep the spacing symmetric on both axises for aesthetic purpose.
                    size_auto_fit = window.SizeContents + window.WindowPadding - new ImVec2(0.0f, style.ItemSpacing.y);
                }
                else
                {
                    size_auto_fit = Clamp(window.SizeContents + window.WindowPadding, style.WindowMinSize, Max(style.WindowMinSize, g.IO.DisplaySize - g.Style.DisplaySafeAreaPadding));

                    // Handling case of auto fit window not fitting in screen on one axis, we are growing auto fit size on the other axis to compensate for expected scrollbar. FIXME: Might turn bigger than DisplaySize-WindowPadding.
                    if (size_auto_fit.x < window.SizeContents.x && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar) == 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_HorizontalScrollbar) > 0)
                        size_auto_fit.y += style.ScrollbarSize;
                    if (size_auto_fit.y < window.SizeContents.y && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar) == 0)
                        size_auto_fit.x += style.ScrollbarSize;
                    size_auto_fit.y = Max(size_auto_fit.y - style.ItemSpacing.y, 0.0f);
                }

                // Handle automatic resize
                if (window.Collapsed)
                {
                    // We still process initial auto-fit on collapsed windows to get a window width,
                    // But otherwise we don't honor ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize when collapsed.
                    if (window.AutoFitFramesX > 0)
                        window.SizeFull.x = window.AutoFitOnlyGrows ? Max(window.SizeFull.x, size_auto_fit.x) : size_auto_fit.x;
                    if (window.AutoFitFramesY > 0)
                        window.SizeFull.y = window.AutoFitOnlyGrows ? Max(window.SizeFull.y, size_auto_fit.y) : size_auto_fit.y;
                    window.Size = window.TitleBarRect().GetSize();
                }
                else
                {
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize) > 0 && !window_size_set_by_api)
                    {
                        window.SizeFull = size_auto_fit;
                    }
                    else if ((window.AutoFitFramesX > 0 || window.AutoFitFramesY > 0) && !window_size_set_by_api)
                    {
                        // Auto-fit only grows during the first few frames
                        if (window.AutoFitFramesX > 0)
                            window.SizeFull.x = window.AutoFitOnlyGrows ? Max(window.SizeFull.x, size_auto_fit.x) : size_auto_fit.x;
                        if (window.AutoFitFramesY > 0)
                            window.SizeFull.y = window.AutoFitOnlyGrows ? Max(window.SizeFull.y, size_auto_fit.y) : size_auto_fit.y;
                        if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings) == 0)
                            MarkSettingsDirty();
                    }
                    window.Size = window.SizeFull;
                }

                // Minimum window size
                if ((flags & (ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow | ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize)) == 0)
                {
                    window.SizeFull = Max(window.SizeFull, style.WindowMinSize);
                    if (!window.Collapsed)
                        window.Size = window.SizeFull;
                }

                // POSITION

                // Position child window
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) > 0)
                    parent_window.DC.ChildWindows.push_back(window);
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) > 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) == 0)
                {
                    window.Pos = window.PosFloat = parent_window.DC.CursorPos;
                    window.Size = window.SizeFull = size_on_first_use; // NB: argument name 'size_on_first_use' misleading here, it's really just 'size' as provided by user.
                }

                bool window_pos_center = false;
                window_pos_center |= (window.SetWindowPosCenterWanted && window.HiddenFrames == 0);
                window_pos_center |= ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Modal) > 0 && !window_pos_set_by_api && window_appearing_after_being_hidden);
                if (window_pos_center)
                {
                    // Center (any sort of window)
                    SetWindowPos(Max(style.DisplaySafeAreaPadding, fullscreen_rect.GetCenter() - window.SizeFull * 0.5f));
                }
                else if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu) > 0)
                {
                    System.Diagnostics.Debug.Assert(window_pos_set_by_api);
                    ImRect rect_to_avoid;
                    if (parent_window.DC.MenuBarAppending)
                        rect_to_avoid = new ImRect(-float.MaxValue, parent_window.Pos.y + parent_window.TitleBarHeight(), float.MaxValue, parent_window.Pos.y + parent_window.TitleBarHeight() + parent_window.MenuBarHeight());
                    else
                        rect_to_avoid = new ImRect(parent_window.Pos.x + style.ItemSpacing.x, -float.MaxValue, parent_window.Pos.x + parent_window.Size.x - style.ItemSpacing.x - parent_window.ScrollbarSizes.x, float.MaxValue); // We want some overlap to convey the relative depth of each popup (here hard-coded to 4)
                    window.PosFloat = FindBestPopupWindowPos(window.PosFloat, window.Size, ref window.AutoPosLastDirection, rect_to_avoid);
                }
                else if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0 && !window_pos_set_by_api && window_appearing_after_being_hidden)
                {
                    ImRect rect_to_avoid = new ImRect(window.PosFloat.x - 1, window.PosFloat.y - 1, window.PosFloat.x + 1, window.PosFloat.y + 1);
                    window.PosFloat = FindBestPopupWindowPos(window.PosFloat, window.Size, ref window.AutoPosLastDirection, rect_to_avoid);
                }

                // Position tooltip (always follows mouse)
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) != 0 && !window_pos_set_by_api)
                {
                    ImRect rect_to_avoid = new ImRect(g.IO.MousePos.x - 16, g.IO.MousePos.y - 8, g.IO.MousePos.x + 24, g.IO.MousePos.y + 24); // FIXME: Completely hard-coded. Perhaps center on cursor hit-point instead?
                    window.PosFloat = FindBestPopupWindowPos(g.IO.MousePos, window.Size, ref window.AutoPosLastDirection, rect_to_avoid);
                    if (window.AutoPosLastDirection == -1)
                        window.PosFloat = g.IO.MousePos + new ImVec2(2, 2); // If there's not enough room, for tooltip we prefer avoiding the cursor at all cost even if it means that part of the tooltip won't be visible.
                }

                // User moving window (at the beginning of the frame to avoid input lag or sheering). Only valid for root windows.
                KeepAliveID(window.MoveID);
                if (g.ActiveId == window.MoveID)
                {
                    if (g.IO.MouseDown[0])
                    {
                        if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoMove) == 0)
                        {
                            window.PosFloat += g.IO.MouseDelta;
                            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings) == 0)
                                MarkSettingsDirty();
                        }
                        System.Diagnostics.Debug.Assert(g.MovedWindow != null);
                        FocusWindow(g.MovedWindow);
                    }
                    else
                    {
                        SetActiveID(0);
                        g.MovedWindow = null;   // Not strictly necessary but doing it for sanity.
                    }
                }

                // Clamp position so it stays visible
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) == 0)
                {
                    if (!window_pos_set_by_api && window.AutoFitFramesX <= 0 && window.AutoFitFramesY <= 0 && g.IO.DisplaySize.x > 0.0f && g.IO.DisplaySize.y > 0.0f) // Ignore zero-sized display explicitly to avoid losing positions if a window manager reports zero-sized window when initializing or minimizing.
                    {
                        ImVec2 padding = Max(style.DisplayWindowPadding, style.DisplaySafeAreaPadding);
                        window.PosFloat = Max(window.PosFloat + window.Size, padding) - window.Size;
                        window.PosFloat = Min(window.PosFloat, g.IO.DisplaySize - padding);
                    }
                }
                window.Pos = new ImVec2((float)(int)window.PosFloat.x, (float)(int)window.PosFloat.y);

                // Default item width. Make it proportional to window size if window manually resizes
                if (window.Size.x > 0.0f && (flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) == 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize) == 0)
                    window.ItemWidthDefault = (float)(int)(window.Size.x * 0.65f);
                else
                    window.ItemWidthDefault = (float)(int)(g.FontSize * 16.0f);

                // Prepare for focus requests
                window.FocusIdxAllRequestCurrent = (window.FocusIdxAllRequestNext == int.MaxValue || window.FocusIdxAllCounter == -1) ? int.MaxValue : (window.FocusIdxAllRequestNext + (window.FocusIdxAllCounter + 1)) % (window.FocusIdxAllCounter + 1);
                window.FocusIdxTabRequestCurrent = (window.FocusIdxTabRequestNext == int.MaxValue || window.FocusIdxTabCounter == -1) ? int.MaxValue : (window.FocusIdxTabRequestNext + (window.FocusIdxTabCounter + 1)) % (window.FocusIdxTabCounter + 1);
                window.FocusIdxAllCounter = window.FocusIdxTabCounter = -1;
                window.FocusIdxAllRequestNext = window.FocusIdxTabRequestNext = int.MaxValue;

                // Apply scrolling
                if (window.ScrollTarget.x < float.MaxValue)
                {
                    window.Scroll.x = window.ScrollTarget.x;
                    window.ScrollTarget.x = float.MaxValue;
                }
                if (window.ScrollTarget.y < float.MaxValue)
                {
                    float center_ratio = window.ScrollTargetCenterRatio.y;
                    window.Scroll.y = window.ScrollTarget.y - ((1.0f - center_ratio) * window.TitleBarHeight()) - (center_ratio * window.SizeFull.y);
                    window.ScrollTarget.y = float.MaxValue;
                }
                window.Scroll = Max(window.Scroll, new ImVec2(0.0f, 0.0f));
                if (!window.Collapsed && !window.SkipItems)
                    window.Scroll = Min(window.Scroll, Max(new ImVec2(0.0f, 0.0f), window.SizeContents - window.SizeFull + window.ScrollbarSizes));

                // Modal window darkens what is behind them
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Modal) != 0 && window == GetFrontMostModalRootWindow())
                    window.DrawList.AddRectFilled(fullscreen_rect.Min, fullscreen_rect.Max, GetColorU32(ImGuiCol.ImGuiCol_ModalWindowDarkening, g.ModalWindowDarkeningRatio));

                // Draw window + handle manual resize
                ImRect title_bar_rect = window.TitleBarRect();
                float window_rounding = (flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) > 0 ? style.ChildWindowRounding : style.WindowRounding;
                if (window.Collapsed)
                {
                    // Draw title bar only
                    RenderFrame(title_bar_rect.GetTL(), title_bar_rect.GetBR(), GetColorU32(ImGuiCol.ImGuiCol_TitleBgCollapsed), true, window_rounding);
                }
                else
                {
                    uint resize_col = 0;
                    float resize_corner_size = Max(g.FontSize * 1.35f, window_rounding + 1.0f + g.FontSize * 0.2f);
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize) == 0 && window.AutoFitFramesX <= 0 && window.AutoFitFramesY <= 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoResize) == 0)
                    {
                        // Manual resize
                        ImVec2 br = window.Rect().GetBR();
                        ImRect resize_rect = new ImRect(br - new ImVec2(resize_corner_size * 0.75f, resize_corner_size * 0.75f), br);
                        uint resize_id = window.GetID("#RESIZE");
                        bool? hovered = false, held = false;
                        ButtonBehavior(resize_rect, resize_id, ref hovered, ref held, ImGuiButtonFlags.ImGuiButtonFlags_FlattenChilds);
                        resize_col = GetColorU32(held.Value ? ImGuiCol.ImGuiCol_ResizeGripActive : hovered.Value ? ImGuiCol.ImGuiCol_ResizeGripHovered : ImGuiCol.ImGuiCol_ResizeGrip);

                        if (hovered.Value || held.Value)
                            g.MouseCursor = ImGuiMouseCursor.ImGuiMouseCursor_ResizeNWSE;

                        if (g.HoveredWindow == window && held.Value && g.IO.MouseDoubleClicked[0])
                        {
                            // Manual auto-fit when double-clicking
                            window.SizeFull = size_auto_fit;
                            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings) == 0)
                                MarkSettingsDirty();
                            SetActiveID(0);
                        }
                        else if (held.Value)
                        {
                            window.SizeFull = Max(window.SizeFull + g.IO.MouseDelta, style.WindowMinSize);
                            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings) == 0)
                                MarkSettingsDirty();
                        }

                        window.Size = window.SizeFull;
                        title_bar_rect = window.TitleBarRect();
                    }

                    // Scrollbars
                    window.ScrollbarY = (flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysVerticalScrollbar) > 0 || ((window.SizeContents.y > window.Size.y + style.ItemSpacing.y) && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar) == 0);
                    window.ScrollbarX = (flags & ImGuiWindowFlags.ImGuiWindowFlags_ForceHorizontalScrollbar) > 0 || ((window.SizeContents.x > window.Size.x - (window.ScrollbarY ? style.ScrollbarSize : 0.0f) - window.WindowPadding.x) && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar) == 0 && (flags & ImGuiWindowFlags.ImGuiWindowFlags_HorizontalScrollbar) > 0);
                    window.ScrollbarSizes = new ImVec2(window.ScrollbarY ? style.ScrollbarSize : 0.0f, window.ScrollbarX ? style.ScrollbarSize : 0.0f);
                    window.BorderSize = (flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) > 0 ? 1.0f : 0.0f;

                    // Window background
                    if (bg_alpha > 0.0f)
                    {
                        ImGuiCol col_idx;
                        if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) != 0)
                            col_idx = ImGuiCol.ImGuiCol_ComboBg;
                        else if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) != 0)
                            col_idx = ImGuiCol.ImGuiCol_TooltipBg;
                        else if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0)
                            col_idx = ImGuiCol.ImGuiCol_WindowBg;
                        else if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0)
                            col_idx = ImGuiCol.ImGuiCol_ChildWindowBg;
                        else
                            col_idx = ImGuiCol.ImGuiCol_WindowBg;
                        window.DrawList.AddRectFilled(window.Pos, window.Pos + window.Size, GetColorU32(col_idx, bg_alpha), window_rounding);
                    }

                    // Title bar
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) == 0)
                        window.DrawList.AddRectFilled(title_bar_rect.GetTL(), title_bar_rect.GetBR(), GetColorU32((g.FocusedWindow != null && window.RootNonPopupWindow == g.FocusedWindow.RootNonPopupWindow) ? ImGuiCol.ImGuiCol_TitleBgActive : ImGuiCol.ImGuiCol_TitleBg), window_rounding, 1 | 2);

                    // Menu bar
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_MenuBar) > 0)
                    {
                        ImRect menu_bar_rect = window.MenuBarRect();
                        window.DrawList.AddRectFilled(menu_bar_rect.GetTL(), menu_bar_rect.GetBR(), GetColorU32(ImGuiCol.ImGuiCol_MenuBarBg), (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) > 0 ? window_rounding : 0.0f, 1 | 2);
                    }

                    // Scrollbars
                    if (window.ScrollbarX)
                        Scrollbar(window, true);
                    if (window.ScrollbarY)
                        Scrollbar(window, false);

                    // Render resize grip
                    // (after the input handling so we don't have a frame of latency)
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoResize) == 0)
                    {
                        ImVec2 br = window.Rect().GetBR();
                        window.DrawList.PathLineTo(br + new ImVec2(-resize_corner_size, -window.BorderSize));
                        window.DrawList.PathLineTo(br + new ImVec2(-window.BorderSize, -resize_corner_size));
                        window.DrawList.PathArcToFast(new ImVec2(br.x - window_rounding - window.BorderSize, br.y - window_rounding - window.BorderSize), window_rounding, 0, 3);
                        window.DrawList.PathFill(resize_col);
                    }

                    // Borders
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) > 0)
                    {
                        window.DrawList.AddRect(window.Pos + new ImVec2(1, 1), window.Pos + window.Size + new ImVec2(1, 1), GetColorU32(ImGuiCol.ImGuiCol_BorderShadow), window_rounding);
                        window.DrawList.AddRect(window.Pos, window.Pos + window.Size, GetColorU32(ImGuiCol.ImGuiCol_Border), window_rounding);
                        if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) == 0)
                            window.DrawList.AddLine(title_bar_rect.GetBL() + new ImVec2(1, 0), title_bar_rect.GetBR() - new ImVec2(1, 0), GetColorU32(ImGuiCol.ImGuiCol_Border));
                    }
                }

                // Setup drawing context
                window.DC.IndentX = 0.0f + window.WindowPadding.x - window.Scroll.x;
                window.DC.ColumnsOffsetX = 0.0f;
                window.DC.CursorStartPos = window.Pos + new ImVec2(window.DC.IndentX + window.DC.ColumnsOffsetX, window.TitleBarHeight() + window.MenuBarHeight() + window.WindowPadding.y - window.Scroll.y);
                window.DC.CursorPos = window.DC.CursorStartPos;
                window.DC.CursorPosPrevLine = window.DC.CursorPos;
                window.DC.CursorMaxPos = window.DC.CursorStartPos;
                window.DC.CurrentLineHeight = window.DC.PrevLineHeight = 0.0f;
                window.DC.CurrentLineTextBaseOffset = window.DC.PrevLineTextBaseOffset = 0.0f;
                window.DC.MenuBarAppending = false;
                window.DC.MenuBarOffsetX = Max(window.WindowPadding.x, style.ItemSpacing.x);
                window.DC.LogLinePosY = window.DC.CursorPos.y - 9999.0f;
                window.DC.ChildWindows.resize(0);
                window.DC.LayoutType = ImGuiLayoutType.ImGuiLayoutType_Vertical;
                window.DC.ItemWidth = window.ItemWidthDefault;
                window.DC.TextWrapPos = -1.0f; // disabled
                window.DC.AllowKeyboardFocus = true;
                window.DC.ButtonRepeat = false;
                window.DC.ItemWidthStack.resize(0);
                window.DC.TextWrapPosStack.resize(0);
                window.DC.AllowKeyboardFocusStack.resize(0);
                window.DC.ButtonRepeatStack.resize(0);
                window.DC.ColorEditMode = ImGuiColorEditMode.ImGuiColorEditMode_UserSelect;
                window.DC.ColumnsCurrent = 0;
                window.DC.ColumnsCount = 1;
                window.DC.ColumnsStartPosY = window.DC.CursorPos.y;
                window.DC.ColumnsCellMinY = window.DC.ColumnsCellMaxY = window.DC.ColumnsStartPosY;
                window.DC.TreeDepth = 0;
                window.DC.StateStorage = window.StateStorage;
                window.DC.GroupStack.resize(0);
                window.MenuColumns.Update(3, style.ItemSpacing.x, !window_was_active);

                if (window.AutoFitFramesX > 0)
                    window.AutoFitFramesX--;
                if (window.AutoFitFramesY > 0)
                    window.AutoFitFramesY--;

                // Title bar
                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) == 0)
                {
                    if (allowClose)
                        CloseWindowButton(ref p_opened);

                    ImVec2 text_size = CalcTextSize(name, 0, -1, true);
                    if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoCollapse) == 0)
                        RenderCollapseTriangle(window.Pos + style.FramePadding, !window.Collapsed, 1.0f, true);

                    ImVec2 text_min = window.Pos + style.FramePadding;
                    ImVec2 text_max = window.Pos + new ImVec2(window.Size.x - style.FramePadding.x, style.FramePadding.y * 2 + text_size.y);
                    ImVec2 clip_max = new ImVec2(window.Pos.x + window.Size.x - (allowClose ? title_bar_rect.GetHeight() - 3 : style.FramePadding.x), text_max.y); // Match the size of CloseWindowButton()
                    bool pad_left = (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoCollapse) == 0;
                    bool pad_right = allowClose;
                    if ((style.WindowTitleAlign & ImGuiAlign.ImGuiAlign_Center) > 0) pad_right = pad_left;
                    if (pad_left) text_min.x += g.FontSize + style.ItemInnerSpacing.x;
                    if (pad_right) text_max.x -= g.FontSize + style.ItemInnerSpacing.x;
                    RenderTextClipped(text_min, text_max, name, 0, -1, text_size, style.WindowTitleAlign, null, clip_max);
                    //RenderTextClipped(text_min, text_max, string.Format("{0:X8}", (uint)window.ID), 0, -1, text_size, style.WindowTitleAlign, null, clip_max);

                }

                // Save clipped aabb so we can access it in constant-time in FindHoveredWindow()
                window.ClippedWindowRect = window.Rect();
                window.ClippedWindowRect.Clip(window.ClipRect);

                // Pressing CTRL+C while holding on a window copy its content to the clipboard
                // This works but 1. doesn't handle multiple Begin/End pairs, 2. recursing into another Begin/End pair - so we need to work that out and add better logging scope.
                // Maybe we can support CTRL+C on every element?
                /*
                if (g.ActiveId == move_id)
                if (g.IO.KeyCtrl && IsKeyPressedMap(ImGuiKey_C))
                ImGui::LogToClipboard();
                */
            }

            // Inner clipping rectangle
            // We set this up after processing the resize grip so that our clip rectangle doesn't lag by a frame
            // Note that if our window is collapsed we will end up with a null clipping rectangle which is the correct behavior.
            ImRect title_bar_rect3 = window.TitleBarRect();
            float border_size = window.BorderSize;
            ImRect clip_rect;
            clip_rect.Min.x = title_bar_rect3.Min.x + 0.5f + Max(border_size, window.WindowPadding.x * 0.5f);
            clip_rect.Min.y = title_bar_rect3.Max.y + window.MenuBarHeight() + 0.5f + border_size;
            clip_rect.Max.x = window.Pos.x + window.Size.x - window.ScrollbarSizes.x - Max(border_size, window.WindowPadding.x * 0.5f);
            clip_rect.Max.y = window.Pos.y + window.Size.y - border_size - window.ScrollbarSizes.y;
            PushClipRect(clip_rect.Min, clip_rect.Max, true);

            // Clear 'accessed' flag last thing
            if (first_begin_of_the_frame)
                window.Accessed = false;
            window.BeginCount++;

            // Child window can be out of sight and have "negative" clip windows.
            // Mark them as collapsed so commands are skipped earlier (we can't manually collapse because they have no title bar).
            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) > 0)
            {
                System.Diagnostics.Debug.Assert((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) != 0);
                window.Collapsed = parent_window != null && parent_window.Collapsed;

                if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize) == 0 && window.AutoFitFramesX <= 0 && window.AutoFitFramesY <= 0)
                    window.Collapsed |= (window.ClippedWindowRect.Min.x >= window.ClippedWindowRect.Max.x || window.ClippedWindowRect.Min.y >= window.ClippedWindowRect.Max.y);

                // We also hide the window from rendering because we've already added its border to the command list.
                // (we could perform the check earlier in the function but it is simpler at this point)
                if (window.Collapsed)
                    window.Active = false;
            }
            if (style.Alpha <= 0.0f)
                window.Active = false;

            // Return false if we don't intend to display anything to allow user to perform an early out optimization
            window.SkipItems = (window.Collapsed || !window.Active) && window.AutoFitFramesX <= 0 && window.AutoFitFramesY <= 0;
            return !window.SkipItems;
        }

        public void End()
        {
            ImGuiState g = State;
            ImGuiWindow window = g.CurrentWindow;

            Columns(1, "#CloseColumns");
            PopClipRect();   // inner window clip rectangle

            // Stop logging
            //TODO: if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == 0)    // FIXME: add more options for scope of logging
            //    LogFinish();

            // Pop
            // NB: we don't clear 'window.RootWindow'. The pointer is allowed to live until the next call to Begin().
            g.CurrentWindowStack.pop_back();
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) > 0)
                g.CurrentPopupStack.pop_back();
            CheckStacksSize(window, false);
            SetCurrentWindow(g.CurrentWindowStack.empty() ? null : g.CurrentWindowStack.back());
        }

        ImGuiWindow GetFrontMostModalRootWindow()
        {
            ImGuiState g = State;
            if (!g.OpenedPopupStack.empty())
            {
                ImGuiWindow front_most_popup = g.OpenedPopupStack[g.OpenedPopupStack.Size - 1].Window;
                if (front_most_popup != null)
                {
                    if ((front_most_popup.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Modal) != 0)
                        return front_most_popup;
                }
            }
            return null;
        }

        //void SetActiveID(uint id, ImGuiWindow window = null)
        //{
        //    ImGuiState g = State;
        //    g.ActiveId = id;
        //    g.ActiveIdAllowOverlap = false;
        //    g.ActiveIdIsJustActivated = true;
        //    g.ActiveIdWindow = window;
        //}

        //void PushFont(ImFont font)
        //{
        //    ImGuiState g = State;
        //    if (font == null)
        //        font = g.IO.Fonts.Fonts[0];
        //    SetCurrentFont(font);
        //    g.FontStack.push_back(font);
        //    g.CurrentWindow.DrawList.PushTextureID(font.ContainerAtlas.TexID);
        //}

        //void PopFont()
        //{
        //    ImGuiState g = State;
        //    g.CurrentWindow.DrawList.PopTextureID();
        //    g.FontStack.pop_back();
        //    SetCurrentFont(g.FontStack.empty() ? g.IO.Fonts.Fonts[0] : g.FontStack[g.FontStack.Size - 1]);
        //}

        void CloseInactivePopups()
        {
            ImGuiState g = State;
            if (g.OpenedPopupStack.empty())
                return;

            // When popups are stacked, clicking on a lower level popups puts focus back to it and close popups above it.
            // Don't close our own child popup windows
            int n = 0;
            if (g.FocusedWindow != null)
            {
                for (n = 0; n < g.OpenedPopupStack.Size; n++)
                {
                    ImGuiPopupRef popup = g.OpenedPopupStack[n];
                    if (popup.Window == null)
                        continue;
                    System.Diagnostics.Debug.Assert((popup.Window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0);
                    if ((popup.Window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow)
                        continue;

                    bool has_focus = false;
                    for (int m = n; m < g.OpenedPopupStack.Size && !has_focus; m++)
                        has_focus = (g.OpenedPopupStack[m].Window != null && g.OpenedPopupStack[m].Window.RootWindow == g.FocusedWindow.RootWindow);
                    if (!has_focus)
                        break;
                }
            }
            if (n < g.OpenedPopupStack.Size)   // This test is not required but it allows to set a useful breakpoint on the line below
                g.OpenedPopupStack.resize(n);
        }

        //internal void KeepAliveID(uint id)
        //{
        //    ImGuiState g = State;
        //    if (g.ActiveId == id)
        //        g.ActiveIdIsAlive = true;
        //}

        bool ButtonBehavior(ImRect bb, uint id, ref bool? out_hovered, ref bool? out_held, ImGuiButtonFlags flags = 0)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            if ((flags & ImGuiButtonFlags.ImGuiButtonFlags_Disabled) > 0)
            {
                if (out_hovered.HasValue) out_hovered = false;
                if (out_held.HasValue) out_held = false;
                if (g.ActiveId == id) SetActiveID(0);
                return false;
            }

            bool pressed = false;
            bool hovered = IsHovered(bb, id, (flags & ImGuiButtonFlags.ImGuiButtonFlags_FlattenChilds) != 0);
            if (hovered)
            {
                SetHoveredID(id);
                if ((flags & ImGuiButtonFlags.ImGuiButtonFlags_NoKeyModifiers) == 0 || (!g.IO.KeyCtrl && !g.IO.KeyShift && !g.IO.KeyAlt))
                {
                    if (g.IO.MouseDoubleClicked[0] && (flags & ImGuiButtonFlags.ImGuiButtonFlags_PressedOnDoubleClick) > 0)
                    {
                        pressed = true;
                    }
                    else if (g.IO.MouseClicked[0])
                    {
                        if ((flags & ImGuiButtonFlags.ImGuiButtonFlags_PressedOnClick) > 0)
                        {
                            pressed = true;
                            SetActiveID(0);
                        }
                        else
                        {
                            SetActiveID(id, window);
                        }
                        FocusWindow(window);
                    }
                    else if (g.IO.MouseReleased[0] && (flags & ImGuiButtonFlags.ImGuiButtonFlags_PressedOnRelease) > 0)
                    {
                        pressed = true;
                        SetActiveID(0);
                    }
                    else if ((flags & ImGuiButtonFlags.ImGuiButtonFlags_Repeat) > 0 && g.ActiveId == id && IsMouseClicked(0, true))
                    {
                        pressed = true;
                    }
                }
            }

            bool held = false;
            if (g.ActiveId == id)
            {
                if (g.IO.MouseDown[0])
                {
                    held = true;
                }
                else
                {
                    if (hovered)
                        pressed = true;
                    SetActiveID(0);
                }
            }

            if (out_hovered.HasValue) out_hovered = hovered;
            if (out_held.HasValue) out_held = held;

            return pressed;
        }

        // Calculate text size. Text can be multi-line. Optionally ignore text after a ## marker.
        // CalcTextSize("") should return ImVec2(0.0f, GImGui->FontSize)
        ImVec2 CalcTextSize(string str, int text, int text_end = -1, bool hide_text_after_double_hash = false, float wrap_width = -1f)
        {
            ImGuiState g = State;

            //TODO: Check is this is correct
            if (text_end == -1)
                text_end = str.Length;

            int text_display_end;
            if (hide_text_after_double_hash)
                text_display_end = FindRenderedTextEnd(str, text, text_end);      // Hide anything after a '##' string
            else
                text_display_end = text_end;

            ImFont font = g.Font;
            float font_size = g.FontSize;
            if (text == text_display_end)
                return new ImVec2(0.0f, font_size);
            ImVec2 text_size;
            font.CalcTextSizeA(out text_size, font_size, float.MaxValue, wrap_width, str, text, text_display_end);

            // Cancel out character spacing for the last character of a line (it is baked into glyph->XAdvance field)
            float font_scale = font_size / font.FontSize;
            float character_spacing_x = 1.0f * font_scale;
            if (text_size.x > 0.0f)
                text_size.x -= character_spacing_x;
            text_size.x = (int)(text_size.x + 0.95f);

            return text_size;
        }
        // Save and compare stack sizes on Begin()/End() to detect usage errors
        void CheckStacksSize(ImGuiWindow window, bool write)
        {
            // NOT checking: DC.ItemWidth, DC.AllowKeyboardFocus, DC.ButtonRepeat, DC.TextWrapPos (per window) to allow user to conveniently push once and not pop (they are cleared on Begin)
            ImGuiState g = State;
            int p_backup = 0;
            {
                // PopID()
                int current = window.IDStack.Size;
                if (write)
                    window.DC.StackSizesBackup[p_backup] = current;
                else
                    System.Diagnostics.Debug.Assert(window.DC.StackSizesBackup[p_backup] == current);
                p_backup++;
            }
            {
                // EndGroup
                int current = window.DC.GroupStack.Size;
                if (write)
                    window.DC.StackSizesBackup[p_backup] = current;
                else
                    System.Diagnostics.Debug.Assert(window.DC.StackSizesBackup[p_backup] == current);
                p_backup++;
            }
            {
                // EndPopup() / EndMenu()
                int current = g.CurrentPopupStack.Size;
                if (write)
                    window.DC.StackSizesBackup[p_backup] = current;
                else
                    System.Diagnostics.Debug.Assert(window.DC.StackSizesBackup[p_backup] == current);
                p_backup++;
            }
            {
                // PopStyleColor()
                int current = g.ColorModifiers.Size;
                if (write)
                    window.DC.StackSizesBackup[p_backup] = current;
                else
                    System.Diagnostics.Debug.Assert(window.DC.StackSizesBackup[p_backup] == current);
                p_backup++;
            }
            {
                // PopStyleVar()
                int current = g.StyleModifiers.Size;
                if (write)
                    window.DC.StackSizesBackup[p_backup] = current;
                else
                    System.Diagnostics.Debug.Assert(window.DC.StackSizesBackup[p_backup] == current);
                p_backup++;
            }
            {
                // PopFont()
                int current = g.FontStack.Size;
                if (write)
                    window.DC.StackSizesBackup[p_backup] = current;
                else
                    System.Diagnostics.Debug.Assert(window.DC.StackSizesBackup[p_backup] == current);
                p_backup++;
            }

            System.Diagnostics.Debug.Assert(p_backup == window.DC.StackSizesBackup.Length);
        }

        // Upper-right button to close a window.
        bool CloseWindowButton(ref bool p_opened)
        {
            ImGuiWindow window = GetCurrentWindow();

            uint id = window.GetID("#CLOSE");
            float size = window.TitleBarHeight() - 4.0f;
            ImRect bb = new ImRect(window.Rect().GetTR() + new ImVec2(-2.0f - size, 2.0f), window.Rect().GetTR() + new ImVec2(-2.0f, 2.0f + size));

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(bb, id, ref hovered, ref held);

            // Render
            uint col = GetColorU32((held.Value && hovered.Value) ? ImGuiCol.ImGuiCol_CloseButtonActive : hovered.Value ? ImGuiCol.ImGuiCol_CloseButtonHovered : ImGuiCol.ImGuiCol_CloseButton);
            ImVec2 center = bb.GetCenter();
            window.DrawList.AddCircleFilled(center, Max(2.0f, size * 0.5f), col, 16);

            float cross_extent = (size * 0.5f * 0.7071f) - 1.0f;
            if (hovered.Value)
            {
                window.DrawList.AddLine(center + new ImVec2(+cross_extent, +cross_extent), center + new ImVec2(-cross_extent, -cross_extent), GetColorU32(ImGuiCol.ImGuiCol_Text));
                window.DrawList.AddLine(center + new ImVec2(+cross_extent, -cross_extent), center + new ImVec2(-cross_extent, +cross_extent), GetColorU32(ImGuiCol.ImGuiCol_Text));
            }

            if (p_opened && pressed)
                p_opened = false;

            return pressed;
        }

        void Columns(int columns_count = 1, string id = null, bool border = true)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();
            System.Diagnostics.Debug.Assert(columns_count >= 1);

            if (window.DC.ColumnsCount != 1)
            {
                if (window.DC.ColumnsCurrent != 0)
                    ItemSize(new ImVec2(0, 0));   // Advance to column 0
                PopItemWidth();
                PopClipRect();
                window.DrawList.ChannelsMerge();

                window.DC.ColumnsCellMaxY = Max(window.DC.ColumnsCellMaxY, window.DC.CursorPos.y);
                window.DC.CursorPos.y = window.DC.ColumnsCellMaxY;
            }

            // Draw columns borders and handle resize at the time of "closing" a columns set
            if (window.DC.ColumnsCount != columns_count && window.DC.ColumnsCount != 1 && window.DC.ColumnsShowBorders && !window.SkipItems)
            {
                float y1 = window.DC.ColumnsStartPosY;
                float y2 = window.DC.CursorPos.y;
                for (int i = 1; i < window.DC.ColumnsCount; i++)
                {
                    float x = window.Pos.x + GetColumnOffset(i);
                    uint column_id = window.DC.ColumnsSetID + (uint)i;
                    ImRect column_rect = new ImRect(new ImVec2(x - 4, y1), new ImVec2(x + 4, y2));
                    if (IsClippedEx(column_rect, column_id, false))
                        continue;

                    bool? hovered = false, held = false;

                    //TODO: Check last true param ButtonBehavior(column_rect, column_id, ref hovered, ref held, true);
                    ButtonBehavior(column_rect, column_id, ref hovered, ref held, (ImGuiButtonFlags)1);
                    if ((hovered.HasValue && hovered.Value) || (held.HasValue && held.Value))
                        g.MouseCursor = ImGuiMouseCursor.ImGuiMouseCursor_ResizeEW;

                    // Draw before resize so our items positioning are in sync with the line being drawn
                    uint col = GetColorU32((held.HasValue && held.Value) ? ImGuiCol.ImGuiCol_ColumnActive : (hovered.HasValue && hovered.Value) ? ImGuiCol.ImGuiCol_ColumnHovered : ImGuiCol.ImGuiCol_Column);
                    float xi = (int)x;
                    window.DrawList.AddLine(new ImVec2(xi, y1 + 1.0f), new ImVec2(xi, y2), col);

                    if ((held.HasValue && held.Value))
                    {
                        if (g.ActiveIdIsJustActivated)
                            g.ActiveClickDeltaToCenter.x = x - g.IO.MousePos.x;

                        x = GetDraggedColumnOffset(i);
                        SetColumnOffset(i, x);
                    }
                }
            }

            // Differentiate column ID with an arbitrary prefix for cases where users name their columns set the same as another widget. 
            // In addition, when an identifier isn't explicitly provided we include the number of columns in the hash to make it uniquer.
            PushID(0x11223347 + (id != null ? 0 : columns_count));
            window.DC.ColumnsSetID = window.GetID(id != null ? id : "columns");
            PopID();

            // Set state for first column
            window.DC.ColumnsCurrent = 0;
            window.DC.ColumnsCount = columns_count;
            window.DC.ColumnsShowBorders = border;

            float content_region_width = window.SizeContentsExplicit.x != 0 ? window.SizeContentsExplicit.x : window.Size.x;
            window.DC.ColumnsMinX = window.DC.IndentX; // Lock our horizontal range
            window.DC.ColumnsMaxX = content_region_width - window.Scroll.x - ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar) != 0 ? 0 : g.Style.ScrollbarSize);// - window.WindowPadding().x;
            window.DC.ColumnsStartPosY = window.DC.CursorPos.y;
            window.DC.ColumnsCellMinY = window.DC.ColumnsCellMaxY = window.DC.CursorPos.y;
            window.DC.ColumnsOffsetX = 0.0f;
            window.DC.CursorPos.x = (float)(int)(window.Pos.x + window.DC.IndentX + window.DC.ColumnsOffsetX);

            if (window.DC.ColumnsCount != 1)
            {
                // Cache column offsets
                window.DC.ColumnsData.resize(columns_count + 1);
                for (int column_index = 0; column_index < columns_count + 1; column_index++)
                {
                    uint column_id = window.DC.ColumnsSetID + (uint)column_index;
                    KeepAliveID(column_id);
                    float default_t = column_index / (float)window.DC.ColumnsCount;
                    float t = window.DC.StateStorage.GetFloat(column_id, default_t);      // Cheaply store our floating point value inside the integer (could store an union into the map?)
                    var colData = window.DC.ColumnsData[column_index];
                    colData.OffsetNorm = t;
                    window.DC.ColumnsData[column_index] = colData;
                }
                window.DrawList.ChannelsSplit(window.DC.ColumnsCount);
                PushColumnClipRect();
                PushItemWidth(GetColumnWidth() * 0.65f);
            }
            else
            {
                window.DC.ColumnsData.resize(0);
            }
        }

        ImGuiWindow CreateNewWindow(string name, ImVec2 size, ImGuiWindowFlags flags)
        {
            ImGuiState g = State;

            // Create window the first time
            //ImGuiWindow* window = (ImGuiWindow*)ImGui::MemAlloc(sizeof(ImGuiWindow));
            var window = new ImGuiWindow(name);
            //IM_PLACEMENT_NEW(window) ImGuiWindow(name);
            window.Flags = flags;

            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings) != 0)
            {
                // User can disable loading and saving of settings. Tooltip and child windows also don't store settings.
                window.Size = window.SizeFull = size;
            }
            else
            {
                // Retrieve settings from .ini file
                // Use SetWindowPos() or SetNextWindowPos() with the appropriate condition flag to change the initial position of a window.
                window.PosFloat = new ImVec2(60, 60);
                window.Pos = new ImVec2((float)(int)window.PosFloat.x, (float)(int)window.PosFloat.y);

                ImGuiIniData settings = FindWindowSettings(name);
                if (settings == null)
                {
                    settings = AddWindowSettings(name);
                }
                else
                {
                    window.SetWindowPosAllowFlags &= ~(int)ImGuiSetCond.ImGuiSetCond_FirstUseEver;
                    window.SetWindowSizeAllowFlags &= ~(int)ImGuiSetCond.ImGuiSetCond_FirstUseEver;
                    window.SetWindowCollapsedAllowFlags &= ~(int)ImGuiSetCond.ImGuiSetCond_FirstUseEver;
                }

                if (settings.Pos.x != float.MaxValue)
                {
                    window.PosFloat = settings.Pos;
                    window.Pos = new ImVec2((int)window.PosFloat.x, (int)window.PosFloat.y);
                    window.Collapsed = settings.Collapsed;
                }

                if (LengthSqr(settings.Size) > 0.00001f && (flags & ImGuiWindowFlags.ImGuiWindowFlags_NoResize) == 0)
                    size = settings.Size;
                window.Size = window.SizeFull = size;
            }

            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize) != 0)
            {
                window.AutoFitFramesX = window.AutoFitFramesY = 2;
                window.AutoFitOnlyGrows = false;
            }
            else
            {
                if (window.Size.x <= 0.0f)
                    window.AutoFitFramesX = 2;
                if (window.Size.y <= 0.0f)
                    window.AutoFitFramesY = 2;
                window.AutoFitOnlyGrows = (window.AutoFitFramesX > 0) || (window.AutoFitFramesY > 0);
            }

            if ((flags & ImGuiWindowFlags.ImGuiWindowFlags_NoBringToFrontOnFocus) != 0)
                g.Windows.insert(0, window); // Quite slow but rare and only once
            else
                g.Windows.push_back(window);
            return window;
        }

        ImGuiIniData AddWindowSettings(string name)
        {
            ImGuiState g = State;
            g.Settings.push_back(new ImGuiIniData());
            ImGuiIniData ini = g.Settings.back();
            ini.Name = name;
            ini.ID = Hash(0, name);
            ini.Collapsed = false;
            ini.Pos = new ImVec2(float.MaxValue, float.MaxValue);
            ini.Size = new ImVec2(0, 0);
            return ini;
        }

        ImVec2 FindBestPopupWindowPos(ImVec2 base_pos, ImVec2 size, ref int last_dir, ImRect r_inner)
        {
            ImGuiStyle style = State.Style;

            // Clamp into visible area while not overlapping the cursor. Safety padding is optional if our popup size won't fit without it.
            ImVec2 safe_padding = style.DisplaySafeAreaPadding;
            ImRect r_outer = GetVisibleRect();
            r_outer.Reduce(new ImVec2((size.x - r_outer.GetWidth() > safe_padding.x * 2) ? safe_padding.x : 0.0f, (size.y - r_outer.GetHeight() > safe_padding.y * 2) ? safe_padding.y : 0.0f));
            ImVec2 base_pos_clamped = Clamp(base_pos, r_outer.Min, r_outer.Max - size);

            for (int n = (last_dir != -1) ? -1 : 0; n < 4; n++)   // Last, Right, down, up, left. (Favor last used direction).
            {
                int dir = (n == -1) ? last_dir : n;
                ImRect rect = new ImRect(dir == 0 ? r_inner.Max.x : r_outer.Min.x, dir == 1 ? r_inner.Max.y : r_outer.Min.y, dir == 3 ? r_inner.Min.x : r_outer.Max.x, dir == 2 ? r_inner.Min.y : r_outer.Max.y);
                if (rect.GetWidth() < size.x || rect.GetHeight() < size.y)
                    continue;
                last_dir = dir;
                return new ImVec2(dir == 0 ? r_inner.Max.x : dir == 3 ? r_inner.Min.x - size.x : base_pos_clamped.x, dir == 1 ? r_inner.Max.y : dir == 2 ? r_inner.Min.y - size.y : base_pos_clamped.y);
            }

            // Fallback, try to keep within display
            last_dir = -1;
            ImVec2 pos = base_pos;
            pos.x = Max(Min(pos.x + size.x, r_outer.Max.x) - size.x, r_outer.Min.x);
            pos.y = Max(Min(pos.y + size.y, r_outer.Max.y) - size.y, r_outer.Min.y);
            return pos;
        }

        int FindRenderedTextEnd(string data, int text = 0, int text_end = -1)
        {
            int text_display_end = text;
            if (text_end == -1)
                text_end = data.Length;

            while (text_display_end < text_end)
            {
                if (data[text_display_end] == '#' || (text_display_end < text_end - 2 && data[text_display_end + 1] == '#'))
                    break;
                text_display_end++;
            }
            return text_display_end;
        }

        ImGuiWindow FindWindowByName(string name)
        {
            // FIXME-OPT: Store sorted hashes -> pointers so we can do a bissection in a contiguous block
            ImGuiState g = State; ;
            uint id = Hash(0, name);
            for (int i = 0; i < g.Windows.Size; i++)
                if (g.Windows[i].ID == id)
                    return g.Windows[i];
            return null;
        }

        ImGuiIniData FindWindowSettings(string name)
        {
            ImGuiState g = State;
            uint id = Hash(0, name);
            for (int i = 0; i != g.Settings.Size; i++)
            {
                ImGuiIniData ini = g.Settings[i];
                if (ini.ID == id)
                    return ini;
            }
            return null;
        }

        internal uint GetColorU32(ImGuiCol idx, float alpha_mul = 1f)
        {
            var style = State.Style;
            ImVec4 c = style.Colors[(int)idx];
            c.w *= style.Alpha * alpha_mul;
            return ColorConvertFloat4ToU32(c);
        }

        internal uint GetColorU32(ImVec4 col)
        {
            var style = State.Style;
            ImVec4 c = col;
            c.w *= style.Alpha;
            return ColorConvertFloat4ToU32(c);
        }

        void MarkSettingsDirty()
        {
            ImGuiState g = State;
            if (g.SettingsDirtyTimer <= 0.0f)
                g.SettingsDirtyTimer = g.IO.IniSavingRate;
        }

        ImGuiWindow GetCurrentWindowRead()
        {
            return State.CurrentWindow;
        }
        ImGuiWindow GetCurrentWindow()
        {
            ImGuiState g = State;
            g.CurrentWindow.Accessed = true;
            return g.CurrentWindow;
        }

        // Helper to calculate coarse clipping of large list of evenly sized items.
        // NB: Prefer using the ImGuiListClipper higher-level helper if you can!
        // NB: 'items_count' is only used to clamp the result, if you don't know your count you can use INT_MAX
        // If you are displaying thousands of items and you have a random access to the list, you can perform clipping yourself to save on CPU.
        // {
        //    float item_height = GetTextLineHeightWithSpacing();
        //    int display_start, display_end;
        //    CalcListClipping(count, item_height, &display_start, &display_end);            // calculate how many to clip/display
        //    SetCursorPosY(GetCursorPosY() + (display_start) * item_height);         // advance cursor
        //    for (int i = display_start; i < display_end; i++)                                     // display only visible items
        //        // TODO: display visible item
        //    SetCursorPosY(GetCursorPosY() + (count - display_end) * item_height);   // advance cursor
        // }
        internal void CalcListClipping(int items_count, float items_height, ref int out_items_display_start, ref int out_items_display_end)
        {
            ImGuiState g = State; ;
            ImGuiWindow window = GetCurrentWindowRead();
            if (g.LogEnabled)
            {
                // If logging is active, do not perform any clipping
                out_items_display_start = 0;
                out_items_display_end = items_count;
                return;
            }

            ImVec2 pos = window.DC.CursorPos;
            int start = (int)((window.ClipRect.Min.y - pos.y) / items_height);
            int end = (int)((window.ClipRect.Max.y - pos.y) / items_height);
            start = Clamp(start, 0, items_count);
            end = Clamp(end + 1, start, items_count);
            out_items_display_start = start;
            out_items_display_end = end;
        }

        // Find window given position, search front-to-back
        ImGuiWindow FindHoveredWindow(ImVec2 pos, bool excluding_childs)
        {
            ImGuiState g = State; ;
            for (int i = g.Windows.Size - 1; i >= 0; i--)
            {
                ImGuiWindow window = g.Windows[i];
                if (!window.Active)
                    continue;
                if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoInputs) != 0)
                    continue;
                if (excluding_childs && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0)
                    continue;

                // Using the clipped AABB so a child window will typically be clipped by its parent.
                ImRect bb = new ImRect(window.ClippedWindowRect.Min - g.Style.TouchExtraPadding, window.ClippedWindowRect.Max + g.Style.TouchExtraPadding);
                if (bb.Contains(pos))
                    return window;
            }
            return null;
        }

        // Test if mouse cursor is hovering given rectangle
        // NB- Rectangle is clipped by our current clip setting
        // NB- Expand the rectangle to be generous on imprecise inputs systems (g.Style.TouchExtraPadding)
        bool IsMouseHoveringRect(ImVec2 r_min, ImVec2 r_max, bool clip = true)
        {
            ImGuiState g = State; ;
            ImGuiWindow window = GetCurrentWindowRead();

            // Clip
            ImRect rect_clipped = new ImRect(r_min, r_max);
            if (clip)
                rect_clipped.Clip(window.ClipRect);

            // Expand for touch input
            ImRect rect_for_touch = new ImRect(rect_clipped.Min - g.Style.TouchExtraPadding, rect_clipped.Max + g.Style.TouchExtraPadding);
            return rect_for_touch.Contains(g.IO.MousePos);
        }

        bool IsMouseHoveringWindow()
        {
            ImGuiState g = State; ;
            return g.HoveredWindow == g.CurrentWindow;
        }

        bool IsMouseHoveringAnyWindow()
        {
            ImGuiState g = State; ;
            return g.HoveredWindow != null;
        }

        bool IsPosHoveringAnyWindow(ImVec2 pos)
        {
            return FindHoveredWindow(pos, false) != null;
        }

        bool IsKeyPressedMap(ImGuiKey key, bool repeat = true)
        {
            int key_index = State.IO.KeyMap[(int)key];
            return IsKeyPressed(key_index, repeat);
        }

        int GetKeyIndex(ImGuiKey key)
        {
            System.Diagnostics.Debug.Assert(key >= 0 && key < ImGuiKey.ImGuiKey_COUNT);
            return State.IO.KeyMap[(int)key];
        }

        bool IsKeyDown(int key_index)
        {
            if (key_index < 0) return false;
            System.Diagnostics.Debug.Assert(key_index >= 0 && key_index < State.IO.KeysDown.Length);
            return State.IO.KeysDown[key_index];
        }

        bool IsKeyPressed(int key_index, bool repeat = true)
        {
            ImGuiState g = State; ;
            if (key_index < 0) return false;
            System.Diagnostics.Debug.Assert(key_index >= 0 && key_index < g.IO.KeysDown.Length);
            float t = g.IO.KeysDownDuration[key_index];
            if (t == 0.0f)
                return true;

            if (repeat && t > g.IO.KeyRepeatDelay)
            {
                float delay = g.IO.KeyRepeatDelay, rate = g.IO.KeyRepeatRate;
                if ((fmodf(t - delay, rate) > rate * 0.5f) != (fmodf(t - delay - g.IO.DeltaTime, rate) > rate * 0.5f))
                    return true;
            }
            return false;
        }

        bool IsKeyReleased(int key_index)
        {
            ImGuiState g = State; ;
            if (key_index < 0) return false;
            System.Diagnostics.Debug.Assert(key_index >= 0 && key_index < g.IO.KeysDown.Length);
            if (g.IO.KeysDownDurationPrev[key_index] >= 0.0f && !g.IO.KeysDown[key_index])
                return true;
            return false;
        }

        bool IsMouseDown(int button)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            return g.IO.MouseDown[button];
        }

        bool IsMouseClicked(int button, bool repeat = false)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            float t = g.IO.MouseDownDuration[button];
            if (t == 0.0f)
                return true;

            if (repeat && t > g.IO.KeyRepeatDelay)
            {
                float delay = g.IO.KeyRepeatDelay, rate = g.IO.KeyRepeatRate;
                if ((fmodf(t - delay, rate) > rate * 0.5f) != (fmodf(t - delay - g.IO.DeltaTime, rate) > rate * 0.5f))
                    return true;
            }

            return false;
        }

        bool IsMouseReleased(int button)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            return g.IO.MouseReleased[button];
        }

        bool IsMouseDoubleClicked(int button)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            return g.IO.MouseDoubleClicked[button];
        }

        bool IsMouseDragging(int button = 0, float lock_threshold = -1f)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            if (!g.IO.MouseDown[button])
                return false;
            if (lock_threshold < 0.0f)
                lock_threshold = g.IO.MouseDragThreshold;
            return g.IO.MouseDragMaxDistanceSqr[button] >= lock_threshold * lock_threshold;
        }

        ImVec2 GetMousePos()
        {
            return State.IO.MousePos;
        }

        // NB: prefer to call right after BeginPopup(). At the time Selectable/MenuItem is activated, the popup is already closed!
        ImVec2 GetMousePosOnOpeningCurrentPopup()
        {
            ImGuiState g = State; ;
            if (g.CurrentPopupStack.Size > 0)
                return g.OpenedPopupStack[g.CurrentPopupStack.Size - 1].MousePosOnOpen;
            return g.IO.MousePos;
        }

        ImVec2 GetMouseDragDelta(int button = 0, float lock_threshold = -1f)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            if (lock_threshold < 0.0f)
                lock_threshold = g.IO.MouseDragThreshold;
            if (g.IO.MouseDown[button])
                if (g.IO.MouseDragMaxDistanceSqr[button] >= lock_threshold * lock_threshold)
                    return g.IO.MousePos - g.IO.MouseClickedPos[button];     // Assume we can only get active with left-mouse button (at the moment).
            return new ImVec2(0.0f, 0.0f);
        }

        void ResetMouseDragDelta(int button = 0)
        {
            ImGuiState g = State; ;
            System.Diagnostics.Debug.Assert(button >= 0 && button < g.IO.MouseDown.Length);
            // NB: We don't need to reset g.IO.MouseDragMaxDistanceSqr
            g.IO.MouseClickedPos[button] = g.IO.MousePos;
        }

        ImGuiMouseCursor GetMouseCursor()
        {
            return State.MouseCursor;
        }

        void SetMouseCursor(ImGuiMouseCursor cursor_type)
        {
            State.MouseCursor = cursor_type;
        }

        void CaptureKeyboardFromApp(bool capture = true)
        {
            State.CaptureKeyboardNextFrame = capture ? 1 : 0;
        }

        void CaptureMouseFromApp(bool capture = true)
        {
            State.CaptureMouseNextFrame = capture ? 1 : 0;
        }

        bool IsItemHovered()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.LastItemHoveredAndUsable;
        }

        bool IsItemHoveredRect()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.LastItemHoveredRect;
        }

        bool IsItemActive()
        {
            ImGuiState g = State; ;
            if (g.ActiveId != 0)
            {
                ImGuiWindow window = GetCurrentWindowRead();
                return g.ActiveId == window.DC.LastItemID;
            }
            return false;
        }

        bool IsAnyItemHovered()
        {
            return State.HoveredId != 0 || State.HoveredIdPreviousFrame != 0;
        }

        bool IsAnyItemActive()
        {
            return State.ActiveId != 0;
        }

        bool IsItemVisible()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            ImRect r = window.ClipRect;
            return r.Overlaps(window.DC.LastItemRect);
        }

        // Allow last item to be overlapped by a subsequent item. Both may be activated during the same frame before the later one takes priority.
        void SetItemAllowOverlap()
        {
            ImGuiState g = State; ;
            if (g.HoveredId == g.CurrentWindow.DC.LastItemID)
                g.HoveredIdAllowOverlap = true;
            if (g.ActiveId == g.CurrentWindow.DC.LastItemID)
                g.ActiveIdAllowOverlap = true;
        }

        ImVec2 GetItemRectMin()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.LastItemRect.Min;
        }

        ImVec2 GetItemRectMax()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.LastItemRect.Max;
        }

        ImVec2 GetItemRectSize()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.LastItemRect.GetSize();
        }

        ImVec2 CalcItemRectClosestPoint(ImVec2 pos, bool on_edge, float outward)
        {
            ImGuiWindow window = GetCurrentWindowRead();
            ImRect rect = window.DC.LastItemRect;
            rect.Expand(outward);
            return rect.GetClosestPoint(pos, on_edge);
        }

        void ColorEditMode(ImGuiColorEditMode mode)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.ColorEditMode = mode;
        }

        // Horizontal separating line.
        void Separator()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            if (window.DC.ColumnsCount > 1)
                PopClipRect();

            float x1 = window.Pos.x;
            float x2 = window.Pos.x + window.Size.x;
            if (!window.DC.GroupStack.empty())
                x1 += window.DC.IndentX;

            ImRect bb = new ImRect(new ImVec2(x1, window.DC.CursorPos.y), new ImVec2(x2, window.DC.CursorPos.y));
            ItemSize(new ImVec2(0.0f, 0.0f)); // NB: we don't provide our width so that it doesn't get feed back into AutoFit   // FIXME: Height should be 1.0f not 0.0f ?
            if (!ItemAdd(bb, null))
            {
                if (window.DC.ColumnsCount > 1)
                    PushColumnClipRect();
                return;
            }

            window.DrawList.AddLine(bb.Min, bb.Max, GetColorU32(ImGuiCol.ImGuiCol_Border));

            ImGuiState g = State;
            //TODO: if (g.LogEnabled)
            //TODO:     LogText(IM_NEWLINE "--------------------------------");

            if (window.DC.ColumnsCount > 1)
            {
                PushColumnClipRect();
                window.DC.ColumnsCellMinY = window.DC.CursorPos.y;
            }
        }

        void Spacing()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;
            ItemSize(new ImVec2(0, 0));
        }

        void Dummy(ImVec2 size)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + size);
            ItemSize(bb);
            ItemAdd(bb, null);
        }

        bool IsRectVisible(ImVec2 size)
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.ClipRect.Overlaps(new ImRect(window.DC.CursorPos, window.DC.CursorPos + size));
        }

        void BeginGroup()
        {
            ImGuiWindow window = GetCurrentWindow();

            window.DC.GroupStack.resize(window.DC.GroupStack.Size + 1);
            ImGuiGroupData group_data = window.DC.GroupStack.back();
            group_data.BackupCursorPos = window.DC.CursorPos;
            group_data.BackupCursorMaxPos = window.DC.CursorMaxPos;
            group_data.BackupIndentX = window.DC.IndentX;
            group_data.BackupCurrentLineHeight = window.DC.CurrentLineHeight;
            group_data.BackupCurrentLineTextBaseOffset = window.DC.CurrentLineTextBaseOffset;
            group_data.BackupLogLinePosY = window.DC.LogLinePosY;
            group_data.AdvanceCursor = true;

            window.DC.GroupStack.back(group_data);
            window.DC.IndentX = window.DC.CursorPos.x - window.Pos.x;
            window.DC.CursorMaxPos = window.DC.CursorPos;
            window.DC.CurrentLineHeight = 0.0f;
            window.DC.LogLinePosY = window.DC.CursorPos.y - 9999.0f;
        }

        void EndGroup()
        {
            ImGuiWindow window = GetCurrentWindow();
            ImGuiStyle style = GetStyle();

            System.Diagnostics.Debug.Assert(!window.DC.GroupStack.empty());  // Mismatched BeginGroup()/EndGroup() calls

            ImGuiGroupData group_data = window.DC.GroupStack.back();

            ImRect group_bb = new ImRect(group_data.BackupCursorPos, window.DC.CursorMaxPos);
            group_bb.Max.y -= style.ItemSpacing.y;      // Cancel out last vertical spacing because we are adding one ourselves.
            group_bb.Max = Max(group_bb.Min, group_bb.Max);

            window.DC.CursorPos = group_data.BackupCursorPos;
            window.DC.CursorMaxPos = Max(group_data.BackupCursorMaxPos, window.DC.CursorMaxPos);
            window.DC.CurrentLineHeight = group_data.BackupCurrentLineHeight;
            window.DC.CurrentLineTextBaseOffset = group_data.BackupCurrentLineTextBaseOffset;
            window.DC.IndentX = group_data.BackupIndentX;
            window.DC.LogLinePosY = window.DC.CursorPos.y - 9999.0f;

            if (group_data.AdvanceCursor)
            {
                window.DC.CurrentLineTextBaseOffset = Max(window.DC.PrevLineTextBaseOffset, group_data.BackupCurrentLineTextBaseOffset);      // FIXME: Incorrect, we should grab the base offset from the *first line* of the group but it is hard to obtain now.
                ItemSize(group_bb.GetSize(), group_data.BackupCurrentLineTextBaseOffset);
                ItemAdd(group_bb, null);
            }

            window.DC.GroupStack.pop_back();

            //window.DrawList.AddRect(group_bb.Min, group_bb.Max, 0xFFFF00FF);   // Debug
        }

        // Gets back to previous line and continue with horizontal layout
        //      pos_x == 0      : follow on previous item
        //      pos_x != 0      : align to specified column
        //      spacing_w < 0   : use default spacing if column_x==0, no spacing if column_x!=0
        //      spacing_w >= 0  : enforce spacing

        void SameLine(float pos_x = 0.0f, float spacing_w = -1.0f)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            if (pos_x != 0.0f)
            {
                if (spacing_w < 0.0f) spacing_w = 0.0f;
                window.DC.CursorPos.x = window.Pos.x - window.Scroll.x + pos_x + spacing_w;
                window.DC.CursorPos.y = window.DC.CursorPosPrevLine.y;
            }
            else
            {
                if (spacing_w < 0.0f) spacing_w = g.Style.ItemSpacing.x;
                window.DC.CursorPos.x = window.DC.CursorPosPrevLine.x + spacing_w;
                window.DC.CursorPos.y = window.DC.CursorPosPrevLine.y;
            }
            window.DC.CurrentLineHeight = window.DC.PrevLineHeight;
            window.DC.CurrentLineTextBaseOffset = window.DC.PrevLineTextBaseOffset;
        }

        void NextColumn()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            if (window.DC.ColumnsCount > 1)
            {
                PopItemWidth();
                PopClipRect();

                window.DC.ColumnsCellMaxY = Max(window.DC.ColumnsCellMaxY, window.DC.CursorPos.y);
                if (++window.DC.ColumnsCurrent < window.DC.ColumnsCount)
                {
                    // Columns 1+ cancel out IndentX
                    window.DC.ColumnsOffsetX = GetColumnOffset(window.DC.ColumnsCurrent) - window.DC.IndentX + g.Style.ItemSpacing.x;
                    window.DrawList.ChannelsSetCurrent(window.DC.ColumnsCurrent);
                }
                else
                {
                    window.DC.ColumnsCurrent = 0;
                    window.DC.ColumnsOffsetX = 0.0f;
                    window.DC.ColumnsCellMinY = window.DC.ColumnsCellMaxY;
                    window.DrawList.ChannelsSetCurrent(0);
                }
                window.DC.CursorPos.x = (float)(int)(window.Pos.x + window.DC.IndentX + window.DC.ColumnsOffsetX);
                window.DC.CursorPos.y = window.DC.ColumnsCellMinY;
                window.DC.CurrentLineHeight = 0.0f;
                window.DC.CurrentLineTextBaseOffset = 0.0f;

                PushColumnClipRect();
                PushItemWidth(GetColumnWidth() * 0.65f);  // FIXME: Move on columns setup
            }
        }

        int GetColumnIndex()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.ColumnsCurrent;
        }

        int GetColumnsCount()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.ColumnsCount;
        }

        float GetDraggedColumnOffset(int column_index)
        {
            // Active (dragged) column always follow mouse. The reason we need this is that dragging a column to the right edge of an auto-resizing
            // window creates a feedback loop because we store normalized positions. So while dragging we enforce absolute positioning.
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindowRead();
            System.Diagnostics.Debug.Assert(column_index > 0); // We cannot drag column 0. If you get this assert you may have a conflict between the ID of your columns and another widgets.
            System.Diagnostics.Debug.Assert(g.ActiveId == window.DC.ColumnsSetID + (uint)column_index);

            float x = g.IO.MousePos.x + g.ActiveClickDeltaToCenter.x - window.Pos.x;
            x = Clamp(x, GetColumnOffset(column_index - 1) + g.Style.ColumnsMinSpacing, GetColumnOffset(column_index + 1) - g.Style.ColumnsMinSpacing);

            return (float)(int)x;
        }

        float GetColumnOffset(int column_index)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindowRead();
            if (column_index < 0)
                column_index = window.DC.ColumnsCurrent;

            if (g.ActiveId != 0)
            {
                uint column_id = window.DC.ColumnsSetID + (uint)column_index;
                if (g.ActiveId == column_id)
                    return GetDraggedColumnOffset(column_index);
            }

            System.Diagnostics.Debug.Assert(column_index < window.DC.ColumnsData.Size);
            float t = window.DC.ColumnsData[column_index].OffsetNorm;
            float x_offset = window.DC.ColumnsMinX + t * (window.DC.ColumnsMaxX - window.DC.ColumnsMinX);
            return (float)(int)x_offset;
        }

        void SetColumnOffset(int column_index, float offset)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (column_index < 0)
                column_index = window.DC.ColumnsCurrent;

            System.Diagnostics.Debug.Assert(column_index < window.DC.ColumnsData.Size);
            float t = (offset - window.DC.ColumnsMinX) / (window.DC.ColumnsMaxX - window.DC.ColumnsMinX);

            var column_data = window.DC.ColumnsData[column_index];
            column_data.OffsetNorm = t;
            window.DC.ColumnsData[column_index] = column_data;

            uint column_id = window.DC.ColumnsSetID + (uint)column_index;
            window.DC.StateStorage.SetFloat(column_id, t);
        }

        float GetColumnWidth(int column_index = -1)
        {
            ImGuiWindow window = GetCurrentWindowRead();
            if (column_index < 0)
                column_index = window.DC.ColumnsCurrent;

            float w = GetColumnOffset(column_index + 1) - GetColumnOffset(column_index);
            return w;
        }

        void PushColumnClipRect(int column_index = -1)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (column_index < 0)
                column_index = window.DC.ColumnsCurrent;

            float x1 = window.Pos.x + GetColumnOffset(column_index) - 1;
            float x2 = window.Pos.x + GetColumnOffset(column_index + 1) - 1;
            PushClipRect(new ImVec2(x1, -float.MaxValue), new ImVec2(x2, +float.MaxValue), true);
        }

        void PushID(string str_id)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.IDStack.push_back(window.GetID(str_id));
        }

        void PushID(string text, int str_id_begin, int str_id_end)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.IDStack.push_back(window.GetID(text, str_id_begin, str_id_end));
        }

        //void PushID(const void* ptr_id)
        //{
        //    ImGuiWindow window = GetCurrentWindow();
        //    window.IDStack.push_back(window.GetID(ptr_id));
        //}

        void PushID(int int_id)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.IDStack.push_back(window.GetID(int_id.ToString()));
        }

        void PopID()
        {
            ImGuiWindow window = GetCurrentWindow();
            window.IDStack.pop_back();
        }

        uint GetID(string str_id)
        {
            return State.CurrentWindow.GetID(str_id);
        }

        uint GetID(string text, int str_id_begin, int str_id_end)
        {
            return State.CurrentWindow.GetID(text, str_id_begin, str_id_end);
        }

        //uint GetID(const void* ptr_id)
        //{
        //    return GImGui->CurrentWindow->GetID(ptr_id);
        //}











        // Moving window to front of display (which happens to be back of our sorted list)
        void FocusWindow(ImGuiWindow window)
        {
            ImGuiState g = State;

            // Always mark the window we passed as focused. This is used for keyboard interactions such as tabbing.
            g.FocusedWindow = window;

            // Passing NULL allow to disable keyboard focus
            if (window == null)
                return;

            // And move its root window to the top of the pile
            if (window.RootWindow != null)
                window = window.RootWindow;

            // Steal focus on active widgets
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0) // FIXME: This statement should be unnecessary. Need further testing before removing it..
                if (g.ActiveId != 0 && g.ActiveIdWindow != null && g.ActiveIdWindow.RootWindow != window)
                    SetActiveID(0);

            // Bring to front
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoBringToFrontOnFocus) != 0 || g.Windows.back() == window)
                return;
            for (int i = 0; i < g.Windows.Size; i++)
                if (g.Windows[i] == window)
                {
                    g.Windows.erase(i);
                    break;
                }
            g.Windows.push_back(window);
        }

        void PushItemWidth(float item_width)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.ItemWidth = (item_width == 0.0f ? window.ItemWidthDefault : item_width);
            window.DC.ItemWidthStack.push_back(window.DC.ItemWidth);
        }

        void PushMultiItemsWidths(int components, float w_full = 0f)
        {
            ImGuiWindow window = GetCurrentWindow();
            ImGuiStyle style = State.Style;
            if (w_full <= 0.0f)
                w_full = CalcItemWidth();
            float w_item_one = Max(1.0f, (float)(int)((w_full - (style.ItemInnerSpacing.x) * (components - 1)) / (float)components));
            float w_item_last = Max(1.0f, (float)(int)(w_full - (w_item_one + style.ItemInnerSpacing.x) * (components - 1)));
            window.DC.ItemWidthStack.push_back(w_item_last);
            for (int i = 0; i < components - 1; i++)
                window.DC.ItemWidthStack.push_back(w_item_one);
            window.DC.ItemWidth = window.DC.ItemWidthStack.back();
        }

        void PopItemWidth()
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.ItemWidthStack.pop_back();
            window.DC.ItemWidth = window.DC.ItemWidthStack.empty() ? window.ItemWidthDefault : window.DC.ItemWidthStack.back();
        }

        float CalcItemWidth()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            float w = window.DC.ItemWidth;
            if (w < 0.0f)
            {
                // Align to a right-side limit. We include 1 frame padding in the calculation because this is how the width is always used (we add 2 frame padding to it), but we could move that responsibility to the widget as well.
                float width_to_right_edge = GetContentRegionAvail().x;
                w = Max(1.0f, width_to_right_edge + w);
            }
            w = (float)(int)w;
            return w;
        }

        internal void SetCurrentFont(ImFont font)
        {
            ImGuiState g = State;
            System.Diagnostics.Debug.Assert(font != null && font.IsLoaded());    // Font Atlas not created. Did you call io.Fonts.GetTexDataAsRGBA32 / GetTexDataAsAlpha8 ?
            System.Diagnostics.Debug.Assert(font.Scale > 0.0f);
            g.Font = font;
            g.FontBaseSize = g.IO.FontGlobalScale * g.Font.FontSize * g.Font.Scale;
            g.FontSize = g.CurrentWindow != null ? g.CurrentWindow.CalcFontSize() : 0.0f;
            g.FontTexUvWhitePixel = g.Font.ContainerAtlas.TexUvWhitePixel;
        }

        void PushFont(ImFont font)
        {
            ImGuiState g = State;
            if (font == null)
                font = g.IO.Fonts.Fonts[0];
            SetCurrentFont(font);
            g.FontStack.push_back(font);
            g.CurrentWindow.DrawList.PushTextureID(font.ContainerAtlas.TexID);
        }

        void PopFont()
        {
            ImGuiState g = State;
            g.CurrentWindow.DrawList.PopTextureID();
            g.FontStack.pop_back();
            SetCurrentFont(g.FontStack.empty() ? g.IO.Fonts.Fonts[0] : g.FontStack.back());
        }

        void PushAllowKeyboardFocus(bool allow_keyboard_focus)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.AllowKeyboardFocus = allow_keyboard_focus;
            window.DC.AllowKeyboardFocusStack.push_back(allow_keyboard_focus);
        }

        void PopAllowKeyboardFocus()
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.AllowKeyboardFocusStack.pop_back();
            window.DC.AllowKeyboardFocus = window.DC.AllowKeyboardFocusStack.empty() ? true : window.DC.AllowKeyboardFocusStack.back();
        }

        void PushButtonRepeat(bool repeat)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.ButtonRepeat = repeat;
            window.DC.ButtonRepeatStack.push_back(repeat);
        }

        void PopButtonRepeat()
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.ButtonRepeatStack.pop_back();
            window.DC.ButtonRepeat = window.DC.ButtonRepeatStack.empty() ? false : window.DC.ButtonRepeatStack.back();
        }

        void PushTextWrapPos(float wrap_pos_x = 0f)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.TextWrapPos = wrap_pos_x;
            window.DC.TextWrapPosStack.push_back(wrap_pos_x);
        }

        void PopTextWrapPos()
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.TextWrapPosStack.pop_back();
            window.DC.TextWrapPos = window.DC.TextWrapPosStack.empty() ? -1.0f : window.DC.TextWrapPosStack.back();
        }

        void PushStyleColor(ImGuiCol idx, ImVec4 col)
        {
            ImGuiState g = State;
            ImGuiColMod backup;
            backup.Col = idx;
            backup.PreviousValue = g.Style.Colors[(int)idx];
            g.ColorModifiers.push_back(backup);
            g.Style.Colors[(int)idx] = col;
        }

        void PopStyleColor(int count = 1)
        {
            ImGuiState g = State;
            while (count > 0)
            {
                ImGuiColMod backup = g.ColorModifiers.back();
                g.Style.Colors[(int)backup.Col] = backup.PreviousValue;
                g.ColorModifiers.pop_back();
                count--;
            }
        }

        //TODO: float* GetStyleVarFloatAddr(ImGuiStyleVar idx)
        //{
        //    ImGuiState g = State;
        //    switch (idx)
        //    {
        //        case ImGuiStyleVar_Alpha: return &g.Style.Alpha;
        //        case ImGuiStyleVar_WindowRounding: return &g.Style.WindowRounding;
        //        case ImGuiStyleVar_ChildWindowRounding: return &g.Style.ChildWindowRounding;
        //        case ImGuiStyleVar_FrameRounding: return &g.Style.FrameRounding;
        //        case ImGuiStyleVar_IndentSpacing: return &g.Style.IndentSpacing;
        //        case ImGuiStyleVar_GrabMinSize: return &g.Style.GrabMinSize;
        //    }
        //    return null;
        //}

        //TODO: static ImVec2* GetStyleVarVec2Addr(ImGuiStyleVar idx)
        //{
        //    ImGuiState g = State;
        //    switch (idx)
        //    {
        //        case ImGuiStyleVar_WindowPadding: return &g.Style.WindowPadding;
        //        case ImGuiStyleVar_WindowMinSize: return &g.Style.WindowMinSize;
        //        case ImGuiStyleVar_FramePadding: return &g.Style.FramePadding;
        //        case ImGuiStyleVar_ItemSpacing: return &g.Style.ItemSpacing;
        //        case ImGuiStyleVar_ItemInnerSpacing: return &g.Style.ItemInnerSpacing;
        //    }
        //    return NULL;
        //}

        //TODO: static ImVec2* GetStyleVarVec2Addr(ImGuiStyleVar idx)
        //{
        //    ImGuiState g = State;
        //    switch (idx)
        //    {
        //        case ImGuiStyleVar_WindowPadding: return &g.Style.WindowPadding;
        //        case ImGuiStyleVar_WindowMinSize: return &g.Style.WindowMinSize;
        //        case ImGuiStyleVar_FramePadding: return &g.Style.FramePadding;
        //        case ImGuiStyleVar_ItemSpacing: return &g.Style.ItemSpacing;
        //        case ImGuiStyleVar_ItemInnerSpacing: return &g.Style.ItemInnerSpacing;
        //    }
        //    return NULL;
        //}

        void PushStyleVar(ImGuiStyleVar idx, float val)
        {
            ImGuiState g = State;
            ImGuiStyleMod backup;
            backup.Var = idx;

            float prev;
            switch (idx)
            {
                case ImGuiStyleVar.ImGuiStyleVar_Alpha: prev = g.Style.Alpha; g.Style.Alpha = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_WindowRounding: prev = g.Style.WindowRounding; g.Style.WindowRounding = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_ChildWindowRounding: prev = g.Style.ChildWindowRounding; g.Style.ChildWindowRounding = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_FrameRounding: prev = g.Style.FrameRounding; g.Style.FrameRounding = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_IndentSpacing: prev = g.Style.IndentSpacing; g.Style.IndentSpacing = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_GrabMinSize: prev = g.Style.GrabMinSize; g.Style.GrabMinSize = val; break;
                default:
                    System.Diagnostics.Debug.Assert(false); // Called function with wrong-type? Variable is not a float.
                    prev = 0;
                    break;
            }
            backup.PreviousValue = new ImVec2(prev, 0);
            g.StyleModifiers.push_back(backup);
        }

        void PushStyleVar(ImGuiStyleVar idx, ImVec2 val)
        {
            ImGuiState g = State;
            ImGuiStyleMod backup;
            backup.Var = idx;
            switch (idx)
            {
                case ImGuiStyleVar.ImGuiStyleVar_WindowPadding: backup.PreviousValue = g.Style.WindowPadding; g.Style.WindowPadding = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_WindowMinSize: backup.PreviousValue = g.Style.WindowMinSize; g.Style.WindowMinSize = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_FramePadding: backup.PreviousValue = g.Style.FramePadding; g.Style.FramePadding = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_ItemSpacing: backup.PreviousValue = g.Style.ItemSpacing; g.Style.ItemSpacing = val; break;
                case ImGuiStyleVar.ImGuiStyleVar_ItemInnerSpacing: backup.PreviousValue = g.Style.ItemInnerSpacing; g.Style.ItemInnerSpacing = val; break;
                default:
                    System.Diagnostics.Debug.Assert(false); // Called function with wrong-type? Variable is not a vec2.
                    backup.PreviousValue = new ImVec2(0, 0);
                    break;
            }

            g.StyleModifiers.push_back(backup);
        }

        void PopStyleVar(int count = 1)
        {
            ImGuiState g = State;
            while (count > 0)
            {
                ImGuiStyleMod backup = g.StyleModifiers.back();
                switch (backup.Var)
                {
                    //float
                    case ImGuiStyleVar.ImGuiStyleVar_Alpha: g.Style.Alpha = backup.PreviousValue.x; break;
                    case ImGuiStyleVar.ImGuiStyleVar_WindowRounding: g.Style.WindowRounding = backup.PreviousValue.x; break;
                    case ImGuiStyleVar.ImGuiStyleVar_ChildWindowRounding: g.Style.ChildWindowRounding = backup.PreviousValue.x; break;
                    case ImGuiStyleVar.ImGuiStyleVar_FrameRounding: g.Style.FrameRounding = backup.PreviousValue.x; break;
                    case ImGuiStyleVar.ImGuiStyleVar_IndentSpacing: g.Style.IndentSpacing = backup.PreviousValue.x; break;
                    case ImGuiStyleVar.ImGuiStyleVar_GrabMinSize: g.Style.GrabMinSize = backup.PreviousValue.x; break;
                    //vec2
                    case ImGuiStyleVar.ImGuiStyleVar_WindowPadding: g.Style.WindowPadding = backup.PreviousValue; break;
                    case ImGuiStyleVar.ImGuiStyleVar_WindowMinSize: g.Style.WindowMinSize = backup.PreviousValue; break;
                    case ImGuiStyleVar.ImGuiStyleVar_FramePadding: g.Style.FramePadding = backup.PreviousValue; break;
                    case ImGuiStyleVar.ImGuiStyleVar_ItemSpacing: g.Style.ItemSpacing = backup.PreviousValue; break;
                    case ImGuiStyleVar.ImGuiStyleVar_ItemInnerSpacing: g.Style.ItemInnerSpacing = backup.PreviousValue; break;
                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
                g.StyleModifiers.pop_back();
                count--;
            }
        }

        string GetStyleColName(ImGuiCol idx)
        {
            // Create switch-case from enum with regexp: ImGuiCol_{.*}, -. case ImGuiCol_\1: return "\1";
            switch (idx)
            {
                case ImGuiCol.ImGuiCol_Text: return "Text";
                case ImGuiCol.ImGuiCol_TextDisabled: return "TextDisabled";
                case ImGuiCol.ImGuiCol_WindowBg: return "WindowBg";
                case ImGuiCol.ImGuiCol_ChildWindowBg: return "ChildWindowBg";
                case ImGuiCol.ImGuiCol_Border: return "Border";
                case ImGuiCol.ImGuiCol_BorderShadow: return "BorderShadow";
                case ImGuiCol.ImGuiCol_FrameBg: return "FrameBg";
                case ImGuiCol.ImGuiCol_FrameBgHovered: return "FrameBgHovered";
                case ImGuiCol.ImGuiCol_FrameBgActive: return "FrameBgActive";
                case ImGuiCol.ImGuiCol_TitleBg: return "TitleBg";
                case ImGuiCol.ImGuiCol_TitleBgCollapsed: return "TitleBgCollapsed";
                case ImGuiCol.ImGuiCol_TitleBgActive: return "TitleBgActive";
                case ImGuiCol.ImGuiCol_MenuBarBg: return "MenuBarBg";
                case ImGuiCol.ImGuiCol_ScrollbarBg: return "ScrollbarBg";
                case ImGuiCol.ImGuiCol_ScrollbarGrab: return "ScrollbarGrab";
                case ImGuiCol.ImGuiCol_ScrollbarGrabHovered: return "ScrollbarGrabHovered";
                case ImGuiCol.ImGuiCol_ScrollbarGrabActive: return "ScrollbarGrabActive";
                case ImGuiCol.ImGuiCol_ComboBg: return "ComboBg";
                case ImGuiCol.ImGuiCol_CheckMark: return "CheckMark";
                case ImGuiCol.ImGuiCol_SliderGrab: return "SliderGrab";
                case ImGuiCol.ImGuiCol_SliderGrabActive: return "SliderGrabActive";
                case ImGuiCol.ImGuiCol_Button: return "Button";
                case ImGuiCol.ImGuiCol_ButtonHovered: return "ButtonHovered";
                case ImGuiCol.ImGuiCol_ButtonActive: return "ButtonActive";
                case ImGuiCol.ImGuiCol_Header: return "Header";
                case ImGuiCol.ImGuiCol_HeaderHovered: return "HeaderHovered";
                case ImGuiCol.ImGuiCol_HeaderActive: return "HeaderActive";
                case ImGuiCol.ImGuiCol_Column: return "Column";
                case ImGuiCol.ImGuiCol_ColumnHovered: return "ColumnHovered";
                case ImGuiCol.ImGuiCol_ColumnActive: return "ColumnActive";
                case ImGuiCol.ImGuiCol_ResizeGrip: return "ResizeGrip";
                case ImGuiCol.ImGuiCol_ResizeGripHovered: return "ResizeGripHovered";
                case ImGuiCol.ImGuiCol_ResizeGripActive: return "ResizeGripActive";
                case ImGuiCol.ImGuiCol_CloseButton: return "CloseButton";
                case ImGuiCol.ImGuiCol_CloseButtonHovered: return "CloseButtonHovered";
                case ImGuiCol.ImGuiCol_CloseButtonActive: return "CloseButtonActive";
                case ImGuiCol.ImGuiCol_PlotLines: return "PlotLines";
                case ImGuiCol.ImGuiCol_PlotLinesHovered: return "PlotLinesHovered";
                case ImGuiCol.ImGuiCol_PlotHistogram: return "PlotHistogram";
                case ImGuiCol.ImGuiCol_PlotHistogramHovered: return "PlotHistogramHovered";
                case ImGuiCol.ImGuiCol_TextSelectedBg: return "TextSelectedBg";
                case ImGuiCol.ImGuiCol_TooltipBg: return "TooltipBg";
                case ImGuiCol.ImGuiCol_ModalWindowDarkening: return "ModalWindowDarkening";
            }
            System.Diagnostics.Debug.Assert(false);
            return "Unknown";
        }

        bool IsWindowHovered()
        {
            ImGuiState g = State;
            return g.HoveredWindow == g.CurrentWindow && IsWindowContentHoverable(g.HoveredRootWindow);
        }

        bool IsWindowFocused()
        {
            ImGuiState g = State;
            return g.FocusedWindow == g.CurrentWindow;
        }

        bool IsRootWindowFocused()
        {
            ImGuiState g = State;
            ImGuiWindow root_window = g.CurrentWindow.RootWindow;
            return g.FocusedWindow == root_window;
        }

        bool IsRootWindowOrAnyChildFocused()
        {
            ImGuiState g = State;
            ImGuiWindow root_window = g.CurrentWindow.RootWindow;
            return g.FocusedWindow != null && g.FocusedWindow.RootWindow == root_window;
        }

        float GetWindowWidth()
        {
            ImGuiWindow window = State.CurrentWindow;
            return window.Size.x;
        }

        float GetWindowHeight()
        {
            ImGuiWindow window = State.CurrentWindow;
            return window.Size.y;
        }

        ImVec2 GetWindowPos()
        {
            ImGuiState g = State;
            ImGuiWindow window = g.CurrentWindow;
            return window.Pos;
        }

        void SetWindowScrollY(ImGuiWindow window, float new_scroll_y)
        {
            window.DC.CursorMaxPos.y += window.Scroll.y;
            window.Scroll.y = new_scroll_y;
            window.DC.CursorMaxPos.y -= window.Scroll.y;
        }

        void SetWindowPos(ImGuiWindow window, ImVec2 pos, ImGuiSetCond cond = 0)
        {
            // Test condition (NB: bit 0 is always true) and clear flags for next time
            if (cond != 0 && (window.SetWindowPosAllowFlags & (int)cond) == 0)
                return;
            window.SetWindowPosAllowFlags &= ~(int)(ImGuiSetCond.ImGuiSetCond_Once | ImGuiSetCond.ImGuiSetCond_FirstUseEver | ImGuiSetCond.ImGuiSetCond_Appearing);
            window.SetWindowPosCenterWanted = false;

            // Set
            ImVec2 old_pos = window.Pos;
            window.PosFloat = pos;
            window.Pos = new ImVec2((float)(int)window.PosFloat.x, (float)(int)window.PosFloat.y);
            window.DC.CursorPos += (window.Pos - old_pos);    // As we happen to move the window while it is being appended to (which is a bad idea - will smear) let's at least offset the cursor
            window.DC.CursorMaxPos += (window.Pos - old_pos); // And more importantly we need to adjust this so size calculation doesn't get affected.
        }

        void SetWindowPos(ImVec2 pos, ImGuiSetCond cond = 0)
        {
            ImGuiWindow window = GetCurrentWindow();
            SetWindowPos(window, pos, cond);
        }

        void SetWindowPos(string name, ImVec2 pos, ImGuiSetCond cond = 0)
        {
            ImGuiWindow window = FindWindowByName(name);
            if (window != null)
                SetWindowPos(window, pos, cond);
        }

        ImVec2 GetWindowSize()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.Size;
        }

        static void SetWindowSize(ImGuiWindow window, ImVec2 size, ImGuiSetCond cond = 0)
        {
            // Test condition (NB: bit 0 is always true) and clear flags for next time
            if (cond != 0 && (window.SetWindowSizeAllowFlags & (int)cond) == 0)
                return;
            window.SetWindowSizeAllowFlags &= ~(int)(ImGuiSetCond.ImGuiSetCond_Once | ImGuiSetCond.ImGuiSetCond_FirstUseEver | ImGuiSetCond.ImGuiSetCond_Appearing);

            // Set
            if (size.x > 0.0f)
            {
                window.AutoFitFramesX = 0;
                window.SizeFull.x = size.x;
            }
            else
            {
                window.AutoFitFramesX = 2;
                window.AutoFitOnlyGrows = false;
            }
            if (size.y > 0.0f)
            {
                window.AutoFitFramesY = 0;
                window.SizeFull.y = size.y;
            }
            else
            {
                window.AutoFitFramesY = 2;
                window.AutoFitOnlyGrows = false;
            }
        }

        void SetWindowSize(ImVec2 size, ImGuiSetCond cond = 0)
        {
            SetWindowSize(State.CurrentWindow, size, cond);
        }

        void SetWindowSize(string name, ImVec2 size, ImGuiSetCond cond = 0)
        {
            ImGuiWindow window = FindWindowByName(name);
            if (window != null)
                SetWindowSize(window, size, cond);
        }

        static void SetWindowCollapsed(ImGuiWindow window, bool collapsed, ImGuiSetCond cond = 0)
        {
            // Test condition (NB: bit 0 is always true) and clear flags for next time
            if (cond != 0 && (window.SetWindowCollapsedAllowFlags & (int)cond) == 0)
                return;
            window.SetWindowCollapsedAllowFlags &= ~(int)(ImGuiSetCond.ImGuiSetCond_Once | ImGuiSetCond.ImGuiSetCond_FirstUseEver | ImGuiSetCond.ImGuiSetCond_Appearing);

            // Set
            window.Collapsed = collapsed;
        }

        void SetWindowCollapsed(bool collapsed, ImGuiSetCond cond = 0)
        {
            SetWindowCollapsed(State.CurrentWindow, collapsed, cond);
        }

        bool IsWindowCollapsed()
        {
            return State.CurrentWindow.Collapsed;
        }

        void SetWindowCollapsed(string name, bool collapsed, ImGuiSetCond cond = 0)
        {
            ImGuiWindow window = FindWindowByName(name);
            if (window != null)
                SetWindowCollapsed(window, collapsed, cond);
        }

        void SetWindowFocus()
        {
            FocusWindow(State.CurrentWindow);
        }

        void SetWindowFocus(string name)
        {
            if (name != null)
            {
                ImGuiWindow window = FindWindowByName(name);
                if (window != null)
                    FocusWindow(window);
            }
            else
            {
                FocusWindow(null);
            }
        }

        void SetNextWindowPos(ImVec2 pos, ImGuiSetCond cond = 0)
        {
            ImGuiState g = State;
            g.SetNextWindowPosVal = pos;
            g.SetNextWindowPosCond = cond != 0 ? cond : ImGuiSetCond.ImGuiSetCond_Always;
        }

        void SetNextWindowPosCenter(ImGuiSetCond cond = 0)
        {
            ImGuiState g = State;
            g.SetNextWindowPosVal = new ImVec2(-float.MaxValue, -float.MaxValue);
            g.SetNextWindowPosCond = cond != 0 ? cond : ImGuiSetCond.ImGuiSetCond_Always;
        }

        void SetNextWindowSize(ImVec2 size, ImGuiSetCond cond = 0)
        {
            ImGuiState g = State;
            g.SetNextWindowSizeVal = size;
            g.SetNextWindowSizeCond = cond != 0 ? cond : ImGuiSetCond.ImGuiSetCond_Always;
        }

        void SetNextWindowContentSize(ImVec2 size)
        {
            ImGuiState g = State;
            g.SetNextWindowContentSizeVal = size;
            g.SetNextWindowContentSizeCond = ImGuiSetCond.ImGuiSetCond_Always;
        }

        void SetNextWindowContentWidth(float width)
        {
            ImGuiState g = State;
            g.SetNextWindowContentSizeVal = new ImVec2(width, g.SetNextWindowContentSizeCond != 0 ? g.SetNextWindowContentSizeVal.y : 0.0f);
            g.SetNextWindowContentSizeCond = ImGuiSetCond.ImGuiSetCond_Always;
        }

        void SetNextWindowCollapsed(bool collapsed, ImGuiSetCond cond = 0)
        {
            ImGuiState g = State;
            g.SetNextWindowCollapsedVal = collapsed;
            g.SetNextWindowCollapsedCond = cond != 0 ? cond : ImGuiSetCond.ImGuiSetCond_Always;
        }

        void SetNextWindowFocus()
        {
            ImGuiState g = State;
            g.SetNextWindowFocus = true;
        }

        // In window space (not screen space!)
        // FIXME-OPT: Could cache and maintain it (pretty much only change on columns change)
        ImVec2 GetContentRegionMax()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            ImVec2 content_region_size = new ImVec2(window.SizeContentsExplicit.x != 0 ? window.SizeContentsExplicit.x : window.Size.x - window.ScrollbarSizes.x, window.SizeContentsExplicit.y != 0 ? window.SizeContentsExplicit.y : window.Size.y - window.ScrollbarSizes.y);
            ImVec2 mx = content_region_size - window.Scroll - window.WindowPadding;
            if (window.DC.ColumnsCount != 1)
                mx.x = GetColumnOffset(window.DC.ColumnsCurrent + 1) - window.WindowPadding.x;
            return mx;
        }

        ImVec2 GetContentRegionAvail()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return GetContentRegionMax() - (window.DC.CursorPos - window.Pos);
        }

        float GetContentRegionAvailWidth()
        {
            return GetContentRegionAvail().x;
        }

        // In window space (not screen space!)
        ImVec2 GetWindowContentRegionMin()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return new ImVec2(-window.Scroll.x, -window.Scroll.y + window.TitleBarHeight() + window.MenuBarHeight()) + window.WindowPadding;
        }

        ImVec2 GetWindowContentRegionMax()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            ImVec2 content_region_size = new ImVec2(window.SizeContentsExplicit.x != 0 ? window.SizeContentsExplicit.x : window.Size.x, window.SizeContentsExplicit.y != 0 ? window.SizeContentsExplicit.y : window.Size.y);
            ImVec2 m = content_region_size - window.Scroll - window.WindowPadding - window.ScrollbarSizes;
            return m;
        }

        float GetWindowContentRegionWidth()
        {
            return GetWindowContentRegionMax().x - GetWindowContentRegionMin().x;
        }

        float GetTextLineHeight()
        {
            ImGuiState g = State;
            return g.FontSize;
        }

        float GetTextLineHeightWithSpacing()
        {
            ImGuiState g = State;
            return g.FontSize + g.Style.ItemSpacing.y;
        }

        float GetItemsLineHeightWithSpacing()
        {
            ImGuiState g = State;
            return g.FontSize + g.Style.FramePadding.y * 2.0f + g.Style.ItemSpacing.y;
        }

        ImDrawList GetWindowDrawList()
        {
            ImGuiWindow window = GetCurrentWindow();
            return window.DrawList;
        }

        ImFont GetWindowFont()
        {
            ImGuiState g = State;
            return g.Font;
        }

        float GetWindowFontSize()
        {
            ImGuiState g = State;
            return g.FontSize;
        }

        void SetWindowFontScale(float scale)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();
            window.FontWindowScale = scale;
            g.FontSize = window.CalcFontSize();
        }

        // User generally sees positions in window coordinates. Internally we store CursorPos in absolute screen coordinates because it is more convenient.
        // Conversion happens as we pass the value to user, but it makes our naming convention confusing because GetCursorPos() == (DC.CursorPos - window.Pos). May want to rename 'DC.CursorPos'.
        ImVec2 GetCursorPos()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.CursorPos - window.Pos + window.Scroll;
        }

        float GetCursorPosX()
        {
            ImGuiWindow window = GetCurrentWindow();
            return window.DC.CursorPos.x - window.Pos.x + window.Scroll.x;
        }

        internal float GetCursorPosY()
        {
            ImGuiWindow window = GetCurrentWindow();
            return window.DC.CursorPos.y - window.Pos.y + window.Scroll.y;
        }

        void SetCursorPos(ImVec2 local_pos)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.CursorPos = window.Pos - window.Scroll + local_pos;
            window.DC.CursorMaxPos = Max(window.DC.CursorMaxPos, window.DC.CursorPos);
        }

        void SetCursorPosX(float x)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.CursorPos.x = window.Pos.x - window.Scroll.x + x;
            window.DC.CursorMaxPos.x = Max(window.DC.CursorMaxPos.x, window.DC.CursorPos.x);
        }

        internal void SetCursorPosY(float y)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.CursorPos.y = window.Pos.y - window.Scroll.y + y;
            window.DC.CursorMaxPos.y = Max(window.DC.CursorMaxPos.y, window.DC.CursorPos.y);
        }

        ImVec2 GetCursorStartPos()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.CursorStartPos - window.Pos;
        }

        ImVec2 GetCursorScreenPos()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.CursorPos;
        }

        void SetCursorScreenPos(ImVec2 screen_pos)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.CursorPos = screen_pos;
        }

        float GetScrollX()
        {
            return State.CurrentWindow.Scroll.x;
        }

        float GetScrollY()
        {
            return State.CurrentWindow.Scroll.y;
        }

        float GetScrollMaxX()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.SizeContents.x - window.SizeFull.x - window.ScrollbarSizes.x;
        }

        float GetScrollMaxY()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.SizeContents.y - window.SizeFull.y - window.ScrollbarSizes.y;
        }

        void SetScrollX(float scroll_x)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.ScrollTarget.x = scroll_x;
            window.ScrollTargetCenterRatio.x = 0.0f;
        }

        void SetScrollY(float scroll_y)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.ScrollTarget.y = scroll_y + window.TitleBarHeight(); // title bar height canceled out when using ScrollTargetRelY
            window.ScrollTargetCenterRatio.y = 0.0f;
        }

        void SetScrollFromPosY(float pos_y, float center_y_ratio = 0.5f)
        {
            // We store a target position so centering can occur on the next frame when we are guaranteed to have a known window size
            ImGuiWindow window = GetCurrentWindow();
            System.Diagnostics.Debug.Assert(center_y_ratio >= 0.0f && center_y_ratio <= 1.0f);
            window.ScrollTarget.y = (int)(pos_y + window.Scroll.y);
            if (center_y_ratio <= 0.0f && window.ScrollTarget.y <= window.WindowPadding.y)    // Minor hack to make "scroll to top" take account of WindowPadding, else it would scroll to (WindowPadding.y - ItemSpacing.y)
                window.ScrollTarget.y = 0.0f;
            window.ScrollTargetCenterRatio.y = center_y_ratio;
        }

        // center_y_ratio: 0.0f top of last item, 0.5f vertical center of last item, 1.0f bottom of last item.
        void SetScrollHere(float center_y_ratio = 0.5f)
        {
            ImGuiWindow window = GetCurrentWindow();
            float target_y = window.DC.CursorPosPrevLine.y + (window.DC.PrevLineHeight * center_y_ratio) + (State.Style.ItemSpacing.y * (center_y_ratio - 0.5f) * 2.0f); // Precisely aim above, in the middle or below the last line.
            SetScrollFromPosY(target_y - window.Pos.y, center_y_ratio);
        }

        void SetKeyboardFocusHere(int offset = 0)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.FocusIdxAllRequestNext = window.FocusIdxAllCounter + 1 + offset;
            window.FocusIdxTabRequestNext = int.MaxValue;
        }

        void SetStateStorage(ImGuiStorage tree)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.StateStorage = tree != null ? tree : window.StateStorage;
        }

        ImGuiStorage GetStateStorage()
        {
            ImGuiWindow window = GetCurrentWindowRead();
            return window.DC.StateStorage;
        }

        void SetCurrentWindow(ImGuiWindow window)
        {
            ImGuiState g = State;
            g.CurrentWindow = window;
            if (window != null)
                g.FontSize = window.CalcFontSize();
        }

        ImGuiWindow GetParentWindow()
        {
            ImGuiState g = State;
            System.Diagnostics.Debug.Assert(g.CurrentWindowStack.Size >= 2);
            return g.CurrentWindowStack[g.CurrentWindowStack.Size - 2];
        }

        void SetActiveID(uint id, ImGuiWindow window = null)
        {
            ImGuiState g = State;
            g.ActiveId = id;
            g.ActiveIdAllowOverlap = false;
            g.ActiveIdIsJustActivated = true;
            g.ActiveIdWindow = window;
        }

        void SetHoveredID(uint id)
        {
            ImGuiState g = State;
            g.HoveredId = id;
            g.HoveredIdAllowOverlap = false;
        }

        internal void KeepAliveID(uint id)
        {
            ImGuiState g = State;
            if (g.ActiveId == id)
                g.ActiveIdIsAlive = true;
        }

        // Advance cursor given item size for layout.
        void ItemSize(ImVec2 size, float text_offset_y = 0f)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // Always align ourselves on pixel boundaries
            ImGuiState g = State;
            float line_height = Max(window.DC.CurrentLineHeight, size.y);
            float text_base_offset = Max(window.DC.CurrentLineTextBaseOffset, text_offset_y);
            window.DC.CursorPosPrevLine = new ImVec2(window.DC.CursorPos.x + size.x, window.DC.CursorPos.y);
            window.DC.CursorPos = new ImVec2((float)(int)(window.Pos.x + window.DC.IndentX + window.DC.ColumnsOffsetX), (int)(window.DC.CursorPos.y + line_height + g.Style.ItemSpacing.y));
            window.DC.CursorMaxPos.x = Max(window.DC.CursorMaxPos.x, window.DC.CursorPosPrevLine.x);
            window.DC.CursorMaxPos.y = Max(window.DC.CursorMaxPos.y, window.DC.CursorPos.y);

            //window.DrawList.AddCircle(window.DC.CursorMaxPos, 3.0f, 0xFF0000FF, 4); // Debug

            window.DC.PrevLineHeight = line_height;
            window.DC.PrevLineTextBaseOffset = text_base_offset;
            window.DC.CurrentLineHeight = window.DC.CurrentLineTextBaseOffset = 0.0f;
        }

        void ItemSize(ImRect bb, float text_offset_y = 0f)
        {
            ItemSize(bb.GetSize(), text_offset_y);
        }

        // Declare item bounding box for clipping and interaction.
        // Note that the size can be different than the one provided to ItemSize(). Typically, widgets that spread over available surface
        // declares their minimum size requirement to ItemSize() and then use a larger region for drawing/interaction, which is passed to ItemAdd().
        bool ItemAdd(ImRect bb, uint? id)
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DC.LastItemID = id.HasValue ? id.Value : 0;
            window.DC.LastItemRect = bb;
            if (IsClippedEx(bb, id, false))
            {
                window.DC.LastItemHoveredAndUsable = window.DC.LastItemHoveredRect = false;
                return false;
            }

            // This is a sensible default, but widgets are free to override it after calling ItemAdd()
            ImGuiState g = State;
            if (IsMouseHoveringRect(bb.Min, bb.Max))
            {
                // Matching the behavior of IsHovered() but ignore if ActiveId==window.MoveID (we clicked on the window background)
                // So that clicking on items with no active id such as Text() still returns true with IsItemHovered()
                window.DC.LastItemHoveredRect = true;
                window.DC.LastItemHoveredAndUsable = false;
                if (g.HoveredRootWindow == window.RootWindow)
                    if (g.ActiveId == 0 || (id.HasValue && g.ActiveId == id.Value) || g.ActiveIdAllowOverlap || (g.ActiveId == window.MoveID))
                        if (IsWindowContentHoverable(window))
                            window.DC.LastItemHoveredAndUsable = true;
            }
            else
            {
                window.DC.LastItemHoveredAndUsable = window.DC.LastItemHoveredRect = false;
            }

            return true;
        }

        bool IsClippedEx(ImRect bb, uint? id, bool clip_even_when_logged)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindowRead();

            if (!bb.Overlaps(window.ClipRect))
            {
                if (!id.HasValue || id.Value != State.ActiveId)
                    if (clip_even_when_logged || !g.LogEnabled)
                        return true;
            }
            return false;
        }

        bool IsHovered(ImRect bb, uint id, bool flatten_childs = false)
        {
            ImGuiState g = State;
            if (g.HoveredId == 0 || g.HoveredId == id || g.HoveredIdAllowOverlap)
            {
                ImGuiWindow window = GetCurrentWindowRead();
                if (g.HoveredWindow == window || (flatten_childs && g.HoveredRootWindow == window.RootWindow))
                    if ((g.ActiveId == 0 || g.ActiveId == id || g.ActiveIdAllowOverlap) && IsMouseHoveringRect(bb.Min, bb.Max))
                        if (IsWindowContentHoverable(g.HoveredRootWindow))
                            return true;
            }
            return false;
        }

        bool FocusableItemRegister(ImGuiWindow window, bool is_active, bool tab_stop = true)
        {
            ImGuiState g = State;

            bool allow_keyboard_focus = window.DC.AllowKeyboardFocus;
            window.FocusIdxAllCounter++;
            if (allow_keyboard_focus)
                window.FocusIdxTabCounter++;

            // Process keyboard input at this point: TAB, Shift-TAB switch focus
            // We can always TAB out of a widget that doesn't allow tabbing in.
            if (tab_stop && window.FocusIdxAllRequestNext == int.MaxValue && window.FocusIdxTabRequestNext == int.MaxValue && is_active && IsKeyPressedMap(ImGuiKey.ImGuiKey_Tab))
            {
                // Modulo on index will be applied at the end of frame once we've got the total counter of items.
                window.FocusIdxTabRequestNext = window.FocusIdxTabCounter + (g.IO.KeyShift ? (allow_keyboard_focus ? -1 : 0) : +1);
            }

            if (window.FocusIdxAllCounter == window.FocusIdxAllRequestCurrent)
                return true;

            if (allow_keyboard_focus)
                if (window.FocusIdxTabCounter == window.FocusIdxTabRequestCurrent)
                    return true;

            return false;
        }

        void FocusableItemUnregister(ImGuiWindow window)
        {
            window.FocusIdxAllCounter--;
            window.FocusIdxTabCounter--;
        }

        ImVec2 CalcItemSize(ImVec2 size, float default_x, float default_y)
        {
            ImGuiState g = State;
            ImVec2 content_max = new ImVec2(0, 0);
            if (size.x < 0.0f || size.y < 0.0f)
                content_max = g.CurrentWindow.Pos + GetContentRegionMax();
            if (size.x <= 0.0f)
                size.x = (size.x == 0.0f) ? default_x : Max(content_max.x - g.CurrentWindow.DC.CursorPos.x, 4.0f) + size.x;
            if (size.y <= 0.0f)
                size.y = (size.y == 0.0f) ? default_y : Max(content_max.y - g.CurrentWindow.DC.CursorPos.y, 4.0f) + size.y;
            return size;
        }

        float CalcWrapWidthForPos(ImVec2 pos, float wrap_pos_x)
        {
            if (wrap_pos_x < 0.0f)
                return 0.0f;

            ImGuiWindow window = GetCurrentWindowRead();
            if (wrap_pos_x == 0.0f)
                wrap_pos_x = GetContentRegionMax().x + window.Pos.x;
            else if (wrap_pos_x > 0.0f)
                wrap_pos_x += window.Pos.x - window.Scroll.x; // wrap_pos_x is provided is window local space

            float wrap_width = wrap_pos_x > 0.0f ? Max(wrap_pos_x - pos.x, 0.00001f) : 0.0f;
            return wrap_width;
        }

        string GetClipboardText()
        {
            return State.IO.GetClipboardTextFn != null ? State.IO.GetClipboardTextFn() : "";
        }

        void SetClipboardText(string text)
        {
            if (State.IO.SetClipboardTextFn != null)
                State.IO.SetClipboardTextFn(text);
        }

        string GetVersion()
        {
            //TODO: return IMGUI_VERSION;
            return "0.0.0.1";
        }

        //// Internal state access - if you want to share ImGui state between modules (e.g. DLL) or allocate it yourself
        //// Note that we still point to some static data and members (such as GFontAtlas), so the state instance you end up using will point to the static data within its module
        //string GetInternalState()
        //{
        //    return State;
        //}

        //size_t GetInternalStateSize()
        //{
        //    return sizeof(ImGuiState);
        //}

        //void SetInternalState(void* state, bool construct)
        //{
        //    if (construct)
        //        IM_PLACEMENT_NEW(state) ImGuiState();
        //    State = (ImGuiState*)state;
        //}

        internal ImGuiStyle GetStyle()
        {
            return State.Style;
        }

        // Same value as passed to your RenderDrawListsFn() function. valid after Render() and until the next call to NewFrame()
        internal ImDrawData GetDrawData()
        {
            return State.RenderDrawData.Valid ? State.RenderDrawData : null;
        }

        internal float GetTime()
        {
            return State.Time;
        }

        internal int GetFrameCount()
        {
            return State.FrameCount;
        }

        // NB: behavior of ImGui after Shutdown() is not tested/guaranteed at the moment. This function is merely here to free heap allocations.
        public void Shutdown()
        {
            ImGuiState g = State;

            // The fonts atlas can be used prior to calling NewFrame(), so we clear it even if g.Initialized is FALSE (which would happen if we never called NewFrame)
            if (g.IO.Fonts != null) // Testing for null to allow user to nullify in case of running Shutdown() on multiple contexts. Bit hacky.
                g.IO.Fonts.Clear();

            // Cleanup of other data are conditional on actually having used ImGui.
            if (!g.Initialized)
                return;

            SaveSettings(g);

            for (int i = 0; i < g.Windows.Size; i++)
            {
                g.Windows[i].Dispose();
                g.Windows[i] = null;
                //g.Windows[i].~ImGuiWindow();
                //MemFree(g.Windows[i]);
            }
            g.Windows.clear();
            g.WindowsSortBuffer.clear();
            g.CurrentWindowStack.clear();
            g.FocusedWindow = null;
            g.HoveredWindow = null;
            g.HoveredRootWindow = null;
            //for (int i = 0; i < g.Settings.Size; i++)
            //    MemFree(g.Settings[i].Name);
            g.Settings.clear();
            g.ColorModifiers.clear();
            g.StyleModifiers.clear();
            g.FontStack.clear();
            g.OpenedPopupStack.clear();
            g.CurrentPopupStack.clear();
            for (int i = 0; i < g.RenderDrawLists.Length; i++)
                g.RenderDrawLists[i].clear();
            g.OverlayDrawList.ClearFreeMemory();
            g.ColorEditModeStorage.Clear();
            if (g.PrivateClipboard != null)
            {
                //MemFree(g.PrivateClipboard);
                g.PrivateClipboard = null;
            }
            g.InputTextState.Text.clear();
            g.InputTextState.InitialText.clear();
            g.InputTextState.TempTextBuffer.clear();

            //TODO: if (g.LogFile && g.LogFile != stdout)
            //{
            //    fclose(g.LogFile);
            //    g.LogFile = null;
            //}
            //if (g.LogClipboard)
            //{
            //    g.LogClipboard.~ImGuiTextBuffer();
            //    MemFree(g.LogClipboard);
            //}

            g.Initialized = false;
        }

        // FIXME: Add a more explicit sort order in the window structure.
        static int ChildWindowComparer(ImGuiWindow a, ImGuiWindow b)
        {
            int d;
            if ((d = (a.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) - (b.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup)) != 0)
                return d;
            if ((d = (a.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) - (b.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip)) != 0)
                return d;
            if ((d = (a.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) - (b.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ComboBox)) != 0)
                return d;
            return 0;
        }

        void AddWindowToSortedBuffer(ImVector<ImGuiWindow> out_sorted_windows, ImGuiWindow window)
        {
            out_sorted_windows.push_back(window);
            if (window.Active)
            {
                int count = window.DC.ChildWindows.Size;
                if (count > 1)
                    window.DC.ChildWindows.sort(ChildWindowComparer);
                for (int i = 0; i < count; i++)
                {
                    ImGuiWindow child = window.DC.ChildWindows[i];
                    if (child.Active)
                        AddWindowToSortedBuffer(out_sorted_windows, child);
                }
            }
        }

        void AddDrawListToRenderList(ImVector<ImDrawList> out_render_list, ImDrawList draw_list)
        {
            if (draw_list.CmdBuffer.empty())
                return;

            // Remove trailing command if unused
            ImDrawCmd last_cmd = draw_list.CmdBuffer.back();
            if (last_cmd.ElemCount == 0 && last_cmd.UserCallback == null)
            {
                draw_list.CmdBuffer.pop_back();
                if (draw_list.CmdBuffer.empty())
                    return;
            }

            // Check that draw_list doesn't use more vertices than indexable (default ImDrawIdx = 2 bytes = 64K vertices)
            // If this assert triggers because you are drawing lots of stuff manually, A) workaround by calling BeginChild()/EndChild() to put your draw commands in multiple draw lists, B) #define ImDrawIdx to a 'unsigned int' in imconfig.h and render accordingly.
            System.Diagnostics.Debug.Assert(draw_list._VtxCurrentIdx == draw_list.VtxBuffer.Size);                                                 // Sanity check. Bug or mismatch between PrimReserve() calls and incrementing _VtxCurrentIdx, _VtxWritePtr etc.
            //System.Diagnostics.Debug.Assert(draw_list._VtxCurrentIdx <= (1 << 8));  // Too many vertices in same ImDrawList. See comment above.

            out_render_list.push_back(draw_list);
            State.IO.MetricsRenderVertices += draw_list.VtxBuffer.Size;
            State.IO.MetricsRenderIndices += draw_list.IdxBuffer.Size;
        }

        void AddWindowToRenderList(ImVector<ImDrawList> out_render_list, ImGuiWindow window)
        {
            AddDrawListToRenderList(out_render_list, window.DrawList);
            for (int i = 0; i < window.DC.ChildWindows.Size; i++)
            {
                ImGuiWindow child = window.DC.ChildWindows[i];
                if (!child.Active) // clipped children may have been marked not active
                    continue;
                if ((child.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0 && child.HiddenFrames > 0)
                    continue;
                AddWindowToRenderList(out_render_list, child);
            }
        }

        void PushClipRect(ImVec2 clip_rect_min, ImVec2 clip_rect_max, bool intersect_with_existing_clip_rect)
        {
            ImGuiWindow window = GetCurrentWindow();

            ImRect cr = new ImRect(clip_rect_min, clip_rect_max);
            if (intersect_with_existing_clip_rect)
            {
                // Clip our argument with the current clip rect
                cr.Clip(window.ClipRect);
            }
            cr.Max.x = Max(cr.Min.x, cr.Max.x);
            cr.Max.y = Max(cr.Min.y, cr.Max.y);

            System.Diagnostics.Debug.Assert(cr.Min.x <= cr.Max.x && cr.Min.y <= cr.Max.y);
            window.ClipRect = cr;
            window.DrawList.PushClipRect(new ImVec4(cr.Min.x, cr.Min.y, cr.Max.x, cr.Max.y));
        }

        void PopClipRect()
        {
            ImGuiWindow window = GetCurrentWindow();
            window.DrawList.PopClipRect();
            window.ClipRect = new ImRect(window.DrawList._ClipRectStack.back());
        }

        ImRect GetVisibleRect()
        {
            ImGuiState g = State;
            if (g.IO.DisplayVisibleMin.x != g.IO.DisplayVisibleMax.x && g.IO.DisplayVisibleMin.y != g.IO.DisplayVisibleMax.y)
                return new ImRect(g.IO.DisplayVisibleMin, g.IO.DisplayVisibleMax);
            return new ImRect(0.0f, 0.0f, g.IO.DisplaySize.x, g.IO.DisplaySize.y);
        }

        bool IsWindowContentHoverable(ImGuiWindow window)
        {
            // An active popup disable hovering on other windows (apart from its own children)
            ImGuiState g = State;
            ImGuiWindow focused_window, focused_root_window;
            if ((focused_window = g.FocusedWindow) != null)
                if ((focused_root_window = focused_window.RootWindow) != null)
                    if ((focused_root_window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0 && focused_root_window.WasActive && focused_root_window != window.RootWindow)
                        return false;

            return true;
        }

        // Zero-tolerance, poor-man .ini parsing
        // FIXME: Write something less rubbish
        void LoadSettings()
        {
            
            ImGuiState g = State;                               // get access to state
            string filename = g.IO.IniFilename;                 // get filename

            if (filename == null)                               // file name is not set
                return;                                         // quit

            if (!File.Exists(filename))                         // the file does not exist
            {
                File.Create(filename);
                return;                                          // quit
            }

            if (new FileInfo(filename).Length == 0)             // the file is empty
                return;                                         // quit

            StreamReader f = new StreamReader(filename);        // open the file for reading

            ImGuiIniData settings = null;                       // create settings holder
           
            while(!f.EndOfStream)                               // while the whole file has not been read
            {
                string line = f.ReadLine();                     // read next line

                if (line.StartsWith("[") && line.EndsWith("]")) // the line is a name setting
                {
                    char[] braces = { '[', ']' };               // characters to trim off string
                    string name = line.Trim(braces);            // trim off characters
                    settings = FindWindowSettings(name);        // find a setting ID that matches name
                    if (settings == null)                       // the setting does not exist
                        settings = AddWindowSettings(name);     // create the setting
                }
                else if (settings != null)                      // there is a current setting
                {
                    if(line.StartsWith("Pos="))                 // the position line
                    {
                        string[] parts = line.Split(new[]       // split the line into substrings
                        {
                            '=', ','                            // split at the '=' and the ','
                        });

                        settings.Pos = new ImVec2(              // set the postion 
                            float.Parse(parts[1]),              // parse the x value from the string
                            float.Parse(parts[2]));             // parse the y value from the string
                    }
                    else if (line.StartsWith("Size="))          // the size line
                    {
                        string[] parts = line.Split(new[]       // split the line into substrings
                        {
                            '=', ','                            // split at the '=' and the ','
                        });

                        settings.Size = new ImVec2(             // set the size 
                            float.Parse(parts[1]),              // parse the x value from the string
                            float.Parse(parts[2]));             // parse the y value from the string
                    }
                    else if (line.StartsWith("Collapsed="))
                    {
                        string[] parts = line.Split(new[]       // split the line into substrings
                        {
                            '='                                 // split at the '='
                        });

                        bool value;                             // holder for collapsed value
                        if (parts[1] == "False")                // the read in string states false
                            value = false;                      // value is false
                        else                                    // the read in string states something else
                            value = true;                       // value is true
                        settings.Collapsed = value;             // set collapsed
                    }
                }
            }

            f.Close();                                          // close the file

            //TODO: LoadSettings()
            //const char* filename = g.IO.IniFilename;

            //if (!filename)
            //    return;

            //int file_size;
            //char* file_data = (char*)ImLoadFileToMemory(filename, "rb", &file_size, 1);
            //if (!file_data)
            //    return;

            //ImGuiIniData* settings = NULL;
            //const char* buf_end = file_data + file_size;
            //for (const char* line_start = file_data; line_start < buf_end; )
            //{
            //    const char* line_end = line_start;
            //    while (line_end < buf_end && *line_end != '\n' && *line_end != '\r')
            //        line_end++;

            //    if (line_start[0] == '[' && line_end > line_start && line_end[-1] == ']')
            //    {
            //        char name[64];
            //        ImFormatString(name, IM_ARRAYSIZE(name), "%.*s", (int)(line_end - line_start - 2), line_start + 1);
            //        settings = FindWindowSettings(name);
            //        if (!settings)
            //            settings = AddWindowSettings(name);
            //    }
            //    else if (settings)
            //    {
            //        float x, y;
            //        int i;
            //        if (sscanf(line_start, "Pos=%f,%f", &x, &y) == 2)
            //            settings->Pos = ImVec2(x, y);
            //        else if (sscanf(line_start, "Size=%f,%f", &x, &y) == 2)
            //            settings->Size = ImMax(ImVec2(x, y), g.Style.WindowMinSize);
            //        else if (sscanf(line_start, "Collapsed=%d", &i) == 1)
            //            settings->Collapsed = (i != 0);
            //    }

            //    line_start = line_end + 1;
            //}

            //ImGui::MemFree(file_data);
        }

        static void SaveSettings(ImGuiState state)
        {
            ImGuiState g = state;                               // get access to state

            string filename = g.IO.IniFilename;                 // get filename
            if (filename == null)                               // file name is not set
                return;                                         // quit
                                      
            for (int i = 0; i != g.Windows.Size; i++)           // Gather data from windows that were active during this session
            {
                ImGuiWindow window = g.Windows[i];              // grab the window

                ImGuiWindowFlags saveflag = ImGuiWindowFlags    // get the value of don't save settings flag
                    .ImGuiWindowFlags_NoSavedSettings;

                if ((window.Flags &saveflag).Equals(1))         // window flags has the don't save settings flag set
                    continue;                                   // skip this window

                uint ID = Hash(0, window.Name);                 // get the ID of the window

                ImGuiIniData settings = null;                   // create a settings holder

                for (int n = 0; n != g.Settings.Size; n++)      // of all the settings objects in the container
                    if (g.Settings[i].ID == ID)                 // the droid you were looking for
                        settings = g.Settings[i];               // save it into the holder

                if(settings != null)                            // the setting object was found
                {
                    settings.Pos       = window.Pos;            // transfer position value
                    settings.Size      = window.SizeFull;       // transfer size value
                    settings.Collapsed = window.Collapsed;      // transfer collapsed value
                }
            }

            if (!File.Exists(filename))                         // ini file does not exist
                File.Create(filename);                          // create it

            StreamWriter f = new StreamWriter(filename);        // open the file for writing

            for (int i = 0; i != g.Settings.Size; i++)          // loop through all of the settings in container
            {
                ImGuiIniData settings = g.Settings[i];          // grab a settings object
                if (settings.Pos.x == 3.402823466e+38F)         // if the position is some max value
                    continue;                                   // skip it

                string name      = settings.Name,               // get the name value
                       delimiter = "###";                       // delimiter for IDs with same name

                if (name.Contains(delimiter))                   // name contains delimiter
                {
                    string[] substrs =                          // save the substrings 
                        name.Split(new[] { delimiter }, 0);     // without the delimiter
                    name = "";                                  // clear the name

                    for (int n = 0; n < substrs.Length; n++)    // loop through the substrings and combine them
                    {
                        if (n == 0) name += substrs[n];         // the first substring doesn't need a preceding whitespace
                        else name += " " + substrs[n];          // all other substrings need a preceding whitespace
                    }
                }

                f.WriteLine(string.Format("[{0}]", name));      // write name value to file
                f.WriteLine(string.Format("Pos={0},{1}",        // write position value to file
                    (int)settings.Pos.x, (int)settings.Pos.y));
                f.WriteLine(string.Format("Size={0},{1}",       // write size value to file
                    (int)settings.Size.x, (int)settings.Size.y));
                f.WriteLine(string.Format("Collapsed={0}",      // write collasped value to file
                    settings.Collapsed));
                f.WriteLine();                                  // separate entries with a blank line
            }

            f.Close();                                          // close the file

            //TODO: SaveSettings()
            //        ImGuiState & g = *GImGui;

            //        const char* filename = g.IO.IniFilename;
            //        if (!filename)
            //            return;

            //        // Gather data from windows that were active during this session
            //        for (int i = 0; i != g.Windows.Size; i++)
            //        {
            //            ImGuiWindow* window = g.Windows[i];
            //            if (window->Flags & ImGuiWindowFlags_NoSavedSettings)
            //                continue;
            //            ImGuiIniData* settings = FindWindowSettings(window->Name);
            //            settings->Pos = window->Pos;
            //            settings->Size = window->SizeFull;
            //            settings->Collapsed = window->Collapsed;
            //        }

            //        // Write .ini file
            //        // If a window wasn't opened in this session we preserve its settings
            //        FILE* f = fopen(filename, "wt");
            //        if (!f)
            //            return;
            //        for (int i = 0; i != g.Settings.Size; i++)
            //        {
            //            const ImGuiIniData* settings = &g.Settings[i];
            //            if (settings->Pos.x == FLT_MAX)
            //                continue;
            //            const char* name = settings->Name;
            //            if (const char* p = strstr(name, "###"))  // Skip to the "###" marker if any. We don't skip past to match the behavior of GetID()
            //        name = p;
            //        fprintf(f, "[%s]\n", name);
            //        fprintf(f, "Pos=%d,%d\n", (int)settings->Pos.x, (int)settings->Pos.y);
            //        fprintf(f, "Size=%d,%d\n", (int)settings->Size.x, (int)settings->Size.y);
            //        fprintf(f, "Collapsed=%d\n", settings->Collapsed);
            //        fprintf(f, "\n");
            //    }

            //fclose(f);
        }

        void RenderTextClipped(ImVec2 pos_min, ImVec2 pos_max, string data, int text, int text_end = -1, ImVec2? text_size_if_known = null, ImGuiAlign align = ImGuiAlign.ImGuiAlign_Default, ImVec2? clip_min = null, ImVec2? clip_max = null)
        {
            // Hide anything after a '##' string
            int text_display_end = FindRenderedTextEnd(data, text, text_end);
            int text_len = (int)(text_display_end - text);
            if (text_len == 0)
                return;

            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            // Perform CPU side clipping for single clipped element to avoid using scissor state
            ImVec2 pos = pos_min;
            ImVec2 text_size = text_size_if_known.HasValue ? text_size_if_known.Value : CalcTextSize(data, text, text_display_end, false, 0.0f);

            if (!clip_max.HasValue)
                clip_max = pos_max;
            bool need_clipping = (pos.x + text_size.x >= clip_max.Value.x) || (pos.y + text_size.y >= clip_max.Value.y);
            if (!clip_min.HasValue)
                clip_min = pos_min;
            else
                need_clipping |= (pos.x < clip_min.Value.x) || (pos.y < clip_min.Value.y);

            // Align
            if ((align & ImGuiAlign.ImGuiAlign_Center) != 0) pos.x = Max(pos.x, (pos.x + pos_max.x - text_size.x) * 0.5f);
            else if ((align & ImGuiAlign.ImGuiAlign_Right) != 0) pos.x = Max(pos.x, pos_max.x - text_size.x);
            if ((align & ImGuiAlign.ImGuiAlign_VCenter) != 0) pos.y = Max(pos.y, (pos.y + pos_max.y - text_size.y) * 0.5f);

            // Render
            if (need_clipping)
            {
                ImVec4 fine_clip_rect = new ImVec4(clip_min.Value.x, clip_min.Value.y, clip_max.Value.x, clip_max.Value.y);
                window.DrawList.AddText(g.Font, g.FontSize, pos, GetColorU32(ImGuiCol.ImGuiCol_Text), data, text, text_display_end, 0.0f, fine_clip_rect);
            }
            else
            {
                window.DrawList.AddText(g.Font, g.FontSize, pos, GetColorU32(ImGuiCol.ImGuiCol_Text), data, text, text_display_end, 0.0f, null);
            }
            //TODO: if (g.LogEnabled)
            //    LogRenderedText(pos, text, text_display_end);
        }

        // Render a triangle to denote expanded/collapsed state
        void RenderCollapseTriangle(ImVec2 p_min, bool opened, float scale = 1f, bool shadow = false)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            float h = g.FontSize * 1.00f;
            float r = h * 0.40f * scale;
            ImVec2 center = p_min + new ImVec2(h * 0.50f, h * 0.50f * scale);

            ImVec2 a, b, c;
            if (opened)
            {
                center.y -= r * 0.25f;
                a = center + new ImVec2(0, 1) * r;
                b = center + new ImVec2(-0.866f, -0.5f) * r;
                c = center + new ImVec2(0.866f, -0.5f) * r;
            }
            else
            {
                a = center + new ImVec2(1, 0) * r;
                b = center + new ImVec2(-0.500f, 0.866f) * r;
                c = center + new ImVec2(-0.500f, -0.866f) * r;
            }

            if (shadow && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) != 0)
                window.DrawList.AddTriangleFilled(a + new ImVec2(2, 2), b + new ImVec2(2, 2), c + new ImVec2(2, 2), GetColorU32(ImGuiCol.ImGuiCol_BorderShadow));
            window.DrawList.AddTriangleFilled(a, b, c, GetColorU32(ImGuiCol.ImGuiCol_Text));
        }

        // Vertical scrollbar
        // The entire piece of code below is rather confusing because:
        // - We handle absolute seeking (when first clicking outside the grab) and relative manipulation (afterward or when clicking inside the grab)
        // - We store values as normalized ratio and in a form that allows the window content to change while we are holding on a scrollbar
        // - We handle both horizontal and vertical scrollbars, which makes the terminology not ideal.
        void Scrollbar(ImGuiWindow window, bool horizontal)
        {
            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(horizontal ? "#SCROLLX" : "#SCROLLY");

            // Render background
            bool other_scrollbar = (horizontal ? window.ScrollbarY : window.ScrollbarX);
            float other_scrollbar_size_w = other_scrollbar ? style.ScrollbarSize : 0.0f;
            ImRect window_rect = window.Rect();
            float border_size = window.BorderSize;
            ImRect bb = horizontal
                ? new ImRect(window.Pos.x + border_size, window_rect.Max.y - style.ScrollbarSize, window_rect.Max.x - other_scrollbar_size_w - border_size, window_rect.Max.y - border_size)
                : new ImRect(window_rect.Max.x - style.ScrollbarSize, window.Pos.y + border_size, window_rect.Max.x - border_size, window_rect.Max.y - other_scrollbar_size_w - border_size);
            if (!horizontal)
                bb.Min.y += window.TitleBarHeight() + ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_MenuBar) != 0 ? window.MenuBarHeight() - border_size : 0.0f);

            float window_rounding = (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0 ? style.ChildWindowRounding : style.WindowRounding;
            int window_rounding_corners;
            if (horizontal)
                window_rounding_corners = 8 | (other_scrollbar ? 0 : 4);
            else
                window_rounding_corners = ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) != 0 ? 2 : 0) | (other_scrollbar ? 0 : 4);
            window.DrawList.AddRectFilled(bb.Min, bb.Max, GetColorU32(ImGuiCol.ImGuiCol_ScrollbarBg), window_rounding, window_rounding_corners);
            bb.Reduce(new ImVec2(Clamp((float)(int)((bb.Max.x - bb.Min.x - 2.0f) * 0.5f), 0.0f, 3.0f), Clamp((float)(int)((bb.Max.y - bb.Min.y - 2.0f) * 0.5f), 0.0f, 3.0f)));

            // V denote the main axis of the scrollbar
            float scrollbar_size_v = horizontal ? bb.GetWidth() : bb.GetHeight();
            float scroll_v = horizontal ? window.Scroll.x : window.Scroll.y;
            float win_size_avail_v = (horizontal ? window.Size.x : window.Size.y) - other_scrollbar_size_w;
            float win_size_contents_v = horizontal ? window.SizeContents.x : window.SizeContents.y;

            // The grabable box size generally represent the amount visible (vs the total scrollable amount)
            // But we maintain a minimum size in pixel to allow for the user to still aim inside.
            float grab_h_pixels = Min(Max(scrollbar_size_v * Saturate(win_size_avail_v / Max(win_size_contents_v, win_size_avail_v)), style.GrabMinSize), scrollbar_size_v);
            float grab_h_norm = grab_h_pixels / scrollbar_size_v;

            // Handle input right away. None of the code of Begin() is relying on scrolling position before calling Scrollbar().
            bool? held = false;
            bool? hovered = false;
            bool previously_held = (g.ActiveId == id);
            ButtonBehavior(bb, id, ref hovered, ref held);

            float scroll_max = Max(1.0f, win_size_contents_v - win_size_avail_v);
            float scroll_ratio = Saturate(scroll_v / scroll_max);
            float grab_v_norm = scroll_ratio * (scrollbar_size_v - grab_h_pixels) / scrollbar_size_v;
            if (held.Value && grab_h_norm < 1.0f)
            {
                float scrollbar_pos_v = horizontal ? bb.Min.x : bb.Min.y;
                float mouse_pos_v = horizontal ? g.IO.MousePos.x : g.IO.MousePos.y;
                float click_delta_to_grab_center_v = horizontal ? g.ScrollbarClickDeltaToGrabCenter.x : g.ScrollbarClickDeltaToGrabCenter.y;

                // Click position in scrollbar normalized space (0.0f.1.0f)
                float clicked_v_norm = Saturate((mouse_pos_v - scrollbar_pos_v) / scrollbar_size_v);
                SetHoveredID(id);

                bool seek_absolute = false;
                if (!previously_held)
                {
                    // On initial click calculate the distance between mouse and the center of the grab
                    if (clicked_v_norm >= grab_v_norm && clicked_v_norm <= grab_v_norm + grab_h_norm)
                    {
                        click_delta_to_grab_center_v = clicked_v_norm - grab_v_norm - grab_h_norm * 0.5f;
                    }
                    else
                    {
                        seek_absolute = true;
                        click_delta_to_grab_center_v = 0.0f;
                    }
                }

                // Apply scroll
                // It is ok to modify Scroll here because we are being called in Begin() after the calculation of SizeContents and before setting up our starting position
                float scroll_v_norm = Saturate((clicked_v_norm - click_delta_to_grab_center_v - grab_h_norm * 0.5f) / (1.0f - grab_h_norm));
                scroll_v = (float)(int)(0.5f + scroll_v_norm * scroll_max);//(win_size_contents_v - win_size_v));
                if (horizontal)
                    window.Scroll.x = scroll_v;
                else
                    window.Scroll.y = scroll_v;

                // Update values for rendering
                scroll_ratio = Saturate(scroll_v / scroll_max);
                grab_v_norm = scroll_ratio * (scrollbar_size_v - grab_h_pixels) / scrollbar_size_v;

                // Update distance to grab now that we have seeked and saturated
                if (seek_absolute)
                    click_delta_to_grab_center_v = clicked_v_norm - grab_v_norm - grab_h_norm * 0.5f;

                if (horizontal)
                    g.ScrollbarClickDeltaToGrabCenter.x = click_delta_to_grab_center_v;
                else
                    g.ScrollbarClickDeltaToGrabCenter.y = click_delta_to_grab_center_v;
            }

            // Render
            uint grab_col = GetColorU32(held.Value ? ImGuiCol.ImGuiCol_ScrollbarGrabActive : hovered.Value ? ImGuiCol.ImGuiCol_ScrollbarGrabHovered : ImGuiCol.ImGuiCol_ScrollbarGrab);
            if (horizontal)
                window.DrawList.AddRectFilled(new ImVec2(Lerp(bb.Min.x, bb.Max.x, grab_v_norm), bb.Min.y), new ImVec2(Lerp(bb.Min.x, bb.Max.x, grab_v_norm) + grab_h_pixels, bb.Max.y), grab_col, style.ScrollbarRounding);
            else
                window.DrawList.AddRectFilled(new ImVec2(bb.Min.x, Lerp(bb.Min.y, bb.Max.y, grab_v_norm)), new ImVec2(bb.Max.x, Lerp(bb.Min.y, bb.Max.y, grab_v_norm) + grab_h_pixels), grab_col, style.ScrollbarRounding);
        }

        // Render a rectangle shaped with optional rounding and borders
        void RenderFrame(ImVec2 p_min, ImVec2 p_max, uint fill_col, bool border = true, float rounding = 0.0f)
        {
            ImGuiWindow window = GetCurrentWindow();

            window.DrawList.AddRectFilled(p_min, p_max, fill_col, rounding);
            if (border && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) != 0)
            {
                window.DrawList.AddRect(p_min + new ImVec2(1, 1), p_max + new ImVec2(1, 1), GetColorU32(ImGuiCol.ImGuiCol_BorderShadow), rounding);
                window.DrawList.AddRect(p_min, p_max, GetColorU32(ImGuiCol.ImGuiCol_Border), rounding);
            }
        }

        // This is normally called by Render(). You may want to call it directly if you want to avoid calling Render() but the gain will be very minimal.
        void EndFrame()
        {
            ImGuiState g = State;
            System.Diagnostics.Debug.Assert(g.Initialized);                       // Forgot to call NewFrame()
            System.Diagnostics.Debug.Assert(g.FrameCountEnded != g.FrameCount);   // EndFrame() called multiple times, or forgot to call NewFrame() again

            // Render tooltip
            if (!string.IsNullOrEmpty(g.Tooltip))
            {
                BeginTooltip();
                TextUnformatted(g.Tooltip);
                EndTooltip();
            }

            // Notify OS when our Input Method Editor cursor has moved (e.g. CJK inputs using Microsoft IME)
            if (g.IO.ImeSetInputScreenPosFn != null && LengthSqr(g.OsImePosRequest - g.OsImePosSet) > 0.0001f)
            {
                g.IO.ImeSetInputScreenPosFn((int)g.OsImePosRequest.x, (int)g.OsImePosRequest.y);
                g.OsImePosSet = g.OsImePosRequest;
            }

            // Hide implicit "Debug" window if it hasn't been used
            System.Diagnostics.Debug.Assert(g.CurrentWindowStack.Size == 1);    // Mismatched Begin()/End() calls
            if (g.CurrentWindow != null && !g.CurrentWindow.Accessed)
                g.CurrentWindow.Active = false;
            End();

            // Click to focus window and start moving (after we're done with all our widgets)
            if (g.ActiveId == 0)
                g.MovedWindow = null;
            if (g.ActiveId == 0 && g.HoveredId == 0 && g.IO.MouseClicked[0])
            {
                if (!(g.FocusedWindow != null && !g.FocusedWindow.WasActive && g.FocusedWindow.Active)) // Unless we just made a popup appear
                {
                    if (g.HoveredRootWindow != null)
                    {
                        FocusWindow(g.HoveredWindow);
                        if ((g.HoveredWindow.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoMove) == 0)
                        {
                            g.MovedWindow = g.HoveredWindow;
                            SetActiveID(g.HoveredRootWindow.MoveID, g.HoveredRootWindow);
                        }
                    }
                    else if (g.FocusedWindow != null && GetFrontMostModalRootWindow() == null)
                    {
                        // Clicking on void disable focus
                        FocusWindow(null);
                    }
                }
            }

            // Sort the window list so that all child windows are after their parent
            // We cannot do that on FocusWindow() because childs may not exist yet
            g.WindowsSortBuffer.resize(0);
            g.WindowsSortBuffer.reserve(g.Windows.Size);
            for (int i = 0; i != g.Windows.Size; i++)
            {
                ImGuiWindow window = g.Windows[i];
                if (window.Active && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0)       // if a child is active its parent will add it
                    continue;
                AddWindowToSortedBuffer(g.WindowsSortBuffer, window);
            }
            System.Diagnostics.Debug.Assert(g.Windows.Size == g.WindowsSortBuffer.Size);  // we done something wrong
            g.Windows.swap(g.WindowsSortBuffer);

            // Clear Input data for next frame
            g.IO.MouseWheel = 0.0f;
            //TODO: memset(g.IO.InputCharacters, 0, sizeof(g.IO.InputCharacters));
            for (var i = 0; i < g.IO.InputCharacters.Length; i++)
                g.IO.InputCharacters[0] = (char)0;

            g.FrameCountEnded = g.FrameCount;
        }

        internal void Render()
        {
            ImGuiState g = State;
            System.Diagnostics.Debug.Assert(g.Initialized);   // Forgot to call NewFrame()

            if (g.FrameCountEnded != g.FrameCount)
                EndFrame();
            g.FrameCountRendered = g.FrameCount;

            // Skip render altogether if alpha is 0.0
            // Note that vertex buffers have been created and are wasted, so it is best practice that you don't create windows in the first place, or consistently respond to Begin() returning false.
            if (g.Style.Alpha > 0.0f)
            {
                // Gather windows to render
                g.IO.MetricsRenderVertices = g.IO.MetricsRenderIndices = g.IO.MetricsActiveWindows = 0;
                for (int i = 0; i < g.RenderDrawLists.Length; i++)
                    g.RenderDrawLists[i].resize(0);
                for (int i = 0; i != g.Windows.Size; i++)
                {
                    ImGuiWindow window = g.Windows[i];
                    if (window.Active && window.HiddenFrames <= 0 && (window.Flags & (ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow)) == 0)
                    {
                        // FIXME: Generalize this with a proper layering system so e.g. user can draw in specific layers, below text, ..
                        g.IO.MetricsActiveWindows++;
                        if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Popup) != 0)
                            AddWindowToRenderList(g.RenderDrawLists[1], window);
                        else if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) != 0)
                            AddWindowToRenderList(g.RenderDrawLists[2], window);
                        else
                            AddWindowToRenderList(g.RenderDrawLists[0], window);
                    }
                }

                // Flatten layers
                int n = g.RenderDrawLists[0].Size;
                int flattened_size = n;
                for (int i = 1; i < g.RenderDrawLists.Length; i++)
                    flattened_size += g.RenderDrawLists[i].Size;
                g.RenderDrawLists[0].resize(flattened_size);
                for (int i = 1; i < g.RenderDrawLists.Length; i++)
                {
                    ImVector<ImDrawList> layer = g.RenderDrawLists[i];
                    if (layer.empty())
                        continue;
                    for (var k = 0; k < layer.Size; k++)
                        g.RenderDrawLists[0][n++] = layer[k];
                    //TODO: Validate -- memcpy(&g.RenderDrawLists[0][n], &layer[0], layer.Size * sizeof(ImDrawList*));
                    //n += layer.Size;
                }

                // Draw software mouse cursor if requested
                if (g.IO.MouseDrawCursor)
                {
                    ImGuiMouseCursorData cursor_data = g.MouseCursorData[(int)g.MouseCursor];
                    ImVec2 pos = g.IO.MousePos - cursor_data.HotOffset;
                    ImVec2 size = cursor_data.Size;
                    ImTextureID tex_id = g.IO.Fonts.TexID;
                    g.OverlayDrawList.PushTextureID(tex_id);
                    g.OverlayDrawList.AddImage(tex_id, pos + new ImVec2(1, 0), pos + new ImVec2(1, 0) + size, cursor_data.TexUvMin[1], cursor_data.TexUvMax[1], 0x30000000); // Shadow
                    g.OverlayDrawList.AddImage(tex_id, pos + new ImVec2(2, 0), pos + new ImVec2(2, 0) + size, cursor_data.TexUvMin[1], cursor_data.TexUvMax[1], 0x30000000); // Shadow
                    g.OverlayDrawList.AddImage(tex_id, pos, pos + size, cursor_data.TexUvMin[1], cursor_data.TexUvMax[1], 0xFF000000); // Black border
                    g.OverlayDrawList.AddImage(tex_id, pos, pos + size, cursor_data.TexUvMin[0], cursor_data.TexUvMax[0], 0xFFFFFFFF); // White fill
                    g.OverlayDrawList.PopTextureID();
                }
                if (!g.OverlayDrawList.VtxBuffer.empty())
                    AddDrawListToRenderList(g.RenderDrawLists[0], g.OverlayDrawList);

                // Setup draw data
                g.RenderDrawData.Valid = true;
                g.RenderDrawData.CmdLists = (g.RenderDrawLists[0].Size > 0) ? g.RenderDrawLists[0] : null;
                g.RenderDrawData.CmdListsCount = g.RenderDrawLists[0].Size;
                g.RenderDrawData.TotalVtxCount = g.IO.MetricsRenderVertices;
                g.RenderDrawData.TotalIdxCount = g.IO.MetricsRenderIndices;

                // Render. If user hasn't set a callback then they may retrieve the draw data via GetDrawData()
                if (g.RenderDrawData.CmdListsCount > 0 && g.IO.RenderDrawListsFn != null)
                    g.IO.RenderDrawListsFn(g.RenderDrawData);
            }
        }

        void BeginTooltip()
        {
            bool throwAway = true;
            ImGuiState g = State;
            ImGuiWindowFlags flags = ImGuiWindowFlags.ImGuiWindowFlags_Tooltip | ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags.ImGuiWindowFlags_NoMove | ImGuiWindowFlags.ImGuiWindowFlags_NoResize | ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings | ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize;
            BeginEx("##Tooltip", ref throwAway, new ImVec2(0, 0), g.Style.Colors[(int)ImGuiCol.ImGuiCol_TooltipBg].w, flags, false);
        }

        void EndTooltip()
        {
            System.Diagnostics.Debug.Assert((GetCurrentWindowRead().Flags & ImGuiWindowFlags.ImGuiWindowFlags_Tooltip) != 0);   // Mismatched BeginTooltip()/EndTooltip() calls
            End();
        }

        void TextUnformatted(string data, int text = 0, int text_end = -1)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            if (string.IsNullOrEmpty(data))
                return;

            ImGuiState g = State;
            //TODO: Check - System.Diagnostics.Debug.Assert(data != null);
            int text_begin = text;
            if (text_end == -1)
                text_end = data.Length; // FIXME-OPT

            float wrap_pos_x = window.DC.TextWrapPos;
            bool wrap_enabled = wrap_pos_x >= 0.0f;
            if (text_end - text > 2000 && !wrap_enabled)
            {
                // Long text!
                // Perform manual coarse clipping to optimize for long multi-line text
                // From this point we will only compute the width of lines that are visible. Optimization only available when word-wrapping is disabled.
                // We also don't vertically center the text within the line full height, which is unlikely to matter because we are likely the biggest and only item on the line.
                int line = text;
                float line_height = GetTextLineHeight();
                ImVec2 text_pos = window.DC.CursorPos + new ImVec2(0.0f, window.DC.CurrentLineTextBaseOffset);
                ImRect clip_rect = window.ClipRect;
                ImVec2 text_size = new ImVec2(0, 0);

                if (text_pos.y <= clip_rect.Max.y)
                {
                    ImVec2 pos = text_pos;

                    // Lines to skip (can't skip when logging text)
                    if (!g.LogEnabled)
                    {
                        int lines_skippable = (int)((clip_rect.Min.y - text_pos.y) / line_height);
                        if (lines_skippable > 0)
                        {
                            int lines_skipped = 0;
                            while (line < text_end && lines_skipped < lines_skippable)
                            {
                                int line_end = data.IndexOf('\n', line);
                                if (line_end == -1)
                                    line_end = text_end;
                                line = line_end + 1;
                                lines_skipped++;
                            }
                            pos.y += lines_skipped * line_height;
                        }
                    }

                    // Lines to render
                    if (line < text_end)
                    {
                        ImRect line_rect = new ImRect(pos, pos + new ImVec2(GetWindowWidth(), line_height));
                        while (line < text_end)
                        {
                            int line_end = data.IndexOf('\n', line);
                            if (IsClippedEx(line_rect, null, false))
                                break;

                            ImVec2 line_size = CalcTextSize(data, line, line_end, false);
                            text_size.x = Max(text_size.x, line_size.x);
                            RenderText(pos, data, line, line_end, false);
                            if (line_end == -1)
                                line_end = text_end;
                            line = line_end + 1;
                            line_rect.Min.y += line_height;
                            line_rect.Max.y += line_height;
                            pos.y += line_height;
                        }

                        // Count remaining lines
                        int lines_skipped = 0;
                        while (line < text_end)
                        {
                            int line_end = data.IndexOf('\n', line);
                            if (line_end == -1)
                                line_end = text_end;
                            line = line_end + 1;
                            lines_skipped++;
                        }
                        pos.y += lines_skipped * line_height;
                    }

                    text_size.y += (pos - text_pos).y;
                }

                ImRect bb = new ImRect(text_pos, text_pos + text_size);
                ItemSize(bb);
                ItemAdd(bb, null);
            }
            else
            {
                float wrap_width = wrap_enabled ? CalcWrapWidthForPos(window.DC.CursorPos, wrap_pos_x) : 0.0f;
                ImVec2 text_size = CalcTextSize(data, text_begin, text_end, false, wrap_width);

                // Account of baseline offset
                ImVec2 text_pos = window.DC.CursorPos;
                text_pos.y += window.DC.CurrentLineTextBaseOffset;

                ImRect bb = new ImRect(text_pos, text_pos + text_size);
                ItemSize(text_size);
                if (!ItemAdd(bb, null))
                    return;

                // Render (we don't hide text after ## in this end-user function)
                RenderTextWrapped(bb.Min, data, text_begin, text_end, wrap_width);
            }
        }

        // Internal ImGui functions to render text
        // RenderText***() functions calls ImDrawList::AddText() calls ImBitmapFont::RenderText()
        void RenderText(ImVec2 pos, string data, int text = 0, int text_end = -1, bool hide_text_after_hash = true)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            // Hide anything after a '##' string
            int text_display_end;
            if (hide_text_after_hash)
            {
                text_display_end = FindRenderedTextEnd(data, text, text_end);
            }
            else
            {
                if (text_end == -1)
                    text_end = data.Length; // FIXME-OPT
                text_display_end = text_end;
            }

            int text_len = (text_display_end - text);
            if (text_len > 0)
            {
                window.DrawList.AddText(g.Font, g.FontSize, pos, GetColorU32(ImGuiCol.ImGuiCol_Text), data, text, text_display_end);
                //TODO: if (g.LogEnabled)
                //    LogRenderedText(pos, text, text_display_end);
            }
        }

        void RenderTextWrapped(ImVec2 pos, string data, int text, int text_end, float wrap_width)
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();

            if (text_end == -1)
                text_end = data.Length; // FIXME-OPT

            int text_len = (text_end - text);
            if (text_len > 0)
            {
                window.DrawList.AddText(g.Font, g.FontSize, pos, GetColorU32(ImGuiCol.ImGuiCol_Text), data, text, text_end, wrap_width);
                //TODO: if (g.LogEnabled)
                //    LogRenderedText(pos, text, text_end);
            }
        }

        //// Handle clipping on CPU immediately (vs typically let the GPU clip the triangles that are overlapping the clipping rectangle edges)
        //void RenderTextClipped(ImVec2 pos_min, ImVec2 pos_max, string data, int text, int text_end, ImVec2* text_size_if_known, ImGuiAlign align, ImVec2? clip_min, ImVec2? clip_max)
        //{
        //    // Hide anything after a '##' string
        //    char* text_display_end = FindRenderedTextEnd(text, text_end);
        //    int text_len = (int)(text_display_end - text);
        //    if (text_len == 0)
        //        return;

        //    ImGuiState & g = *GImGui;
        //    ImGuiWindow* window = GetCurrentWindow();

        //    // Perform CPU side clipping for single clipped element to avoid using scissor state
        //    ImVec2 pos = pos_min;
        //    ImVec2 text_size = text_size_if_known ? *text_size_if_known : CalcTextSize(text, text_display_end, false, 0.0f);

        //    if (!clip_max) clip_max = &pos_max;
        //    bool need_clipping = (pos.x + text_size.x >= clip_max.x) || (pos.y + text_size.y >= clip_max.y);
        //    if (!clip_min) clip_min = &pos_min; else need_clipping |= (pos.x < clip_min.x) || (pos.y < clip_min.y);

        //    // Align
        //    if (align & ImGuiAlign_Center) pos.x = ImMax(pos.x, (pos.x + pos_max.x - text_size.x) * 0.5f);
        //    else if (align & ImGuiAlign_Right) pos.x = ImMax(pos.x, pos_max.x - text_size.x);
        //    if (align & ImGuiAlign_VCenter) pos.y = ImMax(pos.y, (pos.y + pos_max.y - text_size.y) * 0.5f);

        //    // Render
        //    if (need_clipping)
        //    {
        //        ImVec4 fine_clip_rect(clip_min.x, clip_min.y, clip_max.x, clip_max.y);
        //        window.DrawList.AddText(g.Font, g.FontSize, pos, GetColorU32(ImGuiCol_Text), text, text_display_end, 0.0f, &fine_clip_rect);
        //    }
        //    else
        //    {
        //        window.DrawList.AddText(g.Font, g.FontSize, pos, GetColorU32(ImGuiCol_Text), text, text_display_end, 0.0f, NULL);
        //    }
        //    if (g.LogEnabled)
        //        LogRenderedText(pos, text, text_display_end);
        //}

        public void Text(string text, params object[] args)
        {
            if (args != null || args.Length > 0)
                text = string.Format(text, args);

            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            //int text_end = g.TempBuffer + ImFormatStringV(g.TempBuffer, IM_ARRAYSIZE(g.TempBuffer), fmt, args);
            TextUnformatted(text);
        }

        void AlignFirstTextHeightToWidgets()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // Declare a dummy item size to that upcoming items that are smaller will center-align on the newly expanded line height.
            ImGuiState g = State; ;
            ItemSize(new ImVec2(0, g.FontSize + g.Style.FramePadding.y * 2), g.Style.FramePadding.y);
            SameLine(0, 0);
        }

        public void LabelText(string label, object val)
        {
            LabelText(label, val == null ? string.Empty : val.ToString(), null);
        }

        // Add a label+text combo aligned to other label+value widgets
        public void LabelText(string label, string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State; ;
            ImGuiStyle style = g.Style;
            float w = CalcItemWidth();

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImRect value_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(w, label_size.y + style.FramePadding.y * 2));
            ImRect total_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(w + (label_size.x > 0.0f ? style.ItemInnerSpacing.x : 0.0f), style.FramePadding.y * 2) + label_size);
            ItemSize(total_bb, style.FramePadding.y);
            if (!ItemAdd(total_bb, null))
                return;

            // Render
            //char* value_text_begin = &g.TempBuffer[0];
            //char* value_text_end = value_text_begin + ImFormatStringV(g.TempBuffer, IM_ARRAYSIZE(g.TempBuffer), fmt, args);
            RenderTextClipped(value_bb.Min, value_bb.Max, text, 0, -1, null, ImGuiAlign.ImGuiAlign_VCenter);
            if (label_size.x > 0.0f)
                RenderText(new ImVec2(value_bb.Max.x + style.ItemInnerSpacing.x, value_bb.Min.y + style.FramePadding.y), label);
        }


        bool ButtonEx(string label, ImVec2 size_arg, ImGuiButtonFlags flags)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State; ;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);

            ImVec2 pos = window.DC.CursorPos;
            if ((flags & ImGuiButtonFlags.ImGuiButtonFlags_AlignTextBaseLine) != 0 && style.FramePadding.y < window.DC.CurrentLineTextBaseOffset)
                pos.y += window.DC.CurrentLineTextBaseOffset - style.FramePadding.y;
            ImVec2 size = CalcItemSize(size_arg, label_size.x + style.FramePadding.x * 2.0f, label_size.y + style.FramePadding.y * 2.0f);

            ImRect bb = new ImRect(pos, pos + size);
            ItemSize(bb, style.FramePadding.y);
            if (!ItemAdd(bb, id))
                return false;

            if (window.DC.ButtonRepeat) flags |= ImGuiButtonFlags.ImGuiButtonFlags_Repeat;
            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(bb, id, ref hovered, ref held, flags);

            // Render
            uint col = GetColorU32((hovered.Value && held.Value) ? ImGuiCol.ImGuiCol_ButtonActive : hovered.Value ? ImGuiCol.ImGuiCol_ButtonHovered : ImGuiCol.ImGuiCol_Button);
            RenderFrame(bb.Min, bb.Max, col, true, style.FrameRounding);
            RenderTextClipped(bb.Min, bb.Max, label, 0, -1, label_size, ImGuiAlign.ImGuiAlign_Center | ImGuiAlign.ImGuiAlign_VCenter);

            // Automatically close popups
            //if (pressed && !(flags & ImGuiButtonFlags_DontClosePopups) && (window.Flags & ImGuiWindowFlags_Popup))
            //    CloseCurrentPopup();

            return pressed;
        }

        public bool Button(string label, ImVec2? _size_arg = null)
        {
            var size_arg = _size_arg.HasValue ? _size_arg.Value : ImVec2.Zero;
            return ButtonEx(label, size_arg, 0);
        }


        // Small buttons fits within text without additional vertical spacing.
        bool SmallButton(string label)
        {
            ImGuiState g = State; ;
            float backup_padding_y = g.Style.FramePadding.y;
            g.Style.FramePadding.y = 0.0f;
            bool pressed = ButtonEx(label, new ImVec2(0, 0), ImGuiButtonFlags.ImGuiButtonFlags_AlignTextBaseLine);
            g.Style.FramePadding.y = backup_padding_y;
            return pressed;
        }

        // Tip: use PushID()/PopID() to push indices or pointers in the ID stack.
        // Then you can keep 'str_id' empty or the same for all your buttons (instead of creating a string based on a non-string id)
        bool InvisibleButton(string str_id, ImVec2 size_arg)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            uint id = window.GetID(str_id);
            ImVec2 size = CalcItemSize(size_arg, 0.0f, 0.0f);
            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + size);
            ItemSize(bb);
            if (!ItemAdd(bb, id))
                return false;

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(bb, id, ref hovered, ref held);

            return pressed;
        }

        void TextColored(ImVec4 col, string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            PushStyleColor(ImGuiCol.ImGuiCol_Text, col);
            TextUnformatted(text);
            PopStyleColor();
        }

        void TextDisabled(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            PushStyleColor(ImGuiCol.ImGuiCol_Text, State.Style.Colors[(int)ImGuiCol.ImGuiCol_TextDisabled]);
            TextUnformatted(text);
            PopStyleColor();
        }

        void TextWrapped(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            PushTextWrapPos(0.0f);
            TextUnformatted(text);
            PopTextWrapPos();
        }

        void Bullet()
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            float line_height = Max(Min(window.DC.CurrentLineHeight, g.FontSize + g.Style.FramePadding.y * 2), g.FontSize);
            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(g.FontSize, line_height));
            ItemSize(bb);
            if (!ItemAdd(bb, null))
            {
                SameLine(0, style.FramePadding.x * 2);
                return;
            }

            // Render
            float bullet_size = g.FontSize * 0.15f;
            window.DrawList.AddCircleFilled(bb.Min + new ImVec2(style.FramePadding.x + g.FontSize * 0.5f, line_height * 0.5f), bullet_size, GetColorU32(ImGuiCol.ImGuiCol_Text));

            // Stay on same line
            SameLine(0, style.FramePadding.x * 2);
        }

        // Text with a little bullet aligned to the typical tree node.
        void BulletText(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;

            ImVec2 label_size = CalcTextSize(text, 0, -1, true);
            float text_base_offset_y = Max(0.0f, window.DC.CurrentLineTextBaseOffset); // Latch before ItemSize changes it
            float line_height = Max(Min(window.DC.CurrentLineHeight, g.FontSize + g.Style.FramePadding.y * 2), g.FontSize);
            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(g.FontSize + (label_size.x > 0.0f ? (label_size.x + style.FramePadding.x * 2) : 0.0f), Max(line_height, label_size.y)));  // Empty text doesn't add padding
            ItemSize(bb);
            if (!ItemAdd(bb, null))
                return;

            // Render
            float bullet_size = g.FontSize * 0.15f;
            window.DrawList.AddCircleFilled(bb.Min + new ImVec2(style.FramePadding.x + g.FontSize * 0.5f, line_height * 0.5f), bullet_size, GetColorU32(ImGuiCol.ImGuiCol_Text));
            RenderText(bb.Min + new ImVec2(g.FontSize + style.FramePadding.x * 2, text_base_offset_y), text);
        }

        // If returning 'true' the node is open and the user is responsible for calling TreePop
        bool TreeNode(string str_id, string text, params object[] args)
        {
            if (str_id == null)
                str_id = text;

            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;

            PushID(str_id);
            bool opened = CollapsingHeader(text, "", false);
            PopID();

            if (opened)
                TreePush(str_id);

            return opened;
        }

        //// If returning 'true' the node is open and the user is responsible for calling TreePop
        //bool TreeNode(string ptr_id, string text, params object[] args)
        //{
        //    if (ptr_id == null)
        //        ptr_id = text;

        //    if (args != null && args.Length > 0)
        //        text = string.Format(text, args);

        //    ImGuiWindow window = GetCurrentWindow();
        //    if (window.SkipItems)
        //        return false;

        //    ImGuiState g = State;

        //    PushID(ptr_id);
        //    bool opened = CollapsingHeader(g.TempBuffer, "", false);
        //    PopID();

        //    if (opened)
        //        TreePush(ptr_id);

        //    return opened;
        //}

        bool TreeNode(string str_label_id)
        {
            return TreeNode(str_label_id, str_label_id);
        }

        void SetNextTreeNodeOpened(bool opened, ImGuiSetCond cond)
        {
            ImGuiState g = State;
            g.SetNextTreeNodeOpenedVal = opened;
            g.SetNextTreeNodeOpenedCond = cond != 0 ? cond : ImGuiSetCond.ImGuiSetCond_Always;
        }

        bool TreeNodeBehaviorIsOpened(uint id, ImGuiTreeNodeFlags flags)
        {
            // We only write to the tree storage if the user clicks (or explicitely use SetNextTreeNode*** functions)
            ImGuiState g = State;
            ImGuiWindow window = g.CurrentWindow;
            ImGuiStorage storage = window.DC.StateStorage;

            bool opened;
            if (g.SetNextTreeNodeOpenedCond != 0)
            {
                if ((g.SetNextTreeNodeOpenedCond & ImGuiSetCond.ImGuiSetCond_Always) != 0)
                {
                    opened = g.SetNextTreeNodeOpenedVal;
                    storage.SetInt(id, opened ? 1 : 0);
                }
                else
                {
                    // We treat ImGuiSetCondition_Once and ImGuiSetCondition_FirstUseEver the same because tree node state are not saved persistently.
                    int stored_value = storage.GetInt(id, -1);
                    if (stored_value == -1)
                    {
                        opened = g.SetNextTreeNodeOpenedVal;
                        storage.SetInt(id, opened ? 1 : 0);
                    }
                    else
                    {
                        opened = stored_value != 0;
                    }
                }
                g.SetNextTreeNodeOpenedCond = 0;
            }
            else
            {
                opened = storage.GetInt(id, (flags & ImGuiTreeNodeFlags.ImGuiTreeNodeFlags_DefaultOpen) != 0 ? 1 : 0) != 0;
            }

            // When logging is enabled, we automatically expand tree nodes (but *NOT* collapsing headers.. seems like sensible behavior).
            // NB- If we are above max depth we still allow manually opened nodes to be logged.
            //TODO logging
            //if (g.LogEnabled && !(flags & ImGuiTreeNodeFlags_NoAutoExpandOnLog) && window->DC.TreeDepth < g.LogAutoExpandMaxDepth)
            //    opened = true;

            return opened;
        }

        // FIXME: Split into CollapsingHeader(label, default_open?) and TreeNodeBehavior(label), obsolete the 4 parameters function.
        bool CollapsingHeader(string label, string str_id = null, bool display_frame = true, bool default_open = false)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            ImVec2 padding = display_frame ? style.FramePadding : new ImVec2(style.FramePadding.x, 0.0f);

            System.Diagnostics.Debug.Assert(str_id != null || label != null);
            if (str_id == null)
                str_id = label;
            if (label == null)
                label = str_id;
            bool label_hide_text_after_double_hash = (label == str_id); // Only search and hide text after ## if we have passed label and ID separately, otherwise allow "##" within format string.
            uint id = window.GetID(str_id);
            ImVec2 label_size = CalcTextSize(label, 0, -1, label_hide_text_after_double_hash);

            // We vertically grow up to current line height up the typical widget height.
            float text_base_offset_y = Max(0.0f, window.DC.CurrentLineTextBaseOffset - padding.y); // Latch before ItemSize changes it
            float frame_height = Max(Min(window.DC.CurrentLineHeight, g.FontSize + g.Style.FramePadding.y * 2), label_size.y + padding.y * 2);
            ImRect bb = new ImRect(window.DC.CursorPos, new ImVec2(window.Pos.x + GetContentRegionMax().x, window.DC.CursorPos.y + frame_height));
            if (display_frame)
            {
                // Framed header expand a little outside the default padding
                bb.Min.x -= (float)(int)(window.WindowPadding.x * 0.5f) - 1;
                bb.Max.x += (float)(int)(window.WindowPadding.x * 0.5f) - 1;
            }

            float collapser_width = g.FontSize + (display_frame ? padding.x * 2 : padding.x);
            float text_width = g.FontSize + (label_size.x > 0.0f ? label_size.x + padding.x * 2 : 0.0f);   // Include collapser
            ItemSize(new ImVec2(text_width, frame_height), text_base_offset_y);

            // For regular tree nodes, we arbitrary allow to click past 2 worth of ItemSpacing
            // (Ideally we'd want to add a flag for the user to specify we want want the hit test to be done up to the right side of the content or not)
            ImRect interact_bb = display_frame ? bb : new ImRect(bb.Min.x, bb.Min.y, bb.Min.x + text_width + style.ItemSpacing.x * 2, bb.Max.y);
            bool opened = TreeNodeBehaviorIsOpened(id, (default_open ? ImGuiTreeNodeFlags.ImGuiTreeNodeFlags_DefaultOpen : 0) | (display_frame ? ImGuiTreeNodeFlags.ImGuiTreeNodeFlags_NoAutoExpandOnLog : 0));
            if (!ItemAdd(interact_bb, id))
                return opened;

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(interact_bb, id, ref hovered, ref held, ImGuiButtonFlags.ImGuiButtonFlags_NoKeyModifiers);
            if (pressed)
            {
                opened = !opened;
                window.DC.StateStorage.SetInt(id, opened ? 1 : 0);
            }

            // Render
            uint col = GetColorU32((held.Value && hovered.Value) ? ImGuiCol.ImGuiCol_HeaderActive : hovered.Value ? ImGuiCol.ImGuiCol_HeaderHovered : ImGuiCol.ImGuiCol_Header);
            ImVec2 text_pos = bb.Min + padding + new ImVec2(collapser_width, text_base_offset_y);
            if (display_frame)
            {
                // Framed type
                RenderFrame(bb.Min, bb.Max, col, true, style.FrameRounding);
                RenderCollapseTriangle(bb.Min + padding + new ImVec2(0.0f, text_base_offset_y), opened, 1.0f, true);
                //TODO: logging
                //if (g.LogEnabled)
                //{
                //    // NB: '##' is normally used to hide text (as a library-wide feature), so we need to specify the text range to make sure the ## aren't stripped out here.
                //    char log_prefix[] = "\n##";
                //    char log_suffix[] = "##";
                //    LogRenderedText(text_pos, log_prefix, log_prefix + 3);
                //    RenderTextClipped(text_pos, bb.Max, label, NULL, &label_size);
                //    LogRenderedText(text_pos, log_suffix + 1, log_suffix + 3);
                //}
                //else
                {
                    RenderTextClipped(text_pos, bb.Max, label, 0, -1, label_size);
                }
            }
            else
            {
                // Unframed typed for tree nodes
                if (hovered.Value)
                    RenderFrame(bb.Min, bb.Max, col, false);

                RenderCollapseTriangle(bb.Min + new ImVec2(padding.x, g.FontSize * 0.15f + text_base_offset_y), opened, 0.70f, false);
                //TODO logging
                //if (g.LogEnabled)
                //    LogRenderedText(text_pos, ">");
                RenderText(text_pos, label, 0, -1, label_hide_text_after_double_hash);
            }

            return opened;
        }

        void Indent()
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();
            window.DC.IndentX += g.Style.IndentSpacing;
            window.DC.CursorPos.x = window.Pos.x + window.DC.IndentX + window.DC.ColumnsOffsetX;
        }

        void Unindent()
        {
            ImGuiState g = State;
            ImGuiWindow window = GetCurrentWindow();
            window.DC.IndentX -= g.Style.IndentSpacing;
            window.DC.CursorPos.x = window.Pos.x + window.DC.IndentX + window.DC.ColumnsOffsetX;
        }

        void TreePush(string str_id)
        {
            ImGuiWindow window = GetCurrentWindow();
            Indent();
            window.DC.TreeDepth++;
            PushID(str_id != null ? str_id : "#TreePush");
        }

        //void TreePush(void* ptr_id)
        //{
        //    ImGuiWindow window = GetCurrentWindow();
        //    Indent();
        //    window.DC.TreeDepth++;
        //    PushID(ptr_id ? ptr_id : (void*)"#TreePush");
        //}

        void TreePop()
        {
            ImGuiWindow window = GetCurrentWindow();
            Unindent();
            window.DC.TreeDepth--;
            PopID();
        }

        void Value(string prefix, bool b)
        {
            Text("{0}: {1}", prefix, (b ? "true" : "false"));
        }

        void Value(string prefix, int v)
        {
            Text("{0}: {1}", prefix, v);
        }

        void Value(string prefix, uint v)
        {
            Text("{0}: {1}", prefix, v);
        }

        void Value(string prefix, float v, int dplace = -1)
        {
            if (dplace == 0)
            {
                Text("{0}: {1}", prefix, v);
            }
            else if (dplace < 0)
            {
                Text("{0}: {1:0.000}", prefix, v);
            }
            else
            {
                Text("{0}: {1:0." + new string('0', dplace) + "}", prefix, v);
            }
        }

        // FIXME: May want to remove those helpers?
        void ValueColor(string prefix, ImVec4 v)
        {
            Text("{0}: ({1:0.00},{2:0.00},{3:0.00},{4:0.00})", prefix, v.x, v.y, v.z, v.w);
            SameLine();
            ColorButton(v, true);
        }

        void ValueColor(string prefix, uint v)
        {
            Text("{0}: {1:X8})", prefix, v);
            SameLine();

            ImVec4 col;
            col.x = (float)((v >> 0) & 0xFF) / 255.0f;
            col.y = (float)((v >> 8) & 0xFF) / 255.0f;
            col.z = (float)((v >> 16) & 0xFF) / 255.0f;
            col.w = (float)((v >> 24) & 0xFF) / 255.0f;
            ColorButton(col, true);
        }



        // A little colored square. Return true when clicked.
        // FIXME: May want to display/ignore the alpha component in the color display? Yet show it in the tooltip.
        bool ColorButton(ImVec4 col, bool small_height = false, bool outline_border = true)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID("#colorbutton");
            float square_size = g.FontSize;
            ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(square_size + style.FramePadding.y * 2, square_size + (small_height ? 0 : style.FramePadding.y * 2)));
            ItemSize(bb, small_height ? 0.0f : style.FramePadding.y);
            if (!ItemAdd(bb, id))
                return false;

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(bb, id, ref hovered, ref held);
            RenderFrame(bb.Min, bb.Max, GetColorU32(col), outline_border, style.FrameRounding);

            if (hovered.Value)
                SetTooltip("Color:\n({0:0.00},{1:0.00},{2:0.00},{3:0.00})\n#{0:X2}{1:X2}{2:X2}{3:X3}", col.x, col.y, col.z, col.w, IM_F32_TO_INT8(col.x), IM_F32_TO_INT8(col.y), IM_F32_TO_INT8(col.z), IM_F32_TO_INT8(col.z));

            return pressed;
        }

        // Tooltip is stored and turned into a BeginTooltip()/EndTooltip() sequence at the end of the frame. Each call override previous value.
        void SetTooltip(string text, params object[] args)
        {
            if (args != null && args.Length > 0)
                text = string.Format(text, args);

            ImGuiState g = State;
            g.Tooltip = text;
        }


        bool Checkbox(string label, ref bool v)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);

            ImRect check_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(label_size.y + style.FramePadding.y * 2, label_size.y + style.FramePadding.y * 2));
            ItemSize(check_bb, style.FramePadding.y);

            ImRect total_bb = check_bb;
            if (label_size.x > 0)
                SameLine(0, style.ItemInnerSpacing.x);
            ImRect text_bb = new ImRect(window.DC.CursorPos + new ImVec2(0, style.FramePadding.y), window.DC.CursorPos + new ImVec2(0, style.FramePadding.y) + label_size);
            if (label_size.x > 0)
            {
                ItemSize(new ImVec2(text_bb.GetWidth(), check_bb.GetHeight()), style.FramePadding.y);
                total_bb = new ImRect(Min(check_bb.Min, text_bb.Min), Max(check_bb.Max, text_bb.Max));
            }

            if (!ItemAdd(total_bb, id))
                return false;

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(total_bb, id, ref hovered, ref held);
            if (pressed)
                v = !(v);

            RenderFrame(check_bb.Min, check_bb.Max, GetColorU32((held.Value && hovered.Value) ? ImGuiCol.ImGuiCol_FrameBgActive : hovered.Value ? ImGuiCol.ImGuiCol_FrameBgHovered : ImGuiCol.ImGuiCol_FrameBg), true, style.FrameRounding);
            if (v)
            {
                float check_sz = Min(check_bb.GetWidth(), check_bb.GetHeight());
                float pad = Max(1.0f, (float)(int)(check_sz / 6.0f));
                window.DrawList.AddRectFilled(check_bb.Min + new ImVec2(pad, pad), check_bb.Max - new ImVec2(pad, pad), GetColorU32(ImGuiCol.ImGuiCol_CheckMark), style.FrameRounding);
            }

            //TODO logging
            //if (g.LogEnabled)
            //    LogRenderedText(text_bb.GetTL(), *v ? "[x]" : "[ ]");
            if (label_size.x > 0.0f)
                RenderText(text_bb.GetTL(), label);

            return pressed;
        }

        bool CheckboxFlags(string label, ref uint flags, uint flags_value)
        {
            bool v = ((flags & flags_value) == flags_value);
            bool pressed = Checkbox(label, ref v);
            if (pressed)
            {
                if (v)
                    flags |= flags_value;
                else
                    flags &= ~flags_value;
            }

            return pressed;
        }

        bool RadioButton(string label, bool active)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            uint id = window.GetID(label);
            ImVec2 label_size = CalcTextSize(label, 0, -1, true);

            ImRect check_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + new ImVec2(label_size.y + style.FramePadding.y * 2 - 1, label_size.y + style.FramePadding.y * 2 - 1));
            ItemSize(check_bb, style.FramePadding.y);

            ImRect total_bb = check_bb;
            if (label_size.x > 0)
                SameLine(0, style.ItemInnerSpacing.x);
            ImRect text_bb = new ImRect(window.DC.CursorPos + new ImVec2(0, style.FramePadding.y), window.DC.CursorPos + new ImVec2(0, style.FramePadding.y) + label_size);
            if (label_size.x > 0)
            {
                ItemSize(new ImVec2(text_bb.GetWidth(), check_bb.GetHeight()), style.FramePadding.y);
                total_bb.Add(text_bb);
            }

            if (!ItemAdd(total_bb, id))
                return false;

            ImVec2 center = check_bb.GetCenter();
            center.x = (float)(int)center.x + 0.5f;
            center.y = (float)(int)center.y + 0.5f;
            float radius = check_bb.GetHeight() * 0.5f;

            bool? hovered = false, held = false;
            bool pressed = ButtonBehavior(total_bb, id, ref hovered, ref held);

            window.DrawList.AddCircleFilled(center, radius, GetColorU32((held.Value && hovered.Value) ? ImGuiCol.ImGuiCol_FrameBgActive : hovered.Value ? ImGuiCol.ImGuiCol_FrameBgHovered : ImGuiCol.ImGuiCol_FrameBg), 16);
            if (active)
            {
                float check_sz = Min(check_bb.GetWidth(), check_bb.GetHeight());
                float pad = Max(1.0f, (float)(int)(check_sz / 6.0f));
                window.DrawList.AddCircleFilled(center, radius - pad, GetColorU32(ImGuiCol.ImGuiCol_CheckMark), 16);
            }

            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) != 0)
            {
                window.DrawList.AddCircle(center + new ImVec2(1, 1), radius, GetColorU32(ImGuiCol.ImGuiCol_BorderShadow), 16);
                window.DrawList.AddCircle(center, radius, GetColorU32(ImGuiCol.ImGuiCol_Border), 16);
            }

            //TODO logging
            //if (g.LogEnabled)
            //    LogRenderedText(text_bb.GetTL(), active ? "(x)" : "( )");
            if (label_size.x > 0.0f)
                RenderText(text_bb.GetTL(), label);

            return pressed;
        }

        bool RadioButton(string label, ref int v, int v_button)
        {
            bool pressed = RadioButton(label, v == v_button);
            if (pressed)
            {
                v = v_button;
            }
            return pressed;
        }


        bool show_clip_rects = true;
        void NodeDrawList(ImDrawList draw_list, string label)
        {
            ImGuiState g = State;                // Access private state
            bool node_opened = TreeNode(draw_list._OwnerName, "{0}: '{1}' {2} vtx, {3} indices, {4} cmds", label, draw_list._OwnerName != null ? draw_list._OwnerName : "", draw_list.VtxBuffer.Size, draw_list.IdxBuffer.Size, draw_list.CmdBuffer.Size);
            if (draw_list == GetWindowDrawList())
            {
                SameLine();
                TextColored(new ImColor(255, 100, 100), "CURRENTLY APPENDING"); // Can't display stats for active draw list! (we don't have the data double-buffered)
            }
            if (!node_opened)
                return;

            int elem_offset = 0;
            //for (ImDrawCmd pcmd = draw_list.CmdBuffer.begin(); pcmd < draw_list.CmdBuffer.end(); elem_offset += pcmd.ElemCount, pcmd++)
            for (var pcmdidx = 0; pcmdidx < draw_list.CmdBuffer.Size; pcmdidx++)
            {
                var pcmd = draw_list.CmdBuffer[pcmdidx];
                //elem_offset += (int)pcmd.ElemCount;

                //TODO show metric callback data
                //if (pcmd.UserCallback != null)
                //{
                //    BulletText("Callback %p, user_data %p", pcmd.UserCallback, pcmd.UserCallbackData);
                //    continue;
                //}
                BulletText("Draw {0} {1} vtx, tex = {2}, clip_rect = ({3},{4})..({5},{6})", pcmd.ElemCount, draw_list.IdxBuffer.Size > 0 ? "indexed" : "non-indexed", pcmd.TextureId, pcmd.ClipRect.x, pcmd.ClipRect.y, pcmd.ClipRect.z, pcmd.ClipRect.w);
                if (show_clip_rects && IsItemHovered())
                {
                    ImRect clip_rect = new ImRect(pcmd.ClipRect);
                    ImRect vtxs_rect = ImRect.Empty;
                    ImVector<ImDrawIdx> idx_buffer = (draw_list.IdxBuffer.Size > 0) ? draw_list.IdxBuffer : null;
                    for (int i = elem_offset; i < elem_offset + (int)pcmd.ElemCount; i++)
                        vtxs_rect.Add(draw_list.VtxBuffer[idx_buffer != null ? idx_buffer[i] : i].pos);
                    g.OverlayDrawList.PushClipRectFullScreen();
                    clip_rect.Round(); g.OverlayDrawList.AddRect(clip_rect.Min, clip_rect.Max, new ImColor(255, 255, 0));
                    vtxs_rect.Round(); g.OverlayDrawList.AddRect(vtxs_rect.Min, vtxs_rect.Max, new ImColor(255, 0, 255));
                    g.OverlayDrawList.PopClipRect();
                }
            }
            TreePop();
        }

        void NodeWindows(ImVector<ImGuiWindow> windows, string label)
        {
            if (!TreeNode(label, "{0} ({1})", label, windows.Size))
                return;
            for (int i = 0; i < windows.Size; i++)
                NodeWindow(windows[i], "Window");
            TreePop();
        }

        void NodeWindow(ImGuiWindow window, string label)
        {
            if (!TreeNode(window.Name, "{0} '{1}', {2} ", label, window.Name, window.Active || window.WasActive))
                return;
            NodeDrawList(window.DrawList, "DrawList");
            if (window.RootWindow != window) NodeWindow(window.RootWindow, "RootWindow");
            if (window.DC.ChildWindows.Size > 0) NodeWindows(window.DC.ChildWindows, "ChildWindows");
            BulletText("Storage: {0} bytes", window.StateStorage.Data.Size * (int)9/*sizeof(ImGuiStorage.Pair)*/);
            TreePop();
        }

        public void ShowMetricsWindow(ref bool opened)
        {
            if (Begin("ImGui Metrics", ref opened))
            {
                Text("ImGui {0}", GetVersion());
                Text("Application average {0:0.000} ms/frame ({1:0.0} FPS)", 1000.0f / GetIO().Framerate, GetIO().Framerate);
                Text("{0} vertices, {1} indices ({2} triangles)", GetIO().MetricsRenderVertices, GetIO().MetricsRenderIndices, GetIO().MetricsRenderIndices / 3);
                Text("{0} allocations", GetIO().MetricsAllocs);
                Checkbox("Show clipping rectangles when hovering a ImDrawCmd", ref show_clip_rects);
                Separator();

                ImGuiState g = State;                // Access private state
                NodeWindows(g.Windows, "Windows");
                if (TreeNode("DrawList", "Active DrawLists ({0})", g.RenderDrawLists[0].Size))
                {
                    for (int i = 0; i < g.RenderDrawLists[0].Size; i++)
                        NodeDrawList(g.RenderDrawLists[0][i], "DrawList");
                    TreePop();
                }
                if (TreeNode("Popups", "Opened Popups Stack ({0})", g.OpenedPopupStack.Size))
                {
                    for (int i = 0; i < g.OpenedPopupStack.Size; i++)
                    {
                        ImGuiWindow window = g.OpenedPopupStack[i].Window;
                        BulletText("PopupID: {0:X8}, Window: '{1}'{2}{3}", g.OpenedPopupStack[i].PopupID,
                            window != null ? window.Name : "NULL",
                            window != null && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0 ? " ChildWindow" : "", window != null && (window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildMenu) != 0 ? " ChildMenu" : "");
                    }
                    TreePop();
                }
                if (TreeNode("Basic state"))
                {
                    Text("FocusedWindow: '{0}'", g.FocusedWindow != null ? g.FocusedWindow.Name : "NULL");
                    Text("HoveredWindow: '{0}'", g.HoveredWindow != null ? g.HoveredWindow.Name : "NULL");
                    Text("HoveredRootWindow: '{0}'", g.HoveredRootWindow != null ? g.HoveredRootWindow.Name : "NULL");
                    Text("HoveredID: 0x{0:X8}/0x{1:X8}", (uint)g.HoveredId, (uint)g.HoveredIdPreviousFrame); // Data is "in-flight" so depending on when the Metrics window is called we may see current frame information or not
                    Text("ActiveID: 0x{0:X8}/0x{1:X8}", (uint)g.ActiveId, (uint)g.ActiveIdPreviousFrame);
                    TreePop();
                }
            }
            End();
        }


        int InputTextCalcTextLenAndLineCount(string text, int text_begin, ref int out_text_end)
        {

            int line_count = 0;
            int s = text_begin;
            char c;
            while ((c = text[s++]) != 0) // We are only matching for \n so we can ignore UTF-8 decoding
                if (c == '\n')
                    line_count++;
            s--;
            if (text[s] != '\n' && text[s] != '\r')
                line_count++;
            out_text_end = s;
            return line_count;
        }

        int InputTextCalcTextLenAndLineCount(char[] text, int text_begin, ref int out_text_end)
        {

            int line_count = 0;
            int s = text_begin;
            char c;
            while ((c = text[s++]) != 0) // We are only matching for \n so we can ignore UTF-8 decoding
                if (c == '\n')
                    line_count++;
            s--;
            if (text[s] != '\n' && text[s] != '\r')
                line_count++;
            out_text_end = s;
            return line_count;
        }


        internal ImVec2 InputTextCalcTextSizeW(string text, int text_begin, int text_end, ref int? remaining, ref ImVec2? out_offset, bool stop_on_new_line = false)
        {
            ImFont font = State.Font;
            float line_height = State.FontSize;
            float scale = line_height / State.FontSize;

            ImVec2 text_size = new ImVec2(0, 0);
            float line_width = 0.0f;

            int s = text_begin;
            while (s < text_end)
            {
                uint c = (uint)(text[s++]);
                if (c == '\n')
                {
                    text_size.x = Max(text_size.x, line_width);
                    text_size.y += line_height;
                    line_width = 0.0f;
                    if (stop_on_new_line)
                        break;
                    continue;
                }
                if (c == '\r')
                    continue;

                float char_width = font.GetCharAdvance((ushort)c) * scale;
                line_width += char_width;
            }

            if (text_size.x < line_width)
                text_size.x = line_width;

            if (out_offset.HasValue)
                out_offset = new ImVec2(line_width, text_size.y + line_height);  // offset allow for the possibility of sitting after a trailing \n

            if (line_width > 0 || text_size.y == 0.0f)                        // whereas size.y will ignore the trailing \n
                text_size.y += line_height;

            if (remaining.HasValue)
                remaining = s;

            return text_size;
        }

        internal ImVec2 InputTextCalcTextSizeW(char[] text, int text_begin, int text_end, ref int? remaining, ref ImVec2? out_offset, bool stop_on_new_line = false)
        {
            ImFont font = State.Font;
            float line_height = State.FontSize;
            float scale = line_height / State.FontSize;

            ImVec2 text_size = new ImVec2(0, 0);
            float line_width = 0.0f;

            int s = text_begin;
            while (s < text_end)
            {
                uint c = (uint)(text[s++]);
                if (c == '\n')
                {
                    text_size.x = Max(text_size.x, line_width);
                    text_size.y += line_height;
                    line_width = 0.0f;
                    if (stop_on_new_line)
                        break;
                    continue;
                }
                if (c == '\r')
                    continue;

                float char_width = font.GetCharAdvance((ushort)c) * scale;
                line_width += char_width;
            }

            if (text_size.x < line_width)
                text_size.x = line_width;

            if (out_offset.HasValue)
                out_offset = new ImVec2(line_width, text_size.y + line_height);  // offset allow for the possibility of sitting after a trailing \n

            if (line_width > 0 || text_size.y == 0.0f)                        // whereas size.y will ignore the trailing \n
                text_size.y += line_height;

            if (remaining.HasValue)
                remaining = s;

            return text_size;
        }


        int ImStrbolW(ImVector<char> data, int buf_mid_line, int buf_begin) // find beginning-of-line
        {
            while (buf_mid_line > buf_begin && data[buf_mid_line - 1] != '\n')
                buf_mid_line--;
            return buf_mid_line;
        }

        internal static int ImStrLen(char[] data)
        {
            for (var i = 0; i < data.Length; i++)
                if (data[i] == 0)
                    return i;

            return 0;
        }

        static bool isprint(char c)
        {
            return char.IsPunctuation(c) || char.IsSeparator(c) || char.IsLetterOrDigit(c);
        }
        // Return false to discard a character.
        static bool InputTextFilterCharacter(ref char c, ImGuiInputTextFlags flags, ImGuiTextEditCallback callback, object user_data)
        {
            //uint c = *p_char;
            
            if (c < 128 && c != ' ' && !isprint(c))
            {
                bool pass = false;
                pass |= (c == '\n' && (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_Multiline) != 0);
                pass |= (c == '\t' && (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_AllowTabInput) != 0);
                if (!pass)
                    return false;
            }

            if (c >= 0xE000 && c <= 0xF8FF) // Filter private Unicode range. I don't imagine anybody would want to input them. GLFW on OSX seems to send private characters for special keys like arrow keys.
                return false;

            if ((flags & (ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal | ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal | ImGuiInputTextFlags.ImGuiInputTextFlags_CharsUppercase | ImGuiInputTextFlags.ImGuiInputTextFlags_CharsNoBlank)) != 0)
            {
                if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal) != 0)
                    if (!(c >= '0' && c <= '9') && (c != '.') && (c != '-') && (c != '+') && (c != '*') && (c != '/'))
                        return false;

                if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal) != 0)
                    if (!(c >= '0' && c <= '9') && !(c >= 'a' && c <= 'f') && !(c >= 'A' && c <= 'F'))
                        return false;

                if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsUppercase) != 0)
                    if (c >= 'a' && c <= 'z')
                        c = (char)(c + ('A' - 'a'));

                if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CharsNoBlank) != 0)
                    if (char.IsSeparator(c))
                        return false;
            }

            //TODO text edit callback
            //if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCharFilter) != 0)
            //{
            //    ImGuiTextEditCallbackData callback_data;
            //    memset(&callback_data, 0, sizeof(ImGuiTextEditCallbackData));
            //    callback_data.EventFlag = ImGuiInputTextFlags_CallbackCharFilter;
            //    callback_data.EventChar = (ImWchar)c;
            //    callback_data.Flags = flags;
            //    callback_data.UserData = user_data;
            //    if (callback(&callback_data) != 0)
            //        return false;
            //    *p_char = callback_data.EventChar;
            //    if (!callback_data.EventChar)
            //        return false;
            //}

            return true;
        }

        // Helper to create a child window / scrolling region that looks like a normal widget frame.
        bool BeginChildFrame(uint id, ImVec2 size, ImGuiWindowFlags extra_flags = 0)
        {
            ImGuiState g = State;
            ImGuiStyle style = g.Style;
            PushStyleColor(ImGuiCol.ImGuiCol_ChildWindowBg, style.Colors[(int)ImGuiCol.ImGuiCol_FrameBg]);
            PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_ChildWindowRounding, style.FrameRounding);
            return BeginChild(id, size, false, ImGuiWindowFlags.ImGuiWindowFlags_NoMove | extra_flags);
        }

        void EndChildFrame()
        {
            EndChild();
            PopStyleVar();
            PopStyleColor();
        }


        bool BeginChild(string str_id, ImVec2 size_arg, bool border, ImGuiWindowFlags extra_flags)
        {
            ImGuiWindow window = GetCurrentWindow();
            ImGuiWindowFlags flags = ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags.ImGuiWindowFlags_NoResize | ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings | ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow;

            ImVec2 content_avail = GetContentRegionAvail();
            ImVec2 size = Round(size_arg);
            if (size.x <= 0.0f)
            {
                if (size.x == 0.0f)
                    flags |= ImGuiWindowFlags.ImGuiWindowFlags_ChildWindowAutoFitX;
                size.x = Max(content_avail.x, 4.0f) - fabsf(size.x); // Arbitrary minimum zero-ish child size of 4.0f (0.0f causing too much issues)
            }
            if (size.y <= 0.0f)
            {
                if (size.y == 0.0f)
                    flags |= ImGuiWindowFlags.ImGuiWindowFlags_ChildWindowAutoFitY;
                size.y = Max(content_avail.y, 4.0f) - fabsf(size.y);
            }
            if (border)
                flags |= ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders;
            flags |= extra_flags;

            //ImFormatString(title, IM_ARRAYSIZE(title), "%s.%s", window.Name, str_id);
            var title = string.Format("{0}.{1}", window.Name, str_id);

            float alpha = 1.0f;
            bool opened = true;
            bool ret = BeginEx(title, ref opened, size, alpha, flags, false);

            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders) == 0)
                GetCurrentWindow().Flags &= ~ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders;

            return ret;
        }

        bool BeginChild(uint id, ImVec2 size, bool border, ImGuiWindowFlags extra_flags)
        {
            //ImFormatString(str_id, IM_ARRAYSIZE(str_id), "child_%08x", id);
            var str_id = string.Format("child_{0:X8}", id);
            return BeginChild(str_id, size, border, extra_flags);
        }

        void EndChild()
        {
            ImGuiWindow window = GetCurrentWindow();

            System.Diagnostics.Debug.Assert((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) != 0);   // Mismatched BeginChild()/EndChild() callss
            if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) != 0 || window.BeginCount > 1)
            {
                End();
            }
            else
            {
                // When using auto-filling child window, we don't provide full width/height to ItemSize so that it doesn't feed back into automatic size-fitting.
                ImVec2 sz = GetWindowSize();
                if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindowAutoFitX) != 0) // Arbitrary minimum zero-ish child size of 4.0f causes less trouble than a 0.0f
                    sz.x = Max(4.0f, sz.x);
                if ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindowAutoFitY) != 0)
                    sz.y = Max(4.0f, sz.y);

                End();

                window = GetCurrentWindow();
                ImRect bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + sz);
                ItemSize(sz);
                ItemAdd(bb, null);
            }
        }

        public bool InputText(string label, char[] buf, int buf_size, ImGuiInputTextFlags flags = 0, ImGuiTextEditCallback callback = null, object user_data = null)
        {
            System.Diagnostics.Debug.Assert((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_Multiline) == 0); // call InputTextMultiline()
            bool ret = InputTextEx(label, buf, (int)buf_size, new ImVec2(0, 0), flags, callback, user_data);
            return ret;
        }
        // Edit a string of text
        // NB: when active, hold on a privately held copy of the text (and apply back to 'buf'). So changing 'buf' while active has no effect.
        // FIXME: Rather messy function partly because we are doing UTF8 > u16 > UTF8 conversions on the go to more easily handle stb_textedit calls. Ideally we should stay in UTF-8 all the time. See https://github.com/nothings/stb/issues/188
        bool InputTextEx(string label, char[] buf, int buf_size, ImVec2 size_arg, ImGuiInputTextFlags flags, ImGuiTextEditCallback callback = null, object user_data = null)
        {
            ImGuiWindow window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            System.Diagnostics.Debug.Assert(!((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackHistory) != 0 && (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_Multiline) != 0)); // Can't use both together (they both use up/down keys)
            System.Diagnostics.Debug.Assert(!((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCompletion) != 0 && (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_AllowTabInput) != 0)); // Can't use both together (they both use tab key)

            ImGuiState g = State;
            ImGuiIO io = g.IO;
            ImGuiStyle style = g.Style;

            uint id = window.GetID(label);
            bool is_multiline = (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_Multiline) != 0;
            bool is_editable = (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_ReadOnly) == 0;
            bool is_password = (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_Password) != 0;

            ImVec2 label_size = CalcTextSize(label, 0, -1, true);
            ImVec2 size = CalcItemSize(size_arg, CalcItemWidth(), (is_multiline ? GetTextLineHeight() * 8.0f : label_size.y) + style.FramePadding.y * 2.0f); // Arbitrary default of 8 lines high for multi-line
            ImRect frame_bb = new ImRect(window.DC.CursorPos, window.DC.CursorPos + size);
            ImRect total_bb = new ImRect(frame_bb.Min, frame_bb.Max + new ImVec2(label_size.x > 0.0f ? (style.ItemInnerSpacing.x + label_size.x) : 0.0f, 0.0f));

            ImGuiWindow draw_window = window;
            if (is_multiline)
            {
                BeginGroup();
                if (!BeginChildFrame(id, frame_bb.GetSize()))
                {
                    EndChildFrame();
                    EndGroup();
                    return false;
                }
                draw_window = GetCurrentWindow();
                draw_window.DC.CursorPos += style.FramePadding;
                size.x -= draw_window.ScrollbarSizes.x;
            }
            else
            {
                ItemSize(total_bb, style.FramePadding.y);
                if (!ItemAdd(total_bb, id))
                    return false;
            }

            // Password pushes a temporary font with only a fallback glyph
            if (is_password)
            {
                //ImFont.Glyph glyph = g.Font.FindGlyph('*');
                ImFont.Glyph glyph = new ImFont.Glyph();
                ImFont password_font = g.InputTextPasswordFont;
                password_font.FontSize = g.Font.FontSize;
                password_font.Scale = g.Font.Scale;
                password_font.DisplayOffset = g.Font.DisplayOffset;
                password_font.Ascent = g.Font.Ascent;
                password_font.Descent = g.Font.Descent;
                password_font.ContainerAtlas = g.Font.ContainerAtlas;
                password_font.FallbackGlyph = glyph;
                password_font.FallbackXAdvance = glyph.XAdvance;
                System.Diagnostics.Debug.Assert(password_font.Glyphs.empty() && password_font.IndexXAdvance.empty() && password_font.IndexLookup.empty());
                PushFont(password_font);
            }

            // NB: we are only allowed to access 'edit_state' if we are the active widget.
            ImGuiTextEditState edit_state = g.InputTextState;

            bool is_ctrl_down = io.KeyCtrl;
            bool is_shift_down = io.KeyShift;
            bool is_alt_down = io.KeyAlt;
            bool focus_requested = FocusableItemRegister(window, g.ActiveId == id, (flags & (ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCompletion | ImGuiInputTextFlags.ImGuiInputTextFlags_AllowTabInput)) == 0);    // Using completion callback disable keyboard tabbing
            bool focus_requested_by_code = focus_requested && (window.FocusIdxAllCounter == window.FocusIdxAllRequestCurrent);
            bool focus_requested_by_tab = focus_requested && !focus_requested_by_code;

            bool hovered = IsHovered(frame_bb, id);
            if (hovered)
            {
                SetHoveredID(id);
                g.MouseCursor = ImGuiMouseCursor.ImGuiMouseCursor_TextInput;
            }
            bool user_clicked = hovered && io.MouseClicked[0];
            bool user_scrolled = is_multiline && g.ActiveId == 0 && edit_state.Id == id && g.ActiveIdPreviousFrame == draw_window.GetID("#SCROLLY");

            bool select_all = (g.ActiveId != id) && (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_AutoSelectAll) != 0;
            if (focus_requested || user_clicked || user_scrolled)
            {
                if (g.ActiveId != id)
                {
                    // Start edition
                    // Take a copy of the initial buffer value (both in original UTF-8 format and converted to wchar)
                    // From the moment we focused we are ignoring the content of 'buf' (unless we are in read-only mode)
                    int prev_len_w = edit_state.CurLenW;
                    edit_state.Text.resize(buf_size + 1);        // wchar count <= utf-8 count. we use +1 to make sure that .Data isn't NULL so it doesn't crash.
                    edit_state.InitialText.resize(buf_size + 1); // utf-8. we use +1 to make sure that .Data isn't NULL so it doesn't crash.

                    //looks like its just copying buf to InitialText?
                    //ImFormatString(edit_state.InitialText.Data, edit_state.InitialText.Size, "%s", buf);
                    foreach (var ch in buf)
                    {
                        edit_state.Text.push_back(ch);
                        edit_state.InitialText.push_back(ch);
                    }

                    //int buf_end = -1;
                    edit_state.CurLenW = ImStrLen(buf); //TODO: Check if this is right - ImTextStrFromUtf8(edit_state.Text.Data, edit_state.Text.Size, buf, NULL, &buf_end);
                    edit_state.CurLenA = edit_state.CurLenW; //TODO: Check if this is right - (int)(buf_end - buf); // We can't get the result from ImFormatString() above because it is not UTF-8 aware. Here we'll cut off malformed UTF-8.
                    edit_state.CursorAnimReset();

                    // Preserve cursor position and undo/redo stack if we come back to same widget
                    // FIXME: We should probably compare the whole buffer to be on the safety side. Comparing buf (utf8) and edit_state.Text (wchar).
                    bool recycle_state = (edit_state.Id == id) && (prev_len_w == edit_state.CurLenW);
                    if (recycle_state)
                    {
                        // Recycle existing cursor/selection/undo stack but clamp position
                        // Note a single mouse click will override the cursor/position immediately by calling stb_textedit_click handler.
                        edit_state.CursorClamp();
                    }
                    else
                    {
                        edit_state.Id = id;
                        edit_state.ScrollX = 0.0f;
                        //TODO: stb_textedit_initialize_state(&edit_state.StbState, !is_multiline);
                        if (!is_multiline && focus_requested_by_code)
                            select_all = true;
                    }
                    if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_AlwaysInsertMode) != 0)
                        edit_state.StbState.insert_mode = true;
                    if (!is_multiline && (focus_requested_by_tab || (user_clicked && is_ctrl_down)))
                        select_all = true;
                }
                SetActiveID(id, window);
                FocusWindow(window);
            }
            else if (io.MouseClicked[0])
            {
                // Release focus when we click outside
                if (g.ActiveId == id)
                    SetActiveID(0);
            }

            bool value_changed = false;
            bool enter_pressed = false;

            if (g.ActiveId == id)
            {
                if (!is_editable && !g.ActiveIdIsJustActivated)
                {
                    // When read-only we always use the live data passed to the function
                    edit_state.Text.resize(buf_size + 1);
                    //int buf_end = -1;
                    foreach (var ch in buf)
                        edit_state.Text.push_back(ch);

                    edit_state.CurLenW = ImStrLen(buf); //TODO: double check - ImTextStrFromUtf8(edit_state.Text.Data, edit_state.Text.Size, buf, NULL, &buf_end);
                    edit_state.CurLenA = edit_state.CurLenW;
                    edit_state.CursorClamp();
                }

                edit_state.BufSizeA = buf_size;

                // Although we are active we don't prevent mouse from hovering other elements unless we are interacting right now with the widget.
                // Down the line we should have a cleaner library-wide concept of Selected vs Active.
                g.ActiveIdAllowOverlap = !io.MouseDown[0];

                // Edit in progress
                float mouse_x = (g.IO.MousePos.x - frame_bb.Min.x - style.FramePadding.x) + edit_state.ScrollX;
                float mouse_y = (is_multiline ? (g.IO.MousePos.y - draw_window.DC.CursorPos.y - style.FramePadding.y) : (g.FontSize * 0.5f));

                if (select_all || (hovered && io.MouseDoubleClicked[0]))
                {
                    edit_state.SelectAll();
                    edit_state.SelectedAllMouseLock = true;
                }
                else if (io.MouseClicked[0] && !edit_state.SelectedAllMouseLock)
                {
                    stb_textedit.stb_textedit_click(edit_state, edit_state.StbState, mouse_x, mouse_y);
                    edit_state.CursorAnimReset();
                }
                else if (io.MouseDown[0] && !edit_state.SelectedAllMouseLock && (io.MouseDelta.x != 0.0f || io.MouseDelta.y != 0.0f))
                {
                    stb_textedit.stb_textedit_drag(edit_state, edit_state.StbState, mouse_x, mouse_y);
                    edit_state.CursorAnimReset();
                    edit_state.CursorFollow = true;
                }
                if (edit_state.SelectedAllMouseLock && !io.MouseDown[0])
                    edit_state.SelectedAllMouseLock = false;

                if (g.IO.InputCharacters[0] != 0)
                {
                    // Process text input (before we check for Return because using some IME will effectively send a Return?)
                    // We ignore CTRL inputs, but need to allow CTRL+ALT as some keyboards (e.g. German) use AltGR - which is Alt+Ctrl - to input certain characters.
                    if (!(is_ctrl_down && !is_alt_down) && is_editable)
                    {
                        for (int n = 0; n < g.IO.InputCharacters.Length && g.IO.InputCharacters[n] != 0; n++)
                        {
                            var c = g.IO.InputCharacters[n];
                            g.IO.InputCharacters[n] = '\0';
                            if (c != 0)
                            {
                                // Insert character if they pass filtering
                                if (!InputTextFilterCharacter(ref c, flags, callback, user_data))
                                    continue;

                                if (IsKeyPressed(c))
                                    edit_state.OnKeyPressed(c);
                            }
                        }
                    }

                    // Consume characters
                    //we consume them in the for loop
                    //memset(g.IO.InputCharacters, 0, sizeof(g.IO.InputCharacters));
                }

                // Handle various key-presses
                bool cancel_edit = false;
                int k_mask = (is_shift_down ? stb_textedit.STB_TEXTEDIT_K_SHIFT : 0);
                bool is_ctrl_only = is_ctrl_down && !is_alt_down && !is_shift_down;
                //System.Diagnostics.Debug.WriteLine($"is_ctrl_only: {is_ctrl_only}, z: {IsKeyPressedMap(ImGuiKey.ImGuiKey_Z)}, is_editable");
                if (IsKeyPressedMap(ImGuiKey.ImGuiKey_LeftArrow)) { edit_state.OnKeyPressed(is_ctrl_down ? stb_textedit.STB_TEXTEDIT_K_WORDLEFT | k_mask : stb_textedit.STB_TEXTEDIT_K_LEFT | k_mask); }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_RightArrow)) { edit_state.OnKeyPressed(is_ctrl_down ? stb_textedit.STB_TEXTEDIT_K_WORDRIGHT | k_mask : stb_textedit.STB_TEXTEDIT_K_RIGHT | k_mask); }
                else if (is_multiline && IsKeyPressedMap(ImGuiKey.ImGuiKey_UpArrow)) { if (is_ctrl_down) SetWindowScrollY(draw_window, draw_window.Scroll.y - g.FontSize); else edit_state.OnKeyPressed(stb_textedit.STB_TEXTEDIT_K_UP | k_mask); }
                else if (is_multiline && IsKeyPressedMap(ImGuiKey.ImGuiKey_DownArrow)) { if (is_ctrl_down) SetWindowScrollY(draw_window, draw_window.Scroll.y + g.FontSize); else edit_state.OnKeyPressed(stb_textedit.STB_TEXTEDIT_K_DOWN | k_mask); }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_Home)) { edit_state.OnKeyPressed(is_ctrl_down ? stb_textedit.STB_TEXTEDIT_K_TEXTSTART | k_mask : stb_textedit.STB_TEXTEDIT_K_LINESTART | k_mask); }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_End)) { edit_state.OnKeyPressed(is_ctrl_down ? stb_textedit.STB_TEXTEDIT_K_TEXTEND | k_mask : stb_textedit.STB_TEXTEDIT_K_LINEEND | k_mask); }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_Delete) && is_editable) { edit_state.OnKeyPressed(stb_textedit.STB_TEXTEDIT_K_DELETE | k_mask); }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_Backspace) && is_editable) { edit_state.OnKeyPressed(stb_textedit.STB_TEXTEDIT_K_BACKSPACE | k_mask); }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_Enter))
                {
                    bool ctrl_enter_for_new_line = (flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CtrlEnterForNewLine) != 0;
                    if (!is_multiline || (ctrl_enter_for_new_line && !is_ctrl_down) || (!ctrl_enter_for_new_line && is_ctrl_down))
                    {
                        SetActiveID(0);
                        enter_pressed = true;
                    }
                    else if (is_editable)
                    {
                        char c = '\n'; // Insert new line
                        if (InputTextFilterCharacter(ref c, flags, callback, user_data))
                            edit_state.OnKeyPressed((int)c);
                    }
                }
                else if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_AllowTabInput) != 0 && IsKeyPressedMap(ImGuiKey.ImGuiKey_Tab) && !is_ctrl_down && !is_shift_down && !is_alt_down && is_editable)
                {
                    char c = '\t'; // Insert TAB
                    if (InputTextFilterCharacter(ref c, flags, callback, user_data))
                        edit_state.OnKeyPressed((int)c);
                }
                else if (IsKeyPressedMap(ImGuiKey.ImGuiKey_Escape)) { SetActiveID(0); cancel_edit = true; }
                else if (is_ctrl_only && IsKeyPressedMap(ImGuiKey.ImGuiKey_Z) && is_editable) { edit_state.OnKeyPressed(stb_textedit.STB_TEXTEDIT_K_UNDO); edit_state.ClearSelection(); }
                else if (is_ctrl_only && IsKeyPressedMap(ImGuiKey.ImGuiKey_Y) && is_editable) { edit_state.OnKeyPressed(stb_textedit.STB_TEXTEDIT_K_REDO); edit_state.ClearSelection(); }
                else if (is_ctrl_only && IsKeyPressedMap(ImGuiKey.ImGuiKey_A)) { edit_state.SelectAll(); edit_state.CursorFollow = true; }
                else if (is_ctrl_only && !is_password && ((IsKeyPressedMap(ImGuiKey.ImGuiKey_X) && is_editable) || IsKeyPressedMap(ImGuiKey.ImGuiKey_C)) && (!is_multiline || edit_state.HasSelection()))
                {
                    // Cut, Copy
                    bool cut = IsKeyPressedMap(ImGuiKey.ImGuiKey_X);
                    if (cut && !edit_state.HasSelection())
                        edit_state.SelectAll();

                    if (g.IO.SetClipboardTextFn != null)
                    {
                        int ib = edit_state.HasSelection() ? Min(edit_state.StbState.select_start, edit_state.StbState.select_end) : 0;
                        int ie = edit_state.HasSelection() ? Max(edit_state.StbState.select_start, edit_state.StbState.select_end) : edit_state.CurLenW;
                        edit_state.TempTextBuffer.resize((ie - ib) * 4 + 1);
                        var str = new string(edit_state.TempTextBuffer.Data, ib, ie);
                        //ImTextStrToUtf8(edit_state.TempTextBuffer.Data, edit_state.TempTextBuffer.Size, edit_state.Text.Data + ib, edit_state.Text.Data + ie);
                        g.IO.SetClipboardTextFn(str);
                    }

                    if (cut)
                    {
                        edit_state.CursorFollow = true;
                        stb_textedit.stb_textedit_cut(edit_state, edit_state.StbState);
                    }
                }
                else if (is_ctrl_only && IsKeyPressedMap(ImGuiKey.ImGuiKey_V) && is_editable)
                {
                    // Paste
                    if (g.IO.GetClipboardTextFn != null)
                    {
                        var str = g.IO.GetClipboardTextFn();
                        if (!string.IsNullOrEmpty(str))
                        {
                            // Remove new-line from pasted buffer
                            var clipboard_filtered_len = 0;
                            var clipboard_filtered = new char[str.Length];
                            foreach(var ch in str)
                            {
                                if(isprint(ch))
                                    clipboard_filtered[clipboard_filtered_len++] = ch;
                            }
                            //clipboard_filtered[clipboard_filtered_len] = 0;
                            if (clipboard_filtered_len > 0) // If everything was filtered, ignore the pasting operation
                            {
                                stb_textedit.stb_textedit_paste(edit_state, edit_state.StbState, clipboard_filtered, clipboard_filtered_len);
                                edit_state.CursorFollow = true;
                            }
                            //MemFree(clipboard_filtered);
                        }
                    }
                }

                if (cancel_edit)
                {
                    // Restore initial value
                    if (is_editable)
                    {
                        //ImFormatString(buf, buf_size, "%s", edit_state.InitialText.Data);
                        for (var i = 0; i < edit_state.InitialText.Size && i < buf_size; i++)
                        {
                            if (buf[i] != edit_state.InitialText[i])
                            {
                                buf[i] = edit_state.InitialText[i];
                                value_changed = true;
                            }
                        }
                    }
                }
                else
                {
                    // Apply new value immediately - copy modified buffer back
                    // Note that as soon as the input box is active, the in-widget value gets priority over any underlying modification of the input buffer
                    // FIXME: We actually always render 'buf' when calling DrawList.AddText, making the comment above incorrect.
                    // FIXME-OPT: CPU waste to do this every time the widget is active, should mark dirty state from the stb_textedit callbacks.
                    if (is_editable)
                    {
                        edit_state.TempTextBuffer.resize(edit_state.Text.Size * 4); //guess this is for utf32?
                                                                                    //ImTextStrToUtf8(edit_state.TempTextBuffer.Data, edit_state.TempTextBuffer.Size, edit_state.Text.Data, NULL);
                        for (var i = 0; i < edit_state.TempTextBuffer.Size && i < edit_state.Text.Size; i++)
                            edit_state.TempTextBuffer.Data[i] = edit_state.Text[i];
                    }

                    // User callback
                    if ((flags & (ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCompletion | ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackHistory | ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackAlways)) != 0)
                    {
                        System.Diagnostics.Debug.Assert(callback != null);

                        // The reason we specify the usage semantic (Completion/History) is that Completion needs to disable keyboard TABBING at the moment.
                        ImGuiInputTextFlags event_flag = 0;
                        ImGuiKey event_key = ImGuiKey.ImGuiKey_COUNT;
                        if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCompletion) != 0 && IsKeyPressedMap(ImGuiKey.ImGuiKey_Tab))
                        {
                            event_flag = ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCompletion;
                            event_key = ImGuiKey.ImGuiKey_Tab;
                        }
                        else if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackHistory) != 0 && IsKeyPressedMap(ImGuiKey.ImGuiKey_UpArrow))
                        {
                            event_flag = ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackHistory;
                            event_key = ImGuiKey.ImGuiKey_UpArrow;
                        }
                        else if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackHistory) != 0 && IsKeyPressedMap(ImGuiKey.ImGuiKey_DownArrow))
                        {
                            event_flag = ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackHistory;
                            event_key = ImGuiKey.ImGuiKey_DownArrow;
                        }
                        else if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackAlways) != 0)
                            event_flag = ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackAlways;

                        //TODO callback state
                        if (event_flag != 0)
                        {
                            ImGuiTextEditCallbackData callback_data = new ImGuiTextEditCallbackData();
                            //memset(&callback_data, 0, sizeof(ImGuiTextEditCallbackData));
                            callback_data.EventFlag = event_flag;
                            callback_data.Flags = flags;
                            //TODO: callback_data.UserData = user_data;
                            callback_data.ReadOnly = !is_editable;

                            callback_data.EventKey = event_key;
                            callback_data.Buf = edit_state.TempTextBuffer.Data;
                            callback_data.BufTextLen = edit_state.CurLenA;
                            callback_data.BufSize = edit_state.BufSizeA;
                            callback_data.BufDirty = false;

                            // We have to convert from wchar-positions to UTF-8-positions, which can be pretty slow (an incentive to ditch the ImWchar buffer, see https://github.com/nothings/stb/issues/188)
                            var text = edit_state.Text;
                            //int utf8_cursor_pos = callback_data.CursorPos = ImTextCountUtf8BytesFromStr(text, text + edit_state.StbState.cursor);
                            int utf8_cursor_pos = callback_data.CursorPos = edit_state.StbState.cursor;
                            //int utf8_selection_start = callback_data.SelectionStart = ImTextCountUtf8BytesFromStr(text, text + edit_state.StbState.select_start);
                            int utf8_selection_start = callback_data.SelectionStart = edit_state.StbState.select_start;
                            //int utf8_selection_end = callback_data.SelectionEnd = ImTextCountUtf8BytesFromStr(text, text + edit_state.StbState.select_end);
                            int utf8_selection_end = callback_data.SelectionEnd = edit_state.StbState.select_end;

                            // Call user code
                            callback(callback_data);

                            // Read back what user may have modified
                            System.Diagnostics.Debug.Assert(callback_data.Buf == edit_state.TempTextBuffer.Data);  // Invalid to modify those fields
                            System.Diagnostics.Debug.Assert(callback_data.BufSize == edit_state.BufSizeA);
                            System.Diagnostics.Debug.Assert(callback_data.Flags == flags);
                            //if (callback_data.CursorPos != utf8_cursor_pos) edit_state.StbState.cursor = ImTextCountCharsFromUtf8(callback_data.Buf, callback_data.Buf + callback_data.CursorPos);
                            if (callback_data.CursorPos != utf8_cursor_pos) edit_state.StbState.cursor = callback_data.CursorPos;
                            //if (callback_data.SelectionStart != utf8_selection_start) edit_state.StbState.select_start = ImTextCountCharsFromUtf8(callback_data.Buf, callback_data.Buf + callback_data.SelectionStart);
                            if (callback_data.SelectionStart != utf8_selection_start) edit_state.StbState.select_start = callback_data.SelectionStart;
                            //if (callback_data.SelectionEnd != utf8_selection_end) edit_state.StbState.select_end = ImTextCountCharsFromUtf8(callback_data.Buf, callback_data.Buf + callback_data.SelectionEnd);
                            if (callback_data.SelectionEnd != utf8_selection_end) edit_state.StbState.select_end = callback_data.SelectionEnd;
                            if (callback_data.BufDirty)
                            {
                                System.Diagnostics.Debug.Assert(callback_data.BufTextLen == callback_data.Buf.Length); // You need to maintain BufTextLen if you change the text!
                                //edit_state.CurLenW = ImTextStrFromUtf8(edit_state.Text.Data, edit_state.Text.Size, callback_data.Buf, NULL);
                                edit_state.CurLenW = callback_data.BufTextLen;
                                edit_state.CurLenA = callback_data.BufTextLen;  // Assume correct length and valid UTF-8 from user, saves us an extra strlen()
                                edit_state.CursorAnimReset();
                            }
                        }
                    }

                    // Copy back to user buffer
                    //double loop, might as well just copy and not compare
                    if (is_editable /*&& strcmp(edit_state.TempTextBuffer.Data, buf) != 0*/)
                    {
                        //ImFormatString(buf, buf_size, "%s", edit_state.TempTextBuffer.Data);
                        for (var i = 0; i < edit_state.TempTextBuffer.Size && i < buf_size; i++)
                        {
                            //doing a check so that we only set value_changed when data changed
                            if (buf[i] != edit_state.TempTextBuffer[i])
                            {
                                buf[i] = edit_state.TempTextBuffer[i];
                                value_changed = true;
                            }
                        }
                    }
                }
            }

            if (!is_multiline)
                RenderFrame(frame_bb.Min, frame_bb.Max, GetColorU32(ImGuiCol.ImGuiCol_FrameBg), true, style.FrameRounding);

            // Render
            ImVec4 clip_rect = new ImVec4(frame_bb.Min.x, frame_bb.Min.y, frame_bb.Min.x + size.x, frame_bb.Min.y + size.y); // Not using frame_bb.Max because we have adjusted size
            ImVec2 render_pos = is_multiline ? draw_window.DC.CursorPos : frame_bb.Min + style.FramePadding;
            ImVec2 text_size = new ImVec2(0f, 0f);
            if (g.ActiveId == id || (edit_state.Id == id && is_multiline && g.ActiveId == draw_window.GetID("#SCROLLY")))
            {
                edit_state.CursorAnim += g.IO.DeltaTime;

                // We need to:
                // - Display the text (this can be more easily clipped)
                // - Handle scrolling, highlight selection, display cursor (those all requires some form of 1d.2d cursor position calculation)
                // - Measure text height (for scrollbar)
                // We are attempting to do most of that in **one main pass** to minimize the computation cost (non-negligible for large amount of text) + 2nd pass for selection rendering (we could merge them by an extra refactoring effort)
                //ImWchar* text_begin = edit_state.Text.Data;
                int text_begin = 0;
                ImVec2 cursor_offset = ImVec2.Zero, select_start_offset = ImVec2.Zero;

                {
                    // Count lines + find lines numbers straddling 'cursor' and 'select_start' position.
                    int[] searches_input_ptr = new int[2];
                    searches_input_ptr[0] = text_begin + edit_state.StbState.cursor;
                    searches_input_ptr[1] = -1;
                    int searches_remaining = 1;
                    int[] searches_result_line_number = { -1, -999 };
                    if (edit_state.StbState.select_start != edit_state.StbState.select_end)
                    {
                        searches_input_ptr[1] = text_begin + Min(edit_state.StbState.select_start, edit_state.StbState.select_end);
                        searches_result_line_number[1] = -1;
                        searches_remaining++;
                    }

                    // Iterate all lines to find our line numbers
                    // In multi-line mode, we never exit the loop until all lines are counted, so add one extra to the searches_remaining counter.
                    searches_remaining += is_multiline ? 1 : 0;
                    int line_count = 0;
                    for (int s = text_begin; edit_state.Text[s] != 0; s++)
                        if (edit_state.Text[s] == '\n')
                        {
                            line_count++;
                            if (searches_result_line_number[0] == -1 && s >= searches_input_ptr[0]) { searches_result_line_number[0] = line_count; if (--searches_remaining <= 0) break; }
                            if (searches_result_line_number[1] == -1 && s >= searches_input_ptr[1]) { searches_result_line_number[1] = line_count; if (--searches_remaining <= 0) break; }
                        }
                    line_count++;
                    if (searches_result_line_number[0] == -1) searches_result_line_number[0] = line_count;
                    if (searches_result_line_number[1] == -1) searches_result_line_number[1] = line_count;

                    int? remaining = null;
                    ImVec2? out_offset = null;
                    // Calculate 2d position by finding the beginning of the line and measuring distance
                    cursor_offset.x = InputTextCalcTextSizeW(edit_state.Text.Data, ImStrbolW(edit_state.Text, searches_input_ptr[0], text_begin), searches_input_ptr[0], ref remaining, ref out_offset).x;
                    cursor_offset.y = searches_result_line_number[0] * g.FontSize;
                    if (searches_result_line_number[1] >= 0)
                    {
                        select_start_offset.x = InputTextCalcTextSizeW(edit_state.Text.Data, ImStrbolW(edit_state.Text, searches_input_ptr[1], text_begin), searches_input_ptr[1], ref remaining, ref out_offset).x;
                        select_start_offset.y = searches_result_line_number[1] * g.FontSize;
                    }

                    // Calculate text height
                    if (is_multiline)
                        text_size = new ImVec2(size.x, line_count * g.FontSize);
                }

                // Scroll
                if (edit_state.CursorFollow)
                {
                    // Horizontal scroll in chunks of quarter width
                    if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_NoHorizontalScroll) == 0)
                    {
                        float scroll_increment_x = size.x * 0.25f;
                        if (cursor_offset.x < edit_state.ScrollX)
                            edit_state.ScrollX = (float)(int)Max(0.0f, cursor_offset.x - scroll_increment_x);
                        else if (cursor_offset.x - size.x >= edit_state.ScrollX)
                            edit_state.ScrollX = (float)(int)(cursor_offset.x - size.x + scroll_increment_x);
                    }
                    else
                    {
                        edit_state.ScrollX = 0.0f;
                    }

                    // Vertical scroll
                    if (is_multiline)
                    {
                        float scroll_y = draw_window.Scroll.y;
                        if (cursor_offset.y - g.FontSize < scroll_y)
                            scroll_y = Max(0.0f, cursor_offset.y - g.FontSize);
                        else if (cursor_offset.y - size.y >= scroll_y)
                            scroll_y = cursor_offset.y - size.y;
                        draw_window.DC.CursorPos.y += (draw_window.Scroll.y - scroll_y);   // To avoid a frame of lag
                        draw_window.Scroll.y = scroll_y;
                        render_pos.y = draw_window.DC.CursorPos.y;
                    }
                }
                edit_state.CursorFollow = false;
                ImVec2 render_scroll = new ImVec2(edit_state.ScrollX, 0.0f);

                // Draw selection
                if (edit_state.StbState.select_start != edit_state.StbState.select_end)
                {
                    int text_selected_begin = text_begin + Min(edit_state.StbState.select_start, edit_state.StbState.select_end);
                    int text_selected_end = text_begin + Max(edit_state.StbState.select_start, edit_state.StbState.select_end);

                    float bg_offy_up = is_multiline ? 0.0f : -1.0f;    // FIXME: those offsets should be part of the style? they don't play so well with multi-line selection.
                    float bg_offy_dn = is_multiline ? 0.0f : 2.0f;
                    uint bg_color = GetColorU32(ImGuiCol.ImGuiCol_TextSelectedBg);
                    ImVec2 rect_pos = render_pos + select_start_offset - render_scroll;
                    for (int p = text_selected_begin; p < text_selected_end;)
                    {
                        if (rect_pos.y > clip_rect.w + g.FontSize)
                            break;
                        if (rect_pos.y < clip_rect.y)
                        {
                            while (p < text_selected_end)
                                if (buf[p++] == '\n') //TODO: what should we access here?
                                    break;
                        }
                        else
                        {
                            var temp = (int?)p;
                            ImVec2? out_offset = null;
                            ImVec2 rect_size = InputTextCalcTextSizeW(buf, p, text_selected_end, ref temp, ref out_offset, true); p = temp.Value;
                            if (rect_size.x <= 0.0f) rect_size.x = (float)(int)(g.Font.GetCharAdvance((ushort)' ') * 0.50f); // So we can see selected empty lines
                            ImRect rect = new ImRect(rect_pos + new ImVec2(0.0f, bg_offy_up - g.FontSize), rect_pos + new ImVec2(rect_size.x, bg_offy_dn));
                            rect.Clip(new ImRect(clip_rect));
                            if (rect.Overlaps(new ImRect(clip_rect)))
                                draw_window.DrawList.AddRectFilled(rect.Min, rect.Max, bg_color);
                        }
                        rect_pos.x = render_pos.x - render_scroll.x;
                        rect_pos.y += g.FontSize;
                    }
                }


                //System.Diagnostics.Debug.WriteLine($"render_pos: {render_pos},render_pos: {render_scroll},render_scroll: {render_pos},CurLenA: {edit_state.CurLenA},clip_rect: {clip_rect}");
                draw_window.DrawList.AddText(g.Font, g.FontSize, render_pos - render_scroll, GetColorU32(ImGuiCol.ImGuiCol_Text), buf, 0, edit_state.CurLenA, 0.0f, (is_multiline ? null : (ImVec4?)clip_rect));

                // Draw blinking cursor
                ImVec2 cursor_screen_pos = render_pos + cursor_offset - render_scroll;
                bool cursor_is_visible = (g.InputTextState.CursorAnim <= 0.0f) || fmodf(g.InputTextState.CursorAnim, 1.20f) <= 0.80f;
                if (cursor_is_visible)
                    draw_window.DrawList.AddLine(cursor_screen_pos + new ImVec2(0.0f, -g.FontSize + 0.5f), cursor_screen_pos + new ImVec2(0.0f, -1.5f), GetColorU32(ImGuiCol.ImGuiCol_Text));

                // Notify OS of text input position for advanced IME (-1 x offset so that Windows IME can cover our cursor. Bit of an extra nicety.)
                if (is_editable)
                    g.OsImePosRequest = new ImVec2(cursor_screen_pos.x - 1, cursor_screen_pos.y - g.FontSize);
            }
            else
            {
                // Render text only
                int buf_end = -1;
                if (is_multiline)
                    text_size = new ImVec2(size.x, InputTextCalcTextLenAndLineCount(buf, 0, ref buf_end) * g.FontSize); // We don't need width
                draw_window.DrawList.AddText(g.Font, g.FontSize, render_pos, GetColorU32(ImGuiCol.ImGuiCol_Text), buf, 0, buf_end, 0.0f, is_multiline ? null : (ImVec4?)clip_rect);
            }

            if (is_multiline)
            {
                Dummy(text_size + new ImVec2(0.0f, g.FontSize)); // Always add room to scroll an extra line
                EndChildFrame();
                EndGroup();
            }

            if (is_password)
                PopFont();

            //TODO: Log as text
            //if (g.LogEnabled && !is_password)
            //    LogRenderedText(render_pos, buf, NULL);

            if (label_size.x > 0)
                RenderText(new ImVec2(frame_bb.Max.x + style.ItemInnerSpacing.x, frame_bb.Min.y + style.FramePadding.y), label);

            if ((flags & ImGuiInputTextFlags.ImGuiInputTextFlags_EnterReturnsTrue) != 0)
                return enter_pressed;
            else
                return value_changed;
        }

    }
}
