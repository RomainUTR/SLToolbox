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
            foreach (var editor in _cachedEditors.Values)
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

                    EditorGUI.BeginChangeCheck();

                    Color defaultColor = GUI.backgroundColor;
                    bool isRequiredError = IsRequiredAndEmpty(prop);

                    if (isRequiredError)
                    {
                        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                    }

                    if (HasInlineAttribute(prop))
                    {
                        DrawInlineEditor(prop);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }

                    GUI.backgroundColor = defaultColor;

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();

                        var callbackAttr = GetAttribute<SLCallbackAttribute>(prop);
                        if (callbackAttr != null)
                        {
                            MethodInfo method = GetMethodViaReflection(target.GetType(), callbackAttr.MethodName);
                            if (method != null)
                            {
                                method.Invoke(target, null);
                            }
                        }
                    }
                }
                while (prop.NextVisible(false));
            }

            DrawSLButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsRequiredAndEmpty(SerializedProperty prop)
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (prop.objectReferenceValue == null)
                {
                    return GetAttribute<SLRequiredAttribute>(prop) != null;
                }
            }
            else if (prop.type == "SceneReference")
            {
                var innerProp = prop.FindPropertyRelative("sceneAsset");

                if (innerProp != null && innerProp.objectReferenceValue == null)
                {
                    return GetAttribute<SLRequiredAttribute>(prop) != null;
                }
            }

            return false;
        }

        private bool HasInlineAttribute(SerializedProperty prop)
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                return GetAttribute<SLInlineAttribute>(prop) != null;
            }
            return false;
        }

        private void DrawInlineEditor(SerializedProperty prop)
        {
            EditorGUILayout.PropertyField(prop);

            if (prop.objectReferenceValue == null) return;

            GUILayout.Space(-4f);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            UnityEditor.Editor editor = null;
            string key = prop.propertyPath;

            if (!_cachedEditors.TryGetValue(key, out editor) || editor.target != prop.objectReferenceValue)
            {
                if (editor != null) DestroyImmediate(editor);
                editor = UnityEditor.Editor.CreateEditor(prop.objectReferenceValue);
                _cachedEditors[key] = editor;
            }

            prop.isExpanded = EditorGUILayout.InspectorTitlebar(prop.isExpanded, editor.target);

            EditorGUI.indentLevel = oldIndent;

            if (prop.isExpanded)
            {
                editor.serializedObject.Update();
                SerializedProperty innerProp = editor.serializedObject.GetIterator();

                if (innerProp.NextVisible(true))
                {
                    do
                    {
                        if (innerProp.name == "m_Script") continue;
                        EditorGUILayout.PropertyField(innerProp, true);
                    }
                    while (innerProp.NextVisible(false));
                }
                editor.serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawSLButtons()
        {
            var methods = target.GetType().GetMethods(
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

        private T GetAttribute<T>(SerializedProperty prop) where T : System.Attribute
        {
            FieldInfo field = GetFieldViaReflection(target.GetType(), prop.name);
            if (field == null) return null;
            return field.GetCustomAttribute<T>();
        }

        private FieldInfo GetFieldViaReflection(System.Type type, string fieldName)
        {
            if (type == null) return null;
            FieldInfo field = type.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null) return field;
            return GetFieldViaReflection(type.BaseType, fieldName);
        }

        private MethodInfo GetMethodViaReflection(System.Type type, string methodName)
        {
            if (type == null) return null;
            MethodInfo method = type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null) return method;
            return GetMethodViaReflection(type.BaseType, methodName);
        }
    }
}