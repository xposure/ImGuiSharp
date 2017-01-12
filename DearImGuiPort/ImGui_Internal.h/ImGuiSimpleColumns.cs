namespace ImGui
{
    // Simple column measurement currently used for MenuItem() only. This is very short-sighted for now and NOT a generic helper.
    internal class ImGuiSimpleColumns
    {
        internal int Count;
        internal float Spacing;
        internal float Width, NextWidth;
        internal float[] Pos = new float[8];
        internal float[] NextWidths = new float[8];

        //ImGuiSimpleColumns();
        internal void Update(int count, float spacing, bool clear)
        {
            System.Diagnostics.Debug.Assert(Count <= Pos.Length);
            Count = count;
            Width = NextWidth = 0.0f;
            Spacing = spacing;
            if (clear)
                for (var i = 0; i < NextWidths.Length; i++)
                    NextWidths[i] = 0;

            for (int i = 0; i < Count; i++)
            {
                if (i > 0 && NextWidths[i] > 0.0f)
                    Width += Spacing;
                Pos[i] = (float)(int)Width;
                Width += NextWidths[i];
                NextWidths[i] = 0.0f;
            }
        }
        internal float DeclColumns(float w0, float w1, float w2)
        {
            NextWidth = 0.0f;
            NextWidths[0] = ImGui.Max(NextWidths[0], w0);
            NextWidths[1] = ImGui.Max(NextWidths[1], w1);
            NextWidths[2] = ImGui.Max(NextWidths[2], w2);
            for (int i = 0; i < 3; i++)
                NextWidth += NextWidths[i] + ((i > 0 && NextWidths[i] > 0.0f) ? Spacing : 0.0f);
            return ImGui.Max(Width, NextWidth);
        }
        internal float CalcExtraSpace(float avail_w)
        {
            return ImGui.Max(0.0f, avail_w - Width);
        }
    }
}
