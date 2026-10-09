using System;
using System.Collections.Generic;

namespace DigitalMenu.Entities;

public partial class Restaurant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<ItemVariant> ItemVariants { get; set; } = new List<ItemVariant>();

    public virtual ICollection<MenuItemAllergen> MenuItemAllergens { get; set; } = new List<MenuItemAllergen>();

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
