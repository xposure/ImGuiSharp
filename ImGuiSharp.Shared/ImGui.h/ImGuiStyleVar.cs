using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGui
{
    // a variable identifier for styling

    // Enumeration for PushStyleVar() / PopStyleVar()
    // NB: the enum only refers to fields of ImGuiStyle() which makes sense to be pushed/poped in UI code. Feel free to add others.
    public enum ImGuiStyleVar : int
    {
        ImGuiStyleVar_Alpha,               // float
        ImGuiStyleVar_WindowPadding,       // ImVec2
        ImGuiStyleVar_WindowRounding,      // float
        ImGuiStyleVar_WindowMinSize,       // ImVec2
        ImGuiStyleVar_ChildWindowRounding, // float
        ImGuiStyleVar_FramePadding,        // ImVec2
        ImGuiStyleVar_FrameRounding,       // float
        ImGuiStyleVar_ItemSpacing,         // ImVec2
        ImGuiStyleVar_ItemInnerSpacing,    // ImVec2
        ImGuiStyleVar_IndentSpacing,       // float
        ImGuiStyleVar_GrabMinSize          // float
    };

}
