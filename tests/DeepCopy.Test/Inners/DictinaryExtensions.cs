using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepCopy.Test.Inners
{
    internal static class DictinaryExtensions
    {
        public static bool StructuralEquals<TKey, TValue>(this Dictionary<TKey, TValue> self, Dictionary<TKey, TValue> target)
        {
            var keys = self.Keys;
            var targetKeys = target.Keys;
            if (!keys.SequenceEqual(targetKeys)) return false;

            return self.All(x => x.Value.Equals(target[x.Key]));
        }
    }
}
