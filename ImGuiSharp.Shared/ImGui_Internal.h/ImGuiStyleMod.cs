namespace ImGui
{
    // Stacked style modifier, backup of modified data so we can restore it
    internal struct ImGuiStyleMod
    {
        public ImGuiStyleVar Var;
        public ImVec2 PreviousValue;
    }
}
