using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class ItemVariant
{
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public bool? IsAvailable { get; set; }

    public Guid RestaurantId { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual MenuItem MenuItemNavigation { get; set; } = null!;

    public virtual Restaurant Restaurant { get; set; } = null!;
}
