using vidster_api.Models;

namespace vidster_api.ViewModels;

public partial class CreatorViewModel
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Address { get; set; } = null!;
    
    public double Rating { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
    
    public virtual ICollection<TagInCreator> TagsInCreator { get; set; } = new List<TagInCreator>();
}
