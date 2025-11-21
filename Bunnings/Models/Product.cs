using System;
using System.Collections.Generic;

namespace Bunnings
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string SKU { get; set; }
        public required List<string> ImageUrls { get; set; }
        public required Brand Brand { get; set; }
    }
}
