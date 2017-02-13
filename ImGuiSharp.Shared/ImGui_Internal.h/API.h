//-----------------------------------------------------------------------------
// Internal API
// No guarantee of forward compatibility here.
//-----------------------------------------------------------------------------

namespace ImGui
{
	// We should always have a CurrentWindow in the stack (there is an implicit "Debug" window)
	// If this ever crash because g.CurrentWindow is NULL it means that either
	// - ImGui::NewFrame() has never been called, which is illegal.
	// - You are calling ImGui functions after ImGui::Render() and before the next ImGui::NewFrame(), which is also illegal.
	inline    ImGuiWindow*  GetCurrentWindowRead() { ImGuiState& g = *GImGui; return g.CurrentWindow; }
	inline    ImGuiWindow*  GetCurrentWindow() { ImGuiState& g = *GImGui; g.CurrentWindow->Accessed = true; return g.CurrentWindow; }
	IMGUI_API ImGuiWindow*  GetParentWindow();
	IMGUI_API ImGuiWindow*  FindWindowByName(const char* name);
	IMGUI_API void          FocusWindow(ImGuiWindow* window);

	IMGUI_API void          SetActiveID(uint id, ImGuiWindow* window);
	IMGUI_API void          SetHoveredID(uint id);
	IMGUI_API void          KeepAliveID(uint id);

	IMGUI_API void          EndFrame();                 // Automatically called by Render()

	IMGUI_API void          ItemSize(const ImVec2& size, float text_offset_y = 0.0f);
	IMGUI_API void          ItemSize(const ImRect& bb, float text_offset_y = 0.0f);
	IMGUI_API bool          ItemAdd(const ImRect& bb, const uint* id);
	IMGUI_API bool          IsClippedEx(const ImRect& bb, const uint* id, bool clip_even_when_logged);
	IMGUI_API bool          IsHovered(const ImRect& bb, uint id, bool flatten_childs = false);
	IMGUI_API bool          FocusableItemRegister(ImGuiWindow* window, bool is_active, bool tab_stop = true);      // Return true if focus is requested
	IMGUI_API void          FocusableItemUnregister(ImGuiWindow* window);
	IMGUI_API ImVec2        CalcItemSize(ImVec2 size, float default_x, float default_y);
	IMGUI_API float         CalcWrapWidthForPos(const ImVec2& pos, float wrap_pos_x);

	IMGUI_API void          OpenPopupEx(const char* str_id, bool reopen_existing);

	inline IMGUI_API uint  GetColorU32(ImGuiCol idx, float alpha_mul) { ImVec4 c = GImGui->Style.Colors[idx]; c.w *= GImGui->Style.Alpha * alpha_mul; return ImGui::ColorConvertFloat4ToU32(c); }
	inline IMGUI_API uint  GetColorU32(const ImVec4& col) { ImVec4 c = col; c.w *= GImGui->Style.Alpha; return ImGui::ColorConvertFloat4ToU32(c); }

	// NB: All position are in absolute pixels coordinates (not window coordinates)
	// FIXME: Refactor all RenderText* functions into one.
	IMGUI_API void          RenderText(ImVec2 pos, const char* text, const char* text_end = NULL, bool hide_text_after_hash = true);
	IMGUI_API void          RenderTextWrapped(ImVec2 pos, const char* text, const char* text_end, float wrap_width);
	IMGUI_API void          RenderTextClipped(const ImVec2& pos_min, const ImVec2& pos_max, const char* text, const char* text_end, const ImVec2* text_size_if_known, ImGuiAlign align = ImGuiAlign_Default, const ImVec2* clip_min = NULL, const ImVec2* clip_max = NULL);
	IMGUI_API void          RenderFrame(ImVec2 p_min, ImVec2 p_max, uint fill_col, bool border = true, float rounding = 0.0f);
	IMGUI_API void          RenderCollapseTriangle(ImVec2 p_min, bool opened, float scale = 1.0f, bool shadow = false);
	IMGUI_API void          RenderCheckMark(ImVec2 pos, uint col);
	IMGUI_API const char*   FindRenderedTextEnd(const char* text, const char* text_end = NULL); // Find the optional ## from which we stop displaying text.

	IMGUI_API void          PushClipRect(const ImVec2& clip_rect_min, const ImVec2& clip_rect_max, bool intersect_with_existing_clip_rect = true);
	IMGUI_API void          PopClipRect();

	IMGUI_API bool          ButtonBehavior(const ImRect& bb, uint id, bool* out_hovered, bool* out_held, ImGuiButtonFlags flags = 0);
	IMGUI_API bool          ButtonEx(const char* label, const ImVec2& size_arg = ImVec2(0, 0), ImGuiButtonFlags flags = 0);

	IMGUI_API bool          SliderBehavior(const ImRect& frame_bb, uint id, float* v, float v_min, float v_max, float power, int decimal_precision, ImGuiSliderFlags flags = 0);
	IMGUI_API bool          SliderFloatN(const char* label, float* v, int components, float v_min, float v_max, const char* display_format, float power);
	IMGUI_API bool          SliderIntN(const char* label, int* v, int components, int v_min, int v_max, const char* display_format);

	IMGUI_API bool          DragBehavior(const ImRect& frame_bb, uint id, float* v, float v_speed, float v_min, float v_max, int decimal_precision, float power);
	IMGUI_API bool          DragFloatN(const char* label, float* v, int components, float v_speed, float v_min, float v_max, const char* display_format, float power);
	IMGUI_API bool          DragIntN(const char* label, int* v, int components, float v_speed, int v_min, int v_max, const char* display_format);

	IMGUI_API bool          InputTextEx(const char* label, char* buf, int buf_size, const ImVec2& size_arg, ImGuiInputTextFlags flags, ImGuiTextEditCallback callback = NULL, void* user_data = NULL);
	IMGUI_API bool          InputFloatN(const char* label, float* v, int components, int decimal_precision, ImGuiInputTextFlags extra_flags);
	IMGUI_API bool          InputIntN(const char* label, int* v, int components, ImGuiInputTextFlags extra_flags);
	IMGUI_API bool          InputScalarEx(const char* label, ImGuiDataType data_type, void* data_ptr, void* step_ptr, void* step_fast_ptr, const char* scalar_format, ImGuiInputTextFlags extra_flags);
	IMGUI_API bool          InputScalarAsWidgetReplacement(const ImRect& aabb, const char* label, ImGuiDataType data_type, void* data_ptr, uint id, int decimal_precision);

	IMGUI_API bool          TreeNodeBehaviorIsOpened(uint id, ImGuiTreeNodeFlags flags = 0);                     // Consume previous SetNextTreeNodeOpened() data, if any. May return true when logging

	IMGUI_API void          PlotEx(ImGuiPlotType plot_type, const char* label, float(*values_getter)(void* data, int idx), void* data, int values_count, int values_offset, const char* overlay_text, float scale_min, float scale_max, ImVec2 graph_size);

	IMGUI_API int           ParseFormatPrecision(const char* fmt, int default_value);
	IMGUI_API float         RoundScalar(float value, int decimal_precision);

} // namespace ImGuiP