extern alias DeepCopy121;
extern alias DeepCopy130;
extern alias DeepCopy142;
extern alias DeepCopy150;

using BenchmarkDotNet.Attributes;
using DeepCopy.Benchmark.Datas;

namespace DeepCopy.Benchmark
{
    public class OneTimeBenchmark
    {
        private readonly TestObject _object;

        public OneTimeBenchmark()
        {
            _object = new TestObject();
        }

        [IterationSetup]
        public void Setup()
        {
            ObjectCloner.Cleanup<TestObject>();
            ObjectCloner.Cleanup<Child>();
            ObjectCloner.Cleanup<List<int>>();
            ObjectCloner.Cleanup<Dictionary<int, string>>();

            DeepCopy121::DeepCopy.ObjectCloner.Cleanup<TestObject>();
            DeepCopy121::DeepCopy.ObjectCloner.Cleanup<Child>();
            DeepCopy121::DeepCopy.ObjectCloner.Cleanup<List<int>>();
            DeepCopy121::DeepCopy.ObjectCloner.Cleanup<Dictionary<int, string>>();

            DeepCopy130::DeepCopy.ObjectCloner.Cleanup<TestObject>();
            DeepCopy130::DeepCopy.ObjectCloner.Cleanup<Child>();
            DeepCopy130::DeepCopy.ObjectCloner.Cleanup<List<int>>();
            DeepCopy130::DeepCopy.ObjectCloner.Cleanup<Dictionary<int, string>>();

            DeepCopy142::DeepCopy.ObjectCloner.Cleanup<TestObject>();
            DeepCopy142::DeepCopy.ObjectCloner.Cleanup<Child>();
            DeepCopy142::DeepCopy.ObjectCloner.Cleanup<List<int>>();
            DeepCopy142::DeepCopy.ObjectCloner.Cleanup<Dictionary<int, string>>();

            DeepCopy150::DeepCopy.ObjectCloner.Cleanup<TestObject>();
            DeepCopy150::DeepCopy.ObjectCloner.Cleanup<Child>();
            DeepCopy150::DeepCopy.ObjectCloner.Cleanup<List<int>>();
            DeepCopy150::DeepCopy.ObjectCloner.Cleanup<Dictionary<int, string>>();
        }

        [Benchmark]
        public void CloneWithV121()
        {
            var cloned = DeepCopy121::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark]
        public void CloneWithV130()
        {
            var cloned = DeepCopy130::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark]
        public void CloneWithV142()
        {
            var cloned = DeepCopy142::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark(Baseline = true)]
        public void CloneWithLatest()
        {
            var cloned = DeepCopy.ObjectCloner.Clone(_object);
        }
    }
}
