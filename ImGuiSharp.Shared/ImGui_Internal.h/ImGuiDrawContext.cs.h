// Transient per-window data, reset at the beginning of the frame
// FIXME: That's theory, in practice the delimitation between ImGuiWindow and ImGuiDrawContext is quite tenuous and could be reconsidered.
struct IMGUI_API ImGuiDrawContext
{
	ImVec2                  CursorPos;
	ImVec2                  CursorPosPrevLine;
	ImVec2                  CursorStartPos;
	ImVec2                  CursorMaxPos;           // Implicitly calculate the size of our contents, always extending. Saved into window->SizeContents at the end of the frame
	float                   CurrentLineHeight;
	float                   CurrentLineTextBaseOffset;
	float                   PrevLineHeight;
	float                   PrevLineTextBaseOffset;
	float                   LogLinePosY;
	int                     TreeDepth;
	uint                 LastItemID;
	ImRect                  LastItemRect;
	bool                    LastItemHoveredAndUsable;  // Item rectangle is hovered, and its window is currently interactable with (not blocked by a popup preventing access to the window)
	bool                    LastItemHoveredRect;       // Item rectangle is hovered, but its window may or not be currently interactable with (might be blocked by a popup preventing access to the window)
	bool                    MenuBarAppending;
	float                   MenuBarOffsetX;
	ImVector<ImGuiWindow*>  ChildWindows;
	ImGuiStorage*           StateStorage;
	ImGuiLayoutType         LayoutType;

	// We store the current settings outside of the vectors to increase memory locality (reduce cache misses). The vectors are rarely modified. Also it allows us to not heap allocate for short-lived windows which are not using those settings.
	float                   ItemWidth;              // == ItemWidthStack.back(). 0.0: default, >0.0: width in pixels, <0.0: align xx pixels to the right of window
	float                   TextWrapPos;            // == TextWrapPosStack.back() [empty == -1.0f]
	bool                    AllowKeyboardFocus;     // == AllowKeyboardFocusStack.back() [empty == true]
	bool                    ButtonRepeat;           // == ButtonRepeatStack.back() [empty == false]
	ImVector<float>         ItemWidthStack;
	ImVector<float>         TextWrapPosStack;
	ImVector<bool>          AllowKeyboardFocusStack;
	ImVector<bool>          ButtonRepeatStack;
	ImVector<ImGuiGroupData>GroupStack;
	ImGuiColorEditMode      ColorEditMode;
	int                     StackSizesBackup[6];    // Store size of various stacks for asserting

	float                   IndentX;                // Indentation / start position from left of window (increased by TreePush/TreePop, etc.)
	float                   ColumnsOffsetX;         // Offset to the current column (if ColumnsCurrent > 0). FIXME: This and the above should be a stack to allow use cases like Tree->Column->Tree. Need revamp columns API.
	int                     ColumnsCurrent;
	int                     ColumnsCount;
	float                   ColumnsMinX;
	float                   ColumnsMaxX;
	float                   ColumnsStartPosY;
	float                   ColumnsCellMinY;
	float                   ColumnsCellMaxY;
	bool                    ColumnsShowBorders;
	uint                 ColumnsSetID;
	ImVector<ImGuiColumnData> ColumnsData;

	ImGuiDrawContext()
	{
		CursorPos = CursorPosPrevLine = CursorStartPos = CursorMaxPos = ImVec2(0.0f, 0.0f);
		CurrentLineHeight = PrevLineHeight = 0.0f;
		CurrentLineTextBaseOffset = PrevLineTextBaseOffset = 0.0f;
		LogLinePosY = -1.0f;
		TreeDepth = 0;
		LastItemID = 0;
		LastItemRect = ImRect(0.0f, 0.0f, 0.0f, 0.0f);
		LastItemHoveredAndUsable = LastItemHoveredRect = false;
		MenuBarAppending = false;
		MenuBarOffsetX = 0.0f;
		StateStorage = NULL;
		LayoutType = ImGuiLayoutType_Vertical;
		ItemWidth = 0.0f;
		ButtonRepeat = false;
		AllowKeyboardFocus = true;
		TextWrapPos = -1.0f;
		ColorEditMode = ImGuiColorEditMode_RGB;
		memset(StackSizesBackup, 0, sizeof(StackSizesBackup));

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
};