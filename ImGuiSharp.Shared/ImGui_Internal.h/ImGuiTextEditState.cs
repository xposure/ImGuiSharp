using STB_TEXTEDIT_POSITIONTYPE = System.Int32;
using STB_TEXTEDIT_CHARTYPE = System.Char;
using ImWchar = System.Char;
using STB_TEXTEDIT_STRING = ImGui.ImGuiTextEditState;


namespace ImGui
{
    internal class STB_TexteditState
    {
        /////////////////////
        //
        // internal data
        //

        internal int cursor;
        // position of the text cursor within the string

        internal int select_start;          // selection start point
        internal int select_end;
        // selection start and end point in characters; if equal, no selection.
        // note that start may be less than or greater than end (e.g. when
        // dragging the mouse, start is where the initial click was, and you
        // can drag in either direction)

        internal bool insert_mode;
        // each textfield keeps its own insert mode state. to keep an app-wide
        // insert mode, copy this value in/out of the app state

        /////////////////////
        //
        // private data
        //
        internal byte cursor_at_end_of_line; // not implemented yet
        internal byte initialized;
        internal byte has_preferred_x;
        internal byte single_line;
        internal byte padding1, padding2, padding3;
        internal float preferred_x; // this determines where the cursor up/down tries to seek to along x
        internal StbUndoState undostate = new StbUndoState();

        internal bool STB_TEXT_HAS_SELECTION()
        {
            return select_start != select_end;
        }
    }

    internal class StbUndoState
    {
        public const int STB_TEXTEDIT_UNDOSTATECOUNT = 99;
        public const int STB_TEXTEDIT_UNDOCHARCOUNT = 999;

        // private data
        internal StbUndoRecord[] undo_rec = new StbUndoRecord[STB_TEXTEDIT_UNDOSTATECOUNT];
        internal STB_TEXTEDIT_CHARTYPE[] undo_char = new STB_TEXTEDIT_CHARTYPE[STB_TEXTEDIT_UNDOCHARCOUNT];
        internal short undo_point, redo_point;
        internal short undo_char_point, redo_char_point;
    }

    internal struct StbUndoRecord
    {
        // private data
        internal STB_TEXTEDIT_POSITIONTYPE where;
        internal short insert_length;
        internal short delete_length;
        internal short char_storage;
    }



    // Internal state of the currently focused/edited text input box
    internal class ImGuiTextEditState
    {
        internal uint Id;                         // widget id owning the text state
        internal ImVector<char> Text;                       // edit buffer, we need to persist but can't guarantee the persistence of the user-provided buffer. so we copy into own buffer.
        internal ImVector<char> InitialText;                // backup of end-user buffer at the time of focus (in UTF-8, unaltered)
        internal ImVector<char> TempTextBuffer;
        internal int CurLenA, CurLenW;           // we need to maintain our buffer length in both UTF-8 and wchar format.
        internal int BufSizeA;                   // end-user buffer size
        internal float ScrollX;
        internal STB_TexteditState StbState;
        internal float CursorAnim;
        internal bool CursorFollow;
        internal bool SelectedAllMouseLock;


        internal ImGuiTextEditState()
        {
            Text = new ImVector<char>();
            InitialText = new ImVector<char>();
            TempTextBuffer = new ImVector<char>();
            StbState = new STB_TexteditState();
        }
        internal void CursorAnimReset() { CursorAnim = -0.30f; }                                   // After a user-input the cursor stays on for a while without blinking
        internal void CursorClamp() { StbState.cursor = ImGui.Min(StbState.cursor, CurLenW); StbState.select_start = ImGui.Min(StbState.select_start, CurLenW); StbState.select_end = ImGui.Min(StbState.select_end, CurLenW); }
        internal bool HasSelection() { return StbState.select_start != StbState.select_end; }
        internal void ClearSelection() { StbState.select_start = StbState.select_end = StbState.cursor; }
        internal void SelectAll() { StbState.select_start = 0; StbState.select_end = CurLenW; StbState.cursor = StbState.select_end; StbState.has_preferred_x = 0; }
        internal void OnKeyPressed(int key)
        {
            stb_textedit.stb_textedit_key(this, StbState, key);
            CursorFollow = true;
            CursorAnimReset();
        }
    }
}
