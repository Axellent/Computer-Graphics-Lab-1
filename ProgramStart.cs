using System;

namespace NAJ_Lab1
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class ProgramStart
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ChopperGame chopperGame = new ChopperGame();
            chopperGame.StartGame();
        }
    }
#endif
}
