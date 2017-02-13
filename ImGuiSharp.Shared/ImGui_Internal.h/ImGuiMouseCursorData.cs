namespace ImGui
{
    // Mouse cursor data (used when io.MouseDrawCursor is set)
    internal class ImGuiMouseCursorData
    {
        public ImGuiMouseCursor Type;
        public ImVec2 HotOffset;
        public ImVec2 Size;
        public ImVec2[] TexUvMin = new ImVec2[2];
        public ImVec2[] TexUvMax = new ImVec2[2];        
    }
}
