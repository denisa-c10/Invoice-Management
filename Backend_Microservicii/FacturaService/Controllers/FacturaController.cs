using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FacturaService.Data;
using FacturaService.Modele;
using FacturaService.DTOs;

namespace FacturaService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacturaController : ControllerBase
{
    private readonly FacturaDbContext _context;
    private readonly HttpClient _httpClient;

    // Folosim IHttpClientFactory în loc de HttpClient direct
    public FacturaController(FacturaDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient(); // Creăm clientul aici!
    }

    [HttpGet("GetFacturaComplet/{nrFactura}")]
    public async Task<ActionResult<FacturaResponseDto>> GetFacturaComplet(int nrFactura)
    {
        // 1. Luăm factura din propria bază de date
        var factura = await _context.Facturi.FirstOrDefaultAsync(f => f.NrFactura == nrFactura);

        if (factura == null) return NotFound("Factura nu a fost găsită.");

        ClientDto? clientDetails = null;

        try
        {
            // 2. Facem APEL HTTP către ClientService !!!
            // ATENȚIE: Înlocuiește 5001 cu portul real pe care rulează ClientService-ul tău!
            string urlClientService = $"http://localhost:5054/api/Client/GetClientById/{factura.IdClient}";

            var response = await _httpClient.GetAsync(urlClientService);

            if (response.IsSuccessStatusCode)
            {
                // Traducem răspunsul JSON în obiectul nostru C#
                clientDetails = await response.Content.ReadFromJsonAsync<ClientDto>();
            }
        }
        catch (Exception ex)
        {
            // Dacă ClientService e picat, clientDetails rămâne null, 
            // dar FacturaService încă funcționează parțial! Asta e forța microserviciilor!
            Console.WriteLine($"Eroare la contactarea ClientService: {ex.Message}");
        }

        // 3. Asamblăm răspunsul final
        var rezultat = new FacturaResponseDto
        {
            NrFactura = factura.NrFactura,
            DataFactura = factura.DataFactura,
            Client = clientDetails // Va contine numele, sau null daca nu a mers apelul
        };

        return Ok(rezultat);
    }

    [HttpPost("AdaugaFactura")]
    public ActionResult AdaugaFactura([FromBody] FacturaModel factura)
    {
        _context.Facturi.Add(factura);
        _context.SaveChanges();
        return Ok("Factura salvată cu IdClient: " + factura.IdClient);
    }
}