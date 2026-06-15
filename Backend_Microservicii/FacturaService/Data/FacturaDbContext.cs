using Microsoft.EntityFrameworkCore;
using FacturaService.Modele;

namespace FacturaService.Data;

public class FacturaDbContext : DbContext
{
    public FacturaDbContext(DbContextOptions<FacturaDbContext> options) : base(options)
    {
    }

    public DbSet<FacturaModel> Facturi { get; set; }
    public DbSet<ProdusFacturaModel> ProduseFactura { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Păstrăm doar relația compusă dintre Factură și Liniile ei
        modelBuilder.Entity<FacturaModel>()
            .HasMany(f => f.ProduseFactura)
            .WithOne(pf => pf.Factura)
            .HasForeignKey(pf => pf.NrFactura)
            .OnDelete(DeleteBehavior.Cascade);
    }
}