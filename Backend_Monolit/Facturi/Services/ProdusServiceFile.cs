namespace Facturi.Services;

public class ProdusServiceFile : IProdus
{
    private const string FilePath = "produs.txt";
    private readonly ILogger<ProdusServiceFile> _logger;
    public ProdusServiceFile(ILogger<ProdusServiceFile> logger)
    {
        _logger = logger;

        if (!System.IO.File.Exists(FilePath))
        {
            System.IO.File.WriteAllText(FilePath, "[]");
        }
    }

    public List<ProdusModel> GetProdusModels()
    {
        _logger.LogInformation($"Fetching all product models: {DateTime.UtcNow}");
        if (System.IO.File.Exists(FilePath))
        {
            string produsJson = System.IO.File.ReadAllText(FilePath);
            var listaProduse = System.Text.Json.JsonSerializer.Deserialize<List<ProdusModel>>(produsJson);
            return listaProduse ?? new List<ProdusModel>();
        }
        return new List<ProdusModel>();
    }

    public ProdusModel GetProdusModelById(int idProdus)
    {
        _logger.LogInformation($"Fetching product model by ID: {idProdus} at {DateTime.UtcNow}");
        var listaProduse = GetProdusModels();

        return listaProduse.FirstOrDefault(p => p.IdProdus == idProdus);
    }

    public List<ProdusModel> GetProdusModelsByNume(string nume)
    {
        _logger.LogInformation("Fetching product models by name.");
        var listaProduse = GetProdusModels();

        return listaProduse.Where(p => p.NumeProdus.Equals(nume, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public void AddProdus(ProdusModel produs)
    {
        _logger.LogInformation("Adding new product model.");
        var produse = GetProdusModels();
        produse.Add(produs);
        string produsJson = System.Text.Json.JsonSerializer.Serialize(produse);
        System.IO.File.WriteAllText(FilePath, produsJson);
    }

    public void UpdateProdus(int idProdus, ProdusModel produs)
    {
        _logger.LogInformation("Updating product model.");
        var produse = GetProdusModels();
        var index = produse.FindIndex(p => p.IdProdus == idProdus);
        if (index != -1)
        {
            produse[index] = produs;
            string produsJson = System.Text.Json.JsonSerializer.Serialize(produse);
            System.IO.File.WriteAllText(FilePath, produsJson);
        }
        else
        {
            _logger.LogWarning($"Product with ID {idProdus} not found for update.");
        }

    }

    public void DeleteProdus(int id)
    {
        _logger.LogInformation("Deleting product model.");
        var produse = GetProdusModels();
        var index = produse.FindIndex(p => p.IdProdus == id);
        if (index != -1)
        {
            produse.RemoveAt(index);
            string produsJson = System.Text.Json.JsonSerializer.Serialize(produse);
            System.IO.File.WriteAllText(FilePath, produsJson);
        }
    }
}
