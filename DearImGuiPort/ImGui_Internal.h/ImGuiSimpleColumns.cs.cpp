//-----------------------------------------------------------------------------
// ImGuiSimpleColumns
//-----------------------------------------------------------------------------

ImGuiSimpleColumns::ImGuiSimpleColumns()
{
	Count = 0;
	Spacing = Width = NextWidth = 0.0f;
	memset(Pos, 0, sizeof(Pos));
	memset(NextWidths, 0, sizeof(NextWidths));
}

void ImGuiSimpleColumns::Update(int count, float spacing, bool clear)
{
	IM_ASSERT(Count <= IM_ARRAYSIZE(Pos));
	Count = count;
	Width = NextWidth = 0.0f;
	Spacing = spacing;
	if (clear) memset(NextWidths, 0, sizeof(NextWidths));
	for (int i = 0; i < Count; i++)
	{
		if (i > 0 && NextWidths[i] > 0.0f)
			Width += Spacing;
		Pos[i] = (float)(int)Width;
		Width += NextWidths[i];
		NextWidths[i] = 0.0f;
	}
}

float ImGuiSimpleColumns::DeclColumns(float w0, float w1, float w2) // not using va_arg because they promote float to double
{
	NextWidth = 0.0f;
	NextWidths[0] = ImMax(NextWidths[0], w0);
	NextWidths[1] = ImMax(NextWidths[1], w1);
	NextWidths[2] = ImMax(NextWidths[2], w2);
	for (int i = 0; i < 3; i++)
		NextWidth += NextWidths[i] + ((i > 0 && NextWidths[i] > 0.0f) ? Spacing : 0.0f);
	return ImMax(Width, NextWidth);
}

float ImGuiSimpleColumns::CalcExtraSpace(float avail_w)
{
	return ImMax(0.0f, avail_w - Width);
}