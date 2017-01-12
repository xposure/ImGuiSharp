// Mouse cursor data (used when io.MouseDrawCursor is set)
struct ImGuiMouseCursorData
{
	ImGuiMouseCursor    Type;
	ImVec2              HotOffset;
	ImVec2              Size;
	ImVec2              TexUvMin[2];
	ImVec2              TexUvMax[2];
};