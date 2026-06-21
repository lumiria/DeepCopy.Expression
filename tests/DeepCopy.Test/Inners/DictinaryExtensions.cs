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

            for (int i = 0; i < self.Count; i++)
            {
                var key = self.Keys.ElementAt(i);
                var targetKey = target.Keys.ElementAt(i);

                if (!(self[key]?.Equals(target[targetKey]) ?? target[targetKey] == null)) return false;
            }
            return true;
        }

        public static bool StructuralEquals<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> target)
#if NET8_0_OR_GREATER
            where TKey : notnull
#endif
        {
            var keys = self.Keys;
            var targetKeys = target.Keys;
            if (!keys.SequenceEqual(targetKeys)) return false;

            for (int i = 0; i < self.Count; i++)
            {
                var key = self.Keys.ElementAt(i);
                var targetKey = target.Keys.ElementAt(i);

                if (!(self[key]?.Equals(target[targetKey]) ?? target[targetKey] == null)) return false;
            }
            return true;
        }
    }
}
