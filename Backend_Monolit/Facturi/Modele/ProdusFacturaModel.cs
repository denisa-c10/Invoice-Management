using System.ComponentModel.DataAnnotations;
namespace Facturi;

//v2 cu Composition in loc de Inheritance
public class ProdusFacturaModel2
{
    [Key]
    public int IdProdusFactura { get; set; }
    public int Cantitate { get; set; }
    public decimal Pret { get; set; }

    public int NrFactura { get; set; }    //asta va fi FK
    public int IdProdus { get; set; } //FK catre ProdusModel

    //prop de navigatie catre FacturaModel si ProdusModel
    [System.Text.Json.Serialization.JsonIgnore]
    public FacturaModel2? Factura { get; set; } //navigare inapoi catre factura
    public ProdusModel? Produs { get; set; } //navigare catre produsul de baza

    // Logica de business: calcul subtotal linie = cantitate * pret
    public decimal Subtotal => Cantitate * Pret;
}
