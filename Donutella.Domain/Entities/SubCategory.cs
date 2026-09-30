using Donutella.Domain.Common.Base;
using Donutella.Domain.Entities;

public class SubCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}
