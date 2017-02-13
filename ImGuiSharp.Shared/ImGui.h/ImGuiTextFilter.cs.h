// Helper: Parse and apply text filters. In format "aaaaa[,bbbb][,ccccc]"
struct ImGuiTextFilter
{
	struct TextRange
	{
		const char* b;
		const char* e;

		TextRange() { b = e = NULL; }
		TextRange(const char* _b, const char* _e) { b = _b; e = _e; }
		const char* begin() const { return b; }
		const char* end() const { return e; }
		bool empty() const { return b == e; }
		char front() const { return *b; }
		static bool isblank(char c) { return c == ' ' || c == '\t'; }
		void trim_blanks() { while (b < e && isblank(*b)) b++; while (e > b && isblank(*(e - 1))) e--; }
		IMGUI_API void split(char separator, ImVector<TextRange>& out);
	};

	char                InputBuf[256];
	ImVector<TextRange> Filters;
	int                 CountGrep;

	ImGuiTextFilter(const char* default_filter = "");
	void                Clear() { InputBuf[0] = 0; Build(); }
	bool                Draw(const char* label = "Filter (inc,-exc)", float width = 0.0f);    // Helper calling InputText+Build
	bool                PassFilter(const char* text, const char* text_end = NULL) const;
	bool                IsActive() const { return !Filters.empty(); }
	IMGUI_API void      Build();
};