using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RomainUTR.SLToolbox.Editor
{
    public static class SceneBootloader
    {
        const string MenuPath = "Tools/SL Toolbox/Play From First Scene";

        [MenuItem(MenuPath)]
        private static void TogglePlayModeStartScene()
        {
            bool isCurrentlyActive = EditorSceneManager.playModeStartScene != null;

            if (!isCurrentlyActive)
            {
                var sceneToLoad = EditorBuildSettings.scenes[0];

                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneToLoad.path);
                EditorSceneManager.playModeStartScene = sceneAsset;

                Debug.Log($"<color=green>Bootloader Activ� :</color> Le jeu se lancera sur {sceneToLoad.path}");
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
                Debug.Log("<color=yellow>Bootloader D�sactiv� :</color> Retour au comportement normal.");
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateTogglePlayModeStartScene()
        {
            Menu.SetChecked(MenuPath, EditorSceneManager.playModeStartScene != null);
            return true;
        }
    }
}