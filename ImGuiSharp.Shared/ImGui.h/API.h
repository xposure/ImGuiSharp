// ImGui end-user API
// In a namespace so that user can add extra functions in a separate file (e.g. Value() helpers for your vector or common types)
namespace ImGui
{
	// Main
	IMGUI_API ImGuiIO&      GetIO();
	IMGUI_API ImGuiStyle&   GetStyle();
	IMGUI_API ImDrawData*   GetDrawData();                              // same value as passed to your io.RenderDrawListsFn() function. valid after Render() and until the next call to NewFrame().
	IMGUI_API void          NewFrame();
	IMGUI_API void          Render();                                   // finalize rendering data, then call your io.RenderDrawListsFn() function if set. 
	IMGUI_API void          Shutdown();
	IMGUI_API void          ShowUserGuide();                            // help block
	IMGUI_API void          ShowStyleEditor(ImGuiStyle* ref = NULL);    // style editor block
	IMGUI_API void          ShowTestWindow(bool* opened = NULL);        // test window, demonstrate ImGui features
	IMGUI_API void          ShowMetricsWindow(bool* opened = NULL);     // metrics window for debugging imgui

																		// Window
	IMGUI_API bool          Begin(const char* name, bool* p_opened = NULL, ImGuiWindowFlags flags = 0);                                                   // see .cpp for details. return false when window is collapsed, so you can early out in your code. 'bool* p_opened' creates a widget on the upper-right to close the window (which sets your bool to false).
	IMGUI_API bool          Begin(const char* name, bool* p_opened, const ImVec2& size_on_first_use, float bg_alpha = -1.0f, ImGuiWindowFlags flags = 0); // this is the older/longer API. the extra parameters aren't very relevant. call SetNextWindowSize() instead if you want to set a window size. For regular windows, 'size_on_first_use' only applies to the first time EVER the window is created and probably not what you want! maybe obsolete this API eventually.
	IMGUI_API void          End();
	IMGUI_API bool          BeginChild(const char* str_id, const ImVec2& size = ImVec2(0, 0), bool border = false, ImGuiWindowFlags extra_flags = 0);  // begin a scrolling region. size==0.0f: use remaining window size, size<0.0f: use remaining window size minus abs(size). size>0.0f: fixed size. each axis can use a different mode, e.g. ImVec2(0,400).
	IMGUI_API bool          BeginChild(uint id, const ImVec2& size = ImVec2(0, 0), bool border = false, ImGuiWindowFlags extra_flags = 0);          // "
	IMGUI_API void          EndChild();
	IMGUI_API ImVec2        GetContentRegionMax();                                              // current content boundaries (typically window boundaries including scrolling, or current column boundaries), in windows coordinates
	IMGUI_API ImVec2        GetContentRegionAvail();                                            // == GetContentRegionMax() - GetCursorPos()
	IMGUI_API float         GetContentRegionAvailWidth();                                       //
	IMGUI_API ImVec2        GetWindowContentRegionMin();                                        // content boundaries min (roughly (0,0)-Scroll), in window coordinates
	IMGUI_API ImVec2        GetWindowContentRegionMax();                                        // content boundaries max (roughly (0,0)+Size-Scroll) where Size can be override with SetNextWindowContentSize(), in window coordinates
	IMGUI_API float         GetWindowContentRegionWidth();                                      // 
	IMGUI_API ImDrawList*   GetWindowDrawList();                                                // get rendering command-list if you want to append your own draw primitives
	IMGUI_API ImVec2        GetWindowPos();                                                     // get current window position in screen space (useful if you want to do your own drawing via the DrawList api)
	IMGUI_API ImVec2        GetWindowSize();                                                    // get current window size
	IMGUI_API float         GetWindowWidth();
	IMGUI_API float         GetWindowHeight();
	IMGUI_API bool          IsWindowCollapsed();
	IMGUI_API void          SetWindowFontScale(float scale);                                    // per-window font scale. Adjust IO.FontGlobalScale if you want to scale all windows

