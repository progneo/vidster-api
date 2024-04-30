using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vidster_api.Models;

public partial class ServiceOfCreator
{
    public int Id { get; set; }

    public int CreatorId { get; set; }

    public int ServiceId { get; set; }

    public int Price { get; set; }

    public Creator? Creator { get; set; }

    public Service? Service { get; set; }
}