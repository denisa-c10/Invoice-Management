namespace Facturi.Services;

public class ClientServiceFile : IClient
{
    private const string FilePath = "client.txt";

    private readonly ILogger<ClientServiceFile> _logger;
    public ClientServiceFile(ILogger<ClientServiceFile> logger)
    {
        _logger = logger;
        if (!System.IO.File.Exists(FilePath))
        {
            System.IO.File.WriteAllText(FilePath, "[]");
        }
    }

    public List<ClientModel> GetClienti()
    {
        if (System.IO.File.Exists(FilePath))
        {
            string clientJson = System.IO.File.ReadAllText(FilePath);
            var listaClienti = System.Text.Json.JsonSerializer.Deserialize<List<ClientModel>>(clientJson);
            return listaClienti ?? new List<ClientModel>();
        }
        return new List<ClientModel>();
    }

    public ClientModel GetClientById(int id)
    {
        if (System.IO.File.Exists(FilePath))
        {
            string clientJson = System.IO.File.ReadAllText(FilePath);
            var listaClienti = System.Text.Json.JsonSerializer.Deserialize<List<ClientModel>>(clientJson);
            if (listaClienti != null)
            {
                return listaClienti.FirstOrDefault(c => c.IdClient == id);
            }
        }
        return null;
    }

    public List<ClientModel> GetClientiByNume(string nume)
    {
        if (System.IO.File.Exists(FilePath))
        {
            string clientJson = System.IO.File.ReadAllText(FilePath);
            var listaClienti = System.Text.Json.JsonSerializer.Deserialize<List<ClientModel>>(clientJson);
            if (listaClienti != null)
            {
                return listaClienti.Where(c => c.NumeClient.Equals(nume, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
        return new List<ClientModel>();
    }

    public void AddClient(ClientModel client)
    {
        var clienti = GetClienti();
        clienti.Add(client);
        string clientJson = System.Text.Json.JsonSerializer.Serialize(clienti);
        System.IO.File.WriteAllText(FilePath, clientJson);
    }

    public void UpdateClient(int idClient, ClientModel client)
    {
        var clienti = GetClienti();
        var index = clienti.FindIndex(c => c.IdClient == idClient);
        if (index != -1)
        {
            clienti[index] = client;
            string clientJson = System.Text.Json.JsonSerializer.Serialize(clienti);
            System.IO.File.WriteAllText(FilePath, clientJson);
        }
    }

    public void DeleteClient(int idClient)
    {
        var clienti = GetClienti();
        var index = clienti.FindIndex(c => c.IdClient == idClient);
        if (index != -1)
        {
            clienti.RemoveAt(index);
            string clientJson = System.Text.Json.JsonSerializer.Serialize(clienti);
            System.IO.File.WriteAllText(FilePath, clientJson);
        }
    }
}
