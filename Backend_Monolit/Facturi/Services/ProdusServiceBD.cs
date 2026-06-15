namespace Facturi.Services;

public class ProdusServiceBD : IProdus
{
    private readonly AppDbContext _context;
    public ProdusServiceBD(AppDbContext context)
    {
        //initializare conexiune la baza de date
        _context = context;
    }

    public void AddProdus(ProdusModel produs)
    {
        _context.Produse.Add(produs); //insert into Produse values (...)
        _context.SaveChanges(); //commit la tranzactie
    }

    public void DeleteProdus(int id)
    {
        var produs = _context.Produse.FirstOrDefault(p => p.IdProdus == id);
        var existaProdusInFactura = _context.ProduseFactura.Any(pf => pf.IdProdus == id);
        if (produs != null)
        {
            if (!existaProdusInFactura)
            {
                _context.Produse.Remove(produs);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Produsul nu poate fi sters deoarece exista in factura");
            }
        }
    }

    public ProdusModel GetProdusModelById(int id)
    {
        //select * from Produse where IdProdus = id
        var unProdus = _context.Produse.FirstOrDefault(p => p.IdProdus == id);
        return unProdus;
    }

    public List<ProdusModel> GetProdusModels()
    {
         return _context.Produse.ToList(); //select * from Produse
    }

    public List<ProdusModel> GetProdusModelsByNume(string nume)
    {
        //select * from Produse where Nume like '%nume%'
        var listaProduse = _context.Produse.Where(p => p.NumeProdus.Contains(nume)).ToList();
        return listaProduse;
    }

    public void UpdateProdus(int id, ProdusModel produs)
    {
        var produsExistent = _context.Produse.FirstOrDefault(p => p.IdProdus == id);
        if (produsExistent != null)
        {
            produsExistent.NumeProdus = produs.NumeProdus;
            produsExistent.Pret = produs.Pret;
            _context.SaveChanges();
        }
    }
}