using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Stm32ULoader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
//            Application.EnableVisualStyles(); // Disable for correct progress bar color
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
