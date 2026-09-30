using System.ComponentModel.DataAnnotations;

namespace Donutella.Web.Models.ViewModels.Cart;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^0?1[0125]\d{8}$",
        ErrorMessage = "Enter a valid Egyptian mobile number (e.g. 01001234567)")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
}
