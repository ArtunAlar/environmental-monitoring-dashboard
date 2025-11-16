<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-cyan-100">
    <div class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-gray-800 mb-2">Alberta Water Resource Visualizer</h1>
        <p class="text-lg text-gray-600">Real-time monitoring of Alberta's water levels and environmental risk</p>
        <div class="mt-4 flex justify-center items-center space-x-6 text-sm bg-white rounded-lg p-4 shadow-sm">
          <div class="flex items-center space-x-2">
            <div class="w-4 h-4 bg-blue-500 rounded-full border-2 border-white shadow"></div>
            <span class="font-medium">High Water</span>
          </div>
          <div class="flex items-center space-x-2">
            <div class="w-4 h-4 bg-green-500 rounded-full border-2 border-white shadow"></div>
            <span class="font-medium">Normal</span>
          </div>
          <div class="flex items-center space-x-2">
            <div class="w-4 h-4 bg-orange-500 rounded-full border-2 border-white shadow"></div>
            <span class="font-medium">Low</span>
          </div>
          <div class="flex items-center space-x-2">
            <div class="w-4 h-4 bg-red-500 rounded-full border-2 border-white shadow"></div>
            <span class="font-medium">Critical Low</span>
          </div>
        </div>
      </div>

      <!-- Map Container -->
      <div class="bg-white rounded-xl shadow-xl p-6 mb-8 border border-gray-200">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-xl font-semibold text-gray-800">Water Station Locations</h2>
          <div v-if="lastUpdate" class="text-sm text-gray-500">
            Last updated: {{ formatDate(lastUpdate) }}
          </div>
        </div>
        <div id="water-map" class="h-[500px] w-full rounded-lg border-2 border-gray-300 shadow-inner bg-gray-50 relative">
          <!-- Loading overlay -->
          <div v-if="loading && stations.length === 0" class="map-loading">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mb-2"></div>
            <p class="text-gray-600 text-sm">Loading water station data...</p>
          </div>
          
          <!-- Error message -->
          <div v-if="error && stations.length === 0" class="map-loading">
            <div class="text-red-600 mb-2">
              <svg class="w-8 h-8 mx-auto" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
              </svg>
            </div>
            <p class="text-red-600 text-sm font-medium">{{ error }}</p>
            <button @click="loadStations" class="mt-2 px-4 py-2 bg-blue-500 text-white text-sm rounded hover:bg-blue-600 transition-colors">
              Retry
            </button>
          </div>
        </div>
      </div>

      <!-- Station Details Panel -->
      <div v-if="selectedStation" class="bg-white rounded-lg shadow-lg p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h2 class="text-2xl font-bold text-gray-800">{{ selectedStation.station.stationName }}</h2>
            <p class="text-gray-600">Station ID: {{ selectedStation.station.stationId }}</p>
            <p class="text-sm text-gray-500">Last Updated: {{ formatDate(selectedStation.station.lastUpdated) }}</p>
          </div>
          <button 
            @click="selectedStation = null"
            class="text-gray-400 hover:text-gray-600 text-2xl font-bold"
          >
            ×
          </button>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <!-- Water Level Card -->
          <div class="bg-gradient-to-r from-blue-50 to-blue-100 rounded-lg p-4">
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-semibold text-blue-800">Water Level</h3>
              <div 
                class="px-3 py-1 rounded-full text-sm font-medium"
                :class="getStatusColor(selectedStation.waterLevelStatus)"
              >
                {{ selectedStation.waterLevelStatus }}
              </div>
            </div>
            <div class="text-3xl font-bold text-blue-900">
              {{ selectedStation.currentWaterLevel.toFixed(2) }} m
            </div>
          </div>

          <!-- Discharge Card -->
          <div class="bg-gradient-to-r from-green-50 to-green-100 rounded-lg p-4">
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-semibold text-green-800">Discharge</h3>
              <div 
                class="px-3 py-1 rounded-full text-sm font-medium"
                :class="getStatusColor(selectedStation.dischargeStatus)"
              >
                {{ selectedStation.dischargeStatus }}
              </div>
            </div>
            <div class="text-3xl font-bold text-green-900">
              {{ selectedStation.currentDischarge.toFixed(1) }} m³/s
            </div>
          </div>
        </div>

        <!-- Recent Data Chart -->
        <div class="bg-gray-50 rounded-lg p-4">
          <h3 class="text-lg font-semibold text-gray-800 mb-4">Recent Data (Last Hour)</h3>
          <div class="h-64">
            <canvas id="station-chart"></canvas>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import { waterDataService, type WaterStation, type StationDetailResponse } from '@/services/waterDataService'
import L from 'leaflet'
import Chart from 'chart.js/auto'

// Import Leaflet CSS
import 'leaflet/dist/leaflet.css'

// Reactive data
const stations = ref<WaterStation[]>([])
const selectedStation = ref<StationDetailResponse | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)
const map = ref<L.Map | null>(null)
const markers = ref<L.Marker[]>([])
let chart: Chart | null = null
let refreshInterval: number | null = null
const lastUpdate = ref<string | null>(null)

