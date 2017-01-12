namespace ImGui
{
    // Storage for current popup stack
    internal class ImGuiPopupRef
    {
        internal uint PopupID;        // Set on OpenPopup()
                                         //ImGuiWindow* Window;         // Resolved on BeginPopup() - may stay unresolved if user never calls OpenPopup()
        internal ImGuiWindow Window;
        //ImGuiWindow ParentWindow;   // Set on OpenPopup()
        internal ImGuiWindow ParentWindow;
        internal uint ParentMenuSet;  // Set on OpenPopup()
        internal ImVec2 MousePosOnOpen; // Copy of mouse position at the time of opening popup

        internal ImGuiPopupRef(uint id, ImGuiWindow parent_window, uint parent_menu_set, ImVec2 mouse_pos) { PopupID = id; Window = null; ParentWindow = parent_window; ParentMenuSet = parent_menu_set; MousePosOnOpen = mouse_pos; }
    }
}
