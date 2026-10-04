using UnityEngine;
using RomainUTR.SLToolbox.Runtime;
using UnityEditor;

namespace RomainUTR.SLToolbox.Editor
{
    [CustomEditor(typeof(BaseEventRSE), true)]
    public class EventTracker : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            BaseEventRSE rse = (BaseEventRSE)target;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Go to Play mode to view your call history.",  MessageType.Info);
                return;
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Debug History", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                rse.EventHistory.Clear();
            }
            EditorGUILayout.EndHorizontal();

            if (rse.EventHistory.Count == 0) return;
            
            for (int i = rse.EventHistory.Count - 1; i >= 0; i--)
            {
                EventLog log = rse.EventHistory[i];
                
                EditorGUILayout.BeginVertical("box");
                
                string displayPayload = string.IsNullOrEmpty(log.Payload) ? "()" : $"  =>  {log.Payload}";
                EditorGUILayout.LabelField($"[{log.Timecode:F2}s] {log.CallerName}{displayPayload}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"{log.FileName} : ligne {log.LineNumber}");
                
                if (GUILayout.Button("Open Editor"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(log.FullPath, log.LineNumber);
                }
                
                EditorGUILayout.EndVertical();
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;   
        }
    }
}
