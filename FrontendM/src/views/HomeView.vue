<template>
  <div class="dashboard-container">
    <div class="welcome-banner">
      <div class="banner-text">
        <h1>Sistem Gestionare Facturi</h1>
        <p>Aici este panoul tău de control.</p>
      </div>
      <div class="banner-actions" style="display: flex; align-items: center; gap: 15px;">
        <div class="banner-date">{{ dataCurenta }}</div>
        <button class="btn-logout" @click="delogare">🚪 Deconectare</button>
      </div>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon icon-blue"></div>
        <div class="stat-info">
          <span class="stat-label">Produse (Tipuri)</span>
          <h2 class="stat-value">
              <span v-if="loading">...</span>
              <span v-else>{{ totalProduse }}</span>
          </h2>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon icon-green"></div>
        <div class="stat-info">
          <span class="stat-label">Clienți activi</span>
          <h2 class="stat-value">
              <span v-if="loading">...</span>
              <span v-else>{{ totalClienti }}</span>
          </h2>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon icon-purple"></div>
        <div class="stat-info">
          <span class="stat-label">Facturi emise</span>
          <h2 class="stat-value">
              <span v-if="loading">...</span>
              <span v-else>{{ totalFacturi }}</span>
          </h2>
        </div>
      </div>
    </div>

    <div class="quick-actions">
      <h3>Acțiuni Rapide</h3>
      <div class="action-buttons">
        <router-link to="/produse" class="btn-action">Adaugă Produs</router-link>
        <router-link to="/clienti" class="btn-action">Adaugă Client</router-link>
        <router-link to="/facturi" class="btn-action">Emite Factură</router-link>
      </div>
    </div>

    <div class="websocket-wrapper">
        
        <div class="websocket-section signalr-box">
            <h3>Test WebSockets (SignalR)</h3>
            <div class="websocket-actions">
                <button class="btn-ws btn-ws-primary" @click="apeleazaComanda1SignalR">Trimite Alertă (SignalR)</button>
                <button class="btn-ws btn-ws-secondary" @click="apeleazaComanda2SignalR">Cere Status</button>
            </div>

            <div class="websocket-data">
                <div class="ws-status">
                    <strong>Status Server:</strong> 
                    <span :class="statusServerSignalR ? 'text-success' : 'text-muted'">
                        {{ statusServerSignalR || 'Așteptare status...' }}
                    </span>
                </div>
                <div class="ws-messages">
                    <strong>Alerte Primite:</strong>
                    <ul>
                        <li v-for="(msg, index) in mesajePrimiteSignalR" :key="index">🔔 {{ msg }}</li>
                        <li v-if="mesajePrimiteSignalR.length === 0" class="text-muted">Nicio alertă primită.</li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="websocket-section nativ-box">
            <h3>Test WebSockets (Nativ)</h3>
            <div class="websocket-actions">
                <button class="btn-ws btn-ws-warning" @click="apeleazaComanda1Nativ">Trimite Alertă (Nativ)</button>
                <button class="btn-ws btn-ws-secondary" @click="apeleazaComanda2Nativ">Cere Status</button>
            </div>

            <div class="websocket-data">
                <div class="ws-status">
                    <strong>Status Server:</strong> 
                    <span :class="statusServerNativ ? 'text-warning-dark' : 'text-muted'">
                        {{ statusServerNativ || 'Așteptare status...' }}
                    </span>
                </div>
                <div class="ws-messages">
                    <strong>Alerte Primite:</strong>
                    <ul>
                        <li v-for="(msg, index) in mesajePrimiteNativ" :key="index">⚡ {{ msg }}</li>
                        <li v-if="mesajePrimiteNativ.length === 0" class="text-muted">Nicio alertă primită.</li>
                    </ul>
                </div>
            </div>
        </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue' 
import { useRouter } from 'vue-router' // NOU: Importăm router-ul pentru redirecționare
import * as signalR from '@microsoft/signalr' 

// Inițializăm router-ul
const router = useRouter()

// Statistici Dashboard
const totalProduse = ref(0)
const totalClienti = ref(0)
const totalFacturi = ref(0)
const dataCurenta = ref('')
const loading = ref(true)
const apiUrl = 'http://localhost:5116' 

