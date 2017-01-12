// Lightweight std::vector<> like class to avoid dragging dependencies (also: windows implementation of STL with debug enabled is absurdly slow, so let's bypass it so our code runs fast in debug).
// Our implementation does NOT call c++ constructors because we don't use them in ImGui. Don't use this class as a straight std::vector replacement in your code!
template<typename T>
class ImVector
{
public:
	int                         Size;
	int                         Capacity;
	T*                          Data;

	typedef T                   value_type;
	typedef value_type*         iterator;
	typedef const value_type*   const_iterator;

	ImVector() { Size = Capacity = 0; Data = NULL; }
	~ImVector() { if (Data) ImGui::MemFree(Data); }

	inline bool                 empty() const { return Size == 0; }
	inline int                  size() const { return Size; }
	inline int                  capacity() const { return Capacity; }

	inline value_type&          operator[](int i) { IM_ASSERT(i < Size); return Data[i]; }
	inline const value_type&    operator[](int i) const { IM_ASSERT(i < Size); return Data[i]; }

	inline void                 clear() { if (Data) { Size = Capacity = 0; ImGui::MemFree(Data); Data = NULL; } }
	inline iterator             begin() { return Data; }
	inline const_iterator       begin() const { return Data; }
	inline iterator             end() { return Data + Size; }
	inline const_iterator       end() const { return Data + Size; }
	inline value_type&          front() { IM_ASSERT(Size > 0); return Data[0]; }
	inline const value_type&    front() const { IM_ASSERT(Size > 0); return Data[0]; }
	inline value_type&          back() { IM_ASSERT(Size > 0); return Data[Size - 1]; }
	inline const value_type&    back() const { IM_ASSERT(Size > 0); return Data[Size - 1]; }
	inline void                 swap(ImVector<T>& rhs) { int rhs_size = rhs.Size; rhs.Size = Size; Size = rhs_size; int rhs_cap = rhs.Capacity; rhs.Capacity = Capacity; Capacity = rhs_cap; value_type* rhs_data = rhs.Data; rhs.Data = Data; Data = rhs_data; }

	inline int                  _grow_capacity(int new_size) { int new_capacity = Capacity ? (Capacity + Capacity / 2) : 8; return new_capacity > new_size ? new_capacity : new_size; }

	inline void                 resize(int new_size) { if (new_size > Capacity) reserve(_grow_capacity(new_size)); Size = new_size; }
	inline void                 reserve(int new_capacity)
	{
		if (new_capacity <= Capacity) return;
		T* new_data = (value_type*)ImGui::MemAlloc((size_t)new_capacity * sizeof(value_type));
		memcpy(new_data, Data, (size_t)Size * sizeof(value_type));
		ImGui::MemFree(Data);
		Data = new_data;
		Capacity = new_capacity;
	}

	inline void                 push_back(const value_type& v) { if (Size == Capacity) reserve(_grow_capacity(Size + 1)); Data[Size++] = v; }
	inline void                 pop_back() { IM_ASSERT(Size > 0); Size--; }

	inline iterator             erase(const_iterator it) { IM_ASSERT(it >= Data && it < Data + Size); const ptrdiff_t off = it - Data; memmove(Data + off, Data + off + 1, ((size_t)Size - (size_t)off - 1) * sizeof(value_type)); Size--; return Data + off; }
	inline iterator             insert(const_iterator it, const value_type& v) { IM_ASSERT(it >= Data && it <= Data + Size); const ptrdiff_t off = it - Data; if (Size == Capacity) reserve(Capacity ? Capacity * 2 : 4); if (off < (int)Size) memmove(Data + off + 1, Data + off, ((size_t)Size - (size_t)off) * sizeof(value_type)); Data[off] = v; Size++; return Data + off; }
};