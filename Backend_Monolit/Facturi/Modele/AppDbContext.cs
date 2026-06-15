using Microsoft.EntityFrameworkCore;
using Facturi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FacturaModel2> Facturi { get; set; }
    public DbSet<ClientModel> Clienti { get; set; }
    public DbSet<ProdusModel> Produse { get; set; }
    public DbSet<ProdusFacturaModel2> ProduseFactura { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //tip de date zecimal pt Pret
        modelBuilder.Entity<ProdusFacturaModel2>()
            .Property(p => p.Pret)
            .HasConversion<double>();

        modelBuilder.Entity<ProdusModel>()
            .Property(p => p.Pret)
            .HasConversion<double>();

        // Relatia Client -> Facturi (Un client are mai multe facturi)
        modelBuilder.Entity<ClientModel>()
            .HasMany(c => c.Facturi)
            .WithOne(f => f.ClientFactura)
            .OnDelete(DeleteBehavior.Cascade);


        //strategie Table-per-Type (TPT) pentru FacturaModel si ProdusFacturaModel
        modelBuilder.Entity<ProdusModel>().ToTable("Produse");


        //v2 APARTENETA (rand din factura care are un "stăpîn") CU COMPOZITIE IN LOC DE MOSTENIRE
        //1. Relația ProdusFactura -> Factura (Fiecare rând de factură aparține unei facturi)
        modelBuilder.Entity<ProdusFacturaModel2>()
            .ToTable("ProduseFactura2")
            .HasOne(pf => pf.Factura)
            .WithMany(f => f.ProduseFactura2)
            .HasForeignKey(pf => pf.NrFactura)
            .OnDelete(DeleteBehavior.Cascade);

        //2. Relația ProdusFactura -> Produs (Fiecare rând de factură se referă la un produs specific)
        modelBuilder.Entity<ProdusFacturaModel2>()
            //.ToTable("ProduseFactura2")
            .HasOne(pf => pf.Produs)
            //.WithMany(f => f.ProduseFactura2)
            .WithMany()
            .HasForeignKey(pf => pf.IdProdus)
            .OnDelete(DeleteBehavior.Restrict);

        //SAU (Compozitie --> Factura contine o lista de...)
        modelBuilder.Entity<FacturaModel2>()
            .HasMany(f => f.ProduseFactura2)      // Factura are mai multe rânduri de produse
            .WithOne(pf => pf.Factura)           // Fiecare rând aparține unei singure facturi
            .HasForeignKey(pf => pf.NrFactura)   // Cheia externă este în tabela ProduseFactura2
            .OnDelete(DeleteBehavior.Cascade);   // Dacă șterg factura, se șterg și rândurile ei

    }
}