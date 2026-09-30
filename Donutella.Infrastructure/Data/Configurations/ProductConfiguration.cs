using Donutella.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donutella.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.Image)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(p => p.SubCategory)
            .WithMany(sc => sc.Products)
            .HasForeignKey(p => p.SubCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}