namespace trading_processor.Models;

public class LongPosition : IPosition
{
    public PositionType Type => PositionType.Long;
    public double TakeProfit { get; set; }
    public double StopLoss { get; set; }
    public double OpenLong { get; set; }
    public double OpenShort { get; set; }
    public int Amount { get; set; }
    public DateTime Opened { get; set; }
    public DateTime Closed { get; set; }
    public double OpenPrice { get; set; }
    public double ClosePrice { get; set; }
    public double Profit => (ClosePrice - OpenPrice) * Amount;
}