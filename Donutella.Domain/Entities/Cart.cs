using Donutella.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donutella.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

