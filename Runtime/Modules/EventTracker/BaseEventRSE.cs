using System.Collections.Generic;
using UnityEngine;

namespace RomainUTR.SLToolbox.Runtime
{
    public struct EventLog
    {
        public float Timecode;
        public string CallerName;
        public string FileName;
        public string FullPath;
        public int LineNumber;
        public string Payload;
    }
    
    public abstract class BaseEventRSE : ScriptableObject
    {
        [System.NonSerialized]
        public List<EventLog> EventHistory = new List<EventLog>();
        
        protected void RecordEvent(string callerName, string sourceFilePath, int sourceLineNumber, string payload = "")
        {
#if UNITY_EDITOR
            string fileName = System.IO.Path.GetFileName(sourceFilePath);
        
            EventHistory.Add(new EventLog
            {
                Timecode = Time.time,
                CallerName = callerName,
                FileName = fileName,
                FullPath = sourceFilePath,
                LineNumber = sourceLineNumber,
                Payload = payload
            });
#endif
        }
        
        protected void RecordEvent<T>(string callerName, string sourceFilePath, int sourceLineNumber, T payload)
        {
#if UNITY_EDITOR
            string fileName = System.IO.Path.GetFileName(sourceFilePath);
            string payloadStr = payload != null ? payload.ToString() : "null";

            EventHistory.Add(new EventLog
            {
                Timecode = Time.time,
                CallerName = callerName,
                FileName = fileName,
                FullPath = sourceFilePath,
                LineNumber = sourceLineNumber,
                Payload = payloadStr
            });
#endif
        }
    }
}