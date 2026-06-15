using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientService.Data;
using ClientService.Modele;

namespace ClientService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly ClientDbContext _context;

    public ClientController(ClientDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetClienti")]
    public ActionResult<List<ClientModel>> GetClienti()
    {
        return Ok(_context.Clienti.ToList());
    }

    [HttpGet("GetClientById/{id}")]
    public ActionResult<ClientModel> GetClientById(int id)
    {
        var client = _context.Clienti.Find(id);
        if (client == null) return NotFound($"Clientul {id} nu exista in acest microserviciu.");
        return Ok(client);
    }

    [HttpPost("AdaugaClient")]
    public ActionResult AdaugaClient([FromBody] ClientModel client)
    {
        _context.Clienti.Add(client);
        _context.SaveChanges();
        return Ok("Client adăugat cu succes în baza de date izolată!");
    }
}