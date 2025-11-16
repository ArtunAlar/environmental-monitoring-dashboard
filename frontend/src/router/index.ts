import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

// Import components
import Home from '@/views/Home.vue'
import CarbonCalculatorView from '@/views/CarbonCalculatorView.vue'
import WaterVisualizerView from '@/views/WaterVisualizerView.vue'
import BirdMigrationView from '@/views/BirdMigrationView.vue'

/**
 * Route definitions for the application
 * - Home: Main dashboard with sidebar navigation
 * - Carbon Footprint: The carbon footprint sonification project
 * - Water Visualizer: Alberta water resource monitoring project
 * - Bird Migration: Interactive bird migration visualization using eBird API
 */
const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: {
      title: 'Project Dashboard'
    }
  },
  {
    path: '/projects/carbon-footprint',
    name: 'CarbonFootprint',
    component: CarbonCalculatorView,
    meta: {
      title: 'Carbon Footprint Sonification'
    }
  },
  {
    path: '/water-visualizer',
    name: 'WaterVisualizer',
    component: WaterVisualizerView,
    meta: {
      title: 'Alberta Water Resource Visualizer'
    }
  },
  {
    path: '/projects/bird-migration',
    name: 'BirdMigration',
    component: BirdMigrationView,
    meta: {
      title: 'Bird Migration Route Animation'
    }
  },
  // Redirect any unknown routes to home
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

/**
 * Vue Router instance configuration
 * Uses HTML5 history mode for clean URLs
 */
const router = createRouter({
  history: createWebHistory(),
  routes,
  /**
   * Scroll behavior: Maintain scroll position or scroll to top
   */
  scrollBehavior(_to, _from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

/**
 * Global navigation guard to update document title
 */
router.beforeEach((to, _from, next) => {
  // Update document title based on route meta
  if (to.meta?.title) {
    document.title = `${to.meta.title} - Project Dashboard`
  } else {
    document.title = 'Project Dashboard'
  }
  next()
})

export default router