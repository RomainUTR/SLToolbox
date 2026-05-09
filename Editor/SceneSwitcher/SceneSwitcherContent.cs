using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace RomainUTR.SLToolbox.Editor
{
    public class SceneSwitcherContent : PopupWindowContent
    {
        private string searchQuery = "";
        private Vector2 scrollPos;
        private string[] allScenePaths;

        private const string PREFS_KEY = "RomainUTR_SLToolbox_FavScenes";
        private List<string> favoriteScenes = new List<string>();

        public SceneSwitcherContent()
        {
            allScenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => path.StartsWith("Assets/"))
                .ToArray();

            LoadFavorites();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 300);
        }

        public override void OnGUI(Rect rect)
        {
            DrawToolbar();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            var filteredScenes = allScenePaths
                .Where(path => path.IndexOf(searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            var favScenesToDisplay = filteredScenes.Where(path => favoriteScenes.Contains(path)).ToArray();
            var otherScenesToDisplay = filteredScenes.Where(path => !favoriteScenes.Contains(path)).ToArray();

            if (favScenesToDisplay.Length > 0)
            {
                GUILayout.Label("Favorites", EditorStyles.boldLabel);
                foreach (string path in favScenesToDisplay)
                {
                    DrawSceneRow(path, true);
                }

                EditorGUILayout.Space(5);
            }

            if (otherScenesToDisplay.Length > 0)
            {
                GUILayout.Label("All Scenes", EditorStyles.boldLabel);
                foreach (string path in otherScenesToDisplay)
                {
                    DrawSceneRow(path, false);
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.SetNextControlName("SearchField");
            searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);

            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                EditorApplication.delayCall += CreateNewScene;
            }

            if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(searchQuery))
            {
                GUI.FocusControl("SearchField");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSceneRow(string path, bool isFavorite)
        {
            string sceneName = Path.GetFileNameWithoutExtension(path);

            GUILayout.BeginHorizontal();

            string starIcon = isFavorite ? "★" : "☆";

            Color defaultColor = GUI.color;

            if (isFavorite)
            {
                GUI.color = SLToolboxPreferences.GetStarFavoriteColor();
            }

            if (GUILayout.Button(starIcon, EditorStyles.label, GUILayout.Width(20)))
            {
                ToggleFavorite(path);
            }

            GUI.color = defaultColor;

            if (GUILayout.Button(sceneName, EditorStyles.miniButton))
            {
                EditorApplication.delayCall += () =>
                {
                    LoadScene(path);
                    editorWindow.Close();
                };
            }

            GUILayout.EndHorizontal();
        }

        private void LoadFavorites()
        {
            string savedData = EditorPrefs.GetString(PREFS_KEY, "");

            if (!string.IsNullOrEmpty(savedData))
            {
                string[] paths = savedData.Split(',');
                favoriteScenes = new List<string>(paths);
            }
            else
            {
                favoriteScenes = new List<string>();
            }
        }

        private void ToggleFavorite(string path)
        {
            if (favoriteScenes.Contains(path))
            {
                favoriteScenes.Remove(path);
            }
            else
            {
                favoriteScenes.Add(path);
            }

            SaveFavorites();
        }

        private void SaveFavorites()
        {
            string dataToSave = string.Join(",", favoriteScenes);
            EditorPrefs.SetString(PREFS_KEY, dataToSave);
        }

        private void CreateNewScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string path = EditorUtility.SaveFilePanelInProject("Create New Scene", "New Scene", "unity", "Choose location for the new scene");

                if (!string.IsNullOrEmpty(path))
                {
                    UnityEngine.SceneManagement.Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(newScene, path);
                    editorWindow.Close();
                }
            }
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