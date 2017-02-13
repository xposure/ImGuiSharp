//-----------------------------------------------------------------------------
// ImGuiTextFilter
//-----------------------------------------------------------------------------

// Helper: Parse and apply text filters. In format "aaaaa[,bbbb][,ccccc]"
ImGuiTextFilter::ImGuiTextFilter(const char* default_filter)
{
	if (default_filter)
	{
		ImFormatString(InputBuf, IM_ARRAYSIZE(InputBuf), "%s", default_filter);
		Build();
	}
	else
	{
		InputBuf[0] = 0;
		CountGrep = 0;
	}
}

bool ImGuiTextFilter::Draw(const char* label, float width)
{
	if (width != 0.0f)
		ImGui::PushItemWidth(width);
	bool value_changed = ImGui::InputText(label, InputBuf, IM_ARRAYSIZE(InputBuf));
	if (width != 0.0f)
		ImGui::PopItemWidth();
	if (value_changed)
		Build();
	return value_changed;
}

void ImGuiTextFilter::TextRange::split(char separator, ImVector<TextRange>& out)
{
	out.resize(0);
	const char* wb = b;
	const char* we = wb;
	while (we < e)
	{
		if (*we == separator)
		{
			out.push_back(TextRange(wb, we));
			wb = we + 1;
		}
		we++;
	}
	if (wb != we)
		out.push_back(TextRange(wb, we));
}

void ImGuiTextFilter::Build()
{
	Filters.resize(0);
	TextRange input_range(InputBuf, InputBuf + strlen(InputBuf));
	input_range.split(',', Filters);

	CountGrep = 0;
	for (int i = 0; i != Filters.Size; i++)
	{
		Filters[i].trim_blanks();
		if (Filters[i].empty())
			continue;
		if (Filters[i].front() != '-')
			CountGrep += 1;
	}
}

bool ImGuiTextFilter::PassFilter(const char* text, const char* text_end) const
{
	if (Filters.empty())
		return true;

	if (text == NULL)
		text = "";

	for (int i = 0; i != Filters.Size; i++)
	{
		const TextRange& f = Filters[i];
		if (f.empty())
			continue;
		if (f.front() == '-')
		{
			// Subtract
			if (ImStristr(text, text_end, f.begin() + 1, f.end()) != NULL)
				return false;
		}
		else
		{
			// Grep
			if (ImStristr(text, text_end, f.begin(), f.end()) != NULL)
				return true;
		}
	}

	// Implicit * grep
	if (CountGrep == 0)
		return true;

	return false;
}