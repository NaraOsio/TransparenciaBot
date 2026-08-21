using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TransparenciaBot.Servicos;

public class EnvioWhatsAppServico(
    HttpClient httpClient,
    IConfiguration configuracao)
{
    public async Task EnviarTextoAsync(
        string telefoneDestino,
        string texto,
        CancellationToken cancellationToken)
    {
        var token = configuracao["WhatsApp:AccessToken"]
            ?? throw new InvalidOperationException(
                "O AccessToken do WhatsApp não foi configurado.");

        var telefoneId = configuracao["WhatsApp:PhoneNumberId"]
            ?? throw new InvalidOperationException(
                "O PhoneNumberId do WhatsApp não foi configurado.");

        using var requisicao = new HttpRequestMessage(
            HttpMethod.Post,
            $"{telefoneId}/messages");

        requisicao.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        requisicao.Content = JsonContent.Create(new
        {
            messaging_product = "whatsapp",
            to = telefoneDestino,
            type = "text",
            text = new
            {
                body = texto,
                preview_url = false
            }
        });

        using var resposta = await httpClient.SendAsync(
            requisicao,
            cancellationToken);

        if (!resposta.IsSuccessStatusCode)
{
    var detalheErro = await resposta.Content.ReadAsStringAsync(
        cancellationToken);

    throw new InvalidOperationException(
        $"A Meta recusou o envio: {detalheErro}");
}
    }
}