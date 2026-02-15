using UnityEngine;
using UnityEditor;
using System.Linq;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool needButton = IsSceneMissingFromBuild(property);
            return EditorGUIUtility.singleLineHeight + (needButton ? 25f : 0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
            var scenePathProp = property.FindPropertyRelative("scenePath");

            Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Object newScene = EditorGUI.ObjectField(fieldRect, label, sceneAssetProp.objectReferenceValue, typeof(SceneAsset), false);
            
            if (EditorGUI.EndChangeCheck())
            {
                sceneAssetProp.objectReferenceValue = newScene;
                if (newScene != null)
                {
                    scenePathProp.stringValue = AssetDatabase.GetAssetPath(newScene);
                }
                else
                {
                    scenePathProp.stringValue = string.Empty;
                }
            }

            if (IsSceneMissingFromBuild(property))
            {
                Rect buttonRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, 20);
                
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Rouge clair

                if (GUI.Button(buttonRect, "⚠️ Fix: Add to Build Settings"))
                {
                    AddSceneToBuildSettings(scenePathProp.stringValue);
                }

                GUI.backgroundColor = oldColor;
            }

            EditorGUI.EndProperty();
        }

        // Vérifie si la scène actuelle est absente des settings
        private bool IsSceneMissingFromBuild(SerializedProperty property)
        {
            var sceneAsset = property.FindPropertyRelative("sceneAsset").objectReferenceValue;
            if (sceneAsset == null) return false; 

            string path = AssetDatabase.GetAssetPath(sceneAsset);
            
            // On regarde la liste des scènes du build
            return !EditorBuildSettings.scenes.Any(s => s.path == path);
        }

        private void AddSceneToBuildSettings(string path)
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[SLToolbox] Scène ajoutée au Build : {path}");
        }
    }
}