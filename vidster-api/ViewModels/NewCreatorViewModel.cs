namespace vidster_api.ViewModels;

public class NewCreatorViewModel
{
    public string Username { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Address { get; set; } = null!;

    public List<int> Tags { get; set; } = [];
}
