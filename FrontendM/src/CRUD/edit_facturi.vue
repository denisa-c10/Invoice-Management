<template>
  <div class="page-container">
    <section class="action-menu">
        <h2>Gestionare Facturi</h2>
        <button class="btn btn-success" @click="adaugaFactura">
            <i class="fas fa-file-invoice"></i> Adaugă Factură
        </button>
    </section>

    <main class="content-area">
        <div class="search-bar-container">
            <i class="fas fa-search search-icon"></i>
            <input 
                type="text" 
                v-model="searchQuery" 
                placeholder="Caută după nr. factură, dată sau total..." 
                class="search-input" 
            />
        </div>

        <table class="data-table">
            <thead>
                <tr>
                    <th>Nr. Factură</th>
                    <th>Data Factură</th>
                    <th>Total (RON)</th>
                    <th>Status</th> 
                    <th>Acțiuni</th>
                </tr>
            </thead>
            <tbody>
                <tr v-if="lista_facturi.length === 0">
                    <td colspan="5" class="text-center empty-state">Nu există facturi înregistrate.</td>
                </tr>
                <tr v-else-if="facturiFiltrate.length === 0">
                    <td colspan="5" class="text-center empty-state">
                        Nu s-a găsit nicio factură pentru "<strong>{{ searchQuery }}</strong>".
                    </td>
                </tr>
                <tr v-else v-for="factura in facturiFiltrate" :key="factura.nrFactura">
                    <td class="fw-bold">#{{ factura.nrFactura }}</td>
                    <td>{{ formatareData(factura.dataFactura) }}</td>
                    <td class="amount-cell">{{ factura.total }}</td>
                    
                    <td>
                        <span :class="factura.estePlatita ? 'badge badge-success' : 'badge badge-muted'">
                            {{ factura.estePlatita ? '💵 Plătită' : '⏳ În așteptare' }}
                        </span>
                    </td>

                    <td class="actions">
                        <button 
                            class="btn-icon" 
                            :style="{ color: factura.estePlatita ? '#e53e3e' : '#38a169' }"
                            @click.prevent="schimbaStatusPlata(factura)" 
                            :title="factura.estePlatita ? 'Marchează ca Neplătită' : 'Încasează Factura'"
                        >
                            <i :class="factura.estePlatita ? 'fas fa-times-circle' : 'fas fa-check-circle'"></i>
                            {{ factura.estePlatita ? 'Anulează' : 'Încasează' }}
                        </button>

                        <button class="btn-icon btn-pdf" @click.prevent="genereazaPDF(factura)" title="Descarcă PDF">
                            📄 PDF
                        </button>
                        <button class="btn-icon btn-edit" @click.prevent="editFactura(factura)" title="Edit">
                            <i class="fas fa-edit"></i> Edit
                        </button>
                        <button class="btn-icon btn-delete" @click.prevent="stergeFactura(factura)" title="Șterge">
                            <i class="fas fa-trash"></i> Șterge
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </main>
    
    <FacturaDialog 
        :esteVizibil="showDialog" 
        :facturaEditata="facturaSelectata" 
        :clienti="lista_clienti" 
        :produse="lista_produse"
        @salveaza="salveazaFacturaInAPI"
        @anuleaza="showDialog = false"
    />

    <div style="display: none;">
      <div id="factura-print-template" class="factura-pdf">
        <div class="factura-header" style="position: relative;">
          <div v-if="facturaPentruPDF?.estePlatita" class="stampila-pdf stampila-platit">ACHITAT</div>
          <div v-else class="stampila-pdf stampila-neplatit">NEACHITAT</div>

          <div class="companie-info">
            <h3>FURNIZOR</h3>
            <p><strong>S.C. Sistem Facturi S.R.L.</strong></p>
            <p>CIF: RO12345678</p>
            <p>Sediu: Str. Principală, Nr. 1, Brașov</p>
          </div>
          <div class="factura-detalii">
            <h1>FACTURĂ</h1>
            <p><strong>Număr:</strong> #{{ facturaPentruPDF?.nrFactura }}</p>
            <p><strong>Dată:</strong> {{ formatareData(facturaPentruPDF?.dataFactura) }}</p>
            <p><strong>Status:</strong> {{ facturaPentruPDF?.estePlatita ? 'ACHITATĂ' : 'NEACHITATĂ' }}</p>
          </div>
        </div>

        <div class="factura-client">
          <h3>CLIENT</h3>
          <p><strong>{{ facturaPentruPDF?.clientNume || 'Client Necunoscut' }}</strong></p>
          <p>Adresă: {{ facturaPentruPDF?.clientAdresa || '-' }}</p>
          <p>Telefon: {{ facturaPentruPDF?.clientTelefon || '-' }}</p>
        </div>

        <table class="factura-produse">
          <thead>
            <tr>
              <th>Nr. crt.</th>
              <th>Denumire Produs</th>
              <th>Cantitate</th>
              <th>Preț Unitar</th>
              <th>Valoare (RON)</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(prod, index) in facturaPentruPDF?.produse" :key="index">
              <td>{{ index + 1 }}</td>
              <td>{{ prod.nume || 'Produs sters' }}</td>
              <td>{{ prod.cantitate }}</td>
              <td>{{ prod.pret }}</td>
              <td>{{ (prod.cantitate * prod.pret).toFixed(2) }}</td>
            </tr>
            <tr v-if="!facturaPentruPDF?.produse || facturaPentruPDF.produse.length === 0">
                <td colspan="5" style="text-align:center; color:#a0aec0;">Niciun produs adăugat.</td>
            </tr>
          </tbody>
        </table>

        <div class="factura-total">
          <div class="total-box">
            <h2><strong>Total de plată:</strong> {{ facturaPentruPDF?.total }} RON</h2>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
