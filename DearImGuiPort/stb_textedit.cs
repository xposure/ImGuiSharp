using STB_TEXTEDIT_POSITIONTYPE = System.Int32;
using STB_TEXTEDIT_CHARTYPE = System.Char;
using ImWchar = System.Char;
using STB_TEXTEDIT_STRING = ImGui.ImGuiTextEditState;

namespace ImGui
{
    internal class stb_textedit
    {
        //        internal class ImGuiTextEditState
        //        {
        //            internal uint Id;                         // widget id owning the text state
        //            internal ImVector<ImWchar> Text;                       // edit buffer, we need to persist but can't guarantee the persistence of the user-provided buffer. so we copy into own buffer.
        //            internal ImVector<char> InitialText;                // backup of end-user buffer at the time of focus (in UTF-8, unaltered)
        //            internal ImVector<char> TempTextBuffer;
        //            internal int CurLenA, CurLenW;           // we need to maintain our buffer length in both UTF-8 and wchar format.
        //            internal int BufSizeA;                   // end-user buffer size
        //            internal float ScrollX;
        //            internal STB_TexteditState StbState;
        //            internal float CursorAnim;
        //            internal bool CursorFollow;
        //            internal bool SelectedAllMouseLock;


        //            internal ImGuiTextEditState()
        //            {
        //            }
        //            internal void CursorAnimReset() { CursorAnim = -0.30f; }                                   // After a user-input the cursor stays on for a while without blinking
        //            internal void CursorClamp() { StbState.cursor = ImGui.Min(StbState.cursor, CurLenW); StbState.select_start = ImGui.Min(StbState.select_start, CurLenW); StbState.select_end = ImGui.Min(StbState.select_end, CurLenW); }
        //            internal bool HasSelection() { return StbState.select_start != StbState.select_end; }
        //            internal void ClearSelection() { StbState.select_start = StbState.select_end = StbState.cursor; }
        //            internal void SelectAll() { StbState.select_start = 0; StbState.select_end = CurLenW; StbState.cursor = StbState.select_end; StbState.has_preferred_x = 0; }
        //            internal void OnKeyPressed(int key)
        //            {
        //                stb_textedit_key(this, StbState, key);
        //                CursorFollow = true;
        //                CursorAnimReset();
        //            }
        //        };

        private const float STB_TEXTEDIT_GETWIDTH_NEWLINE = -1.0f;
        private const int STB_TEXTEDIT_UNDOSTATECOUNT = 99;
        private const int STB_TEXTEDIT_UNDOCHARCOUNT = 999;

        // We don't use an enum so we can build even with conflicting symbols (if another user of stb_textedit.h leak their STB_TEXTEDIT_K_* symbols)
        internal const int STB_TEXTEDIT_K_LEFT = 0x10000; // keyboard input to move cursor left
        internal const int STB_TEXTEDIT_K_RIGHT = 0x10001; // keyboard input to move cursor right
        internal const int STB_TEXTEDIT_K_UP = 0x10002;// keyboard input to move cursor up
        internal const int STB_TEXTEDIT_K_DOWN = 0x10003;// keyboard input to move cursor down
        internal const int STB_TEXTEDIT_K_LINESTART = 0x10004; // keyboard input to move cursor to start of line
        internal const int STB_TEXTEDIT_K_LINEEND = 0x10005; // keyboard input to move cursor to end of line
        internal const int STB_TEXTEDIT_K_TEXTSTART = 0x10006; // keyboard input to move cursor to start of text
        internal const int STB_TEXTEDIT_K_TEXTEND = 0x10007; // keyboard input to move cursor to end of text
        internal const int STB_TEXTEDIT_K_DELETE = 0x10008; // keyboard input to delete selection or character under cursor
        internal const int STB_TEXTEDIT_K_BACKSPACE = 0x10009; // keyboard input to delete selection or character left of cursor
        internal const int STB_TEXTEDIT_K_UNDO = 0x1000A; // keyboard input to perform undo
        internal const int STB_TEXTEDIT_K_REDO = 0x1000B;// keyboard input to perform redo
        internal const int STB_TEXTEDIT_K_WORDLEFT = 0x1000C;// keyboard input to move cursor left one word
        internal const int STB_TEXTEDIT_K_WORDRIGHT = 0x1000D; // keyboard input to move cursor right one word
        internal const int STB_TEXTEDIT_K_SHIFT = 0x20000;
        //internal const int STB_TEXTEDIT_K_INSERT      //keyboard input to toggle insert mode
        //    STB_TEXTEDIT_IS_SPACE(ch)  true if character is whitespace (e.g. 'isspace'),
        //                                 required for WORDLEFT/WORDRIGHT
        //internal const int STB_TEXTEDIT_K_WORDLEFT    //keyboard input to move cursor left one word // e.g. ctrl-LEFT
        //internal const int STB_TEXTEDIT_K_WORDRIGHT   //keyboard input to move cursor right one word // e.g. ctrl-RIGHT


        //internal class STB_TexteditState
        //{
        //    /////////////////////
        //    //
        //    // internal data
        //    //

        //    internal int cursor;
        //    // position of the text cursor within the string

        //    internal int select_start;          // selection start point
        //    internal int select_end;
        //    // selection start and end point in characters; if equal, no selection.
        //    // note that start may be less than or greater than end (e.g. when
        //    // dragging the mouse, start is where the initial click was, and you
        //    // can drag in either direction)

        //    internal byte insert_mode;
        //    // each textfield keeps its own insert mode state. to keep an app-wide
        //    // insert mode, copy this value in/out of the app state

