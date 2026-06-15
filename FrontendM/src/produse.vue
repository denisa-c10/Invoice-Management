<template>
  <div class="page-container">
    <section class="action-menu">
        <h2>Gestionare Produse</h2>
        <button class="btn btn-success" @click.prevent="adaugaProdus">
            <i class="fas fa-plus"></i> Adaugă Produs
        </button>
    </section>

    <main class="content-area">
        <table class="data-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nume Produs</th>
                    <th>Cantitate</th>
                    <th>Acțiuni</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="p in lista_produse" :key="p.id">
                    <td>{{ p.id }}</td>
                    <td class="fw-bold">{{ p.nume }}</td>
                    <td>
                        <div class="quantity-controls">
                            <button class="btn-qty" @click.prevent="p.cantitate--" :disabled="p.cantitate <= 1">-</button>
                            <span class="qty-value">{{ p.cantitate }}</span>
                            <button class="btn-qty" @click.prevent="cresteCantitatea(p)">+</button>
                        </div>
                    </td>
                    <td class="actions">
                        <button class="btn-icon btn-edit" title="Edit"><i class="fas fa-edit"></i> Edit</button>
                        <button class="btn-icon btn-delete" @click.prevent="stergeProdus(p)" title="Șterge"><i class="fas fa-trash"></i> Șterge</button>
                    </td>
                </tr>
            </tbody>
        </table>
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const lista_produse = ref([
    {id: 1, nume: 'Ciocolata', cantitate: 1},
    {id: 2, nume: 'Paine', cantitate: 2}
])

function adaugaProdus() {
    const id = lista_produse.value.length > 0 ? Math.max(...lista_produse.value.map(p => p.id)) + 1 : 1;
    lista_produse.value.push({id, nume: 'Produs Nou', cantitate: 1})
}

function stergeProdus(produs) {
    lista_produse.value = lista_produse.value.filter(p => p.id !== produs.id)
}

function cresteCantitatea(produs) {
    produs.cantitate++
}
</script>

<style scoped>
.page-container { padding: 20px; }
.action-menu { display: flex; justify-content: space-between; align-items: center; background: white; padding: 20px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
.btn { padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; }
.btn-success { background-color: #48bb78; color: white; }
.data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }
.data-table th { background: #f7fafc; text-align: left; padding: 15px; border-bottom: 2px solid #e2e8f0; }
.data-table td { padding: 15px; border-bottom: 1px solid #edf2f7; vertical-align: middle; }
.quantity-controls { display: flex; align-items: center; gap: 10px; }
.btn-qty { border: 1px solid #cbd5e0; background: white; width: 25px; height: 25px; border-radius: 4px; cursor: pointer; }
.btn-icon { border: none; background: none; cursor: pointer; margin-right: 10px; font-weight: bold; }
.btn-edit { color: #4299e1; }
.btn-delete { color: #e53e3e; }
</style>