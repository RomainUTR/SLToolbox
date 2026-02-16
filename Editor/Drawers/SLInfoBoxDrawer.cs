using UnityEngine;
using UnityEditor;
using RomainUTR.SLToolbox;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomPropertyDrawer(typeof(SLInfoBoxAttribute))]
    public class SLInfoBoxDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            SLInfoBoxAttribute attr = (SLInfoBoxAttribute)attribute;
            
            float height = EditorStyles.helpBox.CalcHeight(new GUIContent(attr.Text), EditorGUIUtility.currentViewWidth);
            
            return height + 6f;
        }

        public override void OnGUI(Rect position)
        {
            SLInfoBoxAttribute attr = (SLInfoBoxAttribute)attribute;

            MessageType messageType = MessageType.Info;
            switch(attr.Type)
            {
                case SLInfoBoxType.Warning: messageType = MessageType.Warning; break;
                case SLInfoBoxType.Error: messageType = MessageType.Error; break;
            }

            position.y += 2f;
            position.height -= 4f;

            EditorGUI.HelpBox(position, attr.Text, messageType);
        }
    }
}