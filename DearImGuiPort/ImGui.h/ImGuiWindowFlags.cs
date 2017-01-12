namespace ImGui
{
    using System;

    // window flags for Begin*() 

    // Flags for ImGui::Begin()
    [Flags]
    public enum ImGuiWindowFlags : int
    {
        // Default: 0
        ImGuiWindowFlags_NoTitleBar = 1 << 0,   // Disable title-bar
        ImGuiWindowFlags_NoResize = 1 << 1,   // Disable user resizing with the lower-right grip
        ImGuiWindowFlags_NoMove = 1 << 2,   // Disable user moving the window
        ImGuiWindowFlags_NoScrollbar = 1 << 3,   // Disable scrollbars (window can still scroll with mouse or programatically)
        ImGuiWindowFlags_NoScrollWithMouse = 1 << 4,   // Disable user vertically scrolling with mouse wheel
        ImGuiWindowFlags_NoCollapse = 1 << 5,   // Disable user collapsing window by double-clicking on it
        ImGuiWindowFlags_AlwaysAutoResize = 1 << 6,   // Resize every window to its content every frame
        ImGuiWindowFlags_ShowBorders = 1 << 7,   // Show borders around windows and items
        ImGuiWindowFlags_NoSavedSettings = 1 << 8,   // Never load/save settings in .ini file
        ImGuiWindowFlags_NoInputs = 1 << 9,   // Disable catching mouse or keyboard inputs
        ImGuiWindowFlags_MenuBar = 1 << 10,  // Has a menu-bar
        ImGuiWindowFlags_HorizontalScrollbar = 1 << 11,  // Allow horizontal scrollbar to appear (off by default). You need to use SetNextWindowContentSize(ImVec2(width,0.0f)); prior to calling Begin() to specify width. Read code in imgui_demo in the "Horizontal Scrolling" section.
        ImGuiWindowFlags_NoFocusOnAppearing = 1 << 12,  // Disable taking focus when transitioning from hidden to visible state
        ImGuiWindowFlags_NoBringToFrontOnFocus = 1 << 13,  // Disable bringing window to front when taking focus (e.g. clicking on it or programatically giving it focus)
        ImGuiWindowFlags_AlwaysVerticalScrollbar = 1 << 14,  // Always show vertical scrollbar (even if ContentSize.y < Size.y)
        ImGuiWindowFlags_ForceHorizontalScrollbar = 1 << 15,  // Always show horizontal scrollbar (even if ContentSize.x < Size.x)
                                                              // [Internal]
        ImGuiWindowFlags_ChildWindow = 1 << 20,  // Don't use! For internal use by BeginChild()
        ImGuiWindowFlags_ChildWindowAutoFitX = 1 << 21,  // Don't use! For internal use by BeginChild()
        ImGuiWindowFlags_ChildWindowAutoFitY = 1 << 22,  // Don't use! For internal use by BeginChild()
        ImGuiWindowFlags_ComboBox = 1 << 23,  // Don't use! For internal use by ComboBox()
        ImGuiWindowFlags_Tooltip = 1 << 24,  // Don't use! For internal use by BeginTooltip()
        ImGuiWindowFlags_Popup = 1 << 25,  // Don't use! For internal use by BeginPopup()
        ImGuiWindowFlags_Modal = 1 << 26,  // Don't use! For internal use by BeginPopupModal()
        ImGuiWindowFlags_ChildMenu = 1 << 27   // Don't use! For internal use by BeginMenu()
    };

}
