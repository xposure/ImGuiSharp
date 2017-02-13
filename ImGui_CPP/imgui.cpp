// dear imgui, v1.48 WIP
// (main code and documentation)

// See ImGui::ShowTestWindow() in imgui_demo.cpp for demo code.
// Newcomers, read 'Programmer guide' below for notes on how to setup ImGui in your codebase.
// Get latest version at https://github.com/ocornut/imgui
// Releases change-log at https://github.com/ocornut/imgui/releases
// Developed by Omar Cornut and every direct or indirect contributors to the GitHub.
// This library is free but I need your support to sustain development and maintenance.
// If you work for a company, please consider financial support, e.g: https://www.patreon.com/imgui

/*

 Index
 - MISSION STATEMENT
 - END-USER GUIDE
 - PROGRAMMER GUIDE (read me!)
 - API BREAKING CHANGES (read me when you update!)
 - FREQUENTLY ASKED QUESTIONS (FAQ), TIPS
   - How can I help?
   - How do I update to a newer version of ImGui?
   - Can I have multiple widgets with the same label? Can I have widget without a label? (Yes) / A primer on the use of labels/IDs in ImGui.
   - I integrated ImGui in my engine and the text or lines are blurry..
   - I integrated ImGui in my engine and some elements are disappearing when I move windows around..
   - How can I load a different font than the default?
   - How can I load multiple fonts?
   - How can I display and input non-latin characters such as Chinese, Japanese, Korean, Cyrillic?
 - ISSUES & TODO-LIST
 - CODE


 MISSION STATEMENT
 =================

 - easy to use to create code-driven and data-driven tools
 - easy to use to create ad hoc short-lived tools and long-lived, more elaborate tools
 - easy to hack and improve
 - minimize screen real-estate usage
 - minimize setup and maintenance
 - minimize state storage on user side
 - portable, minimize dependencies, run on target (consoles, phones, etc.)
 - efficient runtime (NB- we do allocate when "growing" content - creating a window / opening a tree node for the first time, etc. - but a typical frame won't allocate anything)
 - read about immediate-mode gui principles @ http://mollyrocket.com/861, http://mollyrocket.com/forums/index.html

 Designed for developers and content-creators, not the typical end-user! Some of the weaknesses includes:
 - doesn't look fancy, doesn't animate
 - limited layout features, intricate layouts are typically crafted in code
 - occasionally uses statically sized buffers for string manipulations - won't crash, but some very long pieces of text may be clipped. functions like ImGui::TextUnformatted() don't have such restriction.


 END-USER GUIDE
 ==============

 - double-click title bar to collapse window
 - click upper right corner to close a window, available when 'bool* p_opened' is passed to ImGui::Begin()
 - click and drag on lower right corner to resize window
 - click and drag on any empty space to move window
 - double-click/double-tap on lower right corner grip to auto-fit to content
 - TAB/SHIFT+TAB to cycle through keyboard editable fields
 - use mouse wheel to scroll
 - use CTRL+mouse wheel to zoom window contents (if IO.FontAllowScaling is true)
 - CTRL+Click on a slider or drag box to input value as text
 - text editor:
   - Hold SHIFT or use mouse to select text.
   - CTRL+Left/Right to word jump
   - CTRL+Shift+Left/Right to select words
   - CTRL+A our Double-Click to select all
   - CTRL+X,CTRL+C,CTRL+V to use OS clipboard
   - CTRL+Z,CTRL+Y to undo/redo
   - ESCAPE to revert text to its original value
   - You can apply arithmetic operators +,*,/ on numerical values. Use +- to subtract (because - would set a negative value!)


 PROGRAMMER GUIDE
 ================

 - read the FAQ below this section!
 - your code creates the UI, if your code doesn't run the UI is gone! == very dynamic UI, no construction/destructions steps, less data retention on your side, no state duplication, less sync, less bugs.
 - call and read ImGui::ShowTestWindow() for demo code demonstrating most features.
 - see examples/ folder for standalone sample applications. Prefer reading examples/opengl_example/ first at it is the simplest.
 - customization: PushStyleColor()/PushStyleVar() or the style editor to tweak the look of the interface (e.g. if you want a more compact UI or a different color scheme).

 - getting started:
   - init: call ImGui::GetIO() to retrieve the ImGuiIO structure and fill the fields marked 'Settings'.
   - init: call io.Fonts->GetTexDataAsRGBA32(...) and load the font texture pixels into graphics memory.
   - every frame:
      1/ in your mainloop or right after you got your keyboard/mouse info, call ImGui::GetIO() and fill the fields marked 'Input'
      2/ call ImGui::NewFrame().
      3/ use any ImGui function you want between NewFrame() and Render()
      4/ call ImGui::Render() to render all the accumulated command-lists. it will call your RenderDrawListFn handler that you set in the IO structure.
   - all rendering information are stored into command-lists until ImGui::Render() is called.
   - ImGui never touches or know about your GPU state. the only function that knows about GPU is the RenderDrawListFn handler that you must provide.
   - effectively it means you can create widgets at any time in your code, regardless of considerations of being in "update" vs "render" phases.
   - refer to the examples applications in the examples/ folder for instruction on how to setup your code.
   - a typical application skeleton may be:

        // Application init
        ImGuiIO& io = ImGui::GetIO();
        io.DisplaySize.x = 1920.0f;
        io.DisplaySize.y = 1280.0f;
        io.IniFilename = "imgui.ini";
        io.RenderDrawListsFn = my_render_function;  // Setup a render function, or set to NULL and call GetDrawData() after Render() to access the render data.
        // TODO: Fill others settings of the io structure

        // Load texture atlas
        // There is a default font so you don't need to care about choosing a font yet
        unsigned char* pixels;
        int width, height;
        io.Fonts->GetTexDataAsRGBA32(pixels, &width, &height);
        // TODO: At this points you've got a texture pointed to by 'pixels' and you need to upload that your your graphic system 
        // TODO: Store your texture pointer/identifier (whatever your engine uses) in 'io.Fonts->TexID'

        // Application main loop
        while (true)
        {
            // 1) get low-level inputs (e.g. on Win32, GetKeyboardState(), or poll your events, etc.)
            // TODO: fill all fields of IO structure and call NewFrame
            ImGuiIO& io = ImGui::GetIO();
            io.DeltaTime = 1.0f/60.0f;
            io.MousePos = mouse_pos;
            io.MouseDown[0] = mouse_button_0;
            io.MouseDown[1] = mouse_button_1;
            io.KeysDown[i] = ...

            // 2) call NewFrame(), after this point you can use ImGui::* functions anytime
            ImGui::NewFrame();

            // 3) most of your application code here
            ImGui::Begin("My window");
            ImGui::Text("Hello, world.");
            ImGui::End();
            MyGameUpdate(); // may use ImGui functions
            MyGameRender(); // may use ImGui functions

            // 4) render & swap video buffers
            ImGui::Render();
            // swap video buffer, etc.
        }

   - after calling ImGui::NewFrame() you can read back flags from the IO structure to tell how ImGui intends to use your inputs.
     When 'io.WantCaptureMouse' or 'io.WantCaptureKeyboard' flags are set you may want to discard/hide the inputs from the rest of your application.
     When 'io.WantInputsCharacters' is set to may want to notify your OS to popup an on-screen keyboard, if available.


 API BREAKING CHANGES
 ====================

 Occasionally introducing changes that are breaking the API. The breakage are generally minor and easy to fix.
 Here is a change-log of API breaking changes, if you are using one of the functions listed, expect to have to fix some code.
 Also read releases logs https://github.com/ocornut/imgui/releases for more details.

 - 2016/03/02 (1.48) - InputText() completion/history/always callbacks: if you modify the text buffer manually (without using DeleteChars()/InsertChars() helper) you need to maintain the BufTextLen field. added an assert.
 - 2016/01/23 (1.48) - fixed not honoring exact width passed to PushItemWidth(), previously it would add extra FramePadding.x*2 over that width. if you had manual pixel-perfect alignment in place it might affect you.
 - 2015/12/27 (1.48) - fixed ImDrawList::AddRect() which used to render a rectangle 1 px too large on each axis.
 - 2015/12/04 (1.47) - renamed Color() helpers to ValueColor() - dangerously named, rarely used and probably to be made obsolete.
 - 2015/08/29 (1.45) - with the addition of horizontal scrollbar we made various fixes to inconsistencies with dealing with cursor position.
                       GetCursorPos()/SetCursorPos() functions now include the scrolled amount. It shouldn't affect the majority of users, but take note that SetCursorPosX(100.0f) puts you at +100 from the starting x position which may include scrolling, not at +100 from the window left side.
                       GetContentRegionMax()/GetWindowContentRegionMin()/GetWindowContentRegionMax() functions allow include the scrolled amount. Typically those were used in cases where no scrolling would happen so it may not be a problem, but watch out!
 - 2015/08/29 (1.45) - renamed style.ScrollbarWidth to style.ScrollbarSize
 - 2015/08/05 (1.44) - split imgui.cpp into extra files: imgui_demo.cpp imgui_draw.cpp imgui_internal.h that you need to add to your project.
 - 2015/07/18 (1.44) - fixed angles in ImDrawList::PathArcTo(), PathArcToFast() (introduced in 1.43) being off by an extra PI for no justifiable reason
 - 2015/07/14 (1.43) - add new ImFontAtlas::AddFont() API. For the old AddFont***, moved the 'font_no' parameter of ImFontAtlas::AddFont** functions to the ImFontConfig structure.
                       you need to render your textured triangles with bilinear filtering to benefit from sub-pixel positioning of text.
 - 2015/07/08 (1.43) - switched rendering data to use indexed rendering. this is saving a fair amount of CPU/GPU and enables us to get anti-aliasing for a marginal cost.
                       this necessary change will break your rendering function! the fix should be very easy. sorry for that :(
                     - if you are using a vanilla copy of one of the imgui_impl_XXXX.cpp provided in the example, you just need to update your copy and you can ignore the rest.
                     - the signature of the io.RenderDrawListsFn handler has changed!
                            ImGui_XXXX_RenderDrawLists(ImDrawList** const cmd_lists, int cmd_lists_count)
                       became:
                            ImGui_XXXX_RenderDrawLists(ImDrawData* draw_data).
                              argument   'cmd_lists'        -> 'draw_data->CmdLists'
                              argument   'cmd_lists_count'  -> 'draw_data->CmdListsCount'
                              ImDrawList 'commands'         -> 'CmdBuffer'
                              ImDrawList 'vtx_buffer'       -> 'VtxBuffer'
                              ImDrawList  n/a               -> 'IdxBuffer' (new)
                              ImDrawCmd  'vtx_count'        -> 'ElemCount'
                              ImDrawCmd  'clip_rect'        -> 'ClipRect'
                              ImDrawCmd  'user_callback'    -> 'UserCallback'
                              ImDrawCmd  'texture_id'       -> 'TextureId'
                     - each ImDrawList now contains both a vertex buffer and an index buffer. For each command, render ElemCount/3 triangles using indices from the index buffer.
                     - if you REALLY cannot render indexed primitives, you can call the draw_data->DeIndexAllBuffers() method to de-index the buffers. This is slow and a waste of CPU/GPU. Prefer using indexed rendering!
                     - refer to code in the examples/ folder or ask on the GitHub if you are unsure of how to upgrade. please upgrade!
 - 2015/07/10 (1.43) - changed SameLine() parameters from int to float.
 - 2015/07/02 (1.42) - renamed SetScrollPosHere() to SetScrollFromCursorPos(). Kept inline redirection function (will obsolete).
 - 2015/07/02 (1.42) - renamed GetScrollPosY() to GetScrollY(). Necessary to reduce confusion along with other scrolling functions, because positions (e.g. cursor position) are not equivalent to scrolling amount.
 - 2015/06/14 (1.41) - changed ImageButton() default bg_col parameter from (0,0,0,1) (black) to (0,0,0,0) (transparent) - makes a difference when texture have transparence
 - 2015/06/14 (1.41) - changed Selectable() API from (label, selected, size) to (label, selected, flags, size). Size override should have been rarely be used. Sorry!
 - 2015/05/31 (1.40) - renamed GetWindowCollapsed() to IsWindowCollapsed() for consistency. Kept inline redirection function (will obsolete).
 - 2015/05/31 (1.40) - renamed IsRectClipped() to IsRectVisible() for consistency. Note that return value is opposite! Kept inline redirection function (will obsolete).
 - 2015/05/27 (1.40) - removed the third 'repeat_if_held' parameter from Button() - sorry! it was rarely used and inconsistent. Use PushButtonRepeat(true) / PopButtonRepeat() to enable repeat on desired buttons.
 - 2015/05/11 (1.40) - changed BeginPopup() API, takes a string identifier instead of a bool. ImGui needs to manage the open/closed state of popups. Call OpenPopup() to actually set the "opened" state of a popup. BeginPopup() returns true if the popup is opened.
 - 2015/05/03 (1.40) - removed style.AutoFitPadding, using style.WindowPadding makes more sense (the default values were already the same).
 - 2015/04/13 (1.38) - renamed IsClipped() to IsRectClipped(). Kept inline redirection function (will obsolete).
 - 2015/04/09 (1.38) - renamed ImDrawList::AddArc() to ImDrawList::AddArcFast() for compatibility with future API
 - 2015/04/03 (1.38) - removed ImGuiCol_CheckHovered, ImGuiCol_CheckActive, replaced with the more general ImGuiCol_FrameBgHovered, ImGuiCol_FrameBgActive.
 - 2014/04/03 (1.38) - removed support for passing -FLT_MAX..+FLT_MAX as the range for a SliderFloat(). Use DragFloat() or Inputfloat() instead.
 - 2015/03/17 (1.36) - renamed GetItemBoxMin()/GetItemBoxMax()/IsMouseHoveringBox() to GetItemRectMin()/GetItemRectMax()/IsMouseHoveringRect(). Kept inline redirection function (will obsolete).
 - 2015/03/15 (1.36) - renamed style.TreeNodeSpacing to style.IndentSpacing, ImGuiStyleVar_TreeNodeSpacing to ImGuiStyleVar_IndentSpacing
 - 2015/03/13 (1.36) - renamed GetWindowIsFocused() to IsWindowFocused(). Kept inline redirection function (will obsolete).
 - 2015/03/08 (1.35) - renamed style.ScrollBarWidth to style.ScrollbarWidth (casing)
 - 2015/02/27 (1.34) - renamed OpenNextNode(bool) to SetNextTreeNodeOpened(bool, ImGuiSetCond). Kept inline redirection function (will obsolete).
 - 2015/02/27 (1.34) - renamed ImGuiSetCondition_*** to ImGuiSetCond_***, and _FirstUseThisSession becomes _Once.
 - 2015/02/11 (1.32) - changed text input callback ImGuiTextEditCallback return type from void-->int. reserved for future use, return 0 for now.
 - 2015/02/10 (1.32) - renamed GetItemWidth() to CalcItemWidth() to clarify its evolving behavior
 - 2015/02/08 (1.31) - renamed GetTextLineSpacing() to GetTextLineHeightWithSpacing()
 - 2015/02/01 (1.31) - removed IO.MemReallocFn (unused)
 - 2015/01/19 (1.30) - renamed ImGuiStorage::GetIntPtr()/GetFloatPtr() to GetIntRef()/GetIntRef() because Ptr was conflicting with actual pointer storage functions.
 - 2015/01/11 (1.30) - big font/image API change! now loads TTF file. allow for multiple fonts. no need for a PNG loader.
              (1.30) - removed GetDefaultFontData(). uses io.Fonts->GetTextureData*() API to retrieve uncompressed pixels.
                       this sequence:
                           const void* png_data;
                           unsigned int png_size;
                           ImGui::GetDefaultFontData(NULL, NULL, &png_data, &png_size);
                           // <Copy to GPU>
                       became:
                           unsigned char* pixels;
                           int width, height;
                           io.Fonts->GetTexDataAsRGBA32(&pixels, &width, &height);
                           // <Copy to GPU>
                           io.Fonts->TexID = (your_texture_identifier);
                       you now have much more flexibility to load multiple TTF fonts and manage the texture buffer for internal needs.
                       it is now recommended that you sample the font texture with bilinear interpolation.
              (1.30) - added texture identifier in ImDrawCmd passed to your render function (we can now render images). make sure to set io.Fonts->TexID.
              (1.30) - removed IO.PixelCenterOffset (unnecessary, can be handled in user projection matrix)
              (1.30) - removed ImGui::IsItemFocused() in favor of ImGui::IsItemActive() which handles all widgets
 - 2014/12/10 (1.18) - removed SetNewWindowDefaultPos() in favor of new generic API SetNextWindowPos(pos, ImGuiSetCondition_FirstUseEver)
 - 2014/11/28 (1.17) - moved IO.Font*** options to inside the IO.Font-> structure (FontYOffset, FontTexUvForWhite, FontBaseScale, FontFallbackGlyph)
 - 2014/11/26 (1.17) - reworked syntax of IMGUI_ONCE_UPON_A_FRAME helper macro to increase compiler compatibility
 - 2014/11/07 (1.15) - renamed IsHovered() to IsItemHovered()
 - 2014/10/02 (1.14) - renamed IMGUI_INCLUDE_IMGUI_USER_CPP to IMGUI_INCLUDE_IMGUI_USER_INL and imgui_user.cpp to imgui_user.inl (more IDE friendly)
 - 2014/09/25 (1.13) - removed 'text_end' parameter from IO.SetClipboardTextFn (the string is now always zero-terminated for simplicity)
 - 2014/09/24 (1.12) - renamed SetFontScale() to SetWindowFontScale()
 - 2014/09/24 (1.12) - moved IM_MALLOC/IM_REALLOC/IM_FREE preprocessor defines to IO.MemAllocFn/IO.MemReallocFn/IO.MemFreeFn
 - 2014/08/30 (1.09) - removed IO.FontHeight (now computed automatically)
 - 2014/08/30 (1.09) - moved IMGUI_FONT_TEX_UV_FOR_WHITE preprocessor define to IO.FontTexUvForWhite
 - 2014/08/28 (1.09) - changed the behavior of IO.PixelCenterOffset following various rendering fixes


 FREQUENTLY ASKED QUESTIONS (FAQ), TIPS
 ======================================

 Q: How can I help?
 A: - If you are experienced enough with ImGui and with C/C++, look at the todo list and see how you want/can help!
    - Become a Patron/donate. Convince your company to become a Patron or provide serious funding for development time.

 Q: How do I update to a newer version of ImGui?
 A: Overwrite the following files:
      imgui.cpp
      imgui.h
      imgui_demo.cpp
      imgui_draw.cpp
      imgui_internal.h
      stb_rect_pack.h
      stb_textedit.h
      stb_truetype.h
    Don't overwrite imconfig.h if you have made modification to your copy.
    Check the "API BREAKING CHANGES" sections for a list of occasional API breaking changes. If you have a problem with a function, search for its name
    in the code, there will likely be a comment about it. Please report any issue to the GitHub page!

 Q: Can I have multiple widgets with the same label? Can I have widget without a label? (Yes)
 A: Yes. A primer on the use of labels/IDs in ImGui..

   - Elements that are not clickable, such as Text() items don't need an ID.

   - Interactive widgets require state to be carried over multiple frames (most typically ImGui often needs to remember what is the "active" widget).
     to do so they need an unique ID. unique ID are typically derived from a string label, an integer index or a pointer.

       Button("OK");        // Label = "OK",     ID = hash of "OK"
       Button("Cancel");    // Label = "Cancel", ID = hash of "Cancel"

   - ID are uniquely scoped within windows, tree nodes, etc. so no conflict can happen if you have two buttons called "OK" in two different windows
     or in two different locations of a tree.

   - If you have a same ID twice in the same location, you'll have a conflict:

       Button("OK");
       Button("OK");           // ID collision! Both buttons will be treated as the same.

     Fear not! this is easy to solve and there are many ways to solve it!

   - When passing a label you can optionally specify extra unique ID information within string itself. This helps solving the simpler collision cases.
     use "##" to pass a complement to the ID that won't be visible to the end-user:

       Button("Play");         // Label = "Play",   ID = hash of "Play"
       Button("Play##foo1");   // Label = "Play",   ID = hash of "Play##foo1" (different from above)
       Button("Play##foo2");   // Label = "Play",   ID = hash of "Play##foo2" (different from above)

   - If you want to completely hide the label, but still need an ID:

       Checkbox("##On", &b);   // Label = "",       ID = hash of "##On" (no label!)

   - Occasionally/rarely you might want change a label while preserving a constant ID. This allows you to animate labels.
     For example you may want to include varying information in a window title bar (and windows are uniquely identified by their ID.. obviously)
     Use "###" to pass a label that isn't part of ID:

       Button("Hello###ID";   // Label = "Hello",  ID = hash of "ID"
       Button("World###ID";   // Label = "World",  ID = hash of "ID" (same as above)
       
       sprintf(buf, "My game (%f FPS)###MyGame");
       Begin(buf);            // Variable label,   ID = hash of "MyGame"

   - Use PushID() / PopID() to create scopes and avoid ID conflicts within the same Window.
     This is the most convenient way of distinguishing ID if you are iterating and creating many UI elements.
     You can push a pointer, a string or an integer value. Remember that ID are formed from the concatenation of everything in the ID stack!

       for (int i = 0; i < 100; i++)
       {
         PushID(i);
         Button("Click");   // Label = "Click",  ID = hash of integer + "label" (unique)
         PopID();
       }

       for (int i = 0; i < 100; i++)
       {
         MyObject* obj = Objects[i];
         PushID(obj);
         Button("Click");   // Label = "Click",  ID = hash of pointer + "label" (unique)
         PopID();
       }

       for (int i = 0; i < 100; i++)
       {
         MyObject* obj = Objects[i];
         PushID(obj->Name);
         Button("Click");   // Label = "Click",  ID = hash of string + "label" (unique)
         PopID();
       }

   - More example showing that you can stack multiple prefixes into the ID stack:

       Button("Click");     // Label = "Click",  ID = hash of "Click"
       PushID("node");
       Button("Click");     // Label = "Click",  ID = hash of "node" + "Click"
         PushID(my_ptr);
           Button("Click"); // Label = "Click",  ID = hash of "node" + ptr + "Click"
         PopID();
       PopID();

   - Tree nodes implicitly creates a scope for you by calling PushID().

       Button("Click");     // Label = "Click",  ID = hash of "Click"
       if (TreeNode("node"))
       {
         Button("Click");   // Label = "Click",  ID = hash of "node" + "Click"
         TreePop();
       }

   - When working with trees, ID are used to preserve the opened/closed state of each tree node.
     Depending on your use cases you may want to use strings, indices or pointers as ID.
      e.g. when displaying a single object that may change over time (1-1 relationship), using a static string as ID will preserve your node open/closed state when the targeted object change.
      e.g. when displaying a list of objects, using indices or pointers as ID will preserve the node open/closed state differently. experiment and see what makes more sense!

 Q: I integrated ImGui in my engine and the text or lines are blurry..
 A: In your Render function, try translating your projection matrix by (0.5f,0.5f) or (0.375f,0.375f).
    Also make sure your orthographic projection matrix and io.DisplaySize matches your actual framebuffer dimension.

 Q. I integrated ImGui in my engine and some elements are disappearing when I move windows around..
    Most likely you are mishandling the clipping rectangles in your render function. Rectangles provided by ImGui are defined as (x1,y1,x2,y2) and NOT as (x1,y1,width,height).

 Q: How can I load a different font than the default? (default is an embedded version of ProggyClean.ttf, rendered at size 13)
 A: Use the font atlas to load the TTF file you want:

      ImGuiIO& io = ImGui::GetIO();
      io.Fonts->AddFontFromFileTTF("myfontfile.ttf", size_in_pixels);
      io.Fonts->GetTexDataAsRGBA32() or GetTexDataAsAlpha8()

 Q: How can I load multiple fonts?
 A: Use the font atlas to pack them into a single texture:
    (Read extra_fonts/README.txt and the code in ImFontAtlas for more details.)

      ImGuiIO& io = ImGui::GetIO();
      ImFont* font0 = io.Fonts->AddFontDefault();
      ImFont* font1 = io.Fonts->AddFontFromFileTTF("myfontfile.ttf", size_in_pixels);
      ImFont* font2 = io.Fonts->AddFontFromFileTTF("myfontfile2.ttf", size_in_pixels);
      io.Fonts->GetTexDataAsRGBA32() or GetTexDataAsAlpha8()
      // the first loaded font gets used by default
      // use ImGui::PushFont()/ImGui::PopFont() to change the font at runtime

      // Options
      ImFontConfig config;
      config.OversampleH = 3;
      config.OversampleV = 1;
      config.GlyphExtraSpacing.x = 1.0f;
      io.Fonts->LoadFromFileTTF("myfontfile.ttf", size_pixels, &config);

      // Combine multiple fonts into one
      ImWchar ranges[] = { 0xf000, 0xf3ff, 0 };
      ImFontConfig config;
      config.MergeMode = true;
      io.Fonts->AddFontDefault();
      io.Fonts->LoadFromFileTTF("fontawesome-webfont.ttf", 16.0f, &config, ranges);
      io.Fonts->LoadFromFileTTF("myfontfile.ttf", size_pixels, NULL, &config, io.Fonts->GetGlyphRangesJapanese());

 Q: How can I display and input non-Latin characters such as Chinese, Japanese, Korean, Cyrillic?
 A: When loading a font, pass custom Unicode ranges to specify the glyphs to load. ImGui will support UTF-8 encoding across the board.
    Character input depends on you passing the right character code to io.AddInputCharacter(). The example applications do that.

      io.Fonts->AddFontFromFileTTF("myfontfile.ttf", size_in_pixels, NULL, io.Fonts->GetGlyphRangesJapanese());  // Load Japanese characters
      io.Fonts->GetTexDataAsRGBA32() or GetTexDataAsAlpha8()
      io.ImeWindowHandle = MY_HWND;      // To input using Microsoft IME, give ImGui the hwnd of your application

 - tip: the construct 'IMGUI_ONCE_UPON_A_FRAME { ... }' will run the block of code only once a frame. You can use it to quickly add custom UI in the middle of a deep nested inner loop in your code.
 - tip: you can create widgets without a Begin()/End() block, they will go in an implicit window called "Debug"
 - tip: you can call Begin() multiple times with the same name during the same frame, it will keep appending to the same window. this is also useful to set yourself in the context of another window (to get/set other settings)
 - tip: you can call Render() multiple times (e.g for VR renders).
 - tip: call and read the ShowTestWindow() code in imgui_demo.cpp for more example of how to use ImGui!


 ISSUES & TODO-LIST
 ==================
 Issue numbers (#) refer to github issues listed at https://github.com/ocornut/imgui/issues
 The list below consist mostly of notes of things to do before they are requested/discussed by users (at that point it usually happens on the github)

 - doc: add a proper documentation+regression testing system (#435)
 - window: maximum window size settings (per-axis). for large popups in particular user may not want the popup to fill all space.
 - window: add a way for very transient windows (non-saved, temporary overlay over hundreds of objects) to "clean" up from the global window list. perhaps a lightweight explicit cleanup pass.
 - window: calling SetNextWindowSize() every frame with <= 0 doesn't do anything, may be useful to allow (particularly when used for a single axis).
 - window: auto-fit feedback loop when user relies on any dynamic layout (window width multiplier, column) appears weird to end-user. clarify.
 - window: allow resizing of child windows (possibly given min/max for each axis?)
 - window: background options for child windows, border option (disable rounding)
 - window: add a way to clear an existing window instead of appending (e.g. for tooltip override using a consistent api rather than the deferred tooltip)
 - window: resizing from any sides? + mouse cursor directives for app.
!- window: begin with *p_opened == false should return false.
 - window: get size/pos helpers given names (see discussion in #249)
 - window: a collapsed window can be stuck behind the main menu bar?
 - window: detect extra End() call that pop the "Debug" window out and assert at call site instead of later.
 - window: consider renaming "GetWindowFont" which conflict with old Windows #define (#340)
 - window/tooltip: allow to set the width of a tooltip to allow TextWrapped() etc. while keeping the height automatic.
 - window: increase minimum size of a window with menus or fix the menu rendering so that it doesn't look odd.
 - draw-list: maintaining bounding box per command would allow to merge draw command when clipping isn't relied on (typical non-scrolling window or non-overflowing column would merge with previous command).
!- scrolling: allow immediately effective change of scroll if we haven't appended items yet
 - splitter/separator: formalize the splitter idiom into an official api (we want to handle n-way split) (#319)
 - widgets: display mode: widget-label, label-widget (aligned on column or using fixed size), label-newline-tab-widget etc.
 - widgets: clean up widgets internal toward exposing everything.
 - widgets: add disabled and read-only modes (#211)
 - main: considering adding EndFrame()/Init(). some constructs are awkward in the implementation because of the lack of them.
 - main: make it so that a frame with no window registered won't refocus every window on subsequent frames (~bump LastFrameActive of all windows). 
 - main: IsItemHovered() make it more consistent for various type of widgets, widgets with multiple components, etc. also effectively IsHovered() region sometimes differs from hot region, e.g tree nodes
 - main: IsItemHovered() info stored in a stack? so that 'if TreeNode() { Text; TreePop; } if IsHovered' return the hover state of the TreeNode?
 - input text: add ImGuiInputTextFlags_EnterToApply? (off #218)
 - input text: reorganize event handling, allow CharFilter to modify buffers, allow multiple events? (#541)
 - input text multi-line: don't directly call AddText() which does an unnecessary vertex reserve for character count prior to clipping. and/or more line-based clipping to AddText(). and/or reorganize TextUnformatted/RenderText for more efficiency for large text (e.g TextUnformatted could clip and log separately, etc).
 - input text multi-line: way to dynamically grow the buffer without forcing the user to initially allocate for worse case (follow up on #200)
 - input text multi-line: line numbers? status bar? (follow up on #200)
 - input number: optional range min/max for Input*() functions
 - input number: holding [-]/[+] buttons could increase the step speed non-linearly (or user-controlled)
 - input number: use mouse wheel to step up/down
 - input number: applying arithmetics ops (+,-,*,/) messes up with text edit undo stack.
 - button: provide a button that looks framed.
 - text: proper alignment options
 - image/image button: misalignment on padded/bordered button?
 - image/image button: parameters are confusing, image() has tint_col,border_col whereas imagebutton() has bg_col/tint_col. Even thou they are different parameters ordering could be more consistent. can we fix that?
 - layout: horizontal layout helper (#97)
 - layout: horizontal flow until no space left (#404)
 - layout: more generic alignment state (left/right/centered) for single items?
 - layout: clean up the InputFloatN/SliderFloatN/ColorEdit4 layout code. item width should include frame padding.
 - layout: BeginGroup() needs a border option.
 - columns: declare column set (each column: fixed size, %, fill, distribute default size among fills) (#513, #125)
 - columns: add a conditional parameter to SetColumnOffset() (#513, #125)
 - columns: separator function or parameter that works within the column (currently Separator() bypass all columns) (#125)
 - columns: columns header to act as button (~sort op) and allow resize/reorder (#513, #125)
 - columns: user specify columns size (#513, #125)
 - columns: flag to add horizontal separator above/below?
 - columns/layout: setup minimum line height (equivalent of automatically calling AlignFirstTextHeightToWidgets)
 - combo: sparse combo boxes (via function call?) / iterators
 - combo: contents should extends to fit label if combo widget is small
 - combo/listbox: keyboard control. need InputText-like non-active focus + key handling. considering keyboard for custom listbox (pr #203)
 - listbox: multiple selection
 - listbox: user may want to initial scroll to focus on the one selected value?
 - listbox: keyboard navigation.
 - listbox: scrolling should track modified selection.
!- popups/menus: clarify usage of popups id, how MenuItem/Selectable closing parent popups affects the ID, etc. this is quite fishy needs improvement! (#331, #402)
 - popups: add variant using global identifier similar to Begin/End (#402)
 - popups: border options. richer api like BeginChild() perhaps? (#197)
 - tooltip: tooltip that doesn't fit in entire screen seems to lose their "last prefered button" and may teleport when moving mouse
 - menus: local shortcuts, global shortcuts (#456, #126)
 - menus: icons
 - menus: menubars: some sort of priority / effect of main menu-bar on desktop size?
 - menus: calling BeginMenu() twice with a same name doesn't seem to append nicely
 - statusbar: add a per-window status bar helper similar to what menubar does.
 - tabs (#261, #351)
 - separator: separator on the initial position of a window is not visible (cursorpos.y <= clippos.y)
 - color: the color helpers/typing is a mess and needs sorting out.
 - color: add a better color picker (#346)
 - node/graph editor (#306)
 - pie menus patterns (#434)
 - plot: PlotLines() should use the polygon-stroke facilities (currently issues with averaging normals)
 - plot: make it easier for user to draw extra stuff into the graph (e.g: draw basis, highlight certain points, 2d plots, multiple plots)
 - plot: "smooth" automatic scale over time, user give an input 0.0(full user scale) 1.0(full derived from value)
 - plot: add a helper e.g. Plot(char* label, float value, float time_span=2.0f) that stores values and Plot them for you - probably another function name. and/or automatically allow to plot ANY displayed value (more reliance on stable ID)
 - slider: allow using the [-]/[+] buttons used by InputFloat()/InputInt()
 - slider: initial absolute click is imprecise. change to relative movement slider (same as scrollbar).
 - slider: add dragging-based widgets to edit values with mouse (on 2 axises), saving screen real-estate.
 - slider: tint background based on value (e.g. v_min -> v_max, or use 0.0f either side of the sign)
 - slider & drag: int data passing through a float
 - drag float: up/down axis
 - drag float: added leeway on edge (e.g. a few invisible steps past the clamp limits)
 - text edit: clean up the mess caused by converting UTF-8 <> wchar. the code is rather inefficient right now.
 - text edit: centered text for slider as input text so it matches typical positioning.
 - text edit: flag to disable live update of the user buffer.
 - text edit: field resize behavior - field could stretch when being edited? hover tooltip shows more text?
 - tree node / optimization: avoid formatting when clipped.
 - tree node: clarify spacing, perhaps provide API to query exact spacing. provide API to draw the primitive. same with Bullet().
 - tree node: tree-node/header right-most side doesn't take account of horizontal scrolling.
 - tree node: add treenode/treepush int variants? because (void*) cast from int warns on some platforms/settings
 - tree node / selectable render mismatch which is visible if you use them both next to each other (e.g. cf. property viewer)
 - textwrapped: figure out better way to use TextWrapped() in an always auto-resize context (tooltip, etc.) (git issue #249)
 - settings: write more decent code to allow saving/loading new fields
 - settings: api for per-tool simple persistent data (bool,int,float,columns sizes,etc.) in .ini file
 - style: add window shadows.
 - style/optimization: store rounded corners in texture to use 1 quad per corner (filled and wireframe) to lower the cost of rounding.
 - style: color-box not always square?
 - style: a concept of "compact style" that the end-user can easily rely on (e.g. PushStyleCompact()?) that maps to other settings? avoid implementing duplicate helpers such as SmallCheckbox(), etc.
 - style: try to make PushStyleVar() more robust to incorrect parameters (to be more friendly to edit & continues situation).
 - style: global scale setting.
 - text: simple markup language for color change?
 - font: dynamic font atlas to avoid baking huge ranges into bitmap and make scaling easier.
 - font: helper to add glyph redirect/replacements (e.g. redirect alternate apostrophe unicode code points to ascii one, etc.)
 - log: LogButtons() options for specifying depth and/or hiding depth slider
 - log: have more control over the log scope (e.g. stop logging when leaving current tree node scope)
 - log: be able to log anything (e.g. right-click on a window/tree-node, shows context menu? log into tty/file/clipboard)
 - log: let user copy any window content to clipboard easily (CTRL+C on windows? while moving it? context menu?). code is commented because it fails with multiple Begin/End pairs.
 - filters: set a current filter that tree node can automatically query to hide themselves
 - filters: handle wildcards (with implicit leading/trailing *), regexps
 - shortcuts: add a shortcut api, e.g. parse "&Save" and/or "Save (CTRL+S)", pass in to widgets or provide simple ways to use (button=activate, input=focus)
!- keyboard: tooltip & combo boxes are messing up / not honoring keyboard tabbing
 - keyboard: full keyboard navigation and focus. (#323)
 - focus: SetKeyboardFocusHere() on with >= 0 offset could be done on same frame (else latch and modulate on beginning of next frame)
 - input: rework IO system to be able to pass actual ordered/timestamped events.
 - input: allow to decide and pass explicit double-clicks (e.g. for windows by the CS_DBLCLKS style).
 - input: support track pad style scrolling & slider edit.
 - misc: provide a way to compile out the entire implementation while providing a dummy API (e.g. #define IMGUI_DUMMY_IMPL)
 - misc: double-clicking on title bar to minimize isn't consistent, perhaps move to single-click on left-most collapse icon?
 - misc: provide HoveredTime and ActivatedTime to ease the creation of animations.
 - style editor: have a more global HSV setter (e.g. alter hue on all elements). consider replacing active/hovered by offset in HSV space? (#438)
 - style editor: color child window height expressed in multiple of line height.
 - remote: make a system like RemoteImGui first-class citizen/project (#75)
 - drawlist: user probably can't call Clear() because we expect a texture to be pushed in the stack.
 - examples: directx9/directx11: save/restore device state more thoroughly.
 - optimization: use another hash function than crc32, e.g. FNV1a
 - optimization/render: merge command-lists with same clip-rect into one even if they aren't sequential? (as long as in-between clip rectangle don't overlap)?
 - optimization: turn some the various stack vectors into statically-sized arrays
 - optimization: better clipping for multi-component widgets
*/

