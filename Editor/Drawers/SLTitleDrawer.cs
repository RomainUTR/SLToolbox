using RomainUTR.SLToolbox;
using UnityEditor;
using UnityEngine;

namespace RomainUTR.SLToolbox
{
    [CustomPropertyDrawer(typeof(SLTitleAttribute))]
    public class SLTitleDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 30f;
        }

        public override void OnGUI(Rect position)
        {
            SLTitleAttribute attr = (SLTitleAttribute)attribute;

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 13;
            style.alignment = TextAnchor.LowerLeft;

            if (ColorUtility.TryParseHtmlString(attr.ColorHtml, out Color color))
            {
                style.normal.textColor = color;
            }

            Rect labelRect = new Rect(position.x, position.y + 5, position.width, 20);
            GUI.Label(labelRect, attr.Title, style);

            Rect lineRect = new Rect(position.x, position.y + 25, position.width, 2);
            EditorGUI.DrawRect(lineRect, new Color(1f, 1f, 1f, 0.3f));
        }
    }
}
