namespace ImGui
{
    using System;

    [Flags]
    internal enum ImGuiButtonFlags : int
    {
        ImGuiButtonFlags_Repeat = 1 << 0,   // hold to repeat
        ImGuiButtonFlags_PressedOnClick = 1 << 1,   // return pressed on click (default requires click+release)
        ImGuiButtonFlags_PressedOnRelease = 1 << 2,   // return pressed on release (default requires click+release)
        ImGuiButtonFlags_PressedOnDoubleClick = 1 << 3,   // return pressed on double-click (default requires click+release)
        ImGuiButtonFlags_FlattenChilds = 1 << 4,   // allow interaction even if a child window is overlapping
        ImGuiButtonFlags_DontClosePopups = 1 << 5,   // disable automatically closing parent popup on press
        ImGuiButtonFlags_Disabled = 1 << 6,   // disable interaction
        ImGuiButtonFlags_AlignTextBaseLine = 1 << 7,   // vertically align button to match text baseline - ButtonEx() only
        ImGuiButtonFlags_NoKeyModifiers = 1 << 8    // disable interaction if a key modifier is held
    }
}
