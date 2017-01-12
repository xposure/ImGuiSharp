namespace ImGui
{
    // A single vertex (20 bytes by default, override layout with IMGUI_OVERRIDE_DRAWVERT_STRUCT_LAYOUT)
    public struct ImDrawVert
    {
        public ImVec2 pos;
        public ImVec2 uv;
        public uint col;
    }
}
