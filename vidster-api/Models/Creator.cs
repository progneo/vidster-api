using System;
using System.Collections.Generic;

namespace vidster_api.Models;

public partial class Creator
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
    
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    public virtual ICollection<TagInCreator> TagsInCreator { get; set; } = new List<TagInCreator>();
    
    public virtual ICollection<ServiceOfCreator> ServiceOfCreator { get; set; } = new List<ServiceOfCreator>();
}