        //    /////////////////////
        //    //
        //    // private data
        //    //
        //    internal byte cursor_at_end_of_line; // not implemented yet
        //    internal byte initialized;
        //    internal byte has_preferred_x;
        //    internal byte single_line;
        //    internal byte padding1, padding2, padding3;
        //    internal float preferred_x; // this determines where the cursor up/down tries to seek to along x
        //    internal StbUndoState undostate;

        //    internal bool STB_TEXT_HAS_SELECTION()
        //    {
        //        return select_start != select_end;
        //    }
        //}


        ////////////////////////////////////////////////////////////////////////
        //
        //     StbTexteditRow
        //
        // Result of layout query, used by stb_textedit to determine where
        // the text in each row is.

        // result of layout query
        internal struct StbTexteditRow
        {
            internal float x0, x1;             // starting x location, end x location (allows for align=right, etc)
            internal float baseline_y_delta;  // position of baseline relative to previous row's baseline
            internal float ymin, ymax;         // height of row above and below baseline
            internal int num_chars;
        }

        internal struct StbFindState
        {
            internal float x, y;    // position of n'th character
            internal float height; // height of line
            internal int first_char, length; // first char of row, and length
            internal int prev_first;  // first char of previous row
        }

        static int STB_TEXTEDIT_STRINGLEN(STB_TEXTEDIT_STRING obj) { return obj.CurLenW; }
        static ImWchar STB_TEXTEDIT_GETCHAR(STB_TEXTEDIT_STRING obj, int idx) { return obj.Text[idx]; }
        static float STB_TEXTEDIT_GETWIDTH(STB_TEXTEDIT_STRING obj, int line_start_idx, int char_idx) { ImWchar c = obj.Text[line_start_idx + char_idx]; if (c == '\n') return STB_TEXTEDIT_GETWIDTH_NEWLINE; return ImGui.Instance.Font.GetCharAdvance(c) * (ImGui.Instance.FontSize / ImGui.Instance.Font.FontSize); }
        static int STB_TEXTEDIT_KEYTOTEXT(int key) { return key >= 0x10000 ? 0 : key; }
        static ImWchar STB_TEXTEDIT_NEWLINE = '\n';
        static void STB_TEXTEDIT_LAYOUTROW(ref StbTexteditRow r, STB_TEXTEDIT_STRING obj, int line_start_idx)
        {
            //TODO new string doesn't seem like a good idea here
            var text = new string(obj.Text.Data);
            //ImWchar* text = obj.Text.Data;
            int? text_remaining = 0;
            ImVec2? offset = null;
            ImVec2 size = ImGui.Instance.InputTextCalcTextSizeW(text, line_start_idx, obj.CurLenW, ref text_remaining, ref offset, true);
            r.x0 = 0.0f;
            r.x1 = size.x;
            r.baseline_y_delta = size.y;
            r.ymin = 0.0f;
            r.ymax = size.y;
            r.num_chars = (int)(text_remaining.Value - line_start_idx);
        }

        //        static bool is_separator(uint c) { return c == ',' || c == ';' || c == '(' || c == ')' || c == '{' || c == '}' || c == '[' || c == ']' || c == '|'; }
        static void STB_TEXTEDIT_DELETECHARS(STB_TEXTEDIT_STRING obj, int pos, int n)
        {
            int dst = pos;

            // We maintain our buffer length in both UTF-8 and wchar formats
            //obj.CurLenA -= obj.Text.Size - dst;

            obj.CurLenA -= n;
            //obj.CurLenA -= ImTextCountUtf8BytesFromStr(dst, dst + n);
            obj.CurLenW -= n;

            // Offset remaining text
            int src = pos + n;
            char c;
            while ((c = obj.Text[src++]) != 0)
                obj.Text[dst++] = c;

            //TODO should we just resize or use \0?
            //obj.Text.resize(dst);
            obj.Text[dst] = '\0';
        }

        static int memcpy<T>(T[] dstarr, T[] srcarr, int dest, int src, int n)
        {
            int dp = dest;
            int sp = src;
            while (n-- > 0)
                dstarr[dp++] = srcarr[sp++];
            return dest;
        }

        static int memmove<T>(T[] dstarr, T[] srcarr, int dest, int src, int n)
        {
            int pd = dest;
            int ps = src;
            if (ps < pd)
                for (pd += n, ps += n; n-- > 0;)
                    dstarr[--pd] = srcarr[--ps];
            else
                while (n-- > 0)
                    dstarr[pd++] = srcarr[ps++];
            return dest;
        }

        static bool STB_TEXTEDIT_INSERTCHARS(STB_TEXTEDIT_STRING obj, int pos, char[] new_text, int new_text_len, int offset = 0)
        {
            int text_len = obj.CurLenW;
            if (new_text_len + text_len + 1 > obj.Text.Size)
                return false;

            int new_text_len_utf8 = new_text_len;//= ImTextCountUtf8BytesFromStr(new_text, new_text + new_text_len);
            if (new_text_len_utf8 + obj.CurLenA + 1 > obj.BufSizeA)
                return false;

            if (pos != text_len)
                memmove(obj.Text.Data, obj.Text.Data, pos + new_text_len, pos + offset, (text_len - pos));
            memcpy(obj.Text.Data, new_text, pos, offset, new_text_len);

            obj.CurLenW += new_text_len;
            obj.CurLenA += new_text_len_utf8;
            obj.Text[obj.CurLenW] = '\0';

            return true;
        }


        /////////////////////////////////////////////////////////////////////////////
        //
        //      Mouse input handling
        //

