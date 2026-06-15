using Facturi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Facturi.Hubs;

namespace Facturi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class FacturaController : ControllerBase
{
    private const string FilePath = "factura.txt";

    private readonly IFactura _facturaService;
    private readonly IHubContext<FacturiHub> _hubContext;

    // Injectăm hub-ul în constructor
    public FacturaController(IFactura facturaService, IHubContext<FacturiHub> hubContext)
    {
        _facturaService = facturaService;
        _hubContext = hubContext;
    }

    [HttpGet]
    public ActionResult<List<FacturaModel2>> GetFacturi()
    {
        var lista = _facturaService.GetFacturi();
        return Ok(lista);
    }

    [HttpGet]
    public ActionResult<FacturaModel2> GetFacturaByNr(int nrFactura)
    {
        var factura = _facturaService.GetFacturaByNr(nrFactura);
        if (factura != null)
        {
            return Ok(factura);
        }
        return NotFound($"Factura cu numarul {nrFactura} nu a fost gasita.");
    }

    [HttpGet]
    public ActionResult<List<FacturaModel2>> GetFacturiByClient(int idClient)
    {
        var facturi = _facturaService.GetFacturiByClient(idClient);

        if (facturi != null && facturi.Count > 0)
        {
            return Ok(facturi);
        }
        return NotFound($"Nu s-au gasit facturi pentru clientul cu id-ul {idClient}.");
    }

    [HttpPost]
    public async Task<ActionResult> AdaugaFactura([FromBody] FacturaModel2 factura)
    {
        _facturaService.AddFactura(factura);

        //  Anunțăm factura nouă
        await _hubContext.Clients.All.SendAsync("UpdateFacturi");

        return Ok("Factura adaugata cu succes!");
    }

    [HttpPut]
    public async Task<ActionResult> UpdateFactura([FromQuery] int nrFactura, [FromBody] FacturaModel2 facturaActualizata) // NOU: async Task
    {
        var factura = _facturaService.GetFacturaByNr(nrFactura);
        if (factura != null)
        {
            _facturaService.UpdateFactura(nrFactura, facturaActualizata);

            //  Anunțăm modificarea (ex: încasarea!)
            await _hubContext.Clients.All.SendAsync("UpdateFacturi");

            return Ok("Factura actualizata cu succes!");
        }
        return NotFound($"Factura cu numarul {nrFactura} nu a fost gasita.");
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteFactura(int nrFactura) // NOU: async Task
    {
        var factura = _facturaService.GetFacturaByNr(nrFactura);
        if (factura != null)
        {
            _facturaService.DeleteFactura(nrFactura);

            //  Anunțăm ștergerea
            await _hubContext.Clients.All.SendAsync("UpdateFacturi");

            return Ok("Factura stearsa cu succes!");
        }
        return NotFound($"Factura cu numarul {nrFactura} nu a fost gasita.");
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteProdusDinFactura(int nrFactura, int idProdusFactura) // NOU: async Task
    {
        _facturaService.DeleteProdusDinFactura(nrFactura, idProdusFactura);

        //  Anunțăm modificarea
        await _hubContext.Clients.All.SendAsync("UpdateFacturi");

        return Ok("Produs sters din factura cu succes!");
    }
}