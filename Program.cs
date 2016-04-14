using System;
using GameEngine;

namespace NAJ_Lab1
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
            Engine engine = new Engine();
            engine.StartEngine();
        }
    }
#endif
}
