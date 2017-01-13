namespace ImGui
{
    using System;
    using System.Collections.Generic;

    // Lightweight std::vector<> like class to avoid dragging dependencies (also: windows implementation of STL with debug enabled is absurdly slow, so let's bypass it so our code runs fast in debug).
    // Our implementation does NOT call c++ constructors because we don't use them in ImGui. Don't use this class as a straight std::vector replacement in your code!
    public class ImVector<T> : IDisposable
    {
        public int Size;
        public int Capacity;

        public T[] Data;
        //T*                          Data;

        public Type value_type = typeof(T);

        //typedef value_type*         iterator;
        //typedef const value_type*   const_iterator;

        public ImVector() { Size = Capacity = 0; }
        public ImVector(params T[] args) : base()
        {
            reserve(args.Length);
            foreach (var a in args)
                push_back(a);
        }
        //~ImVector()                 { if (Data) ImGui::MemFree(Data); }

        public bool empty() { return Size == 0; }
        public int size() { return Size; }
        public int capacity() { return Capacity; }

        public T this[int i]
        {
            get
            {
                //IM_ASSERT(i < Size);
                return Data[i];
            }
            set
            {
                Data[i] = value;
            }

        }

        public void clear() { Size = Capacity = 0; Data = null; }
        //public  IEnumerator             begin()                         { return Data; }
        //public  const_iterator       begin() const                   { return Data; }
        public T front()
        {
            System.Diagnostics.Debug.Assert(Size > 0);
            return Data[0];
        }
        //public  const value_type&    front() const                   { IM_ASSERT(Size > 0); return Data[0]; }
        public T back()
        {
            System.Diagnostics.Debug.Assert(Size > 0);
            return Data[Size - 1];
        }

        public void back(T val)
        {
            System.Diagnostics.Debug.Assert(Size > 0);
            Data[Size - 1] = val;
        }
        //public  const value_type&    back() const                    { IM_ASSERT(Size > 0); return Data[Size-1]; }
        //public  void                 swap(ImVector<T>& rhs)          { int rhs_size = rhs.Size; rhs.Size = Size; Size = rhs_size; int rhs_cap = rhs.Capacity; rhs.Capacity = Capacity; Capacity = rhs_cap; value_type* rhs_data = rhs.Data; rhs.Data = Data; Data = rhs_data; }

        public int _grow_capacity(int new_size)
        {
            int new_capacity = Capacity > 0 ? (Capacity + Capacity / 2) : 8;
            return new_capacity > new_size ? new_capacity : new_size;
        }

        public void resize(int new_size)
        {
            if (new_size > Capacity)
                reserve(_grow_capacity(new_size));
            Size = new_size;
        }

        public void reserve(int new_capacity)
        {
            if (new_capacity <= Capacity) return;
            Capacity = new_capacity;
            if (Data == null)
                Data = new T[new_capacity];
            else
                Array.Resize(ref Data, new_capacity);
        }

        public void push_back(T v)
        {
            if (Size == Capacity)
                reserve(_grow_capacity(Size + 1));
            Data[Size++] = v;
        }
        public void pop_back()
        {
            //IM_ASSERT(Size > 0);
            Size--;
        }

        public void erase(int it)
        {
            System.Diagnostics.Debug.Assert(it >= 0 && it < Size);
            for (var i = it; i < Size - 1; i++)
                Data[i] = Data[i + 1];
            Size--;
        }

        public void insert(int it, T v)
        {
            System.Diagnostics.Debug.Assert(it >= 0 && it <= Size);
            var off = it;
            if (Size == Capacity)
                reserve(Capacity > 0 ? Capacity * 2 : 4);
            if (off < Size)
            {
                //memmove( off + 1, off, ((size_t)Size - (size_t)off) * sizeof(value_type));
                for (int i = Size; i > 0; i--)
                    Data[i] = Data[i - 1];
            }
            Data[off] = v;
            Size++;
            //return Data + off;
        }

        public void sort(Func<T, T, int> sorter)
        {
            Array.Sort<T>(Data, new Comparison<T>(sorter));
        }

        public void swap(ImVector<T> rhs)
        {
            int rhs_size = rhs.Size;
            rhs.Size = Size;
            Size = rhs_size;

            int rhs_cap = rhs.Capacity;
            rhs.Capacity = Capacity;
            Capacity = rhs_cap;

            T[] rhs_data = rhs.Data;
            rhs.Data = Data;
            Data = rhs_data;
        }


        public void Dispose()
        {
            Data = null;
        }

    }
}
