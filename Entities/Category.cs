using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public Guid RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public int? DisplayOrder { get; set; }

    public bool? IsVisible { get; set; }

    public virtual ICollection<MenuItem> MenuItemCategories { get; set; } = new List<MenuItem>();

    public virtual ICollection<MenuItem> MenuItemCategoryNavigations { get; set; } = new List<MenuItem>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
