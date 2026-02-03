using UnityEngine;
using UnityEngine.InputSystem;

public static class RebindStorage
{
    private const string PREFS_KEY = "InputBindings";

    public static void Save(InputActionAsset asset)
    {
        string overrides = asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(PREFS_KEY, overrides);
        PlayerPrefs.Save();
    }

    public static void Load(InputActionAsset asset)
    {
        if (PlayerPrefs.HasKey(PREFS_KEY))
        {
            string overrides = PlayerPrefs.GetString(PREFS_KEY);
            if (!string.IsNullOrEmpty(overrides))
            {
                asset.LoadBindingOverridesFromJson(overrides);
            }
        }
    }
}
