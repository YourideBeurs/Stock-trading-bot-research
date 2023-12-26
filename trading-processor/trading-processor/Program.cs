using System.Diagnostics;

namespace trading_processor;

public static class Program
{
    private const string FilePath = "C:\\Development\\research\\Stock-trading-bot-research\\trading-processor\\data\\output.csv";
    private const double Cash = 100000;
    
    public static void Main()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var processor = new Processor(FilePath, Cash);
        processor.Run();
        stopwatch.Stop();
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");
    }
}