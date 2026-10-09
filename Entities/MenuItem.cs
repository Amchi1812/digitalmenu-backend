using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class MenuItem
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal BasePrice { get; set; }

    public bool? IsAvailable { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Category CategoryNavigation { get; set; } = null!;

    public virtual ICollection<ItemVariant> ItemVariantMenuItemNavigations { get; set; } = new List<ItemVariant>();

    public virtual ICollection<ItemVariant> ItemVariantMenuItems { get; set; } = new List<ItemVariant>();

    public virtual ICollection<MenuItemAllergen> MenuItemAllergenMenuItemNavigations { get; set; } = new List<MenuItemAllergen>();

    public virtual ICollection<MenuItemAllergen> MenuItemAllergenMenuItems { get; set; } = new List<MenuItemAllergen>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
