using System.ComponentModel.DataAnnotations;

namespace Donutella.Web.Models.ViewModels.Review;

public class ReviewViewModel
{
    public int Id { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    [Required]
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    [Required]
    public int UserId { get; set; }
}