<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 to-blue-50">
    <div class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-gray-800 mb-2">Bird Migration Route Animation</h1>
        <p class="text-lg text-gray-600">Visualize migratory bird patterns across Alberta</p>
        
        <!-- Controls Panel -->
        <div class="mt-6 bg-white rounded-lg shadow-lg p-6 max-w-4xl mx-auto">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
            <!-- Species Selection -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Species</label>
              <select 
                v-model="selectedSpecies" 
                @change="loadObservations"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">All Species</option>
                <option v-for="species in speciesList" :key="species.speciesCode" :value="species.speciesCode">
                  {{ species.commonName }}
                </option>
              </select>
            </div>
            
            <!-- Date Range -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
              <input 
                v-model="startDate" 
                type="date" 
                @change="loadObservations"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">End Date</label>
              <input 
                v-model="endDate" 
                type="date" 
                @change="loadObservations"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
            </div>
            
            <!-- Region Selection -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Region</label>
              <select 
                v-model="selectedRegion" 
                @change="loadObservations"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="CA-AB">Alberta</option>
                <option value="CA-BC">British Columbia</option>
                <option value="CA-SK">Saskatchewan</option>
                <option value="CA-MB">Manitoba</option>
              </select>
            </div>
          </div>
          
          <!-- Animation Controls -->
          <div class="flex items-center justify-between border-t pt-4">
            <div class="flex items-center space-x-4">
              <button 
                @click="toggleAnimation" 
                :class="isAnimating ? 'bg-red-500 hover:bg-red-600' : 'bg-green-500 hover:bg-green-600'"
                class="px-4 py-2 text-white rounded-md transition-colors"
              >
                {{ isAnimating ? 'Stop' : 'Start' }} Animation
              </button>
              
              <button 
                @click="resetAnimation" 
                class="px-4 py-2 bg-gray-500 text-white rounded-md hover:bg-gray-600 transition-colors"
              >
                Reset
              </button>
            </div>
            
            <div class="flex items-center space-x-4">
              <label class="text-sm font-medium text-gray-700">Speed:</label>
              <input 
                v-model.number="animationSpeed" 
                type="range" 
                min="0.5" 
                max="3" 
                step="0.5"
                class="w-20"
              >
              <span class="text-sm text-gray-600">{{ animationSpeed }}x</span>
            </div>
          </div>
          
          <!-- Progress Bar -->
          <div v-if="isAnimating" class="mt-4">
            <div class="flex justify-between text-sm text-gray-600 mb-1">
              <span>{{ formatDate(animationStartDate) }}</span>
              <span>{{ Math.round(animationProgress * 100) }}%</span>
              <span>{{ formatDate(animationEndDate) }}</span>
            </div>
            <div class="w-full bg-gray-200 rounded-full h-2">
              <div 
                class="bg-blue-500 h-2 rounded-full transition-all duration-300" 
                :style="{ width: (animationProgress * 100) + '%' }"
              ></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Map Container -->
      <div class="bg-white rounded-xl shadow-xl p-6 mb-8 border border-gray-200">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-xl font-semibold text-gray-800">Migration Routes</h2>
          <div class="text-sm text-gray-500">
            <span v-if="currentObservations.length">{{ currentObservations.length }} observations</span>
            <span v-if="totalSpecies">• {{ totalSpecies }} species</span>
          </div>
        </div>
        
        <div id="bird-map" class="h-[600px] w-full rounded-lg border-2 border-gray-300 shadow-inner bg-gray-50 relative">
          <!-- Loading overlay -->
          <div v-if="loading" class="map-loading">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-green-600 mb-2"></div>
            <p class="text-gray-600 text-sm">Loading bird observations...</p>
          </div>
          
          <!-- Error message -->
          <div v-if="error" class="map-loading">
            <div class="text-red-600 mb-2">
              <svg class="w-8 h-8 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
              </svg>
            </div>
            <p class="text-red-600 text-sm font-medium">{{ error }}</p>
            <button @click="loadObservations" class="mt-2 px-4 py-2 bg-green-500 text-white text-sm rounded hover:bg-green-600 transition-colors">
              Retry
            </button>
          </div>
          
          <!-- Legend -->
          <div class="absolute top-4 right-4 bg-white bg-opacity-90 rounded-lg p-3 shadow-lg z-10">
            <h4 class="text-sm font-semibold text-gray-800 mb-2">Observation Count</h4>
            <div class="space-y-1 text-xs">
              <div class="flex items-center space-x-2">
                <div class="w-3 h-3 bg-yellow-400 rounded-full"></div>
                <span>1-5 birds</span>
              </div>
              <div class="flex items-center space-x-2">
                <div class="w-3 h-3 bg-orange-500 rounded-full"></div>
                <span>6-20 birds</span>
              </div>
              <div class="flex items-center space-x-2">
                <div class="w-3 h-3 bg-red-600 rounded-full"></div>
                <span>21+ birds</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Species Info Panel -->
      <div v-if="selectedSpeciesInfo" class="bg-white rounded-lg shadow-lg p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h2 class="text-2xl font-bold text-gray-800">{{ selectedSpeciesInfo.commonName }}</h2>
            <p class="text-gray-600 italic">{{ selectedSpeciesInfo.scientificName }}</p>
            <p class="text-sm text-gray-500">Family: {{ selectedSpeciesInfo.family }} • Order: {{ selectedSpeciesInfo.order }}</p>
          </div>
          <button 
            @click="selectedSpeciesInfo = null"
            class="text-gray-400 hover:text-gray-600 text-2xl font-bold"
          >
            ×
          </button>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="bg-green-50 rounded-lg p-4">
            <h3 class="text-lg font-semibold text-green-800 mb-2">Total Observations</h3>
            <div class="text-2xl font-bold text-green-900">{{ currentObservations.length }}</div>
          </div>
          
          <div class="bg-blue-50 rounded-lg p-4">
            <h3 class="text-lg font-semibold text-blue-800 mb-2">Date Range</h3>
            <div class="text-sm text-blue-900">
              {{ formatDate(startDate) }} to {{ formatDate(endDate) }}
            </div>
          </div>
          
          <div class="bg-purple-50 rounded-lg p-4">
            <h3 class="text-lg font-semibold text-purple-800 mb-2">Total Birds</h3>
            <div class="text-2xl font-bold text-purple-900">{{ totalBirdCount }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick, computed } from 'vue'
