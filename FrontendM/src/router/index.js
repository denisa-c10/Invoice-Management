import { createRouter, createWebHistory } from 'vue-router'
import EditProduse from '../CRUD/edit_produse.vue'
import EditClient from '../CRUD/edit_client.vue'
import EditFacturi from '../CRUD/edit_facturi.vue'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue' // 1. ADAUGĂ IMPORTUL PENTRU LOGIN

const rute = [
  // Ruta de login este SINGURA publică (nu are requiresAuth)
  { path: '/login', name: 'login', component: LoginView },

  // Toate celelalte rute sunt protejate de stegulețul "requiresAuth"
  { path: '/', name: 'home', component: HomeView, meta: { requiresAuth: true } },
  { path: '/produse', name: 'produse', component: EditProduse, meta: { requiresAuth: true } },
  { path: '/clienti', name: 'clienti', component: EditClient, meta: { requiresAuth: true } },
  { path: '/facturi', name: 'facturi', component: EditFacturi, meta: { requiresAuth: true } },
  { path: '/edit-produse/:id', component: EditProduse, meta: { requiresAuth: true } },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: rute,
})

// ==========================================
// 3. PAZNICUL DE RUTE (Navigation Guard)
// ==========================================
// PAZNICUL DE RUTE MODERN
router.beforeEach((to, from) => {
  const necesitaAutentificare = to.matched.some(record => record.meta.requiresAuth)
  const token = localStorage.getItem('token_jwt')

  if (necesitaAutentificare && !token) {
    // Îl trimitem la login
    return '/login'
  } else if (to.path === '/login' && token) {
    // E deja logat, îl trimitem acasă
    return '/'
  }

  // Dacă nu intră pe niciun if, îl lasă automat să treacă mai departe
})

export default router