#if defined(_MSC_VER) && !defined(_CRT_SECURE_NO_WARNINGS)
#define _CRT_SECURE_NO_WARNINGS
#endif

#include "imgui.h"
#define IMGUI_DEFINE_MATH_OPERATORS
#define IMGUI_DEFINE_PLACEMENT_NEW
#include "imgui_internal.h"

#include <ctype.h>      // toupper, isprint
#include <math.h>       // sqrtf, fabsf, fmodf, powf, cosf, sinf, floorf, ceilf
#include <stdlib.h>     // NULL, malloc, free, qsort, atoi
#include <stdio.h>      // vsnprintf, sscanf, printf
#if defined(_MSC_VER) && _MSC_VER <= 1500 // MSVC 2008 or earlier
#include <stddef.h>     // intptr_t
#else
#include <stdint.h>     // intptr_t
#endif

#ifdef _MSC_VER
#pragma warning (disable: 4127) // condition expression is constant
#pragma warning (disable: 4505) // unreferenced local function has been removed (stb stuff)
#pragma warning (disable: 4996) // 'This function or variable may be unsafe': strcpy, strdup, sprintf, vsnprintf, sscanf, fopen
#define snprintf _snprintf
#endif

// Clang warnings with -Weverything
#ifdef __clang__
#pragma clang diagnostic ignored "-Wold-style-cast"         // warning : use of old-style cast                              // yes, they are more terse.
#pragma clang diagnostic ignored "-Wfloat-equal"            // warning : comparing floating point with == or != is unsafe   // storing and comparing against same constants ok.
#pragma clang diagnostic ignored "-Wformat-nonliteral"      // warning : format string is not a string literal              // passing non-literal to vsnformat(). yes, user passing incorrect format strings can crash the code.
#pragma clang diagnostic ignored "-Wexit-time-destructors"  // warning : declaration requires an exit-time destructor       // exit-time destruction order is undefined. if MemFree() leads to users code that has been disabled before exit it might cause problems. ImGui coding style welcomes static/globals.
#pragma clang diagnostic ignored "-Wglobal-constructors"    // warning : declaration requires a global destructor           // similar to above, not sure what the exact difference it.
#pragma clang diagnostic ignored "-Wsign-conversion"        // warning : implicit conversion changes signedness             //
#pragma clang diagnostic ignored "-Wmissing-noreturn"       // warning : function xx could be declared with attribute 'noreturn' warning    // GetDefaultFontData() asserts which some implementation makes it never return.
#pragma clang diagnostic ignored "-Wdeprecated-declarations"// warning : 'xx' is deprecated: The POSIX name for this item.. // for strdup used in demo code (so user can copy & paste the code)
#pragma clang diagnostic ignored "-Wint-to-void-pointer-cast" // warning : cast to 'void *' from smaller integer type 'int'
#endif
#ifdef __GNUC__
#pragma GCC diagnostic ignored "-Wunused-function"          // warning: 'xxxx' defined but not used
#pragma GCC diagnostic ignored "-Wint-to-pointer-cast"      // warning: cast to pointer from integer of different size
#endif

