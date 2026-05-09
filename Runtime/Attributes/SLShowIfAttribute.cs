using System;
using UnityEngine;

namespace RomainUTR.SLToolbox.Runtime
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SLShowIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; private set; }

        public SLShowIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }
    }
}