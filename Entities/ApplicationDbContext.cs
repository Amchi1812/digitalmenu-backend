using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu.Entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Allergen> Allergens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ItemVariant> ItemVariants { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemAllergen> MenuItemAllergens { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Allergen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("allergens_pkey");

            entity.ToTable("allergens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IconName)
                .HasMaxLength(50)
                .HasColumnName("icon_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => new { e.RestaurantId, e.DisplayOrder }, "idx_categories_restaurant");

            entity.HasIndex(e => new { e.Id, e.RestaurantId }, "uq_categories_tenant").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.IsVisible)
                .HasDefaultValue(true)
                .HasColumnName("is_visible");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Categories)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("categories_restaurant_id_fkey");
        });

        modelBuilder.Entity<ItemVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_variants_pkey");

            entity.ToTable("item_variants");

            entity.HasIndex(e => e.MenuItemId, "idx_item_variants_menu_item");

            entity.HasIndex(e => e.RestaurantId, "idx_item_variants_restaurant");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.ItemVariantMenuItems)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("item_variants_menu_item_id_fkey");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.ItemVariants)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("item_variants_restaurant_id_fkey");

            entity.HasOne(d => d.MenuItemNavigation).WithMany(p => p.ItemVariantMenuItemNavigations)
                .HasPrincipalKey(p => new { p.Id, p.RestaurantId })
                .HasForeignKey(d => new { d.MenuItemId, d.RestaurantId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_item_variants_menu_item_tenant");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_items_pkey");

            entity.ToTable("menu_items");

            entity.HasIndex(e => new { e.CategoryId, e.DisplayOrder }, "idx_menu_items_category");

            entity.HasIndex(e => e.RestaurantId, "idx_menu_items_restaurant");

            entity.HasIndex(e => new { e.Id, e.RestaurantId }, "uq_menu_items_tenant").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("display_order");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItemCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("menu_items_category_id_fkey");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("menu_items_restaurant_id_fkey");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.MenuItemCategoryNavigations)
                .HasPrincipalKey(p => new { p.Id, p.RestaurantId })
                .HasForeignKey(d => new { d.CategoryId, d.RestaurantId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_menu_items_category_tenant");
        });

        modelBuilder.Entity<MenuItemAllergen>(entity =>
        {
            entity.HasKey(e => new { e.MenuItemId, e.AllergenId }).HasName("menu_item_allergens_pkey");

            entity.ToTable("menu_item_allergens");

            entity.HasIndex(e => e.AllergenId, "idx_menu_item_allergens_allergen");

            entity.HasIndex(e => e.RestaurantId, "idx_menu_item_allergens_restaurant");

            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.AllergenId).HasColumnName("allergen_id");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

            entity.HasOne(d => d.Allergen).WithMany(p => p.MenuItemAllergens)
                .HasForeignKey(d => d.AllergenId)
                .HasConstraintName("menu_item_allergens_allergen_id_fkey");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.MenuItemAllergenMenuItems)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("menu_item_allergens_menu_item_id_fkey");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.MenuItemAllergens)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("menu_item_allergens_restaurant_id_fkey");

            entity.HasOne(d => d.MenuItemNavigation).WithMany(p => p.MenuItemAllergenMenuItemNavigations)
                .HasPrincipalKey(p => new { p.Id, p.RestaurantId })
                .HasForeignKey(d => new { d.MenuItemId, d.RestaurantId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_menu_item_allergens_item_tenant");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("restaurants_pkey");

            entity.ToTable("restaurants");

            entity.HasIndex(e => e.Slug, "restaurants_slug_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'RestoranAdmin'::character varying")
                .HasColumnName("role");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Users)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("users_restaurant_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
