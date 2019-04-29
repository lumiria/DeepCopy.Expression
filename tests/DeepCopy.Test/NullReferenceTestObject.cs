using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DeepCopy.Test
{
    public class NullReferenceTestObject
    {
        public string Value1 { get; }

        public List<int> Value2 { get; set; }

        public ObservableCollection<Child> Value3 { get; set; }
    }
}