import { birdMigrationService, type BirdObservation, type BirdSpecies, type MigrationRouteData } from '@/services/birdMigrationService'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

// Reactive data
const observations = ref<BirdObservation[]>([])
const speciesList = ref<BirdSpecies[]>([])
const selectedSpecies = ref<string>('')
const selectedRegion = ref<string>('CA-AB')
const selectedSpeciesInfo = ref<BirdSpecies | null>(null)
const startDate = ref<string>('')
const endDate = ref<string>('')
const loading = ref(true)
const error = ref<string | null>(null)
const map = ref<L.Map | null>(null)
const markers = ref<L.Marker[]>([])
const animationMarkers = ref<L.Marker[]>([])

// Animation state
const isAnimating = ref(false)
const animationSpeed = ref<number>(1)
const animationProgress = ref<number>(0)
const animationStartDate = ref<string>('')
const animationEndDate = ref<string>('')
const currentAnimationFrame = ref<number>(0)
let animationInterval: number | null = null

// Computed properties
const currentObservations = computed(() => {
  if (!isAnimating.value) return observations.value
  
  // During animation, show observations up to current frame
  const progress = animationProgress.value
  const frameCount = Math.floor(observations.value.length * progress)
  return observations.value.slice(0, frameCount)
})

const totalSpecies = computed(() => speciesList.value.length)
const totalBirdCount = computed(() => 
  observations.value.reduce((sum, obs) => sum + obs.count, 0)
)

// Initialize map
const initializeMap = () => {
  nextTick(() => {
    const mapElement = document.getElementById('bird-map')
    if (!mapElement) {
      console.error('Map element not found')
      return
    }

    console.log('Initializing bird migration map...')
    
    // Alberta center coordinates
    const albertaCenter: [number, number] = [54.0, -115.0]
    
    map.value = L.map('bird-map', {
      center: albertaCenter,
      zoom: 5,
      zoomControl: true,
      attributionControl: true
    })
    
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 18
    }).addTo(map.value!)

    // Add scale control
    L.control.scale().addTo(map.value!)
    
    console.log('Map initialized successfully')
    
    // Add event listener for popup content using event delegation
    map.value.on('popupopen', (e) => {
      console.log('Popup opened, attaching event handlers...')
      setTimeout(() => {
        const popup = e.popup
        const content = popup.getContent()
        if (typeof content === 'string') {
          // Parse the HTML content to find the button
          const tempDiv = document.createElement('div')
          tempDiv.innerHTML = content
          const button = tempDiv.querySelector('.view-species-btn')
          if (button && button.getAttribute('data-species-code')) {
            const speciesCode = button.getAttribute('data-species-code')
            console.log('Found View Species Info button for:', speciesCode)
            // Find the actual button in the DOM
            const actualButton = document.querySelector('.view-species-btn[data-species-code="' + speciesCode + '"]')
            if (actualButton) {
              actualButton.addEventListener('click', (event) => {
                event.preventDefault()
                event.stopPropagation()
                console.log('View Species Info button clicked for species:', speciesCode)
                selectBirdSpecies(speciesCode)
              })
              console.log('Event handler attached successfully')
            } else {
              console.log('Button not found in DOM for species:', speciesCode)
            }
          } else {
            console.log('No View Species Info button found in popup')
          }
        }
      }, 100)
    })
  })
}