// --- IMPORTURI ---
// Funcții fundamentale din Vue.js pentru a controla starea și ciclul de viață al paginii
import { ref, computed, onMounted, onUnmounted } from 'vue' 
// Librărie externă pentru alerte pop-up frumoase
import Swal from 'sweetalert2'
// Componenta fiu care reprezintă fereastra modală pentru editare/adăugare
import FacturaDialog from './FacturaDialog.vue'
// Librărie pentru generarea fișierelor PDF din cod HTML
import html2pdf from 'html2pdf.js'
// Librărie Microsoft pentru conexiuni WebSockets în timp real
import * as signalR from '@microsoft/signalr' 

// --- VARIABILE REACTIVE (STARE) ---
// Folosim 'ref' pentru a face aceste variabile "reactive". 
// Dacă valoarea lor se modifică în cod, interfața grafică se va actualiza automat.
const lista_facturi = ref([])
const lista_clienti = ref([]) 
const lista_produse = ref([]) 

// Variabile pentru controlul ferestrei modale (FacturaDialog)
const facturaSelectata = ref(null) // Păstrează datele facturii care este editată momentan
const showDialog = ref(false)      // Controlează vizibilitatea ferestrei modale (true = deschis, false = închis)

// Variabile pentru funcționalități specifice
const facturaPentruPDF = ref(null) // Păstrează temporar datele mapate strict pentru șablonul PDF ascuns
const searchQuery = ref('')        // Păstrează textul introdus de utilizator în bara de căutare

// --- URL-uri BACKEND ---
// Adresele de bază ale API-ului tău din ASP.NET Core
const apiFacturaUrl = 'http://localhost:5116/Factura'
const apiClientUrl = 'http://localhost:5116/Client'
const apiProdusUrl = 'http://localhost:5116/Produs'

// Instanța conexiunii SignalR (folosim 'let' deoarece va fi construită mai târziu în onMounted)
let connectionFacturi = null;

// --- FUNCȚII UTILITARE ---
// Transformă data dintr-un format server (ex: "2026-06-07T00:00:00") într-unul vizual ("07.06.2026")
function formatareData(dataStr) {
    if(!dataStr) return '';
    try { 
        return new Date(dataStr).toLocaleDateString('ro-RO'); 
    } catch { 
        return dataStr; // Fallback în caz că data e coruptă
    }
}

