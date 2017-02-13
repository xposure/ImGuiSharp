// Stacked style modifier, backup of modified data so we can restore it
struct ImGuiStyleMod
{
	ImGuiStyleVar   Var;
	ImVec2          PreviousValue;
};