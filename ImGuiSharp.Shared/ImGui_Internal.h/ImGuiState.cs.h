// Main state for ImGui
struct ImGuiState
{
	bool                    Initialized;
	ImGuiIO                 IO;
	ImGuiStyle              Style;
	ImFont*                 Font;                               // (Shortcut) == FontStack.empty() ? IO.Font : FontStack.back()
	float                   FontSize;                           // (Shortcut) == FontBaseSize * g.CurrentWindow->FontWindowScale == window->FontSize()
	float                   FontBaseSize;                       // (Shortcut) == IO.FontGlobalScale * Font->Scale * Font->FontSize. Size of characters.
	ImVec2                  FontTexUvWhitePixel;                // (Shortcut) == Font->TexUvWhitePixel

	float                   Time;
	int                     FrameCount;
	int                     FrameCountEnded;
	int                     FrameCountRendered;
	ImVector<ImGuiWindow*>  Windows;
	ImVector<ImGuiWindow*>  WindowsSortBuffer;
	ImGuiWindow*            CurrentWindow;                      // Being drawn into
	ImVector<ImGuiWindow*>  CurrentWindowStack;
	ImGuiWindow*            FocusedWindow;                      // Will catch keyboard inputs
	ImGuiWindow*            HoveredWindow;                      // Will catch mouse inputs
	ImGuiWindow*            HoveredRootWindow;                  // Will catch mouse inputs (for focus/move only)
	uint                 HoveredId;                          // Hovered widget
	bool                    HoveredIdAllowOverlap;
	uint                 HoveredIdPreviousFrame;
	uint                 ActiveId;                           // Active widget
	uint                 ActiveIdPreviousFrame;
	bool                    ActiveIdIsAlive;
	bool                    ActiveIdIsJustActivated;            // Set at the time of activation for one frame
	bool                    ActiveIdAllowOverlap;               // Set only by active widget
	ImGuiWindow*            ActiveIdWindow;
	ImGuiWindow*            MovedWindow;                        // Track the child window we clicked on to move a window. Pointer is only valid if ActiveID is the "#MOVE" identifier of a window.
	ImVector<ImGuiIniData>  Settings;                           // .ini Settings
	float                   SettingsDirtyTimer;                 // Save .ini settinngs on disk when time reaches zero
	ImVector<ImGuiColMod>   ColorModifiers;                     // Stack for PushStyleColor()/PopStyleColor()
	ImVector<ImGuiStyleMod> StyleModifiers;                     // Stack for PushStyleVar()/PopStyleVar()
	ImVector<ImFont*>       FontStack;                          // Stack for PushFont()/PopFont()
	ImVector<ImGuiPopupRef> OpenedPopupStack;                   // Which popups are open (persistent)
	ImVector<ImGuiPopupRef> CurrentPopupStack;                  // Which level of BeginPopup() we are in (reset every frame)

																// Storage for SetNexWindow** and SetNextTreeNode*** functions
	ImVec2                  SetNextWindowPosVal;
	ImVec2                  SetNextWindowSizeVal;
	ImVec2                  SetNextWindowContentSizeVal;
	bool                    SetNextWindowCollapsedVal;
	ImGuiSetCond            SetNextWindowPosCond;
	ImGuiSetCond            SetNextWindowSizeCond;
	ImGuiSetCond            SetNextWindowContentSizeCond;
	ImGuiSetCond            SetNextWindowCollapsedCond;
	bool                    SetNextWindowFocus;
	bool                    SetNextTreeNodeOpenedVal;
	ImGuiSetCond            SetNextTreeNodeOpenedCond;

	// Render
	ImDrawData              RenderDrawData;                     // Main ImDrawData instance to pass render information to the user
	ImVector<ImDrawList*>   RenderDrawLists[3];
	float                   ModalWindowDarkeningRatio;
	ImDrawList              OverlayDrawList;                    // Optional software render of mouse cursors, if io.MouseDrawCursor is set + a few debug overlays
	ImGuiMouseCursor        MouseCursor;
	ImGuiMouseCursorData    MouseCursorData[ImGuiMouseCursor_Count_];

