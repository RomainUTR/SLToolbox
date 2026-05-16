using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEditor;

namespace RomainUTR.SLToolbox.Editor
{
    public static class MainToolbarSceneSwitcher
    {
        [MainToolbarElement("SLToolbox/SceneSwitcher", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateSceneSwitcher()
        {
            var content = new MainToolbarContent("🎬 Scenes", "SL Toolbox - Quick Scene Switcher");
            var dropdown = new MainToolbarDropdown(content, (rect) => PopupWindow.Show(rect, new SceneSwitcherPopup()));

            return dropdown;
        }
    }
}
