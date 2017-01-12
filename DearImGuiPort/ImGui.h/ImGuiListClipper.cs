
namespace ImGui
{
    // Helper: Manually clip large list of items.
    // If you are displaying thousands of even spaced items and you have a random access to the list, you can perform clipping yourself to save on CPU.
    // Usage:
    //    ImGuiListClipper clipper(count, ImGui::GetTextLineHeightWithSpacing());
    //    for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++) // display only visible items
    //        ImGui::Text("line number %d", i);
    //    clipper.End();
    // NB: 'count' is only used to clamp the result, if you don't know your count you can use INT_MAX
    internal class ImGuiListClipper
    {
        internal float ItemsHeight;
        internal int ItemsCount, DisplayStart, DisplayEnd;

        internal ImGuiListClipper() { ItemsHeight = 0.0f; ItemsCount = DisplayStart = DisplayEnd = -1; }
        internal ImGuiListClipper(int count, float height) { ItemsCount = -1; Begin(count, height); }
        ~ImGuiListClipper() { System.Diagnostics.Debug.Assert(ItemsCount == -1); } // user forgot to call End()

        internal void Begin(int count, float height)        // items_height: generally pass GetTextLineHeightWithSpacing() or GetItemsLineHeightWithSpacing()
        {
            System.Diagnostics.Debug.Assert(ItemsCount == -1);
            ItemsCount = count;
            ItemsHeight = height;
            ImGui.Instance.CalcListClipping(ItemsCount, ItemsHeight, ref DisplayStart, ref DisplayEnd); // calculate how many to clip/display
            ImGui.Instance.SetCursorPosY(ImGui.Instance.GetCursorPosY() + DisplayStart * ItemsHeight);    // advance cursor
        }
        internal void End()
        {
            System.Diagnostics.Debug.Assert(ItemsCount >= 0);
            ImGui.Instance.SetCursorPosY(ImGui.Instance.GetCursorPosY() + (ItemsCount - DisplayEnd) * ItemsHeight); // advance cursor
            ItemsCount = -1;
        }
    };
}
