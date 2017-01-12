using System.Runtime.CompilerServices;

namespace ImGui
{
    public static class ImMath
    {
        public const float PI = (float)System.Math.PI;
        public const float TwoPI = (float)(System.Math.PI * 2);

        public static float Cosf(float a)
        {
            return (float)System.Math.Cos(a);
        }

        public static float Sinf(float a)
        {
            return (float)System.Math.Sin(a);
        }

        public static float InvLength(ImVec2 lhs, float fail_value)
        {
            float d = lhs.x * lhs.x + lhs.y * lhs.y;
            if (d > 0.0f)
                return 1.0f / (float)System.Math.Sqrt(d);
            return fail_value;
        }

        public static float fmodf(float x, float y)
        {
            return x % y;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Acos(System.Double arg0) { return System.Math.Acos(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Asin(System.Double arg0) { return System.Math.Asin(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Atan(System.Double arg0) { return System.Math.Atan(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Atan2(System.Double arg0, System.Double arg1) { return System.Math.Atan2(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Ceiling(System.Decimal arg0) { return System.Math.Ceiling(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Ceiling(System.Double arg0) { return System.Math.Ceiling(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Cos(System.Double arg0) { return System.Math.Cos(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Cosh(System.Double arg0) { return System.Math.Cosh(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Floor(System.Decimal arg0) { return System.Math.Floor(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Floor(System.Double arg0) { return System.Math.Floor(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Sin(System.Double arg0) { return System.Math.Sin(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Tan(System.Double arg0) { return System.Math.Tan(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Sinh(System.Double arg0) { return System.Math.Sinh(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Tanh(System.Double arg0) { return System.Math.Tanh(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Round(System.Double arg0) { return System.Math.Round(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Round(System.Double arg0, System.Int32 arg1) { return System.Math.Round(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Round(System.Double arg0, System.MidpointRounding arg1) { return System.Math.Round(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Round(System.Double arg0, System.Int32 arg1, System.MidpointRounding arg2) { return System.Math.Round(arg0, arg1, arg2); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Round(System.Decimal arg0) { return System.Math.Round(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Round(System.Decimal arg0, System.Int32 arg1) { return System.Math.Round(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Round(System.Decimal arg0, System.MidpointRounding arg1) { return System.Math.Round(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Round(System.Decimal arg0, System.Int32 arg1, System.MidpointRounding arg2) { return System.Math.Round(arg0, arg1, arg2); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Truncate(System.Decimal arg0) { return System.Math.Truncate(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Truncate(System.Double arg0) { return System.Math.Truncate(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Sqrt(System.Double arg0) { return System.Math.Sqrt(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Log(System.Double arg0) { return System.Math.Log(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Log10(System.Double arg0) { return System.Math.Log10(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Exp(System.Double arg0) { return System.Math.Exp(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Pow(System.Double arg0, System.Double arg1) { return System.Math.Pow(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double IEEERemainder(System.Double arg0, System.Double arg1) { return System.Math.IEEERemainder(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.SByte Abs(System.SByte arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int16 Abs(System.Int16 arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Abs(System.Int32 arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int64 Abs(System.Int64 arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Single Abs(System.Single arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Abs(System.Double arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Abs(System.Decimal arg0) { return System.Math.Abs(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.SByte Max(System.SByte arg0, System.SByte arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte Max(System.Byte arg0, System.Byte arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int16 Max(System.Int16 arg0, System.Int16 arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt16 Max(System.UInt16 arg0, System.UInt16 arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Max(System.Int32 arg0, System.Int32 arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt32 Max(System.UInt32 arg0, System.UInt32 arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int64 Max(System.Int64 arg0, System.Int64 arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt64 Max(System.UInt64 arg0, System.UInt64 arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Single Max(System.Single arg0, System.Single arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Max(System.Double arg0, System.Double arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Max(System.Decimal arg0, System.Decimal arg1) { return System.Math.Max(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.SByte Min(System.SByte arg0, System.SByte arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Byte Min(System.Byte arg0, System.Byte arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int16 Min(System.Int16 arg0, System.Int16 arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt16 Min(System.UInt16 arg0, System.UInt16 arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Min(System.Int32 arg0, System.Int32 arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt32 Min(System.UInt32 arg0, System.UInt32 arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int64 Min(System.Int64 arg0, System.Int64 arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.UInt64 Min(System.UInt64 arg0, System.UInt64 arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Single Min(System.Single arg0, System.Single arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Min(System.Double arg0, System.Double arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Decimal Min(System.Decimal arg0, System.Decimal arg1) { return System.Math.Min(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Double Log(System.Double arg0, System.Double arg1) { return System.Math.Log(arg0, arg1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.SByte arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.Int16 arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.Int32 arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.Int64 arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.Single arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.Double arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int32 Sign(System.Decimal arg0) { return System.Math.Sign(arg0); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Int64 BigMul(System.Int32 arg0, System.Int32 arg1) { return System.Math.BigMul(arg0, arg1); }

    }
}
