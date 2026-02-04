using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using RomainUTR.SLToolbox;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class GlobalSLEditor : UnityEditor.Editor
    {
        private Dictionary<string, UnityEditor.Editor> _cachedEditors = new Dictionary<string, UnityEditor.Editor>();

        private void OnDisable()
        {
            foreach(var editor in _cachedEditors.Values)
            {
                DestroyImmediate(editor);
            }
            _cachedEditors.Clear();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty prop = serializedObject.GetIterator();

            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.name == "m_Script")
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.PropertyField(prop);
                        }
                        continue;
                    }

                    if (HasInlineAttribute(prop))
                    {
                        DrawInlineEditor(prop);
                    } else
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }

                while (prop.NextVisible(false));
            }

            DrawSLButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private bool HasInlineAttribute(SerializedProperty prop)
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var field = target.GetType().GetField(prop.name,
                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null)
                {
                    return field.GetCustomAttribute<SLInlineAttribute>() != null;
                }
            }

            return false;
        }

        private void DrawInlineEditor(SerializedProperty prop)
        {
            EditorGUILayout.PropertyField(prop);

            if (prop.objectReferenceValue == null) return;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(GUI.skin.box);

            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, "Settings (Inline)");

            if (prop.isExpanded)
            {
                string key = prop.propertyPath;
                UnityEditor.Editor editor = null;

                if (!_cachedEditors.TryGetValue(key, out editor) || editor.target != prop.objectReferenceValue)
                {
                    if (editor != null) DestroyImmediate(editor);
                    editor = UnityEditor.Editor.CreateEditor(prop.objectReferenceValue);
                    _cachedEditors[key] = editor;
                }

                editor.OnInspectorGUI();
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        private void DrawSLButtons()
        {
            var targetType = target.GetType();

            var methods = targetType.GetMethods(
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<SLButtonAttribute>();

                if (attr != null)
                {
                    string label = string.IsNullOrEmpty(attr.ButtonText)
                        ? ObjectNames.NicifyVariableName(method.Name)
                        : attr.ButtonText;

                    float height = 30f;
                    if (attr.Size == SLButtonSize.Medium) height = 45f;
                    if (attr.Size == SLButtonSize.Large) height = 60f;

                    Color originalColor = GUI.backgroundColor;
                    if (attr.CustomColor.HasValue) GUI.backgroundColor = attr.CustomColor.Value;

                    if (GUILayout.Button(label, GUILayout.Height(height)))
                    {
                        foreach (var t in targets)
                        {
                            method.Invoke(t, null);
                        }
                    }

                    GUI.backgroundColor = originalColor;
                }
            }
        }
    }
}