using System;
using System.Collections.Generic;

namespace vidster_api.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
