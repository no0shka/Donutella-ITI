using Donutella.Web.Models.ViewModels.Review;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Donutella.Web.Models.ViewModels.Product;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, 100000, ErrorMessage = "Stock must be 0 or greater")]
    public int Stock { get; set; }

    [Required]
    public int SubCategoryId { get; set; }

    public string SubCategoryName { get; set; } = string.Empty;

    public IFormFile? MainImageFile { get; set; }

    
    public List<IFormFile> ImageFiles { get; set; } = new();

 
    public string Image { get; set; } = string.Empty;

    public List<string> ProductImages { get; set; } = new();

    public decimal AverageRating { get; set; }

    public int ReviewCount { get; set; }

    public List<ReviewViewModel> Reviews { get; set; } = new();

    public IEnumerable<SelectListItem> SubCategories { get; set; }
        = new List<SelectListItem>();
}