using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RomainUTR.SLToolbox.BetterUnity.Runtime
{
    public static class IEnumerableUtils
    {
        public static T GetRandom<T>(this IEnumerable<T> elems)
        {
            if (!elems.Any())
            {
                return default(T);
            }

            return elems.ElementAt(Random.Range(0, elems.Count()));
        }
    }
}