        // traverse the layout to locate the nearest character to a display position
        static int stb_text_locate_coord(STB_TEXTEDIT_STRING str, float x, float y)
        {
            StbTexteditRow r = new StbTexteditRow();
            int n = STB_TEXTEDIT_STRINGLEN(str);
            float base_y = 0, prev_x;
            int i = 0, k;

            if (y < 0)
                return 0;

            r.x0 = r.x1 = 0;
            r.ymin = r.ymax = 0;
            r.num_chars = 0;

            // search rows to find one that straddles 'y'
            while (i < n)
            {
                STB_TEXTEDIT_LAYOUTROW(ref r, str, i);
                if (r.num_chars <= 0)
                    return n;

                if (y < base_y + r.ymax)
                    break;

                i += r.num_chars;
                base_y += r.baseline_y_delta;
            }

            // below all text, return 'after' last character
            if (i >= n)
                return n;

            // check if it's before the beginning of the line
            if (x < r.x0)
                return i;

            // check if it's before the end of the line
            if (x < r.x1)
            {
                // search characters in row for one that straddles 'x'
                k = i;
                prev_x = r.x0;
                for (i = 0; i < r.num_chars; ++i)
                {
                    float w = STB_TEXTEDIT_GETWIDTH(str, k, i);
                    if (x < prev_x + w)
                    {
                        if (x < prev_x + w / 2)
                            return k + i;
                        else
                            return k + i + 1;
                    }
                    prev_x += w;
                }
                // shouldn't happen, but if it does, fall through to end-of-line case
            }

            // if the last character is a newline, return that. otherwise return 'after' the last character
            if (STB_TEXTEDIT_GETCHAR(str, i + r.num_chars - 1) == STB_TEXTEDIT_NEWLINE)
                return i + r.num_chars - 1;
            else
                return i + r.num_chars;
        }

        // API click: on mouse down, move the cursor to the clicked location, and reset the selection
        internal static void stb_textedit_click(STB_TEXTEDIT_STRING str, STB_TexteditState state, float x, float y)
        {
            state.cursor = stb_text_locate_coord(str, x, y);
            state.select_start = state.cursor;
            state.select_end = state.cursor;
            state.has_preferred_x = 0;
        }

        // API drag: on mouse drag, move the cursor and selection endpoint to the clicked location
        internal static void stb_textedit_drag(STB_TEXTEDIT_STRING str, STB_TexteditState state, float x, float y)
        {
            int p = stb_text_locate_coord(str, x, y);
            if (state.select_start == state.select_end)
                state.select_start = state.cursor;
            state.cursor = state.select_end = p;
        }

        /////////////////////////////////////////////////////////////////////////////
        //
        //      Keyboard input handling
        //

        //// forward declarations
        //static void stb_text_undo(STB_TEXTEDIT_STRING str, STB_TexteditState state);
        //static void stb_text_redo(STB_TEXTEDIT_STRING str, STB_TexteditState state);
        //static void stb_text_makeundo_delete(STB_TEXTEDIT_STRING str, STB_TexteditState state, int where, int length);
        //static void stb_text_makeundo_insert(STB_TexteditState state, int where, int length);
        //static void stb_text_makeundo_replace(STB_TEXTEDIT_STRING str, STB_TexteditState state, int where, int old_length, int new_length);

        // find the x/y location of a character, and remember info about the previous row in
        // case we get a move-up event (for page up, we'll have to rescan)
        static void stb_textedit_find_charpos(ref StbFindState find, STB_TEXTEDIT_STRING str, int n, int single_line)
        {
            StbTexteditRow r = new StbTexteditRow();
            int prev_start = 0;
            int z = STB_TEXTEDIT_STRINGLEN(str);
            int i = 0, first;

            if (n == z)
            {
                // if it's at the end, then find the last line -- simpler than trying to
                // explicitly handle this case in the regular code
                if (single_line != 0)
                {
                    STB_TEXTEDIT_LAYOUTROW(ref r, str, 0);
                    find.y = 0;
                    find.first_char = 0;
                    find.length = z;
                    find.height = r.ymax - r.ymin;
                    find.x = r.x1;
                }
                else {
                    find.y = 0;
                    find.x = 0;
                    find.height = 1;
                    while (i < z)
                    {
                        STB_TEXTEDIT_LAYOUTROW(ref r, str, i);
                        prev_start = i;
                        i += r.num_chars;
                    }
                    find.first_char = i;
                    find.length = 0;
                    find.prev_first = prev_start;
                }
                return;
            }

            // search rows to find the one that straddles character n
            find.y = 0;

            for (;;)
            {
                STB_TEXTEDIT_LAYOUTROW(ref r, str, i);
                if (n < i + r.num_chars)
                    break;
                prev_start = i;
                i += r.num_chars;
                find.y += r.baseline_y_delta;
            }

            find.first_char = first = i;
            find.length = r.num_chars;
            find.height = r.ymax - r.ymin;
            find.prev_first = prev_start;

            // now scan to find xpos
            find.x = r.x0;
            i = 0;
            for (i = 0; first + i < n; ++i)
                find.x += STB_TEXTEDIT_GETWIDTH(str, first, i);
        }

        static bool STB_TEXT_HAS_SELECTION(STB_TexteditState s)
        {
            return s.STB_TEXT_HAS_SELECTION();
        }

