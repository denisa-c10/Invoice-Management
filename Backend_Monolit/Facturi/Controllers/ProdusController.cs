using Facturi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR; // NOU: Pachetul pentru SignalR
using Facturi.Hubs;                 // NOU: Folderul unde ai creat ProduseHub.cs

namespace Facturi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ProdusController : ControllerBase
{
    private readonly IProdus _produsService;
    private readonly IHubContext<ProduseHub> _hubContext; // Referința către "canalul de radio"

    // NOU: Am adăugat IHubContext în constructor ca să-l primim la pornire
    public ProdusController(IProdus produsService, IHubContext<ProduseHub> hubContext)
    {
        _produsService = produsService;
        _hubContext = hubContext;
    }

    [HttpGet]
    public List<ProdusModel> GetProdusModelsHardcodat()
    {
        var lista = new List<ProdusModel>();
        var p1 = new ProdusModel() { IdProdus = 1, NumeProdus = "mere", Pret = 2.5m };
        var p2 = new ProdusModel() { IdProdus = 2, NumeProdus = "pere", Pret = 3.0m };
        var p3 = new ProdusModel() { IdProdus = 3, NumeProdus = "struguri", Pret = 5.5m };

        lista.Add(p1);
        lista.Add(p2);
        lista.Add(p3);

        return lista;
    }

    [HttpGet]
    public ActionResult<List<ProdusModel>> GetProdusModels()
    {
        return _produsService.GetProdusModels();
    }

    [HttpGet]
    public ActionResult<List<ProdusModel>> GetProdusModelsByNume(string nume)
    {
        return Ok(_produsService.GetProdusModelsByNume(nume));
    }

    [HttpGet]
    public ActionResult<ProdusModel> GetProdusModelById(int id)
    {
        var produs = _produsService.GetProdusModelById(id);
        if (produs != null)
        {
            return Ok(produs);
        }
        return NotFound($"Produsul cu id-ul {id} nu a fost gasit.");
    }

    [HttpPost]
    // NOU: Metoda devine async Task<...> pentru că folosim 'await' mai jos
    public async Task<ActionResult<ProdusModel>> PostProdusModel(ProdusModel produs)
    {
        _produsService.AddProdus(produs);

        //  Urlăm pe canalul de radio către tot frontend-ul să se actualizeze
        await _hubContext.Clients.All.SendAsync("UpdateProduse");

        return CreatedAtAction(nameof(GetProdusModelById), new { id = produs.IdProdus }, produs);
    }

    //metoda de actualizarde a unui produs
    [HttpPut]
    // NOU: Metoda devine async Task<...>
    public async Task<ActionResult<ProdusModel>> PutProdusModel([FromQuery] int id, [FromBody] ProdusModel produs)
    {
        var produsExistent = _produsService.GetProdusModelById(id);
        if (produsExistent != null)
        {
            _produsService.UpdateProdus(id, produs);

            // Anunțăm modificarea de stoc/preț/nume
            await _hubContext.Clients.All.SendAsync("UpdateProduse");

            return Ok(produs);
        }
        return NotFound($"Produsul cu id-ul {id} nu a fost gasit.");
    }


    [HttpDelete]
    // NOU: Metoda devine async Task<...>
    public async Task<ActionResult<ProdusModel>> DeleteProdusModel(int id)
    {
        var produsExistent = _produsService.GetProdusModelById(id);
        if (produsExistent != null)
        {
            _produsService.DeleteProdus(id);

            // Anunțăm ștergerea
            await _hubContext.Clients.All.SendAsync("UpdateProduse");

            return Ok(produsExistent);
        }
        return NotFound($"Produsul cu id-ul {id} nu a fost gasit.");
    }
}