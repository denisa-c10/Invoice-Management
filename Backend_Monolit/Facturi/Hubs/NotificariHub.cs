using Microsoft.AspNetCore.SignalR;

namespace Facturi.Hubs;

public class NotificariHub : Hub
{
    // COMANDA 1: Clientul trimite o notificare care va fi văzută de TOȚI utilizatorii conectați
    public async Task TrimiteAlertaGlobala(string numeUtilizator, string mesaj)
    {
        Console.WriteLine($"[WebSocket] Comanda 1 apelată de {numeUtilizator}");

        // Serverul ia mesajul și îl distribuie tuturor clienților Vue
        await Clients.All.SendAsync("PrimesteAlerta", numeUtilizator, mesaj);
    }

    // COMANDA 2: Clientul cere un status, iar serverul îi răspunde DOAR LUI
    public async Task CereStatusSistem()
    {
        Console.WriteLine("[WebSocket] Comanda 2 apelată");

        // Simulăm verificarea sistemului
        string status = $"Sistemul ERP funcționează perfect! Ora serverului: {DateTime.Now.ToString("HH:mm:ss")}";

        // Răspundem strict clientului care a făcut cererea (Caller)
        await Clients.Caller.SendAsync("PrimesteStatus", status);
    }
}