//-------------------------------------------------------------------------
// Forward Declarations
//-------------------------------------------------------------------------

static void             LogRenderedText(const ImVec2& ref_pos, const char* text, const char* text_end = NULL);

static void             PushMultiItemsWidths(int components, float w_full = 0.0f);
static float            GetDraggedColumnOffset(int column_index);

static bool             IsKeyPressedMap(ImGuiKey key, bool repeat = true);

static void             SetCurrentFont(ImFont* font);
static void             SetCurrentWindow(ImGuiWindow* window);
static void             SetWindowScrollY(ImGuiWindow* window, float new_scroll_y);
static void             SetWindowPos(ImGuiWindow* window, const ImVec2& pos, ImGuiSetCond cond);
static void             SetWindowSize(ImGuiWindow* window, const ImVec2& size, ImGuiSetCond cond);
static void             SetWindowCollapsed(ImGuiWindow* window, bool collapsed, ImGuiSetCond cond);
static ImGuiWindow*     FindHoveredWindow(ImVec2 pos, bool excluding_childs);
static ImGuiWindow*     CreateNewWindow(const char* name, ImVec2 size, ImGuiWindowFlags flags);
static inline bool      IsWindowContentHoverable(ImGuiWindow* window);
static void             ClearSetNextWindowData();
static void             CheckStacksSize(ImGuiWindow* window, bool write);
static void             Scrollbar(ImGuiWindow* window, bool horizontal);
static bool             CloseWindowButton(bool* p_opened);

