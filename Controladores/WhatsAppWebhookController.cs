using Microsoft.AspNetCore.Mvc;
using TransparenciaBot.DadosRecebidos;
using TransparenciaBot.Servicos;

namespace TransparenciaBot.Controladores;

[ApiController]
[Route("api/whatsapp/webhook")]
public class WhatsAppWebhookController(
    IRegistroMensagemServico registroMensagemServico,
    IConfiguration configuracao)
    : ControllerBase
{
    [HttpGet]
    public IActionResult VerificarWebhook(
        [FromQuery(Name = "hub.mode")] string? modo,
        [FromQuery(Name = "hub.verify_token")] string? token,
        [FromQuery(Name = "hub.challenge")] string? desafio)
    {
        var tokenEsperado = configuracao["WhatsApp:VerifyToken"];

        if (modo == "subscribe" &&
            !string.IsNullOrWhiteSpace(tokenEsperado) &&
            token == tokenEsperado &&
            !string.IsNullOrWhiteSpace(desafio))
        {
            return Content(desafio, "text/plain");
        }

        return Forbid();
    }

    [HttpPost]
    public async Task<IActionResult> ReceberMensagem(
        [FromBody] WhatsAppWebhookRequest webhook,
        CancellationToken cancellationToken)
    {
        foreach (var entrada in webhook.Entries)
        {
            foreach (var alteracao in entrada.Changes)
            {
                var mensagens = alteracao.Value?.Messages ?? [];

                foreach (var mensagem in mensagens)
                {
                    await registroMensagemServico.RegistrarAsync(
                        mensagem,
                        cancellationToken);
                }
            }
        }

        return Ok();
    }
}