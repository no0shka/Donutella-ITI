using Donutella.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donutella.Domain.Entities
{
  
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

         public bool IsDeleted { get; set; } = false;
        public ICollection<Product> Products { get; set; } = new List<Product>();


        public ICollection<SubCategory> SubCategories { get; set; }= new List<SubCategory>();
    }
}
