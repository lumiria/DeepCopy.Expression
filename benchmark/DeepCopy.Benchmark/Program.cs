using BenchmarkDotNet.Running;
using DeepCopy.Benchmark;

var switcher = new BenchmarkSwitcher([
    typeof(DeepCopyBenchmark),
    typeof(DeepCopyFullyBenchmark),
    typeof(OneTimeBenchmark),
    typeof(Benchmark),
]);

args = ["0", "1", "2", "3"];
switcher.Run(args);