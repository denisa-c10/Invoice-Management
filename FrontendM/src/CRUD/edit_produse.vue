<template>
  <div class="page-container">
    <section class="action-menu">
        <h2>Gestionare Produse (Stoc)</h2>
        <button class="btn btn-success" @click.prevent="deschideAdaugare">
            <i class="fas fa-plus"></i> Adaugă Produs
        </button>
    </section>

    <main class="content-area">
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nume Produs</th>
                    <th>Preț (RON)</th>
                    <th>Stoc curent</th>
                    <th>Acțiuni</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="p in lista_produse" :key="p.id">
                    <td>{{ p.id }}</td>
                    <td class="fw-bold">{{ p.nume }}</td>
                    <td class="amount-cell">{{ p.pret }}</td>
                    
                    <td>
                        <div class="quantity-controls">
                            <button class="btn-qty" @click.prevent="modificaStoc(p, -1)" :disabled="p.cantitate <= 0">-</button>
                            <span class="qty-value">{{ p.cantitate }}</span>
                            <button class="btn-qty" @click.prevent="modificaStoc(p, 1)">+</button>
                        </div>
                    </td>

                    <td class="actions">
                        <button class="btn-icon btn-edit" @click.prevent="editProdus(p)" title="Edit"><i class="fas fa-edit"></i> Edit</button>
                        <button class="btn-icon btn-delete" @click.prevent="stergeProdus(p)" title="Șterge"><i class="fas fa-trash"></i> Șterge</button>
                    </td>
                </tr>
                <tr v-if="lista_produse.length === 0">
                    <td colspan="5" class="text-center empty-state">Nu există produse în listă.</td>
                </tr>
            </tbody>
        </table>
    </main>

    <ProdusDialog 
        :esteVizibil="showDialog" 
        :produsEditat="produsSelectat" 
        @salveaza="salveazaDinDialog"
        @anuleaza="showDialog = false"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue' // NOU: am adaugat onUnmounted
import Swal from 'sweetalert2'
import ProdusDialog from './ProdusDialog.vue'
import * as signalR from '@microsoft/signalr' // NOU: Importăm SignalR

const showDialog = ref(false)
const produsSelectat = ref(null)
const lista_produse = ref([])

const apiUrl = 'http://localhost:5116/Produs' 

// NOU: Variabila pentru conexiunea SignalR
let connection = null;

onMounted(async () => {
    // 1. Încărcăm produsele normal
    incarcaProduse()

    // 2. Ne conectăm la canalul de WebSockets (SignalR)
    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5116/produseHub") 
        .withAutomaticReconnect()
        .build();

    // 3. Când auzim pe stație mesajul "UpdateProduse", reîncărcăm tabelul
    connection.on("UpdateProduse", () => {
        console.log("🔄 Actualizare în timp real primită! Reîncărcăm stocul...");
        incarcaProduse();
    });

    // 4. Pornim efectiv conexiunea
    try {
        await connection.start();
        console.log("✅ Conectat la ProduseHub (SignalR)!");
    } catch (error) {
        console.error("❌ Eroare la conectarea SignalR:", error);
    }
})

// NOU: Oprim conexiunea când utilizatorul părăsește această pagină
onUnmounted(() => {
    if (connection) {
        connection.stop();
        console.log("🛑 Conexiunea SignalR a fost închisă.");
    }
})

async function incarcaProduse() {
    const token = localStorage.getItem('token_jwt'); 
    try {
        const raspuns = await fetch(`${apiUrl}/GetProdusModels`, {
            headers: { 'Authorization': `Bearer ${token}` } 
        })
        if (raspuns.ok) {
            const date = await raspuns.json()
            lista_produse.value = date.map(p => ({
                id: p.idProdus || p.IdProdus,
                nume: p.numeProdus || p.NumeProdus,
                pret: p.pret || p.Pret,
                cantitate: p.cantitate || p.Cantitate || 0 
            }))
        }
    } catch (error) {
        console.error('Eroare la încărcarea produselor', error)
    }
}

