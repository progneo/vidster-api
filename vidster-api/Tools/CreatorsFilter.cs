namespace vidster_api.Tools;

public class CreatorsFilter
{
    public string? Name { get; set; }
    public List<string>? Tags { get; set; }
    public string SortBy { get; set; } = "name";
    public int CurrentPage { get; set; } = 1;
}