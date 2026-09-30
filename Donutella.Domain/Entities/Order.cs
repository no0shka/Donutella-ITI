using Donutella.Domain.Common.Base;
using Donutella.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donutella.Domain.Entities;
    public class Order : BaseEntity
    {
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalPrice { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

