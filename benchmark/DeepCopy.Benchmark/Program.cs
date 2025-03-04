using BenchmarkDotNet.Running;
using DeepCopy.Benchmark;

var switcher = new BenchmarkSwitcher([
    typeof(DeepCopyBenchmark),
    typeof(DeepCopyFullyBenchmark),
    typeof(OneTimeBenchmark),
]);

args = ["0", "1", "2"];
switcher.Run(args);