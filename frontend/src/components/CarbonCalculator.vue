<template>
  <div class="min-h-screen flex items-center justify-center p-4"
       :class="backgroundClass"
       :style="{ backgroundColor: backgroundColor }">
    
    <div class="bg-white rounded-lg shadow-2xl p-8 w-full max-w-md backdrop-blur-sm bg-opacity-95">
      <h1 class="text-3xl font-bold text-center mb-8 text-gray-800">
        Carbon Footprint Sonification
      </h1>
      
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <div>
          <label for="airTravel" class="block text-sm font-medium text-gray-700 mb-2">
            Air Travel Distance (km)
          </label>
          <input
            id="airTravel"
            v-model.number="airTravelKm"
            type="number"
            min="0"
            step="0.1"
            placeholder="Enter distance in kilometers"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
            :class="{ 'border-red-500': errors.airTravel }"
          />
          <p v-if="errors.airTravel" class="text-red-500 text-sm mt-1">{{ errors.airTravel }}</p>
        </div>
        
        <div>
          <label for="redMeat" class="block text-sm font-medium text-gray-700 mb-2">
            Red Meat Consumption (kg)
          </label>
          <input
            id="redMeat"
            v-model.number="redMeatKg"
            type="number"
            min="0"
            step="0.1"
            placeholder="Enter weight in kilograms"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
            :class="{ 'border-red-500': errors.redMeat }"
          />
          <p v-if="errors.redMeat" class="text-red-500 text-sm mt-1">{{ errors.redMeat }}</p>
        </div>
        
        <button
          type="submit"
          :disabled="isLoading"
          class="w-full bg-blue-600 text-white py-3 px-6 rounded-lg font-medium hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ isLoading ? 'Calculating...' : 'Generate Sound' }}
        </button>
      </form>
      
      <!-- Results Section -->
      <div v-if="totalCO2 !== null" class="mt-8 p-4 bg-gray-50 rounded-lg">
        <h3 class="text-lg font-semibold text-gray-800 mb-2">Results</h3>
        <div class="space-y-2">
          <p class="text-gray-700">
            <span class="font-medium">Total CO2 Emissions:</span>
            <span class="ml-2 text-xl font-bold" :class="co2ColorClass">
              {{ totalCO2.toFixed(2) }} kg
            </span>
          </p>
          <p v-if="message" class="text-gray-600 text-sm">
            {{ message }}
          </p>
        </div>
        
        <!-- Audio Indicator -->
        <div v-if="isPlaying" class="mt-4 flex items-center justify-center">
          <div class="w-4 h-4 bg-blue-500 rounded-full animate-pulse-fast mr-2"></div>
          <span class="text-sm text-gray-600">Playing sound...</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, defineProps } from 'vue'

// Props
const props = defineProps<{
  isSidebarOpen: boolean
}>()

// Reactive state for form inputs
const airTravelKm = ref<number>(0)
const redMeatKg = ref<number>(0)

// Reactive state for results
const totalCO2 = ref<number | null>(null)
const message = ref<string>('')

// Reactive state for UI
const isLoading = ref<boolean>(false)
const isPlaying = ref<boolean>(false)
const errors = ref<{ airTravel?: string; redMeat?: string }>({})

// Reactive state for background color
const backgroundColor = ref<string>('#3b82f6') // Default blue

// Computed property for background class
const backgroundClass = computed(() => 'bg-transition')

// Computed property for CO2 color class
const co2ColorClass = computed(() => {
  if (totalCO2.value === null) return 'text-gray-600'
  return totalCO2.value > 100 ? 'text-red-600' : 'text-blue-600'
})

// Function to calculate background color based on CO2 emissions
const calculateBackgroundColor = (co2: number): string => {
  // Map CO2 emissions to color gradient
  // 0 kg CO2 = Blue (#3b82f6)
  // 100+ kg CO2 = Red (#dc2626)
  // Smooth gradient in between
  
  const maxCO2 = 200 // Assume 200kg as maximum for color calculation
  const ratio = Math.min(co2 / maxCO2, 1)
  
  // Interpolate between blue and red
  const blue = { r: 59, g: 130, b: 246 }  // #3b82f6
  const red = { r: 220, g: 38, b: 38 }    // #dc2626
  
  const r = Math.round(blue.r + (red.r - blue.r) * ratio)
  const g = Math.round(blue.g + (red.g - blue.g) * ratio)
  const b = Math.round(blue.b + (red.b - blue.b) * ratio)
  
  return `rgb(${r}, ${g}, ${b})`
}

// Function to calculate sound frequency based on CO2 emissions
const calculateFrequency = (co2: number): number => {
  // 0-100 kg CO2 → 220-440 Hz (calm, low frequency)
  // >100 kg CO2 → 1000-2000 Hz (stressful, high frequency)
  
  if (co2 <= 100) {
    // Linear interpolation: 0 kg = 220 Hz, 100 kg = 440 Hz
    return 220 + (co2 / 100) * (440 - 220)
  } else {
    // Linear interpolation: 100 kg = 1000 Hz, 200+ kg = 2000 Hz
    const ratio = Math.min((co2 - 100) / 100, 1)
    return 1000 + ratio * (2000 - 1000)
  }
}

// Function to generate sound using Web Audio API
const generateSound = (frequency: number): void => {
  // Create audio context
  const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)()
  
  // Create oscillator for sine wave
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()
  
  // Configure oscillator
  oscillator.type = 'sine'
  oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime)
  
  // Configure gain (volume) with fade in/out
  gainNode.gain.setValueAtTime(0, audioContext.currentTime)
  gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 0.1) // Fade in
  gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 2.9) // Hold volume
  gainNode.gain.linearRampToValueAtTime(0, audioContext.currentTime + 3) // Fade out
  
  // Connect nodes
  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)
  
  // Start and stop oscillator
  oscillator.start(audioContext.currentTime)
  oscillator.stop(audioContext.currentTime + 3)
  
  // Update UI state
  isPlaying.value = true
  setTimeout(() => {
    isPlaying.value = false
  }, 3000)
}

// Function to validate inputs
const validateInputs = (): boolean => {
  errors.value = {}
  let isValid = true
  
  if (airTravelKm.value < 0 || isNaN(airTravelKm.value)) {
    errors.value.airTravel = 'Please enter a valid non-negative number'
    isValid = false
  }
  
  if (redMeatKg.value < 0 || isNaN(redMeatKg.value)) {
    errors.value.redMeat = 'Please enter a valid non-negative number'
    isValid = false
  }
  
  return isValid
}

// Function to call backend API
const calculateCO2 = async (): Promise<void> => {
  try {
    const response = await fetch('/api/carboncalculator/calculate', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        airTravelKm: airTravelKm.value,
        redMeatKg: redMeatKg.value
      })
    })
    
    if (!response.ok) {
      throw new Error('Failed to calculate CO2 emissions')
    }
    
    const data = await response.json()
    totalCO2.value = data.totalCO2
    message.value = data.message
    
    // Update background color based on CO2 emissions
    backgroundColor.value = calculateBackgroundColor(totalCO2.value || 0)
    
    // Generate sound based on CO2 emissions
    const frequency = calculateFrequency(totalCO2.value || 0)
    generateSound(frequency)
    
  } catch (error) {
    console.error('Error calculating CO2:', error)
    message.value = 'Error calculating CO2 emissions. Please try again.'
  }
}

// Form submit handler
const handleSubmit = async (): Promise<void> => {
  if (!validateInputs()) {
    return
  }
  
  isLoading.value = true
  
  try {
    await calculateCO2()
  } finally {
    isLoading.value = false
  }
}
</script>