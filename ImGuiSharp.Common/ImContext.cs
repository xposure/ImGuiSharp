using System.Collections.Generic;
using static System.Math;

namespace ImGui
{
    public struct ImDrawRectCmd
    {
        public ImRect rect;
        public ImColor color;
    }

    public partial class ImCmdBuffer
    {
        private static ImVec2[] _circleVerts = new ImVec2[12];

        public ImDrawList _drawList = new ImDrawList();

        static ImCmdBuffer()
        {
            for (var i = 0; i < _circleVerts.Length; i++)
            {
                var t = ((float)i / (float)_circleVerts.Length) * 2 * (float)PI;
                _circleVerts[i] = new ImVec2() { x = (float)Cos(t), y = (float)Sin(t) };
            }
        }

        private List<int> _cmds = new List<int>();
        private List<ImDrawRectCmd> _drawRectCmds = new List<ImDrawRectCmd>();

        public void reset(object tex)
        {
            _cmds.Clear();
            _drawRectCmds.Clear();
            _drawList.Clear();
            _drawList.PushTextureID(new ImTextureID(tex));
        }

        public void DrawRect(ImRect rect, ImColor color)
        {
            _drawList.AddRectFilled(rect.Min, rect.Max, color.ToARGB());
            var idx = _drawRectCmds.Count;
            _cmds.Add(idx << 8);

            _drawRectCmds.Add(new ImDrawRectCmd() { rect = rect, color = color });
        }

        public void done()
        {
            _drawList.PopTextureID();
        }
    }

    public partial class ImContext
    {
        public const bool AntiAliasedShapes = true;
        public const bool AntiAliasedLines = true;
        public const float CurveTessellationTol = 1.25f;


        private ImCmdBuffer _cmdBuffer = new ImCmdBuffer();
        private ImUIState _uiState;
        private uint bgcolor = 0x555555ff;
        private string name = "Hello World!";

        private ImEvent _currentEvent = new ImEvent();
        private ImGuiState _state = new ImGuiState();
        private ImGuiIO io = new ImGuiIO();

        //public bool button(int id, int x, int y)
        //{

        //}

