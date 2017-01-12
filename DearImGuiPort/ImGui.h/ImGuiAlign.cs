namespace ImGui
{
    using System;

    // alignment
    [Flags]
    public enum ImGuiAlign : int
    {
        ImGuiAlign_Left = 1 << 0,
        ImGuiAlign_Center = 1 << 1,
        ImGuiAlign_Right = 1 << 2,
        ImGuiAlign_Top = 1 << 3,
        ImGuiAlign_VCenter = 1 << 4,
        ImGuiAlign_Default = ImGuiAlign_Left | ImGuiAlign_Top
    }
}
