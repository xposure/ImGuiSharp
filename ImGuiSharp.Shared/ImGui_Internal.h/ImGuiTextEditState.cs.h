// Internal state of the currently focused/edited text input box
struct IMGUI_API ImGuiTextEditState
{
	uint             Id;                         // widget id owning the text state
	ImVector<ImWchar>   Text;                       // edit buffer, we need to persist but can't guarantee the persistence of the user-provided buffer. so we copy into own buffer.
	ImVector<char>      InitialText;                // backup of end-user buffer at the time of focus (in UTF-8, unaltered)
	ImVector<char>      TempTextBuffer;
	int                 CurLenA, CurLenW;           // we need to maintain our buffer length in both UTF-8 and wchar format.
	int                 BufSizeA;                   // end-user buffer size
	float               ScrollX;
	ImGuiStb::STB_TexteditState   StbState;
	float               CursorAnim;
	bool                CursorFollow;
	bool                SelectedAllMouseLock;

	ImGuiTextEditState() { memset(this, 0, sizeof(*this)); }
	void                CursorAnimReset() { CursorAnim = -0.30f; }                                   // After a user-input the cursor stays on for a while without blinking
	void                CursorClamp() { StbState.cursor = ImMin(StbState.cursor, CurLenW); StbState.select_start = ImMin(StbState.select_start, CurLenW); StbState.select_end = ImMin(StbState.select_end, CurLenW); }
	bool                HasSelection() const { return StbState.select_start != StbState.select_end; }
	void                ClearSelection() { StbState.select_start = StbState.select_end = StbState.cursor; }
	void                SelectAll() { StbState.select_start = 0; StbState.select_end = CurLenW; StbState.cursor = StbState.select_end; StbState.has_preferred_x = false; }
	void                OnKeyPressed(int key);
};