        // make the selection/cursor state valid if client altered the string
        static void stb_textedit_clamp(STB_TEXTEDIT_STRING str, STB_TexteditState state)
        {
            int n = STB_TEXTEDIT_STRINGLEN(str);
            if (STB_TEXT_HAS_SELECTION(state))
            {
                if (state.select_start > n) state.select_start = n;
                if (state.select_end > n) state.select_end = n;
                // if clamping forced them to be equal, move the cursor to match
                if (state.select_start == state.select_end)
                    state.cursor = state.select_start;
            }
            if (state.cursor > n) state.cursor = n;
        }

        // delete characters while updating undo
        static void stb_textedit_delete(STB_TEXTEDIT_STRING str, STB_TexteditState state, int where, int len)
        {
            stb_text_makeundo_delete(str, state, where, len);
            STB_TEXTEDIT_DELETECHARS(str, where, len);
            state.has_preferred_x = 0;
        }

        // delete the section
        static void stb_textedit_delete_selection(STB_TEXTEDIT_STRING str, STB_TexteditState state)
        {
            stb_textedit_clamp(str, state);
            if (STB_TEXT_HAS_SELECTION(state))
            {
                if (state.select_start < state.select_end)
                {
                    stb_textedit_delete(str, state, state.select_start, state.select_end - state.select_start);
                    state.select_end = state.cursor = state.select_start;
                }
                else {
                    stb_textedit_delete(str, state, state.select_end, state.select_start - state.select_end);
                    state.select_start = state.cursor = state.select_end;
                }
                state.has_preferred_x = 0;
            }
        }

        // canoncialize the selection so start <= end
        static void stb_textedit_sortselection(STB_TexteditState state)
        {
            if (state.select_end < state.select_start)
            {
                int temp = state.select_end;
                state.select_end = state.select_start;
                state.select_start = temp;
            }
        }

        // move cursor to first character of selection
        static void stb_textedit_move_to_first(STB_TexteditState state)
        {
            if (STB_TEXT_HAS_SELECTION(state))
            {
                stb_textedit_sortselection(state);
                state.cursor = state.select_start;
                state.select_end = state.select_start;
                state.has_preferred_x = 0;
            }
        }

        // move cursor to last character of selection
        static void stb_textedit_move_to_last(STB_TEXTEDIT_STRING str, STB_TexteditState state)
        {
            if (STB_TEXT_HAS_SELECTION(state))
            {
                stb_textedit_sortselection(state);
                stb_textedit_clamp(str, state);
                state.cursor = state.select_end;
                state.select_start = state.select_end;
                state.has_preferred_x = 0;
            }
        }

        //        //# ifdef STB_TEXTEDIT_IS_SPACE
        static int is_word_boundary(STB_TEXTEDIT_STRING _str, int _idx)
        {
            return _idx > 0 ? (char.IsSeparator(STB_TEXTEDIT_GETCHAR(_str, _idx - 1)) && !char.IsSeparator(STB_TEXTEDIT_GETCHAR(_str, _idx))) ? 1 : 0 : 1;
        }

        static int stb_textedit_move_to_word_previous(STB_TEXTEDIT_STRING _str, STB_TexteditState _state)
        {
            int c = _state.cursor - 1;
            while (c >= 0 && is_word_boundary(_str, c) == 0)
                --c;

            if (c < 0)
                c = 0;

            return c;
        }

        static int stb_textedit_move_to_word_next(STB_TEXTEDIT_STRING _str, STB_TexteditState _state)
        {
            int len = STB_TEXTEDIT_STRINGLEN(_str);
            int c = _state.cursor + 1;
            while (c < len && is_word_boundary(_str, c) == 0)
                ++c;

            if (c > len)
                c = len;

            return c;
        }
        //        //#endif

        // update selection and cursor to match each other
        static void stb_textedit_prep_selection_at_cursor(STB_TexteditState state)
        {
            if (!STB_TEXT_HAS_SELECTION(state))
                state.select_start = state.select_end = state.cursor;
            else
                state.cursor = state.select_end;
        }

        // API cut: delete selection
        internal static int stb_textedit_cut(STB_TEXTEDIT_STRING str, STB_TexteditState state)
        {
            if (STB_TEXT_HAS_SELECTION(state))
            {
                stb_textedit_delete_selection(str, state); // implicity clamps
                state.has_preferred_x = 0;
                return 1;
            }
            return 0;
        }

        // API paste: replace existing selection with passed-in text
        internal static int stb_textedit_paste(STB_TEXTEDIT_STRING str, STB_TexteditState state, char[] text, int len, int offset = 0)
        {
            // if there's a selection, the paste should delete it
            stb_textedit_clamp(str, state);
            stb_textedit_delete_selection(str, state);
            // try to insert the characters
            if (STB_TEXTEDIT_INSERTCHARS(str, state.cursor, text, len, offset))
            {
                stb_text_makeundo_insert(state, state.cursor, len);
                state.cursor += len;
                state.has_preferred_x = 0;
                return 1;
            }
            // remove the undo since we didn't actually insert the characters
            if (state.undostate.undo_point != 0)
                --state.undostate.undo_point;
            return 0;
        }

