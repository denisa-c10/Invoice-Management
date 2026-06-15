namespace Facturi.Services;
public interface IClient
{
    List<ClientModel> GetClienti();
    ClientModel GetClientById(int id);
    List<ClientModel> GetClientiByNume(string nume);
    void AddClient(ClientModel client);
    void UpdateClient(int id, ClientModel client);
    void DeleteClient(int id);
}
