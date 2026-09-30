using Donutella.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Donutella.Domain.Entities;
namespace Donutella.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    
    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
