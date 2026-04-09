namespace DeepCopy
{
    public static class DeepCopyOptions
    {
        public static CopyBehavior ReadOnlyStructBehavior { get; set; } = CopyBehavior.TryClone;

        public enum CopyBehavior
        {
            Ignore,
            ShallowCopy,
            TryClone,
        }
    }
}
