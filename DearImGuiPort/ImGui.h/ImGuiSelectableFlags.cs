namespace ImGui
{
    using System;
    // flags for Selectable()

    // Flags for ImGui::Selectable()
    [Flags]
    public enum ImGuiSelectableFlags : int
    {
        // Default: 0
        ImGuiSelectableFlags_DontClosePopups = 1 << 0,   // Clicking this don't close parent popup window
        ImGuiSelectableFlags_SpanAllColumns = 1 << 1,   // Selectable frame can span all columns (text will still fit in current column)
        ImGuiSelectableFlags_AllowDoubleClick = 1 << 2,    // Generate press events on double clicks too

        ImGuiSelectableFlags_Menu = 1 << 3,
        ImGuiSelectableFlags_MenuItem = 1 << 4,
        ImGuiSelectableFlags_Disabled = 1 << 5,
        ImGuiSelectableFlags_DrawFillAvailWidth = 1 << 6
    };
}
