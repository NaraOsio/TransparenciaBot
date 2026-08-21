using TransparenciaBot.DadosRecebidos;
using TransparenciaBot.Modelos;

namespace TransparenciaBot.Servicos;

public interface IRegistroMensagemServico
{
    Task<RegistroMensagemResultado> RegistrarAsync(
        WhatsAppIncomingMessage mensagemRecebida,
        CancellationToken cancellationToken);

    Task AtualizarEstadoAsync(
        Guid mensagemId,
        EstadoMensagem estado,
        CancellationToken cancellationToken);

    Task RegistrarFalhaAsync(
        Guid mensagemId,
        string etapa,
        Exception exception,
        CancellationToken cancellationToken);
}

public record RegistroMensagemResultado(
    Guid MensagemId,
    bool NovaMensagem);