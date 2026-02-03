using System;
using UnityEngine;

namespace RomainUTR.SLToolbox
{
    public enum SLButtonSize { Small, Medium, Large }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SLButtonAttribute : PropertyAttribute
    {
        public string ButtonText { get; private set; }
        public SLButtonSize Size { get; private set; }
        public Color? CustomColor { get; private set; }

        public SLButtonAttribute(string text = null, SLButtonSize size = SLButtonSize.Small)
        {
            this.ButtonText = text;
            this.Size = size;
            this.CustomColor = null;
        }

        public SLButtonAttribute(string text, float r, float g, float b)
        {
            this.ButtonText = text;
            this.Size = SLButtonSize.Medium;
            this.CustomColor = new Color(r, g, b);
        }
    }
}