using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ImGui.MonoGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;

        private Matrix worldMatrix, viewMatrix, projectionMatrix;

        private BasicEffect basicEffect;
        private VertexBuffer vbuff;
        private IndexBuffer ibuff;

        private ImGuiIO io;
        private Texture2D defaultWhiteTexture, fontTexture;

        private int scrollWheel = 0;
        private char[] input = new char[1024];

        private float renderTime = 0f;

        private VertexDeclaration vertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SetupBasicShader();

            // Application init
            io = ImGui.Instance.GetIO();
            io.DisplaySize.x = GraphicsDevice.Viewport.Width;
            io.DisplaySize.y = GraphicsDevice.Viewport.Height;
            io.IniFilename = "imgui.ini";

            SetupFontTextures();
            SetupBuffers();

            //not sure if we really need pointclamp since we add a 1px padding around each glyph
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None, ScissorTestEnable = true };

            SetupKeyMappings();

            base.Initialize();
        }

        void SetupBasicShader()
        {
            viewMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);
            worldMatrix = Matrix.Identity;

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;
        }

        void SetupKeyMappings()
        {
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Tab] = (int)Keys.Tab + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_LeftArrow] = (int)Keys.Left + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_RightArrow] = (int)Keys.Right + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_DownArrow] = (int)Keys.Down + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_PageUp] = (int)Keys.PageUp + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_PageDown] = (int)Keys.PageDown + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Home] = (int)Keys.Home + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_End] = (int)Keys.End + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Delete] = (int)Keys.Delete + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Backspace] = (int)Keys.Back + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Enter] = (int)Keys.Enter + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Escape] = (int)Keys.Escape + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_A] = (int)Keys.A + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_C] = (int)Keys.C + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_V] = (int)Keys.V + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_X] = (int)Keys.X + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Y] = (int)Keys.Y + 0xff;
            io.KeyMap[(int)ImGuiKey.ImGuiKey_Z] = (int)Keys.Z + 0xff;
        }

        void SetupBuffers()
        {
            vbuff = new VertexBuffer(GraphicsDevice, vertexDeclaration, 4096, BufferUsage.WriteOnly);
            ibuff = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 4096 * 4, BufferUsage.WriteOnly);
        }

        bool SetupFontTextures()
        {
            var io = ImGui.Instance.GetIO();

            //font texture
            int width, height;
            var data = io.Fonts.GetTexDataAsAlpha8(out width, out height);

            data = io.Fonts.GetTexDataAsARGB32(out width, out height);
            fontTexture = new Texture2D(GraphicsDevice, width, height);
            fontTexture.SetData(data);

            io.Fonts.TexID = new ImTextureID(fontTexture);
            basicEffect.Texture = fontTexture;

            //default white texture
            defaultWhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            defaultWhiteTexture.SetData(new Color[] { Color.White });

            return true;
        }


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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            //int w = 1920, h = 1080;
            int w = GraphicsDevice.Viewport.Width, h = GraphicsDevice.Viewport.Height;
            int display_w = GraphicsDevice.Viewport.Width, display_h = GraphicsDevice.Viewport.Height;
            //int display_w = GraphicsDevice.PresentationParameters.BackBufferWidth, display_h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            io = ImGui.Instance.GetIO();
            io.DisplaySize = new ImVec2(w, h);
            //io.DisplayFramebufferScale = new ImVec2(w > 0 ? ((float)display_w / w) : 0, h > 0 ? ((float)display_h / h) : 0);
            //// 1) get low-level inputs (e.g. on Win32, GetKeyboardState(), or poll your events, etc.)
            //// TODO: fill all fields of IO structure and call NewFrame
            io.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (IsActive)
            {
                io.MousePos = new ImVec2(mouse.X, mouse.Y);
                io.MouseDrawCursor = true;
            }
            else
            {
                io.MousePos = new ImVec2(-1, -1);
                io.MouseDrawCursor = false;
            }

            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;
            io.MouseWheel = mouse.ScrollWheelValue > scrollWheel ? 1 : mouse.ScrollWheelValue < scrollWheel ? -1 : 0;
            scrollWheel = mouse.ScrollWheelValue;

            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            var capslock = ((((ushort)GetKeyState(0x14)) & 0xffff) != 0);

            var keys = keyboard.GetPressedKeys();
            for (var i = 0; i < io.KeysDown.Length; i++)
                io.KeysDown[i] = false;


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

            ImGui.Instance.NewFrame();

            bool crap = true;
            ImGui.Instance.ShowTestWindow(ref crap);

            base.Update(gameTime);
        }

        private void EnsureBuffers(int vertexCount, int indexCount)
        {
            if (vertexCount >= vbuff.VertexCount)
            {
                vbuff.Dispose();
                vbuff = new VertexBuffer(this.GraphicsDevice, vertexDeclaration, vertexCount * 3 / 2, BufferUsage.WriteOnly);
            }

            if (indexCount >= ibuff.IndexCount)
            {
                ibuff.Dispose();
                ibuff = new IndexBuffer(this.GraphicsDevice, IndexElementSize.SixteenBits, indexCount * 3 / 2, BufferUsage.WriteOnly);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            ImGui.Instance.Text("AVG Render: {0:0.0000}", renderTime);
            var timer = Stopwatch.StartNew();

            ImGui.Instance.Render();

            var data = ImGui.Instance.GetDrawData();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (var k = 0; k < data.CmdListsCount; k++)
            {
                var drawlist = data.CmdLists[k];

                EnsureBuffers(drawlist.VtxBuffer.Size, drawlist.IdxBuffer.Size);

                vbuff.SetData(drawlist.VtxBuffer.Data, 0, drawlist.VtxBuffer.Size);
                ibuff.SetData(drawlist.IdxBuffer.Data, 0, drawlist.IdxBuffer.Size);

                GraphicsDevice.SetVertexBuffer(vbuff);
                GraphicsDevice.Indices = ibuff;

                var idxoffset = 0;
                for (var j = 0; j < drawlist.CmdBuffer.Size; j++)
                {
                    var pcmd = drawlist.CmdBuffer[j];
                    if (pcmd.UserCallback != null)
                        pcmd.UserCallback(drawlist, pcmd);
                    else
                    {
                        GraphicsDevice.ScissorRectangle = new Rectangle((int)pcmd.ClipRect.x, (int)pcmd.ClipRect.y, (int)(pcmd.ClipRect.z - pcmd.ClipRect.x), (int)(pcmd.ClipRect.w - pcmd.ClipRect.y));
                        basicEffect.Texture = (Texture2D)pcmd.TextureId.Data;
                        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, idxoffset, (int)(pcmd.ElemCount / 3));
                        }
                    }
                    idxoffset += (int)pcmd.ElemCount;
                }
            }

            base.Draw(gameTime);

            timer.Stop();
            renderTime = renderTime * 0.8f + (float)timer.Elapsed.TotalSeconds * 0.2f;
        }
    }
}