namespace ImGui
{

    // Transient per-window data, reset at the beginning of the frame
    // FIXME: That's theory, in practice the delimitation between ImGuiWindow and ImGuiDrawContext is quite tenuous and could be reconsidered.
    class ImGuiDrawContext
    {
        internal ImVec2 CursorPos;
        internal ImVec2 CursorPosPrevLine;
        internal ImVec2 CursorStartPos;
        internal ImVec2 CursorMaxPos;           // Implicitly calculate the size of our contents, always extending. Saved into window->SizeContents at the end of the frame
        internal float CurrentLineHeight;
        internal float CurrentLineTextBaseOffset;
        internal float PrevLineHeight;
        internal float PrevLineTextBaseOffset;
        internal float LogLinePosY;
        internal int TreeDepth;
        internal uint LastItemID;
        internal ImRect LastItemRect;
        internal bool LastItemHoveredAndUsable;  // Item rectangle is hovered, and its window is currently interactable with (not blocked by a popup preventing access to the window)
        internal bool LastItemHoveredRect;       // Item rectangle is hovered, but its window may or not be currently interactable with (might be blocked by a popup preventing access to the window)
        internal bool MenuBarAppending;
        internal float MenuBarOffsetX;
        internal ImVector<ImGuiWindow> ChildWindows;
        internal ImGuiStorage StateStorage;
        internal ImGuiLayoutType LayoutType;

        // We store the current settings outside of the vectors to increase memory locality (reduce cache misses). The vectors are rarely modified. Also it allows us to not heap allocate for short-lived windows which are not using those settings.
        internal float ItemWidth;              // == ItemWidthStack.back(). 0.0: default, >0.0: width in pixels, <0.0: align xx pixels to the right of window
        internal float TextWrapPos;            // == TextWrapPosStack.back() [empty == -1.0f]
        internal bool AllowKeyboardFocus;     // == AllowKeyboardFocusStack.back() [empty == true]
        internal bool ButtonRepeat;           // == ButtonRepeatStack.back() [empty == false]
        internal ImVector<float> ItemWidthStack;
        internal ImVector<float> TextWrapPosStack;
        internal ImVector<bool> AllowKeyboardFocusStack;
        internal ImVector<bool> ButtonRepeatStack;
        internal ImVector<ImGuiGroupData> GroupStack;
        internal ImGuiColorEditMode ColorEditMode;
        internal int[] StackSizesBackup = new int[6];    // Store size of various stacks for asserting

        internal float IndentX;                // Indentation / start position from left of window (increased by TreePush/TreePop, etc.)
        internal float ColumnsOffsetX;         // Offset to the current column (if ColumnsCurrent > 0). FIXME: This and the above should be a stack to allow use cases like Tree->Column->Tree. Need revamp columns API.
        internal int ColumnsCurrent;
        internal int ColumnsCount;
        internal float ColumnsMinX;
        internal float ColumnsMaxX;
        internal float ColumnsStartPosY;
        internal float ColumnsCellMinY;
        internal float ColumnsCellMaxY;
        internal bool ColumnsShowBorders;
        internal uint ColumnsSetID;
        internal ImVector<ImGuiColumnData> ColumnsData;

        internal ImGuiDrawContext()
        {
            ItemWidthStack = new ImVector<float>();
            TextWrapPosStack = new ImVector<float>();
            AllowKeyboardFocusStack = new ImVector<bool>();
            ButtonRepeatStack = new ImVector<bool>();
            GroupStack = new ImVector<ImGuiGroupData>();
            ChildWindows = new ImVector<ImGuiWindow>();
            ColumnsData = new ImVector<ImGuiColumnData>();

            CursorPos = CursorPosPrevLine = CursorStartPos = CursorMaxPos = new ImVec2(0.0f, 0.0f);
            CurrentLineHeight = PrevLineHeight = 0.0f;
            CurrentLineTextBaseOffset = PrevLineTextBaseOffset = 0.0f;
            LogLinePosY = -1.0f;
            TreeDepth = 0;
            LastItemID = 0;
            LastItemRect = new ImRect(0.0f, 0.0f, 0.0f, 0.0f);
            LastItemHoveredAndUsable = LastItemHoveredRect = false;
            MenuBarAppending = false;
            MenuBarOffsetX = 0.0f;
            StateStorage = null;
            LayoutType = ImGuiLayoutType.ImGuiLayoutType_Vertical;
            ItemWidth = 0.0f;
            ButtonRepeat = false;
            AllowKeyboardFocus = true;
            TextWrapPos = -1.0f;
            ColorEditMode = ImGuiColorEditMode.ImGuiColorEditMode_RGB;
            //memset(StackSizesBackup, 0, sizeof(StackSizesBackup));

            IndentX = 0.0f;
            ColumnsOffsetX = 0.0f;
            ColumnsCurrent = 0;
            ColumnsCount = 1;
            ColumnsMinX = ColumnsMaxX = 0.0f;
            ColumnsStartPosY = 0.0f;
            ColumnsCellMinY = ColumnsCellMaxY = 0.0f;
            ColumnsShowBorders = true;
            ColumnsSetID = 0;
        }
    }
}
