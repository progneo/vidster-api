using System;
using System.Collections.Generic;

namespace vidster_api.Models;

public partial class Work
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public int CreatorId { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual Creator Creator { get; set; } = null!;
}
