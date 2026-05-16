using UnityEditor;
using UnityEngine;

namespace RomainUTR.SLToolbox.Editor
{
    public static class SLToolboxPreferences
    {
        private const string COLOR_PREF_KEY = "RomainUTR_SLToolbox_StarColor";
        private const string DEFAULT_HEX_COLOR = "#FFCA28";

        private static SLToolboxValues _values;
        public static SLToolboxValues Values
        {
            get
            {
                if (_values == null) _values = SLToolboxValues.Load();
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        public static Color GetStarFavoriteColor()
        {
            string hexColor = EditorPrefs.GetString(COLOR_PREF_KEY, DEFAULT_HEX_COLOR);

            if (ColorUtility.TryParseHtmlString(hexColor, out Color loadedColor))
            {
                return loadedColor;
            }

            return Color.yellow;
        }

        private static void SetStarFavoriteColor(Color newColor)
        {
            string hexColor = "#" + ColorUtility.ToHtmlStringRGBA(newColor);
            EditorPrefs.SetString(COLOR_PREF_KEY, hexColor);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSLToolboxSettingsProvider()
        {
            var provider = new SettingsProvider("Project/SL Toolbox", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Scene Switcher Settings", EditorStyles.boldLabel);

                    EditorGUI.BeginChangeCheck();
                    Color currentColor = GetStarFavoriteColor();
                    Color newColor = EditorGUILayout.ColorField("Favorite Star Color", currentColor);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetStarFavoriteColor(newColor);
                    }

                    GUILayout.Space(20);
                    GUILayout.Label("Script Generator Paths", EditorStyles.boldLabel);

                    Values.defaultPath = DrawPathSetting("Default Path", Values.defaultPath);
                    Values.classPath = DrawPathSetting("Class Path", Values.classPath);

                    GUILayout.Space(5);
                    Values.ssoPath = DrawPathSetting("SSO Path", Values.ssoPath);
                    Values.rsoPath = DrawPathSetting("RSO Path", Values.rsoPath);
                    Values.rsePath = DrawPathSetting("RSE Path", Values.rsePath);

                    GUILayout.Space(20);
                    GUILayout.Label("Asset Generator Paths (Data .asset)", EditorStyles.boldLabel);

                    Values.assetSSOPath = DrawPathSetting("SSO Assets", Values.assetSSOPath);
                    Values.assetRSOPath = DrawPathSetting("RSO Assets", Values.assetRSOPath);
                    Values.assetRSEPath = DrawPathSetting("RSE Assets", Values.assetRSEPath);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Values.Save();
                    }
                }
            };

            return provider;
        }

        private static string DrawPathSetting(string label, string currentPath)
        {
            GUILayout.BeginHorizontal();
            string newPath = EditorGUILayout.TextField(label, currentPath);

            if (GUILayout.Button("Browse", EditorStyles.miniButton, GUILayout.Width(70)))
            {
                string absPath = EditorUtility.OpenFolderPanel($"Select {label} Folder", Application.dataPath, "");

                if (!string.IsNullOrEmpty(absPath))
                {
                    if (absPath.StartsWith(Application.dataPath))
                    {
                        newPath = "Assets" + absPath.Substring(Application.dataPath.Length) + "/";
                    }
                    else
                    {
                        Debug.LogWarning("SL Toolbox: You must select a folder located within the “Assets” folder !");
                    }
                }
            }

            GUILayout.EndHorizontal();
            return newPath;
        }
    }
}
