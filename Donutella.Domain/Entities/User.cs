using Donutella.Domain.Common.Base;
using Donutella.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donutella.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashPassword { get; set; } = string.Empty;
    public RoleType Role { get; set; } = RoleType.Customer;

    public bool IsDeleted { get; set; } = false;

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }


    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Cart? Cart { get; set; }

    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