static void             AddDrawListToRenderList(ImVector<ImDrawList*>& out_render_list, ImDrawList* draw_list);
static void             AddWindowToRenderList(ImVector<ImDrawList*>& out_render_list, ImGuiWindow* window);
static void             AddWindowToSortedBuffer(ImVector<ImGuiWindow*>& out_sorted_windows, ImGuiWindow* window);

static ImGuiIniData*    FindWindowSettings(const char* name);
static ImGuiIniData*    AddWindowSettings(const char* name);
static void             LoadSettings();
static void             SaveSettings();
static void             MarkSettingsDirty();

static void             PushColumnClipRect(int column_index = -1);
static ImRect           GetVisibleRect();

static bool             BeginPopupEx(const char* str_id, ImGuiWindowFlags extra_flags);
static void             CloseInactivePopups();
static void             ClosePopupToLevel(int remaining);
static void             ClosePopup(ImGuiID id);
static bool             IsPopupOpen(ImGuiID id);
static ImGuiWindow*     GetFrontMostModalRootWindow();
static ImVec2           FindBestPopupWindowPos(const ImVec2& base_pos, const ImVec2& size, int* last_dir, const ImRect& rect_to_avoid);

static bool             InputTextFilterCharacter(unsigned int* p_char, ImGuiInputTextFlags flags, ImGuiTextEditCallback callback, void* user_data);
static int              InputTextCalcTextLenAndLineCount(const char* text_begin, const char** out_text_end);
static ImVec2           InputTextCalcTextSizeW(const ImWchar* text_begin, const ImWchar* text_end, const ImWchar** remaining = NULL, ImVec2* out_offset = NULL, bool stop_on_new_line = false);

