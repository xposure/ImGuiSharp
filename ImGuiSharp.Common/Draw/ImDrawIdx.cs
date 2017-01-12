namespace ImGui
{
    public struct ImDrawIdx
    {
        public ushort Value;

        public ImDrawIdx(ushort val)
        {
            Value = val;
        }

        public static implicit operator ushort(ImDrawIdx val)
        {
            return val.Value;
        }

        public static implicit operator ImDrawIdx(ushort val)
        {
            return new ImDrawIdx(val);
        }
    }
}
