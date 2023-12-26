namespace trading_processor.Models;

public interface IPosition
{
    public PositionType Type { get; }
    public double TakeProfit { get; set; }
    public double StopLoss { get; set; }
    public double OpenLong { get; set; }
    public double OpenShort { get; set; }
    public int Amount { get; set; }
    public DateTime Opened { get; set; }
    public DateTime Closed { get; set; }
    public double OpenPrice { get; set; }
    public double ClosePrice { get; set; }
    public double Profit { get; }
}