using System;
using System.Collections.Generic;
using System.Linq;
using Bunnings.Models;

namespace Bunnings.Services
{
    public class ProductService
    {
        private static Brand _gardMan = new Brand
            {Id = 0, LogoUrl = "https://media.bunnings.com/gardman.png", Name = "Gardman"};
        private static Brand _earthWool = new Brand
            { Id = 1, LogoUrl = "https://media.bunnings.com/earthwool.png", Name = "EARTHWOOl" };

        private static IEnumerable<Product> _allProducts = new Product[]
        {
            new Product
            {
                Brand = _gardMan, Name = "Wall basket", Id = 0,
                Description =
                    "This Gardman Wall Basket features an elegant, period-style design and is presented in a rustic \"limed\" finish. It is both strong and durable and comes complete with liner.",
                ImageUrls = new List<string> {"https://media.bunnings.com/image-guid.png"}, Price = 12.98m,
                SKU = "GAR01"
            },
            new Product
            {
                Brand = _gardMan, Name = "Coco roll basket liner", Id = 1,
                Description = "Line any size hanging basket, window box or planter Gardman Coco Roll Basket liner rolls",
                ImageUrls = new List<string> {"https://media.bunnings.com/image-guid.png"}, Price = 17.98m,
                SKU = "GAR02"
            },
            new Product
            {
                Brand = _earthWool, Name = "Earthwool R-4.0", Id = 2,
                Description = "Earthwool® R-4.0 Ceiling batt offers great performance, with excellent energy saving properties, enabling you to keep your home cool in summer and warm in winter.",
                ImageUrls = new List<string> {"https://media.bunnings.com/image-guid.png"}, Price = 71.50m,
                SKU = "EAR01"
            },
            new Product
            {
                Brand = _earthWool, Name = "Earthwool Space Blanket R-1.8", Id = 3,
                Description = "Space Blanket® is a specialist under-metal roof insulation designed for use in residential buildings.",
                ImageUrls = new List<string> {"https://media.bunnings.com/image-guid.png"}, Price = 80.0m,
                SKU = "EAR02"
            }
        };
    
        private static (bool result, string message) ValidateQuery(ProductQuery query) =>
            query.BrandId < 0 ? (false, "BrandId must not be negative") : (
                query.ProductId < 0 ? (false, "ProductId must not be negative") : (
                    query.MinPrice < 0 || query.MaxPrice < 0 ? (false, "Price range must be positive") : (
                        query.MaxPrice < query.MinPrice ? (false, "MaxPrice must be more than MinPrice") : (
                            string.IsNullOrWhiteSpace(query.Search) ? (false, "Search must not be empty") : (true, "")
                        )
                    )
                )
            );

        public Task<(bool result, string message, IEnumerable<Product>?)> GetProductsByQueryAsync(ProductQuery query)
        {
            var (result, message) = ValidateQuery(query);
            var queryResult = result ?
            (true, "", (IEnumerable<Product>?)(from p in _allProducts
                where (!query.BrandId.HasValue || p.Brand.Id == query.BrandId) &&
                      (!query.MinPrice.HasValue || !query.MaxPrice.HasValue ||
                       (p.Price < query.MaxPrice && p.Price > query.MinPrice)) &&
                      (!query.ProductId.HasValue || query.ProductId == p.Id) &&
                      (string.IsNullOrWhiteSpace(query.Search) ||
                       p.Name.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase) ||
                       p.Description.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase))
                   select p)) : (false, message, (IEnumerable<Product>?)null);

            return Task.FromResult(queryResult);
        }
    }
}
