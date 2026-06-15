<template>
    <div v-if="esteVizibil" class="modal-overlay">
        <div class="modal-content">
            <h2 class="modal-title">Detalii Produs</h2>
            
            <form v-if="produs" @submit.prevent="salveazaProdus" class="modal-form">
                <div class="form-group">
                    <label>ID produs</label>
                    <input type="text" class="form-control" :value="produs.idProdus === 0 ? 'NOU' : produs.idProdus" disabled />
                </div>
                
                <div class="form-group">
                    <label for="numeProdus">Nume produs <span class="required">*</span></label>
                    <input type="text" id="numeProdus" class="form-control" v-model="produs.numeProdus" placeholder="Introdu numele..." required />
                </div>

                <div class="form-row">
                    <div class="form-group flex-1">
                        <label for="pret">Preț (RON) <span class="required">*</span></label>
                        <input type="number" id="pret" class="form-control" v-model="produs.pret" step="0.01" min="0" placeholder="0.00" required />
                    </div>
                    
                    <div class="form-group flex-1">
                        <label for="cantitate">Stoc <span class="required">*</span></label>
                        <input type="number" id="cantitate" class="form-control" v-model="produs.cantitate" min="0" required />
                    </div>
                </div>
                
                <div class="modal-actions">
                    <button type="button" class="btn btn-secondary" @click="anuleazaEditare">Anulează</button>
                    <button type="submit" class="btn btn-primary">Salvează</button>
                </div>
            </form>
        </div>
    </div>
</template>

<script setup>
import {ref, watch} from 'vue'

const props = defineProps({
    esteVizibil: Boolean,
    produsEditat: Object
})

const emit = defineEmits(['salveaza', 'anuleaza'])
const produs = ref(null)

watch(() => props.produsEditat, (newVal) => {
    if (newVal) { produs.value = {...newVal} }
})

function salveazaProdus(){
    emit('salveaza', produs.value)
}

function anuleazaEditare(){
    emit('anuleaza')
}
</script>

<style scoped>
/* Aceleași stiluri */
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background-color: rgba(0, 0, 0, 0.6); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background-color: white; width: 100%; max-width: 450px; border-radius: 10px; padding: 30px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); animation: slideIn 0.3s ease-out; }
@keyframes slideIn { from { opacity: 0; transform: translateY(-20px); } to { opacity: 1; transform: translateY(0); } }
.modal-title { color: #2d3748; margin-bottom: 20px; font-size: 1.3rem; border-bottom: 2px solid #edf2f7; padding-bottom: 10px; }
.form-row { display: flex; gap: 15px; }
.flex-1 { flex: 1; }
.form-group { margin-bottom: 15px; text-align: left; }
.form-group label { display: block; margin-bottom: 5px; font-weight: 600; color: #4a5568; font-size: 0.9rem; }
.required { color: #e53e3e; }
.form-control { width: 100%; padding: 10px; border: 1px solid #cbd5e0; border-radius: 6px; font-size: 1rem; outline: none; transition: 0.2s; background: white;}
.form-control:focus { border-color: #4299e1; box-shadow: 0 0 0 3px rgba(66, 153, 225, 0.2); }
.form-control:disabled { background-color: #edf2f7; color: #a0aec0; cursor: not-allowed; }
.modal-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 25px; }
.btn { padding: 10px 20px; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; transition: 0.2s; }
.btn-secondary { background-color: #e2e8f0; color: #4a5568; }
.btn-secondary:hover { background-color: #cbd5e0; }
.btn-primary { background-color: #48bb78; color: white; }
.btn-primary:hover { background-color: #38a169; }
</style>