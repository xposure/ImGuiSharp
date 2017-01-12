namespace ImGui
{
    public unsafe delegate int ImGuiTextEditCallback(ImGuiTextEditCallbackData data);

    // Shared state of ImGui::InputText() when using custom callbacks (advanced)
    public struct ImGuiTextEditCallbackData
    {
        internal ImGuiInputTextFlags EventFlag;      // One of ImGuiInputTextFlags_Callback* // Read-only
        internal ImGuiInputTextFlags Flags;          // What user passed to InputText()      // Read-only
                                                     //internal void*               UserData;       // What user passed to InputText()      // Read-only
        internal bool ReadOnly;       // Read-only mode                       // Read-only

        // CharFilter event:
        internal char EventChar;      // Character input                      // Read-write (replace character or set to zero)

        // Completion,History,Always events:
        // If you modify the buffer contents make sure you update 'BufTextLen' and set 'BufDirty' to true.
        internal ImGuiKey EventKey;       // Key pressed (Up/Down/TAB)            // Read-only
        internal char[] Buf;            // Current text buffer                  // Read-write (pointed data only, can't replace the actual pointer)
        internal int BufTextLen;     // Current text length in bytes         // Read-write
        internal int BufSize;        // Maximum text length in bytes         // Read-only
        internal bool BufDirty;       // Set if you modify Buf/BufTextLen!!   // Write
        internal int CursorPos;      //                                      // Read-write
        internal int SelectionStart; //                                      // Read-write (== to SelectionEnd when no selection)
        internal int SelectionEnd;   //                                      // Read-write

        // NB: Helper functions for text manipulation. Calling those function loses selection.
        internal void DeleteChars(int pos, int bytes_count)
        {
            System.Diagnostics.Debug.Assert(pos + bytes_count <= BufTextLen);
            int dst = pos;
            int src = pos + bytes_count;
            char c;
            while ((c = Buf[src++]) != 0)
                Buf[dst++] = c;
            Buf[dst] = '\0';

            if (CursorPos + bytes_count >= pos)
                CursorPos -= bytes_count;
            else if (CursorPos >= pos)
                CursorPos = pos;
            SelectionStart = SelectionEnd = CursorPos;
            BufDirty = true;
            BufTextLen -= bytes_count;
        }

        internal void InsertChars(int pos, string data, int new_text, int new_text_end)
        {
            int new_text_len = new_text_end > -1 ? (new_text_end - new_text) : data.Length;
            if (new_text_len + BufTextLen + 1 >= BufSize)
                return;

            if (BufTextLen != pos)
            {
                //memmove(Buf + pos + new_text_len, Buf + pos, (size_t)(BufTextLen - pos));
                for (var i = pos; i < BufTextLen - pos; i++)
                    Buf[i + new_text_len] = Buf[i];

            }
            //memcpy(Buf + pos, new_text, (size_t)new_text_len * sizeof(char));
            for (var i = new_text; i < new_text_len; i++)
                Buf[pos + i] = data[i];
            Buf[BufTextLen + new_text_len] = '\0';

            if (CursorPos >= pos)
                CursorPos += new_text_len;
            SelectionStart = SelectionEnd = CursorPos;
            BufDirty = true;
            BufTextLen += new_text_len;
        }

        internal bool HasSelection() { return SelectionStart != SelectionEnd; }
    }
}
