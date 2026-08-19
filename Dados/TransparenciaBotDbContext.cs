using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Modelos;

namespace TransparenciaBot.Dados;

public class TransparenciaBotDbContext(DbContextOptions<TransparenciaBotDbContext> options)
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Mensagem> Mensagens => Set<Mensagem>();
    public DbSet<FalhaProcessamento> FalhasProcessamento => Set<FalhaProcessamento>();
    public DbSet<Gasto> Gastos => Set<Gasto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(usuario => usuario.Id);
            entity.Property(usuario => usuario.TelefoneHash).HasMaxLength(64).IsRequired();
            entity.HasIndex(usuario => usuario.TelefoneHash).IsUnique();
        });

        modelBuilder.Entity<Mensagem>(entity =>
        {
            entity.HasKey(mensagem => mensagem.Id);
            entity.Property(mensagem => mensagem.Conteudo).HasMaxLength(4096).IsRequired();
            entity.Property(mensagem => mensagem.Estado).HasConversion<string>().HasMaxLength(40);
            entity.HasIndex(mensagem => mensagem.IdentificadorWhatsApp).IsUnique();
            entity.HasOne(mensagem => mensagem.Usuario)
                .WithMany(usuario => usuario.Mensagens)
                .HasForeignKey(mensagem => mensagem.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FalhaProcessamento>(entity =>
        {
            entity.HasKey(falha => falha.Id);
            entity.Property(falha => falha.Etapa).HasMaxLength(80).IsRequired();
            entity.Property(falha => falha.Detalhe).HasMaxLength(2000).IsRequired();
            entity.HasOne(falha => falha.Mensagem)
                .WithMany(mensagem => mensagem.Falhas)
                .HasForeignKey(falha => falha.MensagemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Gasto>(entity =>
{
    entity.HasKey(gasto => gasto.Id);

    entity.Property(gasto => gasto.TipoDespesa)
        .HasMaxLength(300)
        .IsRequired();

    entity.Property(gasto => gasto.NomeFornecedor)
        .HasMaxLength(300)
        .IsRequired();

    entity.Property(gasto => gasto.ValorDocumento)
        .HasPrecision(18, 2);

    entity.Property(gasto => gasto.ValorLiquido)
        .HasPrecision(18, 2);

    entity.HasIndex(gasto => new
    {
        gasto.IdDeputadoCamara,
        gasto.Ano
    });
});
    }

}
