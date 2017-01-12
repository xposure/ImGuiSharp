namespace ImGui
{
    // Helper functions to create a color that can be converted to either u32 or float4
    public struct ImColor
    {
        public ImVec4 Value;
        public ImColor(int r, int g, int b, int a = 255) { float sc = 1.0f / 255.0f; Value.x = (float)r * sc; Value.y = (float)g * sc; Value.z = (float)b * sc; Value.w = (float)a * sc; }
        public ImColor(uint rgba) { float sc = 1.0f / 255.0f; Value.x = (float)(rgba & 0xFF) * sc; Value.y = (float)((rgba >> 8) & 0xFF) * sc; Value.z = (float)((rgba >> 16) & 0xFF) * sc; Value.w = (float)(rgba >> 24) * sc; }
        public ImColor(float r, float g, float b, float a = 1.0f) { Value.x = r; Value.y = g; Value.z = b; Value.w = a; }
        public ImColor(ImVec4 col) { Value = col; }

        public static implicit operator uint(ImColor value) { return ImGui.ColorConvertFloat4ToU32(value.Value); }
        public static implicit operator ImVec4(ImColor value) { return value.Value; }
        public static ImColor HSV(float h, float s, float v, float a = 1.0f) { float r, g, b; ImGui.ColorConvertHSVtoRGB(h, s, v, out r, out g, out b); return new ImColor(r, g, b, a); }

        public void SetHSV(float h, float s, float v, float a = 1.0f) { ImGui.ColorConvertHSVtoRGB(h, s, v, out Value.x, out Value.y, out Value.z); Value.w = a; }

        //#define IM_COL32(R,G,B,A)    (((uint)(A)<<24) | ((uint)(B)<<16) | ((uint)(G)<<8) | ((uint)(R)))
        public static ImColor White { get { return new ImColor(0xFFFFFFFF); } }
        public static ImColor Black { get { return new ImColor(0xFF000000); } }
        public static ImColor Transparent { get { return new ImColor(0x00000000); } }

    }
}
