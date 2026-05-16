using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace RomainUTR.SLToolbox.Editor
{
    public class ScriptGeneratorWindow : EditorWindow
    {
        public enum TemplateType
        {
            MonoBehaviour,
            StaticScriptableObject,
            RuntimeScriptableEvent,
            RuntimeScriptableObject,
            Class
        }

        private string targetBaseName = "NewScript";
        private TemplateType selectedTemplate = TemplateType.MonoBehaviour;
        private const string PENDING_SO_NAME_KEY = "SLToolbox_PendingSO_Name";
        private const string PENDING_SO_PATH_KEY = "SLToolbox_PendingSO_Path";

        [MenuItem("SL Toolbox/Script Generator", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<ScriptGeneratorWindow>("Script Forge");
            window.minSize = new Vector2(350, 200);
            window.maxSize = new Vector2(400, 250);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Create New Script", EditorStyles.boldLabel);

            GUI.SetNextControlName("ClassNameField");
            targetBaseName = EditorGUILayout.TextField(targetBaseName, targetBaseName);
            selectedTemplate = (TemplateType)EditorGUILayout.EnumPopup("Template", selectedTemplate);

            GUILayout.Space(10);

            string safeBaseName = Regex.Replace(targetBaseName, @"[^a-zA-Z0-9_]", "");

            string prefix = GetPrefixForTemplate(selectedTemplate);
            string targetFolder = GetPathForTemplate(selectedTemplate);

            string finalClassName = prefix + safeBaseName;

            if (string.IsNullOrEmpty(safeBaseName))
            {
                EditorGUILayout.HelpBox("Please enter a valid name.", MessageType.Warning);
            } else
            {
                EditorGUILayout.HelpBox($"File: {finalClassName}.cs\nFolder: {targetFolder}", MessageType.Info);
            }

            GUILayout.Space(15);

            GUI.enabled = !string.IsNullOrEmpty(safeBaseName);
            if (GUILayout.Button("Create Script", GUILayout.Height(30)))
            {
                GenerateScript(finalClassName, targetFolder);
            }
            GUI.enabled = true;

            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() == string.Empty)
            {
                GUI.FocusControl("ClassNameField");
            }
        }

        private string GetPrefixForTemplate(TemplateType type)
        {
            switch (type)
            {
                case TemplateType.StaticScriptableObject: return "SSO_";
                case TemplateType.RuntimeScriptableObject: return "RSO_";
                case TemplateType.RuntimeScriptableEvent: return "RSE_";
                case TemplateType.MonoBehaviour: return "";
                case TemplateType.Class: return "";
                default: return "";
            }
        }

        private string GetPathForTemplate(TemplateType type)
        {
            switch (type)
            {
                case TemplateType.StaticScriptableObject: return SLToolboxPreferences.Values.ssoPath;
                case TemplateType.RuntimeScriptableObject: return SLToolboxPreferences.Values.rsoPath;
                case TemplateType.RuntimeScriptableEvent: return SLToolboxPreferences.Values.rsePath;
                case TemplateType.Class: return SLToolboxPreferences.Values.classPath;
                default: return SLToolboxPreferences.Values.defaultPath;
            }
        }

        private string GetAssetPathForTemplate(TemplateType type)
        {
            switch (type)
            {
                case TemplateType.StaticScriptableObject: return SLToolboxPreferences.Values.assetSSOPath;
                case TemplateType.RuntimeScriptableObject: return SLToolboxPreferences.Values.assetRSOPath;
                case TemplateType.RuntimeScriptableEvent: return SLToolboxPreferences.Values.assetRSEPath;
                default: return "Assets/";
            }
        }

        private void GenerateScript(string finalClassName, string targetFolder)
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            string filePath = Path.Combine(targetFolder, finalClassName + ".cs");

            if (File.Exists(filePath))
            {
                Debug.LogError($"SL Toolbox: A file named {finalClassName}.cs already exists!");
                return;
            }

            if (selectedTemplate == TemplateType.RuntimeScriptableObject || selectedTemplate == TemplateType.StaticScriptableObject || selectedTemplate == TemplateType.RuntimeScriptableEvent)
            {
                string targetAssetFolder = GetAssetPathForTemplate(selectedTemplate);

                if (!Directory.Exists(targetAssetFolder))
                {
                    Directory.CreateDirectory(targetAssetFolder);
                }

                EditorPrefs.SetString(PENDING_SO_NAME_KEY, finalClassName);
                EditorPrefs.SetString(PENDING_SO_PATH_KEY, targetAssetFolder);
            }

            string templateContent = GetTemplateContent(selectedTemplate);
            string finalContent = templateContent.Replace("#CLASSNAME#", finalClassName);

            File.WriteAllText(filePath, finalContent);
            EditorUtility.DisplayDialog("Script Forge", $"SL Toolbox: {finalClassName} a great success!", "Awesome");
            Close();
            AssetDatabase.Refresh();
        }

        private string GetTemplateContent(TemplateType type)
        {
            string templateFileName = "Template_" + type.ToString();
            string[] guids = AssetDatabase.FindAssets(templateFileName + " t:TextAsset");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                if (textAsset != null)
                {
                    return textAsset.text;
                }
            }

            Debug.LogError($"SL Toolbox: I can't find the template {templateFileName}.txt !");
            return "";
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!EditorPrefs.HasKey(PENDING_SO_NAME_KEY)) return;

            string className = EditorPrefs.GetString(PENDING_SO_NAME_KEY);
            string folderPath = EditorPrefs.GetString(PENDING_SO_PATH_KEY);

            EditorPrefs.DeleteKey(PENDING_SO_NAME_KEY);
            EditorPrefs.DeleteKey(PENDING_SO_PATH_KEY);

            ScriptableObject instance = ScriptableObject.CreateInstance(className);

            if (instance != null)
            {
                string assetPath = Path.Combine(folderPath, className + ".asset").Replace("\\", "/");

                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();

                Debug.Log($"SL Toolbox: The {className}.asset file has been created in {folderPath}");
            }
            else
            {
                Debug.LogError($"SL Toolbox: Instantiation failed. The {className} script may contain errors.");
            }
        }
    }
}
