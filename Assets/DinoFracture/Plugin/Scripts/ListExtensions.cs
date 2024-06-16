using System.Collections.Generic;
using UnityEngine;

namespace DinoFracture
{
    public static class ListExtensions
    {
        public static void RemoveFastAt<T>(this List<T> list, int index)
        {
            Debug.Assert(index >= 0 && index < list.Count);

            if (index < list.Count - 1)
            {
                list[index] = list[list.Count - 1];
            }

            list.RemoveAt(list.Count - 1);
        }
    }
}