// -----------------------------------------
async function modificaStoc(produs, valoare) {
    const cantitateNoua = produs.cantitate + valoare;
    if (cantitateNoua < 0) return; 

    // Actualizăm instant local (SignalR va confirma pe celelalte browsere)
    produs.cantitate = cantitateNoua;

    const produsPentruCsharp = {
        IdProdus: produs.id,
        NumeProdus: produs.nume,
        Pret: produs.pret,
        Cantitate: cantitateNoua
    };

    const token = localStorage.getItem('token_jwt'); 

    try {
        await fetch(`${apiUrl}/PutProdusModel?id=${produs.id}`, {
            method: 'PUT',
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` 
            },
            body: JSON.stringify(produsPentruCsharp)
        });
        // Când acest fetch se termină cu succes, C#-ul va striga "UpdateProduse" pe SignalR
    } catch (error) {
        console.error("Eroare la salvarea cantității", error);
        produs.cantitate -= valoare; 
        Swal.fire({
            toast: true, position: 'top-end', icon: 'error',
            title: 'Eroare la actualizarea stocului', showConfirmButton: false, timer: 3000
        });
    }
}

// -----------------------------------------
// Logica ferestrei de Dialog
// -----------------------------------------
function deschideAdaugare() {
    produsSelectat.value = { idProdus: 0, numeProdus: '', pret: 0, cantitate: 1 }
    showDialog.value = true
}

function editProdus(p) {
    produsSelectat.value = { idProdus: p.id, numeProdus: p.nume, pret: p.pret, cantitate: p.cantitate }
    showDialog.value = true
}

async function salveazaDinDialog(produsModificat) {
    const esteAdaugare = produsModificat.idProdus === 0
    const metoda = esteAdaugare ? 'POST' : 'PUT'
    const endpoint = esteAdaugare 
        ? `${apiUrl}/PostProdusModel` 
        : `${apiUrl}/PutProdusModel?id=${produsModificat.idProdus}`

    const produsPentruCsharp = {
        IdProdus: produsModificat.idProdus,
        NumeProdus: produsModificat.numeProdus,
        Pret: produsModificat.pret,
        Cantitate: produsModificat.cantitate
    };

    const token = localStorage.getItem('token_jwt'); 

    try {
        const raspuns = await fetch(endpoint, {
            method: metoda,
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` 
            },
            body: JSON.stringify(produsPentruCsharp)
        })

        if (raspuns.ok) {
            // Aici nu e neapărat nevoie să mai dăm incarcaProduse() manual, 
            // pentru că SignalR o va declanșa oricum, dar o lăsăm ca plasă de siguranță locală.
            await incarcaProduse() 
            showDialog.value = false
            Swal.fire('Succes!', 'Produsul a fost salvat.', 'success')
        } else {
            Swal.fire('Eroare!', 'Salvarea a eșuat.', 'error')
        }
    } catch (error) {
        Swal.fire('Eroare!', 'A apărut o problemă de conexiune.', 'error')
    }
}

function stergeProdus(p) {
    const token = localStorage.getItem('token_jwt'); 

    Swal.fire({
        title: 'Ești sigur?', text: `Ștergi produsul ${p.nume}?`, icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#e53e3e', cancelButtonColor: '#a0aec0', confirmButtonText: 'Da, șterge!'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                const raspuns = await fetch(`${apiUrl}/DeleteProdusModel?id=${p.id}`, { 
                    method: 'DELETE',
                    headers: { 'Authorization': `Bearer ${token}` } 
                })
                if (raspuns.ok) {
                    lista_produse.value = lista_produse.value.filter(x => x.id !== p.id)
                    Swal.fire('Șters!', 'Produsul a fost șters.', 'success')
                }
            } catch (error) {
                 Swal.fire('Eroare!', 'Problemă de conexiune.', 'error')
            }
        }
    })
}
</script>

<style scoped>
/* Aceleași stiluri vizuale CSS */
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
.amount-cell { font-weight: 600; color: #3182ce; }
.text-center { text-align: center; }
.empty-state { padding: 30px !important; color: #a0aec0; font-style: italic;}
.quantity-controls { display: flex; align-items: center; gap: 10px; background: #f7fafc; padding: 4px; border-radius: 6px; width: fit-content; }
.btn-qty { border: none; background: white; width: 28px; height: 28px; border-radius: 4px; cursor: pointer; font-weight: bold; color: #4a5568; box-shadow: 0 1px 2px rgba(0,0,0,0.05); transition: 0.2s;}
.btn-qty:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-qty:hover:not(:disabled) { background: #edf2f7; }
.qty-value { min-width: 20px; text-align: center; font-weight: 600; }
.actions { display: flex; gap: 10px; }
.btn-icon { border: none; background: none; cursor: pointer; padding: 8px; border-radius: 5px; font-size: 1rem; display: flex; align-items: center; gap: 5px; transition: 0.2s; font-weight: bold;}
.btn-edit { color: #4299e1; }
.btn-edit:hover { background-color: #ebf8ff; color: #2b6cb0; }
.btn-delete { color: #e53e3e; }
.btn-delete:hover { background-color: #fff5f5; color: #c53030; }
</style>