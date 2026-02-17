using UnityEngine;
using UnityEditor;
using RomainUTR.SLToolbox;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomPropertyDrawer(typeof(SLMinMaxAttribute))]
    public class SLMinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.HelpBox(position, "[SLMinMax] need to be used with a Vector 2 (X = min, Y = max)", MessageType.Error);
                return;
            }

            SLMinMaxAttribute attr = (SLMinMaxAttribute)attribute;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int originalIntent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float floatFieldWidth = 50f;
            float padding = 5f;
            float sliderWidth = position.width - (floatFieldWidth * 2) - (padding * 2);

            Rect minRect = new Rect(position.x, position.y, floatFieldWidth, position.height);
            Rect sliderRect = new Rect(minRect.xMax + padding, position.y, sliderWidth, position.height);
            Rect maxRect = new Rect(sliderRect.xMax + padding, position.y, floatFieldWidth, position.height);

            float minVal = property.vector2Value.x;
            float maxVal = property.vector2Value.y;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, attr.MinLimit, attr.MaxLimit);

            maxVal = EditorGUI.FloatField(maxRect, float.Parse(maxVal.ToString("F2")));

            if (EditorGUI.EndChangeCheck())
            {
                minVal = Mathf.Clamp(minVal, attr.MinLimit, maxVal);
                maxVal = Mathf.Clamp(maxVal, minVal, attr.MaxLimit);

                property.vector2Value = new Vector2(minVal, maxVal);
            }

            EditorGUI.indentLevel = originalIntent;
        }
    }
}
