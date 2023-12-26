using trading_processor.Models;

namespace trading_processor.Utils;

public static class ListUtils
{
    public static void ToCsv(this List<IPosition> list, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        var properties = typeof(IPosition).GetProperties();

        writer.WriteLine(string.Join(";", Array.ConvertAll(properties, prop => prop.Name)));

        foreach (var position in list)
        {
            writer.WriteLine(string.Join(";", Array.ConvertAll(properties, prop => prop.GetValue(position, null))));
        }
        
        writer.Dispose();
    }
}