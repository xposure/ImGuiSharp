// 2D axis aligned bounding-box
// NB: we can't rely on ImVec2 math operators being available here
struct IMGUI_API ImRect
{
	ImVec2      Min;    // Upper-left
	ImVec2      Max;    // Lower-right

	ImRect() : Min(FLT_MAX, FLT_MAX), Max(-FLT_MAX, -FLT_MAX) {}
	ImRect(const ImVec2& min, const ImVec2& max) : Min(min), Max(max) {}
	ImRect(const ImVec4& v) : Min(v.x, v.y), Max(v.z, v.w) {}
	ImRect(float x1, float y1, float x2, float y2) : Min(x1, y1), Max(x2, y2) {}

	ImVec2      GetCenter() const { return ImVec2((Min.x + Max.x)*0.5f, (Min.y + Max.y)*0.5f); }
	ImVec2      GetSize() const { return ImVec2(Max.x - Min.x, Max.y - Min.y); }
	float       GetWidth() const { return Max.x - Min.x; }
	float       GetHeight() const { return Max.y - Min.y; }
	ImVec2      GetTL() const { return Min; }                   // Top-left
	ImVec2      GetTR() const { return ImVec2(Max.x, Min.y); }  // Top-right
	ImVec2      GetBL() const { return ImVec2(Min.x, Max.y); }  // Bottom-left
	ImVec2      GetBR() const { return Max; }                   // Bottom-right
	bool        Contains(const ImVec2& p) const { return p.x >= Min.x     && p.y >= Min.y     && p.x < Max.x     && p.y < Max.y; }
	bool        Contains(const ImRect& r) const { return r.Min.x >= Min.x && r.Min.y >= Min.y && r.Max.x < Max.x && r.Max.y < Max.y; }
	bool        Overlaps(const ImRect& r) const { return r.Min.y < Max.y  && r.Max.y > Min.y  && r.Min.x < Max.x && r.Max.x > Min.x; }
	void        Add(const ImVec2& rhs) { if (Min.x > rhs.x)     Min.x = rhs.x;     if (Min.y > rhs.y) Min.y = rhs.y;         if (Max.x < rhs.x) Max.x = rhs.x;         if (Max.y < rhs.y) Max.y = rhs.y; }
	void        Add(const ImRect& rhs) { if (Min.x > rhs.Min.x) Min.x = rhs.Min.x; if (Min.y > rhs.Min.y) Min.y = rhs.Min.y; if (Max.x < rhs.Max.x) Max.x = rhs.Max.x; if (Max.y < rhs.Max.y) Max.y = rhs.Max.y; }
	void        Expand(const float amount) { Min.x -= amount;   Min.y -= amount;   Max.x += amount;   Max.y += amount; }
	void        Expand(const ImVec2& amount) { Min.x -= amount.x; Min.y -= amount.y; Max.x += amount.x; Max.y += amount.y; }
	void        Reduce(const ImVec2& amount) { Min.x += amount.x; Min.y += amount.y; Max.x -= amount.x; Max.y -= amount.y; }
	void        Clip(const ImRect& clip) { if (Min.x < clip.Min.x) Min.x = clip.Min.x; if (Min.y < clip.Min.y) Min.y = clip.Min.y; if (Max.x > clip.Max.x) Max.x = clip.Max.x; if (Max.y > clip.Max.y) Max.y = clip.Max.y; }
	void        Round() { Min.x = (float)(int)Min.x; Min.y = (float)(int)Min.y; Max.x = (float)(int)Max.x; Max.y = (float)(int)Max.y; }
	ImVec2      GetClosestPoint(ImVec2 p, bool on_edge) const
	{
		if (!on_edge && Contains(p))
			return p;
		if (p.x > Max.x) p.x = Max.x;
		else if (p.x < Min.x) p.x = Min.x;
		if (p.y > Max.y) p.y = Max.y;
		else if (p.y < Min.y) p.y = Min.y;
		return p;
	}
};