// Get marker color based on bird count
const getMarkerColor = (count: number): string => {
  if (count <= 5) return '#FCD34D' // Yellow
  if (count <= 20) return '#F97316' // Orange
  return '#DC2626' // Red
}

// Get marker size based on bird count
const getMarkerSize = (count: number): number => {
  if (count <= 5) return 8
  if (count <= 20) return 12
  return 16
}

// Create custom marker icon
const createBirdMarkerIcon = (count: number) => {
  const color = getMarkerColor(count)
  const size = getMarkerSize(count)
  
  return L.divIcon({
    className: 'bird-marker',
    html: `<div style="background-color: ${color}; width: ${size}px; height: ${size}px; border-radius: 50%; border: 2px solid white; box-shadow: 0 2px 8px rgba(0,0,0,0.4); cursor: pointer; transition: all 0.3s ease;"></div>`,
    iconSize: [size, size],
    iconAnchor: [size/2, size/2]
  })
}

// Update map markers - optimized to prevent flickering
const updateMarkers = () => {
  if (!map.value) {
    console.warn('Map not initialized yet')
    return
  }
  
  try {
    if (currentObservations.value.length === 0) {
      console.warn('No observations to display')
      // Clear all markers if no observations
      markers.value.forEach(marker => {
        if (map.value!.hasLayer(marker)) {
          map.value!.removeLayer(marker)
        }
      })
      markers.value = []
      return
    }
    
    // Group observations by location to avoid overlapping markers
    const locationGroups = currentObservations.value.reduce((groups, obs) => {
      const key = `${obs.latitude.toFixed(3)},${obs.longitude.toFixed(3)}`
      if (!groups[key]) {
        groups[key] = []
      }
      groups[key].push(obs)
      return groups
    }, {} as Record<string, BirdObservation[]>)
    
    // Create a map of existing markers for quick lookup
    const existingMarkers = new Map<string, L.Marker>()
    markers.value.forEach(marker => {
      const latLng = marker.getLatLng()
      const key = `${latLng.lat.toFixed(3)},${latLng.lng.toFixed(3)}`
      existingMarkers.set(key, marker)
    })
    
    // Track which markers should remain
    const markersToKeep = new Set<string>()
    
    // Add or update markers for each location group
    Object.entries(locationGroups).forEach(([location, locationObservations]) => {
      const [lat, lng] = location.split(',').map(Number)
      const totalCount = locationObservations.reduce((sum, obs) => sum + obs.count, 0)
      const primaryObservation = locationObservations[0]
      
      // Check if marker already exists at this location
      const existingMarker = existingMarkers.get(location)
      
      if (existingMarker) {
        // Update existing marker
        existingMarker.setIcon(createBirdMarkerIcon(totalCount))
        existingMarker.setLatLng([lat, lng])
        existingMarker.options.title = `${primaryObservation.commonName} - ${totalCount} birds`
        
        // Update popup content
        const popupContent = `
          <div class="p-3 min-w-[250px]">
            <h3 class="font-semibold text-gray-800 text-sm mb-2">${primaryObservation.locationName}</h3>
            <div class="space-y-1">
              ${locationObservations.map(obs => `
                <div class="flex justify-between items-center">
                  <span class="text-xs text-gray-700">${obs.commonName}</span>
                  <span class="text-xs font-medium text-gray-600">${obs.count} birds</span>
                </div>
              `).join('')}
            </div>
            <div class="text-xs text-gray-500 mt-2">
              Date: ${formatDate(primaryObservation.observationDate)}
            </div>
            <button class="mt-2 px-3 py-1 bg-green-500 text-white text-xs rounded hover:bg-green-600 transition-colors view-species-btn" 
                    data-species-code="${primaryObservation.speciesCode}">
              View Species Info
            </button>
          </div>
        `
        existingMarker.bindPopup(popupContent)
        
        // Re-attach click event
        existingMarker.off('click')
        existingMarker.on('click', () => selectBirdSpecies(primaryObservation.speciesCode))
        
        // Handle popup open event to attach button click handlers for existing markers
        existingMarker.off('popupopen')
        existingMarker.on('popupopen', () => {
          setTimeout(() => {
            const button = document.querySelector('.view-species-btn[data-species-code="' + primaryObservation.speciesCode + '"]')
            if (button) {
              button.addEventListener('click', (e) => {
                e.preventDefault()
                selectBirdSpecies(primaryObservation.speciesCode)
              })
            }
          }, 100)
        })
        
        markersToKeep.add(location)
      } else {
        // Create new marker
        const marker = L.marker([lat, lng], {
          icon: createBirdMarkerIcon(totalCount),
          title: `${primaryObservation.commonName} - ${totalCount} birds`
        })
        
        // Create popup content
        const popupContent = `
          <div class="p-3 min-w-[250px]">
            <h3 class="font-semibold text-gray-800 text-sm mb-2">${primaryObservation.locationName}</h3>
            <div class="space-y-1">
              ${locationObservations.map(obs => `
                <div class="flex justify-between items-center">
                  <span class="text-xs text-gray-700">${obs.commonName}</span>
                  <span class="text-xs font-medium text-gray-600">${obs.count} birds</span>
                </div>
              `).join('')}
            </div>
            <div class="text-xs text-gray-500 mt-2">
              Date: ${formatDate(primaryObservation.observationDate)}
            </div>
            <button class="mt-2 px-3 py-1 bg-green-500 text-white text-xs rounded hover:bg-green-600 transition-colors view-species-btn" 
                    data-species-code="${primaryObservation.speciesCode}">
              View Species Info
            </button>
          </div>
        `
        
        marker.bindPopup(popupContent)
        marker.on('click', () => selectBirdSpecies(primaryObservation.speciesCode))
        
        // Handle popup open event to attach button click handlers
        marker.on('popupopen', () => {
          setTimeout(() => {
            const button = document.querySelector('.view-species-btn[data-species-code="' + primaryObservation.speciesCode + '"]')
            if (button) {
              button.addEventListener('click', (e) => {
                e.preventDefault()
                selectBirdSpecies(primaryObservation.speciesCode)
              })
            }
          }, 100)
        })
        
        marker.addTo(map.value!)
        markers.value.push(marker)
        
        markersToKeep.add(location)
      }
    })
    
    // Remove markers that are no longer needed
    const markersToRemove: L.Marker[] = []
    existingMarkers.forEach((marker, location) => {
      if (!markersToKeep.has(location)) {
        if (map.value!.hasLayer(marker)) {
          map.value!.removeLayer(marker)
        }
        markersToRemove.push(marker)
      }
    })
    
    // Clean up removed markers from the array
    markers.value = markers.value.filter(marker => !markersToRemove.includes(marker))
    
  } catch (err) {
    console.error('Error updating markers:', err)
  }
}

