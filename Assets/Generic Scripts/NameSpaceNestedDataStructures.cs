using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.DataStructure
{
    public static class GlobalDataStructures
    {
        public static Dictionary<string, Dictionary<int,GameObject>> StoreItemsContainer = new Dictionary<string, Dictionary<int,GameObject>>(StringComparer.Ordinal);
    }
}