	IMGUI_API void          SetNextWindowPos(const ImVec2& pos, ImGuiSetCond cond = 0);         // set next window position. call before Begin()
	IMGUI_API void          SetNextWindowPosCenter(ImGuiSetCond cond = 0);                      // set next window position to be centered on screen. call before Begin()
	IMGUI_API void          SetNextWindowSize(const ImVec2& size, ImGuiSetCond cond = 0);       // set next window size. set axis to 0.0f to force an auto-fit on this axis. call before Begin()
	IMGUI_API void          SetNextWindowContentSize(const ImVec2& size);                       // set next window content size (enforce the range of scrollbars). set axis to 0.0f to leave it automatic. call before Begin()
	IMGUI_API void          SetNextWindowContentWidth(float width);                             // set next window content width (enforce the range of horizontal scrollbar). call before Begin() 
	IMGUI_API void          SetNextWindowCollapsed(bool collapsed, ImGuiSetCond cond = 0);      // set next window collapsed state. call before Begin()
	IMGUI_API void          SetNextWindowFocus();                                               // set next window to be focused / front-most. call before Begin()
	IMGUI_API void          SetWindowPos(const ImVec2& pos, ImGuiSetCond cond = 0);             // set current window position - call within Begin()/End(). may incur tearing
	IMGUI_API void          SetWindowSize(const ImVec2& size, ImGuiSetCond cond = 0);           // set current window size. set to ImVec2(0,0) to force an auto-fit. may incur tearing
	IMGUI_API void          SetWindowCollapsed(bool collapsed, ImGuiSetCond cond = 0);          // set current window collapsed state
	IMGUI_API void          SetWindowFocus();                                                   // set current window to be focused / front-most
	IMGUI_API void          SetWindowPos(const char* name, const ImVec2& pos, ImGuiSetCond cond = 0);      // set named window position.
	IMGUI_API void          SetWindowSize(const char* name, const ImVec2& size, ImGuiSetCond cond = 0);    // set named window size. set axis to 0.0f to force an auto-fit on this axis.
	IMGUI_API void          SetWindowCollapsed(const char* name, bool collapsed, ImGuiSetCond cond = 0);   // set named window collapsed state
	IMGUI_API void          SetWindowFocus(const char* name);                                              // set named window to be focused / front-most. use NULL to remove focus.

	IMGUI_API float         GetScrollX();                                                       // get scrolling amount [0..GetScrollMaxX()]
	IMGUI_API float         GetScrollY();                                                       // get scrolling amount [0..GetScrollMaxY()]
	IMGUI_API float         GetScrollMaxX();                                                    // get maximum scrolling amount ~~ ContentSize.X - WindowSize.X
	IMGUI_API float         GetScrollMaxY();                                                    // get maximum scrolling amount ~~ ContentSize.Y - WindowSize.Y
	IMGUI_API void          SetScrollX(float scroll_x);                                         // set scrolling amount [0..GetScrollMaxX()]
	IMGUI_API void          SetScrollY(float scroll_y);                                         // set scrolling amount [0..GetScrollMaxY()]
	IMGUI_API void          SetScrollHere(float center_y_ratio = 0.5f);                         // adjust scrolling amount to make current cursor position visible. center_y_ratio=0.0: top, 0.5: center, 1.0: bottom.
	IMGUI_API void          SetScrollFromPosY(float pos_y, float center_y_ratio = 0.5f);        // adjust scrolling amount to make given position valid. use GetCursorPos() or GetCursorStartPos()+offset to get valid positions.
	IMGUI_API void          SetKeyboardFocusHere(int offset = 0);                               // focus keyboard on the next widget. Use positive 'offset' to access sub components of a multiple component widget
	IMGUI_API void          SetStateStorage(ImGuiStorage* tree);                                // replace tree state storage with our own (if you want to manipulate it yourself, typically clear subsection of it)
	IMGUI_API ImGuiStorage* GetStateStorage();

	// Parameters stacks (shared)
	IMGUI_API void          PushFont(ImFont* font);                                             // use NULL as a shortcut to push default font
	IMGUI_API void          PopFont();
	IMGUI_API void          PushStyleColor(ImGuiCol idx, const ImVec4& col);
	IMGUI_API void          PopStyleColor(int count = 1);
	IMGUI_API void          PushStyleVar(ImGuiStyleVar idx, float val);
	IMGUI_API void          PushStyleVar(ImGuiStyleVar idx, const ImVec2& val);
	IMGUI_API void          PopStyleVar(int count = 1);
	IMGUI_API ImFont*       GetFont();                                                          // get current font
	IMGUI_API float         GetFontSize();                                                      // get current font size (= height in pixels) of current font with current scale applied
	IMGUI_API ImVec2        GetFontTexUvWhitePixel();                                           // get UV coordinate for a while pixel, useful to draw custom shapes via the ImDrawList API
	IMGUI_API uint         GetColorU32(ImGuiCol idx, float alpha_mul = 1.0f);                  // retrieve given style color with style alpha applied and optional extra alpha multiplier
	IMGUI_API uint         GetColorU32(const ImVec4& col);                                     // retrieve given color with style alpha applied

																								// Parameters stacks (current window)
	IMGUI_API void          PushItemWidth(float item_width);                                    // width of items for the common item+label case, pixels. 0.0f = default to ~2/3 of windows width, >0.0f: width in pixels, <0.0f align xx pixels to the right of window (so -1.0f always align width to the right side)
	IMGUI_API void          PopItemWidth();
	IMGUI_API float         CalcItemWidth();                                                    // width of item given pushed settings and current cursor position
	IMGUI_API void          PushTextWrapPos(float wrap_pos_x = 0.0f);                           // word-wrapping for Text*() commands. < 0.0f: no wrapping; 0.0f: wrap to end of window (or column); > 0.0f: wrap at 'wrap_pos_x' position in window local space
	IMGUI_API void          PopTextWrapPos();
	IMGUI_API void          PushAllowKeyboardFocus(bool v);                                     // allow focusing using TAB/Shift-TAB, enabled by default but you can disable it for certain widgets
	IMGUI_API void          PopAllowKeyboardFocus();
	IMGUI_API void          PushButtonRepeat(bool repeat);                                      // in 'repeat' mode, Button*() functions return repeated true in a typematic manner (uses io.KeyRepeatDelay/io.KeyRepeatRate for now). Note that you can call IsItemActive() after any Button() to tell if the button is held in the current frame.
	IMGUI_API void          PopButtonRepeat();

