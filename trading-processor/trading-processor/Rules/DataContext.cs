using trading_processor.Core;

namespace trading_processor.Rules;

public static class DataContext
{
    public static DataRule Data(this IRule parent, string filePath)
    {
        return Data(parent, File.OpenRead(filePath));
    }

    public static DataRule Data(this IRule parent, Stream stream)
    {
        return new DataRule(parent, stream);
    }
}