// Load observations
const loadObservations = async () => {
  try {
    loading.value = true
    error.value = null
    
    console.log('Loading observations with params:', {
      region: selectedRegion.value,
      species: selectedSpecies.value || undefined,
      startDate: startDate.value || undefined,
      endDate: endDate.value || undefined
    })
    
    const response = await birdMigrationService.getObservations(
      selectedRegion.value,
      selectedSpecies.value || undefined,
      startDate.value || undefined,
      endDate.value || undefined,
      300
    )
    
    console.log('API Response received:', response)
    observations.value = response.observations
    speciesList.value = response.species
    
    console.log('Data loaded:', {
      observationsCount: observations.value.length,
      speciesCount: speciesList.value.length
    })
    
    // Set animation date range
    if (observations.value.length > 0) {
      animationStartDate.value = observations.value[0].observationDate
      animationEndDate.value = observations.value[observations.value.length - 1].observationDate
    }
    
    updateMarkers()
    
  } catch (err) {
    console.error('Error loading observations:', err)
    error.value = 'Failed to load bird observations. Using fallback data.'
    
    // The backend should return fallback data even on API failure
    // If we reach here, it means the backend itself failed, so we'll use empty data
    observations.value = []
    speciesList.value = []
    
  } finally {
    loading.value = false
  }
}

