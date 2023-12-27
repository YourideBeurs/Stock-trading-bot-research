using static trading_processor.Utils.ListUtils;

namespace trading_processor;

public static class Program
{
    public static void Main()
    {
        var counter = 0;
        var list = new List<Settings>();
        var filePath = "C:\\Eigen data\\Research\\Stock research\\settings.csv";
        var outputFilePath = "C:\\Eigen data\\Research\\Stock research\\settings.csv";
        using var streamReader = new StreamReader(File.OpenRead(filePath));
        while (streamReader.ReadLine() is { } line)
        {
            // Skip header line
            if (counter > 0)
            {
                var values = line.Split(";");
                var settings = new Settings()
                {
                    ShortTakeProfit = double.Parse(values[0]),
                    ShortStopLoss = double.Parse(values[1]),
                    OpenNewShort = double.Parse(values[2]),
                    OpenNewLong = double.Parse(values[3]),
                    LongTakeProfit = double.Parse(values[4]),
                    LongStopLoss = double.Parse(values[5]),
                };
                settings.Cash = Start(settings);
                list.Add(settings);
            }

            counter++;
        }

        streamReader.Dispose();

        list.ToCsv(outputFilePath);
    }

    public static double Start(Settings settings)
    {
        var separator = ";";
        var cash = 100000D;
        int counter = 0;
        var amount = 5;
        var lastSharePrice = 0D;
        var positions = new List<Position>();

        var shortTakeProfit = settings.ShortTakeProfit;
        var shortStopLoss = settings.ShortStopLoss;

        var openNewShort = settings.OpenNewShort;
        var openNewLong = settings.OpenNewLong;

        var longTakeProfit = settings.LongTakeProfit;
        var longStopLoss = settings.LongStopLoss;

        // var shortTakeProfit = 0.03;
        // var shortStopLoss = 0.03;
        //
        // var openNewShort = 0.015;
        // var openNewLong = 0.015;
        //
        // var longTakeProfit = 0.03;
        // var longStopLoss = 0.03;

        var filePath = "C:\\Eigen data\\Research\\Stock research\\data.csv";
        var outputFilePath = "C:\\Eigen data\\Research\\Stock research\\output.csv";
        using var streamReader = new StreamReader(File.OpenRead(filePath));
        while (streamReader.ReadLine() is { } line)
        {
            // Skip header line
            if (counter > 0)
            {
                //2022-12-27 09:30:00
                // Split line
                var values = line.Split(separator);
                var timestamp = DateTimeOffset.Parse(values[0]);
                var sharePrice = double.Parse(values[1]);
                lastSharePrice = sharePrice;

                // Create first long position
                if (counter == 1)
                {
                    positions.Add(new Position()
                    {
                        Type = PositionType.LONG,
                        OpenPrice = sharePrice,
                        OpenTimestamp = timestamp,
                        Amount = 0,
                    });
                }

                // Create empty long position
                if (positions.All(o => o.Status != PositionStatus.OPEN))
                {
                    positions.Add(new Position()
                    {
                        Type = PositionType.LONG,
                        OpenPrice = sharePrice,
                        OpenTimestamp = timestamp,
                        Amount = 0,
                    });
                }

                var openShort = false;
                var openLong = false;

                // Check open positions
                foreach (var position in positions)
                {
                    var close = false;

                    if (position.Status == PositionStatus.OPEN)
                    {
                        if (position.Type == PositionType.LONG)
                        {
                            // Close long position with profit
                            if (sharePrice >= position.OpenPrice * (1 + longTakeProfit))
                            {
                                position.ClosePrice = sharePrice;
                                close = true;
                            }
                            // Open new long position
                            else if (sharePrice >= position.OpenPrice * (1 + openNewLong))
                            {
                                if (!position.LongOpened)
                                {
                                    openLong = true;
                                    position.LongOpened = true;
                                }
                            }
                            // Close long position with loss
                            else if (sharePrice <= position.OpenPrice * (1 - longStopLoss))
                            {
                                position.ClosePrice = sharePrice;
                                close = true;
                            }
                            // Open new short position
                            else if (sharePrice <= position.OpenPrice * (1 - openNewShort))
                            {
                                if (!position.ShortOpened)
                                {
                                    openShort = true;
                                    position.ShortOpened = true;
                                }
                            }
                        }
                        else
                        {
                            // Close short position with profit
                            if (sharePrice <= position.ClosePrice * (1 - shortTakeProfit))
                            {
                                position.OpenPrice = sharePrice;
                                close = true;
                            }
                            // Open new short position
                            else if (sharePrice <= position.ClosePrice * (1 - openNewShort))
                            {
                                if (!position.LongOpened)
                                {
                                    openLong = true;
                                    position.LongOpened = true;
                                }
                            }
                            // Close short position with loss
                            else if (sharePrice >= position.ClosePrice * (1 + shortStopLoss))
                            {
                                position.OpenPrice = sharePrice;
                                close = true;
                            }
                            // Open new long position
                            else if (sharePrice >= position.ClosePrice * (1 + openNewLong))
                            {
                                if (!position.ShortOpened)
                                {
                                    openShort = true;
                                    position.ShortOpened = true;
                                }
                            }
                        }
                    }

                    if (close)
                    {
                        if (position.Type == PositionType.LONG)
                        {
                            position.ClosePrice = sharePrice;
                        }
                        else
                        {
                            position.OpenPrice = sharePrice;
                        }

                        position.CloseTimestamp = timestamp;
                        position.Status = PositionStatus.CLOSED;
                    }
                }

                if (openLong)
                {
                    positions.Add(new Position()
                    {
                        Type = PositionType.LONG,
                        OpenPrice = sharePrice,
                        OpenTimestamp = timestamp,
                        Amount = amount,
                    });
                }

                if (openShort)
                {
                    positions.Add(new Position()
                    {
                        Type = PositionType.SHORT,
                        ClosePrice = sharePrice,
                        OpenTimestamp = timestamp,
                        Amount = amount,
                    });
                }
            }

            counter++;
        }

        foreach (var position in positions)
        {
            if (position.Status == PositionStatus.OPEN)
            {
                if (position.Type == PositionType.LONG)
                {
                    position.ClosePrice = lastSharePrice;
                }
                else
                {
                    position.OpenPrice = lastSharePrice;
                }
            }

            cash += (position.Amount * (position.ClosePrice - position.OpenPrice));
        }

        positions.ToCsv(outputFilePath);
        Console.WriteLine($"Cash: ${cash:N}");

        return cash;
    }

    public class Position
    {
        public PositionType Type { get; set; }
        public int Amount { get; set; }
        public DateTimeOffset OpenTimestamp { get; set; }
        public DateTimeOffset CloseTimestamp { get; set; }
        public double OpenPrice { get; set; }
        public double ClosePrice { get; set; }
        public PositionStatus Status { get; set; } = PositionStatus.OPEN;
        public bool ShortOpened { get; set; }
        public bool LongOpened { get; set; }
        public double Profit => (ClosePrice - OpenPrice) * Amount;
    }
}

public enum PositionType
{
    LONG,
    SHORT
}

public enum PositionStatus
{
    OPEN,
    CLOSED
}

public class Settings
{
    public double ShortTakeProfit { get; set; }
    public double ShortStopLoss { get; set; }
    public double OpenNewShort { get; set; }
    public double OpenNewLong { get; set; }
    public double LongTakeProfit { get; set; }
    public double LongStopLoss { get; set; }
    public double Cash { get; set; }
}