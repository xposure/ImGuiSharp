// Helper: Text buffer for logging/accumulating text
struct ImGuiTextBuffer
{
	ImVector<char>      Buf;

	ImGuiTextBuffer() { Buf.push_back(0); }
	inline char         operator[](int i) { return Buf.Data[i]; }
	const char*         begin() const { return &Buf.front(); }
	const char*         end() const { return &Buf.back(); }      // Buf is zero-terminated, so end() will point on the zero-terminator
	int                 size() const { return Buf.Size - 1; }
	bool                empty() { return Buf.Size <= 1; }
	void                clear() { Buf.clear(); Buf.push_back(0); }
	const char*         c_str() const { return Buf.Data; }
	IMGUI_API void      append(const char* fmt, ...) IM_PRINTFARGS(2);
	IMGUI_API void      appendv(const char* fmt, va_list args);
};