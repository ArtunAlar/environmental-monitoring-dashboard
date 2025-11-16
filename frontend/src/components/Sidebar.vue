<template>
  <div class="relative">
    <!-- Sidebar Overlay -->
    <div
      v-if="isOpen"
      @click="closeSidebar"
      class="fixed inset-0 bg-black bg-opacity-30 z-40 lg:hidden transition-opacity duration-300"
      :class="{ 'opacity-100': isOpen, 'opacity-0': !isOpen }"
    ></div>

    <!-- Sidebar -->
    <div
      class="fixed left-0 top-0 h-full bg-white bg-opacity-95 backdrop-blur-lg shadow-2xl z-40 transition-all duration-300 ease-in-out"
      :class="{ 
        'w-64 translate-x-0': isOpen, 
        'w-64 -translate-x-full lg:translate-x-0': !isOpen 
      }"
    >
      <!-- Sidebar Header -->
      <div class="p-6 border-b border-gray-200">
        <h2 class="text-xl font-bold text-gray-800">Projects</h2>
        <p class="text-sm text-gray-600 mt-1">Select a project to explore</p>
      </div>

      <!-- Navigation Menu -->
      <nav class="p-4">
        <ul class="space-y-2">
          <li v-for="project in projects" :key="project.id">
            <router-link
              :to="project.path"
              @click="handleProjectClick"
              class="flex items-center p-3 rounded-lg transition-all duration-200 hover:bg-blue-50 hover:text-blue-700"
              :class="{ 
                'bg-blue-100 text-blue-700 border-l-4 border-blue-500': $route.path === project.path,
                'text-gray-700': $route.path !== project.path
              }"
            >
              <!-- Project Icon -->
              <div class="mr-3">
                <component 
                  :is="project.icon" 
                  class="w-5 h-5"
                  :class="{ 'text-blue-600': $route.path === project.path }"
                />
              </div>
              
              <!-- Project Info -->
              <div class="flex-1">
                <span class="font-medium">{{ project.name }}</span>
                <p class="text-xs text-gray-500 mt-1">{{ project.description }}</p>
              </div>

              <!-- Active Indicator -->
              <div 
                v-if="$route.path === project.path"
                class="w-2 h-2 bg-blue-500 rounded-full"
              ></div>
            </router-link>
          </li>
        </ul>
      </nav>

      <!-- Footer -->
      <div class="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200">
        <div class="text-xs text-gray-500 text-center">
          <p>Vue.js Dashboard</p>
          <p class="mt-1">Ready for more projects</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, h } from 'vue'
import { useRoute, useRouter } from 'vue-router'

// Props
defineProps<{
  isOpen: boolean
}>()

// Emits
const emit = defineEmits<{
  toggle: []
  close: []
}>()

// Router instance (router will be used when navigation is implemented)
// const route = useRoute()
// const router = useRouter()

// Sidebar state
const isOpen = ref(false)

// Project definitions
const projects = computed(() => [
  {
    id: 'carbon-footprint',
    name: 'Carbon Footprint',
    description: 'Real-time CO2 sonification',
    path: '/projects/carbon-footprint',
    icon: 'div' // Will be replaced with actual icon component
  },
  {
    id: 'water-visualizer',
    name: 'Water Visualizer',
    description: 'Alberta water monitoring',
    path: '/water-visualizer',
    icon: 'div' // Will be replaced with actual icon component
  },
  {
    id: 'bird-migration',
    name: 'Bird Migration',
    description: 'eBird migration patterns',
    path: '/projects/bird-migration',
    icon: 'div' // Will be replaced with actual icon component
  }
])

/**
 * Toggle sidebar open/close state
 */
const toggleSidebar = () => {
  emit('toggle')
}

/**
 * Close sidebar (used for overlay click and project selection)
 */
const closeSidebar = () => {
  emit('close')
}

/**
 * Handle project click - close sidebar on mobile after navigation
 */
const handleProjectClick = () => {
  // Close sidebar on mobile devices after selecting a project
  if (window.innerWidth < 1024) {
    closeSidebar()
  }
}

// Simple icon component - using emoji instead of SVG to avoid compilation issues
const CarbonIcon = {
  render() {
    return h('span', { class: 'text-xl' }, '♻️')
  }
}

const WaterIcon = {
  render() {
    return h('span', { class: 'text-xl' }, '💧')
  }
}

const BirdIcon = {
  render() {
    return h('span', { class: 'text-xl' }, '🦅')
  }
}

// Update projects with actual icon components
projects.value[0].icon = CarbonIcon
projects.value[1].icon = WaterIcon
projects.value[2].icon = BirdIcon
</script>

<style scoped>
/* Smooth transitions for all interactive elements */
.router-link-active {
  @apply bg-blue-100 text-blue-700 border-l-4 border-blue-500;
}

.router-link-active:hover {
  @apply bg-blue-100;
}

/* Custom scrollbar for sidebar */
::-webkit-scrollbar {
  width: 4px;
}

::-webkit-scrollbar-track {
  @apply bg-gray-100;
}

::-webkit-scrollbar-thumb {
  @apply bg-gray-300 rounded;
}

::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-400;
}
</style>