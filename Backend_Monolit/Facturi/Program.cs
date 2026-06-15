using Facturi.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using Facturi.Hubs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURARE BAZĂ DE DATE ---
var configLocStocare = builder.Configuration.GetValue<string>("LocatieStocare");

if (configLocStocare == "Fisier")
{
    builder.Services.AddScoped<IFactura, FacturaServiceFile>();
    builder.Services.AddScoped<IClient, ClientServiceFile>();
    builder.Services.AddScoped<IProdus, ProdusServiceFile>();
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IFactura, FacturaServiceBD>();
    builder.Services.AddScoped<IClient, ClientServiceBD>();
    builder.Services.AddScoped<IProdus, ProdusServiceBD>();
}

//  SERVICIUL DE AUTENTIFICARE 
builder.Services.AddScoped<IAuthService, AuthService>();

// --- 2. CONFIGURARE CORS (Pentru Vue) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitAppVue", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// --- 3. CONTROLLERE ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// --- 4. SWAGGER  ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Monolit", Version = "v1" });

    // Definim butonul de Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introdu token-ul JWT in formatul: Bearer {token-ul_tau}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --- 5. SIGNALR ---
builder.Services.AddSignalR();

// =================================================================
// BUILD (Construim aplicația)
// =================================================================
var app = builder.Build();

// Crearea bazei de date (dacă e cazul)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Activăm Swagger-ul (Interfața grafică)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitAppVue");

// Auth-ul trebuie să fie mereu aici
app.UseAuthentication();
app.UseAuthorization();

// Activăm suportul pentru WebSockets
app.UseWebSockets();

// Mapăm Controllerele
app.MapControllers();

// Mapăm WebSockets prin SignalR
app.MapHub<NotificariHub>("/hub-notificari");
app.MapHub<ProduseHub>("/produseHub");
app.MapHub<ClientiHub>("/clientiHub");
app.MapHub<FacturiHub>("/facturiHub");

app.Run();