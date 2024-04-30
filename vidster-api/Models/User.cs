using System;
using System.Collections.Generic;

namespace vidster_api.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Avatar { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordKey { get; set; } = null!;

    public virtual ICollection<Creator> Creators { get; set; } = new List<Creator>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
