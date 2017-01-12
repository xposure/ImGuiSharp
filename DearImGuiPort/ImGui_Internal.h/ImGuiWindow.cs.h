// Windows data
struct IMGUI_API ImGuiWindow
{
	char*                   Name;
	uint                 ID;
	ImGuiWindowFlags        Flags;
	ImVec2                  PosFloat;
	ImVec2                  Pos;                                // Position rounded-up to nearest pixel
	ImVec2                  Size;                               // Current size (==SizeFull or collapsed title bar size)
	ImVec2                  SizeFull;                           // Size when non collapsed
	ImVec2                  SizeContents;                       // Size of contents (== extents reach of the drawing cursor) from previous frame
	ImVec2                  SizeContentsExplicit;               // Size of contents explicitly set by the user via SetNextWindowContentSize()
	ImVec2                  WindowPadding;                      // Window padding at the time of begin. We need to lock it, in particular manipulation of the ShowBorder would have an effect
	uint                 MoveID;                             // == window->GetID("#MOVE")
	ImVec2                  Scroll;
	ImVec2                  ScrollTarget;                       // target scroll position. stored as cursor position with scrolling canceled out, so the highest point is always 0.0f. (FLT_MAX for no change)
	ImVec2                  ScrollTargetCenterRatio;            // 0.0f = scroll so that target position is at top, 0.5f = scroll so that target position is centered
	bool                    ScrollbarX, ScrollbarY;
	ImVec2                  ScrollbarSizes;
	float                   BorderSize;
	bool                    Active;                             // Set to true on Begin()
	bool                    WasActive;
	bool                    Accessed;                           // Set to true when any widget access the current window
	bool                    Collapsed;                          // Set when collapsing window to become only title-bar
	bool                    SkipItems;                          // == Visible && !Collapsed
	int                     BeginCount;                         // Number of Begin() during the current frame (generally 0 or 1, 1+ if appending via multiple Begin/End pairs)
	uint                 PopupID;                            // ID in the popup stack when this window is used as a popup/menu (because we use generic Name/ID for recycling)
	int                     AutoFitFramesX, AutoFitFramesY;
	bool                    AutoFitOnlyGrows;
	int                     AutoPosLastDirection;
	int                     HiddenFrames;
	int                     SetWindowPosAllowFlags;             // bit ImGuiSetCond_*** specify if SetWindowPos() call will succeed with this particular flag.
	int                     SetWindowSizeAllowFlags;            // bit ImGuiSetCond_*** specify if SetWindowSize() call will succeed with this particular flag.
	int                     SetWindowCollapsedAllowFlags;       // bit ImGuiSetCond_*** specify if SetWindowCollapsed() call will succeed with this particular flag.
	bool                    SetWindowPosCenterWanted;

	ImGuiDrawContext        DC;                                 // Temporary per-window data, reset at the beginning of the frame
	ImVector<uint>       IDStack;                            // ID stack. ID are hashes seeded with the value at the top of the stack
	ImRect                  ClipRect;                           // = DrawList->clip_rect_stack.back(). Scissoring / clipping rectangle. x1, y1, x2, y2.
	ImRect                  ClippedWindowRect;                  // = ClipRect just after setup in Begin()
	int                     LastFrameActive;
	float                   ItemWidthDefault;
	ImGuiSimpleColumns      MenuColumns;                        // Simplified columns storage for menu items
	ImGuiStorage            StateStorage;
	float                   FontWindowScale;                    // Scale multiplier per-window
	ImDrawList*             DrawList;
	ImGuiWindow*            RootWindow;
	ImGuiWindow*            RootNonPopupWindow;

	// Focus
	int                     FocusIdxAllCounter;                 // Start at -1 and increase as assigned via FocusItemRegister()
	int                     FocusIdxTabCounter;                 // (same, but only count widgets which you can Tab through)
	int                     FocusIdxAllRequestCurrent;          // Item being requested for focus
	int                     FocusIdxTabRequestCurrent;          // Tab-able item being requested for focus
	int                     FocusIdxAllRequestNext;             // Item being requested for focus, for next update (relies on layout to be stable between the frame pressing TAB and the next frame)
	int                     FocusIdxTabRequestNext;             // "

public:
	ImGuiWindow(const char* name);
	~ImGuiWindow();

	uint     GetID(const char* str, const char* str_end = NULL);
	uint     GetID(const void* ptr);

	ImRect      Rect() const { return ImRect(Pos.x, Pos.y, Pos.x + Size.x, Pos.y + Size.y); }
	float       CalcFontSize() const { return GImGui->FontBaseSize * FontWindowScale; }
	float       TitleBarHeight() const { return (Flags & ImGuiWindowFlags_NoTitleBar) ? 0.0f : CalcFontSize() + GImGui->Style.FramePadding.y * 2.0f; }
	ImRect      TitleBarRect() const { return ImRect(Pos, ImVec2(Pos.x + SizeFull.x, Pos.y + TitleBarHeight())); }
	float       MenuBarHeight() const { return (Flags & ImGuiWindowFlags_MenuBar) ? CalcFontSize() + GImGui->Style.FramePadding.y * 2.0f : 0.0f; }
	ImRect      MenuBarRect() const { float y1 = Pos.y + TitleBarHeight(); return ImRect(Pos.x, y1, Pos.x + SizeFull.x, y1 + MenuBarHeight()); }
};