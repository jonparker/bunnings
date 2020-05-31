using System;
using System.Collections.Generic;

namespace Bunnings
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public List<string> ImageUrls { get; set; }
        public Brand Brand { get; set; }
    }
}