static inline void      DataTypeFormatString(ImGuiDataType data_type, void* data_ptr, const char* display_format, char* buf, int buf_size);
static inline void      DataTypeFormatString(ImGuiDataType data_type, void* data_ptr, int decimal_precision, char* buf, int buf_size);
static void             DataTypeApplyOp(ImGuiDataType data_type, int op, void* value1, const void* value2);
static void             DataTypeApplyOpFromText(const char* buf, const char* initial_value_buf, ImGuiDataType data_type, void* data_ptr, const char* scalar_format);

//-----------------------------------------------------------------------------
// Platform dependent default implementations
//-----------------------------------------------------------------------------

static const char*      GetClipboardTextFn_DefaultImpl();
static void             SetClipboardTextFn_DefaultImpl(const char* text);
static void             ImeSetInputScreenPosFn_DefaultImpl(int x, int y);

//-----------------------------------------------------------------------------
// Context
//-----------------------------------------------------------------------------

// We access everything through this pointer (always assumed to be != NULL)
// You can swap the pointer to a different context by calling ImGui::SetInternalState()
static ImGuiState       GImDefaultState;
ImGuiState*             GImGui = &GImDefaultState;

// Statically allocated default font atlas. This is merely a maneuver to keep ImFontAtlas definition at the bottom of the .h file (otherwise it'd be inside ImGuiIO)
// Also we wouldn't be able to new() one at this point, before users may define IO.MemAllocFn.
static ImFontAtlas      GImDefaultFontAtlas;

//-----------------------------------------------------------------------------
// User facing structures
//-----------------------------------------------------------------------------

ImGuiStyle::ImGuiStyle()
{
    Alpha                   = 1.0f;             // Global alpha applies to everything in ImGui
    WindowPadding           = ImVec2(8,8);      // Padding within a window
    WindowMinSize           = ImVec2(32,32);    // Minimum window size
    WindowRounding          = 9.0f;             // Radius of window corners rounding. Set to 0.0f to have rectangular windows
    WindowTitleAlign        = ImGuiAlign_Left;  // Alignment for title bar text
    ChildWindowRounding     = 0.0f;             // Radius of child window corners rounding. Set to 0.0f to have rectangular windows
    FramePadding            = ImVec2(4,3);      // Padding within a framed rectangle (used by most widgets)
    FrameRounding           = 0.0f;             // Radius of frame corners rounding. Set to 0.0f to have rectangular frames (used by most widgets).
    ItemSpacing             = ImVec2(8,4);      // Horizontal and vertical spacing between widgets/lines
    ItemInnerSpacing        = ImVec2(4,4);      // Horizontal and vertical spacing between within elements of a composed widget (e.g. a slider and its label)
    TouchExtraPadding       = ImVec2(0,0);      // Expand reactive bounding box for touch-based system where touch position is not accurate enough. Unfortunately we don't sort widgets so priority on overlap will always be given to the first widget. So don't grow this too much!
    WindowFillAlphaDefault  = 0.70f;            // Default alpha of window background, if not specified in ImGui::Begin()
    IndentSpacing           = 22.0f;            // Horizontal spacing when e.g. entering a tree node
    ColumnsMinSpacing       = 6.0f;             // Minimum horizontal spacing between two columns
    ScrollbarSize           = 16.0f;            // Width of the vertical scrollbar, Height of the horizontal scrollbar
    ScrollbarRounding       = 9.0f;             // Radius of grab corners rounding for scrollbar
    GrabMinSize             = 10.0f;            // Minimum width/height of a grab box for slider/scrollbar
    GrabRounding            = 0.0f;             // Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
    DisplayWindowPadding    = ImVec2(22,22);    // Window positions are clamped to be visible within the display area by at least this amount. Only covers regular windows.
    DisplaySafeAreaPadding  = ImVec2(4,4);      // If you cannot see the edge of your screen (e.g. on a TV) increase the safe area padding. Covers popups/tooltips as well regular windows.
    AntiAliasedLines        = true;             // Enable anti-aliasing on lines/borders. Disable if you are really short on CPU/GPU.
    AntiAliasedShapes       = true;             // Enable anti-aliasing on filled shapes (rounded rectangles, circles, etc.)
    CurveTessellationTol    = 1.25f;            // Tessellation tolerance. Decrease for highly tessellated curves (higher quality, more polygons), increase to reduce quality.

    Colors[ImGuiCol_Text]                   = ImVec4(0.90f, 0.90f, 0.90f, 1.00f);
    Colors[ImGuiCol_TextDisabled]           = ImVec4(0.60f, 0.60f, 0.60f, 1.00f);
    Colors[ImGuiCol_WindowBg]               = ImVec4(0.00f, 0.00f, 0.00f, 1.00f);
    Colors[ImGuiCol_ChildWindowBg]          = ImVec4(0.00f, 0.00f, 0.00f, 0.00f);
    Colors[ImGuiCol_Border]                 = ImVec4(0.70f, 0.70f, 0.70f, 0.65f);
    Colors[ImGuiCol_BorderShadow]           = ImVec4(0.00f, 0.00f, 0.00f, 0.00f);
    Colors[ImGuiCol_FrameBg]                = ImVec4(0.80f, 0.80f, 0.80f, 0.30f);   // Background of checkbox, radio button, plot, slider, text input
    Colors[ImGuiCol_FrameBgHovered]         = ImVec4(0.90f, 0.80f, 0.80f, 0.40f);
    Colors[ImGuiCol_FrameBgActive]          = ImVec4(0.90f, 0.65f, 0.65f, 0.45f);
    Colors[ImGuiCol_TitleBg]                = ImVec4(0.50f, 0.50f, 1.00f, 0.45f);
    Colors[ImGuiCol_TitleBgCollapsed]       = ImVec4(0.40f, 0.40f, 0.80f, 0.20f);
    Colors[ImGuiCol_TitleBgActive]          = ImVec4(0.50f, 0.50f, 1.00f, 0.55f);
    Colors[ImGuiCol_MenuBarBg]              = ImVec4(0.40f, 0.40f, 0.55f, 0.80f);
    Colors[ImGuiCol_ScrollbarBg]            = ImVec4(0.20f, 0.25f, 0.30f, 0.60f);
    Colors[ImGuiCol_ScrollbarGrab]          = ImVec4(0.40f, 0.40f, 0.80f, 0.30f);
    Colors[ImGuiCol_ScrollbarGrabHovered]   = ImVec4(0.40f, 0.40f, 0.80f, 0.40f);
    Colors[ImGuiCol_ScrollbarGrabActive]    = ImVec4(0.80f, 0.50f, 0.50f, 0.40f);
    Colors[ImGuiCol_ComboBg]                = ImVec4(0.20f, 0.20f, 0.20f, 0.99f);
    Colors[ImGuiCol_CheckMark]              = ImVec4(0.90f, 0.90f, 0.90f, 0.50f);
    Colors[ImGuiCol_SliderGrab]             = ImVec4(1.00f, 1.00f, 1.00f, 0.30f);
    Colors[ImGuiCol_SliderGrabActive]       = ImVec4(0.80f, 0.50f, 0.50f, 1.00f);
    Colors[ImGuiCol_Button]                 = ImVec4(0.67f, 0.40f, 0.40f, 0.60f);
    Colors[ImGuiCol_ButtonHovered]          = ImVec4(0.67f, 0.40f, 0.40f, 1.00f);
    Colors[ImGuiCol_ButtonActive]           = ImVec4(0.80f, 0.50f, 0.50f, 1.00f);
    Colors[ImGuiCol_Header]                 = ImVec4(0.40f, 0.40f, 0.90f, 0.45f);
    Colors[ImGuiCol_HeaderHovered]          = ImVec4(0.45f, 0.45f, 0.90f, 0.80f);
    Colors[ImGuiCol_HeaderActive]           = ImVec4(0.53f, 0.53f, 0.87f, 0.80f);
    Colors[ImGuiCol_Column]                 = ImVec4(0.50f, 0.50f, 0.50f, 1.00f);
    Colors[ImGuiCol_ColumnHovered]          = ImVec4(0.70f, 0.60f, 0.60f, 1.00f);
    Colors[ImGuiCol_ColumnActive]           = ImVec4(0.90f, 0.70f, 0.70f, 1.00f);
    Colors[ImGuiCol_ResizeGrip]             = ImVec4(1.00f, 1.00f, 1.00f, 0.30f);
    Colors[ImGuiCol_ResizeGripHovered]      = ImVec4(1.00f, 1.00f, 1.00f, 0.60f);
    Colors[ImGuiCol_ResizeGripActive]       = ImVec4(1.00f, 1.00f, 1.00f, 0.90f);
    Colors[ImGuiCol_CloseButton]            = ImVec4(0.50f, 0.50f, 0.90f, 0.50f);
    Colors[ImGuiCol_CloseButtonHovered]     = ImVec4(0.70f, 0.70f, 0.90f, 0.60f);
    Colors[ImGuiCol_CloseButtonActive]      = ImVec4(0.70f, 0.70f, 0.70f, 1.00f);
    Colors[ImGuiCol_PlotLines]              = ImVec4(1.00f, 1.00f, 1.00f, 1.00f);
    Colors[ImGuiCol_PlotLinesHovered]       = ImVec4(0.90f, 0.70f, 0.00f, 1.00f);
    Colors[ImGuiCol_PlotHistogram]          = ImVec4(0.90f, 0.70f, 0.00f, 1.00f);
    Colors[ImGuiCol_PlotHistogramHovered]   = ImVec4(1.00f, 0.60f, 0.00f, 1.00f);
    Colors[ImGuiCol_TextSelectedBg]         = ImVec4(0.00f, 0.00f, 1.00f, 0.35f);
    Colors[ImGuiCol_TooltipBg]              = ImVec4(0.05f, 0.05f, 0.10f, 0.90f);
    Colors[ImGuiCol_ModalWindowDarkening]   = ImVec4(0.20f, 0.20f, 0.20f, 0.35f);
}

