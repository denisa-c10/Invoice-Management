using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdusService.Data;
using ProdusService.Modele;

namespace ProdusService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdusController : ControllerBase
{
    private readonly ProdusDbContext _context;

    public ProdusController(ProdusDbContext context)
    {
        _context = context;
    }

    // Obține toate produsele din catalog
    [HttpGet("GetProduse")]
    public ActionResult<List<ProdusModel>> GetProduse()
    {
        return Ok(_context.Produse.ToList());
    }

    // Obține detaliile unui singur produs (util pentru FacturaService)
    [HttpGet("GetProdusById/{id}")]
    public ActionResult<ProdusModel> GetProdusById(int id)
    {
        var produs = _context.Produse.Find(id);
        if (produs == null) return NotFound($"Produsul cu ID {id} nu a fost găsit.");
        return Ok(produs);
    }

    // Adaugă un produs nou în catalog
    [HttpPost("AdaugaProdus")]
    public ActionResult AdaugaProdus([FromBody] ProdusModel produs)
    {
        _context.Produse.Add(produs);
        _context.SaveChanges();
        return Ok("Produs adăugat cu succes în catalogul microserviciului!");
    }
}