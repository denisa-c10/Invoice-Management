using System.ComponentModel.DataAnnotations;
namespace Facturi;

public class FacturaModel2
{
    [Key]
    public int NrFactura { get; set; }
    public DateTime DataFactura { get; set; }

    public ClientModel ClientFactura { get; set; }
    public List<ProdusFacturaModel2> ProduseFactura2 { get; set; } = new();

    public decimal Total { get; set; }

    // Proprietate pentru a urmări dacă factura a fost achitată
    public bool EstePlatita { get; set; } = false;
}
