extern alias DeepCopy121;
extern alias DeepCopy130;
extern alias DeepCopy142;
extern alias DeepCopy150;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DeepCopy.Benchmark.Datas;

namespace DeepCopy.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    //[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
    //[SimpleJob(RuntimeMoniker.Net90)]
    public class DeepCopyBenchmark
    {
        private readonly TestObject _object;

        public DeepCopyBenchmark()
        {
            _object = new TestObject();
        }

        [GlobalSetup]
        //[IterationSetup]
        public void Setup()
        {
            //Pre-build

            _ = DeepCopy121::DeepCopy.ObjectCloner.Clone(_object);
            _ = DeepCopy130::DeepCopy.ObjectCloner.Clone(_object);
            _ = DeepCopy142::DeepCopy.ObjectCloner.Clone(_object);
            _ = DeepCopy150::DeepCopy.ObjectCloner.Clone(_object);

            _ = ObjectCloner.Clone(_object);
        }

        [Benchmark]
        public void CloneWithV121()
        {
            var _ = DeepCopy121::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark]
        public void CloneWithV130()
        {
            var _ = DeepCopy130::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark]
        public void CloneWithV142()
        {
            var _ = DeepCopy142::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark]
        public void CloneWithV150()
        {
            var _ = DeepCopy150::DeepCopy.ObjectCloner.Clone(_object);
        }

        [Benchmark(Baseline = true)]
        public void CloneWithLatest()
        {
            var _ = ObjectCloner.Clone(_object);
        }
    }
}
