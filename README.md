# Bunnings Product Catalog API

A modern ASP.NET Core Web API for managing and querying product catalog information for a Bunnings hardware store. This API provides endpoints for searching and filtering products by various criteria including brand, price range, and text search.

## Features

- **Product Search**: Search products by name, description, or SKU
- **Brand Filtering**: Filter products by brand ID
- **Price Range Filtering**: Query products within specified price ranges
- **RESTful API**: Clean REST API design with proper HTTP status codes
- **API Documentation**: Built-in Swagger/OpenAPI documentation
- **Health Checks**: Health check endpoint for monitoring
- **CORS Support**: Configured for cross-origin requests
- **Input Validation**: Comprehensive query parameter validation
- **Unit Tests**: Test coverage for controllers and services

## Technology Stack

- **.NET 10.0**: Latest .NET platform
- **ASP.NET Core**: Web API framework
- **Swagger/OpenAPI**: API documentation (Swashbuckle.AspNetCore)
- **xUnit**: Testing framework
- **Moq**: Mocking library for unit tests

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- A code editor (Visual Studio, Visual Studio Code, or Rider)
- (Optional) Git for version control

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/jonparker/bunnings.git
   cd bunnings
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

## Running the Application

### Development Mode

Run the API with hot reload enabled:

```bash
cd Bunnings
dotnet run
```

The API will start and be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Swagger Documentation

Once the application is running, access the interactive API documentation at:
- `https://localhost:5001` or `http://localhost:5000`

The Swagger UI provides a complete overview of all available endpoints and allows you to test them directly from the browser.

## API Endpoints

### Products

#### GET /Product
Query products with various filters.

**Query Parameters:**
- `BrandId` (optional): Filter by brand ID (must be non-negative)
- `ProductId` (optional): Filter by specific product ID (must be non-negative)
- `Search` (optional): Search text for product name or description
- `MinPrice` (optional): Minimum price filter (must be non-negative)
- `MaxPrice` (optional): Maximum price filter (must be greater than MinPrice)

**Example Requests:**
```bash
# Search for products containing "basket"
GET /Product?Search=basket

# Filter by brand
GET /Product?BrandId=0&Search=wall

# Filter by price range
GET /Product?MinPrice=10&MaxPrice=50&Search=earthwool
```

**Response:**
- `200 OK`: Returns array of matching products
- `400 Bad Request`: Invalid query parameters

### Health Check

#### GET /health
Returns the health status of the API.

**Response:**
- `200 OK`: API is healthy

## Sample Data

The API includes sample products from two brands:

1. **Gardman** (BrandId: 0)
   - Wall basket ($12.98)
   - Coco roll basket liner ($17.98)

2. **EARTHWOOL** (BrandId: 1)
   - Earthwool R-4.0 ($71.50)
   - Earthwool Space Blanket R-1.8 ($80.00)

## Running Tests

Run all unit tests:

```bash
dotnet test
```

Run tests with detailed output:

```bash
dotnet test --verbosity detailed
```

Run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Project Structure

```
bunnings/
├── Bunnings/                          # Main API project
│   ├── Controllers/                   # API controllers
│   │   └── ProductController.cs       # Product endpoints
│   ├── Models/                        # Data models
│   │   ├── Brand.cs                   # Brand model
│   │   ├── Product.cs                 # Product model
│   │   └── ProductQuery.cs            # Query parameters model
│   ├── Services/                      # Business logic
│   │   └── ProductService.cs          # Product service with data
│   ├── Properties/                    # Application properties
│   ├── Program.cs                     # Application entry point
│   ├── appsettings.json              # Configuration
│   └── Bunnings.csproj               # Project file
├── Bunnings.Tests/                    # Test project
│   ├── ProductControllerTests.cs      # Controller tests
│   ├── ProductServiceTests.cs         # Service tests
│   └── Bunnings.Tests.csproj         # Test project file
├── Bunnings.sln                       # Solution file
└── README.md                          # This file
```

## Development

### Adding New Products

Products are currently stored in-memory in `ProductService.cs`. To add new products, modify the `_allProducts` collection in the `ProductService` class.

### Configuration

Application settings can be modified in:
- `appsettings.json` - General configuration
- `appsettings.Development.json` - Development-specific settings

### CORS Policy

The API is configured with an "AllowAll" CORS policy for development. For production use, update the CORS configuration in `Program.cs` to restrict allowed origins.

## Security Considerations

- Update CORS policy for production use
- Implement authentication and authorization as needed
- Review and configure HTTPS redirection settings
- Use user secrets for sensitive configuration data

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is available for use under standard software licensing terms. Please contact the repository owner for specific licensing information.

## Support

For issues, questions, or contributions, please open an issue on the GitHub repository.
