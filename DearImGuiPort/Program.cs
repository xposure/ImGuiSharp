using System;

namespace ImGui.MonoGame.NET
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var t = typeof(System.Math);
            //var methods = t.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            
            //foreach (var m in methods)
            //{
            //    Console.Write("[MethodImpl(MethodImplOptions.AggressiveInlining)] public static ");
            //    Console.Write(m.ReturnType.ToString());
            //    Console.Write(" ");
            //    Console.Write(m.Name);
            //    Console.Write("(");
            //    var i = 0;
            //    foreach(var p in m.GetParameters())
            //    {
            //        if (i > 0)
            //            Console.Write(", ");
            //        Console.Write(p.ParameterType.ToString());
            //        Console.Write(" arg{0}", i);

            //        i++;
            //    }
            //    Console.Write(") { ");
            //    Console.Write("return System.Math.{0}(", m.Name);

            //    i = 0;
            //    foreach (var p in m.GetParameters())
            //    {
            //        if (i > 0)
            //            Console.Write(", ");
            //        Console.Write(" arg{0}", i);

            //        i++;
            //    }
            //    Console.Write("); }");
            //    Console.WriteLine();
            //    //Console.WriteLine(m);
            //}
            //Console.Read();

            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}
