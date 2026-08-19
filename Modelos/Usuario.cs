namespace TransparenciaBot.Modelos;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // O número puro não é persistido. O hash permite relacionar mensagens sem expor o telefone.
    public string TelefoneHash { get; set; } = string.Empty;
    public DateTimeOffset CriadoEmUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
}
