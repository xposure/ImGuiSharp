//-----------------------------------------------------------------------------
// Draw List
// Hold a series of drawing commands. The user provides a renderer for ImDrawData which essentially contains an array of ImDrawList.
//-----------------------------------------------------------------------------

// Helpers macros to generate 32-bits encoded colors
#define IM_COL32(R,G,B,A)    (((uint)(A)<<24) | ((uint)(B)<<16) | ((uint)(G)<<8) | ((uint)(R)))
#define IM_COL32_WHITE       (0xFFFFFFFF)
#define IM_COL32_BLACK       (0xFF000000)
#define IM_COL32_BLACK_TRANS (0x00000000)    // Transparent black

// ImColor() is just a helper that implicity converts to either uint (packed 4x1 byte) or ImVec4 (4x1 float)
// None of the ImGui API are using ImColor directly but you can use it as a convenience to pass colors in either uint or ImVec4 formats.
struct ImColor
{
	ImVec4              Value;

	ImColor() { Value.x = Value.y = Value.z = Value.w = 0.0f; }
	ImColor(int r, int g, int b, int a = 255) { float sc = 1.0f / 255.0f; Value.x = (float)r * sc; Value.y = (float)g * sc; Value.z = (float)b * sc; Value.w = (float)a * sc; }
	ImColor(uint rgba) { float sc = 1.0f / 255.0f; Value.x = (float)(rgba & 0xFF) * sc; Value.y = (float)((rgba >> 8) & 0xFF) * sc; Value.z = (float)((rgba >> 16) & 0xFF) * sc; Value.w = (float)(rgba >> 24) * sc; }
	ImColor(float r, float g, float b, float a = 1.0f) { Value.x = r; Value.y = g; Value.z = b; Value.w = a; }
	ImColor(const ImVec4& col) { Value = col; }
	inline operator uint() const { return ImGui::ColorConvertFloat4ToU32(Value); }
	inline operator ImVec4() const { return Value; }

	inline void    SetHSV(float h, float s, float v, float a = 1.0f) { ImGui::ColorConvertHSVtoRGB(h, s, v, Value.x, Value.y, Value.z); Value.w = a; }

	static ImColor HSV(float h, float s, float v, float a = 1.0f) { float r, g, b; ImGui::ColorConvertHSVtoRGB(h, s, v, r, g, b); return ImColor(r, g, b, a); }
};