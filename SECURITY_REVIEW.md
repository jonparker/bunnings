# Security Review & Recommendations
**Date:** November 2025
**Reviewer:** Security Engineer
**Application:** Bunnings Product Catalog API
**Framework:** .NET 10 (LTS)

---

## Executive Summary

This security review evaluates the Bunnings API from an offensive and defensive security perspective. While the application demonstrates good foundational practices, several critical security enhancements are recommended before production deployment.

**Risk Level:** MEDIUM
**Critical Issues:** 2
**High Priority:** 4
**Medium Priority:** 3
**Low Priority:** 2

---

## Critical Issues

### 1. No Authentication/Authorization (CRITICAL)
**Current State:** API endpoints are publicly accessible without any authentication.

**Risk:**
- Unauthorized access to product data
- Potential for abuse and data scraping
- No audit trail of who accessed what
- Compliance violations (GDPR, SOC2, etc.)

**Recommendations:**
```csharp
// Option A: JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Option B: API Key Authentication
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

// Apply authorization globally
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

**Controller Updates:**
```csharp
[Authorize] // Apply to controller or specific endpoints
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    // Optionally use role-based access
    [HttpGet]
    [Authorize(Roles = "ProductReader,Admin")]
    public async Task<IActionResult> GetByQuery([FromQuery]ProductQuery query)
    {
        // ...
    }
}
```

---

### 2. Overly Permissive CORS Policy (CRITICAL)
**Location:** Program.cs:24-32

**Current State:**
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

**Risk:**
- Cross-Site Request Forgery (CSRF) attacks
- Unauthorized cross-origin data access
- Browser-based attacks from malicious sites
- Credential exposure in cross-origin contexts

**Recommendations:**
```csharp
// Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Development", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });
}
else
{
    // Production - strict whitelist
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(
                    "https://bunnings.com.au",
                    "https://www.bunnings.com.au",
                    "https://api.bunnings.com.au")
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Authorization", "Content-Type")
                  .AllowCredentials()
                  .SetPreflightMaxAge(TimeSpan.FromHours(1));
        });
    });
}
```

---

## High Priority Issues

### 3. Missing Rate Limiting (HIGH)
**Risk:** API abuse, DoS attacks, resource exhaustion, credential stuffing

**Recommendations:**
```bash
# Add NuGet package
dotnet add package AspNetCoreRateLimit
```

```csharp
// Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1h",
            Limit = 1000
        }
    };
});

builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// In middleware pipeline
app.UseIpRateLimiting();
```

---

### 4. Input Validation Vulnerabilities (HIGH)
**Location:** ProductService.cs:48-57, ProductController.cs:27

**Current Issues:**
- String search is case-insensitive but no length limits
- No sanitization of search input
- Potential for RegEx DoS if patterns are added
- SQL injection risk if database is added later

**Recommendations:**
```csharp
// ProductQuery.cs - Add validation attributes
using System.ComponentModel.DataAnnotations;

public class ProductQuery
{
    [Range(0, int.MaxValue, ErrorMessage = "BrandId must be non-negative")]
    public int? BrandId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ProductId must be non-negative")]
    public int? ProductId { get; set; }

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Search must be 1-100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Search contains invalid characters")]
    public string? Search { get; set; }

    [Range(0, 1000000, ErrorMessage = "MinPrice must be between 0 and 1,000,000")]
    public decimal? MinPrice { get; set; }

    [Range(0, 1000000, ErrorMessage = "MaxPrice must be between 0 and 1,000,000")]
    public decimal? MaxPrice { get; set; }
}

// ProductController.cs - Enable model validation
[HttpGet]
public async Task<IActionResult> GetByQuery([FromQuery] ProductQuery query)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Invalid query parameters: {ModelState}", ModelState);
        return BadRequest(ModelState);
    }
    // ...
}
```

---

### 5. Information Disclosure (HIGH)
**Location:** Program.cs:44-45

**Current Issues:**
- Generic error messages good, but error endpoint accessible without auth
- Swagger UI exposed in production (Program.cs:62-68)
- Developer exception page in development leaks stack traces

**Recommendations:**
```csharp
// Only enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bunnings API V1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Bunnings API Documentation";
    });
}

// Production error handling - don't expose implementation details
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        // Log the full exception server-side
        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionHandlerPathFeature?.Error,
            "Unhandled exception occurred");

        // Return generic message to client
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An internal server error occurred",
            requestId = Activity.Current?.Id ?? context.TraceIdentifier
        });
    });
});
```

---

### 6. Logging & Monitoring Deficiencies (HIGH)
**Current State:** Basic logging, no security event tracking

**Recommendations:**
```csharp
// Add structured logging with security events
public async Task<IActionResult> GetByQuery([FromQuery] ProductQuery query)
{
    using (_logger.BeginScope(new Dictionary<string, object>
    {
        ["UserId"] = User.Identity?.Name ?? "Anonymous",
        ["IPAddress"] = HttpContext.Connection.RemoteIpAddress?.ToString(),
        ["Endpoint"] = "ProductQuery"
    }))
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Security: Invalid input detected. Query: {@Query}, Errors: {@Errors}",
                query, ModelState);
            return BadRequest(ModelState);
        }

        var (isValid, validationMessage, results) =
            await _productService.GetProductsByQueryAsync(query);

        if (!isValid)
        {
            _logger.LogInformation(
                "Business validation failed: {ValidationMessage}",
                validationMessage);
            return BadRequest(validationMessage);
        }

        _logger.LogInformation(
            "Successful product query returned {Count} results",
            results?.Count() ?? 0);

        return Ok(results);
    }
}
```

**Integrate SIEM:**
- Forward logs to Azure Application Insights, Datadog, or Splunk
- Alert on: repeated validation failures, error spikes, unusual query patterns
- Monitor authentication failures when auth is implemented

---

## Medium Priority Issues

### 7. Missing Security Headers (MEDIUM)
**Recommendations:**
```bash
dotnet add package NetEscapades.AspNetCore.SecurityHeaders
```

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'");
    context.Response.Headers.Add("Permissions-Policy",
        "geolocation=(), microphone=(), camera=()");

    // Remove server header
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");

    await next();
});
```

