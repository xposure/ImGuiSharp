
namespace ImGui
{
    using System.Runtime.InteropServices;

    // Simple custom key value storage
    // Helper: Key->value storage
    // - Store collapse state for a tree (Int 0/1)
    // - Store color edit options (Int using values in ImGuiColorEditMode enum).
    // - Custom user storage for temporary values.
    // Typically you don't have to worry about this since a storage is held within each Window.
    // Declare your own storage if:
    // - You want to manipulate the open/close state of a particular sub-tree in your interface (tree node uses Int 0/1 to store their state).
    // - You want to store custom debug data easily without adding or editing structures in your code.
    public class ImGuiStorage
    {
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct Pair
        {
            [FieldOffset(0)]
            public uint key;

            [FieldOffset(4)]
            public int val_i;

            [FieldOffset(4)]
            public float val_f;

            //union { int val_i; float val_f; void* val_p; }

            public Pair(uint _key, int _val_i)
            {
                val_f = 0f;
                key = _key;
                val_i = _val_i;
            }

            public Pair(uint _key, float _val_f)
            {
                val_i = 0;
                key = _key;
                val_f = _val_f;
            }

            //public Pair(uint _key, void* _val_p)
            //{
            //    key = _key; val_p = _val_p;
            //}
        }

        public ImVector<Pair> Data = new ImVector<Pair>();

        internal void Clear()
        {
            Data.clear();
        }

        // - Get***() functions find pair, never add/allocate. Pairs are sorted so a query is O(log N)
        // - Set***() functions find pair, insertion on demand if missing.
        // - Sorted insertion is costly but should amortize. A typical frame shouldn't need to insert any new pair.
        int FindByKey(uint key)
        {
            var first = 0;
            var last = Data.Size;
            int count = (last - first);
            while (count > 0)
            {
                int count2 = count / 2;
                int mid = first + count2;
                if (Data[mid].key < key)
                {
                    first = ++mid;
                    count -= count2 + 1;
                }
                else
                {
                    count = count2;
                }
            }
            return first;
        }

        internal int GetInt(uint key, int default_val)
        {
            var idx = FindByKey(key);
            if (idx < Data.Size && Data[idx].key == key)
                return Data[idx].val_i;

            return default_val;
        }

        internal float GetFloat(uint key, float default_val)
        {
            var idx = FindByKey(key);
            if (idx < Data.Size && Data[idx].key == key)
                return Data[idx].val_f;

            return default_val;
        }

        //void* GetVoidPtr(uint key) const; // default_val is NULL
        //void SetVoidPtr(uint key, void* val);

        // FIXME-OPT: Need a way to reuse the result of lower_bound when doing GetInt()/SetInt() - not too bad because it only happens on explicit interaction (maximum one a frame)
        public void SetInt(uint key, int val)
        {
            var idx = FindByKey(key);
            if (idx < Data.Size && Data[idx].key == key)
            {
                var pair = Data[idx];
                pair.val_i = val;
                Data[idx] = pair;
            }
            else {
                Data.insert(idx, new Pair(key, val));                
            }
        }

        internal void SetFloat(uint key, float val)
        {
            var idx = FindByKey(key);
            if (idx < Data.Size && Data[idx].key == key)
            {
                var pair = Data[idx];
                pair.val_f = val;
                Data[idx] = pair;
            }
            else {
                Data.insert(idx, new Pair(key, val));
            }
        }

        // Use on your own storage if you know only integer are being stored (open/close all tree nodes)
        internal void SetAllInt(int v)
        {
            for (int i = 0; i < Data.Size; i++)
            {
                var pair = Data[i];
                pair.val_i = v;
                Data[i] = pair;
            }
        }

        // - Get***Ref() functions finds pair, insert on demand if missing, return pointer. Useful if you intend to do Get+Set.
        // - References are only valid until a new value is added to the storage. Calling a Set***() function or a Get***Ref() function invalidates the pointer.
        // - A typical use case where this is convenient:
        //      float* pvar = ImGui::GetFloatRef(key); ImGui::SliderFloat("var", pvar, 0, 100.0f); some_var += *pvar;
        // - You can also use this to quickly create temporary editable values during a session of using Edit&Continue, without restarting your application.
        //int* GetIntRef(uint key, int default_val = 0);
        //float* GetFloatRef(uint key, float default_val = 0);
        //void** GetVoidPtrRef(uint key, void* default_val = NULL);
    }
}
