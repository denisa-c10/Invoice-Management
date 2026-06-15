using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facturi.Services;

public class FacturaServiceBD : IFactura
{
    private readonly AppDbContext _context;
    public FacturaServiceBD(AppDbContext context)
    {
        //initializare conexiune la baza de date
        _context = context;
    }

    public void AddFactura(FacturaModel2 factura)
    {
        // Preluam datele corecte pentru Client. Nu adaugam un client nou!
        if (factura.ClientFactura != null && factura.ClientFactura.IdClient > 0)
        {
            var clientExistent = _context.Clienti.FirstOrDefault(c => c.IdClient == factura.ClientFactura.IdClient);
            if (clientExistent != null)
            {
                // Asiguram ca legam de instanta corecta din DB
                factura.ClientFactura = clientExistent;
            }
        }

        // 3. Variabila pentru a strânge/calcula Totalul
        decimal totalFactura = 0;

        // 1. Parcurgem lista ProduseFactura2
        if (factura.ProduseFactura2 != null && factura.ProduseFactura2.Count > 0)
        {
            foreach (var linie in factura.ProduseFactura2)
            {
                // Resetam identitatea liniei ca EF sa o trateze clar ca un produs_factura PENTRU FACTURA CURENTA, proaspat adaugat
                linie.IdProdusFactura = 0;

                // Nu cream produse noi - legam pe IdProdus
                var produsExistent = _context.Produse.FirstOrDefault(p => p.IdProdus == linie.IdProdus);
                if (produsExistent != null)
                {
                    // Tragem pretul la zi al produsului si evitam inserarea unuia nou asociind instanta existenta
                    linie.Produs = produsExistent;
                    linie.Pret = produsExistent.Pret;

                    // 2. Calculeaza subtotal = Cantitate * Pret
                    decimal subtotal = linie.Cantitate * linie.Pret;

                    // 3. Aduna subtotalul la Totalul facturii
                    totalFactura += subtotal;

                    // --- SCĂDEREA STOCULUI LA ADĂUGAREA FACTURII (Dacă factura e plătită) ---
                    if (factura.EstePlatita)
                    {
                        if (produsExistent.Cantitate < linie.Cantitate)
                        {
                            throw new InvalidOperationException($"Stoc insuficient pentru produsul '{produsExistent.NumeProdus}'. Stoc actual: {produsExistent.Cantitate}. Solicitat: {linie.Cantitate}");
                        }
                        produsExistent.Cantitate -= linie.Cantitate;
                    }
                }
            }
        }

        // Atribuim totalul fizic proprietatii din model
        factura.Total = totalFactura;

        // 4. Salvare in Baza de Date folosind numele corect: Facturi
        _context.Facturi.Add(factura);
        _context.SaveChanges();
    }

    public void DeleteFactura(int nrFactura)
    {
        var factura = _context.Facturi
            .Include(f => f.ProduseFactura2)
            .FirstOrDefault(f => f.NrFactura == nrFactura);

        if (factura != null)
        {
            // --- RETURNARE STOC LA ȘTERGEREA FACTURII (Dacă a fost plătită) ---
            if (factura.EstePlatita)
            {
                foreach (var linie in factura.ProduseFactura2)
                {
                    var produsExistent = _context.Produse.FirstOrDefault(p => p.IdProdus == linie.IdProdus);
                    if (produsExistent != null)
                    {
                        produsExistent.Cantitate += linie.Cantitate;
                    }
                }
            }

            _context.Facturi.Remove(factura);
            _context.SaveChanges();
        }
    }

    public void DeleteProdusDinFactura(int nrFactura, int idProdus)
    {
        var factura = _context.Facturi.FirstOrDefault(f => f.NrFactura == nrFactura);
        if (factura != null)
        {
            var produsDeSters = factura.ProduseFactura2.FirstOrDefault(p => p.IdProdus == idProdus);
            if (produsDeSters != null)
            {
                if (factura.EstePlatita)
                {
                    var produsExistent = _context.Produse.FirstOrDefault(p => p.IdProdus == idProdus);
                    if (produsExistent != null)
                    {
                        produsExistent.Cantitate += produsDeSters.Cantitate;
                    }
                }

                factura.ProduseFactura2.Remove(produsDeSters);
                _context.SaveChanges();
            }
        }
    }

