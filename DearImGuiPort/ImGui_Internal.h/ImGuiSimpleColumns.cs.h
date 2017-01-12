// Simple column measurement currently used for MenuItem() only. This is very short-sighted for now and NOT a generic helper.
struct IMGUI_API ImGuiSimpleColumns
{
	int         Count;
	float       Spacing;
	float       Width, NextWidth;
	float       Pos[8], NextWidths[8];

	ImGuiSimpleColumns();
	void       Update(int count, float spacing, bool clear);
	float      DeclColumns(float w0, float w1, float w2);
	float      CalcExtraSpace(float avail_w);
};