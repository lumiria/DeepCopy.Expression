extern alias DeepCopy121;
extern alias DeepCopy130;
extern alias DeepCopy142;
extern alias DeepCopy150;
using BenchmarkDotNet.Attributes;
using DeepCopy.Benchmark.Datas;

namespace DeepCopy.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    //[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
    //[SimpleJob(RuntimeMoniker.Net90)]
    public class Benchmark
    {
        private readonly Dictionary<int, double> _dict1;

        public Benchmark()
        {
            _dict1 = new()
            {
                { 1, 1.1 },
                { 2, 2.2 },
                { 3, 3.3 },
                { 4, 4.4 },
                { 5, 5.5 },
            };
        }

        [GlobalSetup]
        //[IterationSetup]
        public void Setup()
        {
            //Pre-build

            _ = DeepCopy121::DeepCopy.ObjectCloner.Clone(_dict1);
            _ = DeepCopy130::DeepCopy.ObjectCloner.Clone(_dict1);
            _ = DeepCopy142::DeepCopy.ObjectCloner.Clone(_dict1);
            _ = DeepCopy150::DeepCopy.ObjectCloner.Clone(_dict1);

            _ = ObjectCloner.Clone(_dict1);
        }

        [Benchmark]
        public void CloneWithV121()
        {
            var _ = DeepCopy121::DeepCopy.ObjectCloner.Clone(_dict1);
        }

        [Benchmark]
        public void CloneWithV130()
        {
            var _ = DeepCopy130::DeepCopy.ObjectCloner.Clone(_dict1);
        }

        [Benchmark]
        public void CloneWithV142()
        {
            var _ = DeepCopy142::DeepCopy.ObjectCloner.Clone(_dict1);
        }

        [Benchmark]
        public void CloneWithV150()
        {
            var _ = DeepCopy150::DeepCopy.ObjectCloner.Clone(_dict1);
        }


        [Benchmark(Baseline = true)]
        public void CloneWithLatest()
        {
            var _ = ObjectCloner.Clone(_dict1);
        }
    }
}
