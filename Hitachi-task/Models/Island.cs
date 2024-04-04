namespace Hitachi_task.Models;

public class Island
{
    public string Name { get; set; }
    public int RankByEquator { get; set; }

    public Island(string name, int rankByEquator)
    {
        Name = name;
        RankByEquator = rankByEquator;
    }
}