<template>
    <div v-if="esteVizibil" class="modal-overlay">
        <div class="modal-content">
            <h2 class="modal-title">{{ titlu }}</h2>
            
            <form @submit.prevent="salveazaClient" class="modal-form">
                
                <div class="form-group">
                    <label for="idClient">ID client</label>
                    <input type="text" id="idClient" class="form-control" v-model="clientEditat.idClient" disabled />
                </div>
                
                <div class="form-group">
                    <label for="numeClient">Nume client <span class="required">*</span></label>
                    <input type="text" id="numeClient" class="form-control" v-model="clientEditat.NumeClient" required placeholder="Ex: Popescu Ion" />
                </div>
                
                <div class="form-group">
                    <label for="adresa">Adresa</label>
                    <input type="text" id="adresa" class="form-control" v-model="clientEditat.Adresa" placeholder="Strada, Număr, Oraș..." />
                </div>
                
                <div class="form-group">
                    <label for="telefon">Telefon</label>
                    <input type="text" id="telefon" class="form-control" v-model="clientEditat.Telefon" placeholder="07XX XXX XXX" />
                </div>
                
                <div class="modal-actions">
                    <button type="button" class="btn btn-secondary" @click.prevent="anuleazaEditare">Anulează</button>
                    <button type="submit" class="btn btn-primary">Salvează</button>
                </div>
            </form>
        </div>
    </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
    esteVizibil: Boolean,
    titlu: String,
    clientEditat: Object
})

const emit = defineEmits(['salveaza', 'anuleazaEditare'])

function salveazaClient() {
    emit('salveaza', props.clientEditat)
}

function anuleazaEditare() {
    emit('anuleazaEditare')
}
</script>

<style scoped>
/* Fundalul care acoperă tot ecranul */
.modal-overlay {
    position: fixed;
    top: 0; left: 0; width: 100vw; height: 100vh;
    background-color: rgba(0, 0, 0, 0.6);
    display: flex; justify-content: center; align-items: center;
    z-index: 1000;
}

/* Cutia albă din centru */
.modal-content {
    background-color: white;
    width: 100%;
    max-width: 450px;
    border-radius: 10px;
    padding: 30px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
    animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
    from { opacity: 0; transform: translateY(-20px); }
    to { opacity: 1; transform: translateY(0); }
}

.modal-title {
    color: #2d3748;
    margin-bottom: 25px;
    font-size: 1.4rem;
    border-bottom: 2px solid #edf2f7;
    padding-bottom: 10px;
}

/* Stilizare Formular */
.form-group {
    margin-bottom: 18px;
    text-align: left;
}

.form-group label {
    display: block;
    margin-bottom: 6px;
    font-weight: 600;
    color: #4a5568;
    font-size: 0.9rem;
}

.required { color: #e53e3e; }

.form-control {
    width: 100%;
    padding: 10px 12px;
    border: 1px solid #cbd5e0;
    border-radius: 6px;
    outline: none;
    font-size: 1rem;
    transition: all 0.2s;
    background-color: #fff;
}

.form-control:focus {
    border-color: #4299e1;
    box-shadow: 0 0 0 3px rgba(66, 153, 225, 0.2);
}

.form-control:disabled {
    background-color: #edf2f7;
    color: #a0aec0;
    cursor: not-allowed;
}

/* Zona de butoane */
.modal-actions {
    display: flex;
    justify-content: flex-end;
    gap: 15px;
    margin-top: 30px;
}

.btn {
    padding: 10px 20px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-weight: bold;
    font-size: 0.95rem;
    transition: opacity 0.2s;
}

.btn:hover { opacity: 0.9; }

.btn-secondary {
    background-color: #e2e8f0;
    color: #4a5568;
}

.btn-primary {
    background-color: #4299e1;
    color: white;
}
</style>