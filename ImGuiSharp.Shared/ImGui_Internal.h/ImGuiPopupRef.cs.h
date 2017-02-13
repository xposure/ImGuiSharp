// Storage for current popup stack
struct ImGuiPopupRef
{
	uint         PopupID;        // Set on OpenPopup()
	ImGuiWindow*    Window;         // Resolved on BeginPopup() - may stay unresolved if user never calls OpenPopup()
	ImGuiWindow*    ParentWindow;   // Set on OpenPopup()
	uint         ParentMenuSet;  // Set on OpenPopup()
	ImVec2          MousePosOnOpen; // Copy of mouse position at the time of opening popup

	ImGuiPopupRef(uint id, ImGuiWindow* parent_window, uint parent_menu_set, const ImVec2& mouse_pos) { PopupID = id; Window = NULL; ParentWindow = parent_window; ParentMenuSet = parent_menu_set; MousePosOnOpen = mouse_pos; }
};