using Donutella.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donutella.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}