        // API key: process a keyboard input
        public static void stb_textedit_key(STB_TEXTEDIT_STRING str, STB_TexteditState state, int key)
        {
            retry:
            switch (key)
            {
                default:
                    {
                        int c = STB_TEXTEDIT_KEYTOTEXT(key);
                        if (c > 0)
                        {
                            STB_TEXTEDIT_CHARTYPE ch = (STB_TEXTEDIT_CHARTYPE)c;

                            // can't add newline in single-line mode
                            if (c == '\n' && state.single_line != 0)
                                break;

                            if (state.insert_mode && !STB_TEXT_HAS_SELECTION(state) && state.cursor < STB_TEXTEDIT_STRINGLEN(str))
                            {
                                //TODO: stb_text_makeundo_replace(str, state, state.cursor, 1, 1);
                                STB_TEXTEDIT_DELETECHARS(str, state.cursor, 1);
                                if (STB_TEXTEDIT_INSERTCHARS(str, state.cursor, new char[] { ch }, 1))
                                {
                                    ++state.cursor;
                                    state.has_preferred_x = 0;
                                }
                            }
                            else
                            {
                                stb_textedit_delete_selection(str, state); // implicity clamps
                                if (STB_TEXTEDIT_INSERTCHARS(str, state.cursor, new char[] { ch }, 1))
                                {
                                    stb_text_makeundo_insert(state, state.cursor, 1);
                                    ++state.cursor;
                                    state.has_preferred_x = 0;
                                }
                            }
                        }
                        break;
                    }

                ////# ifdef STB_TEXTEDIT_K_INSERT
                //case STB_TEXTEDIT_K_INSERT:
                //    state.insert_mode = !state.insert_mode;
                //    break;
                ////#endif

                case STB_TEXTEDIT_K_UNDO:
                    stb_text_undo(str, state);
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_REDO:
                    stb_text_redo(str, state);
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_LEFT:
                    // if currently there's a selection, move cursor to start of selection
                    if (STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_move_to_first(state);
                    else
                       if (state.cursor > 0)
                        --state.cursor;
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_RIGHT:
                    // if currently there's a selection, move cursor to end of selection
                    if (STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_move_to_last(str, state);
                    else
                        ++state.cursor;
                    stb_textedit_clamp(str, state);
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_LEFT | STB_TEXTEDIT_K_SHIFT:
                    stb_textedit_clamp(str, state);
                    stb_textedit_prep_selection_at_cursor(state);
                    // move selection left
                    if (state.select_end > 0)
                        --state.select_end;
                    state.cursor = state.select_end;
                    state.has_preferred_x = 0;
                    break;

                ////# ifdef STB_TEXTEDIT_IS_SPACE
                case STB_TEXTEDIT_K_WORDLEFT:
                    if (STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_move_to_first(state);
                    else {
                        state.cursor = stb_textedit_move_to_word_previous(str, state);
                        stb_textedit_clamp(str, state);
                    }
                    break;

                case STB_TEXTEDIT_K_WORDRIGHT:
                    if (STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_move_to_last(str, state);
                    else {
                        state.cursor = stb_textedit_move_to_word_next(str, state);
                        stb_textedit_clamp(str, state);
                    }
                    break;

                case STB_TEXTEDIT_K_WORDLEFT | STB_TEXTEDIT_K_SHIFT:
                    if (!STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_prep_selection_at_cursor(state);

                    state.cursor = stb_textedit_move_to_word_previous(str, state);
                    state.select_end = state.cursor;

                    stb_textedit_clamp(str, state);
                    break;

                case STB_TEXTEDIT_K_WORDRIGHT | STB_TEXTEDIT_K_SHIFT:
                    if (!STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_prep_selection_at_cursor(state);

                    state.cursor = stb_textedit_move_to_word_next(str, state);
                    state.select_end = state.cursor;

                    stb_textedit_clamp(str, state);
                    break;
                ////#endif

                case STB_TEXTEDIT_K_RIGHT | STB_TEXTEDIT_K_SHIFT:
                    stb_textedit_prep_selection_at_cursor(state);
                    // move selection right
                    ++state.select_end;
                    stb_textedit_clamp(str, state);
                    state.cursor = state.select_end;
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_DOWN:
                case STB_TEXTEDIT_K_DOWN | STB_TEXTEDIT_K_SHIFT:
                    {
                        StbFindState find = new StbFindState();
                        StbTexteditRow row = new StbTexteditRow();
                        int i;
                        bool sel = (key & STB_TEXTEDIT_K_SHIFT) != 0;

                        if (state.single_line != 0)
                        {
                            // on windows, up&down in single-line behave like left&right
                            key = STB_TEXTEDIT_K_RIGHT | (key & STB_TEXTEDIT_K_SHIFT);
                            goto retry;
                        }

                        if (sel)
                            stb_textedit_prep_selection_at_cursor(state);
                        else if (STB_TEXT_HAS_SELECTION(state))
                            stb_textedit_move_to_last(str, state);

                        // compute current position of cursor point
                        stb_textedit_clamp(str, state);
                        stb_textedit_find_charpos(ref find, str, state.cursor, state.single_line);

                        // now find character position down a row
                        if (find.length != 0)
                        {
                            float goal_x = state.has_preferred_x != 0 ? state.preferred_x : find.x;
                            float x;
                            int start = find.first_char + find.length;
                            state.cursor = start;
                            STB_TEXTEDIT_LAYOUTROW(ref row, str, state.cursor);
                            x = row.x0;
                            for (i = 0; i < row.num_chars; ++i)
                            {
                                float dx = STB_TEXTEDIT_GETWIDTH(str, start, i);
                                //# ifdef STB_TEXTEDIT_GETWIDTH_NEWLINE
                                if (dx == STB_TEXTEDIT_GETWIDTH_NEWLINE)
                                    break;
                                //#endif
                                x += dx;
                                if (x > goal_x)
                                    break;
                                ++state.cursor;
                            }
                            stb_textedit_clamp(str, state);

                            state.has_preferred_x = 1;
                            state.preferred_x = goal_x;

                            if (sel)
                                state.select_end = state.cursor;
                        }
                        break;
                    }

                case STB_TEXTEDIT_K_UP:
                case STB_TEXTEDIT_K_UP | STB_TEXTEDIT_K_SHIFT:
                    {
                        StbFindState find = new StbFindState();
                        StbTexteditRow row = new StbTexteditRow();
                        int i;bool
                            sel = (key & STB_TEXTEDIT_K_SHIFT) != 0;

                        if (state.single_line != 0)
                        {
                            // on windows, up&down become left&right
                            key = STB_TEXTEDIT_K_LEFT | (key & STB_TEXTEDIT_K_SHIFT);
                            goto retry;
                        }

                        if (sel)
                            stb_textedit_prep_selection_at_cursor(state);
                        else if (STB_TEXT_HAS_SELECTION(state))
                            stb_textedit_move_to_first(state);

                        // compute current position of cursor point
                        stb_textedit_clamp(str, state);
                        stb_textedit_find_charpos(ref find, str, state.cursor, state.single_line);

                        // can only go up if there's a previous row
                        if (find.prev_first != find.first_char)
                        {
                            // now find character position up a row
                            float goal_x = state.has_preferred_x != 0 ? state.preferred_x : find.x;
                            float x;
                            state.cursor = find.prev_first;
                            STB_TEXTEDIT_LAYOUTROW(ref row, str, state.cursor);
                            x = row.x0;
                            for (i = 0; i < row.num_chars; ++i)
                            {
                                float dx = STB_TEXTEDIT_GETWIDTH(str, find.prev_first, i);
                                //# ifdef STB_TEXTEDIT_GETWIDTH_NEWLINE
                                if (dx == STB_TEXTEDIT_GETWIDTH_NEWLINE)
                                    break;
                                //#endif
                                x += dx;
                                if (x > goal_x)
                                    break;
                                ++state.cursor;
                            }
                            stb_textedit_clamp(str, state);

                            state.has_preferred_x = 1;
                            state.preferred_x = goal_x;

                            if (sel)
                                state.select_end = state.cursor;
                        }
                        break;
                    }

                case STB_TEXTEDIT_K_DELETE:
                case STB_TEXTEDIT_K_DELETE | STB_TEXTEDIT_K_SHIFT:
                    if (STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_delete_selection(str, state);
                    else {
                        int n = STB_TEXTEDIT_STRINGLEN(str);
                        if (state.cursor < n)
                            stb_textedit_delete(str, state, state.cursor, 1);
                    }
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_BACKSPACE:
                case STB_TEXTEDIT_K_BACKSPACE | STB_TEXTEDIT_K_SHIFT:
                    if (STB_TEXT_HAS_SELECTION(state))
                        stb_textedit_delete_selection(str, state);
                    else {
                        stb_textedit_clamp(str, state);
                        if (state.cursor > 0)
                        {
                            stb_textedit_delete(str, state, state.cursor - 1, 1);
                            --state.cursor;
                        }
                    }
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_TEXTSTART:
                    state.cursor = state.select_start = state.select_end = 0;
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_TEXTEND:
                    state.cursor = STB_TEXTEDIT_STRINGLEN(str);
                    state.select_start = state.select_end = 0;
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_TEXTSTART | STB_TEXTEDIT_K_SHIFT:
                    stb_textedit_prep_selection_at_cursor(state);
                    state.cursor = state.select_end = 0;
                    state.has_preferred_x = 0;
                    break;

                case STB_TEXTEDIT_K_TEXTEND | STB_TEXTEDIT_K_SHIFT:
                    stb_textedit_prep_selection_at_cursor(state);
                    state.cursor = state.select_end = STB_TEXTEDIT_STRINGLEN(str);
                    state.has_preferred_x = 0;
                    break;


                case STB_TEXTEDIT_K_LINESTART:
                    {
                        StbFindState find = new StbFindState();
                        stb_textedit_clamp(str, state);
                        stb_textedit_move_to_first(state);
                        stb_textedit_find_charpos(ref find, str, state.cursor, state.single_line);
                        state.cursor = find.first_char;
                        state.has_preferred_x = 0;
                        break;
                    }

                case STB_TEXTEDIT_K_LINEEND:
                    {
                        StbFindState find = new StbFindState();
                        stb_textedit_clamp(str, state);
                        stb_textedit_move_to_first(state);
                        stb_textedit_find_charpos(ref find, str, state.cursor, state.single_line);
                        state.cursor = find.first_char + find.length;
                        if (find.length > 0 && STB_TEXTEDIT_GETCHAR(str, state.cursor - 1) == STB_TEXTEDIT_NEWLINE)
                            state.cursor--;
                        state.has_preferred_x = 0;
                        break;
                    }

                case STB_TEXTEDIT_K_LINESTART | STB_TEXTEDIT_K_SHIFT:
                    {
                        StbFindState find = new StbFindState();
                        stb_textedit_clamp(str, state);
                        stb_textedit_prep_selection_at_cursor(state);
                        stb_textedit_find_charpos(ref find, str, state.cursor, state.single_line);
                        state.cursor = state.select_end = find.first_char;
                        state.has_preferred_x = 0;
                        break;
                    }

                case STB_TEXTEDIT_K_LINEEND | STB_TEXTEDIT_K_SHIFT:
                    {
                        StbFindState find = new StbFindState();
                        stb_textedit_clamp(str, state);
                        stb_textedit_prep_selection_at_cursor(state);
                        stb_textedit_find_charpos(ref find, str, state.cursor, state.single_line);
                        state.cursor = state.select_end = find.first_char + find.length;
                        if (find.length > 0 && STB_TEXTEDIT_GETCHAR(str, state.cursor - 1) == STB_TEXTEDIT_NEWLINE)
                            state.cursor = state.select_end = state.cursor - 1;
                        state.has_preferred_x = 0;
                        break;
                    }

                    // @TODO:
                    //    STB_TEXTEDIT_K_PGUP      - move cursor up a page
                    //    STB_TEXTEDIT_K_PGDOWN    - move cursor down a page
            }
        }

        //        /////////////////////////////////////////////////////////////////////////////
        //        //
        //        //      Undo processing
        //        //
        //        // @OPTIMIZE: the undo/redo buffer should be circular

        static void stb_textedit_flush_redo(StbUndoState state)
        {
            state.redo_point = STB_TEXTEDIT_UNDOSTATECOUNT;
            state.redo_char_point = STB_TEXTEDIT_UNDOCHARCOUNT;
        }

        // discard the oldest entry in the undo list
        static void stb_textedit_discard_undo(StbUndoState state)
        {
            if (state.undo_point > 0)
            {
                // if the 0th undo state has characters, clean those up
                if (state.undo_rec[0].char_storage >= 0)
                {
                    int n = state.undo_rec[0].insert_length, i;
                    // delete n characters from all other records
                    state.undo_char_point = (short)(state.undo_char_point - n);  // vsnet05
                    memmove(state.undo_char, state.undo_char, 0, n, state.undo_char_point);
                    for (i = 0; i < state.undo_point; ++i)
                        if (state.undo_rec[i].char_storage >= 0)
                            state.undo_rec[i].char_storage = (short)(state.undo_rec[i].char_storage - n); // vsnet05 // @OPTIMIZE: get rid of char_storage and infer it
                }
                --state.undo_point;
                memmove(state.undo_rec, state.undo_rec, 0, 1, state.undo_point);
            }
        }

        // discard the oldest entry in the redo list--it's bad if this
        // ever happens, but because undo & redo have to store the actual
        // characters in different cases, the redo character buffer can
        // fill up even though the undo buffer didn't
        static void stb_textedit_discard_redo(StbUndoState state)
        {
            int k = STB_TEXTEDIT_UNDOSTATECOUNT - 1;

            if (state.redo_point <= k)
            {
                // if the k'th undo state has characters, clean those up
                if (state.undo_rec[k].char_storage >= 0)
                {
                    int n = state.undo_rec[k].insert_length, i;
                    // delete n characters from all other records
                    state.redo_char_point = (short)(state.redo_char_point + n); // vsnet05

                    memmove(state.undo_char, state.undo_char, state.redo_char_point, state.redo_char_point - n, STB_TEXTEDIT_UNDOSTATECOUNT - state.redo_char_point);
                    for (i = state.redo_point; i < k; ++i)
                        if (state.undo_rec[i].char_storage >= 0)
                            state.undo_rec[i].char_storage = (short)(state.undo_rec[i].char_storage + n); // vsnet05
                }
                ++state.redo_point;
                memmove(state.undo_rec, state.undo_rec, state.redo_point - 1, state.redo_point, STB_TEXTEDIT_UNDOSTATECOUNT - state.redo_point);
            }
        }

        static int stb_text_create_undo_record(StbUndoState state, int numchars)
        {
            // any time we create a new undo record, we discard redo
            stb_textedit_flush_redo(state);

            // if we have no free records, we have to make room, by sliding the
            // existing records down
            if (state.undo_point == STB_TEXTEDIT_UNDOSTATECOUNT)
                stb_textedit_discard_undo(state);

            // if the characters to store won't possibly fit in the buffer, we can't undo
            if (numchars > STB_TEXTEDIT_UNDOCHARCOUNT)
            {
                state.undo_point = 0;
                state.undo_char_point = 0;
                return -1;
            }

            // if we don't have enough free characters in the buffer, we have to make room
            while (state.undo_char_point + numchars > STB_TEXTEDIT_UNDOCHARCOUNT)
                stb_textedit_discard_undo(state);

            //return state.undo_rec[state.undo_point++];
            return state.undo_point++;
        }

        static int stb_text_createundo(StbUndoState state, int pos, int insert_len, int delete_len)
        {
            //TODO was ptr
            int i = stb_text_create_undo_record(state, insert_len);
            if (i == -1)
                return -1;

            var r = state.undo_rec[i];

            r.where = pos;
            r.insert_length = (short)insert_len;
            r.delete_length = (short)delete_len;

            if (insert_len == 0)
            {
                r.char_storage = -1;
                state.undo_rec[i] = r;
                return -1;
            }
            else {
                r.char_storage = state.undo_char_point;
                state.undo_char_point = (short)(state.undo_char_point + insert_len);
                //return &state.undo_char[r.char_storage];
                state.undo_rec[i] = r;
                return r.char_storage;
            }
        }

        static void stb_text_undo(STB_TEXTEDIT_STRING str, STB_TexteditState state)
        {
            StbUndoState s = state.undostate;
            StbUndoRecord u, r;
            if (s.undo_point == 0)
                return;

            // we need to do two things: apply the undo record, and create a redo record
            u = s.undo_rec[s.undo_point - 1];
            r = s.undo_rec[s.redo_point - 1];
            r.char_storage = -1;

            r.insert_length = u.delete_length;
            r.delete_length = u.insert_length;
            r.where = u.where;

            if (u.delete_length != 0)
            {
                // if the undo record says to delete characters, then the redo record will
                // need to re-insert the characters that get deleted, so we need to store
                // them.

                // there are three cases:
                //    there's enough room to store the characters
                //    characters stored for *redoing* don't leave room for redo
                //    characters stored for *undoing* don't leave room for redo
                // if the last is true, we have to bail

                if (s.undo_char_point + u.delete_length >= STB_TEXTEDIT_UNDOCHARCOUNT)
                {
                    // the undo records take up too much character space; there's no space to store the redo characters
                    r.insert_length = 0;

                }
                else {
                    int i;

                    // there's definitely room to store the characters eventually
                    while (s.undo_char_point + u.delete_length > s.redo_char_point)
                    {
                        // there's currently not enough room, so discard a redo record
                        stb_textedit_discard_redo(s);
                        // should never happen:
                        if (s.redo_point == STB_TEXTEDIT_UNDOSTATECOUNT)
                            return;
                    }
                    r = s.undo_rec[s.redo_point - 1];

                    r.char_storage = (short)(s.redo_char_point - u.delete_length);
                    s.redo_char_point = (short)(s.redo_char_point - u.delete_length);

                    // now save the characters
                    for (i = 0; i < u.delete_length; ++i)
                        s.undo_char[r.char_storage + i] = STB_TEXTEDIT_GETCHAR(str, u.where + i);

                    //s.undo_rec[s.redo_point - 1] = r;
                }

                // now we can carry out the deletion
                STB_TEXTEDIT_DELETECHARS(str, u.where, u.delete_length);
            }

            // check type of recorded action:
            if (u.insert_length != 0)
            {
                // easy case: was a deletion, so we need to insert n characters
                STB_TEXTEDIT_INSERTCHARS(str, u.where, s.undo_char, u.insert_length, u.char_storage);
                s.undo_char_point -= u.insert_length;
            }

            state.cursor = u.where + u.insert_length;

            s.undo_point--;
            s.redo_point--;
        }

        static void stb_text_redo(STB_TEXTEDIT_STRING str, STB_TexteditState state)
        {
            StbUndoState s = state.undostate;
            StbUndoRecord u, r;
            if (s.redo_point == STB_TEXTEDIT_UNDOSTATECOUNT)
                return;

            // we need to do two things: apply the redo record, and create an undo record
            u = s.undo_rec[s.undo_point];
            r = s.undo_rec[s.redo_point];

            // we KNOW there must be room for the undo record, because the redo record
            // was derived from an undo record

            u.delete_length = r.insert_length;
            u.insert_length = r.delete_length;
            u.where = r.where;
            u.char_storage = -1;

            if (r.delete_length != 0)
            {
                // the redo record requires us to delete characters, so the undo record
                // needs to store the characters

                if (s.undo_char_point + u.insert_length > s.redo_char_point)
                {
                    u.insert_length = 0;
                    u.delete_length = 0;
                }
                else {
                    int i;
                    u.char_storage = s.undo_char_point;
                    s.undo_char_point = (short)(s.undo_char_point + u.insert_length);

                    // now save the characters
                    for (i = 0; i < u.insert_length; ++i)
                        s.undo_char[u.char_storage + i] = STB_TEXTEDIT_GETCHAR(str, u.where + i);
                }

                STB_TEXTEDIT_DELETECHARS(str, r.where, r.delete_length);
            }

            if (r.insert_length != 0)
            {
                // easy case: need to insert n characters
                STB_TEXTEDIT_INSERTCHARS(str, r.where, s.undo_char, r.insert_length, u.char_storage);
            }

            state.cursor = r.where + r.insert_length;

            s.undo_point++;
            s.redo_point++;
        }

        static void stb_text_makeundo_insert(STB_TexteditState state, int where, int length)
        {
            stb_text_createundo(state.undostate, where, 0, length);
        }

        static void stb_text_makeundo_delete(STB_TEXTEDIT_STRING str, STB_TexteditState state, int where, int length)
        {
            int i;
            int p = stb_text_createundo(state.undostate, where, length, 0);
            if (p > -1)
            {
                var undo = state.undostate.undo_rec[state.undostate.undo_point - 1];
                for (i = 0; i < length; ++i)
                    state.undostate.undo_char[i] = STB_TEXTEDIT_GETCHAR(str, where + i);
            }
        }

        static void stb_text_makeundo_replace(STB_TEXTEDIT_STRING str, STB_TexteditState state, int where, int old_length, int new_length)
        {
            int i;
            int p = stb_text_createundo(state.undostate, where, old_length, new_length);
            if (p > -1)
            {
                var undo = state.undostate.undo_rec[state.undostate.undo_point - 1];
                for (i = 0; i < old_length; ++i)
                    state.undostate.undo_char[i] = STB_TEXTEDIT_GETCHAR(str, where + i);
            }
        }

        // reset the state to default
        static void stb_textedit_clear_state(STB_TexteditState state, int is_single_line)
        {
            state.undostate.undo_point = 0;
            state.undostate.undo_char_point = 0;
            state.undostate.redo_point = STB_TEXTEDIT_UNDOSTATECOUNT;
            state.undostate.redo_char_point = STB_TEXTEDIT_UNDOCHARCOUNT;
            state.select_end = state.select_start = 0;
            state.cursor = 0;
            state.has_preferred_x = 0;
            state.preferred_x = 0;
            state.cursor_at_end_of_line = 0;
            state.initialized = 1;
            state.single_line = (byte)is_single_line;
            state.insert_mode = false;
        }

        // API initialize
        static void stb_textedit_initialize_state(STB_TexteditState state, int is_single_line)
        {
            stb_textedit_clear_state(state, is_single_line);
        }
    }
}
