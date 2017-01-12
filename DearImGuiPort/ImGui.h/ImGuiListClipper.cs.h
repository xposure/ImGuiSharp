// Helper: Manually clip large list of items.
// If you are displaying thousands of even spaced items and you have a random access to the list, you can perform clipping yourself to save on CPU.
// Usage:
//    ImGuiListClipper clipper(count, ImGui::GetTextLineHeightWithSpacing());
//    for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++) // display only visible items
//        ImGui::Text("line number %d", i);
//    clipper.End();
// NB: 'count' is only used to clamp the result, if you don't know your count you can use INT_MAX
struct ImGuiListClipper
{
	float   ItemsHeight;
	int     ItemsCount, DisplayStart, DisplayEnd;

	ImGuiListClipper() { ItemsHeight = 0.0f; ItemsCount = DisplayStart = DisplayEnd = -1; }
	ImGuiListClipper(int count, float height) { ItemsCount = -1; Begin(count, height); }
	~ImGuiListClipper() { IM_ASSERT(ItemsCount == -1); } // user forgot to call End()

	void Begin(int count, float height)        // items_height: generally pass GetTextLineHeightWithSpacing() or GetItemsLineHeightWithSpacing()
	{
		IM_ASSERT(ItemsCount == -1);
		ItemsCount = count;
		ItemsHeight = height;
		ImGui::CalcListClipping(ItemsCount, ItemsHeight, &DisplayStart, &DisplayEnd); // calculate how many to clip/display
		ImGui::SetCursorPosY(ImGui::GetCursorPosY() + DisplayStart * ItemsHeight);    // advance cursor
	}
	void End()
	{
		IM_ASSERT(ItemsCount >= 0);
		ImGui::SetCursorPosY(ImGui::GetCursorPosY() + (ItemsCount - DisplayEnd) * ItemsHeight); // advance cursor
		ItemsCount = -1;
	}
};