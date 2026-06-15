using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace FacturaService.Modele;

public class ProdusFacturaModel
{
    [Key]
    public int IdProdusFactura { get; set; }
    public int Cantitate { get; set; }
    public decimal Pret { get; set; }

    public int NrFactura { get; set; }


    public int IdProdus { get; set; }

    // Navigare înapoi către Factură (pentru Entity Framework)
    [JsonIgnore]
    public FacturaModel? Factura { get; set; }
}