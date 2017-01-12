
namespace ImGui
{
    // All draw command lists required to render the frame
    // All draw data to render an ImGui frame
    public class ImDrawData
    {
        public bool Valid;                  // Only valid after Render() is called and before the next NewFrame() is called.
                                            //public ImDrawList** CmdLists;
        public ImVector<ImDrawList> CmdLists;
        public int CmdListsCount;
        public int TotalVtxCount;          // For convenience, sum of all cmd_lists vtx_buffer.Size
        public int TotalIdxCount;          // For convenience, sum of all cmd_lists idx_buffer.Size

        // Functions
        public ImDrawData() {
            //CmdLists = new ImDrawList();
            Valid = false;
            CmdLists = null;
            CmdListsCount = TotalVtxCount = TotalIdxCount = 0;
        }
        //public void DeIndexAllBuffers();               // For backward compatibility: convert all buffers from indexed to de-indexed, in case you cannot render indexed. Note: this is slow and most likely a waste of resources. Always prefer indexed rendering!
        //public void ScaleClipRects(ImVec2 sc);  // Helper to scale the ClipRect field of each ImDrawCmd. Use if your final output buffer is at a different scale than ImGui expects, or if there is a difference between your window resolution and framebuffer resolution.
    }
}
