using Microsoft.EntityFrameworkCore;
using ClientService.Modele;

namespace ClientService.Data;

public class ClientDbContext : DbContext
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
    {
    }

    // SINGURA tabelă din acest microserviciu! Izolare totală.
    public DbSet<ClientModel> Clienti { get; set; }
}