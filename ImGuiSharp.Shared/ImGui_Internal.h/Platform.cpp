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
					ImGui::TextColored(ImColor(255, 100, 100), "CURRENTLY APPENDING"); // Can't display stats for active draw list! (we don't have the data double-buffered)
					if (node_opened) ImGui::TreePop();
					return;
				}
				if (!node_opened)
					return;

				ImDrawList* overlay_draw_list = &GImGui->OverlayDrawList;   // Render additional visuals into the top-most draw list
				overlay_draw_list->PushClipRectFullScreen();
				int elem_offset = 0;
				for (const ImDrawCmd* pcmd = draw_list->CmdBuffer.begin(); pcmd < draw_list->CmdBuffer.end(); elem_offset += pcmd->ElemCount, pcmd++)
				{
					if (pcmd->UserCallback)
					{
						ImGui::BulletText("Callback %p, user_data %p", pcmd->UserCallback, pcmd->UserCallbackData);
						continue;
					}
					bool draw_opened = ImGui::TreeNode((void*)(pcmd - draw_list->CmdBuffer.begin()), "Draw %-4d %s vtx, tex = %p, clip_rect = (%.0f,%.0f)..(%.0f,%.0f)", pcmd->ElemCount, draw_list->IdxBuffer.Size > 0 ? "indexed" : "non-indexed", pcmd->TextureId, pcmd->ClipRect.x, pcmd->ClipRect.y, pcmd->ClipRect.z, pcmd->ClipRect.w);
					if (show_clip_rects && ImGui::IsItemHovered())
					{
						ImRect clip_rect = pcmd->ClipRect;
						ImRect vtxs_rect;
						ImDrawIdx* idx_buffer = (draw_list->IdxBuffer.Size > 0) ? draw_list->IdxBuffer.Data : NULL;
						for (int i = elem_offset; i < elem_offset + (int)pcmd->ElemCount; i++)
							vtxs_rect.Add(draw_list->VtxBuffer[idx_buffer ? idx_buffer[i] : i].pos);
						clip_rect.Round(); overlay_draw_list->AddRect(clip_rect.Min, clip_rect.Max, ImColor(255, 255, 0));
						vtxs_rect.Round(); overlay_draw_list->AddRect(vtxs_rect.Min, vtxs_rect.Max, ImColor(255, 0, 255));
					}
					if (!draw_opened)
						continue;
					for (int i = elem_offset; i + 2 < elem_offset + (int)pcmd->ElemCount; i += 3)
					{
						ImVec2 triangles_pos[3];
						char buf[300], *buf_p = buf;
						for (int n = 0; n < 3; n++)
						{
							ImDrawVert& v = draw_list->VtxBuffer[(draw_list->IdxBuffer.Size > 0) ? draw_list->IdxBuffer.Data[i + n] : i + n];
							triangles_pos[n] = v.pos;
							buf_p += sprintf(buf_p, "vtx %04d { pos = (%8.2f,%8.2f), uv = (%.6f,%.6f), col = %08X }\n", i + n, v.pos.x, v.pos.y, v.uv.x, v.uv.y, v.col);
						}
						ImGui::Selectable(buf, false);
						if (ImGui::IsItemHovered())
							overlay_draw_list->AddPolyline(triangles_pos, 3, ImColor(255, 255, 0), true, 1.0f, false);  // Add triangle without AA, more readable for large-thin triangle
					}
					ImGui::TreePop();
				}
				overlay_draw_list->PopClipRect();
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