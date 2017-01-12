// Helper: execute a block of code at maximum once a frame
// Convenient if you want to quickly create an UI within deep-nested code that runs multiple times every frame.
// Usage:
//   IMGUI_ONCE_UPON_A_FRAME
//   {
//      // code block will be executed one per frame
//   }
// Attention! the macro expands into 2 statement so make sure you don't use it within e.g. an if() statement without curly braces.
#define IMGUI_ONCE_UPON_A_FRAME    static ImGuiOnceUponAFrame imgui_oaf##__LINE__; if (imgui_oaf##__LINE__)
struct ImGuiOnceUponAFrame
{
	ImGuiOnceUponAFrame() { RefFrame = -1; }
	mutable int RefFrame;
	operator bool() const { int current_frame = ImGui::GetFrameCount(); if (RefFrame == current_frame) return false; RefFrame = current_frame; return true; }
};