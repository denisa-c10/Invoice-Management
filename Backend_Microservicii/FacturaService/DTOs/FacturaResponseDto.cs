namespace FacturaService.DTOs;

public class FacturaResponseDto
{
    public int NrFactura { get; set; }
    public DateTime DataFactura { get; set; }
    public ClientDto? Client { get; set; } // Aici vom pune datele aduse prin HTTP
    // (Am putea adăuga și produsele aici, dar le lăsăm momentan pentru simplitate)
}