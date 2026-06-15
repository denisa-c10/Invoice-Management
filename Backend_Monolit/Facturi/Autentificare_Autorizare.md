# Autentificare și Autorizare în ASP.NET Core Web API

## Autentificare
Autentificarea este procesul de verificare a identității unui utilizator. Înseamnă să răspundem la întrebarea: "Cine ești tu?"  
În acest proiect, autentificarea se face prin verificarea username-ului și parolei în metoda `Login` din `AuthController`. Dacă credențialele sunt valide, se generează un JWT token care confirmă identitatea utilizatorului.

## Autorizare
Autorizarea este procesul de control al accesului la resurse după ce identitatea a fost verificată. Înseamnă să răspundem la întrebarea: "Ce poți face?"  
În ASP.NET Core, autorizarea se face prin atribute precum `[Authorize]`, care verifică dacă utilizatorul are permisiunile necesare (de exemplu, roluri sau politici). Endpoint-urile protejate necesită un token JWT valid pentru acces.

## JWT (JSON Web Token)
JWT este un standard pentru transmiterea securizată a informațiilor între părți ca un obiect JSON. În acest proiect:
- Token-ul este generat în `AuthService` după autentificare reușită
- Conține informații precum username, roluri și dată de expirare
- Este semnat cu o cheie secretă pentru a preveni modificările
- Clientul trimite token-ul în header-ul `Authorization: Bearer <token>` pentru endpoint-urile protejate

## Endpoint-uri Publice și Protejate
- **Endpoint-uri publice**: Nu necesită autentificare. Au atributul `[AllowAnonymous]`. Exemplu: `GET /Auth/PublicInfo` sau `GET /Client/GetClienti` (din ClientController).
- **Endpoint-uri protejate**: Necesită un token JWT valid. Au atributul `[Authorize]`. Exemplu: `GET /Auth/ProtectedInfo` sau majoritatea endpoint-urilor din ClientController, FacturaController etc.

## Testare în Swagger
Swagger (OpenAPI) permite testarea API-ului direct din browser.

### Pași pentru testare:
1. **Pornește aplicația**: Rulează `dotnet run` în directorul proiectului.
2. **Accesează Swagger**: Deschide browser-ul la `https://localhost:5001/swagger` (sau portul configurat).
3. **Testează endpoint public**:
   - Mergi la `Auth` > `GET /Auth/PublicInfo`
   - Apasă "Try it out" și "Execute" - ar trebui să primești răspuns fără autentificare.
4. **Fă login pentru token**:
   - Mergi la `Auth` > `POST /Auth/Login`
   - Apasă "Try it out"
   - Introdu în Request body:
     ```json
     {
       "username": "admin",
       "password": "admin123"
     }
     ```
   - Apasă "Execute" - primești un token în răspuns.
5. **Testează endpoint protejat**:
   - Copiază token-ul din răspunsul de login.
   - Mergi la un endpoint protejat, de exemplu `Auth` > `GET /Auth/ProtectedInfo`
   - Apasă "Try it out"
   - În secțiunea "Authorize" (butonul cu lacăt), introdu `Bearer <token>` (înlocuiește `<token>` cu token-ul real)
   - Apasă "Execute" - ar trebui să primești răspuns cu token valid.
6. **Testează fără token**: Pe același endpoint protejat, șterge autorizarea și apasă "Execute" - ar trebui să primești eroare 401 Unauthorized.

### Note:
- Token-ul expiră după 1 oră (configurat în `AuthService`).
- Pentru endpoint-uri din alți controller-e (Client, Factura), folosește același token obținut de la login.</content>
<parameter name="filePath">c:\Users\User\Desktop\IE III - client - 18.03\Backend\Facturi\Autentificare_Autorizare.md