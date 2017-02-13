using System.Diagnostics;

namespace ImGui
{
    // 2D axis aligned bounding-box
    // NB: we can't rely on ImVec2 math operators being available here
    [DebuggerDisplay("Min: {Min}, Max: {Max}")]
    internal struct ImRect
    {
        public ImVec2 Min;    // Upper-left
        public ImVec2 Max;    // Lower-right

        public static ImRect Empty
        {
            get
            {
                return new ImRect(
                    new ImVec2(float.MaxValue, float.MaxValue),
                    new ImVec2(float.MinValue, float.MinValue));
            }
        }

        public ImRect(ImVec2 min, ImVec2 max)
        {
            Min = min;
            Max = max;
        }
        
        public ImRect(ImVec4 v) : this(new ImVec2(v.x, v.y), new ImVec2(v.z, v.w)) { }
        public ImRect(float x1, float y1, float x2, float y2) : this(new ImVec2(x1, y1), new ImVec2(x2, y2)) { }

        public ImVec2 GetCenter() { return new ImVec2((Min.x + Max.x) * 0.5f, (Min.y + Max.y) * 0.5f); }

        public ImVec2 GetSize() { return new ImVec2(Max.x - Min.x, Max.y - Min.y); }

        public float GetWidth() { return Max.x - Min.x; }
        public float GetHeight() { return Max.y - Min.y; }
        public ImVec2 GetTL() { return Min; }                   // Top-left
        public ImVec2 GetTR() { return new ImVec2(Max.x, Min.y); }  // Top-right
        public ImVec2 GetBL() { return new ImVec2(Min.x, Max.y); }  // Bottom-left
        public ImVec2 GetBR() { return Max; }                   // Bottom-right

        public bool Contains(ImVec2 p) { return p.x >= Min.x && p.y >= Min.y && p.x < Max.x && p.y < Max.y; }
        public bool Contains(ImRect r) { return r.Min.x >= Min.x && r.Min.y >= Min.y && r.Max.x < Max.x && r.Max.y < Max.y; }
        public bool Overlaps(ImRect r) { return r.Min.y < Max.y && r.Max.y > Min.y && r.Min.x < Max.x && r.Max.x > Min.x; }
        public void Add(ImVec2 rhs) { if (Min.x > rhs.x) Min.x = rhs.x; if (Min.y > rhs.y) Min.y = rhs.y; if (Max.x < rhs.x) Max.x = rhs.x; if (Max.y < rhs.y) Max.y = rhs.y; }
        public void Add(ImRect rhs) { if (Min.x > rhs.Min.x) Min.x = rhs.Min.x; if (Min.y > rhs.Min.y) Min.y = rhs.Min.y; if (Max.x < rhs.Max.x) Max.x = rhs.Max.x; if (Max.y < rhs.Max.y) Max.y = rhs.Max.y; }
        public void Expand(float amount) { Min.x -= amount; Min.y -= amount; Max.x += amount; Max.y += amount; }
        public void Expand(ImVec2 amount) { Min.x -= amount.x; Min.y -= amount.y; Max.x += amount.x; Max.y += amount.y; }
        public void Reduce(ImVec2 amount) { Min.x += amount.x; Min.y += amount.y; Max.x -= amount.x; Max.y -= amount.y; }
        public void Clip(ImRect clip) { if (Min.x < clip.Min.x) Min.x = clip.Min.x; if (Min.y < clip.Min.y) Min.y = clip.Min.y; if (Max.x > clip.Max.x) Max.x = clip.Max.x; if (Max.y > clip.Max.y) Max.y = clip.Max.y; }
        public void Round() { Min.x = (float)(int)Min.x; Min.y = (float)(int)Min.y; Max.x = (float)(int)Max.x; Max.y = (float)(int)Max.y; }

        public ImVec2 GetClosestPoint(ImVec2 p, bool on_edge)
        {
            if (!on_edge && Contains(p))
                return p;
            if (p.x > Max.x) p.x = Max.x;
            else if (p.x < Min.x) p.x = Min.x;
            if (p.y > Max.y) p.y = Max.y;
            else if (p.y < Min.y) p.y = Min.y;
            return p;
        }

        public float x
        {
            get { return Min.x; }
            set { Min.x = value; }
        }

        public float y
        {
            get { return Min.y; }
            set { Min.y = value; }
        }

        public float width
        {
            get { return Max.x - Min.x; }
            set { Max.x = Min.x + value; }
        }

        public float height
        {
            get { return Max.y - Min.y; }
            set { Max.y = Min.y + value; }
        }

        public override string ToString()
        {
            return $"{{ Min: {Min}, Max: {Max} }}";
        }
    }
}
