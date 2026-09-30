using Donutella.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donutella.Domain.Entities;
public class CartItem : BaseEntity{
        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; } = false;
        public Cart Cart { get; set; } = null!;

        public Product Product { get; set; } = null!;
}
