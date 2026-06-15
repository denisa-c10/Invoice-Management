using Microsoft.EntityFrameworkCore;
using ClientService.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermiteTot", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Adăugăm baza de date (va crea un fisier "clienti.db" doar pentru acest serviciu)
builder.Services.AddDbContext<ClientDbContext>(options =>
    options.UseSqlite("Data Source=clienti.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Creează automat baza de date și tabelele în Docker dacă nu există
using (var scope = app.Services.CreateScope())
{
    // Înlocuiește "NumeleDbContextuluiTau" cu numele real al clasei tale de DbContext (ex: AppDbContext, ClientContext etc.)
    var dbContext = scope.ServiceProvider.GetRequiredService<ClientDbContext>();
    dbContext.Database.EnsureCreated();
}
app.UseCors("PermiteTot");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();