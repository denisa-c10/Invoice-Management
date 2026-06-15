using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace Facturi.Controllers;

[ApiController]
public class WebSocketController : ControllerBase
{
    // Aici e secretul: 'static' face ca această listă să supraviețuiască
    // și să fie aceeași pentru toți clienții, chiar dacă Controller-ul se recreează.
    private static readonly List<WebSocket> _conexiuniActive = new();

    // Definim ușa exact ca în Program.cs
    [HttpGet("/ws-nativ")]
    public async Task ConectareNativa()
    {
        // În controllere, accesăm WebSockets prin obiectul "HttpContext"
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _conexiuniActive.Add(webSocket);
            Console.WriteLine("[WebSocket Controller] Un client s-a conectat!");

            var buffer = new byte[1024 * 4];

            try
            {
                var rezultat = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!rezultat.CloseStatus.HasValue)
                {
                    var mesajPrimit = Encoding.UTF8.GetString(buffer, 0, rezultat.Count);
                    Console.WriteLine($"[WebSocket Controller] Mesaj primit: {mesajPrimit}");

                    // --- LOGICA COMENZILOR ---
                    if (mesajPrimit.StartsWith("ALERTA|"))
                    {
                        var bițiRăspuns = Encoding.UTF8.GetBytes(mesajPrimit);
                        foreach (var conexiune in _conexiuniActive.ToList())
                        {
                            if (conexiune.State == WebSocketState.Open)
                            {
                                await conexiune.SendAsync(new ArraySegment<byte>(bițiRăspuns), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                    else if (mesajPrimit == "STATUS")
                    {
                        string textStatus = $"STATUS_SERVER|Sistemul funcționează perfect! Ora: {DateTime.Now:HH:mm:ss}";
                        var bițiStatus = Encoding.UTF8.GetBytes(textStatus);
                        await webSocket.SendAsync(new ArraySegment<byte>(bițiStatus), WebSocketMessageType.Text, true, CancellationToken.None);
                    }

                    rezultat = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                _conexiuniActive.Remove(webSocket);
                await webSocket.CloseAsync(rezultat.CloseStatus.Value, rezultat.CloseStatusDescription, CancellationToken.None);
                Console.WriteLine("[WebSocket Controller] Client deconectat.");
            }
            catch (Exception)
            {
                _conexiuniActive.Remove(webSocket);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}