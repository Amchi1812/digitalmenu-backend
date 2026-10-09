using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public Guid? RestaurantId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Restaurant? Restaurant { get; set; } = null!;
}
