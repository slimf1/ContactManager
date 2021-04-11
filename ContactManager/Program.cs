using System;

namespace ContactManagerApplication
{
    class Program
    {
        /// <summary>
        /// Point d'entrée du programme
        /// </summary>
        /// <param name="args">Arguments d'exécution</param>
        static void Main(string[] args)
        {
            ConsoleUi ui = new ConsoleUi();
            ui.DisplayMenu();
        }
    }
}
