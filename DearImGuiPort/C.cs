using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImGui
{
    public unsafe static class C
    {
        public static void memcpy(byte* destination, byte* source, uint length)
        {
            var index = 0;
            while (index < length)
            {
                destination[index] = source[index];
                index++;
            }
        }
    }
}
