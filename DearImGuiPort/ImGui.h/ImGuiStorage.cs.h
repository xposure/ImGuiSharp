// Helper: Key->value storage
// - Store collapse state for a tree (Int 0/1)
// - Store color edit options (Int using values in ImGuiColorEditMode enum).
// - Custom user storage for temporary values.
// Typically you don't have to worry about this since a storage is held within each Window.
// Declare your own storage if:
// - You want to manipulate the open/close state of a particular sub-tree in your interface (tree node uses Int 0/1 to store their state).
// - You want to store custom debug data easily without adding or editing structures in your code.
struct ImGuiStorage
{
	struct Pair
	{
		uint key;
		union { int val_i; float val_f; void* val_p; };
		Pair(uint _key, int _val_i) { key = _key; val_i = _val_i; }
		Pair(uint _key, float _val_f) { key = _key; val_f = _val_f; }
		Pair(uint _key, void* _val_p) { key = _key; val_p = _val_p; }
	};
	ImVector<Pair>      Data;

	// - Get***() functions find pair, never add/allocate. Pairs are sorted so a query is O(log N)
	// - Set***() functions find pair, insertion on demand if missing.
	// - Sorted insertion is costly but should amortize. A typical frame shouldn't need to insert any new pair.
	IMGUI_API void      Clear();
	IMGUI_API int       GetInt(uint key, int default_val = 0) const;
	IMGUI_API void      SetInt(uint key, int val);
	IMGUI_API float     GetFloat(uint key, float default_val = 0.0f) const;
	IMGUI_API void      SetFloat(uint key, float val);
	IMGUI_API void*     GetVoidPtr(uint key) const; // default_val is NULL
	IMGUI_API void      SetVoidPtr(uint key, void* val);

	// - Get***Ref() functions finds pair, insert on demand if missing, return pointer. Useful if you intend to do Get+Set.
	// - References are only valid until a new value is added to the storage. Calling a Set***() function or a Get***Ref() function invalidates the pointer.
	// - A typical use case where this is convenient:
	//      float* pvar = ImGui::GetFloatRef(key); ImGui::SliderFloat("var", pvar, 0, 100.0f); some_var += *pvar;
	// - You can also use this to quickly create temporary editable values during a session of using Edit&Continue, without restarting your application.
	IMGUI_API int*      GetIntRef(uint key, int default_val = 0);
	IMGUI_API float*    GetFloatRef(uint key, float default_val = 0);
	IMGUI_API void**    GetVoidPtrRef(uint key, void* default_val = NULL);

	// Use on your own storage if you know only integer are being stored (open/close all tree nodes)
	IMGUI_API void      SetAllInt(int val);
};