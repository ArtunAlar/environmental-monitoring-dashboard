# Carbon Footprint Sonification - Backend

.NET 7 Web API backend for the Carbon Footprint Sonification web application. Provides REST API endpoints for calculating carbon emissions based on air travel distance and red meat consumption.

## Features

- 🔢 **Accurate CO2 Calculations**: Based on scientific emission factors
- 🔒 **Input Validation**: Comprehensive validation with detailed error messages
- 📚 **Swagger Documentation**: Interactive API documentation
- 🌐 **CORS Support**: Configured for frontend integration
- 📝 **Clean Architecture**: Separation of concerns with service layer
- 🎯 **RESTful Design**: Standard HTTP methods and status codes

## Technology Stack

- **ASP.NET Core 7**: Modern web framework
- **C# 11**: Latest language features with nullable reference types
- **Swagger/OpenAPI**: API documentation and testing
- **Dependency Injection**: Built-in IoC container
- **Model Validation**: Data annotation validation

## API Endpoints

### POST /api/carboncalculator/calculate
Calculates total CO2 emissions based on air travel distance and red meat consumption.

#### Request
**Content-Type**: `application/json`

```json
{
  "airTravelKm": 1000.0,
  "redMeatKg": 2.5
}
```

#### Response
**Success (200 OK)**
```json
{
  "totalCO2": 77.5,
  "message": "Your activities generated 77.50 kg of CO2 emissions. Air travel contributed 115.00 kg and red meat consumption contributed 67.50 kg. This is a significant carbon footprint. You might want to explore more sustainable alternatives."
}
```

**Validation Error (400 Bad Request)**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "AirTravelKm": ["Air travel distance must be non-negative"]
  }
}
```

### GET /api/carboncalculator/health
Health check endpoint to verify API status.

#### Response
```json
{
  "status": "healthy",
  "timestamp": "2023-11-13T12:00:00Z"
}
```

## CO2 Calculation Logic

### Emission Factors
- **Air Travel**: 0.115 kg CO2 per km
- **Red Meat**: 27.0 kg CO2 per kg

### Calculation Formula
```
Total CO2 = (Air Travel Distance × 0.115) + (Red Meat Consumption × 27.0)
```

### Message Generation
The API generates contextual messages based on total emissions:
- **< 10 kg**: Low footprint - encouraging message
- **10-50 kg**: Moderate footprint - improvement suggestions
- **50-100 kg**: Significant footprint - alternative recommendations
- **> 100 kg**: High footprint - substantial change suggestions

## Project Structure

```
├── Controllers/
│   └── CarbonCalculatorController.cs    # API endpoint implementation
├── Models/
│   ├── CarbonCalculationRequest.cs      # Request model with validation
│   └── CarbonCalculationResponse.cs     # Response model
├── Services/
│   └── CarbonCalculatorService.cs       # Business logic implementation
├── Program.cs                           # Application startup configuration
├── appsettings.json                     # Application configuration
└── CarbonFootprintAPI.csproj           # Project file with dependencies
```

## Key Components

### CarbonCalculatorController
REST API controller that handles HTTP requests:
- Input validation and model binding
- Error handling with appropriate HTTP status codes
- Logging for debugging and monitoring
- Response formatting

### CarbonCalculatorService
Business logic service that performs:
- CO2 emission calculations using scientific factors
- Message generation based on emission levels
- Rounding and formatting of results

### Models
- **CarbonCalculationRequest**: Request validation with data annotations
- **CarbonCalculationResponse**: Standardized response format

## Development Setup

### Prerequisites
- .NET 7 SDK
- Visual Studio 2022 or Visual Studio Code with C# extension
- (Optional) Postman or similar tool for API testing

### Installation
```bash
# Restore NuGet packages
dotnet restore
```

### Development Server
```bash
# Run the application
dotnet run
```

The API will be available at: http://localhost:5000

### Swagger UI
Interactive API documentation is available at: http://localhost:5000/swagger

### Build for Production
```bash
# Build the application
dotnet build --configuration Release

# Publish for deployment
dotnet publish --configuration Release --output ./publish
```

## Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Urls": "http://localhost:5000"
}
```

### CORS Configuration
The API is configured to accept requests from `http://localhost:3000` (frontend development server).

## Error Handling

### Validation Errors
- **400 Bad Request**: Invalid input data
- **422 Unprocessable Entity**: Model validation failures

### Server Errors
- **500 Internal Server Error**: Unexpected server errors
- Detailed error logging for debugging

### Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Error Title",
  "detail": "Detailed error message",
  "status": 400
}
```

## Testing

### Manual Testing
1. Use Swagger UI at http://localhost:5000/swagger
2. Test with various input combinations
3. Verify error responses for invalid inputs
4. Check health endpoint functionality

### API Testing with curl
```bash
# Test calculation endpoint
curl -X POST http://localhost:5000/api/carboncalculator/calculate \
  -H "Content-Type: application/json" \
  -d '{"airTravelKm": 1000, "redMeatKg": 2.5}'

# Test health endpoint
curl http://localhost:5000/api/carboncalculator/health
```

## Performance Considerations

### Response Time
- Simple calculations complete in < 10ms
- No external dependencies for calculations
- In-memory processing only

### Scalability
- Stateless design for horizontal scaling
- No session management required
- Minimal resource usage per request

## Security

### Input Validation
- Data annotation validation on models
- Range validation for negative numbers
- Type safety with nullable reference types

### CORS Protection
- Explicit origin configuration
- No wildcard origins in production

## Deployment

### Development
```bash
dotnet run
```

### Production
```bash
# Build for release
dotnet publish --configuration Release

# Run published application
dotnet ./publish/CarbonFootprintAPI.dll
```

### Docker Deployment
Create a Dockerfile:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CarbonFootprintAPI.csproj", "."]
RUN dotnet restore "CarbonFootprintAPI.csproj"
COPY . .
RUN dotnet build "CarbonFootprintAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarbonFootprintAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarbonFootprintAPI.dll"]
```

## Logging

### Log Levels
- **Information**: Successful calculations
- **Warning**: Model validation issues
- **Error**: Unexpected exceptions

### Sample Log Output
```
info: CarbonFootprintAPI.Controllers.CarbonCalculatorController[0]
      Calculating CO2 emissions for AirTravelKm: 1000, RedMeatKg: 2.5
info: CarbonFootprintAPI.Controllers.CarbonCalculatorController[0]
      CO2 calculation completed successfully. Total CO2: 77.5
```

## Monitoring

### Health Checks
The `/health` endpoint provides basic health status for monitoring systems.

### Metrics to Monitor
- Request/response times
- Error rates
- Validation failure rates
- Memory usage

## Contributing

1. Follow RESTful API design principles
2. Use proper HTTP status codes
3. Add comprehensive XML documentation
4. Include input validation
5. Handle errors gracefully
6. Add logging for debugging
7. Update Swagger documentation

## Troubleshooting

### Common Issues

1. **Port already in use**: Change port in appsettings.json or use different port
2. **CORS errors**: Verify CORS configuration matches frontend URL
3. **Model validation failures**: Check data annotation attributes
4. **Swagger not working**: Ensure development environment is configured

### Debug Mode
Enable detailed logging:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```