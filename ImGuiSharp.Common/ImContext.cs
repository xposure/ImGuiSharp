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

        private ImDrawList _drawList = new ImDrawList();

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
        //public bool button(int id, int x, int y)
        //{

        //}

        public void update()
        {
            platformupdate();
            _uiState.hotitem = -1;
            blink++;
        }

        partial void platformupdate();

        public void render(object tex)
        {
            _cmdBuffer.reset(tex);

            
            _cmdBuffer.DrawRect(new ImRect(0, 0, 640, 480), new ImColor(bgcolor));

            button(1, 50, 50);

            button(2, 150, 50);

            if (button(3, 50, 150))
            {
                //bgcolor = (SDL_GetTicks() * 0xc0cac01a) | 0x77;
            }

            if (button(4, 150, 150))
            {
                //exit(0);
            }

            int slidervalue = (int)(bgcolor >> 8 & 0xff);
            if (slider(5, 500, 40, 255, ref slidervalue) == 1)
            {
                bgcolor = (bgcolor & 0xffff00ff) | (uint)slidervalue << 8;
            }

            slidervalue = (int)((bgcolor >> 18) & 0x3f);
            if (slider(6, 550, 40, 63, ref slidervalue) == 1)
            {
                bgcolor = (bgcolor & 0xff00ffff) | (uint)(slidervalue << 18);
            }

            slidervalue = (int)((bgcolor >> 28) & 0xf);
            if (slider(7, 600, 40, 15, ref slidervalue) == 1)
            {
                bgcolor = (bgcolor & 0x00ffffff) | (uint)(slidervalue << 28);
            }

            textfield(8, 50, 250, ref name);
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
