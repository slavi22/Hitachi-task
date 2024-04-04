using Hitachi_task.Models;

namespace Hitachi_task.CsvHandler;

public class CsvWriter
{
    private readonly Dictionary<Island, Day> _dict;
    private readonly bool _isEnglish;

    public CsvWriter(Dictionary<Island, Day> dict, bool isEnglish)
    {
        _dict = dict;
        _isEnglish = isEnglish;
    }

    public string CreateCsv()
    {
        string path = "../../../CsvOutput/LaunchAnalysisReport.csv";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine(_isEnglish ? "Spaceport;Best day" : "Weltraumbahnhof;Bester Tag");
            foreach (var kvp in _dict)
            {
                writer.WriteLine($"{kvp.Key.Name};{kvp.Value.DayOfTheMonth};");
            }
        }

        return path;
    }
}