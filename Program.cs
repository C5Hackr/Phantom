using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Phantom
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bin"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\bin");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PhantomMain());
        }
    }
}
