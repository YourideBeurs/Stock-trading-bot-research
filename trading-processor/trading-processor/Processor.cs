// using System.Globalization;
// using trading_processor.Models;
// using trading_processor.Utils;
//
// namespace trading_processor;
//
// public class Processor
// {
//     private readonly List<IPosition> _openPositions;
//     private readonly List<IPosition> _closedPositions;
//     private readonly string _filePath;
//
//     private const double LongTakeProfit = 1.008;
//     private const double LongStopLoss = 0.992;
//
//     private const double ShortTakeProfit = 0.992;
//     private const double ShortStopLoss = 1.008;
//
//     private const double OpenLong = 1.005;
//     private const double OpenShort = 0.995;
//
//     private double _cash;
//
//     private const double PositionCash = 1000;
//
//     private const string Seperator = ",";
//
//     private const int MaxOpenTrades = 4;
//
//     private const string OpenPositionsPath =
//         "C:\\Development\\research\\Stock-trading-bot-research\\trading-processor\\data\\open.csv";
//
//     private const string ClosedPositionsPath =
//         "C:\\Development\\research\\Stock-trading-bot-research\\trading-processor\\data\\closed.csv";
//
//     public Processor(string filePath, double cash)
//     {
//         _openPositions = new List<IPosition>();
//         _closedPositions = new List<IPosition>();
//         _filePath = filePath;
//         _cash = cash;
//     }
//
//     public void Run()
//     {
//         Console.WriteLine($"Cash is currently: ${_cash:N2}");
//
//         using var reader = File.OpenText(_filePath);
//         var line = reader.ReadLine();
//         var lastLine = line.Split(Seperator);
//         while ((line = reader.ReadLine()) != null)
//         {
//             var values = line.Split(Seperator);
//             if (_openPositions.Count == 0)
//             {
//                 OpenPosition(DateTime.Parse(values[0]), double.Parse(values[1], CultureInfo.InvariantCulture),
//                     PositionType.LONG);
//                 OpenPosition(DateTime.Parse(values[0]), double.Parse(values[1], CultureInfo.InvariantCulture),
//                     PositionType.SHORT);
//             }
//
//             CheckOpenPositions(DateTime.Parse(values[0]), double.Parse(values[1], CultureInfo.InvariantCulture));
//             lastLine = line.Split(Seperator);
//         }
//
//         CloseRemainingPositions(DateTime.Parse(lastLine[0]), double.Parse(lastLine[1], CultureInfo.InvariantCulture));
//
//         _openPositions.ToCsv(OpenPositionsPath);
//         _closedPositions.ToCsv(ClosedPositionsPath);
//         Console.WriteLine($"Cash is currently: ${_cash:N}");
//     }
//
//     public void CheckOpenPositions(DateTime timestamp, double sharePrice)
//     {
//         var openLong = false;
//         var openShort = false;
//         var copy = new List<IPosition>(_openPositions);
//
//         foreach (var position in copy)
//         {
//             if (position.Type == PositionType.Long)
//             {
//                 if (sharePrice >= position.TakeProfit)
//                 {
//                     ClosePosition(timestamp, sharePrice, position);
//                 }
//                 else if (sharePrice >= position.OpenLong)
//                 {
//                     openLong = true;
//                 }
//
//                 if (sharePrice <= position.StopLoss)
//                 {
//                     ClosePosition(timestamp, sharePrice, position);
//                 }
//                 else if (sharePrice <= position.OpenShort)
//                 {
//                     openShort = true;
//                 }
//             }
//             else
//             {
//                 if (sharePrice <= position.TakeProfit)
//                 {
//                     ClosePosition(timestamp, sharePrice, position);
//                 }
//                 else if (sharePrice <= position.OpenShort)
//                 {
//                     openShort = true;
//                 }
//
//                 if (sharePrice >= position.StopLoss)
//                 {
//                     ClosePosition(timestamp, sharePrice, position);
//                 }
//                 else if (sharePrice >= position.OpenLong)
//                 {
//                     openLong = true;
//                 }
//             }
//         }
//
//         if (openLong)
//         {
//             OpenPosition(timestamp, sharePrice, PositionType.LONG);
//         }
//
//         if (openShort)
//         {
//             OpenPosition(timestamp, sharePrice, PositionType.SHORT);
//         }
//     }
//
//     public void OpenPosition(DateTime timestamp, double sharePrice, PositionType type)
//     {
//         if (_openPositions.Count != MaxOpenTrades)
//         {
//             IPosition position;
//             if (type == PositionType.LONG)
//             {
//                 position = new LongPosition()
//                 {
//                     TakeProfit = sharePrice * LongTakeProfit,
//                     StopLoss = sharePrice * LongStopLoss,
//                 };
//             }
//             else
//             {
//                 position = new ShortPosition()
//                 {
//                     TakeProfit = sharePrice * ShortTakeProfit,
//                     StopLoss = sharePrice * ShortStopLoss,
//                 };
//             }
//
//             position.Opened = timestamp;
//             position.OpenLong = sharePrice * OpenLong;
//             position.OpenShort = sharePrice * OpenShort;
//             position.Amount = (int) Math.Floor(PositionCash / sharePrice);
//             position.OpenPrice = sharePrice;
//
//             _cash -= sharePrice * position.Amount;
//             _openPositions.Add(position);
//         }
//     }
//
//     public void ClosePosition(DateTime timestamp, double sharePrice, IPosition position)
//     {
//         if (position.Type == PositionType.)
//         {
//             _cash = sharePrice * position.Amount + _cash;
//         }
//         else
//         {
//             _cash = position.OpenPrice * position.Amount + _cash;
//         }
//
//         position.Closed = timestamp;
//         position.ClosePrice = sharePrice;
//
//         _closedPositions.Add(position);
//         _openPositions.Remove(position);
//     }
//
//     public void CloseRemainingPositions(DateTime timestamp, double sharePrice)
//     {
//         var copy = new List<IPosition>(_openPositions);
//         foreach (var position in copy)
//         {
//             ClosePosition(timestamp, sharePrice, position);
//         }
//     }
// }