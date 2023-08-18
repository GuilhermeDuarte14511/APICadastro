using ClientAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ClientContext : DbContext
{
    public ClientContext(DbContextOptions<ClientContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }

    // Outros membros do contexto aqui
}
