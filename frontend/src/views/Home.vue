<template>
  <div class="min-h-screen relative overflow-hidden">
    <!-- Dynamic Background -->
    <div class="absolute inset-0 bg-gradient-to-br from-blue-900 via-purple-900 to-indigo-900">
      <!-- Animated Background Elements -->
      <div class="absolute inset-0">
        <div class="absolute top-20 left-20 w-72 h-72 bg-blue-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-pulse"></div>
        <div class="absolute top-40 right-20 w-96 h-96 bg-purple-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-pulse animation-delay-2000"></div>
        <div class="absolute bottom-20 left-40 w-80 h-80 bg-indigo-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-pulse animation-delay-4000"></div>
      </div>
    </div>

    <!-- Sidebar Component -->
    <Sidebar 
      :is-open="isSidebarOpen" 
      @toggle="toggleSidebar" 
      @close="closeSidebar" 
    />

    <!-- Main Content Area -->
    <div class="relative z-10 min-h-screen flex items-center justify-center">
      <div class="text-center text-white px-6 max-w-4xl mx-auto">
        <!-- Welcome Message -->
        <transition
          appear
          enter-active-class="transition-all duration-1000 ease-out"
          enter-from-class="opacity-0 transform translate-y-8"
          enter-to-class="opacity-100 transform translate-y-0"
        >
          <div>
            <!-- Main Title -->
            <h1 class="text-5xl md:text-7xl font-bold mb-6 bg-gradient-to-r from-white to-blue-200 bg-clip-text text-transparent">
              Project Dashboard
            </h1>

            <!-- Subtitle -->
            <p class="text-xl md:text-2xl mb-8 text-blue-100 leading-relaxed">
              Explore interactive projects with real-time data visualization and audio feedback
            </p>

            <!-- Rotating Earth Animation -->
            <div class="mb-12 flex justify-center items-center">
              <div class="relative w-64 h-64">
                <!-- Earth -->
                <div class="w-full h-full rounded-full bg-gradient-to-br from-blue-400 via-green-400 to-blue-600 shadow-2xl animate-spin" style="animation-duration: 20s;">
                  <!-- Continents (simple shapes) -->
                  <div class="absolute top-8 left-12 w-16 h-12 bg-green-600 rounded-full opacity-80"></div>
                  <div class="absolute top-20 right-8 w-20 h-16 bg-green-600 rounded-lg opacity-80"></div>
                  <div class="absolute bottom-16 left-8 w-12 h-8 bg-green-600 rounded-full opacity-80"></div>
                  <div class="absolute bottom-12 right-16 w-14 h-10 bg-green-600 rounded-lg opacity-80"></div>
                </div>
                <!-- Glow effect -->
                <div class="absolute inset-0 rounded-full bg-gradient-to-tr from-blue-300 to-green-300 opacity-30 blur-xl animate-pulse"></div>
                <!-- Orbital ring -->
                <div class="absolute inset-0 border-2 border-white border-opacity-30 rounded-full animate-ping" style="animation-duration: 8s;"></div>
              </div>
            </div>

            <!-- Call to Action -->
            <div class="space-y-6">
              <p class="text-lg text-blue-100">
                Ready to explore? Use the sidebar to select a project and begin your interactive journey.
              </p>
            </div>

            <!-- Additional Info -->
            <div class="mt-12 text-sm text-blue-200 opacity-75">
              <p>Built with Vue.js 3 • TypeScript • Tailwind CSS</p>
              <p class="mt-2">Powered by .NET 7 Web API</p>
            </div>
          </div>
        </transition>
      </div>
    </div>

    <!-- Floating Elements -->
    <div class="absolute top-10 right-10 w-4 h-4 bg-white rounded-full opacity-30 animate-bounce"></div>
    <div class="absolute bottom-20 left-10 w-2 h-2 bg-blue-300 rounded-full opacity-40 animate-bounce animation-delay-1000"></div>
    <div class="absolute top-1/2 right-20 w-3 h-3 bg-purple-300 rounded-full opacity-30 animate-bounce animation-delay-2000"></div>
  </div>
</template>

<script setup lang="ts">
import Sidebar from '@/components/Sidebar.vue'

/**
 * Home Dashboard Component
 * Main landing page with welcome message and project overview
 * Features animated background with gradient effects
 * Responsive design with smooth transitions
 * Sidebar is always open on homepage
 */
</script>

<style scoped>
/* Custom animation delays */
.animation-delay-1000 {
  animation-delay: 1s;
}

.animation-delay-2000 {
  animation-delay: 2s;
}

.animation-delay-4000 {
  animation-delay: 4s;
}

/* Enhanced animations */
@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-20px); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
</style>