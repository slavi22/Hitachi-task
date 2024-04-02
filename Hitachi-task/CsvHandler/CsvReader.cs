using Hitachi_task.Enums;
using Hitachi_task.Islands;

namespace Hitachi_task.CsvHandler;

public class CsvReader
{
    private readonly Dictionary<Island, List<string[]>> _dict;

    public CsvReader(Dictionary<Island, List<string[]>> dict)
    {
        _dict = dict;
    }

    public Dictionary<Island, Day> BestDays()
    {
        Dictionary<Island, Day> islandDayDict = new Dictionary<Island, Day>();
        foreach (var island in _dict.Keys)
        {
            islandDayDict.Add(island, BestDay(island.Name));
        }
        //order by the equator rank (asc)
        islandDayDict = islandDayDict.OrderBy(x => x.Key.RankByEquator).ToDictionary(x=>x.Key, x=>x.Value);
        /*foreach (var item in islandDayDict)
        {
            Console.WriteLine($"Island - {item.Key.Name} Rank - {item.Key.RankByEquator}");
            Console.WriteLine($"Day - DayOfTheMonth-{item.Value.DayOfTheMonth} Temp-{item.Value.Temperature} - Wind-{item.Value.Wind} - Humidity-{item.Value.Humidity} - Precipitation-{item.Value.Precipitation} - Lightning-{item.Value.Lightning} - Clouds-{item.Value.Clouds}");
        }*/
        return islandDayDict;
    }

    private Day BestDay(string islandName)
    {
        Island island = _dict.FirstOrDefault(x => x.Key.Name == islandName).Key;
        int[] temp = new int[16];
        int[] wind = new int[16];
        int[] humidity = new int[16];
        int[] precipitation = new int[16];
        bool[] lightning = new bool[16];
        CloudsEnum[] clouds = new CloudsEnum[16];
        for (int i = 1; i < _dict[island].Count; i++)
        {
            for (int j = 1; j < _dict[island][i].Length; j++)
            {
                if (i == 1) //day/parameter
                {
                    temp[j] = int.Parse(_dict[island][i][j]);
                }
                else if (i == 2) //wind
                {
                    wind[j] = int.Parse(_dict[island][i][j]);
                }
                else if (i == 3) //humidity
                {
                    humidity[j] = int.Parse(_dict[island][i][j]);
                }
                else if (i == 4) //precipitation
                {
                    precipitation[j] = int.Parse(_dict[island][i][j]);
                }
                else if (i == 5) //lightning
                {
                    lightning[j] = true ? _dict[island][i][j] == "Yes" : _dict[island][i][j] == "No";
                }
                else if (i == 6) //clouds
                {
                    clouds[j] = (CloudsEnum)Enum.Parse(typeof(CloudsEnum), _dict[island][i][j]);
                }
            }
        }

        List<Day> days = new List<Day>().ToList();
        for (int i = 0; i < 15; i++)
        {
            if ((temp[i] >= 1 && temp[i] <= 32) && wind[i] <= 11 && humidity[i] < 55 && precipitation[i] == 0 &&
                lightning[i] == false && (clouds[i] != CloudsEnum.Cumulus && clouds[i] != CloudsEnum.Nimbus))
            {
                Day day = new Day()
                {
                    DayOfTheMonth = i, Temperature = temp[i], Wind = wind[i], Humidity = humidity[i], Precipitation = precipitation[i],
                    Lightning = lightning[i], Clouds = clouds[i]
                };
                days.Add(day);
            }
        }

        days = days.OrderByDescending(x => x.Wind).ThenByDescending(x => x.Humidity).ToList();
        /*foreach (var day in days)
        {
            Console.WriteLine($"DayOfTheMonth-{bestDay.DayOfTheMonth} Temp-{day.Temperature} - Wind-{day.Wind} - Humidity-{day.Humidity} - Precipitation-{day.Precipitation} - Lightning-{day.Lightning} - Clouds-{day.Clouds}");
        }*/

        Day bestDay = days.LastOrDefault();
        /*Console.WriteLine("Best: ");
        Console.WriteLine($"DayOfTheMonth-{bestDay.DayOfTheMonth} Temp-{bestDay.Temperature} - Wind-{bestDay.Wind} - Humidity-{bestDay.Humidity} - Precipitation-{bestDay.Precipitation} - Lightning-{bestDay.Lightning} - Clouds-{bestDay.Clouds}");*/
        return bestDay;
    }
}