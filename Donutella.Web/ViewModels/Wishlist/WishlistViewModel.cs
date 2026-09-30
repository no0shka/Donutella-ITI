namespace Donutella.Web.Models.ViewModels.Wishlist;

public class WishlistViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal ProductPrice { get; set; }

    public string ProductImage { get; set; } = string.Empty;
}