namespace ImGui
{
using System;

    [Flags]
    internal enum ImGuiTreeNodeFlags : int
    {
        ImGuiTreeNodeFlags_DefaultOpen = 1 << 0,
        ImGuiTreeNodeFlags_NoAutoExpandOnLog = 1 << 1
    }
}
