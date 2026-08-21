using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Dados;
using TransparenciaBot.DadosRecebidos;
using TransparenciaBot.Modelos;

namespace TransparenciaBot.Servicos;

public class RegistroMensagemServico(
    TransparenciaBotDbContext dbContext,
    ILogger<RegistroMensagemServico> logger)
    : IRegistroMensagemServico
{
    public async Task<RegistroMensagemResultado> RegistrarAsync(
        WhatsAppIncomingMessage mensagemRecebida,
        CancellationToken cancellationToken)
    {
        var mensagemExistente = await dbContext.Mensagens
            .AsNoTracking()
            .SingleOrDefaultAsync(
                mensagem => mensagem.IdentificadorWhatsApp == mensagemRecebida.Id,
                cancellationToken);

        if (mensagemExistente is not null)
        {
            return new RegistroMensagemResultado(
                mensagemExistente.Id,
                NovaMensagem: false);
        }

        var telefoneHash = CriarHash(mensagemRecebida.From);

        var usuario = await dbContext.Usuarios.SingleOrDefaultAsync(
            item => item.TelefoneHash == telefoneHash,
            cancellationToken);

        if (usuario is null)
        {
            usuario = new Usuario
            {
                TelefoneHash = telefoneHash
            };

            dbContext.Usuarios.Add(usuario);
        }

        var mensagem = new Mensagem
        {
            Usuario = usuario,
            IdentificadorWhatsApp = mensagemRecebida.Id,
            Conteudo = mensagemRecebida.Text!.Body.Trim(),
            Estado = EstadoMensagem.Recebida
        };

        dbContext.Mensagens.Add(mensagem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new RegistroMensagemResultado(
            mensagem.Id,
            NovaMensagem: true);
    }

    public async Task AtualizarEstadoAsync(
        Guid mensagemId,
        EstadoMensagem estado,
        CancellationToken cancellationToken)
    {
        var mensagem = await dbContext.Mensagens.FindAsync(
            [mensagemId],
            cancellationToken);

        if (mensagem is null)
        {
            logger.LogWarning(
                "Mensagem {MensagemId} não encontrada para atualização de estado.",
                mensagemId);

            return;
        }

        mensagem.Estado = estado;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RegistrarFalhaAsync(
        Guid mensagemId,
        string etapa,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var detalhe = $"{exception.GetType().Name}: {exception.Message}";

        if (detalhe.Length > 2000)
        {
            detalhe = detalhe[..2000];
        }

        dbContext.FalhasProcessamento.Add(new FalhaProcessamento
        {
            MensagemId = mensagemId,
            Etapa = etapa,
            Detalhe = detalhe
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string CriarHash(string valor)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}