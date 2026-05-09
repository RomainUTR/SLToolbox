using UnityEditor;
using UnityEngine;

namespace RomainUTR.SLToolbox.Editor
{
    public static class SLToolboxPreferences
    {
        private const string COLOR_PREF_KEY = "RomainUTR_SLToolbox_StarColor";
        private const string DEFAULT_HEX_COLOR = "#FFCA28";

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
            var provider = new SettingsProvider("Preferences/SL Toolbox", SettingsScope.User)
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
                }
            };

            return provider;
        }
    }
}
