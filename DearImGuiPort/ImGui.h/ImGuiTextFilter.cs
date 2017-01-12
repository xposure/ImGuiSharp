//namespace ImGui
//{
//    // Parse and apply text filters. In format "aaaaa[,bbbb][,ccccc]"
//    internal class ImGuiTextFilter
//    {
//        struct TextRange
//	    {
//		    int b;
//		    int e;

//		    //TextRange() { b = e = -1; }
//		    TextRange(int _b, int _e) { b = _b; e = _e; }
//		    int begin()  { return b; }
//		    int end()  { return e; }
//		    bool empty()  { return b == e; }
//		    char front()  { return *b; }
//		    static bool isblank(char c) { return c == ' ' || c == '\t'; }
//		    void trim_blanks() { while (b < e && isblank(*b)) b++; while (e > b && isblank(*(e - 1))) e--; }
//		    void split(char separator, ImVector<TextRange> @out);
//	    };

//	    char                InputBuf[256];
//	    ImVector<TextRange> Filters;
//	    int                 CountGrep;

//	    ImGuiTextFilter(int default_filter = "");
//	    void                Clear() { InputBuf[0] = 0; Build(); }
//	    bool                Draw(const char* label = "Filter (inc,-exc)", float width = 0.0f);    // Helper calling InputText+Build
//	    bool                PassFilter(const char* text, const char* text_end = NULL) const;
//	    bool                IsActive() const { return !Filters.empty(); }
//	    void      Build();
//    }
//}