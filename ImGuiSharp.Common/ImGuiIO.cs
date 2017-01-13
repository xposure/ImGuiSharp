namespace ImGui
{
    using System;

    // Main configuration and I/O between your application and ImGui
    // This is where your app communicate with ImGui. Access via ImGui::GetIO().
    // Read 'Programmer guide' section in .cpp file for general usage.
    internal partial class ImGuiIO
    {
        //------------------------------------------------------------------
        // Settings (fill once)                 // Default value:
        //------------------------------------------------------------------

        internal bool IsActive;
        internal ImVec2 DisplaySize;              // <unset>              // Display size, in pixels. For clamping windows positions.
        internal float DeltaTime;                // = 1.0f/60.0f         // Time elapsed since last frame, in seconds.
        internal float IniSavingRate;            // = 5.0f               // Maximum time between saving positions/sizes to .ini file, in seconds.
        internal string IniFilename;              // = "imgui.ini"        // Path to .ini file. NULL to disable .ini saving.
        internal string LogFilename;              // = "imgui_log.txt"    // Path to .log file (default parameter to ImGui::LogToFile when no file is specified).
        internal float MouseDoubleClickTime;     // = 0.30f              // Time for a double-click, in seconds.
        internal float MouseDoubleClickMaxDist;  // = 6.0f               // Distance threshold to stay in to validate a double-click, in pixels.
        internal float MouseDragThreshold;       // = 6.0f               // Distance threshold before considering we are dragging
        internal int[] KeyMap = new int[(int)ImGuiKey.ImGuiKey_COUNT];   // <unset>              // Map of indices into the KeysDown[512] entries array
        internal float KeyRepeatDelay;           // = 0.250f             // When holding a key/button, time before it starts repeating, in seconds. (for actions where 'repeat' is active)
        internal float KeyRepeatRate;            // = 0.020f             // When holding a key/button, rate at which it repeats, in seconds.
                                                 //void*         UserData;                 // = NULL               // Store your own data for retrieval by callbacks.

        //internal ImFontAtlas Fonts;                    // <auto>               // Load and assemble one or more fonts into a single tightly packed texture. Output to Fonts array.
        internal float FontGlobalScale;          // = 1.0f               // Global scale all fonts
        internal bool FontAllowUserScaling;     // = false              // Allow user scaling text of individual window with CTRL+Wheel.
        internal ImVec2 DisplayFramebufferScale;  // = (1.0f,1.0f)        // For retina display or other situations where window coordinates are different from framebuffer coordinates. User storage only, presently not used by ImGui.
        internal ImVec2 DisplayVisibleMin;        // <unset> (0.0f,0.0f)  // If you use DisplaySize as a virtual space larger than your screen, set DisplayVisibleMin/Max to the visible area.
        internal ImVec2 DisplayVisibleMax;        // <unset> (0.0f,0.0f)  // If the values are the same, we defaults to Min=(0.0f) and Max=DisplaySize

        //------------------------------------------------------------------
        // User Functions
        //------------------------------------------------------------------

        ////// Rendering function, will be called in Render().
        ////// Alternatively you can keep this to NULL and call GetDrawData() after Render() to get the same pointer.
        ////// See example applications if you are unsure of how to implement this.
        //internal Action<ImDrawData> RenderDrawListsFn;

        //// Optional: access OS clipboard
        //// (default to use native Win32 clipboard on Windows, otherwise uses a private clipboard. Override to access OS clipboard on other architectures)
        internal Func<string> GetClipboardTextFn;

        internal Action<string> SetClipboardTextFn;

        //// Optional: notify OS Input Method Editor of the screen position of your cursor for text input position (e.g. when using Japanese/Chinese IME in Windows)
        //// (default to use native imm32 api on Windows)
        //void        (*ImeSetInputScreenPosFn)(int x, int y);
        //void*       ImeWindowHandle;            // (Windows) Set this to your HWND to get automatic IME cursor positioning.
        internal Action<int, int> ImeSetInputScreenPosFn;

        //IntPtr ImeWindowHandler;

        //------------------------------------------------------------------
        // Input - Fill before calling NewFrame()
        //------------------------------------------------------------------

        internal ImVec2 MousePos;                   // Mouse position, in pixels (set to -1,-1 if no mouse / on another screen, etc.)
        internal bool[] MouseDown = new bool[5];               // Mouse buttons: left, right, middle + extras. ImGui itself mostly only uses left button (BeginPopupContext** are using right button). Others buttons allows us to track if the mouse is being used by your application + available to user as a convenience via IsMouse** API.
        internal float MouseWheel;                 // Mouse wheel: 1 unit scrolls about 5 lines text.
        internal bool MouseDrawCursor;            // Request ImGui to draw a mouse cursor for you (if you are on a platform without a mouse cursor).
        internal bool KeyCtrl;                    // Keyboard modifier pressed: Control
        internal bool KeyShift;                   // Keyboard modifier pressed: Shift
        internal bool KeyAlt;                     // Keyboard modifier pressed: Alt
        internal bool[] KeysDown = new bool[512];              // Keyboard keys that are pressed (in whatever storage order you naturally have access to keyboard data)
        internal char[] InputCharacters = new char[16 + 1];      // List of characters input (translated by user from keypress+keyboard state). Fill using AddInputCharacter() helper.

        //// Functions
        //IMGUI_API void AddInputCharacter(ImWchar c);                        // Helper to add a new character into InputCharacters[]
        //IMGUI_API void AddInputCharactersUTF8(const char* utf8_chars);      // Helper to add new characters into InputCharacters[] from an UTF-8 string
        //IMGUI_API void ClearInputCharacters() { InputCharacters[0] = 0; }   // Helper to clear the text input buffer

        //------------------------------------------------------------------
        // Output - Retrieve after calling NewFrame(), you can use them to discard inputs or hide them from the rest of your application
        //------------------------------------------------------------------

        internal bool WantCaptureMouse;           // Mouse is hovering a window or widget is active (= ImGui will use your mouse input)
        internal bool WantCaptureKeyboard;        // Widget is active (= ImGui will use your keyboard input)
        internal bool WantTextInput;              // Some text input widget is active, which will read input characters from the InputCharacters array.
        internal float Framerate;                  // Framerate estimation, in frame per second. Rolling average estimation based on IO.DeltaTime over 120 frames
        internal int MetricsAllocs;              // Number of active memory allocations
        internal int MetricsRenderVertices;      // Vertices output during last call to Render()
        internal int MetricsRenderIndices;       // Indices output during last call to Render() = number of triangles * 3
        internal int MetricsActiveWindows;       // Number of visible windows (exclude child windows)

        //------------------------------------------------------------------
        // [Internal] ImGui will maintain those fields for you
        //------------------------------------------------------------------

        internal ImVec2 MousePosPrev;               // Previous mouse position
        internal ImVec2 MouseDelta;                 // Mouse delta. Note that this is zero if either current or previous position are negative to allow mouse enabling/disabling.
        internal bool[] MouseClicked = new bool[5];            // Mouse button went from !Down to Down
        internal ImVec2[] MouseClickedPos = new ImVec2[5];         // Position at time of clicking
        internal float[] MouseClickedTime = new float[5];        // Time of last click (used to figure out double-click)
        internal bool[] MouseDoubleClicked = new bool[5];      // Has mouse button been double-clicked?
        internal bool[] MouseReleased = new bool[5];           // Mouse button went from Down to !Down
        internal bool[] MouseDownOwned = new bool[5];          // Track if button was clicked inside a window. We don't request mouse capture from the application if click started outside ImGui bounds.
        internal float[] MouseDownDuration = new float[5];       // Duration the mouse button has been down (0.0f == just clicked)
        internal float[] MouseDownDurationPrev = new float[5];   // Previous time the mouse button has been down
        internal float[] MouseDragMaxDistanceSqr = new float[5]; // Squared maximum distance of how much mouse has traveled from the click point
        internal float[] KeysDownDuration = new float[512];      // Duration the keyboard key has been down (0.0f == just pressed)
        internal float[] KeysDownDurationPrev = new float[512];  // Previous duration the key has been down

        internal ImGuiIO()
        {
            DisplaySize = new ImVec2(-1.0f, -1.0f);
            DeltaTime = 1.0f / 60.0f;
            IniSavingRate = 5.0f;
            IniFilename = "imgui.ini";
            LogFilename = "imgui_log.txt";
            //TODO: We can't access instance here because it will get in a ctor loop, Fonts = ImGui.Instance.ImDefaultFontAtlas;
            //Fonts = ImGui.ImDefaultFontAtlas;
            FontGlobalScale = 1.0f;
            DisplayFramebufferScale = new ImVec2(1.0f, 1.0f);
            MousePos = new ImVec2(-1, -1);
            MousePosPrev = new ImVec2(-1, -1);
            MouseDoubleClickTime = 0.30f;
            MouseDoubleClickMaxDist = 6.0f;
            MouseDragThreshold = 6.0f;
            for (int i = 0; i < MouseDownDuration.Length; i++)
                MouseDownDuration[i] = MouseDownDurationPrev[i] = -1.0f;
            for (int i = 0; i < KeysDownDuration.Length; i++)
                KeysDownDuration[i] = KeysDownDurationPrev[i] = -1.0f;
            for (int i = 0; i < (int)ImGuiKey.ImGuiKey_COUNT; i++)
                KeyMap[i] = -1;
            KeyRepeatDelay = 0.500f;
            KeyRepeatRate = 0.070f;
            //TODO: UserData = null;

            // User functions
            //RenderDrawListsFn = null;
            GetClipboardTextFn = GetClipboardTextFn_DefaultImpl;   // Platform dependent default implementations
            SetClipboardTextFn = SetClipboardTextFn_DefaultImpl;
            ImeSetInputScreenPosFn = ImeSetInputScreenPosFn_DefaultImpl;
        }

        partial void PlatformInitialize();

        internal void AddInputCharacter(char ch)
        {
            for (var i = 0; i < InputCharacters.Length; i++)
            {
                if (InputCharacters[i] == 0)
                {
                    InputCharacters[i] = ch;
                    break;
                }
            }
        }

        // Test if mouse cursor is hovering given rectangle
        // NB- Rectangle is clipped by our current clip setting
        // NB- Expand the rectangle to be generous on imprecise inputs systems (g.Style.TouchExtraPadding)
        public bool IsMouseHoveringRect(ImVec2 r_min, ImVec2 r_max, bool clip = true)
        {
            //ImGuiWindow window = GetCurrentWindowRead();

            // Clip
            ImRect rect_clipped = new ImRect(r_min, r_max);
            return rect_clipped.Contains(MousePos);
            //if (clip)
            //    rect_clipped.Clip(window.ClipRect);

            //// Expand for touch input
            //ImRect rect_for_touch = new ImRect(rect_clipped.Min - g.Style.TouchExtraPadding, rect_clipped.Max + g.Style.TouchExtraPadding);
            //return rect_for_touch.Contains(g.IO.MousePos);
        }


        public bool IsKeyPressedMap(ImGuiKey key, bool repeat = true)
        {
            int key_index = KeyMap[(int)key];
            return IsKeyPressed(key_index, repeat);
        }

        public int GetKeyIndex(ImGuiKey key)
        {
            System.Diagnostics.Debug.Assert(key >= 0 && key < ImGuiKey.ImGuiKey_COUNT);
            return KeyMap[(int)key];
        }

        public bool IsKeyDown(int key_index)
        {
            if (key_index < 0) return false;
            System.Diagnostics.Debug.Assert(key_index >= 0 && key_index < KeysDown.Length);
            return KeysDown[key_index];
        }

        public bool IsKeyPressed(int key_index, bool repeat = true)
        {
            if (key_index < 0) return false;
            System.Diagnostics.Debug.Assert(key_index >= 0 && key_index < KeysDown.Length);
            float t = KeysDownDuration[key_index];
            if (t == 0.0f)
                return true;

            if (repeat && t > KeyRepeatDelay)
            {
                float delay = KeyRepeatDelay, rate = KeyRepeatRate;
                if ((ImMath.fmodf(t - delay, rate) > rate * 0.5f) != (ImMath.fmodf(t - delay - DeltaTime, rate) > rate * 0.5f))
                    return true;
            }
            return false;
        }

        public bool IsKeyReleased(int key_index)
        {
            if (key_index < 0) return false;
            System.Diagnostics.Debug.Assert(key_index >= 0 && key_index < KeysDown.Length);
            if (KeysDownDurationPrev[key_index] >= 0.0f && !KeysDown[key_index])
                return true;
            return false;
        }

        public bool IsMouseDown(int button)
        {
            System.Diagnostics.Debug.Assert(button >= 0 && button < MouseDown.Length);
            return MouseDown[button];
        }

        public bool IsMouseClicked(int button, bool repeat = false)
        {
            System.Diagnostics.Debug.Assert(button >= 0 && button < MouseDown.Length);
            float t = MouseDownDuration[button];
            if (t == 0.0f)
                return true;

            if (repeat && t > KeyRepeatDelay)
            {
                float delay = KeyRepeatDelay, rate = KeyRepeatRate;
                if ((ImMath.fmodf(t - delay, rate) > rate * 0.5f) != (ImMath.fmodf(t - delay - DeltaTime, rate) > rate * 0.5f))
                    return true;
            }

            return false;
        }

        public bool IsMouseReleased(int button)
        {
            System.Diagnostics.Debug.Assert(button >= 0 && button < MouseDown.Length);
            return MouseReleased[button];
        }

        public bool IsMouseDoubleClicked(int button)
        {
            System.Diagnostics.Debug.Assert(button >= 0 && button < MouseDown.Length);
            return MouseDoubleClicked[button];
        }

        public bool IsMouseDragging(int button = 0, float lock_threshold = -1f)
        {
            System.Diagnostics.Debug.Assert(button >= 0 && button < MouseDown.Length);
            if (!MouseDown[button])
                return false;
            if (lock_threshold < 0.0f)
                lock_threshold = MouseDragThreshold;
            return MouseDragMaxDistanceSqr[button] >= lock_threshold * lock_threshold;
        }

        private static string clipboard;
        private string GetClipboardTextFn_DefaultImpl()
        {
            return clipboard;
        }

        private void SetClipboardTextFn_DefaultImpl(string text)
        {
            clipboard = text;
        }

        private void ImeSetInputScreenPosFn_DefaultImpl(int x, int y)
        {
            //TODO: ImeSetInputScreenPosFn_DefaultImpl
        }
    }
}