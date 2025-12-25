using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using System.IO;

namespace RomainUTR.SLToolbox.Editor
{
    public class SceneSwitcherContent : PopupWindowContent
    {
        private string searchQuery = "";
        private Vector2 scrollPos;
        private string[] allScenePaths;

        public SceneSwitcherContent()
        {
            allScenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => path.StartsWith("Assets/"))
                .ToArray();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 300);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.SetNextControlName("SearchField");
            searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);

            if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(searchQuery))
            {
                GUI.FocusControl("SearchField");
            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            var filteredScenes = allScenePaths
                .Where(path => path.IndexOf(searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            foreach (string path in filteredScenes)
            {
                string sceneName = Path.GetFileNameWithoutExtension(path);

                if (GUILayout.Button(sceneName, EditorStyles.miniButton))
                {
                    LoadScene(path);
                    editorWindow.Close();
                }
            }

            GUILayout.EndScrollView();
        }

        private void LoadScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}