<template>
  <div class="page-container">
    <section class="action-menu">
        <h2>Gestionare Clienți</h2>
        <button class="btn btn-success" @click.prevent="adaugaClient">
            <i class="fas fa-user-plus"></i> Adaugă Client
        </button>
    </section>

    <main class="content-area">
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID Client</th>
                    <th>Nume client</th>
                    <th>Adresa</th>
                    <th>Telefon</th>
                    <th>Acțiuni</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="c in lista_clienti" :key="c.idClient">
                    <td>{{ c.idClient }}</td>
                    <td class="fw-bold">{{ c.NumeClient || c.numeClient }}</td>
                    <td>{{ c.Adresa || c.adresa }}</td>
                    <td>{{ c.Telefon || c.telefon }}</td>
                    <td class="actions">
                        <button class="btn-icon btn-edit" @click.prevent="editClient(c)" title="Edit">
                            <i class="fas fa-edit"></i> Edit
                        </button>
                        <button class="btn-icon btn-delete" @click.prevent="stergeClient(c)" title="Șterge">
                            <i class="fas fa-trash"></i> Șterge
                        </button>
                    </td>
                </tr>
                <tr v-if="lista_clienti.length === 0">
                    <td colspan="5" class="text-center empty-state">Nu există clienți în listă.</td>
                </tr>
            </tbody>
        </table>
    </main>

    <ClientDialog 
        :esteVizibil="showDialog" 
        titlu="Gestionare Client"
        :clientEditat="clientSelectat" 
        @salveaza="salveazaClient"
        @anuleazaEditare="showDialog = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue' // NOU: am adăugat onUnmounted
import Swal from 'sweetalert2' 
import ClientDialog from './ClientDialog.vue'
import * as signalR from '@microsoft/signalr' // NOU: Importăm SignalR

// --- VARIABILE PENTRU DIALOG ---
const showDialog = ref(false)
const clientSelectat = ref(null)

// --- VARIABILE PENTRU LISTA DE CLIENȚI ---
const lista_clienti = ref([]) 
const apiUrl = 'http://localhost:5116/Client'

// NOU: Instanța conexiunii SignalR
let connection = null;

// NOU: Am mutat logica de fetch într-o funcție dedicată pentru a putea fi apelată de SignalR
async function incarcaClienti() {
    try {
        const raspuns = await fetch(`${apiUrl}/GetClienti`)
        if (raspuns.ok) {
            const date = await raspuns.json()
            lista_clienti.value = date.map(c => ({
                idClient: c.idClient || c.IdClient,
                NumeClient: c.numeClient || c.NumeClient,
                Adresa: c.adresa || c.Adresa,
                Telefon: c.telefon || c.Telefon
            }))
        }
    } catch (error) {
        console.error('Eroare la încărcarea clienților', error)
        Swal.fire('Eroare!', 'Nu s-au putut încărca clienții din baza de date.', 'error')
    }
}

onMounted(async () => {
    // 1. Încărcăm clienții inițiali
    await incarcaClienti();

    // 2. Ne conectăm la canalul de clienți din C#
    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5116/clientiHub")
        .withAutomaticReconnect()
        .build();

    // 3. Ascultăm evenimentul live
    connection.on("UpdateClienti", () => {
        console.log("🔄 Schimbare detectată la clienți! Reîncărcăm lista...");
        incarcaClienti();
    });

    try {
        await connection.start();
        console.log("✅ Conectat la ClientiHub (SignalR)!");
    } catch (error) {
        console.error("❌ Eroare conectare SignalR Clienți:", error);
    }
})

onUnmounted(() => {
    if (connection) {
        connection.stop();
        console.log("🛑 Conexiunea SignalR pentru Clienți a fost închisă.");
    }
})

// ==========================================
// LOGICA PENTRU DIALOG (Adăugare / Editare)
// ==========================================
function adaugaClient() {
    clientSelectat.value = { idClient: 0, NumeClient: '', Adresa: '', Telefon: '' }
    showDialog.value = true
}

function editClient(client) {
    clientSelectat.value = { ...client }
    showDialog.value = true
}

async function salveazaClient(clientModificat) {
    const esteAdaugare = clientModificat.idClient === 0
    const metoda = esteAdaugare ? 'POST' : 'PUT'
    const endpoint = esteAdaugare 
        ? `${apiUrl}/PostClientModel` 
        : `${apiUrl}/PutClientModel?id=${clientModificat.idClient}`

    try {
        const raspuns = await fetch(endpoint, {
            method: metoda,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(clientModificat)
        })

        if (raspuns.ok) {
            // Nu mai facem push/findIndex manual aici deoarece SignalR va declanșa 
            // automat 'UpdateClienti' pentru toată lumea (inclusiv pentru noi) și va reîncărca baza de date.
            showDialog.value = false
            Swal.fire('Succes!', 'Clientul a fost salvat.', 'success')
        } else {
            Swal.fire('Eroare!', 'Salvarea a eșuat.', 'error')
        }
    } catch (error) {
        console.error('Eroare la salvare', error)
        Swal.fire('Eroare!', 'A apărut o problemă de conexiune.', 'error')
    }
}

function stergeClient(client) {
    Swal.fire({
        title: 'Ești sigur?',
        text: `Ștergi clientul ${client.NumeClient || client.numeClient}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e53e3e',
        cancelButtonColor: '#a0aec0',
        confirmButtonText: 'Da, șterge!'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                const raspuns = await fetch(`${apiUrl}/DeleteClientModel?id=${client.idClient}`, {
                    method: 'DELETE'
                })
                
                if (raspuns.ok) {
                    Swal.fire('Șters!', 'Clientul a fost șters.', 'success')
                } else {
                    Swal.fire('Eroare!', 'Ștergerea a eșuat.', 'error')
                }
            } catch (error) {
                console.error('Eroare la stergere', error)
                Swal.fire('Eroare!', 'A apărut o problemă de conexiune.', 'error')
            }
        }
    })
}
</script>

<style scoped>
/* CSS-ul tău rămâne neatins */
.page-container { padding: 20px; padding-bottom: 50px; }
.action-menu { padding: 20px; background-color: white; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.05); display: flex; justify-content: space-between; align-items: center;}
.action-menu h2 { color: #2d3748; margin: 0; font-size: 1.2rem; }
.btn { padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; display: flex; align-items: center; gap: 8px; transition: 0.2s;}
.btn-success { background-color: #48bb78; color: white; }
.btn-success:hover { background-color: #38a169; }
.data-table { width: 100%; border-collapse: collapse; background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }
.data-table th { background-color: #f7fafc; text-align: left; padding: 15px; font-weight: bold; color: #4a5568; border-bottom: 2px solid #e2e8f0; }
.data-table td { padding: 15px; border-bottom: 1px solid #edf2f7; vertical-align: middle; color: #4a5568;}
.data-table tbody tr:hover { background-color: #fcfcfc; }
.fw-bold { font-weight: 600; color: #2d3748; }
.text-center { text-align: center; }
.empty-state { padding: 30px !important; color: #a0aec0; font-style: italic;}
.actions { display: flex; gap: 10px; }
.btn-icon { border: none; background: none; cursor: pointer; padding: 8px; border-radius: 5px; font-size: 1rem; display: flex; align-items: center; gap: 5px; transition: 0.2s; font-weight: bold;}
.btn-edit { color: #4299e1; }
.btn-edit:hover { background-color: #ebf8ff; color: #2b6cb0; }
.btn-delete { color: #e53e3e; }
.btn-delete:hover { background-color: #fff5f5; color: #c53030; }
</style>