var builder = WebApplication.CreateBuilder(args);

// 1. Setările de CORS pentru a nu avea probleme cu Vue
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermiteVue", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



// 3. Încărcăm rutele dispecerului
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// 4. Activăm STRICT Interfața Grafică (Fără generator)
app.UseSwaggerUI(c =>
{
    // În Docker: folosim localhost cu porturile mapate din docker-compose
    // În local: folosim porturile directe ale serviciilor
    c.SwaggerEndpoint("http://localhost:5117/swagger/v1/swagger.json", "👥 Gestionare Clienți");
    c.SwaggerEndpoint("http://localhost:5118/swagger/v1/swagger.json", "🛒 Gestionare Produse");
    c.SwaggerEndpoint("http://localhost:5116/swagger/v1/swagger.json", "📄 Gestionare Facturi");

    c.RoutePrefix = string.Empty; // Deschide interfața direct pe portul 5000
});

app.UseCors("PermiteVue");
app.MapReverseProxy();
app.Run();