using System.Text.Json.Serialization;

namespace TransparenciaBot.DadosRecebidos;

public class WhatsAppWebhookRequest
{
    [JsonPropertyName("entry")]
    public List<WhatsAppEntry> Entries { get; init; } = [];
}

public class WhatsAppEntry
{
    [JsonPropertyName("changes")]
    public List<WhatsAppChange> Changes { get; init; } = [];
}

public class WhatsAppChange
{
    [JsonPropertyName("value")]
    public WhatsAppValue? Value { get; init; }
}

public class WhatsAppValue
{
    [JsonPropertyName("messages")]
    public List<WhatsAppIncomingMessage> Messages { get; init; } = [];
}

public class WhatsAppIncomingMessage
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("from")]
    public string From { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("text")]
    public WhatsAppText? Text { get; init; }
}

public class WhatsAppText
{
    [JsonPropertyName("body")]
    public string Body { get; init; } = string.Empty;
}