// --- LOGICA BAREI DE CĂUTARE (Filtrare) ---
// 'computed' creează o variabilă derivată care se recalculează AUTOMAT ori de câte ori 'searchQuery' sau 'lista_facturi' se modifică.
// Tabelul din HTML va folosi acest 'facturiFiltrate' în loc de 'lista_facturi' pentru afișare.
const facturiFiltrate = computed(() => {
    // Dacă bara de căutare e goală, returnează toată lista
    if (!searchQuery.value) return lista_facturi.value;
    
    const termenCautat = searchQuery.value.toLowerCase();
    
    // Filtrează array-ul păstrând doar elementele care conțin termenul căutat în ID, Dată sau Total
    return lista_facturi.value.filter(factura => {
        const numarString = factura.nrFactura.toString().toLowerCase();
        const dataString = formatareData(factura.dataFactura).toLowerCase();
        const totalString = factura.total.toString().toLowerCase();
        
        return numarString.includes(termenCautat) || 
               dataString.includes(termenCautat) || 
               totalString.includes(termenCautat);
    });
});

// ==========================================
// FUNCȚIA DE ÎNCASARE FACTURĂ
// ==========================================
function schimbaStatusPlata(factura) {
    const noulStatus = !factura.estePlatita; // Inversează statusul curent (din true în false și invers)
    
    // Construim mesajul de confirmare în funcție de acțiunea dorită
    const textAlerta = noulStatus 
        ? `Marchezi factura #${factura.nrFactura} ca plătită? Această acțiune va SCĂDEA produsele din stoc.`
        : `Anulezi plata pentru factura #${factura.nrFactura}? Această acțiune va RETURNA produsele în stoc.`;

    Swal.fire({
        title: 'Modificare Status Plată',
        text: textAlerta,
        icon: 'info',
        showCancelButton: true,
        confirmButtonColor: noulStatus ? '#38a169' : '#e53e3e',
        cancelButtonText: 'Anulează',
        confirmButtonText: 'Da, continuă!'
    }).then((result) => {
        // Dacă utilizatorul a apăsat "Da"
        if (result.isConfirmed) {
            const token = localStorage.getItem('token_jwt'); // Preluăm cheia de securitate
            
            // Extragem datele relaționale (ținând cont de diferențele litere mari/mici între C# și Vue)
            let clientObj = factura.clientFactura || factura.ClientFactura;
            let produseArray = factura.produseFactura2 || factura.ProduseFactura2 || [];

            // Construim obiectul (payload-ul) fix în formatul pe care îl așteaptă C#-ul
            const facturaPentruCsharp = {
                nrFactura: factura.nrFactura,
                dataFactura: factura.dataFactura,
                total: factura.total,
                estePlatita: noulStatus,
                ClientFactura: {
                    IdClient: clientObj.idClient || clientObj.IdClient,
                    NumeClient: clientObj.numeClient || clientObj.NumeClient,
                    Adresa: clientObj.adresa || clientObj.Adresa,
                    Telefon: clientObj.telefon || clientObj.Telefon
                },
                ProduseFactura2: produseArray.map(p => ({
                    Cantitate: p.cantitate || p.Cantitate,
                    Pret: p.pret || p.Pret,
                    IdProdus: p.idProdus || p.IdProdus
                }))
            };

            // Trimitem cererea PUT către API pentru a actualiza factura în baza de date
            fetch(`${apiFacturaUrl}/UpdateFactura?nrFactura=${factura.nrFactura}`, {
                method: 'PUT',
                headers: { 
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}` // Autentificăm cererea
                },
                body: JSON.stringify(facturaPentruCsharp)
            }).then(async raspuns => {
                if (raspuns.ok) {
                    Swal.fire('Succes!', 'Statusul plății și stocurile au fost actualizate!', 'success');
                    // Aici NU mai facem incarcaFacturi() pentru că SignalR ne va anunța oricum și va reîncărca tabelul automat
                } else {
                    const errText = await raspuns.text();
                    Swal.fire('Eroare la stoc!', errText || 'Nu s-a putut schimba statusul.', 'error');
                }
            }).catch(() => Swal.fire('Eroare!', 'Problemă de conexiune.', 'error'));
        }
    });
}

// --- LOGICĂ DIALOG (Deschidere pentru Editare) ---
function editFactura(factura) {
    // Curățăm formatul datei (păstrăm doar anul-luna-ziua, eliminăm ora 'T00:00:00')
    let dataFormatata = factura.dataFactura || factura.DataFactura || "";
    if (dataFormatata && dataFormatata.includes('T')) {
        dataFormatata = dataFormatata.split('T')[0];
    }

    // Extragem ID-ul clientului stabilind fallback-uri pentru siguranță
    let clientId = "";
    let clientObj = factura.clientFactura || factura.ClientFactura;
    if (clientObj) {
        clientId = clientObj.idClient || clientObj.IdClient;
    }

    // Curățăm lista de produse pentru a fi ușor de citit de componenta 'FacturaDialog'
    let produseArray = factura.produseFactura2 || factura.ProduseFactura2 || [];
    let produseMapate = produseArray.map(p => {
        let numeProdus = "Produs selectat"; 
        if (p.produs && p.produs.numeProdus) numeProdus = p.produs.numeProdus;
        else if (p.Produs && p.Produs.NumeProdus) numeProdus = p.Produs.NumeProdus;

        return {
            idProdus: p.idProdus || p.IdProdus,
            cantitate: p.cantitate || p.Cantitate,
            pret: p.pret || p.Pret,
            nume: numeProdus
        };
    });

    // Populăm variabila reactivă cu datele procesate
    facturaSelectata.value = {
        nrFactura: factura.nrFactura || factura.NrFactura,
        dataFactura: dataFormatata,
        total: factura.total || factura.Total,
        estePlatita: factura.estePlatita || factura.EstePlatita || false, 
        idClient: clientId,
        produse: produseMapate
    };
    
    // Afișăm fereastra modală
    showDialog.value = true;
}

// --- LOGICĂ DIALOG (Deschidere pentru Adăugare Nouă) ---
function adaugaFactura(){
    // Pregătim un obiect "gol" cu data de azi pentru o factură nouă (ID 0)
    facturaSelectata.value = { 
        nrFactura: 0, 
        dataFactura: new Date().toISOString().split('T')[0], 
        total: 0, 
        estePlatita: false, 
        idClient: "", 
        produse: [] 
    }
    showDialog.value = true
}

// --- LOGICĂ GENERARE PDF ---
function genereazaPDF(factura) {
    // 1. Extragem și mapăm datele clientului (evităm null)
    let clientNume = "Client Necunoscut";
    let clientAdresa = "";
    let clientTelefon = "";
    
    let clientObj = factura.clientFactura || factura.ClientFactura;
    if (clientObj) {
        clientNume = clientObj.numeClient || clientObj.NumeClient;
        clientAdresa = clientObj.adresa || clientObj.Adresa;
        clientTelefon = clientObj.telefon || clientObj.Telefon;
    }

    // 2. Extragem și mapăm produsele exact cum are nevoie tabelul HTML din PDF
    let produseArray = factura.produseFactura2 || factura.ProduseFactura2 || [];
    let produseMapate = produseArray.map(p => {
        let numeProdus = "Produs sters/necunoscut"; 
        if (p.produs && p.produs.numeProdus) numeProdus = p.produs.numeProdus;
        else if (p.Produs && p.Produs.NumeProdus) numeProdus = p.Produs.NumeProdus;

        return {
            cantitate: p.cantitate || p.Cantitate,
            pret: p.pret || p.Pret,
            nume: numeProdus
        };
    });

    // 3. Pasăm datele finale către variabila reactivă legată de div-ul '#factura-print-template'
    facturaPentruPDF.value = {
        nrFactura: factura.nrFactura,
        dataFactura: factura.dataFactura,
        total: factura.total,
        estePlatita: factura.estePlatita,
        clientNume: clientNume,
        clientAdresa: clientAdresa,
        clientTelefon: clientTelefon,
        produse: produseMapate
    };

    // 4. Așteptăm 100 milisecunde ca Vue să introducă efectiv variabilele de mai sus în HTML-ul ascuns.
    // Fără setTimeout, html2pdf ar face captura înainte ca textul nou să apară pe ecran.
    setTimeout(() => {
        const element = document.getElementById('factura-print-template');
        const optiuni = {
            margin:       15,
            filename:     `Factura_Nr_${factura.nrFactura}.pdf`,
            image:        { type: 'jpeg', quality: 0.98 },
            html2canvas:  { scale: 2 }, // Dublăm rezoluția de captură pentru a nu fi blurat textul
            jsPDF:        { unit: 'mm', format: 'a4', orientation: 'portrait' }
        };
        
        // Comanda de convertire și declanșare a descărcării în browser
        html2pdf().set(optiuni).from(element).save();
    }, 100);
}

// --- LOGICĂ SALVARE/ȘTERGERE API ---
// Funcția este apelată (emit) din componenta fiu FacturaDialog.vue când apeși pe "Salvează"
function salveazaFacturaInAPI(facturaRealaDinDialog) {
    // Căutăm obiectul complet al clientului pe baza ID-ului selectat din dropdown
    const clientComplet = lista_clienti.value.find(c => c.idClient === facturaRealaDinDialog.idClient);

    // Reconstruim payload-ul exact în forma cerută de backend (PascalCase)
    const facturaPentruCsharp = {
        nrFactura: facturaRealaDinDialog.nrFactura,
        dataFactura: facturaRealaDinDialog.dataFactura,
        total: facturaRealaDinDialog.total,
        estePlatita: facturaRealaDinDialog.estePlatita, 
        ClientFactura: {
            IdClient: clientComplet.idClient,
            NumeClient: clientComplet.NumeClient || clientComplet.numeClient,
            Adresa: clientComplet.Adresa || clientComplet.adresa,
            Telefon: clientComplet.Telefon || clientComplet.telefon
        },
        ProduseFactura2: facturaRealaDinDialog.produse.map(p => ({
            Cantitate: p.cantitate,
            Pret: p.pret,
            IdProdus: p.idProdus
        }))
    };

    // Dacă ID-ul este 0 înseamnă că e o factură nouă (POST), altfel e o actualizare (PUT)
    const metoda = facturaPentruCsharp.nrFactura === 0 ? 'POST' : 'PUT';
    const url = facturaPentruCsharp.nrFactura === 0 
        ? `${apiFacturaUrl}/AdaugaFactura` 
        : `${apiFacturaUrl}/UpdateFactura?nrFactura=${facturaPentruCsharp.nrFactura}`;
    
    const token = localStorage.getItem('token_jwt');

    fetch(url, {
        method: metoda,
        headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify(facturaPentruCsharp)
    }).then(async raspuns => {
        if (raspuns.ok) {
            showDialog.value = false; // Închidem modalul dacă operațiunea a reușit
            Swal.fire('Succes!', 'Factura a fost salvată.', 'success');
        } else {
            const textEroare = await raspuns.text();
            console.error("Eroare de la C#:", textEroare);
            Swal.fire('Eroare!', textEroare || 'A apărut o eroare la salvare.', 'error');
        }
    }).catch(error => {
        console.error('Eroare la salvare factura:', error);
        Swal.fire('Eroare!', 'Eroare de conexiune la salvare.', 'error');
    });
}

// --- ÎNCĂRCARE DATE INIȚIALE ---
// Cere lista completă de facturi de la server și populează variabila reactivă
async function incarcaFacturi() {
    const token = localStorage.getItem('token_jwt');
    try {
        const raspuns = await fetch(`${apiFacturaUrl}/GetFacturi`, {
            headers: { 'Authorization': `Bearer ${token}` }
        })
        if (raspuns.ok) {
            lista_facturi.value = await raspuns.json()
        }
    } catch (e) { 
        console.error('Eroare încărcare facturi', e) 
    }
}

// Execută cererea DELETE către API pentru a elimina factura selectată din baza de date
function stergeFactura(factura){
    const token = localStorage.getItem('token_jwt');

    Swal.fire({
        title: 'Ești sigur?',
        text: `Ștergi factura nr ${factura.nrFactura}? Produsele vor fi returnate în stoc dacă factura e plătită.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e53e3e',
        cancelButtonColor: '#a0aec0',
        confirmButtonText: 'Da, șterge!',
        cancelButtonText: 'Nu, anulează'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`${apiFacturaUrl}/DeleteFactura?nrFactura=${factura.nrFactura}`, { 
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${token}` }
            })
            .then(raspuns => {
                if (raspuns.ok) {
                    Swal.fire('Șters!', 'Factura a fost ștearsă.', 'success')
                } else { 
                    Swal.fire('Eroare!', 'Eroare la ștergerea facturii.', 'error') 
                }
            }).catch(error => {
                console.error('Eroare stergere:', error)
                Swal.fire('Eroare!', 'A apărut o problemă de conexiune.', 'error')
            })
        }
    })
}

// ==========================================
// CICLUL DE VIAȚĂ (LIFECYCLE HOOKS)
// ==========================================

// 'onMounted' se execută o singură dată, imediat după ce Vue a randat componenta în browser
onMounted(async () => {
    const token = localStorage.getItem('token_jwt');
    
    // 1. Aducem listele necesare din baza de date la pornirea paginii
    await incarcaFacturi() // Populează tabelul central

    // Descărcăm clienții (necesari pentru dropdown-ul din modalul de adăugare factură)
    try {
        const raspC = await fetch(`${apiClientUrl}/GetClienti`, {
            headers: { 'Authorization': `Bearer ${token}` }
        })
        if(raspC.ok) lista_clienti.value = await raspC.json()
    } catch(e) { console.error('Nu s-au putut încărca clienții', e) }

    // Descărcăm catalogul de produse (necesar tot pentru selecția din modal)
    try {
        const raspP = await fetch(`${apiProdusUrl}/GetProdusModels`, {
            headers: { 'Authorization': `Bearer ${token}` }
        })
        if (raspP.ok) {
            const dateProduse = await raspP.json()
            // Mapăm direct proprietățile pentru a evita problemele de sintaxă în dropdown
            lista_produse.value = dateProduse.map(p => ({
                id: p.idProdus || p.IdProdus,
                nume: p.numeProdus || p.NumeProdus,
                pret: p.pret || p.Pret
            }))
        }
    } catch(e) { 
        console.error('Nu s-au putut încărca produsele', e) 
    }

    // 2. Setăm țeava de comunicare în timp real via SignalR (WebSockets)
    connectionFacturi = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5116/facturiHub")
        .withAutomaticReconnect() // Încearcă să se reconecteze singur dacă pică netul
        .build();

    // Spunem aplicației ce să facă atunci când C#-ul strigă "UpdateFacturi" prin megafon
    connectionFacturi.on("UpdateFacturi", () => {
        console.log("🔄 Actualizare live primită la facturi! Reîncărcăm tabelul...");
        incarcaFacturi(); // Reapelează API-ul pentru a aduce lista actualizată pe ecran automat
    });

    // Pornim efectiv conexiunea ascultătoare
    try {
        await connectionFacturi.start();
        console.log("✅ Conectat la FacturiHub (SignalR)!");
    } catch (error) {
        console.error("❌ Eroare conectare SignalR Facturi:", error);
    }
})

// 'onUnmounted' se execută strict în momentul în care părăsești această pagină (Navighezi pe Altă Pagină).
// Este crucial pentru a preveni "Memory Leaks" (Ocuparea abuzivă a resurselor RAM ale browserului).
onUnmounted(() => {
    if (connectionFacturi) {
        connectionFacturi.stop(); // Închide politicos conexiunea WebSockets
        console.log("🛑 Conexiunea SignalR pentru Facturi a fost închisă.");
    }
})
</script>

<style scoped>
/* Toate stilurile CSS rămân exact la fel ca înainte */
.page-container { padding: 20px; padding-bottom: 50px; }
.action-menu { padding: 20px; background-color: white; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.05); display: flex; justify-content: space-between; align-items: center;}
.action-menu h2 { color: #2d3748; margin: 0; font-size: 1.2rem; }
.search-bar-container { position: relative; margin-bottom: 20px; width: 100%; max-width: 400px; }
.search-icon { position: absolute; left: 15px; top: 50%; transform: translateY(-50%); color: #a0aec0; }
.search-input { width: 100%; padding: 12px 15px 12px 40px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 0.95rem; outline: none; transition: 0.2s; box-sizing: border-box; }
.search-input:focus { border-color: #4299e1; box-shadow: 0 0 0 3px rgba(66, 153, 225, 0.15); }
.badge { padding: 4px 10px; border-radius: 20px; font-size: 0.85rem; font-weight: bold; display: inline-block; }
.badge-success { background-color: #e6fffa; color: #234e52; border: 1px solid #b2f5ea; }
.badge-muted { background-color: #edf2f7; color: #4a5568; border: 1px solid #e2e8f0; }
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
.actions { display: flex; gap: 10px; }
.btn-icon { border: none; background: none; cursor: pointer; padding: 8px; border-radius: 5px; font-size: 1rem; display: flex; align-items: center; gap: 5px; transition: 0.2s; font-weight: bold;}
.btn-edit { color: #4299e1; }
.btn-edit:hover { background-color: #ebf8ff; color: #2b6cb0; }
.btn-delete { color: #e53e3e; }
.btn-delete:hover { background-color: #fff5f5; color: #c53030; }
.btn-pdf { color: #805ad5; }
.btn-pdf:hover { background-color: #faf5ff; color: #6b46c1; }
.factura-pdf { font-family: 'Arial', sans-serif; color: #2d3748; padding: 10px; background: white; }
.factura-header { display: table; width: 100%; margin-bottom: 40px; padding-bottom: 20px; border-bottom: 2px solid #e2e8f0; }
.companie-info { display: table-cell; width: 50%; }
.factura-detalii { display: table-cell; width: 50%; text-align: right; vertical-align: top; }
.factura-detalii h1 { margin: 0 0 10px 0; color: #3e1f6c; }
.factura-client { margin-bottom: 40px; background: #f7fafc; padding: 15px; border-radius: 8px; }
.factura-client h3, .companie-info h3 { margin: 0 0 8px 0; color: #718096; font-size: 0.9rem; letter-spacing: 1px; }
.factura-produse { width: 100%; border-collapse: collapse; margin-bottom: 40px; }
.factura-produse th { background-color: #f7fafc; color: #4a5568; font-weight: bold; border-bottom: 2px solid #e2e8f0; }
.factura-produse th, .factura-produse td { padding: 12px; text-align: left; border-bottom: 1px solid #edf2f7; }
.factura-total { display: table; width: 100%; }
.total-box { display: table-cell; text-align: right; width: 100%; }
.total-box h2 { color: #3e1f6c; margin-top: 10px; }
.stampila-pdf {
    position: absolute;
    top: 10px;
    left: 35%;
    font-size: 2.2rem;
    font-weight: 900;
    border: 5px solid;
    padding: 10px 30px;
    border-radius: 12px;
    transform: rotate(-15deg);
    opacity: 0.35;
    letter-spacing: 4px;
    z-index: 10;
}
.stampila-platit { color: #38a169; border-color: #38a169; }
.stampila-neplatit { color: #e53e3e; border-color: #e53e3e; }
</style>