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

    public static void ToCsv(this List<Program.Position> list, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        var properties = typeof(Program.Position).GetProperties();

        writer.WriteLine(string.Join(";", Array.ConvertAll(properties, prop => prop.Name)));

        foreach (var position in list)
        {
            writer.WriteLine(string.Join(";", Array.ConvertAll(properties, prop => prop.GetValue(position, null))));
        }

        writer.Dispose();
    }
    
    public static void ToCsv(this List<Settings> list, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        var cultureInfo = new System.Globalization.CultureInfo("nl-NL");
        
        var properties = typeof(Settings).GetProperties();

        writer.WriteLine(string.Join(";", Array.ConvertAll(properties, prop => prop.Name)));

        foreach (var position in list)
        {
            writer.WriteLine(string.Join(";", Array.ConvertAll(properties, prop => 
            {
                var value = prop.GetValue(position, null);
                if (value is double)
                {
                    return ((double)value).ToString(cultureInfo);
                }
                return value.ToString();
            })));
        }

        writer.Dispose();
    }
}