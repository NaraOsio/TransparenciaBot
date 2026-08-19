namespace TransparenciaBot.Modelos;

public class Mensagem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string IdentificadorWhatsApp { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public EstadoMensagem Estado { get; set; } = EstadoMensagem.Recebida;
    public DateTimeOffset RecebidaEmUtc { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<FalhaProcessamento> Falhas { get; set; } = new List<FalhaProcessamento>();
}
