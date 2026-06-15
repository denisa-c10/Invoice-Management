using Facturi.Modele;
using Facturi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facturi.Controllers;

// Marchează această clasă ca fiind un Controller de API (activează validări automate și formatarea JSON)
[ApiController]
// Definește ruta URL-ului. De exemplu: http://localhost:port/Auth/Login
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    // Aici păstrăm o referință către serviciul care știe să valideze parole și să creeze token-uri
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ----------------------------------------------------------------------
    // 1. METODA DE LOGIN (Aici primește utilizatorul "cheia")
    // ----------------------------------------------------------------------
    [HttpPost]
    [AllowAnonymous]
    public ActionResult<TokenResponse> Login([FromBody] LoginRequest request)
    {
        // Pasul A: Verificăm dacă request-ul a venit gol
        if (request == null)
        {
            return BadRequest("Request body lipseste.");
        }

        // Pasul B: Verificăm dacă a uitat să completeze user-ul sau parola
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username si parola sunt obligatorii.");
        }

        // Pasul C: Trimitem datele la serviciul nostru ca să verifice în baza de date dacă parola e corectă
        if (!_authService.ValidateCredentials(request.Username, request.Password))
        {
            return Unauthorized("Username sau parola invalide.");
        }

        // Pasul D: Dacă parola e corectă, generăm Token-ul JWT 
        var token = _authService.GenerateJwtToken(request.Username);
        var expiresAt = DateTime.UtcNow.AddHours(1);

        // Pasul F: Returnăm token-ul către frontend (Vue.js)
        return Ok(new TokenResponse
        {
            Token = token,
            ExpiresAt = expiresAt
        });
    }

    // ----------------------------------------------------------------------
    // 2. ENDPOINT PUBLIC 
    // ----------------------------------------------------------------------
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<string> PublicInfo()
    {
        return Ok("Acesta este un endpoint public. Nu necesita JWT.");
    }

    // ----------------------------------------------------------------------
    // 3. ENDPOINT PROTEJAT 
    // ----------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    public ActionResult<string> ProtectedInfo()
    {
        // Dacă a ajuns aici, înseamnă că token-ul a fost trimis, valid și neexpirat.
        // User.Identity.Name extrage automat numele utilizatorului direct din token-ul decriptat de ASP.NET.
        var user = User.Identity?.Name ?? "necunoscut";

        return Ok($"Salut {user}! Acesta este un endpoint protejat. Tokenul este valid.");
    }
}