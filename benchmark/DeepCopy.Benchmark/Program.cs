using BenchmarkDotNet.Running;
using DeepCopy.Benchmark;

var switcher = new BenchmarkSwitcher([
    typeof(DeepCopyBenchmark),
    typeof(OneTimeBenchmark),
]);

args = ["0", "1"];
switcher.Run(args);