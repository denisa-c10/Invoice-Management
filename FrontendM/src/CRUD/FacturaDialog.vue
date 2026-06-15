<template>
    <div v-if="esteVizibil" class="modal-overlay">
        <div class="modal-content large-modal">
            <h2 class="modal-title">Detalii Factură</h2>
            
            <form v-if="factura" @submit.prevent="salveazaFactura" class="modal-form">
                <div class="form-row">
                    <div class="form-group flex-1">
                        <label>Nr. factură</label>
                        <input type="text" class="form-control" :value="factura.nrFactura === 0 ? 'NOU' : factura.nrFactura" disabled />
                    </div>
                    <div class="form-group flex-1">
                        <label>Data factură <span class="required">*</span></label>
                        <input type="date" class="form-control" v-model="factura.dataFactura" required />
                    </div>
                </div>

                <div class="form-group">
                    <label>Client <span class="required">*</span></label>
                    <select class="form-control" v-model="factura.idClient" required>
                        <option value="" disabled>-- Selectează un client --</option>
                        <option v-for="c in clienti" :key="c.idClient" :value="c.idClient">
                            {{ c.NumeClient || c.numeClient }} ({{ c.Telefon || c.telefon }})
                        </option>
                    </select>
                </div>

                <div class="products-section">
                    <h3>Produse pe factură</h3>
                    <div class="add-product-row">
                        <select class="form-control flex-2" v-model="produsCurent">
                            <option :value="null" disabled>-- Alege produs --</option>
                            <option v-for="p in produse" :key="p.id" :value="p">
                                {{ p.nume }} - {{ p.pret }} RON
                            </option>
                        </select>
                        <input type="number" class="form-control flex-1" v-model="cantitateCurenta" min="1" placeholder="Buc." />
                        
                        <button type="button" class="btn-add-product" @click="adaugaProdusPeFactura" :disabled="!produsCurent">
                            <span class="plus-sign">+</span> Adaugă
                        </button>
                    </div>

                    <ul class="invoice-items">
                        <li v-for="(item, index) in factura.produse" :key="index">
                            <span>{{ item.nume }} (x{{ item.cantitate }})</span>
                            <div class="item-right">
                                <span>{{ item.pret * item.cantitate }} RON</span>
                                <button type="button" class="btn-delete-small" @click="stergeProdusDePeFactura(index)">❌</button>
                            </div>
                        </li>
                        <li v-if="!factura.produse || factura.produse.length === 0" class="empty-items">Nu ai adăugat niciun produs.</li>
                    </ul>
                </div>
                
                <div class="form-group total-group">
                    <label>Total de plată:</label>
                    <h3 class="total-amount">{{ calculeazaTotal() }} RON</h3>
                </div>
                
                <div class="modal-actions">
                    <button type="button" class="btn btn-secondary" @click="anuleazaEditare">Anulează</button>
                    <button type="submit" class="btn btn-primary">Salvează Factura</button>
                </div>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
    esteVizibil: Boolean,
    facturaEditata: Object,
    clienti: Array,
    produse: Array
})

const emit = defineEmits(['salveaza', 'anuleaza'])

const factura = ref(null)
const produsCurent = ref(null)
const cantitateCurenta = ref(1)

watch(() => props.facturaEditata, (newVal) => {
    if (newVal) {
        factura.value = JSON.parse(JSON.stringify(newVal))
        if (!factura.value.produse) {
            factura.value.produse = []
        }
    }
})

function adaugaProdusPeFactura() {
    if (produsCurent.value && cantitateCurenta.value > 0) {
        factura.value.produse.push({
            idProdus: produsCurent.value.id || produsCurent.value.IdProdus,
            nume: produsCurent.value.nume || produsCurent.value.NumeProdus,
            pret: produsCurent.value.pret || produsCurent.value.Pret,
            cantitate: cantitateCurenta.value
        })
        produsCurent.value = null
        cantitateCurenta.value = 1
    }
}

function stergeProdusDePeFactura(index) {
    factura.value.produse.splice(index, 1)
}

