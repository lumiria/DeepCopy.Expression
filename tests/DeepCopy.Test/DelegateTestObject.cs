using System;

namespace DeepCopy.Test
{
    internal sealed class DelegateTestObject
    {
        public DelegateTestObject(Action action)
        {
            TestAction = action;
        }

        public event EventHandler TestEvent;

        public Action TestAction { get; }

        public bool IsEventEmpty =>
            TestEvent == null;
    }
}
