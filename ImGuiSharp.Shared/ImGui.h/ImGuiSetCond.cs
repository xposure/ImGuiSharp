namespace ImGui
{
    using System;
    // condition flags for Set*()

    // Condition flags for ImGui::SetWindow***(), SetNextWindow***(), SetNextTreeNode***() functions
    // All those functions treat 0 as a shortcut to ImGuiSetCond_Always
    [Flags]
    public enum ImGuiSetCond : int
    {
        ImGuiSetCond_Always = 1 << 0, // Set the variable
        ImGuiSetCond_Once = 1 << 1, // Only set the variable on the first call per runtime session
        ImGuiSetCond_FirstUseEver = 1 << 2, // Only set the variable if the window doesn't exist in the .ini file
        ImGuiSetCond_Appearing = 1 << 3  // Only set the variable if the window is appearing after being inactive (or the first time)
    };

}
