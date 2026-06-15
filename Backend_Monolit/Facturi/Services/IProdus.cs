namespace Facturi.Services;
public interface IProdus
{
    List<ProdusModel> GetProdusModels();
    List<ProdusModel> GetProdusModelsByNume(string nume);
    ProdusModel GetProdusModelById(int id);
    void AddProdus(ProdusModel produs);
    void UpdateProdus(int id, ProdusModel produs);
    void DeleteProdus(int id);   
}