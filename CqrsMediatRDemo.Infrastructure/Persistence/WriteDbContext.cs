using CqrsMediatRDemo.Domain.Entities;
using CqrsMediatRDemo.Domain.Events;
using CqrsMediatRDemo.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Infrastructure.Persistence;

public class WriteDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxMessageConfiguration).Assembly);

        // Ignore DomainEvent and its derived types – they are not persisted directly
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.Ignore<ProductPriceChangedEvent>();  // Add any other concrete event types here

        // Existing Product configuration (keep or add if missing)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);

            entity.OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PriceAmount").HasPrecision(18, 2);
                money.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(3);
            });

            // Shadow property for StockQuantity (since it's not exposed as public settable)
            entity.Property<int>("StockQuantity").IsRequired();
        });

        // OutboxMessage already configured via ApplyConfigurationsFromAssembly
    }
}
