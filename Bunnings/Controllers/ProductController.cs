using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bunnings.Models;
using Bunnings.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bunnings.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByQuery([FromQuery]ProductQuery query)
        {
            var (isValid, validationMessage, results) = await _productService.GetProductsByQueryAsync(query);
            if (!isValid)
            {
                _logger.LogInformation("(Get) validation error: {ValidationMessage}", validationMessage);
                return BadRequest(validationMessage);
            }

            return Ok(results);
        }
    }
}
