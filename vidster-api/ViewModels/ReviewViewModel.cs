using System;
using System.Collections.Generic;

namespace vidster_api.Models;

public partial class ReviewViewModel
{
    public int Rating { get; set; }

    public int CreatorId { get; set; }
}
