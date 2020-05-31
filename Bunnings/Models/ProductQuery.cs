using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bunnings.Models
{
    public class ProductQuery
    {
        public int? BrandId { get; set; }
        public int? ProductId { get; set; }
        public string Search { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }
}