	// Cursor / Layout
	IMGUI_API void          BeginGroup();                                                       // lock horizontal starting position. once closing a group it is seen as a single item (so you can use IsItemHovered() on a group, SameLine() between groups, etc.
	IMGUI_API void          EndGroup();
	IMGUI_API void          Separator();                                                        // horizontal line
	IMGUI_API void          SameLine(float pos_x = 0.0f, float spacing_w = -1.0f);              // call between widgets or groups to layout them horizontally
	IMGUI_API void          Spacing();                                                          // add spacing
	IMGUI_API void          Dummy(const ImVec2& size);                                          // add a dummy item of given size
	IMGUI_API void          Indent();                                                           // move content position toward the right by style.IndentSpacing pixels
	IMGUI_API void          Unindent();                                                         // move content position back to the left (cancel Indent)
	IMGUI_API ImVec2        GetCursorPos();                                                     // cursor position is relative to window position
	IMGUI_API float         GetCursorPosX();                                                    // "
	IMGUI_API float         GetCursorPosY();                                                    // "
	IMGUI_API void          SetCursorPos(const ImVec2& local_pos);                              // "
	IMGUI_API void          SetCursorPosX(float x);                                             // "
	IMGUI_API void          SetCursorPosY(float y);                                             // "
	IMGUI_API ImVec2        GetCursorStartPos();                                                // initial cursor position
	IMGUI_API ImVec2        GetCursorScreenPos();                                               // cursor position in absolute screen coordinates [0..io.DisplaySize]
	IMGUI_API void          SetCursorScreenPos(const ImVec2& pos);                              // cursor position in absolute screen coordinates [0..io.DisplaySize]
	IMGUI_API void          AlignFirstTextHeightToWidgets();                                    // call once if the first item on the line is a Text() item and you want to vertically lower it to match subsequent (bigger) widgets
	IMGUI_API float         GetTextLineHeight();                                                // height of font == GetWindowFontSize()
	IMGUI_API float         GetTextLineHeightWithSpacing();                                     // distance (in pixels) between 2 consecutive lines of text == GetWindowFontSize() + GetStyle().ItemSpacing.y
	IMGUI_API float         GetItemsLineHeightWithSpacing();                                    // distance (in pixels) between 2 consecutive lines of standard height widgets == GetWindowFontSize() + GetStyle().FramePadding.y*2 + GetStyle().ItemSpacing.y

																								// Columns
																								// You can also use SameLine(pos_x) for simplified columning. The columns API is still work-in-progress.
	IMGUI_API void          Columns(int count = 1, const char* id = NULL, bool border = true);  // setup number of columns. use an identifier to distinguish multiple column sets. close with Columns(1).
	IMGUI_API void          NextColumn();                                                       // next column
	IMGUI_API int           GetColumnIndex();                                                   // get current column index
	IMGUI_API float         GetColumnOffset(int column_index = -1);                             // get position of column line (in pixels, from the left side of the contents region). pass -1 to use current column, otherwise 0..GetcolumnsCount() inclusive. column 0 is usually 0.0f and not resizable unless you call this
	IMGUI_API void          SetColumnOffset(int column_index, float offset_x);                  // set position of column line (in pixels, from the left side of the contents region). pass -1 to use current column
	IMGUI_API float         GetColumnWidth(int column_index = -1);                              // column width (== GetColumnOffset(GetColumnIndex()+1) - GetColumnOffset(GetColumnOffset())
	IMGUI_API int           GetColumnsCount();                                                  // number of columns (what was passed to Columns())

																								// ID scopes
																								// If you are creating widgets in a loop you most likely want to push a unique identifier so ImGui can differentiate them.
																								// You can also use the "##foobar" syntax within widget label to distinguish them from each others. Read "A primer on the use of labels/IDs" in the FAQ for more details.
	IMGUI_API void          PushID(const char* str_id);                                         // push identifier into the ID stack. IDs are hash of the *entire* stack!
	IMGUI_API void          PushID(const char* str_id_begin, const char* str_id_end);
	IMGUI_API void          PushID(const void* ptr_id);
	IMGUI_API void          PushID(int int_id);
	IMGUI_API void          PopID();
	IMGUI_API uint       GetID(const char* str_id);                                          // calculate unique ID (hash of whole ID stack + given parameter). useful if you want to query into ImGuiStorage yourself. otherwise rarely needed
	IMGUI_API uint       GetID(const char* str_id_begin, const char* str_id_end);
	IMGUI_API uint       GetID(const void* ptr_id);

