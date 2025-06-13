// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace WoodgroveGroceriesApi.Data;

/// <summary>
/// Sample product entity
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Entity Framework DbContext for Woodgrove Groceries
/// </summary>
public class WoodgroveGroceriesContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the WoodgroveGroceriesContext class
    /// </summary>
    /// <param name="options">The options for this context</param>
    public WoodgroveGroceriesContext(DbContextOptions<WoodgroveGroceriesContext> options) : base(options)
    {
    }

    /// <summary>
    /// Products in the database
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Configure the model
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed some sample data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Apples", Description = "Fresh red apples", Price = 2.99m, Category = "Fruits" },
            new Product { Id = 2, Name = "Bananas", Description = "Yellow ripe bananas", Price = 1.99m, Category = "Fruits" },
            new Product { Id = 3, Name = "Bread", Description = "Whole wheat bread", Price = 3.49m, Category = "Bakery" },
            new Product { Id = 4, Name = "Milk", Description = "2% milk", Price = 4.29m, Category = "Dairy" }
        );
    }

    /// <summary>
    /// Gets the EDM model for OData
    /// </summary>
    /// <returns>The EDM model</returns>
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<Product>("Products");
        return builder.GetEdmModel();
    }
}