// Variabile SIGNALR
const mesajePrimiteSignalR = ref([])
const statusServerSignalR = ref('')
let conexiuneSignalR = null 

// Variabile NATIV
const mesajePrimiteNativ = ref([])
const statusServerNativ = ref('')
let wsNativ = null 

onMounted(async () => {
    // 1. Setup Dashboard Data
    const optiuniData = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }
    dataCurenta.value = new Date().toLocaleDateString('ro-RO', optiuniData)

    try {
        const resProduse = await fetch(`${apiUrl}/Produs/GetProdusModels`)
        if (resProduse.ok) totalProduse.value = (await resProduse.json()).length

        const resClienti = await fetch(`${apiUrl}/Client/GetClienti`)
        if (resClienti.ok) totalClienti.value = (await resClienti.json()).length

        const resFacturi = await fetch(`${apiUrl}/Factura/GetFacturi`)
        if (resFacturi.ok) totalFacturi.value = (await resFacturi.json()).length
    } catch (error) {
        console.error("Eroare API:", error)
    } finally {
        loading.value = false 
    }

    // ----------------------------------------------------
    // 2. INIȚIALIZARE SIGNALR
    // ----------------------------------------------------
    conexiuneSignalR = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5116/hub-notificari") 
        .withAutomaticReconnect() 
        .build();

    conexiuneSignalR.on("PrimesteAlerta", (utilizator, mesaj) => {
        mesajePrimiteSignalR.value.push(`${utilizator} spune: ${mesaj}`);
    });

    conexiuneSignalR.on("PrimesteStatus", (status) => {
        statusServerSignalR.value = status;
    });

    try {
        await conexiuneSignalR.start();
        console.log("Conectat la SignalR!");
    } catch (eroare) {
        console.error("Eroare SignalR: ", eroare);
    }

    // ----------------------------------------------------
    // 3. INIȚIALIZARE WEBSOCKETS NATIV
    // ----------------------------------------------------
    wsNativ = new WebSocket("ws://localhost:5116/ws-nativ");

    wsNativ.onopen = () => console.log("Conectat la Nativ!");
    
    wsNativ.onmessage = (event) => {
        const mesaj = event.data;
        if (mesaj.startsWith("ALERTA|")) {
            const parti = mesaj.split("|");
            mesajePrimiteNativ.value.push(`${parti[1]} spune: ${parti[2]}`);
        } else if (mesaj.startsWith("STATUS_SERVER|")) {
            statusServerNativ.value = mesaj.split("|")[1];
        }
    };

    wsNativ.onerror = (err) => console.error("Eroare Nativ: ", err);
    wsNativ.onclose = () => console.log("Conexiune Nativ închisă.");
})

onUnmounted(() => {
    if (conexiuneSignalR) conexiuneSignalR.stop();
    if (wsNativ) wsNativ.close();
})

// ==========================================
// FUNCȚIA DE DELOGARE (NOU)
// ==========================================
function delogare() {
    localStorage.removeItem('token_jwt')
    router.push('/login')
}

// ==========================================
// FUNCȚII PENTRU SIGNALR
// ==========================================
function apeleazaComanda1SignalR() {
    if (conexiuneSignalR && conexiuneSignalR.state === signalR.HubConnectionState.Connected) {
        conexiuneSignalR.invoke("TrimiteAlertaGlobala", "Admin (SignalR)", "Acesta este un test prin SignalR!");
    }
}
function apeleazaComanda2SignalR() {
    if (conexiuneSignalR && conexiuneSignalR.state === signalR.HubConnectionState.Connected) {
        conexiuneSignalR.invoke("CereStatusSistem");
    }
}

// ==========================================
// FUNCȚII PENTRU NATIV
// ==========================================
function apeleazaComanda1Nativ() {
    if (wsNativ && wsNativ.readyState === WebSocket.OPEN) {
        wsNativ.send("ALERTA|Admin (Nativ)|Acesta este un test prin WebSockets Nativ!");
    }
}
function apeleazaComanda2Nativ() {
    if (wsNativ && wsNativ.readyState === WebSocket.OPEN) {
        wsNativ.send("STATUS");
    }
}
</script>

