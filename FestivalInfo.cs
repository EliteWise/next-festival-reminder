public class FestivalInfo
{
    public string Name { get; set; }
    public int Day { get; set; }
    public string Season { get; set; }

    public FestivalInfo(string name, int day, string season)
    {
        Name = name;
        Day = day;
        Season = season;
    }

    public override string ToString() => $"{Name}:\n{Day} {Season}";
}