using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ImGui.MonoGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private Matrix worldMatrix;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private BasicEffect basicEffect;
        private VertexDeclaration vertexDeclaration;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Texture2D texture2;
        private ImGuiIO io;
        private VertexPositionColorTexture[] verts;
        private ushort[] indices;
        private Texture2D white;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //this.IsFixedTimeStep = false;
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

            viewMatrix = Matrix.CreateLookAt(
                new Vector3(0.0f, 0f, -1f),
                Vector3.Zero,
                Vector3.Up
                );
            viewMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreateOrthographic(
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height,
                0.001f, 10000.0f);
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);

            vertexDeclaration = VertexPositionTexture.VertexDeclaration;

            basicEffect = new BasicEffect(GraphicsDevice);

            //worldMatrix = Matrix.CreateTranslation((float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height, 0f);
            worldMatrix = Matrix.Identity;// Matrix.CreateTranslation(0, 0, 0f);
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            //// TODO: Add your initialization logic here

            // Application init
            io = ImGui.Instance.GetIO();
            io.DisplaySize.x = GraphicsDevice.Viewport.Width;
            io.DisplaySize.y = GraphicsDevice.Viewport.Height;
            io.IniFilename = "imgui.ini";

            CreateFontsTexture();

            basicEffect.TextureEnabled = true;

            white = new Texture2D(GraphicsDevice, 1, 1);
            white.SetData(new Color[] { Color.White });

            //GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            //GraphicsDevice.BlendState = new BlendState() {  }
            //GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None, ScissorTestEnable = true };

            verts = new VertexPositionColorTexture[1024];
            indices = new ushort[4096];


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

            base.Initialize();
        }

        bool CreateFontsTexture()
        {
            var io = ImGui.Instance.GetIO();

            int width, height;
            var data = io.Fonts.GetTexDataAsAlpha8(out width, out height);
            var color1 = new int[data.Length];
            var color2 = new int[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                //color1[i] = (data[i] << 24) + 0xff;
                color1[i] = (data[i] << 24) + 0xffffff;
                color2[i] = (0xff << 24) + (data[i] + (data[i] << 8) + (data[i] << 16));
            }

            texture = new Texture2D(GraphicsDevice, width, height);
            texture.SetData(color1);

            data = io.Fonts.GetTexDataAsARGB32(out width, out height);
            texture2 = new Texture2D(GraphicsDevice, width, height);
            texture2.SetData(data);


            io.Fonts.Fonts[0].FontSize = 13;
            var t = new Texture2D(GraphicsDevice, 2, 2);
            t.SetData(new Color[] { Color.Red, Color.Green, Color.Blue, Color.Pink });

            io.Fonts.TexID = new ImTextureID(texture2);
            basicEffect.Texture = texture2;

            vbuff = new VertexBuffer(GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4096, BufferUsage.WriteOnly);
            ibuff = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 4096 * 4, BufferUsage.WriteOnly);
            return true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        int scrollWheel = 0;
        char[] input = new char[1024];

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

            //if (ImGui.Instance.Begin("My window"))
            //{
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.Text("Hello, world.Hello, world.");
            //    ImGui.Instance.InputText("Input", input, input.Length);
            //    ImGui.Instance.LabelText("hello", "0.1234f");
            //    if (ImGui.Instance.Button("click me", new ImVec2(100, 0)))
            //    {

            //    }
            //}
            //ImGui.Instance.End();

            bool crap = true;
            ImGui.Instance.ShowTestWindow(ref crap);
            //ImGui.Instance.ShowMetricsWindow(ref crap);


            index++;
            base.Update(gameTime);

            //ImGui.Instance.EndFrame();
        }

        private void DrawTriangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            basicEffect.Texture = white;
            var verts = new VertexPositionColorTexture[3];
            verts[0] = new VertexPositionColorTexture(p0, Color.Red, new Microsoft.Xna.Framework.Vector2(0, 0));
            verts[1] = new VertexPositionColorTexture(p1, Color.Blue, new Microsoft.Xna.Framework.Vector2(1, 0));
            verts[2] = new VertexPositionColorTexture(p2, Color.Green, new Microsoft.Xna.Framework.Vector2(1, 1));
            var indices = new int[3];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(
                       PrimitiveType.TriangleList,
                       verts,
                       0,  // vertex buffer offset to add to each element of the index buffer
                       3,  // number of vertices in pointList
                       indices,  // the index buffer
                       0,  // first index element to read
                       1   // number of primitives to draw
                   );
            }
        }

        private int index = 0;
        VertexBuffer vbuff;
        IndexBuffer ibuff;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            ImGui.Instance.Render();
            var data = ImGui.Instance.GetDrawData();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            //int w = GraphicsDevice.Viewport.Width, h = GraphicsDevice.Viewport.Height;
            //DrawTriangle(new Vector3(0, 0, 0), new Vector3(100, 0, 0), new Vector3(0, 100, 0));
            //DrawTriangle(new Vector3(w, h, 0), new Vector3(w - 100, h, 0), new Vector3(w, h - 100, 0));


            for (var k = 0; k < data.CmdListsCount; k++)
            {
                var drawlist = data.CmdLists[k];
                if (verts == null || verts.Length < drawlist.VtxBuffer.Size)
                {                    
                    verts = new VertexPositionColorTexture[drawlist.VtxBuffer.Size * 3  / 2];
                    vbuff.Dispose();
                    vbuff = new VertexBuffer(this.GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, verts.Length, BufferUsage.WriteOnly);
                }
                for (var i = 0; i < drawlist.VtxBuffer.Size; i++)
                {
                    var v = drawlist.VtxBuffer[i];
                    //var p = new Vector3((int)v.pos.x, (int)v.pos.y, 0);
                    var p = new Vector3((int)System.Math.Round(v.pos.x, System.MidpointRounding.AwayFromZero), (int)System.Math.Round(v.pos.y, System.MidpointRounding.AwayFromZero), 0);
                    var uv = new Microsoft.Xna.Framework.Vector2(v.uv.x, v.uv.y);
                    var c = ImGui.ColorConvertU32ToFloat4(v.col);
                    verts[i] = new VertexPositionColorTexture(p, new Color(c.x, c.y, c.z, c.w), uv);
                }

                if (indices == null || indices.Length < drawlist.IdxBuffer.Size)
                {
                    indices = new ushort[drawlist.IdxBuffer.Size * 3 / 2];
                    ibuff.Dispose();
                    ibuff = new IndexBuffer(this.GraphicsDevice,IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
                }
                for (var i = 0; i < drawlist.IdxBuffer.Size; i++)
                    indices[i] = drawlist.IdxBuffer[i];

                vbuff.SetData(verts, 0, drawlist.VtxBuffer.Size);
                ibuff.SetData(indices, 0, drawlist.IdxBuffer.Size);

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

                            //GraphicsDevice.DrawUserIndexedPrimitives(
                            //       PrimitiveType.TriangleList,
                            //       verts,
                            //       0,  // vertex buffer offset to add to each element of the index buffer
                            //       verts.Length,  // number of vertices in pointList
                            //       indices,  // the index buffer
                            //       (ushort)idxoffset,  // first index element to read
                            //       (int)(pcmd.ElemCount / 3)   // number of primitives to draw
                            //   );
                        }
                    }
                    idxoffset += (int)pcmd.ElemCount;
                }
            }

            //basicEffect.Texture = white;
            //verts[0] = new VertexPositionColorTexture(new Vector3(0, 0, 0), Color.Red, new Vector2(0, 0));
            //verts[1] = new VertexPositionColorTexture(new Vector3(100, 0, 0), Color.Blue, new Vector2(1, 0));
            //verts[2] = new VertexPositionColorTexture(new Vector3(0, 100, 0), Color.Green, new Vector2(1, 1));
            //indices[0] = 0;
            //indices[1] = 1;
            //indices[2] = 2;

            //foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserIndexedPrimitives(
            //           PrimitiveType.TriangleList,
            //           verts,
            //           0,  // vertex buffer offset to add to each element of the index buffer
            //           3,  // number of vertices in pointList
            //           indices,  // the index buffer
            //           0,  // first index element to read
            //           1   // number of primitives to draw
            //       );
            //}

            ////spriteBatch.Begin();
            //var scale = 4;
            //if (((index / 60) % 2) == 0)
            //{
            //    spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
            //    spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width * scale, texture.Height * scale), Color.White);
            //}
            //else
            //{
            //    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //    spriteBatch.Draw(texture2, new Rectangle(0, 0, texture.Width * scale, texture.Height * scale), Color.White);
            //}
            //spriteBatch.End();
            //// 4) render & swap video buffers
            //ImGui::Render();

            base.Draw(gameTime);
        }
    }
}