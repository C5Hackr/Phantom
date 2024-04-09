using System;
using System.IO;
using Newtonsoft.Json;

namespace Phantom
{
    internal class Settings
    {
        private static string savepath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\settings.json";

        internal static SettingsObject Load()
        {
            if (File.Exists(savepath))
            {
                return JsonConvert.DeserializeObject<SettingsObject>(File.ReadAllText(savepath));
            }
            return null;
        }

        internal static void Save(SettingsObject obj) => File.WriteAllText(savepath, JsonConvert.SerializeObject(obj, Formatting.Indented));
    }

    internal class SettingsObject
    {
        public string inputFile { get; set; }

        public bool antiDebug { get; set; }

        public bool antiVM { get; set; }
        
        public bool selfDelete { get; set; }

        public bool hidden { get; set; }

        public bool runas { get; set; }
        
        public bool startup { get; set; }
        
        public bool uacBypass { get; set; }

        public string[] bindedFiles { get; set; }
    }
}
