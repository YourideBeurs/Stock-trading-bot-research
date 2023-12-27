using trading_processor.Core;

namespace trading_processor.Rules;

public class DataRule : IRule
{
    private readonly IRule _parent;
    private readonly Stream _stream;

    public DataRule(IRule parent, Stream stream)
    {
        _parent = parent;
        _stream = stream;
    }

    public CsvDataRule Csv()
    {
        return new CsvDataRule(this, _stream);
    }

    public void Run()
    {
        _parent.Run();
    }
}