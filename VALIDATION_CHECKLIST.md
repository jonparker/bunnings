# .NET 10 Upgrade Validation Checklist

## Prerequisites
- .NET 10 SDK installed
- Compatible IDE (Visual Studio 2025, Rider, or VS Code with C# extension)

## Phase 3: Build and Test Validation

### 1. Restore Dependencies
```bash
dotnet restore
```
**Expected**: All packages should restore successfully without errors

### 2. Build Solution
```bash
dotnet build
```
**Expected**: Clean build with 0 errors, 0 warnings

### 3. Run Unit Tests
```bash
dotnet test
```
**Expected**: All tests should pass
- ProductServiceTests.ValidateQuery (7 test cases)
- ProductControllerTests.GetByQuery_GivenAQuery_ReturnsAResult

### 4. Run Application
```bash
cd Bunnings
dotnet run
```
**Expected**: Application starts on https://localhost:5001

### 5. Verify Swagger UI
- Navigate to: https://localhost:5001/
- **Expected**: Swagger UI loads correctly
- **Expected**: "Bunnings API v1" title visible
- **Expected**: ProductController GET endpoint visible

### 6. Test API Endpoint
```bash
# Test with valid query
curl "https://localhost:5001/Product?Search=basket"

# Test with invalid query (negative BrandId)
curl "https://localhost:5001/Product?BrandId=-1&Search=test"
```
**Expected**:
- Valid query returns 200 OK with product results
- Invalid query returns 400 Bad Request with validation message

### 7. Verify Functionality
- [ ] Search by product name works
- [ ] Search by description works
- [ ] Filter by BrandId works
- [ ] Price range filtering works
- [ ] Validation errors return appropriate messages

## Known Changes from .NET Core 3.1
- Startup.cs pattern replaced with minimal hosting model
- UseEndpoints() replaced with MapControllers()
- AddEndpointsApiExplorer() added for OpenAPI support
- Swashbuckle.AspNetCore updated to v10.0.1

## Post-Validation
Once all checks pass, the upgrade is complete and ready for production deployment.
