using UnityEngine;

namespace RomainUTR.SLToolbox
{
    public class SLMinMaxAttribute : PropertyAttribute
    {
        public float MinLimit;
        public float MaxLimit;

        public SLMinMaxAttribute(float minLimit, float maxLimit)
        {
            this.MinLimit = minLimit;
            this.MaxLimit = maxLimit;
        }
    }
}