<style scoped>
.dashboard-container { padding: 30px; max-width: 1200px; margin: 0 auto; }
.welcome-banner { background-color: #3e1f6c; color: white; border-radius: 12px; padding: 30px 40px; display: flex; justify-content: space-between; align-items: center; margin-bottom: 40px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
.banner-text h1 { margin: 0 0 10px 0; font-size: 1.8rem; }
.banner-text p { margin: 0; color: #a0aec0; font-size: 1rem; }
.banner-date { background-color: rgba(255, 255, 255, 0.1); padding: 8px 16px; border-radius: 20px; font-size: 0.9rem; font-weight: 500; }

/* NOU: Stil pentru butonul de Logout */
.btn-logout { background-color: rgba(255, 255, 255, 0.2); color: white; border: 1px solid rgba(255, 255, 255, 0.4); padding: 8px 16px; border-radius: 20px; font-weight: bold; cursor: pointer; transition: all 0.2s; }
.btn-logout:hover { background-color: #e53e3e; border-color: #e53e3e; }

.stats-grid { display: flex; gap: 25px; margin-bottom: 40px; }
.stat-card { background: white; border-radius: 12px; padding: 25px; flex: 1; display: flex; align-items: center; gap: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.03); border: 1px solid #edf2f7; transition: transform 0.2s; }
.stat-card:hover { transform: translateY(-3px); box-shadow: 0 6px 15px rgba(0,0,0,0.06); }
.stat-icon { width: 60px; height: 60px; border-radius: 12px; }
.icon-blue { background-color: #ebf8ff; }
.icon-green { background-color: #f0fff4; }
.icon-purple { background-color: #faf5ff; }
.stat-info { display: flex; flex-direction: column; }
.stat-label { color: #718096; font-size: 0.95rem; font-weight: 500; margin-bottom: 5px; }
.stat-value { margin: 0; font-size: 2.2rem; color: #2d3748; font-weight: 700; }
.quick-actions h3 { color: #2d3748; margin-bottom: 20px; }
.action-buttons { display: flex; gap: 15px; }
.btn-action { background: white; border: 1px solid #e2e8f0; padding: 15px 25px; border-radius: 8px; color: #4a5568; font-weight: 600; text-decoration: none; transition: 0.2s; box-shadow: 0 1px 3px rgba(0,0,0,0.02); }
.btn-action:hover { border-color: #cbd5e0; background: #f7fafc; color: #2b6cb0; }

/* AMBELE WEBSOCKETS */
.websocket-wrapper { display: flex; gap: 25px; margin-top: 40px; }
.websocket-section { flex: 1; padding: 25px; background-color: #fff; border-radius: 12px; box-shadow: 0 2px 10px rgba(0,0,0,0.03); }
.websocket-section h3 { margin-bottom: 20px; font-size: 1.2rem; }

.signalr-box { border: 2px solid #63b3ed; }
.nativ-box { border: 2px solid #f6ad55; }

.websocket-actions { display: flex; gap: 10px; margin-bottom: 20px; }
.btn-ws { padding: 10px 15px; border: none; border-radius: 6px; font-weight: bold; cursor: pointer; transition: 0.2s; }
.btn-ws-primary { background-color: #4299e1; color: white; }
.btn-ws-primary:hover { background-color: #3182ce; }
.btn-ws-warning { background-color: #ed8936; color: white; }
.btn-ws-warning:hover { background-color: #dd6b20; }
.btn-ws-secondary { background-color: #edf2f7; color: #4a5568; }
.btn-ws-secondary:hover { background-color: #e2e8f0; }

.websocket-data { background: #f7fafc; padding: 15px; border-radius: 8px; font-size: 0.95rem; }
.ws-status { margin-bottom: 15px; padding-bottom: 10px; border-bottom: 1px solid #e2e8f0; }
.ws-messages ul { list-style: none; padding: 0; margin: 10px 0 0 0; }
.ws-messages li { padding: 5px 0; color: #4a5568; }
.text-success { color: #48bb78; font-weight: bold; }
.text-warning-dark { color: #c05621; font-weight: bold; }
.text-muted { color: #a0aec0; font-style: italic; }
</style>