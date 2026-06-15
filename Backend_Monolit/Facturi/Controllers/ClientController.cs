using Facturi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Facturi.Hubs;

namespace Facturi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ClientController : ControllerBase
{
    private const string FilePath = "client.txt";

    private readonly IClient _clientService;
    private readonly IHubContext<ClientiHub> _hubContext;

    //  Injectăm hub-ul în constructor
    public ClientController(IClient clientService, IHubContext<ClientiHub> hubContext)
    {
        _clientService = clientService;
        _hubContext = hubContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<List<ClientModel>> GetClienti()
    {
        var lista = _clientService.GetClienti();
        return Ok(lista);
    }

    [HttpGet]
    public ActionResult<ClientModel> GetClientById(int id)
    {
        var client = _clientService.GetClientById(id);
        if (client != null)
        {
            return Ok(client);
        }
        return NotFound($"Clientul cu id-ul {id} nu a fost gasit.");
    }

    [HttpGet]
    public ActionResult<List<ClientModel>> GetClientiByNume(string nume)
    {
        var clienti = _clientService.GetClientiByNume(nume);

        if (clienti != null && clienti.Count > 0)
        {
            return Ok(clienti);
        }
        return NotFound($"Nu s-au gasit clienti cu numele {nume}.");
    }

    [HttpPost]
    public async Task<ActionResult<ClientModel>> PostClientModel(ClientModel client) // NOU: async Task
    {
        _clientService.AddClient(client);

        //  Anunțăm toți clienții
        await _hubContext.Clients.All.SendAsync("UpdateClienti");

        return CreatedAtAction(nameof(GetClientById), new { id = client.IdClient }, client);
    }

    [HttpPut]
    public async Task<ActionResult<ClientModel>> PutClientModel([FromQuery] int id, [FromBody] ClientModel client) // NOU: async Task
    {
        var clientExistent = _clientService.GetClientById(id);
        if (clientExistent != null)
        {
            _clientService.UpdateClient(id, client);

            //  Anunțăm modificarea
            await _hubContext.Clients.All.SendAsync("UpdateClienti");

            return Ok(client);
        }
        return NotFound($"Clientul cu id-ul {id} nu a fost gasit.");
    }

    [HttpDelete]
    public async Task<ActionResult<ClientModel>> DeleteClientModel(int id) // NOU: async Task
    {
        var clientExistent = _clientService.GetClientById(id);
        if (clientExistent != null)
        {
            _clientService.DeleteClient(id);

            //  Anunțăm ștergerea
            await _hubContext.Clients.All.SendAsync("UpdateClienti");

            return Ok(clientExistent);
        }
        return NotFound($"Clientul cu id-ul {id} nu a fost gasit.");
    }
}