	// Widgets
	IMGUI_API void          Text(const char* fmt, ...) IM_PRINTFARGS(1);
	IMGUI_API void          TextV(const char* fmt, va_list args);
	IMGUI_API void          TextColored(const ImVec4& col, const char* fmt, ...) IM_PRINTFARGS(2);  // shortcut for PushStyleColor(ImGuiCol_Text, col); Text(fmt, ...); PopStyleColor();
	IMGUI_API void          TextColoredV(const ImVec4& col, const char* fmt, va_list args);
	IMGUI_API void          TextDisabled(const char* fmt, ...) IM_PRINTFARGS(1);                    // shortcut for PushStyleColor(ImGuiCol_Text, style.Colors[ImGuiCol_TextDisabled]); Text(fmt, ...); PopStyleColor();
	IMGUI_API void          TextDisabledV(const char* fmt, va_list args);
	IMGUI_API void          TextWrapped(const char* fmt, ...) IM_PRINTFARGS(1);                     // shortcut for PushTextWrapPos(0.0f); Text(fmt, ...); PopTextWrapPos();. Note that this won't work on an auto-resizing window if there's no other widgets to extend the window width, yoy may need to set a size using SetNextWindowSize().
	IMGUI_API void          TextWrappedV(const char* fmt, va_list args);
	IMGUI_API void          TextUnformatted(const char* text, const char* text_end = NULL);         // doesn't require null terminated string if 'text_end' is specified. no copy done to any bounded stack buffer, recommended for long chunks of text
	IMGUI_API void          LabelText(const char* label, const char* fmt, ...) IM_PRINTFARGS(2);    // display text+label aligned the same way as value+label widgets
	IMGUI_API void          LabelTextV(const char* label, const char* fmt, va_list args);
	IMGUI_API void          Bullet();                                                               // draw a small circle and keep the cursor on the same line. advance you by the same distance as an empty TreeNode() call.
	IMGUI_API void          BulletText(const char* fmt, ...) IM_PRINTFARGS(1);
	IMGUI_API void          BulletTextV(const char* fmt, va_list args);
	IMGUI_API bool          Button(const char* label, const ImVec2& size = ImVec2(0, 0));
	IMGUI_API bool          SmallButton(const char* label);
	IMGUI_API bool          InvisibleButton(const char* str_id, const ImVec2& size);
	IMGUI_API void          Image(ImTextureID user_texture_id, const ImVec2& size, const ImVec2& uv0 = ImVec2(0, 0), const ImVec2& uv1 = ImVec2(1, 1), const ImVec4& tint_col = ImVec4(1, 1, 1, 1), const ImVec4& border_col = ImVec4(0, 0, 0, 0));
	IMGUI_API bool          ImageButton(ImTextureID user_texture_id, const ImVec2& size, const ImVec2& uv0 = ImVec2(0, 0), const ImVec2& uv1 = ImVec2(1, 1), int frame_padding = -1, const ImVec4& bg_col = ImVec4(0, 0, 0, 0), const ImVec4& tint_col = ImVec4(1, 1, 1, 1));    // <0 frame_padding uses default frame padding settings. 0 for no padding
	IMGUI_API bool          CollapsingHeader(const char* label, const char* str_id = NULL, bool display_frame = true, bool default_open = false);
	IMGUI_API bool          Checkbox(const char* label, bool* v);
	IMGUI_API bool          CheckboxFlags(const char* label, unsigned int* flags, unsigned int flags_value);
	IMGUI_API bool          RadioButton(const char* label, bool active);
	IMGUI_API bool          RadioButton(const char* label, int* v, int v_button);
	IMGUI_API bool          Combo(const char* label, int* current_item, const char** items, int items_count, int height_in_items = -1);
	IMGUI_API bool          Combo(const char* label, int* current_item, const char* items_separated_by_zeros, int height_in_items = -1);      // separate items with \0, end item-list with \0\0
	IMGUI_API bool          Combo(const char* label, int* current_item, bool(*items_getter)(void* data, int idx, const char** out_text), void* data, int items_count, int height_in_items = -1);
	IMGUI_API bool          ColorButton(const ImVec4& col, bool small_height = false, bool outline_border = true);
	IMGUI_API bool          ColorEdit3(const char* label, float col[3]);
	IMGUI_API bool          ColorEdit4(const char* label, float col[4], bool show_alpha = true);
	IMGUI_API void          ColorEditMode(ImGuiColorEditMode mode);                                 // FIXME-OBSOLETE: This is inconsistent with most of the API and should be obsoleted.
	IMGUI_API void          PlotLines(const char* label, const float* values, int values_count, int values_offset = 0, const char* overlay_text = NULL, float scale_min = FLT_MAX, float scale_max = FLT_MAX, ImVec2 graph_size = ImVec2(0, 0), int stride = sizeof(float));
	IMGUI_API void          PlotLines(const char* label, float(*values_getter)(void* data, int idx), void* data, int values_count, int values_offset = 0, const char* overlay_text = NULL, float scale_min = FLT_MAX, float scale_max = FLT_MAX, ImVec2 graph_size = ImVec2(0, 0));
	IMGUI_API void          PlotHistogram(const char* label, const float* values, int values_count, int values_offset = 0, const char* overlay_text = NULL, float scale_min = FLT_MAX, float scale_max = FLT_MAX, ImVec2 graph_size = ImVec2(0, 0), int stride = sizeof(float));
	IMGUI_API void          PlotHistogram(const char* label, float(*values_getter)(void* data, int idx), void* data, int values_count, int values_offset = 0, const char* overlay_text = NULL, float scale_min = FLT_MAX, float scale_max = FLT_MAX, ImVec2 graph_size = ImVec2(0, 0));
	IMGUI_API void          ProgressBar(float fraction, const ImVec2& size_arg = ImVec2(-1, 0), const char* overlay = NULL);

