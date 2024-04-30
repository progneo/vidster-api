using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vidster_api.Models;

public partial class TagInCreator
{
    [Column("tag_id")] public int TagId { get; set; }
    public Tag? Tag { get; set; }

    [Column("creator_id")] public int CreatorId { get; set; }
    public Creator? Creator { get; set; }
}