	// Widget state
	ImGuiTextEditState      InputTextState;
	ImFont                  InputTextPasswordFont;
	uint                 ScalarAsInputTextId;                // Temporary text input when CTRL+clicking on a slider, etc.
	ImGuiStorage            ColorEditModeStorage;               // Store user selection of color edit mode
	ImVec2                  ActiveClickDeltaToCenter;
	float                   DragCurrentValue;                   // Currently dragged value, always float, not rounded by end-user precision settings
	ImVec2                  DragLastMouseDelta;
	float                   DragSpeedDefaultRatio;              // If speed == 0.0f, uses (max-min) * DragSpeedDefaultRatio
	float                   DragSpeedScaleSlow;
	float                   DragSpeedScaleFast;
	ImVec2                  ScrollbarClickDeltaToGrabCenter;   // Distance between mouse and center of grab box, normalized in parent space. Use storage?
	char                    Tooltip[1024];
	char*                   PrivateClipboard;                   // If no custom clipboard handler is defined
	ImVec2                  OsImePosRequest, OsImePosSet;       // Cursor position request & last passed to the OS Input Method Editor

																// Logging
	bool                    LogEnabled;
	FILE*                   LogFile;                            // If != NULL log to stdout/ file
	ImGuiTextBuffer*        LogClipboard;                       // Else log to clipboard. This is pointer so our GImGui static constructor doesn't call heap allocators.
	int                     LogStartDepth;
	int                     LogAutoExpandMaxDepth;

	// Misc
	float                   FramerateSecPerFrame[120];          // calculate estimate of framerate for user
	int                     FramerateSecPerFrameIdx;
	float                   FramerateSecPerFrameAccum;
	int                     CaptureMouseNextFrame;              // explicit capture via CaptureInputs() sets those flags
	int                     CaptureKeyboardNextFrame;
	char                    TempBuffer[1024 * 3 + 1];               // temporary text buffer

	ImGuiState()
	{
		Initialized = false;
		Font = NULL;
		FontSize = FontBaseSize = 0.0f;
		FontTexUvWhitePixel = ImVec2(0.0f, 0.0f);

		Time = 0.0f;
		FrameCount = 0;
		FrameCountEnded = FrameCountRendered = -1;
		CurrentWindow = NULL;
		FocusedWindow = NULL;
		HoveredWindow = NULL;
		HoveredRootWindow = NULL;
		HoveredId = 0;
		HoveredIdAllowOverlap = false;
		HoveredIdPreviousFrame = 0;
		ActiveId = 0;
		ActiveIdPreviousFrame = 0;
		ActiveIdIsAlive = false;
		ActiveIdIsJustActivated = false;
		ActiveIdAllowOverlap = false;
		ActiveIdWindow = NULL;
		MovedWindow = NULL;
		SettingsDirtyTimer = 0.0f;

		SetNextWindowPosVal = ImVec2(0.0f, 0.0f);
		SetNextWindowSizeVal = ImVec2(0.0f, 0.0f);
		SetNextWindowCollapsedVal = false;
		SetNextWindowPosCond = 0;
		SetNextWindowSizeCond = 0;
		SetNextWindowContentSizeCond = 0;
		SetNextWindowCollapsedCond = 0;
		SetNextWindowFocus = false;
		SetNextTreeNodeOpenedVal = false;
		SetNextTreeNodeOpenedCond = 0;

		ScalarAsInputTextId = 0;
		ActiveClickDeltaToCenter = ImVec2(0.0f, 0.0f);
		DragCurrentValue = 0.0f;
		DragLastMouseDelta = ImVec2(0.0f, 0.0f);
		DragSpeedDefaultRatio = 0.01f;
		DragSpeedScaleSlow = 0.01f;
		DragSpeedScaleFast = 10.0f;
		ScrollbarClickDeltaToGrabCenter = ImVec2(0.0f, 0.0f);
		memset(Tooltip, 0, sizeof(Tooltip));
		PrivateClipboard = NULL;
		OsImePosRequest = OsImePosSet = ImVec2(-1.0f, -1.0f);

		ModalWindowDarkeningRatio = 0.0f;
		OverlayDrawList._OwnerName = "##Overlay"; // Give it a name for debugging
		MouseCursor = ImGuiMouseCursor_Arrow;
		memset(MouseCursorData, 0, sizeof(MouseCursorData));

		LogEnabled = false;
		LogFile = NULL;
		LogClipboard = NULL;
		LogStartDepth = 0;
		LogAutoExpandMaxDepth = 2;

		memset(FramerateSecPerFrame, 0, sizeof(FramerateSecPerFrame));
		FramerateSecPerFrameIdx = 0;
		FramerateSecPerFrameAccum = 0.0f;
		CaptureMouseNextFrame = CaptureKeyboardNextFrame = -1;
		memset(TempBuffer, 0, sizeof(TempBuffer));
	}
};