using trading_processor.Core;

namespace trading_processor.Rules;

public class StopwatchRule : IRule
{
    private readonly IRule _parent;

    public StopwatchRule(IRule parent)
    {
        _parent = parent;
    }
    public void Run()
    {
        _parent.Run();
    }
}