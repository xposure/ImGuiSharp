namespace ImGui
{
    using Microsoft.Xna.Framework.Input;

    partial class ImGuiIO
    {
        partial void PlatformInitialize()
        {
            KeyMap[(int)ImGuiKey.ImGuiKey_Tab] = (int)Keys.Tab + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_LeftArrow] = (int)Keys.Left + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_RightArrow] = (int)Keys.Right + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_DownArrow] = (int)Keys.Down + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_PageUp] = (int)Keys.PageUp + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_PageDown] = (int)Keys.PageDown + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Home] = (int)Keys.Home + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_End] = (int)Keys.End + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Delete] = (int)Keys.Delete + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Backspace] = (int)Keys.Back + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Enter] = (int)Keys.Enter + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Escape] = (int)Keys.Escape + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_A] = (int)Keys.A + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_C] = (int)Keys.C + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_V] = (int)Keys.V + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_X] = (int)Keys.X + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Y] = (int)Keys.Y + 0xff;
            KeyMap[(int)ImGuiKey.ImGuiKey_Z] = (int)Keys.Z + 0xff;
        }
    }
}
