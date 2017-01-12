using System;
using System.Collections.Generic;
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

            for (var i = 0; i < _cmds.Count; i++)
            {
                var idx = _cmds[i];

                var type = (idx & 0xff);
                idx >>= 8;

                switch (type)
                {
                    case 0:
                        {
                            var pos = (short)_verts.Count;
                            var cmd = _drawRectCmds[idx];
                            var color = GetColor(cmd.color);
                            _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Min.x, cmd.rect.Min.y, 0), color, Vector2.Zero));
                            _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Max.x, cmd.rect.Min.y, 0), color, Vector2.Zero));
                            _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Max.x, cmd.rect.Max.y, 0), color, Vector2.Zero));
                            _verts.Add(new VertexPositionColorTexture(new Vector3(cmd.rect.Min.x, cmd.rect.Max.y, 0), color, Vector2.Zero));

                            var posOrg = pos;
                            _indices.Add(pos);
                            _indices.Add(++pos);
                            _indices.Add(++pos);
                            _indices.Add(pos);
                            _indices.Add(++pos);
                            _indices.Add(posOrg);
                        }
                        break;
                }
            }

            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _verts.ToArray(), 0, _verts.Count, _indices.ToArray(), 0, _indices.Count / 3);
        }

        private Color GetColor(uint color)
        {
            int a = (int)(color & 0xff);
            color >>= 8;

            int r = (int)(color & 0xff);
            color >>= 8;

            int g = (int)(color & 0xff);
            color >>= 8;

            int b = (int)(color & 0xff);


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

        public ImContext(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _whiteTexture = new Texture2D(_graphicsDevice, 1, 1);
            _whiteTexture.SetData(new Color[1] { Color.White });

            _kbState = Keyboard.GetState();
            _msState = Mouse.GetState();
        }


        partial void platformupdate()
        {
            _kbLastState = _kbState;
            _msLastState = _msState;

            _kbState = Keyboard.GetState();
            _msState = Mouse.GetState();


            _uiState.keyenterdown = _kbState.IsKeyDown(Keys.Enter);
            _uiState.keyenterpress = _kbState.IsKeyUp(Keys.Enter) && _kbLastState.IsKeyDown(Keys.Enter);
            _uiState.keyshiftdown = _kbState.IsKeyDown(Keys.LeftShift) || _kbState.IsKeyDown(Keys.RightShift);
            _uiState.keytabpressed = _kbState.IsKeyUp(Keys.Tab) && _kbLastState.IsKeyDown(Keys.Tab);
            _uiState.keybackspacepressed = _kbState.IsKeyUp(Keys.Back) && _kbLastState.IsKeyDown(Keys.Back);


            //_uiState.keyuppressed = _kbState.IsKeyUp(Keys.Up) && _kbLastState.IsKeyDown(Keys.Up);
            //_uiState.keydownpressed = _kbState.IsKeyUp(Keys.Down) && _kbLastState.IsKeyDown(Keys.Down);

            _uiState.keyuppressed = _kbLastState.IsKeyDown(Keys.Up);
            _uiState.keydownpressed = _kbLastState.IsKeyDown(Keys.Down);

            var keys = _kbLastState.GetPressedKeys();
            foreach (var key in keys)
            {
                if (_kbState.IsKeyUp(key))
                {
                    var str = key.ToString();
                    if (str.Length == 1)
                    {
                        var ch = str[0];
                        if (char.IsLetter(ch))
                        {
                            if (_uiState.keyshiftdown)
                                ch = (char)(ch + 32);

                            _uiState.inputchar += ch;
                        }
                        else if (char.IsDigit(ch))
                        {
                            _uiState.inputchar += ch;
                        }
                        else
                        {
                            switch (ch)
                            {
                                case '_':
                                case ' ':
                                    _uiState.inputchar += ch;
                                    break;
                            }
                        }
                    }
                }
            }

            _uiState.mousedown = _msState.LeftButton == ButtonState.Pressed;
            _uiState.mousex = _msState.Position.X;
            _uiState.mousey = _msState.Position.Y;
        }

        partial void platformrender()
        {
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate);
            _graphicsDevice.Textures[0] = _whiteTexture;
            _cmdBuffer.render(_graphicsDevice);
            _spriteBatch.End();

            //_graphicsDevice.Textures[0] = null;

            // If no widget grabbed tab, clear focus
            if (_uiState.keytabpressed)
                _uiState.kbditem = 0;

            _uiState.inputchar = "";

        }
    }
}
