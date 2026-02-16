using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RomainUTR.SLToolbox;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomPropertyDrawer(typeof(SLDropdownAttribute))]
    public class SLDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SLDropdownAttribute attr = (SLDropdownAttribute)attribute;
            object target = property.serializedObject.targetObject;

            MethodInfo method = target.GetType().GetMethod(attr.MethodName, 
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null)
            {
                EditorGUI.LabelField(position, label.text, $"Method '{attr.MethodName}' not found !");
                return;
            }

            var options = method.Invoke(target, null) as IEnumerable<SLDropdownOption>;

            if (options == null)
            {
                EditorGUI.LabelField(position, label.text, "This method need to return IEnumerable<SLDropdownOption>");
                return;
            }

            var optionList = options.ToList();
            string[] displayedOptions = optionList.Select(x => x.Label).ToArray();

            int currentIndex = 0;
            
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                int currentVal = property.intValue;
                currentIndex = optionList.FindIndex(x => x.Value is int i && i == currentVal);
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                string currentVal = property.stringValue;
                currentIndex = optionList.FindIndex(x => x.Value.ToString() == currentVal);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "SLDropdown works only with int or string !");
                return;
            }

            if (currentIndex == -1) currentIndex = 0;

            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayedOptions);

            if (newIndex >= 0 && newIndex < optionList.Count)
            {
                object newValue = optionList[newIndex].Value;

                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = System.Convert.ToInt32(newValue);
                }
                else if (property.propertyType == SerializedPropertyType.String)
                {
                    property.stringValue = newValue.ToString();
                }
            }
        }
    }
}