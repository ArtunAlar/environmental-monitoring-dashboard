<template>
  <div class="min-h-screen relative">
    <!-- Dashboard Navigation Button -->
    <router-link
      to="/"
      class="fixed top-4 right-4 z-50 p-2 bg-white bg-opacity-90 backdrop-blur-sm rounded-lg shadow-lg hover:bg-opacity-100 transition-all duration-300 flex items-center space-x-2"
      aria-label="Go to dashboard"
    >
      <svg class="w-5 h-5 text-gray-800" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
      </svg>
      <span class="text-sm font-medium text-gray-800 hidden sm:inline">Dashboard</span>
    </router-link>

    <!-- Sidebar Component -->
    <Sidebar 
      :is-open="true" 
      @toggle="toggleSidebar" 
      @close="closeSidebar" 
    />

    <!-- Water Visualizer Component -->
    <WaterVisualizer :is-sidebar-open="true" />
  </div>
</template>

<script setup lang="ts">
import Sidebar from '@/components/Sidebar.vue'
import WaterVisualizer from '@/components/WaterVisualizer.vue'

// Sidebar functions (kept for compatibility)
const toggleSidebar = () => {
  // No longer needed but kept for compatibility
}

const closeSidebar = () => {
  // No longer needed but kept for compatibility
}
</script>