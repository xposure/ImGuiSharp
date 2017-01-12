namespace ImGui
{
    // Stacked color modifier, backup of modified data so we can restore it
    internal struct ImGuiColMod
    {
        public ImGuiCol Col;
        public ImVec4 PreviousValue;
    }
}
