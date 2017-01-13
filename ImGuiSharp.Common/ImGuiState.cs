using System.IO;
using System.Text;

namespace ImGui
{
    // Main state for ImGui
    internal class ImGuiState
    {
        internal bool Initialized;
        internal ImGuiIO IO;
        //internal ImGuiStyle Style;
        //internal ImFont Font;                               // (Shortcut) == FontStack.empty() ? IO.Font : FontStack.back()
        internal float FontSize;                           // (Shortcut) == FontBaseSize * g.CurrentWindow->FontWindowScale == window->FontSize()
        internal float FontBaseSize;                       // (Shortcut) == IO.FontGlobalScale * Font->Scale * Font->FontSize. Size of characters.
        internal ImVec2 FontTexUvWhitePixel;                // (Shortcut) == Font->TexUvWhitePixel

        internal float Time;
        internal int FrameCount;
        internal int FrameCountEnded;
        internal int FrameCountRendered;
        //internal ImVector<ImGuiWindow> Windows;
        //internal ImVector<ImGuiWindow> WindowsSortBuffer;
        //internal ImGuiWindow CurrentWindow;                      // Being drawn into
        //internal ImVector<ImGuiWindow> CurrentWindowStack;
        //internal ImGuiWindow FocusedWindow;                      // Will catch keyboard inputs
        //internal ImGuiWindow HoveredWindow;                      // Will catch mouse inputs
        //internal ImGuiWindow HoveredRootWindow;                  // Will catch mouse inputs (for focus/move only)
        internal uint HoveredId;                          // Hovered widget
        //internal bool HoveredIdAllowOverlap;
        internal uint HoveredIdPreviousFrame;
        internal uint ActiveId;                           // Active widget
        internal uint ActiveIdPreviousFrame;
        internal bool ActiveIdIsAlive;
        internal bool ActiveIdIsJustActivated;            // Set at the time of activation for one frame
        //internal bool ActiveIdAllowOverlap;               // Set only by active widget
        //internal ImGuiWindow ActiveIdWindow;
        //internal ImGuiWindow MovedWindow;                        // Track the child window we clicked on to move a window. Pointer is only valid if ActiveID is the "#MOVE" identifier of a window.

        //internal ImVector<ImGuiIniData> Settings;                           // .ini Settings
        //internal float SettingsDirtyTimer;                 // Save .ini settinngs on disk when time reaches zero

        //internal ImVector<ImGuiColMod> ColorModifiers;                     // Stack for PushStyleColor()/PopStyleColor()
        //internal ImVector<ImGuiStyleMod> StyleModifiers;                     // Stack for PushStyleVar()/PopStyleVar()
        //internal ImVector<ImFont> FontStack;                          // Stack for PushFont()/PopFont()
        //internal ImVector<ImGuiPopupRef> OpenedPopupStack;                   // Which popups are open (persistent)
        //internal ImVector<ImGuiPopupRef> CurrentPopupStack;                  // Which level of BeginPopup() we are in (reset every frame)

        //// Storage for SetNexWindow** and SetNextTreeNode*** functions
        //internal ImVec2 SetNextWindowPosVal;

        //internal ImVec2 SetNextWindowSizeVal;
        //internal ImVec2 SetNextWindowContentSizeVal;
        //internal bool SetNextWindowCollapsedVal;
        //internal ImGuiSetCond SetNextWindowPosCond;
        //internal ImGuiSetCond SetNextWindowSizeCond;
        //internal ImGuiSetCond SetNextWindowContentSizeCond;
        //internal ImGuiSetCond SetNextWindowCollapsedCond;
        //internal bool SetNextWindowFocus;
        //internal bool SetNextTreeNodeOpenedVal;
        //internal ImGuiSetCond SetNextTreeNodeOpenedCond;

        //// Render
        //internal ImDrawData RenderDrawData;                     // Main ImDrawData instance to pass render information to the user

        internal ImVector<ImDrawList>[] RenderDrawLists = new ImVector<ImDrawList>[3];
        //internal float ModalWindowDarkeningRatio;
        //internal ImDrawList OverlayDrawList;                    // Optional software render of mouse cursors, if io.MouseDrawCursor is set + a few debug overlays
        //internal ImGuiMouseCursor MouseCursor;
        //internal ImGuiMouseCursorData[] MouseCursorData = new ImGuiMouseCursorData[(int)ImGuiMouseCursor.ImGuiMouseCursor_Count_];

        //// Widget state
        //internal ImGuiTextEditState InputTextState;

        //internal ImFont InputTextPasswordFont;
        //internal uint ScalarAsInputTextId;                // Temporary text input when CTRL+clicking on a slider, etc.
        //internal ImGuiStorage ColorEditModeStorage;               // Store user selection of color edit mode
        internal ImVec2 ActiveClickDeltaToCenter;
        internal float DragCurrentValue;                   // Currently dragged value, always float, not rounded by end-user precision settings
        internal ImVec2 DragLastMouseDelta;
        internal float DragSpeedDefaultRatio;              // If speed == 0.0f, uses (max-min) * DragSpeedDefaultRatio
        internal float DragSpeedScaleSlow;
        internal float DragSpeedScaleFast;
        internal ImVec2 ScrollbarClickDeltaToGrabCenter;   // Distance between mouse and center of grab box, normalized in parent space. Use storage?
        internal string Tooltip;
        internal string PrivateClipboard;                   // If no custom clipboard handler is defined
        internal ImVec2 OsImePosRequest, OsImePosSet;       // Cursor position request & last passed to the OS Input Method Editor

        // Logging
        internal bool LogEnabled;

