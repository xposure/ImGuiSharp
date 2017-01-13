using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ImGui
{
    public partial class ImCmdBuffer
    {
        private List<short> _indices = new List<short>();
        private List<VertexPositionColorTexture> _verts = new List<VertexPositionColorTexture>();

        public void render(GraphicsDevice gd)
        {
            _indices.Clear();
            _verts.Clear();

            var vertData = _drawList.VtxBuffer.Data;
            var idxData = _drawList.IdxBuffer.Data;

            for (var i = 0; i < vertData.Length && i < _drawList.VtxBuffer.Size; i++)
            {
                var v = vertData[i];
                _verts.Add(new VertexPositionColorTexture(new Vector3(v.pos.x, v.pos.y, 0), GetColor(v.col), new Vector2(v.uv.x, v.uv.y)));
            }

            for (var i = 0; i < idxData.Length && i < _drawList.IdxBuffer.Size; i++)
            {
                _indices.Add((short)(ushort)idxData[i]);
            }
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _verts.ToArray(), 0, _verts.Count, _indices.ToArray(), 0, _indices.Count / 3);

            //for (var i = 0; i < _cmds.Count; i++)
            //{
            //    var idx = _cmds[i];

            //    var type = (idx & 0xff);
            //    idx >>= 8;

            //    switch (type)
            //    {
            //        case 0:
            //            {
            //                var pos = (short)_verts.Count;
            //                var cmd = _drawRectCmds[idx];
            //                var color = GetColor(cmd.color);
            //                _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Min.x, cmd.rect.Min.y, 0), color, Vector2.Zero));
            //                _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Max.x, cmd.rect.Min.y, 0), color, Vector2.Zero));
            //                _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Max.x, cmd.rect.Max.y, 0), color, Vector2.Zero));
            //                _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Min.x, cmd.rect.Max.y, 0), color, Vector2.Zero));

            //                var posOrg = pos;
            //                _indices.Add(pos);
            //                _indices.Add(++pos);
            //                _indices.Add(++pos);
            //                _indices.Add(pos);
            //                _indices.Add(++pos);
            //                _indices.Add(posOrg);
            //            }
            //            break;
            //    }
            //}

            //gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _verts.ToArray(), 0, _verts.Count, _indices.ToArray(), 0, _indices.Count / 3);
        }

        private Color GetColor(uint color)
        {

            int b = (int)(color & 0xff);
            color >>= 8;

            int g = (int)(color & 0xff);
            color >>= 8;

            int r = (int)(color & 0xff);
            color >>= 8;

            int a = (int)(color & 0xff);


            return new Color(r, g, b, a);
        }
    }

    public partial class ImContext
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Texture2D _whiteTexture;
        private KeyboardState _kbLastState, _kbState;
        private MouseState _msLastState, _msState;
        private RenderTarget2D _renderTarget;
        private RasterizerState _rasterizerState;


        public ImContext(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _whiteTexture = new Texture2D(_graphicsDevice, 1, 1);
            _whiteTexture.SetData(new Color[1] { Color.White });
            _renderTarget = new RenderTarget2D(graphicsDevice, 640 / 8, 480 / 8);
            _rasterizerState = new RasterizerState();
            _rasterizerState.FillMode = FillMode.WireFrame;

            _kbState = Keyboard.GetState();
            _msState = Mouse.GetState();
        }

        private int scrollWheel = 0;
        //char[] input = new char[1024];

        // An umanaged function that retrieves the states of each key
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        #region Convert Key
        public static char ConvertKeyboardInput(Keys key, bool alt, bool shift, bool ctrl, bool caps)
        {
            var upperCase = caps ^ shift;
            switch (key)
            {
                //Alphabet keys
                case Keys.A: if (upperCase) { return 'A'; } else { return 'a'; }
                case Keys.B: if (upperCase) { return 'B'; } else { return 'b'; }
                case Keys.C: if (upperCase) { return 'C'; } else { return 'c'; }
                case Keys.D: if (upperCase) { return 'D'; } else { return 'd'; }
                case Keys.E: if (upperCase) { return 'E'; } else { return 'e'; }
                case Keys.F: if (upperCase) { return 'F'; } else { return 'f'; }
                case Keys.G: if (upperCase) { return 'G'; } else { return 'g'; }
                case Keys.H: if (upperCase) { return 'H'; } else { return 'h'; }
                case Keys.I: if (upperCase) { return 'I'; } else { return 'i'; }
                case Keys.J: if (upperCase) { return 'J'; } else { return 'j'; }
                case Keys.K: if (upperCase) { return 'K'; } else { return 'k'; }
                case Keys.L: if (upperCase) { return 'L'; } else { return 'l'; }
                case Keys.M: if (upperCase) { return 'M'; } else { return 'm'; }
                case Keys.N: if (upperCase) { return 'N'; } else { return 'n'; }
                case Keys.O: if (upperCase) { return 'O'; } else { return 'o'; }
                case Keys.P: if (upperCase) { return 'P'; } else { return 'p'; }
                case Keys.Q: if (upperCase) { return 'Q'; } else { return 'q'; }
                case Keys.R: if (upperCase) { return 'R'; } else { return 'r'; }
                case Keys.S: if (upperCase) { return 'S'; } else { return 's'; }
                case Keys.T: if (upperCase) { return 'T'; } else { return 't'; }
                case Keys.U: if (upperCase) { return 'U'; } else { return 'u'; }
                case Keys.V: if (upperCase) { return 'V'; } else { return 'v'; }
                case Keys.W: if (upperCase) { return 'W'; } else { return 'w'; }
                case Keys.X: if (upperCase) { return 'X'; } else { return 'x'; }
                case Keys.Y: if (upperCase) { return 'Y'; } else { return 'y'; }
                case Keys.Z: if (upperCase) { return 'Z'; } else { return 'z'; }

                //Decimal keys
                case Keys.D0: if (shift) { return ')'; } else { return '0'; }
                case Keys.D1: if (shift) { return '!'; } else { return '1'; }
                case Keys.D2: if (shift) { return '@'; } else { return '2'; }
                case Keys.D3: if (shift) { return '#'; } else { return '3'; }
                case Keys.D4: if (shift) { return '$'; } else { return '4'; }
                case Keys.D5: if (shift) { return '%'; } else { return '5'; }
                case Keys.D6: if (shift) { return '^'; } else { return '6'; }
                case Keys.D7: if (shift) { return '&'; } else { return '7'; }
                case Keys.D8: if (shift) { return '*'; } else { return '8'; }
                case Keys.D9: if (shift) { return '('; } else { return '9'; }

                //Decimal numpad keys
                case Keys.NumPad0: return '0';
                case Keys.NumPad1: return '1';
                case Keys.NumPad2: return '2';
                case Keys.NumPad3: return '3';
                case Keys.NumPad4: return '4';
                case Keys.NumPad5: return '5';
                case Keys.NumPad6: return '6';
                case Keys.NumPad7: return '7';
                case Keys.NumPad8: return '8';
                case Keys.NumPad9: return '9';

                //Special keys
                case Keys.OemTilde: if (shift) { return '~'; } else { return '`'; }
                case Keys.OemSemicolon: if (shift) { return ':'; } else { return ';'; }
                case Keys.OemQuotes: if (shift) { return '"'; } else { return '\''; }
                case Keys.OemQuestion: if (shift) { return '?'; } else { return '/'; }
                case Keys.OemPlus: if (shift) { return '+'; } else { return '='; }
                case Keys.OemPipe: if (shift) { return '|'; } else { return '\\'; }
                case Keys.OemPeriod: if (shift) { return '>'; } else { return '.'; }
                case Keys.OemOpenBrackets: if (shift) { return '{'; } else { return '['; }
                case Keys.OemCloseBrackets: if (shift) { return '}'; } else { return ']'; }
                case Keys.OemMinus: if (shift) { return '_'; } else { return '-'; }
                case Keys.OemComma: if (shift) { return '<'; } else { return ','; }
                case Keys.Space: return ' ';
            }

            return '\0';
        }
        #endregion

        public int count = 0;
        partial void platformupdate()
        {
            int w = _graphicsDevice.Viewport.Width, h = _graphicsDevice.Viewport.Height;
            int display_w = _graphicsDevice.Viewport.Width, display_h = _graphicsDevice.Viewport.Height;
            //int display_w = GraphicsDevice.PresentationParameters.BackBufferWidth, display_h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            var mouse = Mouse.GetState();
            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;
            io.MouseWheel = mouse.ScrollWheelValue > scrollWheel ? 1 : mouse.ScrollWheelValue < scrollWheel ? -1 : 0;
            scrollWheel = mouse.ScrollWheelValue;

            io.DisplaySize = new ImVec2(w, h);
            //io.DisplayFramebufferScale = new ImVec2(w > 0 ? ((float)display_w / w) : 0, h > 0 ? ((float)display_h / h) : 0);
            if (io.IsActive)
            {
                io.MousePos = new ImVec2(mouse.X, mouse.Y);
                io.MouseDrawCursor = true;
            }
            else
            {
                io.MousePos = new ImVec2(-1, -1);
                io.MouseDrawCursor = false;
            }

            var keyboard = Keyboard.GetState();
            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            var capslock = ((((ushort)GetKeyState(0x14)) & 0xffff) != 0);

            var keys = keyboard.GetPressedKeys();
            foreach (var key in keys)
            {
                var ch = ConvertKeyboardInput(key, io.KeyAlt, io.KeyShift, io.KeyCtrl, capslock);
                if (!io.KeyAlt && !io.KeyCtrl && (int)key < 0x80 && ch != 0)
                {
                    io.KeysDown[ch] = true;
                    io.AddInputCharacter(ch);
                }
                else
                    io.KeysDown[(int)key + 0xff] = true;
            }


            ////_kbLastState = _kbState;
            ////_msLastState = _msState;

            ////_kbState = Keyboard.GetState();
            ////_msState = Mouse.GetState();

            //_currentEvent.IsAltDown = _kbState.IsKeyDown(Keys.LeftAlt) | _kbState.IsKeyDown(Keys.RightAlt);
            //_currentEvent.IsShiftDown = _kbState.IsKeyDown(Keys.LeftShift) | _kbState.IsKeyDown(Keys.RightShift);
            //_currentEvent.IsControlDown = _kbState.IsKeyDown(Keys.LeftControl) | _kbState.IsKeyDown(Keys.RightControl);


            //_uiState.keyenterdown = _kbState.IsKeyDown(Keys.Enter);
            //_uiState.keyenterpress = _kbState.IsKeyUp(Keys.Enter) && _kbLastState.IsKeyDown(Keys.Enter);
            //_uiState.keyshiftdown = _kbState.IsKeyDown(Keys.LeftShift) || _kbState.IsKeyDown(Keys.RightShift);
            //_uiState.keytabpressed = _kbState.IsKeyUp(Keys.Tab) && _kbLastState.IsKeyDown(Keys.Tab);
            //_uiState.keybackspacepressed = _kbState.IsKeyUp(Keys.Back) && _kbLastState.IsKeyDown(Keys.Back);


            ////_uiState.keyuppressed = _kbState.IsKeyUp(Keys.Up) && _kbLastState.IsKeyDown(Keys.Up);
            ////_uiState.keydownpressed = _kbState.IsKeyUp(Keys.Down) && _kbLastState.IsKeyDown(Keys.Down);

            //_uiState.keyuppressed = _kbLastState.IsKeyDown(Keys.Up);
            //_uiState.keydownpressed = _kbLastState.IsKeyDown(Keys.Down);

            //var keys = _kbLastState.GetPressedKeys();
            //foreach (var key in keys)
            //{
            //    if (_kbState.IsKeyUp(key))
            //    {
            //        var str = key.ToString();
            //        if (str.Length == 1)
            //        {
            //            var ch = str[0];
            //            if (char.IsLetter(ch))
            //            {
            //                if (_uiState.keyshiftdown)
            //                    ch = (char)(ch + 32);

            //                _uiState.inputchar += ch;
            //            }
            //            else if (char.IsDigit(ch))
            //            {
            //                _uiState.inputchar += ch;
            //            }
            //            else
            //            {
            //                switch (ch)
            //                {
            //                    case '_':
            //                    case ' ':
            //                        _uiState.inputchar += ch;
            //                        break;
            //                }
            //            }
            //        }
            //    }
            //}

            //_uiState.mousedown = _msState.LeftButton == ButtonState.Pressed;
            //_uiState.mousex = _msState.Position.X;
            //_uiState.mousey = _msState.Position.Y;
        }

        partial void platformrender()
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Immediate,
                rasterizerState: _rasterizerState,
                blendState: BlendState.AlphaBlend);

            _graphicsDevice.Textures[0] = _whiteTexture;
            _cmdBuffer.render(_graphicsDevice);
            _spriteBatch.End();
            _graphicsDevice.Textures[0] = null;

            _graphicsDevice.SetRenderTarget(null);


            _spriteBatch.Begin(rasterizerState: RasterizerState.CullCounterClockwise);
            _spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, 640, 480), Color.White);
            _spriteBatch.End();

            // If no widget grabbed tab, clear focus
            if (_uiState.keytabpressed)
                _uiState.kbditem = 0;

            _uiState.inputchar = "";

        }
    }
}
