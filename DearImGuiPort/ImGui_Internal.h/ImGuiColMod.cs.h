// Stacked color modifier, backup of modified data so we can restore it
struct ImGuiColMod
{
	ImGuiCol    Col;
	ImVec4      PreviousValue;
};