using System;
using UnityEngine;

namespace RomainUTR.SLToolbox
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class SLTitleAttribute : PropertyAttribute
    {
        public string Title { get; set; }
        public string ColorHtml { get; set; }

        public SLTitleAttribute(string title, string colorHtml = "#FFFFFF")
        {
            this.Title = title;
            this.ColorHtml = colorHtml;
        }
    }
}
