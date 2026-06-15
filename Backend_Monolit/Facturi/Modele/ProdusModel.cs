using System.ComponentModel.DataAnnotations;
namespace Facturi;

public class ProdusModel
{
    [Key]
    public int IdProdus { get; set; }

    public string NumeProdus { get; set; }

    public decimal Pret { get; set; }
    public int Cantitate { get; set; }
}