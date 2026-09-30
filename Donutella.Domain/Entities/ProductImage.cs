using Donutella.Domain.Common.Base;

namespace Donutella.Domain.Entities;

public class ProductImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
}