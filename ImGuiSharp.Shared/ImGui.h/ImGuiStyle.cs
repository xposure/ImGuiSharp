namespace ImGui
{
    internal class ImGuiStyle
    {
        internal float Alpha;                         // Global alpha applies to everything in ImGui
        internal ImVec2 WindowPadding;              // Padding within a window
        internal ImVec2 WindowMinSize;              // Minimum window size
        internal float WindowRounding;             // Radius of window corners rounding. Set to 0.0f to have rectangular windows
        internal ImGuiAlign WindowTitleAlign;           // Alignment for title bar text
        internal float ChildWindowRounding;        // Radius of child window corners rounding. Set to 0.0f to have rectangular windows
        internal ImVec2 FramePadding;               // Padding within a framed rectangle (used by most widgets)
        internal float FrameRounding;              // Radius of frame corners rounding. Set to 0.0f to have rectangular frame (used by most widgets).
        internal ImVec2 ItemSpacing;                // Horizontal and vertical spacing between widgets/lines
        internal ImVec2 ItemInnerSpacing;           // Horizontal and vertical spacing between within elements of a composed widget (e.g. a slider and its label)
        internal ImVec2 TouchExtraPadding;          // Expand reactive bounding box for touch-based system where touch position is not accurate enough. Unfortunately we don't sort widgets so priority on overlap will always be given to the first widget. So don't grow this too much!
        internal float WindowFillAlphaDefault;     // Default alpha of window background, if not specified in ImGui::Begin()
        internal float IndentSpacing;              // Horizontal indentation when e.g. entering a tree node
        internal float ColumnsMinSpacing;          // Minimum horizontal spacing between two columns
        internal float ScrollbarSize;              // Width of the vertical scrollbar, Height of the horizontal scrollbar
        internal float ScrollbarRounding;          // Radius of grab corners for scrollbar
        internal float GrabMinSize;                // Minimum width/height of a grab box for slider/scrollbar
        internal float GrabRounding;               // Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
        internal ImVec2 DisplayWindowPadding;       // Window positions are clamped to be visible within the display area by at least this amount. Only covers regular windows.
        internal ImVec2 DisplaySafeAreaPadding;     // If you cannot see the edge of your screen (e.g. on a TV) increase the safe area padding. Covers popups/tooltips as well regular windows.
        internal bool AntiAliasedLines;           // Enable anti-aliasing on lines/borders. Disable if you are really tight on CPU/GPU.
        internal bool AntiAliasedShapes;          // Enable anti-aliasing on filled shapes (rounded rectangles, circles, etc.)
        internal float CurveTessellationTol;       // Tessellation tolerance. Decrease for highly tessellated curves (higher quality, more polygons), increase to reduce quality.
        internal ImVec4[] Colors = new ImVec4[(int)ImGuiCol.ImGuiCol_COUNT];

        internal ImGuiStyle()
        {
            Alpha = 1.0f;             // Global alpha applies to everything in ImGui
            WindowPadding = new ImVec2(8, 8);      // Padding within a window
            WindowMinSize = new ImVec2(32, 32);    // Minimum window size
            WindowRounding = 9.0f;             // Radius of window corners rounding. Set to 0.0f to have rectangular windows
            WindowTitleAlign = ImGuiAlign.ImGuiAlign_Left;  // Alignment for title bar text
            ChildWindowRounding = 0.0f;             // Radius of child window corners rounding. Set to 0.0f to have rectangular windows
            FramePadding = new ImVec2(4, 3);      // Padding within a framed rectangle (used by most widgets)
            FrameRounding = 0.0f;             // Radius of frame corners rounding. Set to 0.0f to have rectangular frames (used by most widgets).
            ItemSpacing = new ImVec2(8, 4);      // Horizontal and vertical spacing between widgets/lines
            ItemInnerSpacing = new ImVec2(4, 4);      // Horizontal and vertical spacing between within elements of a composed widget (e.g. a slider and its label)
            TouchExtraPadding = new ImVec2(0, 0);      // Expand reactive bounding box for touch-based system where touch position is not accurate enough. Unfortunately we don't sort widgets so priority on overlap will always be given to the first widget. So don't grow this too much!
            WindowFillAlphaDefault = 0.70f;            // Default alpha of window background, if not specified in ImGui::Begin()
            IndentSpacing = 22.0f;            // Horizontal spacing when e.g. entering a tree node
            ColumnsMinSpacing = 6.0f;             // Minimum horizontal spacing between two columns
            ScrollbarSize = 16.0f;            // Width of the vertical scrollbar, Height of the horizontal scrollbar
            ScrollbarRounding = 9.0f;             // Radius of grab corners rounding for scrollbar
            GrabMinSize = 10.0f;            // Minimum width/height of a grab box for slider/scrollbar
            GrabRounding = 0.0f;             // Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
            DisplayWindowPadding = new ImVec2(22, 22);    // Window positions are clamped to be visible within the display area by at least this amount. Only covers regular windows.
            DisplaySafeAreaPadding = new ImVec2(4, 4);      // If you cannot see the edge of your screen (e.g. on a TV) increase the safe area padding. Covers popups/tooltips as well regular windows.
            AntiAliasedLines = true;             // Enable anti-aliasing on lines/borders. Disable if you are really short on CPU/GPU.
            AntiAliasedShapes = true;             // Enable anti-aliasing on filled shapes (rounded rectangles, circles, etc.)
            CurveTessellationTol = 1.25f;            // Tessellation tolerance. Decrease for highly tessellated curves (higher quality, more polygons), increase to reduce quality.

            Colors[(int)ImGuiCol.ImGuiCol_Text] = new ImVec4(0.90f, 0.90f, 0.90f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_TextDisabled] = new ImVec4(0.60f, 0.60f, 0.60f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_WindowBg] = new ImVec4(0.00f, 0.00f, 0.00f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_ChildWindowBg] = new ImVec4(0.00f, 0.00f, 0.00f, 0.00f);
            Colors[(int)ImGuiCol.ImGuiCol_Border] = new ImVec4(0.70f, 0.70f, 0.70f, 0.65f);
            Colors[(int)ImGuiCol.ImGuiCol_BorderShadow] = new ImVec4(0.00f, 0.00f, 0.00f, 0.00f);
            Colors[(int)ImGuiCol.ImGuiCol_FrameBg] = new ImVec4(0.80f, 0.80f, 0.80f, 0.30f);   // Background of checkbox, radio button, plot, slider, text input
            Colors[(int)ImGuiCol.ImGuiCol_FrameBgHovered] = new ImVec4(0.90f, 0.80f, 0.80f, 0.40f);
            Colors[(int)ImGuiCol.ImGuiCol_FrameBgActive] = new ImVec4(0.90f, 0.65f, 0.65f, 0.45f);
            Colors[(int)ImGuiCol.ImGuiCol_TitleBg] = new ImVec4(0.50f, 0.50f, 1.00f, 0.45f);
            Colors[(int)ImGuiCol.ImGuiCol_TitleBgCollapsed] = new ImVec4(0.40f, 0.40f, 0.80f, 0.20f);
            Colors[(int)ImGuiCol.ImGuiCol_TitleBgActive] = new ImVec4(0.50f, 0.50f, 1.00f, 0.55f);
            Colors[(int)ImGuiCol.ImGuiCol_MenuBarBg] = new ImVec4(0.40f, 0.40f, 0.55f, 0.80f);
            Colors[(int)ImGuiCol.ImGuiCol_ScrollbarBg] = new ImVec4(0.20f, 0.25f, 0.30f, 0.60f);
            Colors[(int)ImGuiCol.ImGuiCol_ScrollbarGrab] = new ImVec4(0.40f, 0.40f, 0.80f, 0.30f);
            Colors[(int)ImGuiCol.ImGuiCol_ScrollbarGrabHovered] = new ImVec4(0.40f, 0.40f, 0.80f, 0.40f);
            Colors[(int)ImGuiCol.ImGuiCol_ScrollbarGrabActive] = new ImVec4(0.80f, 0.50f, 0.50f, 0.40f);
            Colors[(int)ImGuiCol.ImGuiCol_ComboBg] = new ImVec4(0.20f, 0.20f, 0.20f, 0.99f);
            Colors[(int)ImGuiCol.ImGuiCol_CheckMark] = new ImVec4(0.90f, 0.90f, 0.90f, 0.50f);
            Colors[(int)ImGuiCol.ImGuiCol_SliderGrab] = new ImVec4(1.00f, 1.00f, 1.00f, 0.30f);
            Colors[(int)ImGuiCol.ImGuiCol_SliderGrabActive] = new ImVec4(0.80f, 0.50f, 0.50f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_Button] = new ImVec4(0.67f, 0.40f, 0.40f, 0.60f);
            Colors[(int)ImGuiCol.ImGuiCol_ButtonHovered] = new ImVec4(0.67f, 0.40f, 0.40f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_ButtonActive] = new ImVec4(0.80f, 0.50f, 0.50f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_Header] = new ImVec4(0.40f, 0.40f, 0.90f, 0.45f);
            Colors[(int)ImGuiCol.ImGuiCol_HeaderHovered] = new ImVec4(0.45f, 0.45f, 0.90f, 0.80f);
            Colors[(int)ImGuiCol.ImGuiCol_HeaderActive] = new ImVec4(0.53f, 0.53f, 0.87f, 0.80f);
            Colors[(int)ImGuiCol.ImGuiCol_Column] = new ImVec4(0.50f, 0.50f, 0.50f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_ColumnHovered] = new ImVec4(0.70f, 0.60f, 0.60f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_ColumnActive] = new ImVec4(0.90f, 0.70f, 0.70f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_ResizeGrip] = new ImVec4(1.00f, 1.00f, 1.00f, 0.30f);
            Colors[(int)ImGuiCol.ImGuiCol_ResizeGripHovered] = new ImVec4(1.00f, 1.00f, 1.00f, 0.60f);
            Colors[(int)ImGuiCol.ImGuiCol_ResizeGripActive] = new ImVec4(1.00f, 1.00f, 1.00f, 0.90f);
            Colors[(int)ImGuiCol.ImGuiCol_CloseButton] = new ImVec4(0.50f, 0.50f, 0.90f, 0.50f);
            Colors[(int)ImGuiCol.ImGuiCol_CloseButtonHovered] = new ImVec4(0.70f, 0.70f, 0.90f, 0.60f);
            Colors[(int)ImGuiCol.ImGuiCol_CloseButtonActive] = new ImVec4(0.70f, 0.70f, 0.70f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_PlotLines] = new ImVec4(1.00f, 1.00f, 1.00f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_PlotLinesHovered] = new ImVec4(0.90f, 0.70f, 0.00f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_PlotHistogram] = new ImVec4(0.90f, 0.70f, 0.00f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_PlotHistogramHovered] = new ImVec4(1.00f, 0.60f, 0.00f, 1.00f);
            Colors[(int)ImGuiCol.ImGuiCol_TextSelectedBg] = new ImVec4(0.00f, 0.00f, 1.00f, 0.35f);
            Colors[(int)ImGuiCol.ImGuiCol_TooltipBg] = new ImVec4(0.05f, 0.05f, 0.10f, 0.90f);
            Colors[(int)ImGuiCol.ImGuiCol_ModalWindowDarkening] = new ImVec4(0.20f, 0.20f, 0.20f, 0.35f);
        }
    };
}