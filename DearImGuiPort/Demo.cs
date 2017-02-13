using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGui
{
    internal partial class ImGui
    {
        void ShowHelpMarker(string desc)
        {
            TextDisabled("(?)");
            if (IsItemHovered())
                SetTooltip(desc);
        }

        void ShowUserGuide()
        {
            BulletText("Double-click on title bar to collapse window.");
            BulletText("Click and drag on lower right corner to resize window.");
            BulletText("Click and drag on any empty space to move window.");
            BulletText("Mouse Wheel to scroll.");
            if (GetIO().FontAllowUserScaling)
                BulletText("CTRL+Mouse Wheel to zoom window contents.");
            BulletText("TAB/SHIFT+TAB to cycle through keyboard editable fields.");
            BulletText("CTRL+Click on a slider or drag box to input text.");
            BulletText(
                "While editing text:\n" +
                "- Hold SHIFT or use mouse to select text\n" +
                "- CTRL+Left/Right to word jump\n" +
                "- CTRL+A or double-click to select all\n" +
                "- CTRL+X,CTRL+C,CTRL+V clipboard\n" +
                "- CTRL+Z,CTRL+Y undo/redo\n" +
                "- ESCAPE to revert\n" +
                "- You can apply arithmetic operators +,*,/ on numerical values.\n" +
                "  Use +- to subtract.\n");
        }

        bool show_app_main_menu_bar = false;
        bool show_app_console = false;
        bool show_app_log = false;
        bool show_app_layout = false;
        bool show_app_property_editor = false;
        bool show_app_long_text = false;
        bool show_app_auto_resize = false;
        bool show_app_fixed_overlay = false;
        bool show_app_manipulating_window_title = false;
        bool show_app_custom_rendering = false;
        bool show_app_style_editor = false;

        bool show_app_metrics = false;
        bool show_app_about = false;

        bool no_titlebar = false;
        bool no_border = true;
        bool no_resize = false;
        bool no_move = false;
        bool no_scrollbar = false;
        bool no_collapse = false;
        bool no_menu = false;

        //float wrap_width = 200.0f;        

        //int pressed_count = 0;

        //bool[] selected1 = { false, true, false, false };
        //bool[] selected2 = { false, false, false };
        //bool[] selected3 = { true, false, false, false, false, true, false, false, false, false, true, false, false, false, false, true };

        //bool a = false;
        //bool check = true;
        //int e = 0;
        //float[] arr = { 0.6f, 0.1f, 1.0f, 0.5f, 0.92f, 0.1f, 0.2f };
        //int item = 1;
        //int item2 = -1;

        //string[] items = { "AAAA", "BBBB", "CCCC", "DDDD", "EEEE", "FFFF", "GGGG", "HHHH", "IIII", "JJJJ", "KKKK" };
        //char[] str0 = null;
        //int i0 = 123;
        //float f0 = 0.001f;
        //float[] vec4a = { 0.10f, 0.20f, 0.30f, 0.44f };
        //int i1 = 50, i2 = 42;
        //float f1 = 1.00f, f2 = 0.0067f;
        //int i3 = 0;
        //float f3 = 0.123f, f4 = 0.0f;
        //float angle1 = 0.0f;
        //float[] col1 = { 1.0f, 0.0f, 0.2f };
        //float[] col2 = { 0.4f, 0.7f, 0.0f, 0.5f };
        //string[] listbox_items = { "Apple", "Banana", "Cherry", "Kiwi", "Mango", "Orange", "Pineapple", "Strawberry", "Watermelon" };
        //int listbox_item_current = 1;

        //float begin = 10, end = 90;
        //int begin_i = 100, end_i = 1000;
        //float[] vec4f = { 0.10f, 0.20f, 0.30f, 0.44f };
        //int[] vec4i = { 1, 5, 100, 255 };
        //int int_value = 0;
        //float[] values = { 0.0f, 0.60f, 0.35f, 0.9f, 0.70f, 0.20f, 0.0f };
        //float[] values2 = { 0.20f, 0.80f, 0.40f, 0.25f };
        //bool animate = true;
        //float[] arr2 = { 0.6f, 0.1f, 1.0f, 0.5f, 0.92f, 0.1f, 0.2f };
        ////float refresh_time = GetTime(); // Create dummy data at fixed 60 hz rate for the demo
        //int values_offset = 0;
        //float[] values5 = new float[90];
        //float phase = 0.0f;
        //int func_type = 0, display_count = 70;
        ////struct Funcs
        ////{
        ////    static float Sin(void*, int i) { return sinf(i * 0.1f); }
        ////    static float Saw(void*, int i) { return (i & 1) ? 1.0f : 0.0f; }
        ////};
        //float progress = 0.0f, progress_dir = 1.0f;
        //int line = 50;
        //float f = 0.0f;
        //bool c1 = false, c2 = false, c3 = false, c4 = false;
        //int[] selection = { 0, 1, 2, 3 };
        //float[] values6 = { 0.5f, 0.20f, 0.80f, 0.60f, 0.25f };
        //bool track = true;
        //int track_line = 50, scroll_to_px = 200;
        //ImVec2 size = new ImVec2(100, 100), offset = new ImVec2(50, 20);

        private Dictionary<string, Action> _codeBlocks = new Dictionary<string, Action>();

        // Demonstrate most ImGui features (big function!)
        internal void ShowTestWindow(ref bool p_opened)
        {
            // Examples apps
            //if (show_app_main_menu_bar) ShowExampleAppMainMenuBar();
            //if (show_app_console) ShowExampleAppConsole(ref show_app_console);
            //if (show_app_log) ShowExampleAppLog(ref show_app_log);
            //if (show_app_layout) ShowExampleAppLayout(ref show_app_layout);
            //if (show_app_property_editor) ShowExampleAppPropertyEditor(ref show_app_property_editor);
            //if (show_app_long_text) ShowExampleAppLongText(ref show_app_long_text);
            //if (show_app_auto_resize) ShowExampleAppAutoResize(ref show_app_auto_resize);
            //if (show_app_fixed_overlay) ShowExampleAppFixedOverlay(ref show_app_fixed_overlay);
            //if (show_app_manipulating_window_title) ShowExampleAppManipulatingWindowTitle(ref show_app_manipulating_window_title);
            //if (show_app_custom_rendering) ShowExampleAppCustomRendering(ref show_app_custom_rendering);

            if (show_app_metrics) ShowMetricsWindow(ref show_app_metrics);
            //if (show_app_style_editor) { Begin("Style Editor", ref show_app_style_editor); ShowStyleEditor(); End(); }
            if (show_app_about)
            {
                Begin("About ImGui", ref show_app_about, ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize);
                Text("dear imgui, {0}", GetVersion());
                Separator();
                Text("By Omar Cornut and all github contributors.");
                Text("ImGui is licensed under the MIT License, see LICENSE for more information.");
                End();
            }

            // Demonstrate the various window flags. Typically you would just use the default.
            ImGuiWindowFlags window_flags = 0;
            if (no_titlebar) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar;
            if (!no_border) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_ShowBorders;
            if (no_resize) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_NoResize;
            if (no_move) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_NoMove;
            if (no_scrollbar) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_NoScrollbar;
            if (no_collapse) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_NoCollapse;
            if (!no_menu) window_flags |= ImGuiWindowFlags.ImGuiWindowFlags_MenuBar;
            SetNextWindowSize(new ImVec2(550, 380), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
            if (!Begin("ImGui Demo", ref p_opened, window_flags))
            {
                // Early out if the window is collapsed, as an optimization.
                End();
                return;
            }

            //PushItemWidth(GetWindowWidth() * 0.65f);    // 2/3 of the space for widget and 1/3 for labels
            PushItemWidth(-140);                                 // Right align, keep 140 pixels for labels

            Text("Dear ImGui says hello.");

            #region Menu
            if (BeginMenuBar())
            {
                if (BeginMenu("Menu"))
                {
                    //TODO: ShowExampleMenuFile();
                    EndMenu();
                }
                if (BeginMenu("Examples"))
                {
                    MenuItem("Main menu bar", null, ref show_app_main_menu_bar);
                    MenuItem("Console", null, ref show_app_console);
                    MenuItem("Log", null, ref show_app_log);
                    MenuItem("Simple layout", null, ref show_app_layout);
                    MenuItem("Property editor", null, ref show_app_property_editor);
                    MenuItem("Long text display", null, ref show_app_long_text);
                    MenuItem("Auto-resizing window", null, ref show_app_auto_resize);
                    MenuItem("Simple overlay", null, ref show_app_fixed_overlay);
                    MenuItem("Manipulating window title", null, ref show_app_manipulating_window_title);
                    MenuItem("Custom rendering", null, ref show_app_custom_rendering);
                    EndMenu();
                }
                if (BeginMenu("Help"))
                {
                    MenuItem("Metrics", null, ref show_app_metrics);
                    MenuItem("Style Editor", null, ref show_app_style_editor);
                    MenuItem("About ImGui", null, ref show_app_about);
                    EndMenu();
                }
                EndMenuBar();
            }
            #endregion

            Spacing();

            #region Help
            if (CollapsingHeader("Help"))
            {
                TextWrapped("This window is being created by the ShowTestWindow() function. Please refer to the code for programming reference.\n\nUser Guide:");
                ShowUserGuide();
            }
            #endregion

            #region Window options
            if (CollapsingHeader("Window options"))
            {
                Checkbox("No titlebar", ref no_titlebar); SameLine(150);
                Checkbox("No border", ref no_border); SameLine(300);
                Checkbox("No resize", ref no_resize);
                Checkbox("No move", ref no_move); SameLine(150);
                Checkbox("No scrollbar", ref no_scrollbar); SameLine(300);
                Checkbox("No collapse", ref no_collapse);
                Checkbox("No menu", ref no_menu);

                if (TreeNode("Style"))
                {
                    ShowStyleEditor();
                    TreePop();
                }

                Action _fonts;

                if (!_codeBlocks.TryGetValue("windowoptions_fonts", out _fonts))
                {
                    float window_scale = 1.0f;
                    _codeBlocks["windowoptions_fonts"] = _fonts = new Action(() =>
                    {
                        if (TreeNode("Fonts", "Fonts ({0})", GetIO().Fonts.Fonts.Size))
                        {
                            SameLine(); ShowHelpMarker("Tip: Load fonts with io.Fonts.AddFontFromFileTTF()\nbefore calling io.Fonts.GetTex* functions.");
                            ImFontAtlas atlas = GetIO().Fonts;
                            if (TreeNode("Atlas texture", "Atlas texture (%dx%d pixels)", atlas.TexWidth, atlas.TexHeight))
                            {
                                Image(atlas.TexID, new ImVec2((float)atlas.TexWidth, (float)atlas.TexHeight), new ImVec2(0, 0), new ImVec2(1, 1), new ImColor(255, 255, 255, 255), new ImColor(255, 255, 255, 128));
                                TreePop();
                            }
                            PushItemWidth(100);
                            for (int i = 0; i < atlas.Fonts.Size; i++)
                            {
                                ImFont font = atlas.Fonts[i];
                                BulletText("Font {0}: \'{1}\', {2:0.00} px, {3} glyphs", i, font.ConfigData != null ? font.ConfigData.Name : "", font.FontSize, font.Glyphs.Size);
                                TreePush(i.ToString());
                                //TreePush((void*)(intptr_t)i);
                                if (i > 0) { SameLine(); if (SmallButton("Set as default")) { atlas.Fonts[i] = atlas.Fonts[0]; atlas.Fonts[0] = font; } }
                                PushFont(font);
                                Text("The quick brown fox jumps over the lazy dog");
                                PopFont();
                                if (TreeNode("Details"))
                                {
                                    DragFloat("font scale", ref font.Scale, 0.005f, 0.3f, 2.0f, "{0:0.0}");             // scale only this font
                                    Text("Ascent: {0}, Descent: {1}, Height: {2}", font.Ascent, font.Descent, font.Ascent - font.Descent);
                                    Text("Fallback character: '{0}' ({1})", font.FallbackChar, font.FallbackChar);
                                    //for (int config_i = 0; config_i < font.ConfigDataCount; config_i++)
                                    {
                                        var config_i = 0;
                                        ImFontConfig cfg = font.ConfigData;//[config_i];
                                        BulletText("Input {0}: \'{1}\'\nOversample: ({2},{3}), PixelSnapH: {4}", config_i, cfg.Name, cfg.OversampleH, cfg.OversampleV, cfg.PixelSnapH);
                                    }
                                    TreePop();
                                }
                                TreePop();
                            }
                            var io = GetIO();
                            DragFloat("this window scale", ref window_scale, 0.005f, 0.3f, 2.0f, "{0:0.0}");              // scale only this window
                            DragFloat("global scale", ref io.FontGlobalScale, 0.005f, 0.3f, 2.0f, "{0:0.0}"); // scale everything
                            PopItemWidth();
                            SetWindowFontScale(window_scale);
                            TreePop();
                        }
                    });
                }

                _fonts();

                if (TreeNode("Logging"))
                {
                    TextWrapped("The logging API redirects all text output so you can easily capture the content of a window or a block. Tree nodes can be automatically expanded. You can also call LogText() to output directly to the log without a visual output.");
                    LogButtons();
                    TreePop();
                }
            }
            #endregion

            //#region Widgets
            //if (CollapsingHeader("Widgets"))
            //{
            //    if (TreeNode("Tree"))
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            if (TreeNode(i.ToString(), "Child %d", i))
            //            {
            //                Text("blah blah");
            //                SameLine();
            //                if (SmallButton("print"))
            //                    System.Diagnostics.Debug.WriteLine("Child {0} pressed", i);
            //                TreePop();
            //            }
            //        }
            //        TreePop();
            //    }

            //    if (TreeNode("Bullets"))
            //    {
            //        BulletText("Bullet point 1");
            //        BulletText("Bullet point 2\nOn multiple lines");
            //        Bullet(); Text("Bullet point 3 (two calls)");
            //        Bullet(); SmallButton("Button");
            //        TreePop();
            //    }

            //    if (TreeNode("Colored Text"))
            //    {
            //        // Using shortcut. You can use PushStyleColor()/PopStyleColor() for more flexibility.
            //        TextColored(new ImVec4(1.0f, 0.0f, 1.0f, 1.0f), "Pink");
            //        TextColored(new ImVec4(1.0f, 1.0f, 0.0f, 1.0f), "Yellow");
            //        TextDisabled("Disabled");
            //        TreePop();
            //    }

            //    if (TreeNode("Word Wrapping"))
            //    {
            //        // Using shortcut. You can use PushTextWrapPos()/PopTextWrapPos() for more flexibility.
            //        TextWrapped("This text should automatically wrap on the edge of the window. The current implementation for text wrapping follows simple rules suitable for English and possibly other languages.");
            //        Spacing();

            //        SliderFloat("Wrap width", ref wrap_width, -20, 600, "%.0f");

            //        Text("Test paragraph 1:");
            //        ImVec2 pos = GetCursorScreenPos();
            //        GetWindowDrawList().AddRectFilled(new ImVec2(pos.x + wrap_width, pos.y), new ImVec2(pos.x + wrap_width + 10, pos.y + GetTextLineHeight()), 0xFFFF00FF);
            //        PushTextWrapPos(GetCursorPos().x + wrap_width);
            //        Text("lazy dog. This paragraph is made to fit within %.0f pixels. The quick brown fox jumps over the lazy dog.", wrap_width);
            //        GetWindowDrawList().AddRect(GetItemRectMin(), GetItemRectMax(), 0xFF00FFFF);
            //        PopTextWrapPos();

            //        Text("Test paragraph 2:");
            //        pos = GetCursorScreenPos();
            //        GetWindowDrawList().AddRectFilled(new ImVec2(pos.x + wrap_width, pos.y), new ImVec2(pos.x + wrap_width + 10, pos.y + GetTextLineHeight()), 0xFFFF00FF);
            //        PushTextWrapPos(GetCursorPos().x + wrap_width);
            //        Text("aaaaaaaa bbbbbbbb, cccccccc,dddddddd. eeeeeeee   ffffffff. gggggggg!hhhhhhhh");
            //        GetWindowDrawList().AddRect(GetItemRectMin(), GetItemRectMax(), 0xFF00FFFF);
            //        PopTextWrapPos();

            //        TreePop();
            //    }

            //    //TODO: UTF8
            //    //if (TreeNode("UTF-8 Text"))
            //    //{
            //    //    // UTF-8 test with Japanese characters
            //    //    // (needs a suitable font, try Arial Unicode or M+ fonts http://mplus-fonts.sourceforge.jp/mplus-outline-fonts/index-en.html)
            //    //    // Most compiler appears to support UTF-8 in source code (with Visual Studio you need to save your file as 'UTF-8 without signature')
            //    //    // However for the sake for maximum portability here we are *not* including raw UTF-8 character in this source file, instead we encode the string with hexadecimal constants.
            //    //    // In your own application be reasonable and use UTF-8 in source or retrieve the data from file system!
            //    //    // Note that characters values are preserved even if the font cannot be displayed, so you can safely copy & paste garbled characters into another application.
            //    //    TextWrapped("CJK text will only appears if the font was loaded with the appropriate CJK character ranges. Call io.Font.LoadFromFileTTF() manually to load extra character ranges.");
            //    //    Text("Hiragana: \xe3\x81\x8b\xe3\x81\x8d\xe3\x81\x8f\xe3\x81\x91\xe3\x81\x93 (kakikukeko)");
            //    //    Text("Kanjis: \xe6\x97\xa5\xe6\x9c\xac\xe8\xaa\x9e (nihongo)");
            //    //    static char buf[32] = "\xe6\x97\xa5\xe6\x9c\xac\xe8\xaa\x9e";
            //    //    InputText("UTF-8 input", buf, IM_ARRAYSIZE(buf));
            //    //    TreePop();
            //    //}

            //    if (TreeNode("Images"))
            //    {
            //        TextWrapped("Below we are displaying the font texture (which is the only texture we have access to in this demo). Use the 'ImTextureID' type as storage to pass pointers or identifier to your own texture data. Hover the texture for a zoomed view!");
            //        ImVec2 tex_screen_pos = GetCursorScreenPos();
            //        float tex_w = (float)GetIO().Fonts.TexWidth;
            //        float tex_h = (float)GetIO().Fonts.TexHeight;
            //        ImTextureID tex_id = GetIO().Fonts.TexID;
            //        Text("%.0fx%.0f", tex_w, tex_h);
            //        Image(tex_id, new ImVec2(tex_w, tex_h), new ImVec2(0, 0), new ImVec2(1, 1), new ImColor(255, 255, 255, 255), new ImColor(255, 255, 255, 128));
            //        if (IsItemHovered())
            //        {
            //            BeginTooltip();
            //            float focus_sz = 32.0f;
            //            float focus_x = GetMousePos().x - tex_screen_pos.x - focus_sz * 0.5f; if (focus_x < 0.0f) focus_x = 0.0f; else if (focus_x > tex_w - focus_sz) focus_x = tex_w - focus_sz;
            //            float focus_y = GetMousePos().y - tex_screen_pos.y - focus_sz * 0.5f; if (focus_y < 0.0f) focus_y = 0.0f; else if (focus_y > tex_h - focus_sz) focus_y = tex_h - focus_sz;
            //            Text("Min: (%.2f, %.2f)", focus_x, focus_y);
            //            Text("Max: (%.2f, %.2f)", focus_x + focus_sz, focus_y + focus_sz);
            //            ImVec2 uv0 = new ImVec2((focus_x) / tex_w, (focus_y) / tex_h);
            //            ImVec2 uv1 = new ImVec2((focus_x + focus_sz) / tex_w, (focus_y + focus_sz) / tex_h);
            //            Image(tex_id, new ImVec2(128, 128), uv0, uv1, new ImColor(255, 255, 255, 255), new ImColor(255, 255, 255, 128));
            //            EndTooltip();
            //        }
            //        TextWrapped("And now some textured buttons..");
            //        for (int i = 0; i < 8; i++)
            //        {
            //            if (i > 0)
            //                SameLine();
            //            PushID(i);
            //            int frame_padding = -1 + i;     // -1 = uses default padding
            //            if (ImageButton(tex_id, new ImVec2(32, 32), new ImVec2(0, 0), new ImVec2(32.0f / tex_w, 32 / tex_h), frame_padding, new ImColor(0, 0, 0, 255)))
            //                pressed_count += 1;
            //            PopID();
            //        }
            //        Text("Pressed %d times.", pressed_count);
            //        TreePop();
            //    }

            //    if (TreeNode("Selectables"))
            //    {
            //        if (TreeNode("Basic"))
            //        {
            //            Selectable("1. I am selectable", ref selected1[0]);
            //            Selectable("2. I am selectable", ref selected1[1]);
            //            Text("3. I am not selectable");
            //            Selectable("4. I am selectable", ref selected1[2]);
            //            if (Selectable("5. I am double clickable", ref selected1[3], ImGuiSelectableFlags.ImGuiSelectableFlags_AllowDoubleClick))
            //                if (IsMouseDoubleClicked(0))
            //                    selected1[3] = !selected1[3];
            //            TreePop();
            //        }
            //        if (TreeNode("Rendering more text into the same block"))
            //        {
            //            Selectable("main.c", ref selected2[0]); SameLine(300); Text(" 2,345 bytes");
            //            Selectable("Hello.cpp", ref selected2[1]); SameLine(300); Text("12,345 bytes");
            //            Selectable("Hello.h", ref selected2[2]); SameLine(300); Text(" 2,345 bytes");
            //            TreePop();
            //        }
            //        if (TreeNode("Grid"))
            //        {
            //            for (int i = 0; i < 16; i++)
            //            {
            //                PushID(i);
            //                if (Selectable("Me", ref selected3[i], 0, new ImVec2(50, 50)))
            //                {
            //                    int x = i % 4, y = i / 4;
            //                    if (x > 0) selected3[i - 1] ^= true;
            //                    if (x < 3) selected3[i + 1] ^= true;
            //                    if (y > 0) selected3[i - 4] ^= true;
            //                    if (y < 3) selected3[i + 4] ^= true;
            //                }
            //                if ((i % 4) < 3) SameLine();
            //                PopID();
            //            }
            //            TreePop();
            //        }
            //        TreePop();
            //    }

            //    //            //TODO: Demo Filtered Text Input
            //    //            if (TreeNode("Filtered Text Input"))
            //    //            {
            //    //                char buf1[64] = ""; InputText("default", buf1, 64);
            //    //                char buf2[64] = ""; InputText("decimal", buf2, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_CharsDecimal);
            //    //                char buf3[64] = ""; InputText("hexadecimal", buf3, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_CharsHexadecimal | ImGuiInputTextFlags.ImGuiInputTextFlags_CharsUppercase);
            //    //                char buf4[64] = ""; InputText("uppercase", buf4, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_CharsUppercase);
            //    //                char buf5[64] = ""; InputText("no blank", buf5, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_CharsNoBlank);
            //    //                struct TextFilters { static int FilterImGuiLetters(ImGuiTextEditCallbackData* data) { if (data.EventChar < 256 && strchr("imgui", (char)data.EventChar)) return 0; return 1; } };
            //    //    char buf6[64] = "";
            //    //    InputText("\"imgui\" letters", buf6, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_CallbackCharFilter, TextFilters::FilterImGuiLetters);

            //    //                Text("Password input");
            //    //    char bufpass[64] = "password123";
            //    //                InputText("password", bufpass, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_Password | ImGuiInputTextFlags.ImGuiInputTextFlags_CharsNoBlank);
            //    //                SameLine(); ShowHelpMarker("Display all characters as '*'.\nDisable clipboard cut and copy.\nDisable logging.\n");
            //    //                InputText("password (clear)", bufpass, 64, ImGuiInputTextFlags.ImGuiInputTextFlags_CharsNoBlank);

            //    //                TreePop();
            //    //}

            //    //TODO: Multi-line text input
            //    //if (TreeNode("Multi-line Text Input"))
            //    //{
            //    //    static bool read_only = false;
            //    //    static char text[1024 * 16] =
            //    //        "/*\n"
            //    //        " The Pentium F00F bug, shorthand for F0 0F C7 C8,\n"
            //    //        " the hexadecimal encoding of one offending instruction,\n"
            //    //        " more formally, the invalid operand with locked CMPXCHG8B\n"
            //    //        " instruction bug, is a design flaw in the majority of\n"
            //    //        " Intel Pentium, Pentium MMX, and Pentium OverDrive\n"
            //    //        " processors (all in the P5 microarchitecture).\n"
            //    //        "*/\n\n"
            //    //        "label:\n"
            //    //        "\tlock cmpxchg8b eax\n";

            //    //    PushStyleVar(ImGuiStyleVar_FramePadding, new ImVec2(0, 0));
            //    //    Checkbox("Read-only", &read_only);
            //    //    PopStyleVar();
            //    //    InputTextMultiline("##source", text, IM_ARRAYSIZE(text), new ImVec2(-1.0f, GetTextLineHeight() * 16), ImGuiInputTextFlags_AllowTabInput | (read_only ? ImGuiInputTextFlags_ReadOnly : 0));
            //    //    TreePop();
            //    //}

            //    if (Button("Button")) { System.Diagnostics.Debug.Write("Clicked\n"); a ^= true; }
            //    if (a)
            //    {
            //        SameLine();
            //        Text("Thanks for clicking me!");
            //    }



            //    Checkbox("checkbox", ref check);

            //    RadioButton("radio a", ref e, 0); SameLine();
            //    RadioButton("radio b", ref e, 1); SameLine();
            //    RadioButton("radio c", ref e, 2);

            //    // Color buttons, demonstrate using PushID() to add unique identifier in the ID stack, and changing style.
            //    for (int i = 0; i < 7; i++)
            //    {
            //        if (i > 0) SameLine();
            //        PushID(i);
            //        PushStyleColor(ImGuiCol.ImGuiCol_Button, ImColor.HSV(i / 7.0f, 0.6f, 0.6f));
            //        PushStyleColor(ImGuiCol.ImGuiCol_ButtonHovered, ImColor.HSV(i / 7.0f, 0.7f, 0.7f));
            //        PushStyleColor(ImGuiCol.ImGuiCol_ButtonActive, ImColor.HSV(i / 7.0f, 0.8f, 0.8f));
            //        Button("Click");
            //        PopStyleColor(3);
            //        PopID();
            //    }

            //    Text("Hover over me");
            //    if (IsItemHovered())
            //        SetTooltip("I am a tooltip");

            //    SameLine();
            //    Text("- or me");
            //    if (IsItemHovered())
            //    {
            //        BeginTooltip();
            //        Text("I am a fancy tooltip");
            //        PlotLines("Curve", arr, arr.Length);
            //        EndTooltip();
            //    }

            //    // Testing IMGUI_ONCE_UPON_A_FRAME macro
            //    //for (int i = 0; i < 5; i++)
            //    //{
            //    //  IMGUI_ONCE_UPON_A_FRAME
            //    //  {
            //    //      Text("This will be displayed only once.");
            //    //  }
            //    //}

            //    Separator();

            //    LabelText("label", "Value");

            //    Combo("combo", ref item, new string[] { "aaaa", "bbbb", "cccc", "dddd", "eeee" });   // Combo using values packed in a single constant string (for really quick combo)


            //    Combo("combo scroll", ref item2, items);   // Combo using proper array. You can also pass a callback to retrieve array value, no need to create/copy an array just for that.

            //    {
            //        if (str0 == null)
            //        {
            //            str0 = new char[128];
            //            var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes("Hello World!");
            //            for (var i = 0; i < bytes.Length; i++)
            //                str0[i] = (char)bytes[i];
            //        }

            //        InputText("input text", str0, str0.Length);
            //        SameLine(); ShowHelpMarker("Hold SHIFT or use mouse to select text.\nCTRL+Left/Right to word jump.\nCTRL+A or double-click to select all.\nCTRL+X,CTRL+C,CTRL+V clipboard.\nCTRL+Z,CTRL+Y undo/redo.\nESCAPE to revert.\n");

            //        InputInt("input int", ref i0);
            //        SameLine(); ShowHelpMarker("You can apply arithmetic operators +,*,/ on numerical values.\n  e.g. [ 100 ], input \'*2\', result becomes [ 200 ]\nUse +- to subtract.\n");

            //        InputFloat("input float", ref f0, 0.01f, 1.0f);

            //        InputFloat3("input float3", vec4a);
            //    }

            //    {
            //        DragInt("drag int", ref i1, 1);
            //        SameLine(); ShowHelpMarker("Click and drag to edit value.\nHold SHIFT/ALT for faster/slower edit.\nDouble-click or CTRL+click to input value.");

            //        DragInt("drag int 0..100", ref i2, 1, 0, 100, "{0:0}%");

            //        DragFloat("drag float", ref f1, 0.005f);
            //        DragFloat("drag small float", ref f2, 0.0001f, 0.0f, 0.0f, "{0:0.000000} ns");
            //    }

            //    {
            //        SliderInt("slider int", ref i3, -1, 3);
            //        SameLine(); ShowHelpMarker("CTRL+click to input value.");


            //        SliderFloat("slider float", ref f3, 0.0f, 1.0f, "ratio = {0:0.000}");
            //        SliderFloat("slider log float", ref f4, -10.0f, 10.0f, "{0:0.0000}", 3.0f);
            //        SliderAngle("slider angle", ref angle1);
            //    }

            //    //ColorEdit3("color 1", col1);
            //    //SameLine(); ShowHelpMarker("Click on the colored square to change edit mode.\nCTRL+click on individual component to input value.\n");

            //    //ColorEdit4("color 2", col2);

            //    ListBox("listbox\n(single select)", ref listbox_item_current, listbox_items, 4);

            //    //static int listbox_item_current2 = 2;
            //    //PushItemWidth(-1);
            //    //ListBox("##listbox2", &listbox_item_current2, listbox_items, IM_ARRAYSIZE(listbox_items), 4);
            //    //PopItemWidth();

            //    if (TreeNode("Range Widgets"))
            //    {
            //        Unindent();

            //        DragFloatRange2("range", ref begin, ref end, 0.25f, 0.0f, 100.0f, "Min: {0:0.0} %", "Max: {0:0.0} %");
            //        DragIntRange2("range int (no bounds)", ref begin_i, ref end_i, 5, 0, 0, "Min: {0:0} units", "Max: {0:0} units");

            //        Indent();
            //        TreePop();
            //    }

            //    if (TreeNode("Multi-component Widgets"))
            //    {
            //        Unindent();


            //        InputFloat2("input float2", vec4f);
            //        DragFloat2("drag float2", vec4f, 0.01f, 0.0f, 1.0f);
            //        SliderFloat2("slider float2", vec4f, 0.0f, 1.0f);
            //        DragInt2("drag int2", vec4i, 1, 0, 255);
            //        InputInt2("input int2", vec4i);
            //        SliderInt2("slider int2", vec4i, 0, 255);
            //        Spacing();

            //        InputFloat3("input float3", vec4f);
            //        DragFloat3("drag float3", vec4f, 0.01f, 0.0f, 1.0f);
            //        SliderFloat3("slider float3", vec4f, 0.0f, 1.0f);
            //        DragInt3("drag int3", vec4i, 1, 0, 255);
            //        InputInt3("input int3", vec4i);
            //        SliderInt3("slider int3", vec4i, 0, 255);
            //        Spacing();

            //        InputFloat4("input float4", vec4f);
            //        DragFloat4("drag float4", vec4f, 0.01f, 0.0f, 1.0f);
            //        SliderFloat4("slider float4", vec4f, 0.0f, 1.0f);
            //        InputInt4("input int4", vec4i);
            //        DragInt4("drag int4", vec4i, 1, 0, 255);
            //        SliderInt4("slider int4", vec4i, 0, 255);

            //        Indent();
            //        TreePop();
            //    }

            //    if (TreeNode("Vertical Sliders"))
            //    {
            //        Unindent();
            //        float spacing = 4;
            //        PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_ItemSpacing, new ImVec2(spacing, spacing));

            //        VSliderInt("##int", new ImVec2(18, 160), ref int_value, 0, 5);
            //        SameLine();

            //        PushID("set1");
            //        for (int i = 0; i < 7; i++)
            //        {
            //            if (i > 0) SameLine();
            //            PushID(i);
            //            PushStyleColor(ImGuiCol.ImGuiCol_FrameBg, ImColor.HSV(i / 7.0f, 0.5f, 0.5f));
            //            PushStyleColor(ImGuiCol.ImGuiCol_FrameBgHovered, ImColor.HSV(i / 7.0f, 0.6f, 0.5f));
            //            PushStyleColor(ImGuiCol.ImGuiCol_FrameBgActive, ImColor.HSV(i / 7.0f, 0.7f, 0.5f));
            //            PushStyleColor(ImGuiCol.ImGuiCol_SliderGrab, ImColor.HSV(i / 7.0f, 0.9f, 0.9f));
            //            VSliderFloat("##v", new ImVec2(18, 160), ref values[i], 0.0f, 1.0f, "");
            //            if (IsItemActive() || IsItemHovered())
            //                SetTooltip("{0:0.000}", values[i]);
            //            PopStyleColor(4);
            //            PopID();
            //        }
            //        PopID();

            //        SameLine();
            //        PushID("set2");
            //        int rows = 3;
            //        ImVec2 small_slider_size = new ImVec2(18, (160.0f - (rows - 1) * spacing) / rows);
            //        for (int nx = 0; nx < 4; nx++)
            //        {
            //            if (nx > 0) SameLine();
            //            BeginGroup();
            //            for (int ny = 0; ny < rows; ny++)
            //            {
            //                PushID(nx * rows + ny);
            //                VSliderFloat("##v", small_slider_size, ref values2[nx], 0.0f, 1.0f, "");
            //                if (IsItemActive() || IsItemHovered())
            //                    SetTooltip("{0:0.000}", values2[nx]);
            //                PopID();
            //            }
            //            EndGroup();
            //        }
            //        PopID();

            //        SameLine();
            //        PushID("set3");
            //        for (int i = 0; i < 4; i++)
            //        {
            //            if (i > 0) SameLine();
            //            PushID(i);
            //            PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_GrabMinSize, 40);
            //            VSliderFloat("##v", new ImVec2(40, 160), ref values[i], 0.0f, 1.0f, "{0:0.00}");
            //            PopStyleVar();
            //            PopID();
            //        }
            //        PopID();
            //        PopStyleVar();

            //        Indent();
            //        TreePop();
            //    }
            //}
            //#endregion

            #region Graphs
            //if (CollapsingHeader("Graphs widgets"))
            //{
            //    Checkbox("Animate", ref animate);

            //    PlotLines("Frame Times", arr2, arr2.Length);

            //    // Create a dummy array of contiguous float values to plot
            //    // Tip: If your float aren't contiguous but part of a structure, you can pass a pointer to your first float and the sizeof() of your structure in the Stride parameter.
            //    if (animate)
            //    {
            //        for (; GetTime() > refresh_time + 1.0f / 60.0f; refresh_time += 1.0f / 60.0f)
            //        {
            //            values5[values_offset] = cosf(phase);
            //            values_offset = (values_offset + 1) % values5.Length;
            //            phase += 0.10f * values_offset;
            //        }
            //    }
            //    PlotLines("Lines", values5, values5.Length, values_offset, "avg 0.0", -1.0f, 1.0f, new ImVec2(0, 80));
            //    PlotHistogram("Histogram", arr2, arr2.Length, 0, null, 0.0f, 1.0f, new ImVec2(0, 80));

            //    // Use functions to generate output
            //    // FIXME: This is rather awkward because current plot API only pass in indices. We probably want an API passing floats and user provide sample rate/count.

            //    //TODO: GRAPH
            //    //Separator();
            //    //PushItemWidth(100); Combo("func", ref func_type, "Sin\0Saw\0"); PopItemWidth();
            //    //SameLine();
            //    //SliderInt("Sample count", ref display_count, 1, 500);
            //    //float (* func)(void*, int) = (func_type == 0) ? Funcs::Sin : Funcs::Saw;
            //    //PlotLines("Lines", func, null, display_count, 0, null, -1.0f, 1.0f, new ImVec2(0,80));
            //    //PlotHistogram("Histogram", func, null, display_count, 0, null, -1.0f, 1.0f, new ImVec2(0,80));
            //    //Separator();

            //    // Animate a simple progress bar
            //    if (animate)
            //    {
            //        progress += progress_dir * 0.4f * GetIO().DeltaTime;
            //        if (progress >= +1.1f) { progress = +1.1f; progress_dir *= -1.0f; }
            //        if (progress <= -0.1f) { progress = -0.1f; progress_dir *= -1.0f; }
            //    }

            //    // Typically we would use new ImVec2(-1.0f,0.0f) to use all available width, or new ImVec2(width,0.0f) for a specified width. new ImVec2(0.0f,0.0f) uses ItemWidth.
            //    ProgressBar(progress, new ImVec2(0.0f, 0.0f));
            //    SameLine(0.0f, GetStyle().ItemInnerSpacing.x);
            //    Text("Progress Bar");

            //    //float progress_saturated = (progress < 0.0f) ? 0.0f : (progress > 1.0f) ? 1.0f : progress;
            //    //char buf[32];
            //    //sprintf(buf, "%d/%d", (int)(progress_saturated * 1753), 1753);
            //    //ProgressBar(progress, new ImVec2(0.f, 0.f), buf);
            //}
            #endregion

            #region Layout
            //if (CollapsingHeader("Layout"))
            //{
            //    if (TreeNode("Child regions"))
            //    {
            //        Text("Without border");
            //        bool goto_line = Button("Goto");
            //        SameLine();
            //        PushItemWidth(100);
            //        goto_line |= InputInt("##Line", ref line, 0, 0, ImGuiInputTextFlags.ImGuiInputTextFlags_EnterReturnsTrue);
            //        PopItemWidth();
            //        BeginChild("Sub1", new ImVec2(GetWindowContentRegionWidth() * 0.5f, 300), false, ImGuiWindowFlags.ImGuiWindowFlags_HorizontalScrollbar);
            //        for (int i = 0; i < 100; i++)
            //        {
            //            Text("%04d: scrollable region", i);
            //            if (goto_line && line == i)
            //                SetScrollHere();
            //        }
            //        if (goto_line && line >= 100)
            //            SetScrollHere();
            //        EndChild();

            //        SameLine();

            //        PushStyleVar(ImGuiStyleVar.ImGuiStyleVar_ChildWindowRounding, 5.0f);
            //        BeginChild("Sub2", new ImVec2(0, 300), true);
            //        Text("With border");
            //        Columns(2);
            //        for (int i = 0; i < 100; i++)
            //        {
            //            if (i == 50)
            //                NextColumn();
            //            char buf[32];
            //            sprintf(buf, "%08x", i * 5731);
            //            Button(buf, new ImVec2(-1.0f, 0.0f));
            //        }
            //        EndChild();
            //        PopStyleVar();

            //        TreePop();
            //    }

            //    if (TreeNode("Widgets Width"))
            //    {
            //        Text("PushItemWidth(100)");
            //        SameLine(); ShowHelpMarker("Fixed width.");
            //        PushItemWidth(100);
            //        DragFloat("float##1", ref f);
            //        PopItemWidth();

            //        Text("PushItemWidth(GetWindowWidth() * 0.5f)");
            //        SameLine(); ShowHelpMarker("Half of window width.");
            //        PushItemWidth(GetWindowWidth() * 0.5f);
            //        DragFloat("float##2", ref f);
            //        PopItemWidth();

            //        Text("PushItemWidth(GetContentRegionAvailWidth() * 0.5f)");
            //        SameLine(); ShowHelpMarker("Half of available width.\n(~ right-cursor_pos)\n(works within a column set)");
            //        PushItemWidth(GetContentRegionAvailWidth() * 0.5f);
            //        DragFloat("float##3", ref f);
            //        PopItemWidth();

            //        Text("PushItemWidth(-100)");
            //        SameLine(); ShowHelpMarker("Align to right edge minus 100");
            //        PushItemWidth(-100);
            //        DragFloat("float##4", ref f);
            //        PopItemWidth();

            //        Text("PushItemWidth(-1)");
            //        SameLine(); ShowHelpMarker("Align to right edge");
            //        PushItemWidth(-1);
            //        DragFloat("float##5", ref f);
            //        PopItemWidth();

            //        TreePop();
            //    }

            //    if (TreeNode("Basic Horizontal Layout"))
            //    {
            //        TextWrapped("(Use SameLine() to keep adding items to the right of the preceeding item)");

            //        // Text
            //        Text("Two items: Hello"); SameLine();
            //        TextColored(new ImVec4(1, 1, 0, 1), "Sailor");

            //        // Adjust spacing
            //        Text("More spacing: Hello"); SameLine(0, 20);
            //        TextColored(new ImVec4(1, 1, 0, 1), "Sailor");

            //        // Button
            //        AlignFirstTextHeightToWidgets();
            //        Text("Normal buttons"); SameLine();
            //        Button("Banana"); SameLine();
            //        Button("Apple"); SameLine();
            //        Button("Corniflower");

            //        // Button
            //        Text("Small buttons"); SameLine();
            //        SmallButton("Like this one"); SameLine();
            //        Text("can fit within a text block.");

            //        // Aligned to arbitrary position. Easy/cheap column.
            //        Text("Aligned");
            //        SameLine(150); Text("x=150");
            //        SameLine(300); Text("x=300");
            //        Text("Aligned");
            //        SameLine(150); SmallButton("x=150");
            //        SameLine(300); SmallButton("x=300");

            //        // Checkbox
            //        Checkbox("My", ref c1); SameLine();
            //        Checkbox("Tailor", ref c2); SameLine();
            //        Checkbox("Is", ref c3); SameLine();
            //        Checkbox("Rich", ref c4);

            //        //TODO: Various
            //        //static float f0 = 1.0f, f1 = 2.0f, f2 = 3.0f;
            //        //PushItemWidth(80);
            //        //char* items[] = { "AAAA", "BBBB", "CCCC", "DDDD" };
            //        //static int item = -1;
            //        //Combo("Combo", &item, items, IM_ARRAYSIZE(items)); SameLine();
            //        //SliderFloat("X", &f0, 0.0f, 5.0f); SameLine();
            //        //SliderFloat("Y", &f1, 0.0f, 5.0f); SameLine();
            //        //SliderFloat("Z", &f2, 0.0f, 5.0f);
            //        //PopItemWidth();

            //        PushItemWidth(80);
            //        Text("Lists:");
            //        for (int i = 0; i < 4; i++)
            //        {
            //            if (i > 0) SameLine();
            //            PushID(i);
            //            ListBox("", ref selection[i], items, items.Length);
            //            PopID();
            //            //if (IsItemHovered()) SetTooltip("ListBox %d hovered", i);
            //        }
            //        PopItemWidth();

            //        // Dummy
            //        ImVec2 sz = new ImVec2(30, 30);
            //        Button("A", sz); SameLine();
            //        Dummy(sz); SameLine();
            //        Button("B", sz);

            //        TreePop();
            //    }

            //    if (TreeNode("Groups"))
            //    {
            //        TextWrapped("(Using BeginGroup()/EndGroup() to layout items. BeginGroup() basically locks the horizontal position. EndGroup() bundles the whole group so that you can use functions such as IsItemHovered() on it.)");
            //        BeginGroup();
            //        {
            //            BeginGroup();
            //            Button("AAA");
            //            SameLine();
            //            Button("BBB");
            //            SameLine();
            //            BeginGroup();
            //            Button("CCC");
            //            Button("DDD");
            //            EndGroup();
            //            if (IsItemHovered())
            //                SetTooltip("Group hovered");
            //            SameLine();
            //            Button("EEE");
            //            EndGroup();
            //        }
            //        // Capture the group size and create widgets using the same size
            //        ImVec2 size = GetItemRectSize();
            //        PlotHistogram("##values", values6, values6.Length, 0, null, 0.0f, 1.0f, size);

            //        Button("ACTION", new ImVec2((size.x - GetStyle().ItemSpacing.x) * 0.5f, size.y));
            //        SameLine();
            //        Button("REACTION", new ImVec2((size.x - GetStyle().ItemSpacing.x) * 0.5f, size.y));
            //        EndGroup();
            //        SameLine();

            //        Button("LEVERAGE\nBUZZWORD", size);
            //        SameLine();

            //        ListBoxHeader("List", size);
            //        Selectable("Selected", true);
            //        Selectable("Not Selected", false);
            //        ListBoxFooter();

            //        TreePop();
            //    }

            //    if (TreeNode("Text Baseline Alignment"))
            //    {
            //        TextWrapped("(This is testing the vertical alignment that occurs on text to keep it at the same baseline as widgets. Lines only composed of text or \"small\" widgets fit in less vertical spaces than lines with normal widgets)");

            //        Text("One\nTwo\nThree"); SameLine();
            //        Text("Hello\nWorld"); SameLine();
            //        Text("Banana");

            //        Text("Banana"); SameLine();
            //        Text("Hello\nWorld"); SameLine();
            //        Text("One\nTwo\nThree");

            //        Button("HOP"); SameLine();
            //        Text("Banana"); SameLine();
            //        Text("Hello\nWorld"); SameLine();
            //        Text("Banana");

            //        Button("HOP"); SameLine();
            //        Text("Hello\nWorld"); SameLine();
            //        Text("Banana");

            //        Button("TEST"); SameLine();
            //        Text("TEST"); SameLine();
            //        SmallButton("TEST");

            //        AlignFirstTextHeightToWidgets(); // If your line starts with text, call this to align it to upcoming widgets.
            //        Text("Text aligned to Widget"); SameLine();
            //        Button("Widget"); SameLine();
            //        Text("Widget"); SameLine();
            //        SmallButton("Widget");

            //        // Tree
            //        float spacing = GetStyle().ItemInnerSpacing.x;
            //        Button("Button##1");
            //        SameLine(0.0f, spacing);
            //        if (TreeNode("Node##1")) { for (int i = 0; i < 6; i++) BulletText("Item %d..", i); TreePop(); }    // Dummy tree data

            //        AlignFirstTextHeightToWidgets();         // Vertically align text node a bit lower so it'll be vertically centered with upcoming widget. Otherwise you can use SmallButton (smaller fit).
            //        bool tree_opened = TreeNode("Node##2");  // Common mistake to avoid: if we want to SameLine after TreeNode we need to do it before we add child content.
            //        SameLine(0.0f, spacing); Button("Button##2");
            //        if (tree_opened) { for (int i = 0; i < 6; i++) BulletText("Item %d..", i); TreePop(); }   // Dummy tree data

            //        // Bullet
            //        Button("Button##3");
            //        SameLine(0.0f, spacing);
            //        BulletText("Bullet text");

            //        AlignFirstTextHeightToWidgets();
            //        BulletText("Node");
            //        SameLine(0.0f, spacing); Button("Button##4");

            //        TreePop();
            //    }

            //    //if (TreeNode("Scrolling"))
            //    //{
            //    //    TextWrapped("(Use SetScrollHere() or SetScrollFromPosY() to scroll to a given position.)");
            //    //    Checkbox("Track", ref track);
            //    //    SameLine(130); track |= DragInt("##line", ref track_line, 0.25f, 0, 99, "Line %.0f");
            //    //    bool scroll_to = Button("Scroll To");
            //    //    SameLine(130); scroll_to |= DragInt("##pos_y", ref scroll_to_px, 1.00f, 0, 9999, "y = %.0f px");
            //    //    if (scroll_to) track = false;

            //    //    for (int i = 0; i < 5; i++)
            //    //    {
            //    //        if (i > 0) SameLine();
            //    //        BeginGroup();
            //    //        Text("%s", i == 0 ? "Top" : i == 1 ? "25%" : i == 2 ? "Center" : i == 3 ? "75%" : "Bottom");
            //    //        BeginChild(GetID((void*)(intptr_t)i), new ImVec2(GetWindowWidth() * 0.17f, 200.0f), true);
            //    //        if (scroll_to)
            //    //            SetScrollFromPosY(GetCursorStartPos().y + scroll_to_px, i * 0.25f);
            //    //        for (int line = 0; line < 100; line++)
            //    //        {
            //    //            if (track && line == track_line)
            //    //            {
            //    //                TextColored(ImColor(255, 255, 0), "Line %d", line);
            //    //                SetScrollHere(i * 0.25f); // 0.0f:top, 0.5f:center, 1.0f:bottom
            //    //            }
            //    //            else
            //    //            {
            //    //                Text("Line %d", line);
            //    //            }
            //    //        }
            //    //        EndChild();
            //    //        EndGroup();
            //    //    }
            //    //    TreePop();
            //    //}

            //    //if (TreeNode("Horizontal Scrolling"))
            //    //{
            //    //    Bullet(); TextWrapped("Horizontal scrolling for a window has to be enabled explicitly via the ImGuiWindowFlags.ImGuiWindowFlags_HorizontalScrollbar flag.");
            //    //    Bullet(); TextWrapped("You may want to explicitly specify content width by calling SetNextWindowContentWidth() before Begin().");
            //    //    //static int lines = 7;
            //    //    SliderInt("Lines", &lines, 1, 15);
            //    //    PushStyleVar(ImGuiStyleVar_FrameRounding, 3.0f);
            //    //    PushStyleVar(ImGuiStyleVar_FramePadding, new ImVec2(2.0f, 1.0f));
            //    //    BeginChild("scrolling", new ImVec2(0, GetItemsLineHeightWithSpacing() * 7 + 30), true, ImGuiWindowFlags.ImGuiWindowFlags_HorizontalScrollbar);
            //    //    for (int line = 0; line < lines; line++)
            //    //    {
            //    //        // Display random stuff
            //    //        int num_buttons = 10 + ((line & 1) ? line * 9 : line * 3);
            //    //        for (int n = 0; n < num_buttons; n++)
            //    //        {
            //    //            if (n > 0) SameLine();
            //    //            PushID(n + line * 1000);
            //    //            char num_buf[16];
            //    //            char* label = (!(n % 15)) ? "FizzBuzz" : (!(n % 3)) ? "Fizz" : (!(n % 5)) ? "Buzz" : (sprintf(num_buf, "%d", n), num_buf);
            //    //            float hue = n * 0.05f;
            //    //            PushStyleColor(ImGuiCol_Button, ImColor::HSV(hue, 0.6f, 0.6f));
            //    //            PushStyleColor(ImGuiCol_ButtonHovered, ImColor::HSV(hue, 0.7f, 0.7f));
            //    //            PushStyleColor(ImGuiCol_ButtonActive, ImColor::HSV(hue, 0.8f, 0.8f));
            //    //            Button(label, new ImVec2(40.0f + sinf((float)(line + n)) * 20.0f, 0.0f));
            //    //            PopStyleColor(3);
            //    //            PopID();
            //    //        }
            //    //    }

            //    //    EndChild();
            //    //    PopStyleVar(2);
            //    //    float scroll_x_delta = 0.0f;
            //    //    SmallButton("<<"); if (IsItemActive()) scroll_x_delta = -GetIO().DeltaTime * 1000.0f;
            //    //    SameLine(); Text("Scroll from code"); SameLine();
            //    //    SmallButton(">>"); if (IsItemActive()) scroll_x_delta = +GetIO().DeltaTime * 1000.0f;
            //    //    if (scroll_x_delta != 0.0f)
            //    //    {
            //    //        BeginChild("scrolling"); // Demonstrate a trick: you can use Begin to set yourself in the context of another window (here we are already out of your child window)
            //    //        SetScrollX(GetScrollX() + scroll_x_delta);
            //    //        End();
            //    //    }
            //    //    TreePop();
            //    //}

            //    if (TreeNode("Clipping"))
            //    {
            //        TextWrapped("On a per-widget basis we are occasionally clipping text CPU-side if it won't fit in its frame. Otherwise we are doing coarser clipping + passing a scissor rectangle to the renderer. The system is designed to try minimizing both execution and CPU/GPU rendering cost.");
            //        DragFloat2("size", (float*)&size, 0.5f, 0.0f, 200.0f, "%.0f");
            //        TextWrapped("(Click and drag)");
            //        ImVec2 pos = GetCursorScreenPos();
            //        ImVec4 clip_rect(pos.x, pos.y, pos.x + size.x, pos.y + size.y);
            //        InvisibleButton("##dummy", size);
            //        if (IsItemActive() && IsMouseDragging()) { offset.x += GetIO().MouseDelta.x; offset.y += GetIO().MouseDelta.y; }
            //        GetWindowDrawList().AddRectFilled(pos, new ImVec2(pos.x + size.x, pos.y + size.y), ImColor(90, 90, 120, 255));
            //        GetWindowDrawList().AddText(GetFont(), GetFontSize() * 2.0f, new ImVec2(pos.x + offset.x, pos.y + offset.y), ImColor(255, 255, 255, 255), "Line 1 hello\nLine 2 clip me!", null, 0.0f, &clip_rect);
            //        TreePop();
            //    }
            //}
            #endregion

            #region Popups and Modals
            //if (CollapsingHeader("Popups & Modal windows"))
            //{
            //    if (TreeNode("Popups"))
            //    {
            //        TextWrapped("When a popup is active, it inhibits interacting with windows that are behind the popup. Clicking outside the popup closes it.");

            //        static int selected_fish = -1;
            //        char* names[] = { "Bream", "Haddock", "Mackerel", "Pollock", "Tilefish" };
            //        static bool toggles[] = { true, false, false, false, false };

            //        if (Button("Select.."))
            //            OpenPopup("select");
            //        SameLine();
            //        Text(selected_fish == -1 ? "<None>" : names[selected_fish]);
            //        if (BeginPopup("select"))
            //        {
            //            Text("Aquarium");
            //            Separator();
            //            for (int i = 0; i < IM_ARRAYSIZE(names); i++)
            //                if (Selectable(names[i]))
            //                    selected_fish = i;
            //            EndPopup();
            //        }

            //        if (Button("Toggle.."))
            //            OpenPopup("toggle");
            //        if (BeginPopup("toggle"))
            //        {
            //            for (int i = 0; i < IM_ARRAYSIZE(names); i++)
            //                MenuItem(names[i], "", &toggles[i]);
            //            if (BeginMenu("Sub-menu"))
            //            {
            //                MenuItem("Click me");
            //                EndMenu();
            //            }

            //            Separator();
            //            Text("Tooltip here");
            //            if (IsItemHovered())
            //                SetTooltip("I am a tooltip over a popup");

            //            if (Button("Stacked Popup"))
            //                OpenPopup("another popup");
            //            if (BeginPopup("another popup"))
            //            {
            //                for (int i = 0; i < IM_ARRAYSIZE(names); i++)
            //                    MenuItem(names[i], "", &toggles[i]);
            //                if (BeginMenu("Sub-menu"))
            //                {
            //                    MenuItem("Click me");
            //                    EndMenu();
            //                }
            //                EndPopup();
            //            }
            //            EndPopup();
            //        }

            //        if (Button("Popup Menu.."))
            //            OpenPopup("popup from button");
            //        if (BeginPopup("popup from button"))
            //        {
            //            ShowExampleMenuFile();
            //            EndPopup();
            //        }

            //        Spacing();
            //        TextWrapped("Below we are testing adding menu items to a regular window. It's rather unusual but should work!");
            //        Separator();
            //        // NB: As a quirk in this very specific example, we want to differentiate the parent of this menu from the parent of the various popup menus above.
            //        // To do so we are encloding the items in a PushID()/PopID() block to make them two different menusets. If we don't, opening any popup above and hovering our menu here
            //        // would open it. This is because once a menu is active, we allow to switch to a sibling menu by just hovering on it, which is the desired behavior for regular menus.
            //        PushID("foo");
            //        MenuItem("Menu item", "CTRL+M");
            //        if (BeginMenu("Menu inside a regular window"))
            //        {
            //            ShowExampleMenuFile();
            //            EndMenu();
            //        }
            //        PopID();
            //        Separator();

            //        TreePop();
            //    }

            //    if (TreeNode("Context menus"))
            //    {
            //        static float value = 0.5f;
            //        Text("Value = %.3f (<-- right-click here)", value);
            //        if (BeginPopupContextItem("item context menu"))
            //        {
            //            if (Selectable("Set to zero")) value = 0.0f;
            //            if (Selectable("Set to PI")) value = 3.1415f;
            //            EndPopup();
            //        }

            //        static ImVec4 color = ImColor(1.0f, 0.0f, 1.0f, 1.0f);
            //        ColorButton(color);
            //        if (BeginPopupContextItem("color context menu"))
            //        {
            //            Text("Edit color");
            //            ColorEdit3("##edit", (float*)&color);
            //            if (Button("Close"))
            //                CloseCurrentPopup();
            //            EndPopup();
            //        }
            //        SameLine(); Text("(<-- right-click here)");

            //        TreePop();
            //    }

            //    if (TreeNode("Modals"))
            //    {
            //        TextWrapped("Modal windows are like popups but the user cannot close them by clicking outside the window.");

            //        if (Button("Delete.."))
            //            OpenPopup("Delete?");
            //        if (BeginPopupModal("Delete?", null, ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize))
            //        {
            //            Text("All those beautiful files will be deleted.\nThis operation cannot be undone!\n\n");
            //            Separator();

            //            static bool dont_ask_me_next_time = false;
            //            PushStyleVar(ImGuiStyleVar_FramePadding, new ImVec2(0, 0));
            //            Checkbox("Don't ask me next time", &dont_ask_me_next_time);
            //            PopStyleVar();

            //            if (Button("OK", new ImVec2(120, 0))) { CloseCurrentPopup(); }
            //            SameLine();
            //            if (Button("Cancel", new ImVec2(120, 0))) { CloseCurrentPopup(); }
            //            EndPopup();
            //        }

            //        if (Button("Stacked modals.."))
            //            OpenPopup("Stacked 1");
            //        if (BeginPopupModal("Stacked 1"))
            //        {
            //            Text("Hello from Stacked The First");

            //            if (Button("Another one.."))
            //                OpenPopup("Stacked 2");
            //            if (BeginPopupModal("Stacked 2"))
            //            {
            //                Text("Hello from Stacked The Second");
            //                if (Button("Close"))
            //                    CloseCurrentPopup();
            //                EndPopup();
            //            }

            //            if (Button("Close"))
            //                CloseCurrentPopup();
            //            EndPopup();
            //        }

            //        TreePop();
            //    }
            //}
            #endregion

            #region Columns
            //if (CollapsingHeader("Columns"))
            //{
            //    // Basic columns
            //    if (TreeNode("Basic"))
            //    {
            //        Columns(4, "mycolumns");
            //        Separator();
            //        Text("ID"); NextColumn();
            //        Text("Name"); NextColumn();
            //        Text("Path"); NextColumn();
            //        Text("Flags"); NextColumn();
            //        Separator();
            //        char* names[3] = { "One", "Two", "Three" };
            //        char* paths[3] = { "/path/one", "/path/two", "/path/three" };
            //        static int selected = -1;
            //        for (int i = 0; i < 3; i++)
            //        {
            //            char label[32];
            //            sprintf(label, "%04d", i);
            //            if (Selectable(label, selected == i, ImGuiSelectableFlags_SpanAllColumns))
            //                selected = i;
            //            NextColumn();
            //            Text(names[i]); NextColumn();
            //            Text(paths[i]); NextColumn();
            //            Text("...."); NextColumn();
            //        }
            //        Columns(1);
            //        Separator();
            //        TreePop();
            //    }

            //    // Scrolling columns
            //    /*
            //    if (TreeNode("Scrolling"))
            //    {
            //        BeginChild("##header", new ImVec2(0, GetTextLineHeightWithSpacing()+GetStyle().ItemSpacing.y));
            //        Columns(3);
            //        Text("ID"); NextColumn();
            //        Text("Name"); NextColumn();
            //        Text("Path"); NextColumn();
            //        Columns(1);
            //        Separator();
            //        EndChild();
            //        BeginChild("##scrollingregion", new ImVec2(0, 60));
            //        Columns(3);
            //        for (int i = 0; i < 10; i++)
            //        {
            //            Text("%04d", i); NextColumn();
            //            Text("Foobar"); NextColumn();
            //            Text("/path/foobar/%04d/", i); NextColumn();
            //        }
            //        Columns(1);
            //        EndChild();
            //        TreePop();
            //    }
            //    */

            //    // Create multiple items in a same cell before switching to next column
            //    if (TreeNode("Mixed items"))
            //    {
            //        Columns(3, "mixed");
            //        Separator();

            //        Text("Hello");
            //        Button("Banana");
            //        NextColumn();

            //        Text("ImGui");
            //        Button("Apple");
            //        static float foo = 1.0f;
            //        InputFloat("red", &foo, 0.05f, 0, 3);
            //        Text("An extra line here.");
            //        NextColumn();

            //        Text("Sailor");
            //        Button("Corniflower");
            //        static float bar = 1.0f;
            //        InputFloat("blue", &bar, 0.05f, 0, 3);
            //        NextColumn();

            //        if (CollapsingHeader("Category A")) Text("Blah blah blah"); NextColumn();
            //        if (CollapsingHeader("Category B")) Text("Blah blah blah"); NextColumn();
            //        if (CollapsingHeader("Category C")) Text("Blah blah blah"); NextColumn();
            //        Columns(1);
            //        Separator();
            //        TreePop();
            //    }

            //    // Word wrapping
            //    if (TreeNode("Word-wrapping"))
            //    {
            //        Columns(2, "word-wrapping");
            //        Separator();
            //        TextWrapped("The quick brown fox jumps over the lazy dog.");
            //        TextWrapped("Hello Left");
            //        NextColumn();
            //        TextWrapped("The quick brown fox jumps over the lazy dog.");
            //        TextWrapped("Hello Right");
            //        Columns(1);
            //        Separator();
            //        TreePop();
            //    }

            //    if (TreeNode("Borders"))
            //    {
            //        static bool h_borders = true;
            //        static bool v_borders = true;
            //        Checkbox("horizontal", &h_borders);
            //        SameLine();
            //        Checkbox("vertical", &v_borders);
            //        Columns(4, null, v_borders);
            //        if (h_borders) Separator();
            //        for (int i = 0; i < 8; i++)
            //        {
            //            Text("%c%c%c", 'a' + i, 'a' + i, 'a' + i);
            //            NextColumn();
            //        }
            //        Columns(1);
            //        if (h_borders) Separator();
            //        TreePop();
            //    }

            //    bool node_opened = TreeNode("Tree within single cell");
            //    SameLine(); ShowHelpMarker("NB: Tree node must be poped before ending the cell.\nThere's no storage of state per-cell.");
            //    if (node_opened)
            //    {
            //        Columns(2, "tree items");
            //        Separator();
            //        if (TreeNode("Hello")) { BulletText("Sailor"); TreePop(); }
            //        NextColumn();
            //        if (TreeNode("Bonjour")) { BulletText("Marin"); TreePop(); }
            //        NextColumn();
            //        Columns(1);
            //        Separator();
            //        TreePop();
            //    }
            //}
            #endregion

            #region Filtering
            //if (CollapsingHeader("Filtering"))
            //{
            //    static ImGuiTextFilter filter;
            //    Text("Filter usage:\n"

            //                        "  \"\"         display all lines\n"

            //                        "  \"xxx\"      display lines containing \"xxx\"\n"

            //                        "  \"xxx,yyy\"  display lines containing \"xxx\" or \"yyy\"\n"

            //                        "  \"-xxx\"     hide lines containing \"xxx\"");
            //    filter.Draw();
            //    char* lines[] = { "aaa1.c", "bbb1.c", "ccc1.c", "aaa2.cpp", "bbb2.cpp", "ccc2.cpp", "abc.h", "hello, world" };
            //    for (int i = 0; i < IM_ARRAYSIZE(lines); i++)
            //        if (filter.PassFilter(lines[i]))
            //            BulletText("%s", lines[i]);
            //}
            #endregion

            #region Keyboard, mouse, focus
            //if (CollapsingHeader("Keyboard, Mouse & Focus"))
            //{
            //    if (TreeNode("Tabbing"))
            //    {
            //        Text("Use TAB/SHIFT+TAB to cycle through keyboard editable fields.");
            //        static char buf[32] = "dummy";
            //        InputText("1", buf, IM_ARRAYSIZE(buf));
            //        InputText("2", buf, IM_ARRAYSIZE(buf));
            //        InputText("3", buf, IM_ARRAYSIZE(buf));
            //        PushAllowKeyboardFocus(false);
            //        InputText("4 (tab skip)", buf, IM_ARRAYSIZE(buf));
            //        //SameLine(); ShowHelperMarker("Use PushAllowKeyboardFocus(bool)\nto disable tabbing through certain widgets.");
            //        PopAllowKeyboardFocus();
            //        InputText("5", buf, IM_ARRAYSIZE(buf));
            //        TreePop();
            //    }

            //    if (TreeNode("Focus from code"))
            //    {
            //        bool focus_1 = Button("Focus on 1"); SameLine();
            //        bool focus_2 = Button("Focus on 2"); SameLine();
            //        bool focus_3 = Button("Focus on 3");
            //        int has_focus = 0;
            //        static char buf[128] = "click on a button to set focus";

            //        if (focus_1) SetKeyboardFocusHere();
            //        InputText("1", buf, IM_ARRAYSIZE(buf));
            //        if (IsItemActive()) has_focus = 1;

            //        if (focus_2) SetKeyboardFocusHere();
            //        InputText("2", buf, IM_ARRAYSIZE(buf));
            //        if (IsItemActive()) has_focus = 2;

            //        PushAllowKeyboardFocus(false);
            //        if (focus_3) SetKeyboardFocusHere();
            //        InputText("3 (tab skip)", buf, IM_ARRAYSIZE(buf));
            //        if (IsItemActive()) has_focus = 3;
            //        PopAllowKeyboardFocus();
            //        if (has_focus)
            //            Text("Item with focus: %d", has_focus);
            //        else
            //            Text("Item with focus: <none>");
            //        TextWrapped("Cursor & selection are preserved when refocusing last used item in code.");
            //        TreePop();
            //    }

            //    if (TreeNode("Dragging"))
            //    {
            //        TextWrapped("You can use GetItemActiveDragDelta() to query for the dragged amount on any widget.");
            //        Button("Drag Me");
            //        if (IsItemActive())
            //        {
            //            // Draw a line between the button and the mouse cursor
            //            ImDrawList* draw_list = GetWindowDrawList();
            //            draw_list.PushClipRectFullScreen();
            //            draw_list.AddLine(CalcItemRectClosestPoint(GetIO().MousePos, true, -2.0f), GetIO().MousePos, ImColor(GetStyle().Colors[ImGuiCol_Button]), 4.0f);
            //            draw_list.PopClipRect();
            //            ImVec2 value_raw = GetMouseDragDelta(0, 0.0f);
            //            ImVec2 value_with_lock_threshold = GetMouseDragDelta(0);
            //            ImVec2 mouse_delta = GetIO().MouseDelta;
            //            SameLine(); Text("Raw (%.1f, %.1f), WithLockThresold (%.1f, %.1f), MouseDelta (%.1f, %.1f)", value_raw.x, value_raw.y, value_with_lock_threshold.x, value_with_lock_threshold.y, mouse_delta.x, mouse_delta.y);
            //        }
            //        TreePop();
            //    }

            //    if (TreeNode("Keyboard & Mouse State"))
            //    {
            //        ImGuiIO & io = GetIO();

            //        Text("MousePos: (%g, %g)", io.MousePos.x, io.MousePos.y);
            //        Text("Mouse down:"); for (int i = 0; i < IM_ARRAYSIZE(io.MouseDown); i++) if (io.MouseDownDuration[i] >= 0.0f) { SameLine(); Text("b%d (%.02f secs)", i, io.MouseDownDuration[i]); }
            //        Text("Mouse clicked:"); for (int i = 0; i < IM_ARRAYSIZE(io.MouseDown); i++) if (IsMouseClicked(i)) { SameLine(); Text("b%d", i); }
            //        Text("Mouse dbl-clicked:"); for (int i = 0; i < IM_ARRAYSIZE(io.MouseDown); i++) if (IsMouseDoubleClicked(i)) { SameLine(); Text("b%d", i); }
            //        Text("Mouse released:"); for (int i = 0; i < IM_ARRAYSIZE(io.MouseDown); i++) if (IsMouseReleased(i)) { SameLine(); Text("b%d", i); }
            //        Text("MouseWheel: %.1f", io.MouseWheel);

            //        Text("Keys down:"); for (int i = 0; i < IM_ARRAYSIZE(io.KeysDown); i++) if (io.KeysDownDuration[i] >= 0.0f) { SameLine(); Text("%d (%.02f secs)", i, io.KeysDownDuration[i]); }
            //        Text("Keys pressed:"); for (int i = 0; i < IM_ARRAYSIZE(io.KeysDown); i++) if (IsKeyPressed(i)) { SameLine(); Text("%d", i); }
            //        Text("Keys release:"); for (int i = 0; i < IM_ARRAYSIZE(io.KeysDown); i++) if (IsKeyReleased(i)) { SameLine(); Text("%d", i); }
            //        Text("KeyMods: %s%s%s%s", io.KeyCtrl ? "CTRL " : "", io.KeyShift ? "SHIFT " : "", io.KeyAlt ? "ALT " : "", io.KeySuper ? "SUPER " : "");

            //        Text("WantCaptureMouse: %s", io.WantCaptureMouse ? "true" : "false");
            //        Text("WantCaptureKeyboard: %s", io.WantCaptureKeyboard ? "true" : "false");
            //        Text("WantTextInput: %s", io.WantTextInput ? "true" : "false");

            //        Button("Hovering me sets the\nkeyboard capture flag");
            //        if (IsItemHovered())
            //            CaptureKeyboardFromApp(true);
            //        SameLine();
            //        Button("Holding me clears the\nthe keyboard capture flag");
            //        if (IsItemActive())
            //            CaptureKeyboardFromApp(false);

            //        TreePop();
            //    }

            //    if (TreeNode("Mouse cursors"))
            //    {
            //        TextWrapped("Your application can render a different mouse cursor based on what GetMouseCursor() returns. You can also set io.MouseDrawCursor to ask ImGui to render the cursor for you in software.");
            //        Checkbox("io.MouseDrawCursor", &GetIO().MouseDrawCursor);
            //        Text("Hover to see mouse cursors:");
            //        for (int i = 0; i < ImGuiMouseCursor_Count_; i++)
            //        {
            //            char label[32];
            //            sprintf(label, "Mouse cursor %d", i);
            //            Bullet(); Selectable(label, false);
            //            if (IsItemHovered())
            //                SetMouseCursor(i);
            //        }
            //        TreePop();
            //    }
            //}
            #endregion

            End();
        }

        #region ShowStyleEditor
        void ShowStyleEditor(ImGuiStyle @ref = null)
        {
            ImGuiStyle style = GetStyle();

            ImGuiStyle def = new ImGuiStyle(); // Default style
            if (Button("Revert Style"))
                style = @ref != null ? @ref : def;
            if (@ref != null)
            {
                SameLine();
                if (Button("Save Style"))
                    @ref = style;
            }

            PushItemWidth(GetWindowWidth() * 0.55f);

            if (TreeNode("Rendering"))
            {
                Checkbox("Anti-aliased lines", ref style.AntiAliasedLines);
                Checkbox("Anti-aliased shapes", ref style.AntiAliasedShapes);
                PushItemWidth(100);
                DragFloat("Curve Tessellation Tolerance", ref style.CurveTessellationTol, 0.02f, 0.10f, float.MaxValue, null, 2.0f);
                if (style.CurveTessellationTol < 0.0f) style.CurveTessellationTol = 0.10f;
                DragFloat("Global Alpha", ref style.Alpha, 0.005f, 0.20f, 1.0f, "%.2f"); // Not exposing zero here so user doesn't "lose" the UI (zero alpha clips all widgets). But application code could have a toggle to switch between zero and non-zero.
                PopItemWidth();
                TreePop();
            }

            if (TreeNode("Sizes"))
            {
                SliderFloat2("WindowPadding", ref style.WindowPadding, 0.0f, 20.0f, "{0:0}");
                SliderFloat("WindowRounding", ref style.WindowRounding, 0.0f, 16.0f, "{0:0}");
                SliderFloat("ChildWindowRounding", ref style.ChildWindowRounding, 0.0f, 16.0f, "{0:0}");
                SliderFloat2("FramePadding", ref style.FramePadding, 0.0f, 20.0f, "{0:0}");
                SliderFloat("FrameRounding", ref style.FrameRounding, 0.0f, 16.0f, "{0:0}");
                SliderFloat2("ItemSpacing", ref style.ItemSpacing, 0.0f, 20.0f, "{0:0}");
                SliderFloat2("ItemInnerSpacing", ref style.ItemInnerSpacing, 0.0f, 20.0f, "{0:0}");
                SliderFloat2("TouchExtraPadding", ref style.TouchExtraPadding, 0.0f, 10.0f, "{0:0}");
                SliderFloat("IndentSpacing", ref style.IndentSpacing, 0.0f, 30.0f, "{0:0}");
                SliderFloat("ScrollbarSize", ref style.ScrollbarSize, 1.0f, 20.0f, "{0:0}");
                SliderFloat("ScrollbarRounding", ref style.ScrollbarRounding, 0.0f, 16.0f, "{0:0}");
                SliderFloat("GrabMinSize", ref style.GrabMinSize, 1.0f, 20.0f, "{0:0}");
                SliderFloat("GrabRounding", ref style.GrabRounding, 0.0f, 16.0f, "{0:0}");
                TreePop();
            }

            Action _colors;
            if (!_codeBlocks.TryGetValue("ShowStyleEditor_colors", out _colors))
            {
                int output_dest = 0;
                bool output_only_modified = false;
                //ImGuiColorEditMode edit_mode = ImGuiColorEditMode.ImGuiColorEditMode_RGB;
                int edit_mode = (int)ImGuiColorEditMode.ImGuiColorEditMode_RGB;
                //TODO: ImGuiTextFilter filter;

                _codeBlocks["ShowStyleEditor_colors"] = _colors = new Action(() =>
                {
                    if (TreeNode("Colors"))
                    {
                        if (Button("Copy Colors"))
                        {
                            if (output_dest == 0)
                                LogToClipboard();
                            else
                                LogToTTY();
                            LogText("ImGuiStyle style = GetStyle();" + Environment.NewLine);
                            for (int i = 0; i < (int)ImGuiCol.ImGuiCol_COUNT; i++)
                            {
                                ImVec4 col = style.Colors[i];
                                string name = GetStyleColName((ImGuiCol)i);
                                if (!output_only_modified || col != (@ref != null ? @ref.Colors[i] : def.Colors[i]))
                                    LogText("style.Colors[ImGuiCol_{0}]{5} = ImVec4({1:0.00}, {2:0.00}, {3:0.00}, {4:0.00});" + Environment.NewLine, name, col.x, col.y, col.z, col.w, new string(' ', 22 - name.Length));
                            }
                            LogFinish();
                        }

                        SameLine(); PushItemWidth(120); Combo("##output_type", ref output_dest, new string[] { "To Clipboard", "To TTY" }); PopItemWidth();
                        SameLine(); Checkbox("Only Modified Fields", ref output_only_modified);

                        RadioButton("RGB", ref edit_mode, (int)ImGuiColorEditMode.ImGuiColorEditMode_RGB);
                        SameLine();
                        RadioButton("HSV", ref edit_mode, (int)ImGuiColorEditMode.ImGuiColorEditMode_HSV);
                        SameLine();
                        RadioButton("HEX", ref edit_mode, (int)ImGuiColorEditMode.ImGuiColorEditMode_HEX);
                        //Text("Tip: Click on colored square to change edit mode.");

                        //TODO: filter.Draw("Filter colors", 200);

                        BeginChild("#colors", new ImVec2(0, 300), true, ImGuiWindowFlags.ImGuiWindowFlags_AlwaysVerticalScrollbar);
                        PushItemWidth(-160);
                        ColorEditMode((ImGuiColorEditMode)edit_mode);
                        for (int i = 0; i < (int)ImGuiCol.ImGuiCol_COUNT; i++)
                        {
                            string name = GetStyleColName((ImGuiCol)i);
                            //TODO: if (!filter.PassFilter(name))
                            //    continue;
                            PushID(i);
                            //TODO: ColorEdit4(name, (float*)&style.Colors[i], true);
                            //if (memcmp(&style.Colors[i], (@ref ? @ref.Colors[i] : def.Colors[i]), sizeof(ImVec4)) != 0)
                            if (style.Colors[i] != (@ref != null ? @ref.Colors[i] : def.Colors[i]))
                            {
                                SameLine(); if (Button("Revert")) style.Colors[i] = @ref != null ? @ref.Colors[i] : def.Colors[i];
                                if (@ref != null)
                                {
                                    SameLine(); if (Button("Save")) @ref.Colors[i] = style.Colors[i];
                                }
                            }
                            PopID();
                        }
                        PopItemWidth();
                        EndChild();

                        TreePop();
                    }
                });
            }
            _colors();

            PopItemWidth();

        }
        #endregion

        #region ShowExampleAppMainMenuBar
        //        static void ShowExampleAppMainMenuBar()
        //{
        //    if (BeginMainMenuBar())
        //    {
        //        if (BeginMenu("File"))
        //        {
        //            ShowExampleMenuFile();
        //            EndMenu();
        //        }
        //        if (BeginMenu("Edit"))
        //        {
        //            if (MenuItem("Undo", "CTRL+Z")) { }
        //            if (MenuItem("Redo", "CTRL+Y", false, false)) { }  // Disabled item
        //            Separator();
        //            if (MenuItem("Cut", "CTRL+X")) { }
        //            if (MenuItem("Copy", "CTRL+C")) { }
        //            if (MenuItem("Paste", "CTRL+V")) { }
        //            EndMenu();
        //        }
        //        EndMainMenuBar();
        //    }
        //}
        #endregion

        #region ShowExampleMenuFile
        //        static void ShowExampleMenuFile()
        //{
        //    MenuItem("(dummy menu)", null, false, false);
        //    if (MenuItem("New")) { }
        //    if (MenuItem("Open", "Ctrl+O")) { }
        //    if (BeginMenu("Open Recent"))
        //    {
        //        MenuItem("fish_hat.c");
        //        MenuItem("fish_hat.inl");
        //        MenuItem("fish_hat.h");
        //        if (BeginMenu("More.."))
        //        {
        //            MenuItem("Hello");
        //            MenuItem("Sailor");
        //            if (BeginMenu("Recurse.."))
        //            {
        //                ShowExampleMenuFile();
        //                EndMenu();
        //            }
        //            EndMenu();
        //        }
        //        EndMenu();
        //    }
        //    if (MenuItem("Save", "Ctrl+S")) { }
        //    if (MenuItem("Save As..")) { }
        //    Separator();
        //    if (BeginMenu("Options"))
        //    {
        //        static bool enabled = true;
        //        MenuItem("Enabled", "", &enabled);
        //        BeginChild("child", new ImVec2(0, 60), true);
        //        for (int i = 0; i < 10; i++)
        //            Text("Scrolling Text %d", i);
        //        EndChild();
        //        static float f = 0.5f;
        //        static int n = 0;
        //        SliderFloat("Value", &f, 0.0f, 1.0f);
        //        InputFloat("Input", &f, 0.1f);
        //        Combo("Combo", &n, "Yes\0No\0Maybe\0\0");
        //        EndMenu();
        //    }
        //    if (BeginMenu("Colors"))
        //    {
        //        for (int i = 0; i < ImGuiCol_COUNT; i++)
        //            MenuItem(GetStyleColName((ImGuiCol)i));
        //        EndMenu();
        //    }
        //    if (BeginMenu("Disabled", false)) // Disabled
        //    {
        //        System.Diagnostics.Debug.Assert(0);
        //    }
        //    if (MenuItem("Checked", null, true)) { }
        //    if (MenuItem("Quit", "Alt+F4")) { }
        //}
        #endregion

        #region ShowExampleAppAutoResize
        //        static void ShowExampleAppAutoResize(ref bool opened)
        //{
        //    if (!Begin("Example: Auto-resizing window", opened, ImGuiWindowFlags.ImGuiWindowFlags_AlwaysAutoResize))
        //    {
        //        End();
        //        return;
        //    }

        //    static int lines = 10;
        //    Text("Window will resize every-frame to the size of its content.\nNote that you probably don't want to query the window size to\noutput your content because that would create a feedback loop.");
        //    SliderInt("Number of lines", &lines, 1, 20);
        //    for (int i = 0; i < lines; i++)
        //        Text("%*sThis is line %d", i * 4, "", i); // Pad with space to extend size horizontally
        //    End();
        //}
        #endregion

        #region ShowExampleAppFixedOverlay
        //        static void ShowExampleAppFixedOverlay(ref bool opened)
        //{
        //    SetNextWindowPos(new ImVec2(10, 10));
        //    if (!Begin("Example: Fixed Overlay", opened, new ImVec2(0, 0), 0.3f, ImGuiWindowFlags.ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags.ImGuiWindowFlags_NoResize | ImGuiWindowFlags.ImGuiWindowFlags_NoMove | ImGuiWindowFlags.ImGuiWindowFlags_NoSavedSettings))
        //    {
        //        End();
        //        return;
        //    }
        //    Text("Simple overlay\non the top-left side of the screen.");
        //    Separator();
        //    Text("Mouse Position: (%.1f,%.1f)", GetIO().MousePos.x, GetIO().MousePos.y);
        //    End();
        //}
        //        #endregion
        //        #region ShowExampleAppManipulatingWindowTitle
        //        static void ShowExampleAppManipulatingWindowTitle(ref bool opened)
        //{
        //    (void)opened;

        //    // By default, Windows are uniquely identified by their title.
        //    // You can use the "##" and "###" markers to manipulate the display/ID. Read FAQ at the top of this file!

        //    // Using "##" to display same title but have unique identifier.
        //    SetNextWindowPos(new ImVec2(100, 100), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
        //    Begin("Same title as another window##1");
        //    Text("This is window 1.\nMy title is the same as window 2, but my identifier is unique.");
        //    End();

        //    SetNextWindowPos(new ImVec2(100, 200), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
        //    Begin("Same title as another window##2");
        //    Text("This is window 2.\nMy title is the same as window 1, but my identifier is unique.");
        //    End();

        //    // Using "###" to display a changing title but keep a static identifier "AnimatedTitle"
        //    char buf[128];
        //    sprintf(buf, "Animated title %c %d###AnimatedTitle", "|/-\\"[(int)(GetTime() / 0.25f) & 3], rand());
        //    SetNextWindowPos(new ImVec2(100, 300), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
        //    Begin(buf);
        //    Text("This window has a changing title.");
        //    End();
        //}
        #endregion

        #region ShowExampleAppCustomRendering
        //        static void ShowExampleAppCustomRendering(ref bool opened)
        //{
        //    SetNextWindowSize(new ImVec2(350, 560), ImGuiSetCond.ImGuiSetCond_FirstUseEver);
        //    if (!Begin("Example: Custom rendering", opened))
        //    {
        //        End();
        //        return;
        //    }

        //    // Tip: If you do a lot of custom rendering, you probably want to use your own geometrical types and benefit of overloaded operators, etc.
        //    // Define IM_VEC2_CLASS_EXTRA in imconfig.h to create implicit conversions between your types and ImVec2/ImVec4.
        //    // ImGui defines overloaded operators but they are internal to imgui.cpp and not exposed outside (to avoid messing with your types)
        //    // In this example we are not using the maths operators!
        //    ImDrawList* draw_list = GetWindowDrawList();

        //    // Primitives
        //    Text("Primitives");
        //    static float sz = 36.0f;
        //    static ImVec4 col = ImVec4(1.0f, 1.0f, 0.4f, 1.0f);
        //    DragFloat("Size", &sz, 0.2f, 2.0f, 72.0f, "%.0f");
        //    ColorEdit3("Color", &col.x);
        //    {
        //        ImVec2 p = GetCursorScreenPos();
        //        ImU32 col32 = ImColor(col);
        //        float x = p.x + 4.0f, y = p.y + 4.0f, spacing = 8.0f;
        //        for (int n = 0; n < 2; n++)
        //        {
        //            float thickness = (n == 0) ? 1.0f : 4.0f;
        //            draw_list.AddCircle(new ImVec2(x + sz * 0.5f, y + sz * 0.5f), sz * 0.5f, col32, 20, thickness); x += sz + spacing;
        //            draw_list.AddRect(new ImVec2(x, y), new ImVec2(x + sz, y + sz), col32, 0.0f, ~0, thickness); x += sz + spacing;
        //            draw_list.AddRect(new ImVec2(x, y), new ImVec2(x + sz, y + sz), col32, 10.0f, ~0, thickness); x += sz + spacing;
        //            draw_list.AddTriangle(new ImVec2(x + sz * 0.5f, y), new ImVec2(x + sz, y + sz - 0.5f), new ImVec2(x, y + sz - 0.5f), col32, thickness); x += sz + spacing;
        //            draw_list.AddLine(new ImVec2(x, y), new ImVec2(x + sz, y), col32, thickness); x += sz + spacing;
        //            draw_list.AddLine(new ImVec2(x, y), new ImVec2(x + sz, y + sz), col32, thickness); x += sz + spacing;
        //            draw_list.AddLine(new ImVec2(x, y), new ImVec2(x, y + sz), col32, thickness); x += spacing;
        //            draw_list.AddBezierCurve(new ImVec2(x, y), new ImVec2(x + sz * 1.3f, y + sz * 0.3f), new ImVec2(x + sz - sz * 1.3f, y + sz - sz * 0.3f), new ImVec2(x + sz, y + sz), col32, thickness);
        //            x = p.x + 4;
        //            y += sz + spacing;
        //        }
        //        draw_list.AddCircleFilled(new ImVec2(x + sz * 0.5f, y + sz * 0.5f), sz * 0.5f, col32, 32); x += sz + spacing;
        //        draw_list.AddRectFilled(new ImVec2(x, y), new ImVec2(x + sz, y + sz), col32); x += sz + spacing;
        //        draw_list.AddRectFilled(new ImVec2(x, y), new ImVec2(x + sz, y + sz), col32, 10.0f); x += sz + spacing;
        //        draw_list.AddTriangleFilled(new ImVec2(x + sz * 0.5f, y), new ImVec2(x + sz, y + sz - 0.5f), new ImVec2(x, y + sz - 0.5f), col32); x += sz + spacing;
        //        draw_list.AddRectFilledMultiColor(new ImVec2(x, y), new ImVec2(x + sz, y + sz), ImColor(0, 0, 0), ImColor(255, 0, 0), ImColor(255, 255, 0), ImColor(0, 255, 0));
        //        Dummy(new ImVec2((sz + spacing) * 8, (sz + spacing) * 3));
        //    }
        //    Separator();
        //    {
        //        static ImVector<ImVec2> points;
        //        static bool adding_line = false;
        //        Text("Canvas example");
        //        if (Button("Clear")) points.clear();
        //        if (points.Size >= 2) { SameLine(); if (Button("Undo")) { points.pop_back(); points.pop_back(); } }
        //        Text("Left-click and drag to add lines,\nRight-click to undo");

        //        // Here we are using InvisibleButton() as a convenience to 1) advance the cursor and 2) allows us to use IsItemHovered()
        //        // However you can draw directly and poll mouse/keyboard by yourself. You can manipulate the cursor using GetCursorPos() and SetCursorPos().
        //        // If you only use the ImDrawList API, you can notify the owner window of its extends by using SetCursorPos(max).
        //        ImVec2 canvas_pos = GetCursorScreenPos();            // ImDrawList API uses screen coordinates!
        //        ImVec2 canvas_size = GetContentRegionAvail();        // Resize canvas to what's available
        //        if (canvas_size.x < 50.0f) canvas_size.x = 50.0f;
        //        if (canvas_size.y < 50.0f) canvas_size.y = 50.0f;
        //        draw_list.AddRectFilledMultiColor(canvas_pos, new ImVec2(canvas_pos.x + canvas_size.x, canvas_pos.y + canvas_size.y), ImColor(50, 50, 50), ImColor(50, 50, 60), ImColor(60, 60, 70), ImColor(50, 50, 60));
        //        draw_list.AddRect(canvas_pos, new ImVec2(canvas_pos.x + canvas_size.x, canvas_pos.y + canvas_size.y), ImColor(255, 255, 255));

        //        bool adding_preview = false;
        //        InvisibleButton("canvas", canvas_size);
        //        if (IsItemHovered())
        //        {
        //            ImVec2 mouse_pos_in_canvas = new ImVec2(GetIO().MousePos.x - canvas_pos.x, GetIO().MousePos.y - canvas_pos.y);
        //            if (!adding_line && IsMouseClicked(0))
        //            {
        //                points.push_back(mouse_pos_in_canvas);
        //                adding_line = true;
        //            }
        //            if (adding_line)
        //            {
        //                adding_preview = true;
        //                points.push_back(mouse_pos_in_canvas);
        //                if (!GetIO().MouseDown[0])
        //                    adding_line = adding_preview = false;
        //            }
        //            if (IsMouseClicked(1) && !points.empty())
        //            {
        //                adding_line = adding_preview = false;
        //                points.pop_back();
        //                points.pop_back();
        //            }
        //        }
        //        draw_list.PushClipRect(ImVec4(canvas_pos.x, canvas_pos.y, canvas_pos.x + canvas_size.x, canvas_pos.y + canvas_size.y));      // clip lines within the canvas (if we resize it, etc.)
        //        for (int i = 0; i < points.Size - 1; i += 2)
        //            draw_list.AddLine(new ImVec2(canvas_pos.x + points[i].x, canvas_pos.y + points[i].y), new ImVec2(canvas_pos.x + points[i + 1].x, canvas_pos.y + points[i + 1].y), 0xFF00FFFF, 2.0f);
        //        draw_list.PopClipRect();
        //        if (adding_preview)
        //            points.pop_back();
        //    }
        //    End();
        //}
        #endregion

    }
}
