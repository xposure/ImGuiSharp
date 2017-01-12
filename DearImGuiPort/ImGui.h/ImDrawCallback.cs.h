// Draw callbacks for advanced uses.
// NB- You most likely do NOT need to use draw callbacks just to create your own widget or customized UI rendering (you can poke into the draw list for that)
// Draw callback may be useful for example, if you want to render a complex 3D scene inside a UI element, change your GPU render state, etc.
// The expected behavior from your rendering loop is:
//   if (cmd.UserCallback != NULL)
//       cmd.UserCallback(parent_list, cmd);
//   else
//       RenderTriangles()
typedef void(*ImDrawCallback)(const ImDrawList* parent_list, const ImDrawCmd* cmd);