function calculeazaTotal() {
    if (!factura.value || !factura.value.produse) return 0;
    const suma = factura.value.produse.reduce((acc, item) => acc + (item.pret * item.cantitate), 0);
    factura.value.total = suma; 
    return suma;
}

function salveazaFactura(){
    emit('salveaza', factura.value)
}

function anuleazaEditare(){
    emit('anuleaza')
}
</script>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background-color: rgba(0, 0, 0, 0.6); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background-color: white; width: 100%; border-radius: 10px; padding: 30px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); animation: slideIn 0.3s ease-out; }
.large-modal { max-width: 550px; }

@keyframes slideIn { from { opacity: 0; transform: translateY(-20px); } to { opacity: 1; transform: translateY(0); } }
.modal-title { color: #2d3748; margin-bottom: 20px; font-size: 1.3rem; border-bottom: 2px solid #edf2f7; padding-bottom: 10px; }
.form-group { margin-bottom: 15px; text-align: left; }
.form-row { display: flex; gap: 15px; }
.flex-1 { flex: 1; min-width: 80px; }
.flex-2 { flex: 2; }
.form-group label { display: block; margin-bottom: 5px; font-weight: 600; color: #4a5568; font-size: 0.9rem; }
.required { color: #e53e3e; }
.form-control { width: 100%; padding: 10px; border: 1px solid #cbd5e0; border-radius: 6px; font-size: 1rem; outline: none; transition: 0.2s; background: white;}
.form-control:focus { border-color: #4299e1; box-shadow: 0 0 0 3px rgba(66, 153, 225, 0.2); }
.form-control:disabled { background-color: #edf2f7; color: #a0aec0; cursor: not-allowed; }

/* Secțiunea de produse */
.products-section { background: #f7fafc; padding: 15px; border-radius: 8px; margin-bottom: 15px; border: 1px solid #e2e8f0; }
.products-section h3 { font-size: 1rem; margin-bottom: 10px; color: #2d3748; }
.add-product-row { display: flex; gap: 10px; margin-bottom: 15px; align-items: stretch; }

/* Butonul NOU de Adăugare Produs */
.btn-add-product {
    background-color: #48bb78;
    color: white;
    border: none;
    border-radius: 6px;
    padding: 0 15px;
    font-weight: bold;
    font-size: 0.95rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 6px;
    transition: 0.2s;
    white-space: nowrap;
}
.btn-add-product:hover:not(:disabled) { background-color: #38a169; }
.btn-add-product:disabled { background-color: #cbd5e0; color: #a0aec0; cursor: not-allowed; }
.plus-sign { font-size: 1.3rem; font-weight: bold; line-height: 1; }

.invoice-items { list-style: none; padding: 0; margin: 0; max-height: 120px; overflow-y: auto; }
.invoice-items li { display: flex; justify-content: space-between; align-items: center; padding: 8px 0; border-bottom: 1px dashed #cbd5e0; font-size: 0.95rem; }
.invoice-items li:last-child { border-bottom: none; }
.empty-items { color: #a0aec0; font-style: italic; font-size: 0.9rem !important; }
.item-right { display: flex; align-items: center; gap: 10px; font-weight: bold; }

/* Butonul NOU de ștergere produs de pe listă */
.btn-delete-small { 
    background: none; 
    border: none; 
    padding: 4px; 
    cursor: pointer; 
    font-size: 0.8rem;
    opacity: 0.7;
    transition: 0.2s;
}
.btn-delete-small:hover { opacity: 1; transform: scale(1.1); }

/* Total */
.total-group { text-align: right; background: #ebf8ff; padding: 15px; border-radius: 8px; margin-top: 10px; }
.total-amount { color: #2b6cb0; font-size: 1.5rem; margin: 0; }

.modal-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 25px; }
.btn { padding: 10px 20px; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; transition: 0.2s; }
.btn-secondary { background-color: #e2e8f0; color: #4a5568; }
.btn-secondary:hover { background-color: #cbd5e0; }
.btn-primary { background-color: #4299e1; color: white; }
.btn-primary:hover { background-color: #3182ce; }
</style>