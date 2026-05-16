using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RomainUTR.SLToolbox.Editor
{
    public class SLToolboxValues
    {
        public string defaultPath = "Assets/Scripts/Generated/";
        public string classPath = "Assets/Scripts/Generated/";
        public string ssoPath = "Assets/Scripts/Generated/";
        public string rsoPath = "Assets/Scripts/Generated/";
        public string rsePath = "Assets/Events/";

        public string assetSSOPath = "Assets/Data/SSO/";
        public string assetRSOPath = "Assets/Data/RSO/";
        public string assetRSEPath = "Assets/Data/Events/";
        public string COLOR_PREF_KEY = "RomainUTR_SLToolbox_StarColor";
        public string DEFAULT_HEX_COLOR = "#FFCA28";
        public List<string> favoriteScenes = new List<string>();

        private static readonly string FilePath = Path.Combine("ProjectSettings", "SLToolbox_Preferences.json");

        public void Save()
        {
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(FilePath, json);
        }

        public static SLToolboxValues Load()
        {
            if (!File.Exists(FilePath))
            {
                Debug.Log("SL Toolbox: Creating the default configuration file.");
                SLToolboxValues defaultValues = new SLToolboxValues();
                defaultValues.Save();
                return defaultValues;
            }

            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SLToolboxValues>(json);
        }
    }
}