---

### 8. No Request/Response Encryption Beyond TLS (MEDIUM)
**Recommendations:**
- Enforce HTTPS in production (already done with UseHttpsRedirection)
- Consider implementing field-level encryption for sensitive data
- Implement HSTS with long max-age:

```csharp
app.UseHsts(); // Already present
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});
```

---

### 9. Dependency Management & Supply Chain (MEDIUM)
**Current State:** Using latest packages (good), but no vulnerability scanning

**Recommendations:**
```bash
# Install .NET security scanner
dotnet tool install -g dotnet-outdated-tool
dotnet tool install -g dotnet-audit

# Scan for vulnerabilities
dotnet list package --vulnerable --include-transitive

# Automate in CI/CD
dotnet restore --locked-mode
```

**Add to CI/CD pipeline:**
```yaml
# GitHub Actions example
- name: Vulnerability Scan
  run: |
    dotnet list package --vulnerable --include-transitive
    if [ $? -ne 0 ]; then exit 1; fi
```

---

## Low Priority Issues

### 10. Missing API Versioning (LOW)
**Recommendations:**
```bash
dotnet add package Asp.Versioning.Mvc
```

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class ProductController : ControllerBase
```

---

### 11. No Health Check Details Protection (LOW)
**Current State:** /health endpoint accessible to everyone

**Recommendations:**
```csharp
// Require authentication for detailed health checks
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
}).RequireAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false, // Minimal info for load balancers
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"status\":\"Healthy\"}");
    }
}).AllowAnonymous();
```

---

## Compliance Considerations

### OWASP Top 10 2021 Coverage

| Risk | Status | Notes |
|------|--------|-------|
| A01:2021 Broken Access Control | ❌ FAIL | No authentication implemented |
| A02:2021 Cryptographic Failures | ⚠️ PARTIAL | HTTPS enforced, but no field encryption |
| A03:2021 Injection | ⚠️ PARTIAL | Limited input validation |
| A04:2021 Insecure Design | ⚠️ PARTIAL | No rate limiting or abuse prevention |
| A05:2021 Security Misconfiguration | ❌ FAIL | CORS too permissive, Swagger exposed |
| A06:2021 Vulnerable Components | ✅ PASS | Using latest .NET 10 packages |
| A07:2021 Auth Failures | ❌ FAIL | No authentication |
| A08:2021 Data Integrity Failures | ⚠️ PARTIAL | No integrity checks |
| A09:2021 Logging Failures | ⚠️ PARTIAL | Basic logging present |
| A10:2021 SSRF | ✅ PASS | No external requests |

**Current Score:** 2/10 Pass, 3/10 Fail, 5/10 Partial

---

## Implementation Priority

### Phase 1 (Immediate - Before Production)
1. ✅ Implement Authentication/Authorization
2. ✅ Fix CORS policy
3. ✅ Add rate limiting
4. ✅ Enhance input validation

### Phase 2 (Short Term - Within 2 Weeks)
5. ✅ Implement security headers
6. ✅ Restrict Swagger to development only
7. ✅ Add comprehensive security logging
8. ✅ Set up vulnerability scanning

### Phase 3 (Medium Term - Within 1 Month)
9. ✅ Implement API versioning
10. ✅ Set up SIEM integration
11. ✅ Add automated security testing to CI/CD

---

## Threat Model Summary

### Attack Vectors
1. **Unauthenticated API Abuse** - No auth allows unlimited scraping
2. **CSRF via CORS** - AllowAnyOrigin enables cross-site attacks
3. **DoS via Rate Exhaustion** - No rate limiting
4. **Injection Attacks** - Weak input validation
5. **Information Disclosure** - Swagger exposed in production

### Threat Actors
- **Script Kiddies:** Can easily abuse public API
- **Competitors:** Can scrape product catalog
- **Malicious Insiders:** No audit trail
- **APT Groups:** Could use as reconnaissance

### Mitigations Priority
Focus on authentication, CORS, and rate limiting to address 80% of risk.

---

## Security Testing Checklist

Before production deployment, verify:

- [ ] Authentication required on all endpoints
- [ ] CORS policy whitelisted to specific origins
- [ ] Rate limiting active and tested
- [ ] Input validation prevents injection
- [ ] Swagger UI disabled in production
- [ ] Security headers present in responses
- [ ] HTTPS enforced with valid certificate
- [ ] Logging captures security events
- [ ] Vulnerability scan passed
- [ ] Penetration test completed
- [ ] Security headers verified with securityheaders.com
- [ ] OWASP ZAP scan completed with no high/critical findings

---

## Conclusion

The Bunnings API has a solid technical foundation with .NET 10, but requires critical security enhancements before production deployment. The lack of authentication and overly permissive CORS policy represent immediate risks that must be addressed.

**Recommended Action:** Implement Phase 1 recommendations before any production deployment. The application should not be exposed to the internet without authentication and proper CORS configuration.

**Estimated Effort:**
- Phase 1: 3-5 days
- Phase 2: 5-7 days
- Phase 3: 2-3 weeks

**Contact:** For questions or implementation assistance, consult your security team or a security architect.
