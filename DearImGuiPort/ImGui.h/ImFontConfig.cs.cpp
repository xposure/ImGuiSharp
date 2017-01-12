ImFontConfig::ImFontConfig()
{
	FontData = NULL;
	FontDataSize = 0;
	FontDataOwnedByAtlas = true;
	FontNo = 0;
	SizePixels = 0.0f;
	OversampleH = 3;
	OversampleV = 1;
	PixelSnapH = false;
	GlyphExtraSpacing = ImVec2(0.0f, 0.0f);
	GlyphRanges = NULL;
	MergeMode = false;
	MergeGlyphCenterV = false;
	DstFont = NULL;
	memset(Name, 0, sizeof(Name));
}