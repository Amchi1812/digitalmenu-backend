using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class Allergen
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? IconName { get; set; }

    public virtual ICollection<MenuItemAllergen> MenuItemAllergens { get; set; } = new List<MenuItemAllergen>();
}
