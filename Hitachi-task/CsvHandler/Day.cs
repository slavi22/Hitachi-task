using Hitachi_task.Enums;

namespace Hitachi_task.CsvHandler;

public class Day
{
    public int DayOfTheMonth { get; set; }
    public int Temperature { get; set; }
    public int Wind { get; set; }
    public int Humidity { get; set; }
    public int Precipitation { get; set; }
    public bool Lightning { get; set; }
    public CloudsEnum Clouds { get; set; }
}