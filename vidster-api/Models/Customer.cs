using System;
using System.Collections.Generic;

namespace vidster_api.Models;

public partial class Customer
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
