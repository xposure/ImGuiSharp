namespace ImGui
{
    // Windows data
    internal class ImGuiWindow
    {
        internal string Name;
        internal uint ID;
        internal ImGuiWindowFlags Flags;
        internal ImVec2 PosFloat;
        internal ImVec2 Pos;                                // Position rounded-up to nearest pixel
        internal ImVec2 Size;                               // Current size (==SizeFull or collapsed title bar size)
        internal ImVec2 SizeFull;                           // Size when non collapsed
        internal ImVec2 SizeContents;                       // Size of contents (== extents reach of the drawing cursor) from previous frame
        internal ImVec2 SizeContentsExplicit;               // Size of contents explicitly set by the user via SetNextWindowContentSize()
        internal ImVec2 WindowPadding;                      // Window padding at the time of begin. We need to lock it, in particular manipulation of the ShowBorder would have an effect
        internal uint MoveID;                             // == window->GetID("#MOVE")
        internal ImVec2 Scroll;
        internal ImVec2 ScrollTarget;                       // target scroll position. stored as cursor position with scrolling canceled out, so the highest point is always 0.0f. (FLT_MAX for no change)
        internal ImVec2 ScrollTargetCenterRatio;            // 0.0f = scroll so that target position is at top, 0.5f = scroll so that target position is centered
        internal bool ScrollbarX, ScrollbarY;
        internal ImVec2 ScrollbarSizes;
        internal float BorderSize;
        internal bool Active;                             // Set to true on Begin()
        internal bool WasActive;
        internal bool Accessed;                           // Set to true when any widget access the current window
        internal bool Collapsed;                          // Set when collapsing window to become only title-bar
        internal bool SkipItems;                          // == Visible && !Collapsed
        internal int BeginCount;                         // Number of Begin() during the current frame (generally 0 or 1, 1+ if appending via multiple Begin/End pairs)
        internal uint PopupID;                            // ID in the popup stack when this window is used as a popup/menu (because we use generic Name/ID for recycling)
        internal int AutoFitFramesX, AutoFitFramesY;
        internal bool AutoFitOnlyGrows;
        internal int AutoPosLastDirection;
        internal int HiddenFrames;
        internal int SetWindowPosAllowFlags;             // bit ImGuiSetCond_*** specify if SetWindowPos() call will succeed with this particular flag.
        internal int SetWindowSizeAllowFlags;            // bit ImGuiSetCond_*** specify if SetWindowSize() call will succeed with this particular flag.
        internal int SetWindowCollapsedAllowFlags;       // bit ImGuiSetCond_*** specify if SetWindowCollapsed() call will succeed with this particular flag.
        internal bool SetWindowPosCenterWanted;

        internal ImGuiDrawContext DC;                                 // Temporary per-window data, reset at the beginning of the frame
        internal ImVector<uint> IDStack;                            // ID stack. ID are hashes seeded with the value at the top of the stack
        internal ImRect ClipRect;                           // = DrawList->clip_rect_stack.back(). Scissoring / clipping rectangle. x1, y1, x2, y2.
        internal ImRect ClippedWindowRect;                  // = ClipRect just after setup in Begin()
        internal int LastFrameActive;
        internal float ItemWidthDefault;
        internal ImGuiSimpleColumns MenuColumns;                        // Simplified columns storage for menu items
        internal ImGuiStorage StateStorage;
        internal float FontWindowScale;                    // Scale multiplier per-window
        internal ImDrawList DrawList;
        internal ImGuiWindow RootWindow;
        internal ImGuiWindow RootNonPopupWindow;

        // Focus
        internal int FocusIdxAllCounter;                 // Start at -1 and increase as assigned via FocusItemRegister()
        internal int FocusIdxTabCounter;                 // (same, but only count widgets which you can Tab through)
        internal int FocusIdxAllRequestCurrent;          // Item being requested for focus
        internal int FocusIdxTabRequestCurrent;          // Tab-able item being requested for focus
        internal int FocusIdxAllRequestNext;             // Item being requested for focus, for next update (relies on layout to be stable between the frame pressing TAB and the next frame)
        internal int FocusIdxTabRequestNext;             // "

        internal ImGuiWindow(string name)
        {
            DC = new ImGuiDrawContext();
            IDStack = new ImVector<uint>();
            StateStorage = new ImGuiStorage();
            MenuColumns = new ImGuiSimpleColumns();
            DrawList = new ImDrawList();

            Name = name;

            ID = ImGui.Hash(0, name);
            IDStack.push_back(ID);
            MoveID = GetID("#MOVE");

            Flags = 0;
            PosFloat = Pos = new ImVec2(0.0f, 0.0f);
            Size = SizeFull = new ImVec2(0.0f, 0.0f);
            SizeContents = SizeContentsExplicit = new ImVec2(0.0f, 0.0f);
            WindowPadding = new ImVec2(0.0f, 0.0f);
            Scroll = new ImVec2(0.0f, 0.0f);
            ScrollTarget = new ImVec2(float.MaxValue, float.MaxValue);
            ScrollTargetCenterRatio = new ImVec2(0.5f, 0.5f);
            ScrollbarX = ScrollbarY = false;
            ScrollbarSizes = new ImVec2(0.0f, 0.0f);
            BorderSize = 0.0f;
            Active = WasActive = false;
            Accessed = false;
            Collapsed = false;
            SkipItems = false;
            BeginCount = 0;
            PopupID = 0;
            AutoFitFramesX = AutoFitFramesY = -1;
            AutoFitOnlyGrows = false;
            AutoPosLastDirection = -1;
            HiddenFrames = 0;
            SetWindowPosAllowFlags = SetWindowSizeAllowFlags = SetWindowCollapsedAllowFlags = (int)(ImGuiSetCond.ImGuiSetCond_Always | ImGuiSetCond.ImGuiSetCond_Once | ImGuiSetCond.ImGuiSetCond_FirstUseEver | ImGuiSetCond.ImGuiSetCond_Appearing);
            SetWindowPosCenterWanted = false;

            LastFrameActive = -1;
            ItemWidthDefault = 0.0f;
            FontWindowScale = 1.0f;

            RootWindow = null;
            RootNonPopupWindow = null;

            FocusIdxAllCounter = FocusIdxTabCounter = -1;
            FocusIdxAllRequestCurrent = FocusIdxTabRequestCurrent = int.MaxValue;
            FocusIdxAllRequestNext = FocusIdxTabRequestNext = int.MaxValue;
        }

        public void Dispose()
        {
            DrawList.Dispose();
            DrawList = null;
        }

        internal uint GetID(string str, int start = 0, int end = -1)
        {
            uint seed = IDStack.back();
            uint id = ImGui.Hash(seed, str, start, end);
            ImGui.Instance.KeepAliveID(id);
            return id;
        }

        internal ImRect Rect()
        {
            return new ImRect(Pos.x, Pos.y, Pos.x + Size.x, Pos.y + Size.y);
        }
        internal float CalcFontSize()
        {
            //TODO: Is it ok to access state like this?
            return ImGui.Instance.State.FontBaseSize * FontWindowScale;
        }
        internal float TitleBarHeight()
        {
            //TODO: Is it ok to access style like this?
            return ((Flags & ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) == ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar) ? 0.0f : (CalcFontSize() + ImGui.Instance.Style.FramePadding.y * 2.0f);
        }
        internal ImRect TitleBarRect()
        {
            return new ImRect(Pos, new ImVec2(Pos.x + SizeFull.x, Pos.y + TitleBarHeight()));
        }

        internal float MenuBarHeight()
        {
            //TODO: Is it ok to access style like this?
            return ((Flags & ImGuiWindowFlags.ImGuiWindowFlags_MenuBar) == ImGuiWindowFlags.ImGuiWindowFlags_MenuBar) ? (CalcFontSize() + ImGui.Instance.Style.FramePadding.y * 2.0f) : 0.0f;
        }

        internal ImRect MenuBarRect()
        {
            float y1 = Pos.y + TitleBarHeight();
            return new ImRect(Pos.x, y1, Pos.x + SizeFull.x, y1 + MenuBarHeight());
        }
    }
}
