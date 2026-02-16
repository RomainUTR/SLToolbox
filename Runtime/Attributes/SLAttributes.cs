using UnityEngine;

namespace RomainUTR.SLToolbox
{
    public class SLDropdownAttribute : PropertyAttribute
    {
        public string MethodName;
        public SLDropdownAttribute(string methodName) { this.MethodName = methodName; }
    }

    public class SLCallbackAttribute : PropertyAttribute
    {
        public string MethodName;
        public SLCallbackAttribute(string methodName) { this.MethodName = methodName; }
    }
    
    public enum SLInfoBoxType
    {
        Normal,
        Warning,
        Error
    }

    public class SLInfoBoxAttribute : PropertyAttribute
    {
        public string Text;
        public SLInfoBoxType Type;

        public SLInfoBoxAttribute(string text, SLInfoBoxType type = SLInfoBoxType.Normal)
        {
            this.Text = text;
            this.Type = type;
        }
    }
}
