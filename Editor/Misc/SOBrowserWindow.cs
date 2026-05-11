using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RomainUTR.SLToolbox.Editor
{
    public class SOBrowserWindow : EditorWindow
    {
        private List<Type> soTypes = new List<Type>();
        private string[] soTypeNames;

        private List<ScriptableObject> currentInstances = new List<ScriptableObject>();

        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;

        private Type selectedType;
        private ScriptableObject selectedSO;
        private UnityEditor.Editor soEditor;

        private string typeSearchQuery = "";
        private Vector2 typeScrollPos;
        private Vector2 instanceScrollPos;
        private Vector2 inspectorScrollPos;

        [MenuItem("SL Toolbox/Scriptable Object Browser", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<SOBrowserWindow>("Scriptable Objects Browser");
            window.minSize = new Vector2(1000, 600);
        }

        private void OnEnable()
        {
            ScanForScriptableObjectTypes();
        }

        private void OnDisable()
        {
            if (soEditor != null)
            {
                DestroyImmediate(soEditor);
            }
        }

        void ScanForScriptableObjectTypes()
        {
            string[] ignoredPrefixes = new string[]
            {
                "Unity",
                "System",
                "MoreMountains",
                "Sirenix",
                "Shapes",
                "DG.Tweening"
            };

            var allTypes = TypeCache.GetTypesDerivedFrom<ScriptableObject>();

            soTypes = allTypes
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsGenericType)
                .Where(t =>
                {
                    string assemblyName = t.Assembly.GetName().Name;
                    string namespaceName = t.Namespace ?? "";

                    foreach (string prefix in ignoredPrefixes)
                    {
                        if (assemblyName.StartsWith(prefix) || namespaceName.StartsWith(prefix))
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .Where(t => !typeof(UnityEditor.Editor).IsAssignableFrom(t))
                .Where(t => !typeof(UnityEditor.EditorWindow).IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToList();

            soTypeNames = soTypes.Select(t => t.Name).ToArray();
        }

        private void LoadInstancesForType(Type type)
        {
            currentInstances.Clear();
            SelectSO(null);

            string[] guids = AssetDatabase.FindAssets("t:" + type.Name);

            if (guids.Length == 0)
            {
                guids = AssetDatabase.FindAssets("t:" + type.FullName);
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject instance = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;

                if (instance != null)
                {
                    currentInstances.Add(instance);
                }
            }
        }

        private void SelectSO(ScriptableObject so)
        {
            selectedSO = so;

            if (soEditor != null)
            {
                DestroyImmediate(soEditor);
            }

            if (selectedSO != null)
            {
                soEditor = UnityEditor.Editor.CreateEditor(selectedSO);
            }
        }

        private void OnGUI()
        {
            if (soTypes == null || soTypes.Count == 0)
            {
                GUILayout.Label("No Scriptable Objects found in project.", EditorStyles.boldLabel);
                return;
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("↻ Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                ScanForScriptableObjectTypes();
                if (selectedType != null) LoadInstancesForType(selectedType);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+ Create", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                if (selectedType != null) CreateNewSO(selectedType);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(280));

            typeSearchQuery = GUILayout.TextField(typeSearchQuery, EditorStyles.toolbarSearchField);
            typeScrollPos = GUILayout.BeginScrollView(typeScrollPos, "box");

            foreach (var t in soTypes)
            {
                if (!string.IsNullOrEmpty(typeSearchQuery) && t.Name.IndexOf(typeSearchQuery, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                GUI.backgroundColor = (selectedType == t) ? Color.cyan : Color.white;

                if (GUILayout.Button(t.Name, EditorStyles.miniButton))
                {
                    selectedType = t;
                    LoadInstancesForType(t);
                    GUI.FocusControl(null);
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(250));
            instanceScrollPos = GUILayout.BeginScrollView(instanceScrollPos, "box");

            if (selectedType == null)
            {
                GUILayout.Label("Select a Type first.", EditorStyles.centeredGreyMiniLabel);
            }
            else if (currentInstances.Count == 0)
            {
                GUILayout.Label("No instances found.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var so in currentInstances)
                {
                    GUI.backgroundColor = (selectedSO == so) ? Color.cyan : Color.white;
                    if (GUILayout.Button(so.name, EditorStyles.miniButton))
                    {
                        SelectSO(so);
                        GUI.FocusControl(null);
                    }
                    GUI.backgroundColor = Color.white;
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            inspectorScrollPos = GUILayout.BeginScrollView(inspectorScrollPos);

            if (selectedSO != null && soEditor != null)
            {
                soEditor.DrawHeader();
                soEditor.OnInspectorGUI();
            }
            else
            {
                GUILayout.Label("Select an instance to inspect it.", EditorStyles.centeredGreyMiniLabel);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void CreateNewSO(Type type)
        {
            string path = EditorUtility.SaveFilePanelInProject($"Create {type.Name}", $"New {type.Name}", "asset", "Choose location");

            if (!string.IsNullOrEmpty(path))
            {
                ScriptableObject newInst = ScriptableObject.CreateInstance(type);

                AssetDatabase.CreateAsset(newInst, path);
                AssetDatabase.SaveAssets();

                LoadInstancesForType(type);
                SelectSO(newInst);
            }
        }
    }
}
