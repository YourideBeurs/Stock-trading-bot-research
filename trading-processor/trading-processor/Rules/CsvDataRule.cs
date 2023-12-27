using trading_processor.Core;

namespace trading_processor.Rules;

public class CsvDataRule : IRule
{
    private string _separator;
    private readonly Stream _stream;
    private readonly IRule _parent;
    private int _timestamp;
    private int _sharePrice;

    private readonly List<Action> _actions;

    public CsvDataRule(IRule parent, Stream stream)
    {
        _stream = stream;
        _parent = parent;
        _actions = new List<Action>();
    }

    public CsvDataRule Separator(string separator)
    {
        _actions.Add(() => _separator = separator);
        return this;
    }

    public CsvDataRule Timestamp(int column)
    {
        _actions.Add(() => _timestamp = column);
        return this;
    }

    public CsvDataRule SharePrice(int column)
    {
        _actions.Add(() => _sharePrice = column);
        return this;
    }

    public void Run()
    {
        _actions.ForEach(action => action.Invoke());
        _parent.Run();
    }
}