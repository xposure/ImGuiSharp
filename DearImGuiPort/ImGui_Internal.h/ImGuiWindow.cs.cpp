//-----------------------------------------------------------------------------
// ImGuiWindow
//-----------------------------------------------------------------------------

ImGuiWindow::ImGuiWindow(const char* name)
{
	Name = ImStrdup(name);
	ID = ImHash(name, 0);
	IDStack.push_back(ID);
	MoveID = GetID("#MOVE");

	Flags = 0;
	PosFloat = Pos = ImVec2(0.0f, 0.0f);
	Size = SizeFull = ImVec2(0.0f, 0.0f);
	SizeContents = SizeContentsExplicit = ImVec2(0.0f, 0.0f);
	WindowPadding = ImVec2(0.0f, 0.0f);
	Scroll = ImVec2(0.0f, 0.0f);
	ScrollTarget = ImVec2(FLT_MAX, FLT_MAX);
	ScrollTargetCenterRatio = ImVec2(0.5f, 0.5f);
	ScrollbarX = ScrollbarY = false;
	ScrollbarSizes = ImVec2(0.0f, 0.0f);
	BorderSize = 0.0f;
	Active = WasActive = false;
	Accessed = false;
	Collapsed = false;
	SkipItems = false;
	BeginCount = 0;
	PopupID = 0;
	AutoFitFramesX = AutoFitFramesY = -1;
	AutoFitOnlyGrows = false;
	AutoPosLastDirection = -1;
	HiddenFrames = 0;
	SetWindowPosAllowFlags = SetWindowSizeAllowFlags = SetWindowCollapsedAllowFlags = ImGuiSetCond_Always | ImGuiSetCond_Once | ImGuiSetCond_FirstUseEver | ImGuiSetCond_Appearing;
	SetWindowPosCenterWanted = false;

	LastFrameActive = -1;
	ItemWidthDefault = 0.0f;
	FontWindowScale = 1.0f;

	DrawList = (ImDrawList*)ImGui::MemAlloc(sizeof(ImDrawList));
	IM_PLACEMENT_NEW(DrawList) ImDrawList();
	DrawList->_OwnerName = Name;
	RootWindow = NULL;
	RootNonPopupWindow = NULL;

	FocusIdxAllCounter = FocusIdxTabCounter = -1;
	FocusIdxAllRequestCurrent = FocusIdxTabRequestCurrent = IM_INT_MAX;
	FocusIdxAllRequestNext = FocusIdxTabRequestNext = IM_INT_MAX;
}

ImGuiWindow::~ImGuiWindow()
{
	DrawList->~ImDrawList();
	ImGui::MemFree(DrawList);
	DrawList = NULL;
	ImGui::MemFree(Name);
	Name = NULL;
}

uint ImGuiWindow::GetID(const char* str, const char* str_end)
{
	uint seed = IDStack.back();
	uint id = ImHash(str, str_end ? (int)(str_end - str) : 0, seed);
	ImGui::KeepAliveID(id);
	return id;
}

uint ImGuiWindow::GetID(const void* ptr)
{
	uint seed = IDStack.back();
	uint id = ImHash(&ptr, sizeof(void*), seed);
	ImGui::KeepAliveID(id);
	return id;
}