ImGuiIO::ImGuiIO()
{
    // Most fields are initialized with zero
    memset(this, 0, sizeof(*this));

    DisplaySize = ImVec2(-1.0f, -1.0f);
    DeltaTime = 1.0f/60.0f;
    IniSavingRate = 5.0f;
    IniFilename = "imgui.ini";
    LogFilename = "imgui_log.txt";
    Fonts = &GImDefaultFontAtlas;
    FontGlobalScale = 1.0f;
    DisplayFramebufferScale = ImVec2(1.0f, 1.0f);
    MousePos = ImVec2(-1,-1);
    MousePosPrev = ImVec2(-1,-1);
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
        InputCharacters[n+1] = '\0';
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

//-----------------------------------------------------------------------------
// HELPERS
//-----------------------------------------------------------------------------

#define IM_F32_TO_INT8(_VAL)  ((int)((_VAL) * 255.0f + 0.5f))

#define IM_INT_MIN  (-2147483647-1)
#define IM_INT_MAX  (2147483647)

// Play it nice with Windows users. Notepad in 2015 still doesn't display text data with Unix-style \n.
#ifdef _WIN32
#define IM_NEWLINE "\r\n"
#else
#define IM_NEWLINE "\n"
#endif

const char* ImStristr(const char* haystack, const char* haystack_end, const char* needle, const char* needle_end)
{
    if (!needle_end)
        needle_end = needle + strlen(needle);

    const char un0 = (char)toupper(*needle);
    while ((!haystack_end && *haystack) || (haystack_end && haystack < haystack_end))
    {
        if (toupper(*haystack) == un0)
        {
            const char* b = needle + 1;
            for (const char* a = haystack + 1; b < needle_end; a++, b++)
                if (toupper(*a) != toupper(*b))
                    break;
            if (b == needle_end)
                return haystack;
        }
        haystack++;
    }
    return NULL;
}


// Load file content into memory
// Memory allocated with ImGui::MemAlloc(), must be freed by user using ImGui::MemFree()
void* ImLoadFileToMemory(const char* filename, const char* file_open_mode, int* out_file_size, int padding_bytes)
{
    IM_ASSERT(filename && file_open_mode);
    if (out_file_size)
        *out_file_size = 0;

    FILE* f;
    if ((f = fopen(filename, file_open_mode)) == NULL)
        return NULL;

    long file_size_signed;
    if (fseek(f, 0, SEEK_END) || (file_size_signed = ftell(f)) == -1 || fseek(f, 0, SEEK_SET))
    {
        fclose(f);
        return NULL;
    }

    int file_size = (int)file_size_signed;
    void* file_data = ImGui::MemAlloc(file_size + padding_bytes);
    if (file_data == NULL)
    {
        fclose(f);
        return NULL;
    }
    if (fread(file_data, 1, (size_t)file_size, f) != (size_t)file_size)
    {
        fclose(f);
        ImGui::MemFree(file_data);
        return NULL;
    }
    if (padding_bytes > 0)
        memset((void *)(((char*)file_data) + file_size), 0, padding_bytes);

    fclose(f);
    if (out_file_size)
        *out_file_size = file_size;

    return file_data;
}

//-----------------------------------------------------------------------------
// ImGuiTextFilter
//-----------------------------------------------------------------------------

// Helper: Parse and apply text filters. In format "aaaaa[,bbbb][,ccccc]"
ImGuiTextFilter::ImGuiTextFilter(const char* default_filter)
{
    if (default_filter)
    {
        ImFormatString(InputBuf, IM_ARRAYSIZE(InputBuf), "%s", default_filter);
        Build();
    }
    else
    {
        InputBuf[0] = 0;
        CountGrep = 0;
    }
}

bool ImGuiTextFilter::Draw(const char* label, float width)
{
    if (width != 0.0f)
        ImGui::PushItemWidth(width);
    bool value_changed = ImGui::InputText(label, InputBuf, IM_ARRAYSIZE(InputBuf));
    if (width != 0.0f)
        ImGui::PopItemWidth();
    if (value_changed)
        Build();
    return value_changed;
}

void ImGuiTextFilter::TextRange::split(char separator, ImVector<TextRange>& out)
{
    out.resize(0);
    const char* wb = b;
    const char* we = wb;
    while (we < e)
    {
        if (*we == separator)
        {
            out.push_back(TextRange(wb, we));
            wb = we + 1;
        }
        we++;
    }
    if (wb != we)
        out.push_back(TextRange(wb, we));
}

void ImGuiTextFilter::Build()
{
    Filters.resize(0);
    TextRange input_range(InputBuf, InputBuf+strlen(InputBuf));
    input_range.split(',', Filters);

    CountGrep = 0;
    for (int i = 0; i != Filters.Size; i++)
    {
        Filters[i].trim_blanks();
        if (Filters[i].empty())
            continue;
        if (Filters[i].front() != '-')
            CountGrep += 1;
    }
}

bool ImGuiTextFilter::PassFilter(const char* text, const char* text_end) const
{
    if (Filters.empty())
        return true;

    if (text == NULL)
        text = "";

    for (int i = 0; i != Filters.Size; i++)
    {
        const TextRange& f = Filters[i];
        if (f.empty())
            continue;
        if (f.front() == '-')
        {
            // Subtract
            if (ImStristr(text, text_end, f.begin()+1, f.end()) != NULL)
                return false;
        }
        else
        {
            // Grep
            if (ImStristr(text, text_end, f.begin(), f.end()) != NULL)
                return true;
        }
    }

    // Implicit * grep
    if (CountGrep == 0)
        return true;

    return false;
}

//-----------------------------------------------------------------------------
// ImGuiTextBuffer
//-----------------------------------------------------------------------------

// On some platform vsnprintf() takes va_list by reference and modifies it.
// va_copy is the 'correct' way to copy a va_list but Visual Studio prior to 2013 doesn't have it.
#ifndef va_copy
#define va_copy(dest, src) (dest = src)
#endif

// Helper: Text buffer for logging/accumulating text
void ImGuiTextBuffer::appendv(const char* fmt, va_list args)
{
    va_list args_copy;
    va_copy(args_copy, args);

    int len = vsnprintf(NULL, 0, fmt, args);         // FIXME-OPT: could do a first pass write attempt, likely successful on first pass.
    if (len <= 0)
        return;

    const int write_off = Buf.Size;
    const int needed_sz = write_off + len;
    if (write_off + len >= Buf.Capacity)
    {
        int double_capacity = Buf.Capacity * 2;
        Buf.reserve(needed_sz > double_capacity ? needed_sz : double_capacity);
    }

    Buf.resize(needed_sz);
    ImFormatStringV(&Buf[write_off] - 1, len+1, fmt, args_copy);
}

void ImGuiTextBuffer::append(const char* fmt, ...)
{
    va_list args;
    va_start(args, fmt);
    appendv(fmt, args);
    va_end(args);
}

//-----------------------------------------------------------------------------
// Internal API exposed in imgui_internal.h
//-----------------------------------------------------------------------------




static inline void DataTypeFormatString(ImGuiDataType data_type, void* data_ptr, const char* display_format, char* buf, int buf_size)
{
    if (data_type == ImGuiDataType_Int)
        ImFormatString(buf, buf_size, display_format, *(int*)data_ptr);
    else if (data_type == ImGuiDataType_Float)
        ImFormatString(buf, buf_size, display_format, *(float*)data_ptr);
}

static inline void DataTypeFormatString(ImGuiDataType data_type, void* data_ptr, int decimal_precision, char* buf, int buf_size)
{
    if (data_type == ImGuiDataType_Int)
    {
        if (decimal_precision < 0)
            ImFormatString(buf, buf_size, "%d", *(int*)data_ptr);
        else
            ImFormatString(buf, buf_size, "%.*d", decimal_precision, *(int*)data_ptr);
    }
    else if (data_type == ImGuiDataType_Float)
    {
        if (decimal_precision < 0)
            ImFormatString(buf, buf_size, "%f", *(float*)data_ptr);     // Ideally we'd have a minimum decimal precision of 1 to visually denote that it is a float, while hiding non-significant digits?
        else
            ImFormatString(buf, buf_size, "%.*f", decimal_precision, *(float*)data_ptr);
    }
}

static void DataTypeApplyOp(ImGuiDataType data_type, int op, void* value1, const void* value2)// Store into value1
{
    if (data_type == ImGuiDataType_Int)
    {
        if (op == '+')
            *(int*)value1 = *(int*)value1 + *(const int*)value2;
        else if (op == '-')
            *(int*)value1 = *(int*)value1 - *(const int*)value2;
    }
    else if (data_type == ImGuiDataType_Float)
    {
        if (op == '+')
            *(float*)value1 = *(float*)value1 + *(const float*)value2;
        else if (op == '-')
            *(float*)value1 = *(float*)value1 - *(const float*)value2;
    }
}

// User can input math operators (e.g. +100) to edit a numerical values.
static void DataTypeApplyOpFromText(const char* buf, const char* initial_value_buf, ImGuiDataType data_type, void* data_ptr, const char* scalar_format)
{
    while (ImCharIsSpace(*buf))
        buf++;

    // We don't support '-' op because it would conflict with inputing negative value.
    // Instead you can use +-100 to subtract from an existing value
    char op = buf[0];
    if (op == '+' || op == '*' || op == '/')
    {
        buf++;
        while (ImCharIsSpace(*buf))
            buf++;
    }
    else
    {
        op = 0;
    }
    if (!buf[0])
        return;

    if (data_type == ImGuiDataType_Int)
    {
        if (!scalar_format)
            scalar_format = "%d";
        int* v = (int*)data_ptr;
        int ref_v = *v;
        if (op && sscanf(initial_value_buf, scalar_format, &ref_v) < 1)
            return;

        // Store operand in a float so we can use fractional value for multipliers (*1.1), but constant always parsed as integer so we can fit big integers (e.g. 2000000003) past float precision
        float op_v = 0.0f;
        if (op == '+')      { if (sscanf(buf, "%f", &op_v) == 1) *v = (int)(ref_v + op_v); }                // Add (use "+-" to subtract)
        else if (op == '*') { if (sscanf(buf, "%f", &op_v) == 1) *v = (int)(ref_v * op_v); }                // Multiply
        else if (op == '/') { if (sscanf(buf, "%f", &op_v) == 1 && op_v != 0.0f) *v = (int)(ref_v / op_v); }// Divide
        else                { if (sscanf(buf, scalar_format, &ref_v) == 1) *v = ref_v; }                    // Assign constant
    }
    else if (data_type == ImGuiDataType_Float)
    {
        // For floats we have to ignore format with precision (e.g. "%.2f") because sscanf doesn't take them in
        scalar_format = "%f";
        float* v = (float*)data_ptr;
        float ref_v = *v;
        if (op && sscanf(initial_value_buf, scalar_format, &ref_v) < 1)
            return;
        float op_v = 0.0f;
        if (sscanf(buf, scalar_format, &op_v) < 1)
            return;

        if (op == '+')      { *v = ref_v + op_v; }                      // Add (use "+-" to subtract)
        else if (op == '*') { *v = ref_v * op_v; }                      // Multiply
        else if (op == '/') { if (op_v != 0.0f) *v = ref_v / op_v; }    // Divide
        else                { *v = op_v; }                              // Assign constant
    }
}


bool ImGui::ColorEdit3(const char* label, float col[3])
{
    float col4[4];
    col4[0] = col[0];
    col4[1] = col[1];
    col4[2] = col[2];
    col4[3] = 1.0f;
    const bool value_changed = ImGui::ColorEdit4(label, col4, false);
    col[0] = col4[0];
    col[1] = col4[1];
    col[2] = col4[2];
    return value_changed;
}

// Edit colors components (each component in 0.0f..1.0f range
// Use CTRL-Click to input value and TAB to go to next item.
bool ImGui::ColorEdit4(const char* label, float col[4], bool alpha)
{
    ImGuiWindow* window = GetCurrentWindow();
    if (window->SkipItems)
        return false;

    ImGuiState& g = *GImGui;
    const ImGuiStyle& style = g.Style;
    const ImGuiID id = window->GetID(label);
    const float w_full = CalcItemWidth();
    const float square_sz = (g.FontSize + style.FramePadding.y * 2.0f);

    ImGuiColorEditMode edit_mode = window->DC.ColorEditMode;
    if (edit_mode == ImGuiColorEditMode_UserSelect || edit_mode == ImGuiColorEditMode_UserSelectShowButton)
        edit_mode = g.ColorEditModeStorage.GetInt(id, 0) % 3;

    float f[4] = { col[0], col[1], col[2], col[3] };
    if (edit_mode == ImGuiColorEditMode_HSV)
        ImGui::ColorConvertRGBtoHSV(f[0], f[1], f[2], f[0], f[1], f[2]);

    int i[4] = { IM_F32_TO_INT8(f[0]), IM_F32_TO_INT8(f[1]), IM_F32_TO_INT8(f[2]), IM_F32_TO_INT8(f[3]) };

    int components = alpha ? 4 : 3;
    bool value_changed = false;

    ImGui::BeginGroup();
    ImGui::PushID(label);

    const bool hsv = (edit_mode == 1);
    switch (edit_mode)
    {
    case ImGuiColorEditMode_RGB:
    case ImGuiColorEditMode_HSV:
        {
            // RGB/HSV 0..255 Sliders
            const float w_items_all = w_full - (square_sz + style.ItemInnerSpacing.x);
            const float w_item_one  = ImMax(1.0f, (float)(int)((w_items_all - (style.ItemInnerSpacing.x) * (components-1)) / (float)components));
            const float w_item_last = ImMax(1.0f, (float)(int)(w_items_all - (w_item_one + style.ItemInnerSpacing.x) * (components-1)));

            const bool hide_prefix = (w_item_one <= CalcTextSize("M:999").x);
            const char* ids[4] = { "##X", "##Y", "##Z", "##W" };
            const char* fmt_table[3][4] =
            {
                {   "%3.0f",   "%3.0f",   "%3.0f",   "%3.0f" },
                { "R:%3.0f", "G:%3.0f", "B:%3.0f", "A:%3.0f" },
                { "H:%3.0f", "S:%3.0f", "V:%3.0f", "A:%3.0f" }
            };
            const char** fmt = hide_prefix ? fmt_table[0] : hsv ? fmt_table[2] : fmt_table[1];

            ImGui::PushItemWidth(w_item_one);
            for (int n = 0; n < components; n++)
            {
                if (n > 0)
                    ImGui::SameLine(0, style.ItemInnerSpacing.x);
                if (n + 1 == components)
                    ImGui::PushItemWidth(w_item_last);
                value_changed |= ImGui::DragInt(ids[n], &i[n], 1.0f, 0, 255, fmt[n]);
            }
            ImGui::PopItemWidth();
            ImGui::PopItemWidth();
        }
        break;
    case ImGuiColorEditMode_HEX:
        {
            // RGB Hexadecimal Input
            const float w_slider_all = w_full - square_sz;
            char buf[64];
            if (alpha)
                ImFormatString(buf, IM_ARRAYSIZE(buf), "#%02X%02X%02X%02X", i[0], i[1], i[2], i[3]);
            else
                ImFormatString(buf, IM_ARRAYSIZE(buf), "#%02X%02X%02X", i[0], i[1], i[2]);
            ImGui::PushItemWidth(w_slider_all - style.ItemInnerSpacing.x);
            if (ImGui::InputText("##Text", buf, IM_ARRAYSIZE(buf), ImGuiInputTextFlags_CharsHexadecimal | ImGuiInputTextFlags_CharsUppercase))
            {
                value_changed |= true;
                char* p = buf;
                while (*p == '#' || ImCharIsSpace(*p))
                    p++;
                i[0] = i[1] = i[2] = i[3] = 0;
                if (alpha)
                    sscanf(p, "%02X%02X%02X%02X", (unsigned int*)&i[0], (unsigned int*)&i[1], (unsigned int*)&i[2], (unsigned int*)&i[3]); // Treat at unsigned (%X is unsigned)
                else
                    sscanf(p, "%02X%02X%02X", (unsigned int*)&i[0], (unsigned int*)&i[1], (unsigned int*)&i[2]);
            }
            ImGui::PopItemWidth();
        }
        break;
    }

    ImGui::SameLine(0, style.ItemInnerSpacing.x);

    const ImVec4 col_display(col[0], col[1], col[2], 1.0f);
    if (ImGui::ColorButton(col_display))
        g.ColorEditModeStorage.SetInt(id, (edit_mode + 1) % 3); // Don't set local copy of 'edit_mode' right away!
    
    // Recreate our own tooltip over's ColorButton() one because we want to display correct alpha here
    if (ImGui::IsItemHovered())
        ImGui::SetTooltip("Color:\n(%.2f,%.2f,%.2f,%.2f)\n#%02X%02X%02X%02X", col[0], col[1], col[2], col[3], IM_F32_TO_INT8(col[0]), IM_F32_TO_INT8(col[1]), IM_F32_TO_INT8(col[2]), IM_F32_TO_INT8(col[3]));

    if (window->DC.ColorEditMode == ImGuiColorEditMode_UserSelectShowButton)
    {
        ImGui::SameLine(0, style.ItemInnerSpacing.x);
        const char* button_titles[3] = { "RGB", "HSV", "HEX" };
        if (ButtonEx(button_titles[edit_mode], ImVec2(0,0), ImGuiButtonFlags_DontClosePopups))
            g.ColorEditModeStorage.SetInt(id, (edit_mode + 1) % 3); // Don't set local copy of 'edit_mode' right away!
    }

    const char* label_display_end = FindRenderedTextEnd(label);
    if (label != label_display_end)
    {
        ImGui::SameLine(0, (window->DC.ColorEditMode == ImGuiColorEditMode_UserSelectShowButton) ? -1.0f : style.ItemInnerSpacing.x);
        ImGui::TextUnformatted(label, label_display_end);
    }

    // Convert back
    for (int n = 0; n < 4; n++)
        f[n] = i[n] / 255.0f;
    if (edit_mode == 1)
        ImGui::ColorConvertHSVtoRGB(f[0], f[1], f[2], f[0], f[1], f[2]);

    if (value_changed)
    {
        col[0] = f[0];
        col[1] = f[1];
        col[2] = f[2];
        if (alpha)
            col[3] = f[3];
    }

    ImGui::PopID();
    ImGui::EndGroup();

    return value_changed;
}

//-----------------------------------------------------------------------------
// PLATFORM DEPENDANT HELPERS
//-----------------------------------------------------------------------------

#if defined(_WIN32) && !defined(_WINDOWS_) && (!defined(IMGUI_DISABLE_WIN32_DEFAULT_CLIPBOARD_FUNCS) || !defined(IMGUI_DISABLE_WIN32_DEFAULT_IME_FUNCS))
#undef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

// Win32 API clipboard implementation
#if defined(_WIN32) && !defined(IMGUI_DISABLE_WIN32_DEFAULT_CLIPBOARD_FUNCS)

#ifdef _MSC_VER
#pragma comment(lib, "user32")
#endif

static const char* GetClipboardTextFn_DefaultImpl()
{
    static char* buf_local = NULL;
    if (buf_local)
    {
        ImGui::MemFree(buf_local);
        buf_local = NULL;
    }
    if (!OpenClipboard(NULL))
        return NULL;
    HANDLE wbuf_handle = GetClipboardData(CF_UNICODETEXT);
    if (wbuf_handle == NULL)
        return NULL;
    if (ImWchar* wbuf_global = (ImWchar*)GlobalLock(wbuf_handle))
    {
        int buf_len = ImTextCountUtf8BytesFromStr(wbuf_global, NULL) + 1;
        buf_local = (char*)ImGui::MemAlloc(buf_len * sizeof(char));
        ImTextStrToUtf8(buf_local, buf_len, wbuf_global, NULL);
    }
    GlobalUnlock(wbuf_handle);
    CloseClipboard();
    return buf_local;
}

static void SetClipboardTextFn_DefaultImpl(const char* text)
{
    if (!OpenClipboard(NULL))
        return;

    const int wbuf_length = ImTextCountCharsFromUtf8(text, NULL) + 1;
    HGLOBAL wbuf_handle = GlobalAlloc(GMEM_MOVEABLE, (SIZE_T)wbuf_length * sizeof(ImWchar));
    if (wbuf_handle == NULL)
        return;
    ImWchar* wbuf_global = (ImWchar*)GlobalLock(wbuf_handle);
    ImTextStrFromUtf8(wbuf_global, wbuf_length, text, NULL);
    GlobalUnlock(wbuf_handle);
    EmptyClipboard();
    SetClipboardData(CF_UNICODETEXT, wbuf_handle);
    CloseClipboard();
}

#else

// Local ImGui-only clipboard implementation, if user hasn't defined better clipboard handlers
static const char* GetClipboardTextFn_DefaultImpl()
{
    return GImGui->PrivateClipboard;
}

// Local ImGui-only clipboard implementation, if user hasn't defined better clipboard handlers
static void SetClipboardTextFn_DefaultImpl(const char* text)
{
    ImGuiState& g = *GImGui;
    if (g.PrivateClipboard)
    {
        ImGui::MemFree(g.PrivateClipboard);
        g.PrivateClipboard = NULL;
    }
    const char* text_end = text + strlen(text);
    g.PrivateClipboard = (char*)ImGui::MemAlloc((size_t)(text_end - text) + 1);
    memcpy(g.PrivateClipboard, text, (size_t)(text_end - text));
    g.PrivateClipboard[(int)(text_end - text)] = 0;
}

#endif

// Win32 API IME support (for Asian languages, etc.)
#if defined(_WIN32) && !defined(IMGUI_DISABLE_WIN32_DEFAULT_IME_FUNCS)

#include <imm.h>
#ifdef _MSC_VER
#pragma comment(lib, "imm32")
#endif

static void ImeSetInputScreenPosFn_DefaultImpl(int x, int y)
{
    // Notify OS Input Method Editor of text input position
    if (HWND hwnd = (HWND)GImGui->IO.ImeWindowHandle)
        if (HIMC himc = ImmGetContext(hwnd))
        {
            COMPOSITIONFORM cf;
            cf.ptCurrentPos.x = x;
            cf.ptCurrentPos.y = y;
            cf.dwStyle = CFS_FORCE_POSITION;
            ImmSetCompositionWindow(himc, &cf);
        }
}

#else

static void ImeSetInputScreenPosFn_DefaultImpl(int, int) {}

#endif

//-----------------------------------------------------------------------------
// HELP
//-----------------------------------------------------------------------------

void ImGui::ShowMetricsWindow(bool* opened)
{
    if (ImGui::Begin("ImGui Metrics", opened))
    {
        ImGui::Text("ImGui %s", ImGui::GetVersion());
        ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
        ImGui::Text("%d vertices, %d indices (%d triangles)", ImGui::GetIO().MetricsRenderVertices, ImGui::GetIO().MetricsRenderIndices, ImGui::GetIO().MetricsRenderIndices / 3);
        ImGui::Text("%d allocations", ImGui::GetIO().MetricsAllocs);
        static bool show_clip_rects = true;
        ImGui::Checkbox("Show clipping rectangles when hovering a ImDrawCmd", &show_clip_rects);
        ImGui::Separator();

        struct Funcs
        {
            static void NodeDrawList(ImDrawList* draw_list, const char* label)
            {
                bool node_opened = ImGui::TreeNode(draw_list, "%s: '%s' %d vtx, %d indices, %d cmds", label, draw_list->_OwnerName ? draw_list->_OwnerName : "", draw_list->VtxBuffer.Size, draw_list->IdxBuffer.Size, draw_list->CmdBuffer.Size);
                if (draw_list == ImGui::GetWindowDrawList())
                {
                    ImGui::SameLine();
                    ImGui::TextColored(ImColor(255,100,100), "CURRENTLY APPENDING"); // Can't display stats for active draw list! (we don't have the data double-buffered)
                }
                if (!node_opened)
                    return;

                int elem_offset = 0;
                for (const ImDrawCmd* pcmd = draw_list->CmdBuffer.begin(); pcmd < draw_list->CmdBuffer.end(); elem_offset += pcmd->ElemCount, pcmd++)
                {
                    if (pcmd->UserCallback)
                    {
                        ImGui::BulletText("Callback %p, user_data %p", pcmd->UserCallback, pcmd->UserCallbackData);
                        continue;
                    }
                    ImGui::BulletText("Draw %-4d %s vtx, tex = %p, clip_rect = (%.0f,%.0f)..(%.0f,%.0f)", pcmd->ElemCount, draw_list->IdxBuffer.Size > 0 ? "indexed" : "non-indexed", pcmd->TextureId, pcmd->ClipRect.x, pcmd->ClipRect.y, pcmd->ClipRect.z, pcmd->ClipRect.w);
                    if (show_clip_rects && ImGui::IsItemHovered())
                    {
                        ImRect clip_rect = pcmd->ClipRect;
                        ImRect vtxs_rect;
                        ImDrawIdx* idx_buffer = (draw_list->IdxBuffer.Size > 0) ? draw_list->IdxBuffer.Data : NULL;
                        for (int i = elem_offset; i < elem_offset + (int)pcmd->ElemCount; i++)
                            vtxs_rect.Add(draw_list->VtxBuffer[idx_buffer ? idx_buffer[i] : i].pos);
                        GImGui->OverlayDrawList.PushClipRectFullScreen();
                        clip_rect.Round(); GImGui->OverlayDrawList.AddRect(clip_rect.Min, clip_rect.Max, ImColor(255,255,0));
                        vtxs_rect.Round(); GImGui->OverlayDrawList.AddRect(vtxs_rect.Min, vtxs_rect.Max, ImColor(255,0,255));
                        GImGui->OverlayDrawList.PopClipRect();
                    }
                }
                ImGui::TreePop();
            }

            static void NodeWindows(ImVector<ImGuiWindow*>& windows, const char* label)
            {
                if (!ImGui::TreeNode(label, "%s (%d)", label, windows.Size))
                    return;
                for (int i = 0; i < windows.Size; i++)
                    Funcs::NodeWindow(windows[i], "Window");
                ImGui::TreePop();
            }

            static void NodeWindow(ImGuiWindow* window, const char* label)
            {
                if (!ImGui::TreeNode(window, "%s '%s', %d @ 0x%p", label, window->Name, window->Active || window->WasActive, window))
                    return;
                NodeDrawList(window->DrawList, "DrawList");
                if (window->RootWindow != window) NodeWindow(window->RootWindow, "RootWindow");
                if (window->DC.ChildWindows.Size > 0) NodeWindows(window->DC.ChildWindows, "ChildWindows");
                ImGui::BulletText("Storage: %d bytes", window->StateStorage.Data.Size * (int)sizeof(ImGuiStorage::Pair));
                ImGui::TreePop();
            }
        };

        ImGuiState& g = *GImGui;                // Access private state
        Funcs::NodeWindows(g.Windows, "Windows");
        if (ImGui::TreeNode("DrawList", "Active DrawLists (%d)", g.RenderDrawLists[0].Size))
        {
            for (int i = 0; i < g.RenderDrawLists[0].Size; i++)
                Funcs::NodeDrawList(g.RenderDrawLists[0][i], "DrawList");
            ImGui::TreePop();
        }
        if (ImGui::TreeNode("Popups", "Opened Popups Stack (%d)", g.OpenedPopupStack.Size))
        {
            for (int i = 0; i < g.OpenedPopupStack.Size; i++)
            {
                ImGuiWindow* window = g.OpenedPopupStack[i].Window;
                ImGui::BulletText("PopupID: %08x, Window: '%s'%s%s", g.OpenedPopupStack[i].PopupID, window ? window->Name : "NULL", window && (window->Flags & ImGuiWindowFlags_ChildWindow) ? " ChildWindow" : "", window && (window->Flags & ImGuiWindowFlags_ChildMenu) ? " ChildMenu" : "");
            }
            ImGui::TreePop();
        }
        if (ImGui::TreeNode("Basic state"))
        {
            ImGui::Text("FocusedWindow: '%s'", g.FocusedWindow ? g.FocusedWindow->Name : "NULL");
            ImGui::Text("HoveredWindow: '%s'", g.HoveredWindow ? g.HoveredWindow->Name : "NULL");
            ImGui::Text("HoveredRootWindow: '%s'", g.HoveredRootWindow ? g.HoveredRootWindow->Name : "NULL");
            ImGui::Text("HoveredID: 0x%08X/0x%08X", g.HoveredId, g.HoveredIdPreviousFrame); // Data is "in-flight" so depending on when the Metrics window is called we may see current frame information or not
            ImGui::Text("ActiveID: 0x%08X/0x%08X", g.ActiveId, g.ActiveIdPreviousFrame);
            ImGui::TreePop();
        }
    }
    ImGui::End();
}

//-----------------------------------------------------------------------------

// Include imgui_user.inl at the end of imgui.cpp to access private data/functions that aren't exposed.
// Prefer just including imgui_internal.h from your code rather than using this define. If a declaration is missing from imgui_internal.h add it or request it on the github.
#ifdef IMGUI_INCLUDE_IMGUI_USER_INL
#include "imgui_user.inl"
#endif

//-----------------------------------------------------------------------------
