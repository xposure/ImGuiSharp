namespace ImGui
{
    // color edit mode for ColorEdit*() 

    // Enumeration for ColorEditMode()
    public enum ImGuiColorEditMode : int
    {
        ImGuiColorEditMode_UserSelect = -2,
        ImGuiColorEditMode_UserSelectShowButton = -1,
        ImGuiColorEditMode_RGB = 0,
        ImGuiColorEditMode_HSV = 1,
        ImGuiColorEditMode_HEX = 2
    };

}
