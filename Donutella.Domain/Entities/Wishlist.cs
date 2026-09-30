using Donutella.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donutella.Domain.Entities;

public class Wishlist : BaseEntity
{
    public int UserId { get; set; }

    public int ProductId { get; set; }

    public User User { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