// Initialize map
const initializeMap = () => {
  // Wait for DOM to be ready
  nextTick(() => {
    const mapElement = document.getElementById('water-map')
    if (!mapElement) {
      console.error('Map element not found')
      return
    }

    // Alberta center coordinates
    const albertaCenter: [number, number] = [54.0, -115.0]
    
    map.value = L.map('water-map', {
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
  })
}

// Get status color for markers
const getStatusColor = (status: string) => {
  switch (status) {
    case 'High': return 'bg-blue-500 text-blue-800'
    case 'Normal': return 'bg-green-500 text-green-800'
    case 'Low': return 'bg-orange-500 text-orange-800'
    case 'CriticalLow': return 'bg-red-500 text-red-800'
    default: return 'bg-gray-500 text-gray-800'
  }
}

// Get marker icon color
const getMarkerColor = (status: string) => {
  switch (status) {
    case 'High': return '#3B82F6' // Blue
    case 'Normal': return '#10B981' // Green
    case 'Low': return '#F59E0B' // Orange
    case 'CriticalLow': return '#EF4444' // Red
    default: return '#6B7280' // Gray
  }
}

// Create custom marker icon
const createMarkerIcon = (status: string) => {
  const color = getMarkerColor(status)
  return L.divIcon({
    className: 'custom-marker',
    html: `<div style="background-color: ${color}; width: 16px; height: 16px; border-radius: 50%; border: 3px solid white; box-shadow: 0 2px 8px rgba(0,0,0,0.4); cursor: pointer;"></div>`,
    iconSize: [16, 16],
    iconAnchor: [8, 8]
  })
}

// Fallback stations data
const getFallbackStations = (): WaterStation[] => {
  return [
    {
      stationId: '07EA004',
      stationName: 'Athabasca River at Athabasca',
      province: 'Alberta',
      latitude: 53.917,
      longitude: -118.885,
      status: 'Normal',
      lastUpdated: new Date().toISOString()
    },
    {
      stationId: '07BE001',
      stationName: 'Red Deer River at Red Deer',
      province: 'Alberta',
      latitude: 52.283,
      longitude: -113.785,
      status: 'Normal',
      lastUpdated: new Date().toISOString()
    },
    {
      stationId: '07DA001',
      stationName: 'Bow River at Calgary',
      province: 'Alberta',
      latitude: 51.045,
      longitude: -114.058,
      status: 'Low',
      lastUpdated: new Date().toISOString()
    },
    {
      stationId: '07ED001',
      stationName: 'North Saskatchewan River at Edmonton',
      province: 'Alberta',
      latitude: 53.200,
      longitude: -117.567,
      status: 'Normal',
      lastUpdated: new Date().toISOString()
    },
    {
      stationId: '07AE001',
      stationName: 'Oldman River at Lethbridge',
      province: 'Alberta',
      latitude: 49.685,
      longitude: -112.835,
      status: 'High',
      lastUpdated: new Date().toISOString()
    },
    {
      stationId: '07BB004',
      stationName: 'Battle River near Gadsby',
      province: 'Alberta',
      latitude: 52.825,
      longitude: -113.285,
      status: 'CriticalLow',
      lastUpdated: new Date().toISOString()
    }
  ]
}

// Load stations data
const loadStations = async () => {
  try {
    loading.value = true
    error.value = null
    
    const data = await waterDataService.getWaterStations()
    stations.value = data
    lastUpdate.value = new Date().toISOString()
    
    // Update map markers after data loads
    await nextTick()
    updateMarkers()
    
  } catch (err) {
    console.error('Error loading stations:', err)
    error.value = 'Failed to load water stations data. Using fallback data.'
    
    // Use fallback data
    stations.value = getFallbackStations()
    lastUpdate.value = new Date().toISOString()
    
    await nextTick()
    updateMarkers()
  } finally {
    loading.value = false
  }
}

// Update map markers
const updateMarkers = () => {
  if (!map.value) {
    console.warn('Map not initialized yet')
    return
  }
  
  try {
    // Only clear and re-add markers on first load or if stations count changed significantly
    const shouldRedraw = markers.value.length === 0 || Math.abs(markers.value.length - stations.value.length) > 2
    
    if (shouldRedraw) {
      // Clear existing markers only when necessary
      markers.value.forEach(marker => {
        if (map.value!.hasLayer(marker)) {
          map.value!.removeLayer(marker)
        }
      })
      markers.value = []
      
      if (stations.value.length === 0) {
        console.warn('No stations to display')
        return
      }
      
      // Add new markers
      stations.value.forEach(station => {
        try {
          const marker = L.marker([station.latitude, station.longitude], {
            icon: createMarkerIcon(station.status),
            title: station.stationName
          })
          
          marker.bindPopup(`
            <div class="p-3 min-w-[200px]">
              <h3 class="font-semibold text-gray-800 text-sm mb-1">${station.stationName}</h3>
              <p class="text-xs text-gray-600 mb-1">Status: <span class="font-medium">${station.status}</span></p>
              <p class="text-xs text-gray-500 mb-2">ID: ${station.stationId}</p>
              <button class="mt-2 px-3 py-1 bg-blue-500 text-white text-xs rounded hover:bg-blue-600 transition-colors" 
                      onclick="window.selectStation('${station.stationId}')">
                View Details
              </button>
            </div>
          `)
          
          marker.on('click', () => selectStation(station.stationId))
          marker.addTo(map.value!)
          markers.value.push(marker)
        } catch (err) {
          console.error('Error creating marker for station:', station.stationId, err)
        }
      })
      
      // Fit map to show all markers only on first load
      if (markers.value.length > 0 && shouldRedraw) {
        const group = new L.FeatureGroup(markers.value)
        map.value!.fitBounds(group.getBounds().pad(0.1))
      }
    } else {
      // Just update existing marker colors/status without moving them
      stations.value.forEach((station, index) => {
        if (markers.value[index]) {
          // Update the icon color if status changed
          const currentIcon = markers.value[index].options.icon
          const newIcon = createMarkerIcon(station.status)
          if (currentIcon !== newIcon) {
            markers.value[index].setIcon(newIcon)
          }
          // Update popup content
          markers.value[index].setPopupContent(`
            <div class="p-3 min-w-[200px]">
              <h3 class="font-semibold text-gray-800 text-sm mb-1">${station.stationName}</h3>
              <p class="text-xs text-gray-600 mb-1">Status: <span class="font-medium">${station.status}</span></p>
              <p class="text-xs text-gray-500 mb-2">ID: ${station.stationId}</p>
              <button class="mt-2 px-3 py-1 bg-blue-500 text-white text-xs rounded hover:bg-blue-600 transition-colors" 
                      onclick="window.selectStation('${station.stationId}')">
                View Details
              </button>
            </div>
          `)
        }
      })
    }
    
  } catch (err) {
    console.error('Error updating markers:', err)
  }
}

// Select station and show details
const selectStation = async (stationId: string) => {
  try {
    selectedStation.value = await waterDataService.getStationData(stationId)
    await nextTick()
    createChart()
  } catch (err) {
    console.error('Error loading station data:', err)
    error.value = 'Failed to load station details'
  }
}

// Create chart for station data
const createChart = () => {
  if (!selectedStation.value) return
  
  const ctx = document.getElementById('station-chart') as HTMLCanvasElement
  if (!ctx) return
  
  // Destroy existing chart
  if (chart) {
    chart.destroy()
  }
  
  // Prepare chart data
  const waterLevelData = selectedStation.value.recentData
    .filter(d => d.parameter === '46')
    .slice(-10)
    .reverse()
  
  const dischargeData = selectedStation.value.recentData
    .filter(d => d.parameter === '47')
    .slice(-10)
    .reverse()
  
  chart = new Chart(ctx, {
    type: 'line',
    data: {
      labels: waterLevelData.map(d => new Date(d.timestamp).toLocaleTimeString()),
      datasets: [
        {
          label: 'Water Level (m)',
          data: waterLevelData.map(d => d.value),
          borderColor: '#3B82F6',
          backgroundColor: 'rgba(59, 130, 246, 0.1)',
          tension: 0.4
        },
        {
          label: 'Discharge (m³/s)',
          data: dischargeData.map(d => d.value),
          borderColor: '#10B981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'top'
        },
        title: {
          display: true,
          text: 'Water Level and Discharge Trends'
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  })
}

// Format date helper
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString()
}

// Auto refresh data
const startAutoRefresh = () => {
  refreshInterval = window.setInterval(() => {
    loadStations()
  }, 30000) // Refresh every 30 seconds
}

// Stop auto refresh
const stopAutoRefresh = () => {
  if (refreshInterval) {
    window.clearInterval(refreshInterval)
    refreshInterval = null
  }
}

// Make selectStation available globally for popup buttons
window.selectStation = selectStation

// Lifecycle hooks
onMounted(async () => {
  try {
    console.log('WaterVisualizer mounted, initializing...')
    initializeMap()
    
    // Wait a bit for map to initialize before loading data
    setTimeout(async () => {
      await loadStations()
      startAutoRefresh()
    }, 500)
  } catch (err) {
    console.error('Error during component mount:', err)
    error.value = 'Failed to initialize the water visualizer'
    loading.value = false
  }
})

onUnmounted(() => {
  stopAutoRefresh()
  if (chart) {
    chart.destroy()
  }
  if (map.value) {
    map.value.remove()
  }
})
</script>

<style scoped>
.custom-marker {
  background: transparent;
  border: none;
}

/* Ensure map container has proper dimensions */
#water-map {
  min-height: 500px;
  height: 500px;
  width: 100%;
  background-color: #f8fafc;
  background-image: 
    linear-gradient(rgba(0,0,0,0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0,0,0,0.05) 1px, transparent 1px);
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
</style>