        public void update(float dt, bool active)
        {
            io.DeltaTime = dt;
            io.IsActive = active;

            for (var i = 0; i < io.KeysDown.Length; i++)
                io.KeysDown[i] = false;

            platformupdate();

            //ImGuiState & g = *GImGui;
            var g = io;

            // Check user data
            System.Diagnostics.Debug.Assert(io.DeltaTime >= 0.0f);               // Need a positive DeltaTime (zero is tolerated but will cause some timing issues)
            System.Diagnostics.Debug.Assert(io.DisplaySize.x >= 0.0f && io.DisplaySize.y >= 0.0f);
            //System.Diagnostics.Debug.Assert(io.Fonts.Fonts.Size > 0);           // Font Atlas not created. Did you call io.Fonts.GetTexDataAsRGBA32 / GetTexDataAsAlpha8 ?
            //System.Diagnostics.Debug.Assert(io.Fonts.Fonts[0].IsLoaded());     // Font Atlas not created. Did you call io.Fonts.GetTexDataAsRGBA32 / GetTexDataAsAlpha8 ?
            //System.Diagnostics.Debug.Assert(g.Style.CurveTessellationTol > 0.0f);  // Invalid style setting

            //if (!g.Initialized)
            //{
            //    // Initialize on first frame
            //    //g.LogClipboard = (ImGuiTextBuffer*)ImGui::MemAlloc(sizeof(ImGuiTextBuffer));
            //    //TODO: g.LogClipboard = new ImGuiTextBuffer();
            //    //IM_PLACEMENT_NEW(g.LogClipboard) ImGuiTextBuffer();

            //    //TODO: System.Diagnostics.Debug.Assert(g.Settings.empty());
            //    //TODO: LoadSettings();
            //    g.Initialized = true;
            //}

            //SetCurrentFont(io.Fonts.Fonts[0]);

            _state.Time += io.DeltaTime;
            _state.FrameCount += 1;
            //g.Tooltip = null;
            //g.OverlayDrawList.Clear();
            //g.OverlayDrawList.PushTextureID(io.Fonts.TexID);
            //g.OverlayDrawList.PushClipRectFullScreen();
            //g.OverlayDrawList.AddDrawCmd();

            //// Mark rendering data as invalid to prevent user who may have a handle on it to use it
            //g.RenderDrawData.Valid = false;
            //g.RenderDrawData.CmdLists = null;
            //g.RenderDrawData.CmdListsCount = g.RenderDrawData.TotalVtxCount = g.RenderDrawData.TotalIdxCount = 0;

            // Update inputs state
            if (io.MousePos.x < 0 && io.MousePos.y < 0)
                io.MousePos = new ImVec2(-9999.0f, -9999.0f);
            if ((io.MousePos.x < 0 && io.MousePos.y < 0) || (io.MousePosPrev.x < 0 && io.MousePosPrev.y < 0))   // if mouse just appeared or disappeared (negative coordinate) we cancel out movement in MouseDelta
                io.MouseDelta = new ImVec2(0.0f, 0.0f);
            else
                io.MouseDelta = io.MousePos - io.MousePosPrev;
            io.MousePosPrev = io.MousePos;
            for (int i = 0; i < io.MouseDown.Length; i++)
            {
                io.MouseClicked[i] = io.MouseDown[i] && io.MouseDownDuration[i] < 0.0f;
                io.MouseReleased[i] = !io.MouseDown[i] && io.MouseDownDuration[i] >= 0.0f;
                io.MouseDownDurationPrev[i] = io.MouseDownDuration[i];
                io.MouseDownDuration[i] = io.MouseDown[i] ? (io.MouseDownDuration[i] < 0.0f ? 0.0f : io.MouseDownDuration[i] + io.DeltaTime) : -1.0f;
                io.MouseDoubleClicked[i] = false;
                if (io.MouseClicked[i])
                {
                    if (_state.Time - io.MouseClickedTime[i] < io.MouseDoubleClickTime)
                    {
                        if (ImMath.LengthSqr(io.MousePos - io.MouseClickedPos[i]) < io.MouseDoubleClickMaxDist * io.MouseDoubleClickMaxDist)
                            io.MouseDoubleClicked[i] = true;
                        io.MouseClickedTime[i] = float.MinValue;// -FLT_MAX;    // so the third click isn't turned into a double-click
                    }
                    else
                    {
                        io.MouseClickedTime[i] = _state.Time;
                    }
                    io.MouseClickedPos[i] = io.MousePos;
                    io.MouseDragMaxDistanceSqr[i] = 0.0f;
                }
                else if (io.MouseDown[i])
                {
                    io.MouseDragMaxDistanceSqr[i] = ImMath.Max(io.MouseDragMaxDistanceSqr[i], ImMath.LengthSqr(io.MousePos - io.MouseClickedPos[i]));
                }
            }

            for (int i = 0; i < io.KeysDown.Length; i++)
            {
                io.KeysDownDurationPrev[i] = io.KeysDownDuration[i];
                io.KeysDownDuration[i] = io.KeysDown[i] ? (io.KeysDownDuration[i] < 0.0f ? 0.0f : io.KeysDownDuration[i] + io.DeltaTime) : -1.0f;
            }

            // Calculate frame-rate for the user, as a purely luxurious feature
            _state.FramerateSecPerFrameAccum += io.DeltaTime - _state.FramerateSecPerFrame[_state.FramerateSecPerFrameIdx];
            _state.FramerateSecPerFrame[_state.FramerateSecPerFrameIdx] = io.DeltaTime;
            _state.FramerateSecPerFrameIdx = (_state.FramerateSecPerFrameIdx + 1) % _state.FramerateSecPerFrame.Length;
            io.Framerate = 1.0f / (_state.FramerateSecPerFrameAccum / (float)_state.FramerateSecPerFrame.Length);

            //// Clear reference to active widget if the widget isn't alive anymore
            //g.HoveredIdPreviousFrame = g.HoveredId;
            //g.HoveredId = 0;
            //g.HoveredIdAllowOverlap = false;
            //if (!g.ActiveIdIsAlive && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != 0)
            //    SetActiveID(0);
            //g.ActiveIdPreviousFrame = g.ActiveId;
            //g.ActiveIdIsAlive = false;
            //g.ActiveIdIsJustActivated = false;
            //if (g.ActiveId == 0)
            //    g.MovedWindow = null;

            //// Delay saving settings so we don't spam disk too much
            //if (g.SettingsDirtyTimer > 0.0f)
            //{
            //    g.SettingsDirtyTimer -= io.DeltaTime;
            //    //TODO: if (g.SettingsDirtyTimer <= 0.0f)
            //    //TODO:    SaveSettings();
            //}

            //// Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            //g.HoveredWindow = FindHoveredWindow(io.MousePos, false);
            //if (g.HoveredWindow != null && ((g.HoveredWindow.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow) == ImGuiWindowFlags.ImGuiWindowFlags_ChildWindow))
            //    g.HoveredRootWindow = g.HoveredWindow.RootWindow;
            //else
            //    g.HoveredRootWindow = FindHoveredWindow(io.MousePos, true);

            //ImGuiWindow modal_window = GetFrontMostModalRootWindow();
            //if (modal_window != null)
            //{
            //    g.ModalWindowDarkeningRatio = ImGui.Min(g.ModalWindowDarkeningRatio + io.DeltaTime * 6.0f, 1.0f);
            //    if (g.HoveredRootWindow != modal_window)
            //        g.HoveredRootWindow = g.HoveredWindow = null;
            //}
            //else
            //{
            //    g.ModalWindowDarkeningRatio = 0.0f;
            //}

            //// Are we using inputs? Tell user so they can capture/discard the inputs away from the rest of their application.
            //// When clicking outside of a window we assume the click is owned by the application and won't request capture. We need to track click ownership.
            //int mouse_earliest_button_down = -1;
            //bool mouse_any_down = false;
            //for (int i = 0; i < io.MouseDown.Length; i++)
            //{
            //    if (io.MouseClicked[i])
            //        io.MouseDownOwned[i] = (g.HoveredWindow != null) || (!g.OpenedPopupStack.empty());
            //    mouse_any_down |= io.MouseDown[i];
            //    if (io.MouseDown[i])
            //        if (mouse_earliest_button_down == -1 || io.MouseClickedTime[mouse_earliest_button_down] > io.MouseClickedTime[i])
            //            mouse_earliest_button_down = i;
            //}
            //bool mouse_avail_to_imgui = (mouse_earliest_button_down == -1) || io.MouseDownOwned[mouse_earliest_button_down];
            //if (g.CaptureMouseNextFrame != -1)
            //    io.WantCaptureMouse = (g.CaptureMouseNextFrame != 0);
            //else
            //    io.WantCaptureMouse = (mouse_avail_to_imgui && (g.HoveredWindow != null || mouse_any_down)) || (g.ActiveId != 0) || (!g.OpenedPopupStack.empty());
            //io.WantCaptureKeyboard = (g.CaptureKeyboardNextFrame != -1) ? (g.CaptureKeyboardNextFrame != 0) : (g.ActiveId != 0);
            //io.WantTextInput = (g.ActiveId != 0 && g.InputTextState.Id == g.ActiveId);
            //g.MouseCursor = ImGuiMouseCursor.ImGuiMouseCursor_Arrow;
            //g.CaptureMouseNextFrame = g.CaptureKeyboardNextFrame = -1;
            //g.OsImePosRequest = new ImVec2(1.0f, 1.0f); // OS Input Method Editor showing on top-left of our window by default

            //// If mouse was first clicked outside of ImGui bounds we also cancel out hovering.
            //if (!mouse_avail_to_imgui)
            //    g.HoveredWindow = g.HoveredRootWindow = null;

            //// Scale & Scrolling
            //if (g.HoveredWindow != null && io.MouseWheel != 0.0f && !g.HoveredWindow.Collapsed)
            //{
            //    ImGuiWindow window = g.HoveredWindow;
            //    if (io.KeyCtrl)
            //    {
            //        if (io.FontAllowUserScaling)
            //        {
            //            // Zoom / Scale window
            //            float new_font_scale = ImGui.Clamp(window.FontWindowScale + io.MouseWheel * 0.10f, 0.50f, 2.50f);
            //            float scale = new_font_scale / window.FontWindowScale;
            //            window.FontWindowScale = new_font_scale;

            //            ImVec2 offset = window.Size * (1.0f - scale) * (io.MousePos - window.Pos) / window.Size;
            //            window.Pos += offset;
            //            window.PosFloat += offset;
            //            window.Size *= scale;
            //            window.SizeFull *= scale;
            //        }
            //    }
            //    else
            //    {
            //        // Scroll
            //        if (!((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoScrollWithMouse) == ImGuiWindowFlags.ImGuiWindowFlags_NoScrollWithMouse))
            //        {
            //            int scroll_lines = ((window.Flags & ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) == ImGuiWindowFlags.ImGuiWindowFlags_ComboBox) ? 3 : 5;
            //            SetWindowScrollY(window, window.Scroll.y - io.MouseWheel * window.CalcFontSize() * scroll_lines);
            //        }
            //    }
            //}

            //// Pressing TAB activate widget focus
            //// NB: Don't discard FocusedWindow if it isn't active, so that a window that go on/off programatically won't lose its keyboard focus.
            //if (g.ActiveId == 0 && g.FocusedWindow != null && g.FocusedWindow.Active && IsKeyPressedMap(ImGuiKey.ImGuiKey_Tab, false))
            //    g.FocusedWindow.FocusIdxTabRequestNext = 0;

            //// Mark all windows as not visible
            //for (int i = 0; i != g.Windows.Size; i++)
            //{
            //    ImGuiWindow window = g.Windows[i];
            //    window.WasActive = window.Active;
            //    window.Active = false;
            //    window.Accessed = false;
            //}

            //// No window should be open at the beginning of the frame.
            //// But in order to allow the user to call NewFrame() multiple times without calling Render(), we are doing an explicit clear.
            //g.CurrentWindowStack.resize(0);
            //g.CurrentPopupStack.resize(0);
            //CloseInactivePopups();

            //// Create implicit window - we will only render it if the user has added something to it.
            //SetNextWindowSize(new ImVec2(400, 400), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
            //Begin("Debug");



            _uiState.hotitem = -1;
            blink++;
        }

