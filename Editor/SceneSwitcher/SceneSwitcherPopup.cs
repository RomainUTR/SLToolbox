using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RomainUTR.SLToolbox.Editor
{
    public class SceneSwitcherPopup : PopupWindowContent
    {
        private string[] favScenes;
        private string[] otherScenes;
        private SLToolboxValues config;

        private Vector2 scrollPosition;

        private float calculatedWidth = 250f;

        public override void OnOpen()
        {
            config = SLToolboxPreferences.Values;

            var allScenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => path.StartsWith("Assets/"))
                .ToArray();

            favScenes = allScenePaths.Where(path => config.favoriteScenes.Contains(path)).ToArray();
            otherScenes = allScenePaths.Where(path => !config.favoriteScenes.Contains(path)).ToArray();

            GUIStyle textStyle = new GUIStyle(EditorStyles.label) { fontSize = 13 };
            float maxWidth = EditorStyles.boldLabel.CalcSize(new GUIContent("   Project Scenes")).x;

            foreach (string path in allScenePaths)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                float textWidth = textStyle.CalcSize(new GUIContent(sceneName)).x;

                if (textWidth > maxWidth)
                {
                    maxWidth = textWidth;
                }
            }

            float totalNeededWidth = maxWidth + 60f;
            calculatedWidth = Mathf.Clamp(totalNeededWidth, 200f, 500f);
        }

        public override Vector2 GetWindowSize()
        {
            float headerHeight = 35f;
            float rowHeight = 24f;
            float separatorHeight = (favScenes.Length > 0 && otherScenes.Length > 0) ? 15f : 0f;
            float paddingBottom = 10f;

            int totalScenes = favScenes.Length + otherScenes.Length;
            float calculatedHeight = headerHeight + (totalScenes * rowHeight) + separatorHeight + paddingBottom;

            return new Vector2(calculatedWidth, Mathf.Min(calculatedHeight, 400f));
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(10);
            GUILayout.Label("   Project Scenes", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUIStyle starStyle = new GUIStyle(EditorStyles.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            GUIStyle textStyle = new GUIStyle(EditorStyles.label) { fontSize = 13, alignment = TextAnchor.MiddleLeft };
            Texture sceneIcon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (string path in favScenes)
            {
                DrawSceneRow(path, config, starStyle, textStyle, sceneIcon);
            }

            if (favScenes.Length > 0 && otherScenes.Length > 0)
            {
                GUILayout.Space(5);
                GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
                GUILayout.Space(5);
            }

            foreach (string path in otherScenes)
            {
                DrawSceneRow(path, config, starStyle, textStyle, sceneIcon);
            }

            GUILayout.EndScrollView();
        }

        private void DrawSceneRow(string scenePath, SLToolboxValues config, GUIStyle starStyle, GUIStyle textStyle, Texture sceneIcon)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            bool isFavorite = config.favoriteScenes.Contains(scenePath);

            ColorUtility.TryParseHtmlString(config.DEFAULT_HEX_COLOR, out Color starColor);
            Color inactiveColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            GUI.color = isFavorite ? starColor : inactiveColor;

            if (GUILayout.Button("★", starStyle, GUILayout.Width(25), GUILayout.Height(20)))
            {
                if (isFavorite) config.favoriteScenes.Remove(scenePath);
                else config.favoriteScenes.Add(scenePath);
                config.Save();
                OnOpen();
            }

            GUI.color = Color.white;

            GUILayout.Label(sceneIcon, GUILayout.Width(20), GUILayout.Height(20));

            if (GUILayout.Button(sceneName, textStyle, GUILayout.Height(20)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
                editorWindow.Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
    }
}
