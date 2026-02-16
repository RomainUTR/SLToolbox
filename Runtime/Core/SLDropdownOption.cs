using UnityEngine;

namespace RomainUTR.SLToolbox
{
    public class SLDropdownOption
    {
        public string Label;
        public object Value;

        public SLDropdownOption(string label, object value)
        {
            this.Label = label;
            this.Value = value;
        }
    }
}
