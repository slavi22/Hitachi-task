using Hitachi_task.Islands;

namespace Hitachi_task.CsvHandler;

public class CsvWriter
{
    private readonly Dictionary<Island, Day> _dict;

    public CsvWriter(Dictionary<Island, Day> dict)
    {
        _dict = dict;
    }

    public string CreateCsv()
    {
        string path = "../../../CsvOutput/LaunchAnalysisReport.csv";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Spaceport;Best day");
            foreach (var kvp in _dict)
            {
                writer.WriteLine($"{kvp.Key.Name};{kvp.Value.DayOfTheMonth};");
            }
        }

        return path;
    }
}