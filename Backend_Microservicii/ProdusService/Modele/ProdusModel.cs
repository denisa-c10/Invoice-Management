using System.ComponentModel.DataAnnotations;
namespace ProdusService.Modele;

public class ProdusModel
{
    [Key]
    public int IdProdus { get; set; }
    public string NumeProdus { get; set; } = string.Empty;
    public decimal Pret { get; set; }
}