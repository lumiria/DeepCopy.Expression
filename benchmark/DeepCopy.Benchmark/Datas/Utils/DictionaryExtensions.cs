namespace DeepCopy.Benchmark.Datas.Utils
{
    internal static class DictinaryExtensions
    {
        public static bool StructuralEquals<TKey, TValue>(this Dictionary<TKey, TValue> self, Dictionary<TKey, TValue> target)
            where TKey : notnull
        {
            var keys = self.Keys;
            var targetKeys = target.Keys;
            if (!keys.SequenceEqual(targetKeys)) return false;

            return self.All(x => x.Value is null && target[x.Key] is null || (x.Value?.Equals(target[x.Key]) ?? false));
        }
    }
}