// Select bird species
const selectBirdSpecies = (speciesCode: string) => {
  console.log('selectBirdSpecies called with:', speciesCode)
  const species = speciesList.value.find(s => s.speciesCode === speciesCode)
  if (species) {
    console.log('Found species:', species)
    selectedSpeciesInfo.value = species
    selectedSpecies.value = speciesCode
    // Don't reload observations, just update the view
    // This prevents markers from disappearing
    console.log('Species info panel should now be visible for:', species.commonName)
  } else {
    console.log('Species not found in list:', speciesCode, 'Available species:', speciesList.value.map(s => s.speciesCode))
  }
}

// Animation controls
const toggleAnimation = () => {
  if (isAnimating.value) {
    stopAnimation()
  } else {
    startAnimation()
  }
}

const startAnimation = () => {
  if (observations.value.length === 0) return
  
  isAnimating.value = true
  animationProgress.value = 0
  currentAnimationFrame.value = 0
  
  const frameDelay = 1000 / animationSpeed.value // milliseconds per frame
  
  animationInterval = window.setInterval(() => {
    currentAnimationFrame.value++
    animationProgress.value = Math.min(1, currentAnimationFrame.value / (observations.value.length - 1))
    
    // Only update markers if we have new observations to show
    const previousFrameCount = Math.floor(observations.value.length * ((currentAnimationFrame.value - 1) / Math.max(1, observations.value.length - 1)))
    const currentFrameCount = Math.floor(observations.value.length * animationProgress.value)
    
    // Only update if we have new observations to display
    if (currentFrameCount !== previousFrameCount) {
      updateMarkers()
    }
    
    if (animationProgress.value >= 1) {
      stopAnimation()
    }
  }, frameDelay)
}

const stopAnimation = () => {
  isAnimating.value = false
  if (animationInterval) {
    window.clearInterval(animationInterval)
    animationInterval = null
  }
}

const resetAnimation = () => {
  stopAnimation()
  animationProgress.value = 0
  currentAnimationFrame.value = 0
  updateMarkers()
}

// Format date helper
const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString()
}

// Make selectBirdSpecies available globally for popup buttons
window.selectBirdSpecies = selectBirdSpecies

// Initialize dates
const initializeDates = () => {
  const today = new Date()
  const thirtyDaysAgo = new Date(today.getTime() - (30 * 24 * 60 * 60 * 1000))
  
  startDate.value = thirtyDaysAgo.toISOString().split('T')[0]
  endDate.value = today.toISOString().split('T')[0]
}

// Lifecycle hooks
onMounted(async () => {
  try {
    console.log('BirdMigration component mounted, initializing...')
    console.log('BirdMigration component data:', {
      selectedSpecies: selectedSpecies.value,
      selectedRegion: selectedRegion.value,
      startDate: startDate.value,
      endDate: endDate.value
    })
    initializeMap()
    initializeDates()
    
    // Wait a bit for map to initialize before loading data
    setTimeout(async () => {
      console.log('Loading observations...')
      await loadObservations()
      console.log('Observations loaded:', observations.value.length)
    }, 500)
  } catch (err) {
    console.error('Error during component mount:', err)
    error.value = 'Failed to initialize the bird migration visualizer'
    loading.value = false
  }
})

onUnmounted(() => {
  stopAnimation()
  if (map.value) {
    map.value.remove()
  }
})
</script>

<style scoped>
.bird-marker {
  background: transparent;
  border: none;
}

/* Ensure map container has proper dimensions */
#bird-map {
  min-height: 600px;
  height: 600px;
  width: 100%;
  background-color: #f0f9ff;
  background-image: 
    linear-gradient(rgba(0,0,0,0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0,0,0,0.03) 1px, transparent 1px);
  background-size: 20px 20px;
  position: relative;
  overflow: hidden;
}

/* Ensure map is visible and properly sized */
:deep(.leaflet-container) {
  height: 100% !important;
  width: 100% !important;
  position: absolute !important;
  top: 0;
  left: 0;
  z-index: 1;
  background: transparent;
}

/* Improve popup styling */
:deep(.leaflet-popup-content-wrapper) {
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

:deep(.leaflet-popup-tip) {
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

/* Loading state for map */
.map-loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 10;
  background: rgba(255, 255, 255, 0.9);
  padding: 20px;
  border-radius: 8px;
  text-align: center;
}

/* Animation for markers */
:deep(.bird-marker div) {
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.1);
    opacity: 0.8;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}
</style>