	// Widgets: Drags (tip: ctrl+click on a drag box to input with keyboard. manually input values aren't clamped, can go off-bounds)
	IMGUI_API bool          DragFloat(const char* label, float* v, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f, const char* display_format = "%.3f", float power = 1.0f);     // If v_min >= v_max we have no bound
	IMGUI_API bool          DragFloat2(const char* label, float v[2], float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          DragFloat3(const char* label, float v[3], float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          DragFloat4(const char* label, float v[4], float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          DragFloatRange2(const char* label, float* v_current_min, float* v_current_max, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f, const char* display_format = "%.3f", const char* display_format_max = NULL, float power = 1.0f);
	IMGUI_API bool          DragInt(const char* label, int* v, float v_speed = 1.0f, int v_min = 0, int v_max = 0, const char* display_format = "%.0f");                                       // If v_min >= v_max we have no bound
	IMGUI_API bool          DragInt2(const char* label, int v[2], float v_speed = 1.0f, int v_min = 0, int v_max = 0, const char* display_format = "%.0f");
	IMGUI_API bool          DragInt3(const char* label, int v[3], float v_speed = 1.0f, int v_min = 0, int v_max = 0, const char* display_format = "%.0f");
	IMGUI_API bool          DragInt4(const char* label, int v[4], float v_speed = 1.0f, int v_min = 0, int v_max = 0, const char* display_format = "%.0f");
	IMGUI_API bool          DragIntRange2(const char* label, int* v_current_min, int* v_current_max, float v_speed = 1.0f, int v_min = 0, int v_max = 0, const char* display_format = "%.0f", const char* display_format_max = NULL);

	// Widgets: Input with Keyboard
	IMGUI_API bool          InputText(const char* label, char* buf, size_t buf_size, ImGuiInputTextFlags flags = 0, ImGuiTextEditCallback callback = NULL, void* user_data = NULL);
	IMGUI_API bool          InputTextMultiline(const char* label, char* buf, size_t buf_size, const ImVec2& size = ImVec2(0, 0), ImGuiInputTextFlags flags = 0, ImGuiTextEditCallback callback = NULL, void* user_data = NULL);
	IMGUI_API bool          InputFloat(const char* label, float* v, float step = 0.0f, float step_fast = 0.0f, int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputFloat2(const char* label, float v[2], int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputFloat3(const char* label, float v[3], int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputFloat4(const char* label, float v[4], int decimal_precision = -1, ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputInt(const char* label, int* v, int step = 1, int step_fast = 100, ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputInt2(const char* label, int v[2], ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputInt3(const char* label, int v[3], ImGuiInputTextFlags extra_flags = 0);
	IMGUI_API bool          InputInt4(const char* label, int v[4], ImGuiInputTextFlags extra_flags = 0);

	// Widgets: Sliders (tip: ctrl+click on a slider to input with keyboard. manually input values aren't clamped, can go off-bounds)
	IMGUI_API bool          SliderFloat(const char* label, float* v, float v_min, float v_max, const char* display_format = "%.3f", float power = 1.0f);     // adjust display_format to decorate the value with a prefix or a suffix. Use power!=1.0 for logarithmic sliders
	IMGUI_API bool          SliderFloat2(const char* label, float v[2], float v_min, float v_max, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          SliderFloat3(const char* label, float v[3], float v_min, float v_max, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          SliderFloat4(const char* label, float v[4], float v_min, float v_max, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          SliderAngle(const char* label, float* v_rad, float v_degrees_min = -360.0f, float v_degrees_max = +360.0f);
	IMGUI_API bool          SliderInt(const char* label, int* v, int v_min, int v_max, const char* display_format = "%.0f");
	IMGUI_API bool          SliderInt2(const char* label, int v[2], int v_min, int v_max, const char* display_format = "%.0f");
	IMGUI_API bool          SliderInt3(const char* label, int v[3], int v_min, int v_max, const char* display_format = "%.0f");
	IMGUI_API bool          SliderInt4(const char* label, int v[4], int v_min, int v_max, const char* display_format = "%.0f");
	IMGUI_API bool          VSliderFloat(const char* label, const ImVec2& size, float* v, float v_min, float v_max, const char* display_format = "%.3f", float power = 1.0f);
	IMGUI_API bool          VSliderInt(const char* label, const ImVec2& size, int* v, int v_min, int v_max, const char* display_format = "%.0f");

	// Widgets: Trees
	IMGUI_API bool          TreeNode(const char* str_label_id);                                     // if returning 'true' the node is open and the user is responsible for calling TreePop().
	IMGUI_API bool          TreeNode(const char* str_id, const char* fmt, ...) IM_PRINTFARGS(2);    // read the FAQ about why and how to use ID. to align arbitrary text at the same level as a TreeNode() you can use Bullet().
	IMGUI_API bool          TreeNode(const void* ptr_id, const char* fmt, ...) IM_PRINTFARGS(2);    // "
	IMGUI_API bool          TreeNodeV(const char* str_id, const char* fmt, va_list args);           // "
	IMGUI_API bool          TreeNodeV(const void* ptr_id, const char* fmt, va_list args);           // "
	IMGUI_API void          TreePush(const char* str_id = NULL);                                    // already called by TreeNode(), but you can call Push/Pop yourself for layouting purpose
	IMGUI_API void          TreePush(const void* ptr_id = NULL);                                    // "
	IMGUI_API void          TreePop();
	IMGUI_API void          SetNextTreeNodeOpened(bool opened, ImGuiSetCond cond = 0);              // set next tree node to be opened.

																									// Widgets: Selectable / Lists
	IMGUI_API bool          Selectable(const char* label, bool selected = false, ImGuiSelectableFlags flags = 0, const ImVec2& size = ImVec2(0, 0));  // size.x==0.0: use remaining width, size.x>0.0: specify width. size.y==0.0: use label height, size.y>0.0: specify height
	IMGUI_API bool          Selectable(const char* label, bool* p_selected, ImGuiSelectableFlags flags = 0, const ImVec2& size = ImVec2(0, 0));
	IMGUI_API bool          ListBox(const char* label, int* current_item, const char** items, int items_count, int height_in_items = -1);
	IMGUI_API bool          ListBox(const char* label, int* current_item, bool(*items_getter)(void* data, int idx, const char** out_text), void* data, int items_count, int height_in_items = -1);
	IMGUI_API bool          ListBoxHeader(const char* label, const ImVec2& size = ImVec2(0, 0)); // use if you want to reimplement ListBox() will custom data or interactions. make sure to call ListBoxFooter() afterwards.
	IMGUI_API bool          ListBoxHeader(const char* label, int items_count, int height_in_items = -1); // "
	IMGUI_API void          ListBoxFooter();                                                    // terminate the scrolling region

																								// Widgets: Value() Helpers. Output single value in "name: value" format (tip: freely declare more in your code to handle your types. you can add functions to the ImGui namespace)
	IMGUI_API void          Value(const char* prefix, bool b);
	IMGUI_API void          Value(const char* prefix, int v);
	IMGUI_API void          Value(const char* prefix, unsigned int v);
	IMGUI_API void          Value(const char* prefix, float v, const char* float_format = NULL);
	IMGUI_API void          ValueColor(const char* prefix, const ImVec4& v);
	IMGUI_API void          ValueColor(const char* prefix, unsigned int v);

	// Tooltips
	IMGUI_API void          SetTooltip(const char* fmt, ...) IM_PRINTFARGS(1);                  // set tooltip under mouse-cursor, typically use with ImGui::IsHovered(). last call wins
	IMGUI_API void          SetTooltipV(const char* fmt, va_list args);
	IMGUI_API void          BeginTooltip();                                                     // use to create full-featured tooltip windows that aren't just text
	IMGUI_API void          EndTooltip();

	// Menus
	IMGUI_API bool          BeginMainMenuBar();                                                 // create and append to a full screen menu-bar. only call EndMainMenuBar() if this returns true!
	IMGUI_API void          EndMainMenuBar();
	IMGUI_API bool          BeginMenuBar();                                                     // append to menu-bar of current window (requires ImGuiWindowFlags_MenuBar flag set). only call EndMenuBar() if this returns true!
	IMGUI_API void          EndMenuBar();
	IMGUI_API bool          BeginMenu(const char* label, bool enabled = true);                  // create a sub-menu entry. only call EndMenu() if this returns true!
	IMGUI_API void          EndMenu();
	IMGUI_API bool          MenuItem(const char* label, const char* shortcut = NULL, bool selected = false, bool enabled = true);  // return true when activated. shortcuts are displayed for convenience but not processed by ImGui at the moment
	IMGUI_API bool          MenuItem(const char* label, const char* shortcut, bool* p_selected, bool enabled = true);              // return true when activated + toggle (*p_selected) if p_selected != NULL

																																   // Popups
	IMGUI_API void          OpenPopup(const char* str_id);                                      // mark popup as open. popups are closed when user click outside, or activate a pressable item, or CloseCurrentPopup() is called within a BeginPopup()/EndPopup() block. popup identifiers are relative to the current ID-stack (so OpenPopup and BeginPopup needs to be at the same level). 
	IMGUI_API bool          BeginPopup(const char* str_id);                                     // return true if popup if opened and start outputting to it. only call EndPopup() if BeginPopup() returned true!
	IMGUI_API bool          BeginPopupModal(const char* name, bool* p_opened = NULL, ImGuiWindowFlags extra_flags = 0);             // modal dialog (can't close them by clicking outside)
	IMGUI_API bool          BeginPopupContextItem(const char* str_id, int mouse_button = 1);                                        // helper to open and begin popup when clicked on last item. read comments in .cpp!
	IMGUI_API bool          BeginPopupContextWindow(bool also_over_items = true, const char* str_id = NULL, int mouse_button = 1);  // helper to open and begin popup when clicked on current window.
	IMGUI_API bool          BeginPopupContextVoid(const char* str_id = NULL, int mouse_button = 1);                                 // helper to open and begin popup when clicked in void (no window).
	IMGUI_API void          EndPopup();
	IMGUI_API void          CloseCurrentPopup();                                                // close the popup we have begin-ed into. clicking on a MenuItem or Selectable automatically close the current popup.

																								// Logging: all text output from interface is redirected to tty/file/clipboard. Tree nodes are automatically opened.
	IMGUI_API void          LogToTTY(int max_depth = -1);                                       // start logging to tty
	IMGUI_API void          LogToFile(int max_depth = -1, const char* filename = NULL);         // start logging to file
	IMGUI_API void          LogToClipboard(int max_depth = -1);                                 // start logging to OS clipboard
	IMGUI_API void          LogFinish();                                                        // stop logging (close file, etc.)
	IMGUI_API void          LogButtons();                                                       // helper to display buttons for logging to tty/file/clipboard
	IMGUI_API void          LogText(const char* fmt, ...) IM_PRINTFARGS(1);                     // pass text data straight to log (without being displayed)

																								// Utilities
	IMGUI_API bool          IsItemHovered();                                                    // was the last item hovered by mouse?
	IMGUI_API bool          IsItemHoveredRect();                                                // was the last item hovered by mouse? even if another item is active or window is blocked by popup while we are hovering this
	IMGUI_API bool          IsItemActive();                                                     // was the last item active? (e.g. button being held, text field being edited- items that don't interact will always return false)
	IMGUI_API bool          IsItemVisible();                                                    // was the last item visible? (aka not out of sight due to clipping/scrolling.)
	IMGUI_API bool          IsAnyItemHovered();
	IMGUI_API bool          IsAnyItemActive();
	IMGUI_API ImVec2        GetItemRectMin();                                                   // get bounding rect of last item in screen space
	IMGUI_API ImVec2        GetItemRectMax();                                                   // "
	IMGUI_API ImVec2        GetItemRectSize();                                                  // "
	IMGUI_API void          SetItemAllowOverlap();                                              // allow last item to be overlapped by a subsequent item. sometimes useful with invisible buttons, selectables, etc. to catch unused area.
	IMGUI_API bool          IsWindowHovered();                                                  // is current window hovered and hoverable (not blocked by a popup) (differentiate child windows from each others)
	IMGUI_API bool          IsWindowFocused();                                                  // is current window focused
	IMGUI_API bool          IsRootWindowFocused();                                              // is current root window focused (top parent window in case of child windows)
	IMGUI_API bool          IsRootWindowOrAnyChildFocused();                                    // is current root window or any of its child (including current window) focused
	IMGUI_API bool          IsRectVisible(const ImVec2& size);                                  // test if rectangle of given size starting from cursor pos is visible (not clipped). to perform coarse clipping on user's side (as an optimization)
	IMGUI_API bool          IsPosHoveringAnyWindow(const ImVec2& pos);                          // is given position hovering any active imgui window
	IMGUI_API float         GetTime();
	IMGUI_API int           GetFrameCount();
	IMGUI_API const char*   GetStyleColName(ImGuiCol idx);
	IMGUI_API ImVec2        CalcItemRectClosestPoint(const ImVec2& pos, bool on_edge = false, float outward = +0.0f);   // utility to find the closest point the last item bounding rectangle edge. useful to visually link items
	IMGUI_API ImVec2        CalcTextSize(const char* text, const char* text_end = NULL, bool hide_text_after_double_hash = false, float wrap_width = -1.0f);
	IMGUI_API void          CalcListClipping(int items_count, float items_height, int* out_items_display_start, int* out_items_display_end);    // calculate coarse clipping for large list of evenly sized items. Prefer using the ImGuiListClipper higher-level helper if you can.

	IMGUI_API bool          BeginChildFrame(uint id, const ImVec2& size, ImGuiWindowFlags extra_flags = 0);	// helper to create a child window / scrolling region that looks like a normal widget frame
	IMGUI_API void          EndChildFrame();

	IMGUI_API ImVec4        ColorConvertU32ToFloat4(uint in);
	IMGUI_API uint         ColorConvertFloat4ToU32(const ImVec4& in);
	IMGUI_API void          ColorConvertRGBtoHSV(float r, float g, float b, float& out_h, float& out_s, float& out_v);
	IMGUI_API void          ColorConvertHSVtoRGB(float h, float s, float v, float& out_r, float& out_g, float& out_b);

	// Inputs
	IMGUI_API int           GetKeyIndex(ImGuiKey key);                                          // map ImGuiKey_* values into user's key index. == io.KeyMap[key]
	IMGUI_API bool          IsKeyDown(int key_index);                                           // key_index into the keys_down[] array, imgui doesn't know the semantic of each entry, uses your own indices!
	IMGUI_API bool          IsKeyPressed(int key_index, bool repeat = true);                    // uses user's key indices as stored in the keys_down[] array. if repeat=true. uses io.KeyRepeatDelay / KeyRepeatRate
	IMGUI_API bool          IsKeyReleased(int key_index);                                       // "
	IMGUI_API bool          IsMouseDown(int button);                                            // is mouse button held
	IMGUI_API bool          IsMouseClicked(int button, bool repeat = false);                    // did mouse button clicked (went from !Down to Down)
	IMGUI_API bool          IsMouseDoubleClicked(int button);                                   // did mouse button double-clicked. a double-click returns false in IsMouseClicked(). uses io.MouseDoubleClickTime.
	IMGUI_API bool          IsMouseReleased(int button);                                        // did mouse button released (went from Down to !Down)
	IMGUI_API bool          IsMouseHoveringWindow();                                            // is mouse hovering current window ("window" in API names always refer to current window). disregarding of any consideration of being blocked by a popup. (unlike IsWindowHovered() this will return true even if the window is blocked because of a popup)
	IMGUI_API bool          IsMouseHoveringAnyWindow();                                         // is mouse hovering any visible window
	IMGUI_API bool          IsMouseHoveringRect(const ImVec2& r_min, const ImVec2& r_max, bool clip = true);  // is mouse hovering given bounding rect (in screen space). clipped by current clipping settings. disregarding of consideration of focus/window ordering/blocked by a popup.
	IMGUI_API bool          IsMouseDragging(int button = 0, float lock_threshold = -1.0f);      // is mouse dragging. if lock_threshold < -1.0f uses io.MouseDraggingThreshold
	IMGUI_API ImVec2        GetMousePos();                                                      // shortcut to ImGui::GetIO().MousePos provided by user, to be consistent with other calls
	IMGUI_API ImVec2        GetMousePosOnOpeningCurrentPopup();                                 // retrieve backup of mouse positioning at the time of opening popup we have BeginPopup() into
	IMGUI_API ImVec2        GetMouseDragDelta(int button = 0, float lock_threshold = -1.0f);    // dragging amount since clicking. if lock_threshold < -1.0f uses io.MouseDraggingThreshold
	IMGUI_API void          ResetMouseDragDelta(int button = 0);                                //
	IMGUI_API ImGuiMouseCursor GetMouseCursor();                                                // get desired cursor type, reset in ImGui::NewFrame(), this updated during the frame. valid before Render(). If you use software rendering by setting io.MouseDrawCursor ImGui will render those for you
	IMGUI_API void          SetMouseCursor(ImGuiMouseCursor type);                              // set desired cursor type
	IMGUI_API void          CaptureKeyboardFromApp(bool capture = true);                        // manually override io.WantCaptureKeyboard flag next frame (said flag is entirely left for your application handle). e.g. force capture keyboard when your widget is being hovered.
	IMGUI_API void          CaptureMouseFromApp(bool capture = true);                           // manually override io.WantCaptureMouse flag next frame (said flag is entirely left for your application handle).

																								// Helpers functions to access functions pointers in ImGui::GetIO()
	IMGUI_API void*         MemAlloc(size_t sz);
	IMGUI_API void          MemFree(void* ptr);
	IMGUI_API const char*   GetClipboardText();
	IMGUI_API void          SetClipboardText(const char* text);

	// Internal state/context access - if you want to use multiple ImGui context, or share context between modules (e.g. DLL), or allocate the memory yourself
	IMGUI_API const char*   GetVersion();
	IMGUI_API void*         GetInternalState();
	IMGUI_API size_t        GetInternalStateSize();
	IMGUI_API void          SetInternalState(void* state, bool construct = false);

	// Obsolete (will be removed)
#ifndef IMGUI_DISABLE_OBSOLETE_FUNCTIONS
	static inline ImFont*   GetWindowFont() { return GetFont(); }                              // OBSOLETE 1.48+
	static inline float     GetWindowFontSize() { return GetFontSize(); }                      // OBSOLETE 1.48+
	static inline void      OpenNextNode(bool open) { ImGui::SetNextTreeNodeOpened(open, 0); } // OBSOLETE 1.34+
	static inline bool      GetWindowIsFocused() { return ImGui::IsWindowFocused(); }          // OBSOLETE 1.36+
	static inline bool      GetWindowCollapsed() { return ImGui::IsWindowCollapsed(); }        // OBSOLETE 1.39+
	static inline ImVec2    GetItemBoxMin() { return GetItemRectMin(); }                       // OBSOLETE 1.36+
	static inline ImVec2    GetItemBoxMax() { return GetItemRectMax(); }                       // OBSOLETE 1.36+
	static inline bool      IsClipped(const ImVec2& size) { return !IsRectVisible(size); }     // OBSOLETE 1.38+
	static inline bool      IsRectClipped(const ImVec2& size) { return !IsRectVisible(size); } // OBSOLETE 1.39+
	static inline bool      IsMouseHoveringBox(const ImVec2& rect_min, const ImVec2& rect_max) { return IsMouseHoveringRect(rect_min, rect_max); }  // OBSOLETE 1.36+
	static inline void      SetScrollPosHere() { SetScrollHere(); }                            // OBSOLETE 1.42+
#endif

} // namespace ImGui