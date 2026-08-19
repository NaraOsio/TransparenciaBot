using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.DadosRecebidos;
using TransparenciaBot.Dados;
using TransparenciaBot.Modelos;

namespace TransparenciaBot.Servicos;

public class RegistroMensagemServico(TransparenciaBotDbContext dbContext, ILogger<RegistroMensagemServico> logger)
    : IRegistroMensagemServico
{
    public async Task RegistrarAsync(WhatsAppIncomingMessage mensagemRecebida, CancellationToken cancellationToken)
    {
        // Eventos de status e mídias não são mensagens de texto e serão tratados em incrementos próprios.
        if (mensagemRecebida.Type != "text" || string.IsNullOrWhiteSpace(mensagemRecebida.Text?.Body))
        {
            logger.LogInformation("Evento do WhatsApp ignorado: tipo {Tipo}", mensagemRecebida.Type);
            return;
        }

        if (await dbContext.Mensagens.AnyAsync(
                mensagem => mensagem.IdentificadorWhatsApp == mensagemRecebida.Id,
                cancellationToken))
        {
            return; // A Meta pode reenviar o mesmo webhook; este registro precisa ser idempotente.
        }

        var telefoneHash = CriarHash(mensagemRecebida.From);
        var usuario = await dbContext.Usuarios.SingleOrDefaultAsync(
            item => item.TelefoneHash == telefoneHash,
            cancellationToken);

        if (usuario is null)
        {
            usuario = new Usuario { TelefoneHash = telefoneHash };
            dbContext.Usuarios.Add(usuario);
        }

        dbContext.Mensagens.Add(new Mensagem
        {
            Usuario = usuario,
            IdentificadorWhatsApp = mensagemRecebida.Id,
            Conteudo = mensagemRecebida.Text.Body.Trim(),
            Estado = EstadoMensagem.Recebida
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string CriarHash(string valor)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
