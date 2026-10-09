using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class MenuItemAllergen
{
    public Guid MenuItemId { get; set; }

    public int AllergenId { get; set; }

    public Guid RestaurantId { get; set; }

    public virtual Allergen Allergen { get; set; } = null!;

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual MenuItem MenuItemNavigation { get; set; } = null!;

    public virtual Restaurant Restaurant { get; set; } = null!;
}
