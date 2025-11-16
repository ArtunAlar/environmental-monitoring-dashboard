# Carbon Footprint Sonification Web Application

A real-time web application that calculates carbon footprint emissions and provides audio feedback through dynamic sound generation. Built with Vue.js 3 (Composition API) frontend and .NET 7 (ASP.NET Core Web API) backend.

## Features

- **Real-time CO2 Calculation**: Calculate emissions from air travel and red meat consumption
- **Dynamic Sound Generation**: Web Audio API generates sine wave sounds based on emission levels
- **Visual Feedback**: Background colors change from blue (low emissions) to red (high emissions)
- **Responsive Design**: Mobile-friendly interface with smooth animations
- **Input Validation**: Comprehensive validation for user inputs
- **REST API**: Clean backend architecture with proper error handling

## Architecture

### Frontend (Vue.js 3)
- **Framework**: Vue.js 3 with Composition API
- **Styling**: Tailwind CSS with custom animations
- **Build Tool**: Vite
- **Language**: TypeScript
- **Key Components**:
  - `CarbonCalculator.vue`: Main calculation and sound generation component
  - `App.vue`: Root application component

### Backend (.NET 7)
- **Framework**: ASP.NET Core Web API
- **Language**: C# with nullable reference types
- **Features**: Swagger documentation, CORS support, comprehensive validation
- **Key Components**:
  - `CarbonCalculatorController`: API endpoint for calculations
  - `CarbonCalculatorService`: Business logic for CO2 calculations
  - Models for request/response handling

## CO2 Calculation Logic

### Emission Factors
- **Air Travel**: 0.115 kg CO2 per km
- **Red Meat**: 27.0 kg CO2 per kg

### Sound Generation
- **Low Emissions (0-100 kg CO2)**: 220-440 Hz (calm, low frequency)
- **High Emissions (>100 kg CO2)**: 1000-2000 Hz (stressful, high frequency)
- **Duration**: 3 seconds with fade in/out

### Visual Feedback
- **Background Colors**: Smooth gradient from blue (low) to red (high)
- **Animations**: Smooth transitions and pulsing audio indicators

## Prerequisites

### Frontend
- Node.js (v16 or higher)
- npm or pnpm

### Backend
- .NET 7 SDK
- Visual Studio 2022 or Visual Studio Code with C# extension

## Setup Instructions

### 1. Clone the Repository
```bash
git clone <repository-url>
cd carbon-footprint-sonification
```

### 2. Frontend Setup
```bash
cd frontend
npm install
# or
pnpm install
```

### 3. Backend Setup
```bash
cd backend
dotnet restore
```

## Running the Application

### Frontend Development Server
```bash
cd frontend
npm run serve
# or
pnpm serve
```
The frontend will be available at: http://localhost:3000

### Backend API Server
```bash
cd backend
dotnet run
```
The backend API will be available at: http://localhost:5000

### Swagger Documentation
When running the backend in development mode, Swagger UI is available at: http://localhost:5000/swagger

## API Endpoints

### POST /api/carboncalculator/calculate
Calculates CO2 emissions based on input parameters.

**Request Body:**
```json
{
  "airTravelKm": 1000,
  "redMeatKg": 2.5
}
```

**Response:**
```json
{
  "totalCO2": 77.5,
  "message": "Your activities generated 77.50 kg of CO2 emissions..."
}
```

### GET /api/carboncalculator/health
Health check endpoint to verify API status.

## Project Structure

```
carbon-footprint-sonification/
├── frontend/                 # Vue.js frontend application
│   ├── src/
│   │   ├── components/      # Vue components
│   │   │   └── CarbonCalculator.vue
│   │   ├── App.vue
│   │   ├── main.ts
│   │   └── style.css
│   ├── package.json
│   ├── vite.config.ts
│   └── tailwind.config.js
├── backend/                  # .NET 7 Web API
│   ├── Controllers/
│   │   └── CarbonCalculatorController.cs
│   ├── Models/
│   │   ├── CarbonCalculationRequest.cs
│   │   └── CarbonCalculationResponse.cs
│   ├── Services/
│   │   └── CarbonCalculatorService.cs
│   ├── Program.cs
│   └── CarbonFootprintAPI.csproj
└── README.md
```

## Key Features Implementation

### Frontend Features
- ✅ Reactive form with validation
- ✅ Dynamic background color transitions
- ✅ Web Audio API sound generation
- ✅ Responsive design with Tailwind CSS
- ✅ Error handling and loading states
- ✅ Audio playback indicators

### Backend Features
- ✅ RESTful API design
- ✅ Comprehensive input validation
- ✅ Proper error handling and logging
- ✅ Swagger documentation
- ✅ CORS configuration for frontend integration
- ✅ Clean architecture with service layer

## Technical Details

### Frontend Technologies
- **Vue.js 3**: Modern reactive framework with Composition API
- **TypeScript**: Type-safe JavaScript development
- **Tailwind CSS**: Utility-first CSS framework
- **Vite**: Fast build tool and development server
- **Web Audio API**: Browser-native audio synthesis

### Backend Technologies
- **ASP.NET Core**: Modern web framework
- **C# 11**: Latest language features with nullable reference types
- **Swagger**: API documentation and testing
- **Dependency Injection**: Built-in IoC container
- **Model Validation**: Data annotation validation

## Error Handling

### Frontend
- Input validation with clear error messages
- API error handling with user feedback
- Audio context error handling for browser compatibility

### Backend
- Model validation with detailed error responses
- Exception handling with appropriate HTTP status codes
- Logging for debugging and monitoring

## Browser Compatibility

### Supported Browsers
- Chrome 66+
- Firefox 60+
- Safari 14.1+
- Edge 79+

### Web Audio API
The application uses Web Audio API for sound generation. Most modern browsers support this API, but some older versions may require polyfills.

## Development Notes

### Frontend Development
- Hot module replacement is enabled for fast development
- TypeScript strict mode is enabled for better code quality
- ESLint and Vue-specific linting rules are configured

### Backend Development
- Swagger UI provides interactive API testing
- Detailed logging is configured for development
- CORS is configured to allow frontend requests

## Production Deployment

### Frontend Build
```bash
cd frontend
npm run build
```

### Backend Deployment
The backend can be deployed to any .NET 7 compatible hosting environment:
- Azure App Service
- AWS Elastic Beanstalk
- Docker containers
- Self-hosted environments

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is open source and available under the MIT License.

## Support

For issues and questions:
1. Check the existing issues
2. Create a new issue with detailed description
3. Include steps to reproduce the problem
4. Provide environment details (OS, browser, .NET version, etc.)