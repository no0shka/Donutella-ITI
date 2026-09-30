using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Donutella.Web.Models.ViewModels.SubCategory;

public class SubCategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "SubCategory name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a category")]
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Categories { get; set; }
        = new List<SelectListItem>();
}