namespace ImGui
{
    using System;
    // flags for InputText*()

    // Flags for ImGui::InputText()
    [Flags]
    public enum ImGuiInputTextFlags : int
    {
        // Default: 0
        ImGuiInputTextFlags_CharsDecimal = 1 << 0,   // Allow 0123456789.+-*/
        ImGuiInputTextFlags_CharsHexadecimal = 1 << 1,   // Allow 0123456789ABCDEFabcdef
        ImGuiInputTextFlags_CharsUppercase = 1 << 2,   // Turn a..z into A..Z
        ImGuiInputTextFlags_CharsNoBlank = 1 << 3,   // Filter out spaces, tabs
        ImGuiInputTextFlags_AutoSelectAll = 1 << 4,   // Select entire text when first taking mouse focus
        ImGuiInputTextFlags_EnterReturnsTrue = 1 << 5,   // Return 'true' when Enter is pressed (as opposed to when the value was modified)
        ImGuiInputTextFlags_CallbackCompletion = 1 << 6,   // Call user function on pressing TAB (for completion handling)
        ImGuiInputTextFlags_CallbackHistory = 1 << 7,   // Call user function on pressing Up/Down arrows (for history handling)
        ImGuiInputTextFlags_CallbackAlways = 1 << 8,   // Call user function every time. User code may query cursor position, modify text buffer.
        ImGuiInputTextFlags_CallbackCharFilter = 1 << 9,   // Call user function to filter character. Modify data->EventChar to replace/filter input, or return 1 to discard character.
        ImGuiInputTextFlags_AllowTabInput = 1 << 10,  // Pressing TAB input a '\t' character into the text field
        ImGuiInputTextFlags_CtrlEnterForNewLine = 1 << 11,  // In multi-line mode, allow exiting edition by pressing Enter. Ctrl+Enter to add new line (by default adds new lines with Enter).
        ImGuiInputTextFlags_NoHorizontalScroll = 1 << 12,  // Disable following the cursor horizontally
        ImGuiInputTextFlags_AlwaysInsertMode = 1 << 13,  // Insert mode
        ImGuiInputTextFlags_ReadOnly = 1 << 14,  // Read-only mode
        ImGuiInputTextFlags_Password = 1 << 15,  // Password mode, display all characters as '*'
                                                 // [Internal]
        ImGuiInputTextFlags_Multiline = 1 << 20   // For internal use by InputTextMultiline()
    };

}
