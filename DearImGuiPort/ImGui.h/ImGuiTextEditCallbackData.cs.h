// Shared state of InputText(), passed to callback when a ImGuiInputTextFlags_Callback* flag is used and the corresponding callback is triggered.
struct ImGuiTextEditCallbackData
{
	ImGuiInputTextFlags EventFlag;      // One of ImGuiInputTextFlags_Callback* // Read-only
	ImGuiInputTextFlags Flags;          // What user passed to InputText()      // Read-only
	void*               UserData;       // What user passed to InputText()      // Read-only
	bool                ReadOnly;       // Read-only mode                       // Read-only

										// CharFilter event:
	ImWchar             EventChar;      // Character input                      // Read-write (replace character or set to zero)

										// Completion,History,Always events:
										// If you modify the buffer contents make sure you update 'BufTextLen' and set 'BufDirty' to true.
	ImGuiKey            EventKey;       // Key pressed (Up/Down/TAB)            // Read-only
	char*               Buf;            // Current text buffer                  // Read-write (pointed data only, can't replace the actual pointer)
	int                 BufTextLen;     // Current text length in bytes         // Read-write
	int                 BufSize;        // Maximum text length in bytes         // Read-only
	bool                BufDirty;       // Set if you modify Buf/BufTextLen!!   // Write
	int                 CursorPos;      //                                      // Read-write
	int                 SelectionStart; //                                      // Read-write (== to SelectionEnd when no selection)
	int                 SelectionEnd;   //                                      // Read-write

										// NB: Helper functions for text manipulation. Calling those function loses selection.
	void    DeleteChars(int pos, int bytes_count);
	void    InsertChars(int pos, const char* text, const char* text_end = NULL);
	bool    HasSelection() const { return SelectionStart != SelectionEnd; }
};