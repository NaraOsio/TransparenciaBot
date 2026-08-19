using TransparenciaBot.DadosRecebidos;

namespace TransparenciaBot.Servicos;

public interface IRegistroMensagemServico
{
    Task RegistrarAsync(WhatsAppIncomingMessage mensagemRecebida, CancellationToken cancellationToken);
}
