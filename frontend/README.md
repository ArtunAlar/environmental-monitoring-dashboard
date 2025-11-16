# Carbon Footprint Sonification & Environmental Visualization - Frontend

Vue.js 3 frontend application for environmental monitoring and visualization. This multi-component application provides interactive interfaces for carbon footprint calculation, water monitoring, and bird migration tracking with dynamic audio and visual feedback.

## Features

### 🌍 Carbon Footprint Sonification
- 🎨 **Dynamic Visual Feedback**: Background colors change based on CO2 emissions (blue → red gradient)
- 🔊 **Real-time Audio Generation**: Web Audio API creates sine waves with frequencies mapped to emission levels
- 📱 **Responsive Design**: Mobile-first design with Tailwind CSS
- ⚡ **Reactive Forms**: Vue 3 Composition API with real-time validation
- 🎯 **Input Validation**: Comprehensive validation with clear error messages

### 🌊 Water Visualizer
- 📍 **Interactive Map**: Leaflet.js integration with real-time water station data
- 📊 **Dynamic Charts**: Chart.js visualization of water level data
- 🔄 **Real-time Updates**: Live data from Environment Canada API
- 📱 **Mobile Responsive**: Touch-friendly interface for mobile devices

### 🦅 Bird Migration Tracker
- 🗺️ **Interactive Migration Map**: Leaflet.js with animated migration routes
- 🎯 **Species Filtering**: Filter by bird species and observation dates
- ⏯️ **Timeline Animation**: Play/pause migration patterns over time
- 📍 **Observation Markers**: Color-coded markers based on bird count (yellow: 1-5, orange: 6-20, red: 21+)
- 🔍 **Species Information**: Detailed species data and observation details

## Technology Stack

- **Vue.js 3**: Modern reactive framework with Composition API
- **TypeScript**: Type-safe development
- **Tailwind CSS**: Utility-first CSS framework
- **Vite**: Fast build tool and development server
- **Web Audio API**: Browser-native audio synthesis
- **Leaflet.js**: Interactive mapping library
- **Chart.js**: Data visualization charts
- **Axios**: HTTP client for API communication

## Project Structure

```
src/
├── components/
│   ├── CarbonCalculator.vue    # Carbon footprint calculation and sound generation
│   ├── WaterVisualizer.vue     # Water monitoring with interactive map
│   ├── BirdMigration.vue       # Bird migration tracking and visualization
│   └── Sidebar.vue              # Navigation sidebar with collapsible menu
├── views/
│   ├── Home.vue                 # Dashboard with all applications
│   ├── CarbonCalculatorView.vue # Standalone carbon calculator page
│   ├── WaterVisualizerView.vue  # Standalone water visualizer page
│   └── BirdMigrationView.vue    # Standalone bird migration page
├── services/
│   ├── birdMigrationService.ts  # Bird migration API communication
│   └── waterDataService.ts       # Water data API communication
├── router/
│   └── index.ts                  # Vue Router configuration
├── App.vue                       # Root application component
├── main.ts                       # Application entry point
└── style.css                     # Global styles and Tailwind imports
```

## Key Components

### CarbonCalculator.vue
The main component that handles:
- User input for air travel distance and red meat consumption
- Form validation and error handling
- API communication with backend
- Dynamic background color calculation
- Web Audio API sound generation
- Loading states and user feedback

### WaterVisualizer.vue
Interactive water monitoring component:
- Real-time water station data from Environment Canada
- Interactive map with station markers
- Time series charts for water level data
- Station selection and detailed data views
- Responsive design for mobile devices

### BirdMigration.vue
Comprehensive bird tracking component:
- **eBird API Integration**: Note - Currently using fallback data (see eBird API section below)
- Interactive map with species observations
- Timeline-based animation controls
- Species filtering and search functionality
- Detailed species information panels
- Color-coded observation markers

### Sidebar.vue
Navigation component featuring:
- Collapsible menu structure
- Icons for each application (Leaf, Water, Bird)
- Responsive design that works on mobile
- Integration with Vue Router for navigation

## eBird API Integration - Important Note

### Current Limitation
The Bird Migration component **cannot access the real eBird API 2.0** because:
- eBird API requires **mandatory API key registration** through Cornell Lab of Ornithology
- API keys are only provided after application review and approval process
- Without authentication, all requests return 401 Unauthorized errors

### Fallback Implementation
To ensure the application works without API keys, we've implemented a **comprehensive fallback system**:

