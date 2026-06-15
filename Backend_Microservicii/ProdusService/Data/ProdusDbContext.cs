using Microsoft.EntityFrameworkCore;
using ProdusService.Modele;

namespace ProdusService.Data;

public class ProdusDbContext : DbContext
{
    public ProdusDbContext(DbContextOptions<ProdusDbContext> options) : base(options) { }
    public DbSet<ProdusModel> Produse { get; set; }
}