        partial void platformupdate();

        public void render(object tex)
        {
            _cmdBuffer.reset(tex);


            if(io.IsKeyPressed('a', true))
            {
                System.Diagnostics.Debug.WriteLine("ping" + blink);
            }

            //_cmdBuffer.DrawRect(new ImRect(0, 0, 640, 480), new ImColor(bgcolor));

            //button(1, 50, 50);

            //button(2, 150, 50);

            //if (button(3, 50, 150))
            //{
            //    //bgcolor = (SDL_GetTicks() * 0xc0cac01a) | 0x77;
            //}

            //if (button(4, 150, 150))
            //{
            //    //exit(0);
            //}

            //int slidervalue = (int)(bgcolor >> 8 & 0xff);
            //if (slider(5, 500, 40, 255, ref slidervalue) == 1)
            //{
            //    bgcolor = (bgcolor & 0xffff00ff) | (uint)slidervalue << 8;
            //}

            //slidervalue = (int)((bgcolor >> 18) & 0x3f);
            //if (slider(6, 550, 40, 63, ref slidervalue) == 1)
            //{
            //    bgcolor = (bgcolor & 0xff00ffff) | (uint)(slidervalue << 18);
            //}

            //slidervalue = (int)((bgcolor >> 28) & 0xf);
            //if (slider(7, 600, 40, 15, ref slidervalue) == 1)
            //{
            //    bgcolor = (bgcolor & 0x00ffffff) | (uint)(slidervalue << 28);
            //}

            //textfield(8, 50, 250, ref name);

            //_cmdBuffer._drawList.AddPolyline(
            //     new ImVector<ImVec2>(
            //         new ImVec2(50, 100),
            //         new ImVec2(100, 100),
            //         new ImVec2(100, 200),
            //         new ImVec2(50, 200)
            //     ), 10, 0xffffaa55, true, 25, false);

            //_cmdBuffer._drawList.AddPolyline(
            //     new ImVector<ImVec2>(
            //         new ImVec2(50, 100),
            //         new ImVec2(100, 100),
            //         new ImVec2(100, 200),
            //         new ImVec2(50, 200)
            //     ), 10, 0x00ffffff, true, 25, true);

            _cmdBuffer._drawList.AddPolyline(
             new ImVector<ImVec2>(
                 new ImVec2(25, 25),
                 new ImVec2(25, 50),
                 new ImVec2(50, 50),
                 new ImVec2(50, 25)
             ), 10, 0x00ffffff, true, 10, false);
            //_cmdBuffer.drawrect(_uiState.mousex - 32, _uiState.mousey - 24, 100, 100, 0xffffff00 + (uint)(_uiState.mousedown ? 0xff : 0));
            platformrender();

            if (_uiState.mousedown == false)
            {
                _uiState.activeitem = 0;
            }
            else
            {
                if (_uiState.activeitem == 0)
                    _uiState.activeitem = -1;
            }

            _cmdBuffer.done();
        }

