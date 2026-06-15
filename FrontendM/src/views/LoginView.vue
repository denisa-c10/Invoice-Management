<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <h2>Sistem Facturi</h2>
        <p>Autentificare securizată</p>
      </div>

      <div class="login-form">
        <div class="form-group">
          <label>Nume utilizator</label>
          <input type="text" v-model="username" placeholder="Introduceți username-ul..." @keyup.enter="efectueazaLogin" />
        </div>

        <div class="form-group">
          <label>Parolă</label>
          <input type="password" v-model="password" placeholder="Introduceți parola..." @keyup.enter="efectueazaLogin" />
        </div>

        <div v-if="mesajEroare" class="eroare">{{ mesajEroare }}</div>

        <button class="btn-login" @click="efectueazaLogin" :disabled="loading">
          {{ loading ? 'Se procesează...' : 'Intră în cont' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const username = ref('')
const password = ref('')
const mesajEroare = ref('')
const loading = ref(false)
const router = useRouter()

async function efectueazaLogin() {
  if (!username.value || !password.value) {
    mesajEroare.value = "Te rog completează ambele câmpuri!"
    return
  }

  loading.value = true
  mesajEroare.value = ''

  try {
    const response = await fetch('http://localhost:5116/Auth/Login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        Username: username.value,
        Password: password.value
      })
    })

    if (response.ok) {
      const data = await response.json()
      // 1. Salvăm token-ul în "seiful" browserului
      localStorage.setItem('token_jwt', data.token)
      // 2. Redirecționăm la pagina principală
      router.push('/')
    } else {
      mesajEroare.value = "Username sau parolă incorecte!"
    }
  } catch (error) {
    mesajEroare.value = "Eroare de conexiune cu serverul."
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page { height: 100vh; display: flex; justify-content: center; align-items: center; background-color: #f7fafc; }
.login-card { background: white; padding: 40px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); width: 100%; max-width: 400px; }
.login-header { text-align: center; margin-bottom: 30px; }
.login-header h2 { color: #2d3748; margin: 0 0 5px 0; }
.login-header p { color: #718096; margin: 0; }
.form-group { margin-bottom: 20px; }
.form-group label { display: block; margin-bottom: 8px; color: #4a5568; font-weight: 500; font-size: 0.9rem; }
.form-group input { width: 100%; padding: 12px; border: 1px solid #e2e8f0; border-radius: 8px; box-sizing: border-box; outline: none; transition: border-color 0.2s; }
.form-group input:focus { border-color: #4299e1; }
.btn-login { width: 100%; padding: 12px; background-color: #3e1f6c; color: white; border: none; border-radius: 8px; font-weight: bold; cursor: pointer; transition: 0.2s; margin-top: 10px; }
.btn-login:hover:not(:disabled) { background-color: #2b154c; }
.btn-login:disabled { background-color: #a0aec0; cursor: not-allowed; }
.eroare { color: #e53e3e; font-size: 0.85rem; margin-top: -10px; margin-bottom: 15px; text-align: center; }
</style>