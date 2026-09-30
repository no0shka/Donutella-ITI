using Donutella.Domain.Common.Base;

namespace Donutella.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string Image { get; set; } = string.Empty;

    public int SubCategoryId { get; set; }

    public SubCategory SubCategory { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;

    public ICollection<ProductImage> ProductImages { get; set; }
        = new List<ProductImage>();

    public ICollection<Review> Reviews { get; set; }
        = new List<Review>();

    public ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();

    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();

    public ICollection<Wishlist> Wishlists { get; set; }
        = new List<Wishlist>();
}