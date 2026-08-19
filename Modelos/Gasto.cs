namespace TransparenciaBot.Modelos;

public class Gasto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int IdDeputadoCamara { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }

    public string TipoDespesa { get; set; } = string.Empty;
    public decimal ValorDocumento { get; set; }
    public decimal ValorLiquido { get; set; }

    public string NomeFornecedor { get; set; } = string.Empty;
    public DateTime? DataDocumento { get; set; }
    public string? UrlDocumento { get; set; }

    public DateTimeOffset ImportadoEmUtc { get; set; } = DateTimeOffset.UtcNow;
}