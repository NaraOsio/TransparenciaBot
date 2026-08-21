using Microsoft.AspNetCore.Mvc;
using TransparenciaBot.DadosRecebidos;
using TransparenciaBot.Modelos;
using TransparenciaBot.Servicos;

namespace TransparenciaBot.Controladores;

[ApiController]
[Route("api/whatsapp/webhook")]
public class WhatsAppWebhookController(
    IRegistroMensagemServico registroMensagemServico,
    RespostaConsultaServico respostaConsultaServico,
    EnvioWhatsAppServico envioWhatsAppServico,
    IConfiguration configuracao,
    ILogger<WhatsAppWebhookController> logger)
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
                    if (mensagem.Type != "text" ||
                        string.IsNullOrWhiteSpace(mensagem.Text?.Body))
                    {
                        continue;
                    }

                    var registro = await registroMensagemServico.RegistrarAsync(
                        mensagem,
                        cancellationToken);

                    if (!registro.NovaMensagem)
                    {
                        continue;
                    }

                    var respostaEnviada = false;

                    try
                    {
                        await registroMensagemServico.AtualizarEstadoAsync(
                            registro.MensagemId,
                            EstadoMensagem.EmProcessamento,
                            cancellationToken);

                        var resposta =
                            await respostaConsultaServico.CriarRespostaAsync(
                                mensagem.Text.Body,
                                cancellationToken);

                        await envioWhatsAppServico.EnviarTextoAsync(
                            mensagem.From,
                            resposta,
                            cancellationToken);

                        respostaEnviada = true;

                        await registroMensagemServico.AtualizarEstadoAsync(
                            registro.MensagemId,
                            EstadoMensagem.Respondida,
                            cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(
                            exception,
                            "Falha ao processar a mensagem {MensagemId}.",
                            registro.MensagemId);

                        await registroMensagemServico.AtualizarEstadoAsync(
                            registro.MensagemId,
                            EstadoMensagem.Falhou,
                            cancellationToken);

                        await registroMensagemServico.RegistrarFalhaAsync(
                            registro.MensagemId,
                            respostaEnviada
                                ? "Atualização de estado"
                                : "Processamento ou envio da resposta",
                            exception,
                            cancellationToken);

                        if (!respostaEnviada)
                        {
                            try
                            {
                                await envioWhatsAppServico.EnviarTextoAsync(
                                    mensagem.From,
                                    "Não foi possível concluir sua consulta agora. " +
                                    "Tente novamente em alguns minutos ou digite AJUDA.",
                                    cancellationToken);
                            }
                            catch (Exception erroAoEnviarOrientacao)
                            {
                                logger.LogError(
                                    erroAoEnviarOrientacao,
                                    "Não foi possível enviar a orientação ao usuário.");

                                await registroMensagemServico.RegistrarFalhaAsync(
                                    registro.MensagemId,
                                    "Envio da orientação de falha",
                                    erroAoEnviarOrientacao,
                                    cancellationToken);
                            }
                        }
                    }
                }
            }
        }

        return Ok();
    }
}