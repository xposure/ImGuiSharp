namespace ImGui
{
    // Helper functions to create a color that can be converted to either u32 or float4
    public struct ImColor
    {
        public ImVec4 Value;
        public ImColor(int r, int g, int b, int a = 255) { float sc = 1.0f / 255.0f; Value.x = (float)r * sc; Value.y = (float)g * sc; Value.z = (float)b * sc; Value.w = (float)a * sc; }
        public ImColor(uint rgba) { float sc = 1.0f / 255.0f; Value.x = (float)(rgba & 0xFF) * sc; Value.y = (float)((rgba >> 8) & 0xFF) * sc; Value.z = (float)((rgba >> 16) & 0xFF) * sc; Value.w = (float)(rgba >> 24) * sc; }
        public ImColor(float r, float g, float b, float a = 1.0f) { Value.x = r; Value.y = g; Value.z = b; Value.w = a; }
        public ImColor(ImVec4 col) { Value = col; }

        public static implicit operator uint(ImColor value) { return ColorConvertFloat4ToU32(value.Value); }
        public static implicit operator ImVec4(ImColor value) { return value.Value; }
        public static ImColor HSV(float h, float s, float v, float a = 1.0f) { float r, g, b; ColorConvertHSVtoRGB(h, s, v, out r, out g, out b); return new ImColor(r, g, b, a); }

        public void SetHSV(float h, float s, float v, float a = 1.0f) { ColorConvertHSVtoRGB(h, s, v, out Value.x, out Value.y, out Value.z); Value.w = a; }

        public uint ToRGBA()
        {
            var a = (byte)(Value.x * 255);
            var r = (byte)(Value.y * 255);
            var g = (byte)(Value.z * 255);
            var b = (byte)(Value.w * 255);

            return (uint)((r << 24) + (g << 16) + (b << 8) + a);
        }

        public uint ToARGB()
        {
            var a = (byte)(Value.x * 255);
            var r = (byte)(Value.y * 255);
            var g = (byte)(Value.z * 255);
            var b = (byte)(Value.w * 255);

            return (uint)((a << 24) + (r << 16) + (g << 8) + b);
        }

        private static float[] u32FloatLookup;
        public static ImVec4 ColorConvertU32ToFloat4(uint @in)
        {
            if (u32FloatLookup == null)
            {
                const float s = 1.0f / 255.0f;
                u32FloatLookup = new float[256];
                for (var i = 0; i < 256; i++)
                {
                    u32FloatLookup[i] = i * s;
                }
            }

            var x = u32FloatLookup[@in & 0xff]; @in >>= 8;
            var y = u32FloatLookup[@in & 0xff]; @in >>= 8;
            var z = u32FloatLookup[@in & 0xff]; @in >>= 8;
            var w = u32FloatLookup[@in & 0xff];

            return new ImVec4(x, y, z, w);
            //return new ImVec4((@in & 0xFF), ((@in >> 8) & 0xFF), ((@in >> 16) & 0xFF), (@in >> 24)) * s;
        }

        public static uint ColorConvertFloat4ToU32(ImVec4 @in)
        {
            uint @out;
            @out = (uint)((Saturate(@in.x)) * 255.0f + 0.5f);
            @out |= ((uint)((Saturate(@in.y)) * 255.0f + 0.5f) << 8);
            @out |= ((uint)((Saturate(@in.z)) * 255.0f + 0.5f) << 16);
            @out |= ((uint)((Saturate(@in.w)) * 255.0f + 0.5f) << 24);
            return @out;
        }

        internal static float Saturate(float f)
        {
            return (f < 0.0f) ? 0.0f : (f > 1.0f) ? 1.0f : f;
        }

        // Convert hsv floats ([0-1],[0-1],[0-1]) to rgb floats ([0-1],[0-1],[0-1]), from Foley & van Dam p593
        // also http://en.wikipedia.org/wiki/HSL_and_HSV
        public static void ColorConvertHSVtoRGB(float h, float s, float v, out float out_r, out float out_g, out float out_b)
        {
            if (s == 0.0f)
            {
                // gray
                out_r = out_g = out_b = v;
                return;
            }

            h = ImMath.fmodf(h, 1.0f) / (60.0f / 360.0f);
            int i = (int)h;
            float f = h - (float)i;
            float p = v * (1.0f - s);
            float q = v * (1.0f - s * f);
            float t = v * (1.0f - s * (1.0f - f));

            switch (i)
            {
                case 0: out_r = v; out_g = t; out_b = p; break;
                case 1: out_r = q; out_g = v; out_b = p; break;
                case 2: out_r = p; out_g = v; out_b = t; break;
                case 3: out_r = p; out_g = q; out_b = v; break;
                case 4: out_r = t; out_g = p; out_b = v; break;
                case 5: default: out_r = v; out_g = p; out_b = q; break;
            }
        }

        //#define IM_COL32(R,G,B,A)    (((uint)(A)<<24) | ((uint)(B)<<16) | ((uint)(G)<<8) | ((uint)(R)))
        public static ImColor White { get { return new ImColor(1f, 1f, 1f, 1f); } }
        public static ImColor Black { get { return new ImColor(0f, 0f, 0f, 1f); } }
        public static ImColor Transparent { get { return new ImColor(0f, 0f, 0f, 0f); } }

    }
}
