namespace ImGui
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("X: {x}, Y: {y}")]
    public struct ImVec2
    {
        public float x, y;
        public ImVec2(float _x, float _y) { x = _x; y = _y; }

        public static ImVec2 Zero { get { return new ImVec2(0, 0); } }
        public static ImVec2 One { get { return new ImVec2(1, 1); } }

        public static bool operator ==(ImVec2 lhs, ImVec2 rhs) { return lhs.x == rhs.x && lhs.y == rhs.y; }
        public static bool operator !=(ImVec2 lhs, ImVec2 rhs) { return lhs.x != rhs.x || lhs.y != rhs.y; }
        public static ImVec2 operator *(ImVec2 lhs, float rhs) { return new ImVec2(lhs.x * rhs, lhs.y * rhs); }
        public static ImVec2 operator /(ImVec2 lhs, float rhs) { return new ImVec2(lhs.x / rhs, lhs.y / rhs); }
        public static ImVec2 operator +(ImVec2 lhs, ImVec2 rhs) { return new ImVec2(lhs.x + rhs.x, lhs.y + rhs.y); }
        public static ImVec2 operator -(ImVec2 lhs, ImVec2 rhs) { return new ImVec2(lhs.x - rhs.x, lhs.y - rhs.y); }
        public static ImVec2 operator *(ImVec2 lhs, ImVec2 rhs) { return new ImVec2(lhs.x * rhs.x, lhs.y * rhs.y); }
        public static ImVec2 operator /(ImVec2 lhs, ImVec2 rhs) { return new ImVec2(lhs.x / rhs.x, lhs.y / rhs.y); }

        public override int GetHashCode()
        {
            var h1 = x.GetHashCode();
            var h2 = y.GetHashCode();
            return (((h1 << 5) + h1) ^ h2);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this Vector3 instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this Vector3; False otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is ImVec2))
                return false;
            return Equals((ImVec2)obj);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Vector3 is equal to this Vector3 instance.
        /// </summary>
        /// <param name="other">The Vector3 to compare this instance to.</param>
        /// <returns>True if the other Vector3 is equal to this instance; False otherwise.</returns>
        public bool Equals(ImVec2 other)
        {
            return x == other.x &&
                   y == other.y;
        }

        public override string ToString()
        {
            return string.Format("{{ X: {0}, Y: {1} }}", x, y);
        }
    };
}
