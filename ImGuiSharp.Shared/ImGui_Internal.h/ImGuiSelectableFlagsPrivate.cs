namespace ImGui
{
    using System;

    [Flags]
    internal enum ImGuiSelectableFlagsPrivate_ : int
    {
        // NB: need to be in sync with last value of ImGuiSelectableFlags_
        ImGuiSelectableFlags_Menu = 1 << 3,
        ImGuiSelectableFlags_MenuItem = 1 << 4,
        ImGuiSelectableFlags_Disabled = 1 << 5,
        ImGuiSelectableFlags_DrawFillAvailWidth = 1 << 6
    }
}
