// This is where your app communicate with ImGui. Access via ImGui::GetIO().
// Read 'Programmer guide' section in .cpp file for general usage.
struct ImGuiIO
{
	//------------------------------------------------------------------
	// Settings (fill once)                 // Default value:
	//------------------------------------------------------------------

	ImVec2        DisplaySize;              // <unset>              // Display size, in pixels. For clamping windows positions.
	float         DeltaTime;                // = 1.0f/60.0f         // Time elapsed since last frame, in seconds.
	float         IniSavingRate;            // = 5.0f               // Maximum time between saving positions/sizes to .ini file, in seconds.
	const char*   IniFilename;              // = "imgui.ini"        // Path to .ini file. NULL to disable .ini saving.
	const char*   LogFilename;              // = "imgui_log.txt"    // Path to .log file (default parameter to ImGui::LogToFile when no file is specified).
	float         MouseDoubleClickTime;     // = 0.30f              // Time for a double-click, in seconds.
	float         MouseDoubleClickMaxDist;  // = 6.0f               // Distance threshold to stay in to validate a double-click, in pixels.
	float         MouseDragThreshold;       // = 6.0f               // Distance threshold before considering we are dragging
	int           KeyMap[ImGuiKey_COUNT];   // <unset>              // Map of indices into the KeysDown[512] entries array
	float         KeyRepeatDelay;           // = 0.250f             // When holding a key/button, time before it starts repeating, in seconds. (for actions where 'repeat' is active)
	float         KeyRepeatRate;            // = 0.020f             // When holding a key/button, rate at which it repeats, in seconds.
	void*         UserData;                 // = NULL               // Store your own data for retrieval by callbacks.

	ImFontAtlas*  Fonts;                    // <auto>               // Load and assemble one or more fonts into a single tightly packed texture. Output to Fonts array.
	float         FontGlobalScale;          // = 1.0f               // Global scale all fonts
	bool          FontAllowUserScaling;     // = false              // Allow user scaling text of individual window with CTRL+Wheel.
	ImVec2        DisplayFramebufferScale;  // = (1.0f,1.0f)        // For retina display or other situations where window coordinates are different from framebuffer coordinates. User storage only, presently not used by ImGui.
	ImVec2        DisplayVisibleMin;        // <unset> (0.0f,0.0f)  // If you use DisplaySize as a virtual space larger than your screen, set DisplayVisibleMin/Max to the visible area.
	ImVec2        DisplayVisibleMax;        // <unset> (0.0f,0.0f)  // If the values are the same, we defaults to Min=(0.0f) and Max=DisplaySize

											//------------------------------------------------------------------
											// User Functions
											//------------------------------------------------------------------

											// Rendering function, will be called in Render().
											// Alternatively you can keep this to NULL and call GetDrawData() after Render() to get the same pointer.
											// See example applications if you are unsure of how to implement this.
	void(*RenderDrawListsFn)(ImDrawData* data);

	// Optional: access OS clipboard
	// (default to use native Win32 clipboard on Windows, otherwise uses a private clipboard. Override to access OS clipboard on other architectures)
	const char* (*GetClipboardTextFn)();
	void(*SetClipboardTextFn)(const char* text);

	// Optional: override memory allocations. MemFreeFn() may be called with a NULL pointer.
	// (default to posix malloc/free)
	void*       (*MemAllocFn)(size_t sz);
	void(*MemFreeFn)(void* ptr);

	// Optional: notify OS Input Method Editor of the screen position of your cursor for text input position (e.g. when using Japanese/Chinese IME in Windows)
	// (default to use native imm32 api on Windows)
	void(*ImeSetInputScreenPosFn)(int x, int y);
	void*       ImeWindowHandle;            // (Windows) Set this to your HWND to get automatic IME cursor positioning.

											//------------------------------------------------------------------
											// Input - Fill before calling NewFrame()
											//------------------------------------------------------------------

	ImVec2      MousePos;                   // Mouse position, in pixels (set to -1,-1 if no mouse / on another screen, etc.)
	bool        MouseDown[5];               // Mouse buttons: left, right, middle + extras. ImGui itself mostly only uses left button (BeginPopupContext** are using right button). Others buttons allows us to track if the mouse is being used by your application + available to user as a convenience via IsMouse** API.
	float       MouseWheel;                 // Mouse wheel: 1 unit scrolls about 5 lines text.
	bool        MouseDrawCursor;            // Request ImGui to draw a mouse cursor for you (if you are on a platform without a mouse cursor).
	bool        KeyCtrl;                    // Keyboard modifier pressed: Control
	bool        KeyShift;                   // Keyboard modifier pressed: Shift
	bool        KeyAlt;                     // Keyboard modifier pressed: Alt
	bool        KeysDown[512];              // Keyboard keys that are pressed (in whatever storage order you naturally have access to keyboard data)
	ImWchar     InputCharacters[16 + 1];      // List of characters input (translated by user from keypress+keyboard state). Fill using AddInputCharacter() helper.

											  // Functions
	IMGUI_API void AddInputCharacter(ImWchar c);                        // Helper to add a new character into InputCharacters[]
	IMGUI_API void AddInputCharactersUTF8(const char* utf8_chars);      // Helper to add new characters into InputCharacters[] from an UTF-8 string
	IMGUI_API void ClearInputCharacters() { InputCharacters[0] = 0; }   // Helper to clear the text input buffer

																		//------------------------------------------------------------------
																		// Output - Retrieve after calling NewFrame(), you can use them to discard inputs or hide them from the rest of your application
																		//------------------------------------------------------------------

	bool        WantCaptureMouse;           // Mouse is hovering a window or widget is active (= ImGui will use your mouse input)
	bool        WantCaptureKeyboard;        // Widget is active (= ImGui will use your keyboard input)
	bool        WantTextInput;              // Some text input widget is active, which will read input characters from the InputCharacters array.
	float       Framerate;                  // Framerate estimation, in frame per second. Rolling average estimation based on IO.DeltaTime over 120 frames
	int         MetricsAllocs;              // Number of active memory allocations
	int         MetricsRenderVertices;      // Vertices output during last call to Render()
	int         MetricsRenderIndices;       // Indices output during last call to Render() = number of triangles * 3
	int         MetricsActiveWindows;       // Number of visible windows (exclude child windows)

											//------------------------------------------------------------------
											// [Internal] ImGui will maintain those fields for you
											//------------------------------------------------------------------

	ImVec2      MousePosPrev;               // Previous mouse position
	ImVec2      MouseDelta;                 // Mouse delta. Note that this is zero if either current or previous position are negative to allow mouse enabling/disabling.
	bool        MouseClicked[5];            // Mouse button went from !Down to Down
	ImVec2      MouseClickedPos[5];         // Position at time of clicking
	float       MouseClickedTime[5];        // Time of last click (used to figure out double-click)
	bool        MouseDoubleClicked[5];      // Has mouse button been double-clicked?
	bool        MouseReleased[5];           // Mouse button went from Down to !Down
	bool        MouseDownOwned[5];          // Track if button was clicked inside a window. We don't request mouse capture from the application if click started outside ImGui bounds.
	float       MouseDownDuration[5];       // Duration the mouse button has been down (0.0f == just clicked)
	float       MouseDownDurationPrev[5];   // Previous time the mouse button has been down
	float       MouseDragMaxDistanceSqr[5]; // Squared maximum distance of how much mouse has traveled from the click point
	float       KeysDownDuration[512];      // Duration the keyboard key has been down (0.0f == just pressed)
	float       KeysDownDurationPrev[512];  // Previous duration the key has been down

	IMGUI_API   ImGuiIO();
};