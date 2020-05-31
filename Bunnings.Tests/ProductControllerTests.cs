using Bunnings.Controllers;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Bunnings.Models;
using System.Threading.Tasks;
using Bunnings.Services;

namespace Bunnings.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task GetByQuery_GivenAQuery_ReturnsAResult()
        {
            // Arrange
            const int negativeBrandId = -1;
            var logMock = new Mock<ILogger<ProductController>>();
            var productService = new ProductService();
            var productController = new ProductController(logMock.Object, productService);
            var query = new ProductQuery
            {
                BrandId = negativeBrandId
            };

            // Act
            var result = await productController.GetByQuery(query);

            // Assert
            Assert.NotNull(result);
        }
    }
}
