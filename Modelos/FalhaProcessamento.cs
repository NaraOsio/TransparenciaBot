namespace TransparenciaBot.Modelos;

public class FalhaProcessamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MensagemId { get; set; }
    public Mensagem Mensagem { get; set; } = null!;
    public string Etapa { get; set; } = string.Empty;
    public string Detalhe { get; set; } = string.Empty;
    public DateTimeOffset RegistradaEmUtc { get; set; } = DateTimeOffset.UtcNow;
}
