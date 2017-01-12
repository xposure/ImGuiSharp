//namespace ImGui
//{
//    // Helper: Text buffer for logging/accumulating text
//    public class ImGuiTextBuffer
//    {
//        public ImVector<char> Buf;

//        public ImGuiTextBuffer()
//        {
//            Buf.push_back((char)0);
//        }

//        public char this[int i]
//        {
//            get { return Buf[i]; }
//        }
//        //const char*         begin() const { return &Buf.front(); }
//        //const char*         end() const { return &Buf.back(); }      // Buf is zero-terminated, so end() will point on the zero-terminator
//        public int size()
//        {
//            return Buf.Size - 1;
//        }
//        public bool empty()
//        {
//            return Buf.Size <= 1;
//        }
//        public void clear()
//        {
//            Buf.clear();
//            Buf.push_back((char)0);
//        }

//        public string c_str()
//        {
//            var data = new char[Buf.size()];
//            for (var i = 0; i < data.Length; i++)
//                data[i] = Buf[i];
//            return new string(data);
//        }
//        //void      append(const char* fmt, ...) IM_PRINTFARGS(2);
//        //void      appendv(const char* fmt, va_list args);
//    }
//}
