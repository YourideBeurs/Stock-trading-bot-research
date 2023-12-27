using trading_processor.Rules;

namespace trading_processor.Core;

public static class Dsl
{
    public static TraderContext Trader()
    {
        return new TraderContext();
    }

    public static LongRule AllowLong(this IRule parent, bool allow = true)
    {
        return new LongRule(parent);
    }

    public static StopwatchRule WithStopwatch(this IRule parent)
    {
        return new StopwatchRule(parent);
    }
}