namespace ImGui
{
    // A single draw command within a parent ImDrawList (generally maps to 1 GPU draw call)
    // Typically, 1 command = 1 gpu draw call (unless command is a callback)
    public struct ImDrawCmd
    {
        public uint ElemCount;              // Number of indices (multiple of 3) to be rendered as triangles. Vertices are stored in the callee ImDrawList's vtx_buffer[] array, indices in idx_buffer[].
        public ImVec4 ClipRect;               // Clipping rectangle (x1, y1, x2, y2)
        public ImTextureID TextureId;              // User-provided texture ID. Set by user in ImfontAtlas::SetTexID() for fonts or passed to Image*() functions. Ignore if never using images or multiple fonts atlas.
        public ImDrawCallback UserCallback;           // If != NULL, call the function instead of rendering the vertices. clip_rect and texture_id will be set normally.
        public object UserCallbackData;       // The draw callback code can access this.

        //public ImDrawCmd()
        //{
        //    ElemCount = 0;
        //    ClipRect.x = ClipRect.y = -8192.0f;
        //    ClipRect.z = ClipRect.w = +8192.0f;
        //    TextureId = 0;
        //    UserCallback = null;
        //    //UserCallbackData = null;
        //}
    }
}
