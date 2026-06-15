namespace Facturi.Services;

public class ClientServiceBD : IClient
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClientServiceBD> _logger;
    public ClientServiceBD(AppDbContext context, ILogger<ClientServiceBD> logger)
    {
        //initializare conexiune la baza de date
        _context = context;
        _logger = logger;
    }

    public void AddClient(ClientModel client)
    {
        try
        {
            _context.Clienti.Add(client); //insert into Clienti values (...)
            _context.SaveChanges(); //commit la tranzactie

            _logger.LogInformation("Adaugare client in baza de date: {@Client}", System.Text.Json.JsonSerializer.Serialize(client));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la adaugarea clientului in baza de date: {@Client}", System.Text.Json.JsonSerializer.Serialize(client));
            throw; //rethrow pentru a putea fi tratata la un nivel superior
        }
    }

    public void DeleteClient(int id)
    {
        var client = _context.Clienti.FirstOrDefault(c => c.IdClient == id);
        if (client != null)
        {
            _context.Clienti.Remove(client);
            _context.SaveChanges();
        }
    }

    public ClientModel GetClientById(int id)
    {
        //select * from Clienti where IdClient = id
        var unClient = _context.Clienti.FirstOrDefault(c => c.IdClient == id);
        return unClient;
    }

    //implementare folosind baza de date
    public List<ClientModel> GetClienti()
    {
        return _context.Clienti.ToList(); //select * from Clienti
    }

    public List<ClientModel> GetClientiByNume(string nume)
    {
        //select * from Clienti where Nume like '%nume%'
        var listaClienti = _context.Clienti.Where(c => c.NumeClient.Contains(nume)).ToList();
        return listaClienti;
    }

    public void UpdateClient(int id, ClientModel client)
    {
        var existingClient = _context.Clienti.FirstOrDefault(c => c.IdClient == id);
        if (existingClient != null)
        {
            // v1: Actualizează proprietățile clientului existent cu valorile din clientul primit
            existingClient.NumeClient = client.NumeClient;
            existingClient.Adresa = client.Adresa;
            existingClient.Telefon = client.Telefon;

            //v2: Folosind Entity Framework pentru a marca entitatea ca modificată
            //_context.Entry(existingClient).CurrentValues.SetValues(client);

            _context.SaveChanges();
        }
    }
}