using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Models;

namespace TransparenciaBot.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Politico> Politicos { get; set; } 
      public DbSet<Gasto> Gastos { get; set; }
       public DbSet<Usuario> Usuarios { get; set; } 
       public DbSet<Mensagem> Mensagens { get; set; }
}