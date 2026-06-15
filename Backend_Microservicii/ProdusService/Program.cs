using Microsoft.EntityFrameworkCore;
using ProdusService.Data; // Asigură-te că ai acest using!

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
// AICI ESTE SECRETUL: Trebuie să fie ProdusDbContext (nu ClientDbContext)
builder.Services.AddDbContext<ProdusDbContext>(options =>
    options.UseSqlite("Data Source=produse.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//  Creează automat baza de date și tabelele în Docker dacă nu există
using (var scope = app.Services.CreateScope())
{
    // Înlocuiește "NumeleDbContextuluiTau" cu numele real al clasei tale de DbContext (ex: AppDbContext, ClientContext etc.)
    var dbContext = scope.ServiceProvider.GetRequiredService<ProdusDbContext>();
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