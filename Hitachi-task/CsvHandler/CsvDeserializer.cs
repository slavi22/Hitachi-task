using Hitachi_task.Islands;
using Microsoft.VisualBasic.FileIO;

namespace Hitachi_task.CsvHandler;

public static class CsvDeserializer
{
    public static Dictionary<Island, List<string[]>> Csv(string pathToFolder)
    {
        string[] filePaths = Directory.GetFiles(pathToFolder).OrderBy(x =>
        {
            if (x.EndsWith("Data-Kourou.csv"))
                return 1;
            if (x.EndsWith("Data-Tanegashima.csv"))
                return 2;
            if (x.EndsWith("Data-Cape Canaveral.csv"))
                return 3;
            if (x.EndsWith("Data-Mahia.csv"))
                return 4;
            if (x.EndsWith("Data-Kodiak.csv"))
                return 5;
            return 0;
        }).ToArray();
        Dictionary<Island, List<string[]>> dict = new Dictionary<Island, List<string[]>>();
        int rank = 1;
        foreach (var filePath in filePaths)
        {
            string keyName = Path.GetFileName(filePath.Split(new string[] { "-", "." }, StringSplitOptions.None)[4]).ToLower();
            Island island = new Island(keyName, rank++);
            dict.Add(island, new List<string[]>());
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    dict[island].Add(parser.ReadFields());
                }
            }
        }

        return dict;
    }
}