1. **Dynamic Random Data Generation**: Creates 15-30 realistic bird observations per request
2. **Alberta-Specific Species**: Includes 15 common Alberta bird species with accurate scientific names
3. **Realistic Locations**: Uses actual Alberta coordinates for observation points
4. **Memory Caching**: 15-minute cache to reduce load and improve performance
5. **Color-Coded Markers**: Based on observation count (yellow: 1-5, orange: 6-20, red: 21+)

### Fallback Data Features
- **Species Diversity**: Canada Warbler, American Robin, Red-winged Blackbird, etc.
- **Geographic Accuracy**: Observations within Alberta province boundaries
- **Temporal Realism**: Date-based filtering works with generated data
- **Count Realism**: Random observation counts (1-50 birds per observation)

### How It Currently Works
1. Frontend requests bird data from `/api/birdmigration/observations`
2. Backend checks for eBird API key (not configured)
3. Falls back to `GetFallbackObservations()` method
4. Generates realistic random data with Alberta species and locations
5. Returns formatted data matching eBird API structure
6. Frontend displays data on interactive map with animations

### Future eBird Integration
When API key becomes available:
1. Add `eBirdApiKey` to backend configuration
2. Remove fallback data generation
3. Direct API calls to `https://api.ebird.org/v2/data/obs/recent`
4. Maintain existing frontend interface (no changes needed)

### Sound Generation Logic (Carbon Calculator)

#### Frequency Mapping
- **Low Emissions (0-100 kg CO2)**: 220-440 Hz (calm, soothing)
- **High Emissions (>100 kg CO2)**: 1000-2000 Hz (alerting, stressful)

#### Audio Implementation
```typescript
// Creates a 3-second sine wave with fade in/out
const oscillator = audioContext.createOscillator()
const gainNode = audioContext.createGain()

oscillator.type = 'sine'
oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime)

// Smooth fade in/out
gainNode.gain.setValueAtTime(0, audioContext.currentTime)
gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 0.1)
gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 2.9)
gainNode.gain.linearRampToValueAtTime(0, audioContext.currentTime + 3)
```

### Visual Feedback (Carbon Calculator)

