using UnityEngine;
using UnityEditor;
using System.Reflection;
using RomainUTR.SLToolbox;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class GlobalSLEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawSLButtons();
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