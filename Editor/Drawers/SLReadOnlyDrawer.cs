using UnityEditor;
using UnityEngine;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomPropertyDrawer(typeof(SLReadOnlyAttribute))]
    public class SLReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}
