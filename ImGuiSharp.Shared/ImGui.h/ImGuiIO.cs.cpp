ImGuiIO::ImGuiIO()
{
	// Most fields are initialized with zero
	memset(this, 0, sizeof(*this));

	DisplaySize = ImVec2(-1.0f, -1.0f);
	DeltaTime = 1.0f / 60.0f;
	IniSavingRate = 5.0f;
	IniFilename = "imgui.ini";
	LogFilename = "imgui_log.txt";
	Fonts = &GImDefaultFontAtlas;
	FontGlobalScale = 1.0f;
	DisplayFramebufferScale = ImVec2(1.0f, 1.0f);
	MousePos = ImVec2(-1, -1);
	MousePosPrev = ImVec2(-1, -1);
	MouseDoubleClickTime = 0.30f;
	MouseDoubleClickMaxDist = 6.0f;
	MouseDragThreshold = 6.0f;
	for (int i = 0; i < IM_ARRAYSIZE(MouseDownDuration); i++)
		MouseDownDuration[i] = MouseDownDurationPrev[i] = -1.0f;
	for (int i = 0; i < IM_ARRAYSIZE(KeysDownDuration); i++)
		KeysDownDuration[i] = KeysDownDurationPrev[i] = -1.0f;
	for (int i = 0; i < ImGuiKey_COUNT; i++)
		KeyMap[i] = -1;
	KeyRepeatDelay = 0.250f;
	KeyRepeatRate = 0.050f;
	UserData = NULL;

	// User functions
	RenderDrawListsFn = NULL;
	MemAllocFn = malloc;
	MemFreeFn = free;
	GetClipboardTextFn = GetClipboardTextFn_DefaultImpl;   // Platform dependent default implementations
	SetClipboardTextFn = SetClipboardTextFn_DefaultImpl;
	ImeSetInputScreenPosFn = ImeSetInputScreenPosFn_DefaultImpl;
}

// Pass in translated ASCII characters for text input.
// - with glfw you can get those from the callback set in glfwSetCharCallback()
// - on Windows you can get those using ToAscii+keyboard state, or via the WM_CHAR message
void ImGuiIO::AddInputCharacter(ImWchar c)
{
	const int n = ImStrlenW(InputCharacters);
	if (n + 1 < IM_ARRAYSIZE(InputCharacters))
	{
		InputCharacters[n] = c;
		InputCharacters[n + 1] = '\0';
	}
}

void ImGuiIO::AddInputCharactersUTF8(const char* utf8_chars)
{
	// We can't pass more wchars than ImGuiIO::InputCharacters[] can hold so don't convert more
	const int wchars_buf_len = sizeof(ImGuiIO::InputCharacters) / sizeof(ImWchar);
	ImWchar wchars[wchars_buf_len];
	ImTextStrFromUtf8(wchars, wchars_buf_len, utf8_chars, NULL);
	for (int i = 0; i < wchars_buf_len && wchars[i] != 0; i++)
		AddInputCharacter(wchars[i]);
}