        internal StreamWriter LogFile;                            // If != NULL log to stdout/ file
        internal StringBuilder LogClipboard;                       // Else log to clipboard. This is pointer so our GImGui static constructor doesn't call heap allocators.
        internal int LogStartDepth;

        internal int LogAutoExpandMaxDepth;

        // Misc
        internal float[] FramerateSecPerFrame = new float[120];          // calculate estimate of framerate for user

        internal int FramerateSecPerFrameIdx;
        internal float FramerateSecPerFrameAccum;
        internal int CaptureMouseNextFrame;              // explicit capture via CaptureInputs() sets those flags
        internal int CaptureKeyboardNextFrame;
        internal char[] TempBuffer = new char[1024 * 3 + 1];               // temporary text buffer

        internal ImGuiState()
        {
            IO = new ImGuiIO();
            //Style = new ImGuiStyle();
            //Windows = new ImVector<ImGuiWindow>();
            //WindowsSortBuffer = new ImVector<ImGuiWindow>();
            //CurrentWindowStack = new ImVector<ImGuiWindow>();
            //Settings = new ImVector<ImGuiIniData>();
            //ColorModifiers = new ImVector<ImGuiColMod>();
            //StyleModifiers = new ImVector<ImGuiStyleMod>();
            //FontStack = new ImVector<ImFont>();
            //OpenedPopupStack = new ImVector<ImGuiPopupRef>();
            //CurrentPopupStack = new ImVector<ImGuiPopupRef>();
            //RenderDrawData = new ImDrawData();
            for (var i = 0; i < RenderDrawLists.Length; i++)
                RenderDrawLists[i] = new ImVector<ImDrawList>();
            //OverlayDrawList = new ImDrawList();
            //ColorEditModeStorage = new ImGuiStorage();
            //for (var i = 0; i < MouseCursorData.Length; i++)
            //    MouseCursorData[i] = new ImGuiMouseCursorData();
            //InputTextState = new ImGuiTextEditState();

            Initialized = false;
            //Font = null;
            FontSize = FontBaseSize = 0.0f;
            FontTexUvWhitePixel = new ImVec2(0.0f, 0.0f);

            Time = 0.0f;
            FrameCount = 0;
            FrameCountEnded = FrameCountRendered = -1;
            //CurrentWindow = null;
            //FocusedWindow = null;
            //HoveredWindow = null;
            //HoveredRootWindow = null;
            HoveredId = 0;
            //HoveredIdAllowOverlap = false;
            HoveredIdPreviousFrame = 0;
            ActiveId = 0;
            ActiveIdPreviousFrame = 0;
            ActiveIdIsAlive = false;
            ActiveIdIsJustActivated = false;
            //ActiveIdAllowOverlap = false;
            //ActiveIdWindow = null;
            //MovedWindow = null;
            //SettingsDirtyTimer = 0.0f;

            //SetNextWindowPosVal = new ImVec2(0.0f, 0.0f);
            //SetNextWindowSizeVal = new ImVec2(0.0f, 0.0f);
            //SetNextWindowCollapsedVal = false;
            //SetNextWindowPosCond = 0;
            //SetNextWindowSizeCond = 0;
            //SetNextWindowContentSizeCond = 0;
            //SetNextWindowCollapsedCond = 0;
            //SetNextWindowFocus = false;
            //SetNextTreeNodeOpenedVal = false;
            //SetNextTreeNodeOpenedCond = 0;

            //ScalarAsInputTextId = 0;
            ActiveClickDeltaToCenter = new ImVec2(0.0f, 0.0f);
            DragCurrentValue = 0.0f;
            DragLastMouseDelta = new ImVec2(0.0f, 0.0f);
            DragSpeedDefaultRatio = 0.01f;
            DragSpeedScaleSlow = 0.01f;
            DragSpeedScaleFast = 10.0f;
            ScrollbarClickDeltaToGrabCenter = new ImVec2(0.0f, 0.0f);
            //memset(Tooltip, 0, sizeof(Tooltip));
            PrivateClipboard = null;
            OsImePosRequest = OsImePosSet = new ImVec2(-1.0f, -1.0f);

            //ModalWindowDarkeningRatio = 0.0f;
            //OverlayDrawList._OwnerName = "##Overlay"; // Give it a name for debugging
            //MouseCursor = ImGuiMouseCursor.ImGuiMouseCursor_Arrow;
            //memset(MouseCursorData, 0, sizeof(MouseCursorData));

            LogEnabled = false;
            //LogFile = null;
            //TODO: LogClipboard = null;
            LogStartDepth = 0;
            LogAutoExpandMaxDepth = 2;

            //memset(FramerateSecPerFrame, 0, sizeof(FramerateSecPerFrame));
            FramerateSecPerFrameIdx = 0;
            FramerateSecPerFrameAccum = 0.0f;
            CaptureMouseNextFrame = CaptureKeyboardNextFrame = -1;
            //memset(TempBuffer, 0, sizeof(TempBuffer));
        }

        //private void SetCurrentFont(ImFont font)
        //{
        //    ImGuiState g = this;

        //    System.Diagnostics.Debug.Assert(font != null && font.IsLoaded());    // Font Atlas not created. Did you call io.Fonts->GetTexDataAsRGBA32 / GetTexDataAsAlpha8 ?
        //    System.Diagnostics.Debug.Assert(font.Scale > 0.0f);
        //    g.Font = font;
        //    g.FontBaseSize = g.IO.FontGlobalScale * g.Font.FontSize * g.Font.Scale;
        //    g.FontSize = g.CurrentWindow != null ? g.CurrentWindow.CalcFontSize() : 0.0f;
        //    g.FontTexUvWhitePixel = g.Font.ContainerAtlas.TexUvWhitePixel;
        //}

    }
}