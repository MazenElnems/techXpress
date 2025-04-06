using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; }
    }
}