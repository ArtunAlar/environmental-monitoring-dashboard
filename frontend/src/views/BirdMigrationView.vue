<template>
  <div class="flex min-h-screen bg-gray-100">
    <!-- Sidebar -->
    <Sidebar 
      :isOpen="sidebarOpen" 
      @toggle="toggleSidebar"
      @navigate="handleNavigation"
      currentProject="bird-migration"
    />
    
    <!-- Main Content -->
    <div class="flex-1 flex flex-col">
      <!-- Header with Home Button -->
      <header class="bg-white shadow-sm border-b border-gray-200">
        <div class="flex items-center justify-between px-6 py-4">
          <div class="flex items-center space-x-4">
            <button 
              @click="goHome"
              class="flex items-center space-x-2 px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
            >
              <span>🏠</span>
              <span>Home</span>
            </button>
            <h1 class="text-2xl font-bold text-gray-800">Bird Migration Visualizer</h1>
          </div>
          
          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-500">eBird API Integration</span>
            <button 
              @click="toggleSidebar"
              class="p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
            >
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>
      </header>
      
      <!-- Bird Migration Component -->
      <main class="flex-1 overflow-hidden">
        <BirdMigration />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Sidebar from '@/components/Sidebar.vue'
import BirdMigration from '@/components/BirdMigration.vue'

const router = useRouter()
const sidebarOpen = ref(true)

console.log('BirdMigrationView component loaded')

onMounted(() => {
  console.log('BirdMigrationView mounted')
})

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

const goHome = () => {
  router.push('/')
}

const handleNavigation = (route: string) => {
  if (route !== '/projects/bird-migration') {
    router.push(route)
  }
}
</script>