        // Simple scroll bar IMGUI widget
        public int slider(int id, int x, int y, int max, ref int value)
        {
            // If no widget has keyboard focus, take it
            if (_uiState.kbditem == 0)
                _uiState.kbditem = id;

            // If we have keyboard focus, show it
            if (_uiState.kbditem == id)
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x - 4, y - 4, 40, 280), new ImColor(0xff0000ff));

            // Calculate mouse cursor's relative y offset
            int ypos = ((256 - 16) * value) / max;

            // Check for hotness
            if (regionhit(x + 8, y + 8, 16, 255))
            {
                _uiState.hotitem = id;
                if (_uiState.activeitem == 0 && _uiState.mousedown)
                    _uiState.activeitem = id;
            }

            // Render the scrollbar
            _cmdBuffer.DrawRect(ImRect.FromDeminensions(x, y, 32, 256 + 16), new ImColor(0x777777FF));

            if (_uiState.activeitem == id || _uiState.hotitem == id)
            {
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 8, y + 8 + ypos, 16, 16), new ImColor(0xffffffff));
            }
            else
            {
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 8, y + 8 + ypos, 16, 16), new ImColor(0xaaaaaaff));
            }


            // If we have keyboard focus, we'll need to process the keys
            if (_uiState.kbditem == id)
            {
                if (_uiState.keytabpressed)
                {
                    // If tab is pressed, lose keyboard focus.
                    // Next widget will grab the focus.
                    _uiState.kbditem = 0;
                    // If shift was also pressed, we want to move focus
                    // to the previous widget instead.
                    if (_uiState.keyshiftdown)
                        _uiState.kbditem = _uiState.lastwidget;
                    // Also clear the key so that next widget
                    // won't process it
                    _uiState.keytabpressed = false;
                }

                if (_uiState.keyuppressed)
                {
                    if (value > 0)
                    {
                        value--;
                        return 1;
                    }
                }

                if (_uiState.keydownpressed)
                {
                    if (value < max)
                    {
                        value++;
                        return 1;
                    }
                }

            }

            _uiState.lastwidget = id;

            // Update widget value
            if (_uiState.activeitem == id)
            {
                int mousepos = _uiState.mousey - (y + 8);
                if (mousepos < 0) mousepos = 0;
                if (mousepos > 255) mousepos = 255;
                int v = (mousepos * max) / 255;
                if (v != value)
                {
                    value = v;
                    return 1;
                }
            }

            return 0;
        }

        public bool button(int id, int x, int y)
        {
            // If no widget has keyboard focus, take it
            if (_uiState.kbditem == 0)
                _uiState.kbditem = id;

            // If we have keyboard focus, show it
            if (_uiState.kbditem == id)
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x - 6, y - 6, 84, 68), new ImColor(0xff0000ff));

            if (regionhit(x, y, 64, 48))
            {
                _uiState.hotitem = id;
                if (_uiState.activeitem == 0 && _uiState.mousedown)
                    _uiState.activeitem = id;
            }

            if (_uiState.kbditem == id && _uiState.keyenterdown)
            {
                _uiState.activeitem = id;
                _uiState.hotitem = id;
            }

            _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 8, y + 8, 64, 48), new ImColor(0x000000ff));
            if (_uiState.hotitem == id)
            {
                if (_uiState.activeitem == id)
                {
                    // Button is both 'hot' and 'active'
                    _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 2, y + 2, 64, 48), new ImColor(0xffffffff));
                }
                else
                {
                    // Button is merely 'hot'
                    _cmdBuffer.DrawRect(ImRect.FromDeminensions(x, y, 64, 48), new ImColor(0xffffffff));
                }
            }
            else
            {
                // button is not hot, but it may be active   
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x, y, 64, 48), new ImColor(0xaaaaaaff));
            }

            // If we have keyboard focus, we'll need to process the keys
            if (_uiState.kbditem == id)
            {
                if (_uiState.keytabpressed)
                {
                    // If tab is pressed, lose keyboard focus.
                    // Next widget will grab the focus.
                    _uiState.kbditem = 0;

                    // If shift was also pressed, we want to move focus
                    // to the previous widget instead.
                    if (_uiState.keyshiftdown)
                        _uiState.kbditem = _uiState.lastwidget;

                    // Also clear the key so that next widget
                    // won't process it
                    _uiState.keytabpressed = false;
                }

                if (_uiState.keyenterpress)
                {
                    _uiState.keyenterpress = false;
                    // Had keyboard focus, received return,
                    // so we'll act as if we were clicked.
                    return true;
                }

            }

            _uiState.lastwidget = id;

            // If button is hot and active, but mouse button is not
            // down, the user must have clicked the button.
            if (_uiState.mousedown == false &&
                _uiState.hotitem == id &&
                _uiState.activeitem == id)
                return true;

            // Otherwise, no clicky.
            return false;
        }

        private int blink;

        int textfield(int id, int x, int y, ref string buffer)
        {
            int len = buffer.Length;
            int changed = 0;

            // Check for hotness
            if (regionhit(x - 4, y - 4, 30 * 14 + 8, 24 + 8))
            {
                _uiState.hotitem = id;
                if (_uiState.activeitem == 0 && _uiState.mousedown)
                    _uiState.activeitem = id;
            }

            // If no widget has keyboard focus, take it
            if (_uiState.kbditem == 0)
                _uiState.kbditem = id;

            // If we have keyboard focus, show it
            if (_uiState.kbditem == id)
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x - 6, y - 6, 30 * 14 + 12, 24 + 12), new ImColor(0xff0000ff));

            // Render the text field
            if (_uiState.activeitem == id || _uiState.hotitem == id)
            {
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x - 4, y - 4, 30 * 14 + 8, 24 + 8), new ImColor(0xaaaaaaff));
            }
            else
            {
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x - 4, y - 4, 30 * 14 + 8, 24 + 8), new ImColor(0x777777ff));
            }

            drawstring(buffer, x, y);

            // Render cursor if we have keyboard focus
            if (_uiState.kbditem == id && ((blink >> 4) & 1) == 1)
                drawstring("_", x + len * 14, y);

            // If we have keyboard focus, we'll need to process the keys
            if (_uiState.kbditem == id)
            {
                if (_uiState.keytabpressed)
                {
                    // If tab is pressed, lose keyboard focus.
                    // Next widget will grab the focus.
                    _uiState.kbditem = 0;
                    // If shift was also pressed, we want to move focus
                    // to the previous widget instead.
                    if (_uiState.keyshiftdown)
                        _uiState.kbditem = _uiState.lastwidget;
                    // Also clear the key so that next widget
                    // won't process it
                    _uiState.keytabpressed = false;
                }

                if (_uiState.keybackspacepressed)
                {
                    if (len > 0)
                    {
                        len--;
                        buffer = buffer.Substring(0, len);
                        changed = 1;
                    }
                }

                if (len > 0)
                {
                    buffer += _uiState.inputchar;
                    len = buffer.Length;
                    changed = 1;
                }
            }

            // If button is hot and active, but mouse button is not
            // down, the user must have clicked the widget; give it 
            // keyboard focus.
            if (_uiState.mousedown == false &&
              _uiState.hotitem == id &&
              _uiState.activeitem == id)
                _uiState.kbditem = id;

            _uiState.lastwidget = id;

            return changed;
        }

        public void drawchar(char ch, int x, int y)
        {
            if (ch == ' ') return;

            if (ch == '_')
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 1, y + 12, 12, 2), new ImColor(0xffffffff));
            else if (char.IsUpper(ch))
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 1, y, 12, 12), new ImColor(0xffffffff));
            else
                _cmdBuffer.DrawRect(ImRect.FromDeminensions(x + 1, y + 5, 12, 7), new ImColor(0xffffffff));
        }

        public void drawstring(string text, int x, int y)
        {
            foreach (var ch in text)
            {
                drawchar(ch, x, y);
                x += 14;
            }
        }

        public bool regionhit(int x, int y, int w, int h)
        {
            if (_uiState.mousex < x ||
                 _uiState.mousey < y ||
                 _uiState.mousex >= x + w ||
                 _uiState.mousey >= y + h)
                return false;
            return true;
        }

        partial void platformrender();

    }
}