#### Background Color Calculation
- **Blue (#3b82f6)**: Low emissions (0 kg CO2)
- **Red (#dc2626)**: High emissions (200+ kg CO2)
- **Smooth Gradient**: Linear interpolation between colors

#### Animation Features
- Smooth background color transitions (0.5s duration)
- Pulsing audio indicator during playback
- Responsive design with mobile-first approach

## Development Setup

### Prerequisites
- Node.js (v16 or higher)
- npm or pnpm package manager

### Installation
```bash
# Install dependencies
npm install
# or
pnpm install
```

### Development Server
```bash
# Start development server with hot module replacement
npm run serve
# or
pnpm serve
```

The development server will start at: http://localhost:3000

### Build for Production
```bash
# Build optimized production bundle
npm run build
# or
pnpm build
```

### Code Quality
```bash
# Run ESLint
npm run lint

# Run TypeScript type checking
npm run type-check
```

## API Integration

### Backend API Endpoints

#### Carbon Calculator
- **POST** `/api/calculate` - Calculate carbon emissions
- **GET** `/api/calculate` - Health check

#### Water Visualizer
- **GET** `/api/water/stations` - Get water station list
- **GET** `/api/water/station/{id}` - Get specific station data

#### Bird Migration
- **GET** `/api/birdmigration/observations` - Get bird observations
- **GET** `/api/birdmigration/species` - Get species list

### Request/Response Examples

#### Carbon Calculator Request
```typescript
{
  airTravelKm: number,    // Distance in kilometers
  redMeatKg: number      // Weight in kilograms
}
```

#### Carbon Calculator Response
```typescript
{
  totalCO2: number,      // Total CO2 emissions in kg
  message: string        // Descriptive message
}
```

#### Bird Migration Response (Fallback Data)
```typescript
{
  observations: [
    {
      speciesCode: "canwar",
      commonName: "Canada Warbler",
      scientificName: "Cardellina canadensis",
      observationCount: 12,
      latitude: 53.5461,
      longitude: -113.4937,
      observationDate: "2024-11-16",
      location: "Edmonton, AB"
    }
  ],
  speciesCount: 11,
  totalObservations: 26
}
```

### Error Handling
- Network errors are caught and displayed to users
- Validation errors are shown inline with form fields
- API errors trigger user-friendly error messages
- Fallback data ensures app works even without external APIs

## Browser Compatibility

### Web Audio API Support
- Chrome 66+
- Firefox 60+
- Safari 14.1+
- Edge 79+

### Map Features (Leaflet.js)
- All modern browsers supported
- Mobile touch gestures supported
- Responsive design for all screen sizes

### Fallback Handling
The application gracefully handles:
- Browsers without Web Audio API support
- Missing eBird API keys (fallback data)
- Network connectivity issues
- API service unavailability

## Configuration

### Vite Configuration
The `vite.config.ts` file includes:
- Vue plugin configuration
- Path aliases (`@` → `src/`)
- Proxy configuration for API calls
- Development server settings

### Tailwind Configuration
Custom animations are defined in `tailwind.config.js`:
- `pulse-slow`: 2-second pulse animation
- `pulse-fast`: 1-second pulse animation

### Backend Integration
Frontend expects backend API at:
- Development: `http://localhost:5000/api`
- Production: Configured via environment variables

## Performance Optimizations

### Vue 3 Features
- **Composition API**: Better tree-shaking and TypeScript support
- **Reactive References**: Efficient state management
- **Computed Properties**: Optimized re-rendering

### Map Performance
- **Marker Clustering**: Groups nearby observations
- **Lazy Loading**: Loads data only when needed
- **Memory Caching**: 15-minute cache for bird data
- **Optimized Rendering**: Efficient Leaflet marker updates

### Build Optimizations
- Vite's fast HMR for development
- Tree-shaking for production builds
- CSS purging with Tailwind

## Accessibility

### Form Accessibility
- Proper label associations
- ARIA attributes where appropriate
- Keyboard navigation support
- Screen reader friendly error messages

### Visual Accessibility
- High contrast color combinations
- Clear visual hierarchy
- Adequate touch targets for mobile
- Color-blind friendly marker colors

### Map Accessibility
- Keyboard navigation for map controls
- Screen reader support for map data
- High contrast marker colors
- Clear visual indicators

## Testing

### Manual Testing Checklist
- [ ] Form validation works correctly
- [ ] API calls succeed with valid data
- [ ] Sound generation works in supported browsers
- [ ] Background colors change appropriately
- [ ] Responsive design works on mobile devices
- [ ] Error handling displays user-friendly messages
- [ ] Map interactions work correctly
- [ ] Timeline animations play/pause properly
- [ ] Species filtering functions correctly
- [ ] Fallback data loads when APIs unavailable

### Browser Testing
- [ ] Chrome (desktop & mobile)
- [ ] Firefox (desktop & mobile)
- [ ] Safari (desktop & mobile)
- [ ] Edge (desktop & mobile)

## Deployment

### Static Hosting
The built frontend can be deployed to any static hosting service:
- Vercel
- Netlify
- GitHub Pages
- AWS S3 + CloudFront

### Environment Variables
Configure the API URL in production:
```bash
VITE_API_URL=https://your-api-domain.com/api
```

### Backend Requirements
Ensure backend is running and accessible:
- .NET 7 Web API on port 5000
- CORS configured for frontend domain
- Fallback data services enabled

## Troubleshooting

### Common Issues

1. **Audio not playing**: Check browser autoplay policies and Web Audio API support
2. **API connection failed**: Verify backend is running and CORS is configured
3. **Map not loading**: Check Leaflet CSS/JS imports and API keys
4. **Build errors**: Check Node.js version and reinstall dependencies
5. **Bird data not loading**: Normal behavior - using fallback data system

### Debug Mode
Enable Vue DevTools for debugging:
```bash
npm run serve -- --mode development
```

### Backend Issues
- Ensure backend is running on port 5000
- Check CORS configuration in backend Program.cs
- Verify API endpoints are accessible
- Review fallback data generation logs

### eBird API Issues
**Expected Behavior**: Bird Migration uses fallback data because:
- No eBird API key is configured
- Fallback system provides realistic test data
- All frontend features work with generated data

**When API Key Available**:
1. Add key to backend configuration
2. Restart backend service
3. Frontend will automatically use real data

## Contributing

1. Follow Vue 3 Composition API patterns
2. Use TypeScript for type safety
3. Maintain responsive design principles
4. Test across different browsers and devices
5. Follow existing code style and conventions
6. Document any new API integrations
7. Include fallback mechanisms for external services

## Project Status

✅ **Fully Operational**: All three applications (Carbon Calculator, Water Visualizer, Bird Migration) are functional with comprehensive fallback systems.

🔄 **Current Limitations**:
- Bird Migration uses fallback data (no eBird API key)
- Water data depends on Environment Canada API availability

🎯 **Next Steps** (when resources available):
- Obtain eBird API key for real bird data
- Add more water monitoring stations
- Implement user accounts and data persistence
- Add more environmental monitoring features