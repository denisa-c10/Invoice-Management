namespace Facturi.Services;

public class FacturaServiceFile : IFactura
{
    private const string FilePath = "factura.txt";

    private readonly ILogger<FacturaServiceFile> _logger;
    public FacturaServiceFile(ILogger<FacturaServiceFile> logger)
    {
        _logger = logger;
        if (!System.IO.File.Exists(FilePath))
        {
            System.IO.File.WriteAllText(FilePath, "[]");
        }
    }

    public List<FacturaModel2> GetFacturi()
    {
        if (System.IO.File.Exists(FilePath))
        {
            string facturaJson = System.IO.File.ReadAllText(FilePath);
            var listaFacturi = System.Text.Json.JsonSerializer.Deserialize<List<FacturaModel2>>(facturaJson);
            return listaFacturi ?? new List<FacturaModel2>();
        }
        return new List<FacturaModel2>();
    }

    public FacturaModel2 GetFacturaByNr(int nrFactura)
    {
        if (System.IO.File.Exists(FilePath))
        {
            string facturaJson = System.IO.File.ReadAllText(FilePath);
            var listaFacturi = System.Text.Json.JsonSerializer.Deserialize<List<FacturaModel2>>(facturaJson);
            if (listaFacturi != null)
            {
                return listaFacturi.FirstOrDefault(f => f.NrFactura == nrFactura);
            }
        }
        return null;
    }

    public List<FacturaModel2> GetFacturiByClient(int idClient)
    {
        if (System.IO.File.Exists(FilePath))
        {
            string facturaJson = System.IO.File.ReadAllText(FilePath);
            var listaFacturi = System.Text.Json.JsonSerializer.Deserialize<List<FacturaModel2>>(facturaJson);
            if (listaFacturi != null)
            {
                return listaFacturi.Where(f => f.ClientFactura.IdClient == idClient).ToList();
            }
        }
        return new List<FacturaModel2>();
    }

    public void AddFactura(FacturaModel2 factura)
    {
        var facturi = GetFacturi();
        facturi.Add(factura);
        string facturaJson = System.Text.Json.JsonSerializer.Serialize(facturi);
        System.IO.File.WriteAllText(FilePath, facturaJson);
    }

    public void UpdateFactura(int nrFactura, FacturaModel2 factura)
    {
        var facturi = GetFacturi();
        var index = facturi.FindIndex(f => f.NrFactura == nrFactura);
        if (index != -1)
        {
            facturi[index] = factura;
            string facturaJson = System.Text.Json.JsonSerializer.Serialize(facturi);
            System.IO.File.WriteAllText(FilePath, facturaJson);
        }
    }

    public void DeleteFactura(int nrFactura)
    {
        var facturi = GetFacturi();
        var index = facturi.FindIndex(f => f.NrFactura == nrFactura);
        if (index != -1)
        {
            facturi.RemoveAt(index);
            string facturaJson = System.Text.Json.JsonSerializer.Serialize(facturi);
            System.IO.File.WriteAllText(FilePath, facturaJson);
        }
    }

    public void DeleteProdusDinFactura(int nrFactura, int idProdusFactura)
    {
        var facturi = GetFacturi();
        var factura = facturi.FirstOrDefault(f => f.NrFactura == nrFactura);
        if (factura != null)
        {
            var produsDeSters = factura.ProduseFactura2.FirstOrDefault(p => p.IdProdus == idProdusFactura);
            if (produsDeSters != null)
            {
                factura.ProduseFactura2.Remove(produsDeSters);
                string facturaJsonNou = System.Text.Json.JsonSerializer.Serialize(facturi);
                System.IO.File.WriteAllText(FilePath, facturaJsonNou);
            }
        }
    }
}