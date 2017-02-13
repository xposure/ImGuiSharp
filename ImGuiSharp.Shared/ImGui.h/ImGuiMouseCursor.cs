namespace ImGui
{
    // a mouse cursor identifier

    // Enumeration for GetMouseCursor()
    public enum ImGuiMouseCursor : int
    {
        ImGuiMouseCursor_Arrow = 0,
        ImGuiMouseCursor_TextInput,         // When hovering over InputText, etc.
        ImGuiMouseCursor_Move,              // Unused
        ImGuiMouseCursor_ResizeNS,          // Unused
        ImGuiMouseCursor_ResizeEW,          // When hovering over a column
        ImGuiMouseCursor_ResizeNESW,        // Unused
        ImGuiMouseCursor_ResizeNWSE,        // When hovering over the bottom-right corner of a window
        ImGuiMouseCursor_Count_
    };

}
