using trading_processor.Core;

namespace trading_processor.Rules;

public class LongRule : IRule
{
    private readonly IRule _parent;

    public LongRule(IRule parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        _parent.Run();
    }
}