    public FacturaModel2 GetFacturaByNr(int nrFactura)
    {
        var oFactura = _context.Facturi
            .Include(f => f.ProduseFactura2)
            .ThenInclude(pf => pf.Produs)
                .Include(f => f.ClientFactura)
            .FirstOrDefault(f => f.NrFactura == nrFactura);
        return oFactura;
    }

    public List<FacturaModel2> GetFacturi()
    {
        return _context.Facturi
            .Include(f => f.ClientFactura)
            .Include(f => f.ProduseFactura2)
                .ThenInclude(pf => pf.Produs)
            .ToList();
    }

    public List<FacturaModel2> GetFacturiByClient(int idClient)
    {
        var listaFacturi = _context.Facturi
             .Include(f => f.ClientFactura)
             .Include(f => f.ProduseFactura2)
                 .ThenInclude(pf => pf.Produs)
            .Where(f => f.ClientFactura.IdClient == idClient).ToList();
        return listaFacturi;
    }

    public void UpdateFactura(int nrFactura, FacturaModel2 facturaModificata)
    {
        var facturaDB = _context.Facturi
            .Include(f => f.ProduseFactura2)
            .Include(f => f.ClientFactura)
            .FirstOrDefault(f => f.NrFactura == nrFactura);

        if (facturaDB == null) return;

        facturaDB.DataFactura = facturaModificata.DataFactura;

        // --- ACTUALIZARE STATUS PLĂTITĂ ȘI STOCURI ---

        // Cazul 1: Factura devine platita
        if (!facturaDB.EstePlatita && facturaModificata.EstePlatita)
        {
            foreach (var linieNoua in facturaModificata.ProduseFactura2)
            {
                var produsPeRaft = _context.Produse.Find(linieNoua.IdProdus);
                if (produsPeRaft != null)
                {
                    if (produsPeRaft.Cantitate < linieNoua.Cantitate)
                    {
                        throw new InvalidOperationException($"Stoc insuficient pentru '{produsPeRaft.NumeProdus}'.");
                    }
                    produsPeRaft.Cantitate -= linieNoua.Cantitate;
                }
            }
        }
        // Cazul 2: Factura devine NEplatita
        else if (facturaDB.EstePlatita && !facturaModificata.EstePlatita)
        {
            foreach (var linieVeche in facturaDB.ProduseFactura2)
            {
                var produsPeRaft = _context.Produse.Find(linieVeche.IdProdus);
                if (produsPeRaft != null)
                {
                    produsPeRaft.Cantitate += linieVeche.Cantitate;
                }
            }
        }
        // Cazul 3: Factura era DEJA PLATITA si se modifica Liniile facturii
        else if (facturaDB.EstePlatita && facturaModificata.EstePlatita)
        {
            foreach (var linieVeche in facturaDB.ProduseFactura2)
            {
                var produsPeRaft = _context.Produse.Find(linieVeche.IdProdus);
                if (produsPeRaft != null)
                {
                    produsPeRaft.Cantitate += linieVeche.Cantitate;
                }
            }

            foreach (var linieNoua in facturaModificata.ProduseFactura2)
            {
                var produsPeRaft = _context.Produse.Find(linieNoua.IdProdus);
                if (produsPeRaft != null)
                {
                    if (produsPeRaft.Cantitate < linieNoua.Cantitate)
                    {
                        throw new InvalidOperationException($"Stoc insuficient pentru '{produsPeRaft.NumeProdus}'.");
                    }
                    produsPeRaft.Cantitate -= linieNoua.Cantitate;
                }
            }
        }

        facturaDB.EstePlatita = facturaModificata.EstePlatita;
        facturaDB.Total = facturaModificata.Total;

        if (facturaDB.ClientFactura.IdClient != facturaModificata.ClientFactura.IdClient)
        {
            var noulClient = _context.Clienti.Find(facturaModificata.ClientFactura.IdClient);
            if (noulClient != null)
            {
                facturaDB.ClientFactura = noulClient;
            }
        }

        _context.ProduseFactura.RemoveRange(facturaDB.ProduseFactura2);

        foreach (var pf in facturaModificata.ProduseFactura2)
        {
            pf.IdProdusFactura = 0;
            pf.Produs = null;
            pf.Factura = null;
        }

        facturaDB.ProduseFactura2 = facturaModificata.ProduseFactura2;

        _context.SaveChanges();
    }
}