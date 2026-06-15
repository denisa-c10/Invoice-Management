using System.ComponentModel.DataAnnotations;

namespace FacturaService.Modele;

public class FacturaModel
{
    [Key]
    public int NrFactura { get; set; }
    public DateTime DataFactura { get; set; }

    public int IdClient { get; set; }

    // Relația cu rândurile facturii rămâne (sunt în aceeași bază de date)
    public List<ProdusFacturaModel> ProduseFactura { get; set; } = new();
}