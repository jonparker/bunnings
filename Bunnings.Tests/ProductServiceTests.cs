using System;
using System.Collections;
using Bunnings.Models;
using Bunnings.Services;
using Xunit;

namespace Bunnings.Tests
{
    public class ProductServiceTests
    {
        [Theory]
        [InlineData(-1, 3, 10, 20, "", false, "BrandId must not be negative", false)]
        [InlineData(0, -1, 10, 20, "", false, "ProductId must not be negative",false)]
        [InlineData(0, 0, -1, 20, "", false, "Price range must be positive", false)]
        [InlineData(0, 0, 0, -1, "", false, "Price range must be positive", false)]
        [InlineData(0, 0, 0, 0, "", false, "Search must not be empty", false)]
        [InlineData(0, 0, 0, 0, "insulation", true, "", true)]
        public async Task ValidateQuery(int brandId, int productId, int minPrice, int maxPrice, string query,
            bool expectedResult, string expectedValidationMessage, bool anyResults)
        {
            // Arrange
            var productQuery = new ProductQuery
            {
                BrandId = brandId,
                ProductId = productId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Search = query
            };
            var productService = new ProductService();

            // Act
            var (actualResult, actualValidationMessage, results) = await productService.GetProductsByQueryAsync(productQuery);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedValidationMessage, actualValidationMessage);
            if (anyResults)
                Assert.NotNull(results);
            else 
                Assert.Null(results);
        }
    }
}
