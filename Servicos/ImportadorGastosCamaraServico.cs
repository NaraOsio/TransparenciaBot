using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Dados;
using TransparenciaBot.Modelos;

namespace TransparenciaBot.Servicos;

public class ImportadorGastosCamaraServico(
    HttpClient httpClient,
    TransparenciaBotDbContext dbContext,
    ILogger<ImportadorGastosCamaraServico> logger)

{
   private static DateTime? ConverterData(string? dataTexto)
{
    if (!DateTime.TryParse(dataTexto, out var data))
    {
        return null;
    }

    return DateTime.SpecifyKind(data, DateTimeKind.Utc);
}    public async Task<int> ImportarAnoAsync(
        int ano,
        CancellationToken cancellationToken)
    {
        var jaImportado = await dbContext.Gastos.AnyAsync(
            gasto => gasto.Ano == ano,
            cancellationToken);

        if (jaImportado)
        {
            throw new InvalidOperationException(
                $"O ano {ano} já foi importado.");
        }

        var endereco =
            $"https://www.camara.leg.br/cotas/Ano-{ano}.json.zip";

        using var resposta = await httpClient.GetAsync(
            endereco,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        resposta.EnsureSuccessStatusCode();

        await using var arquivoZip = await resposta.Content.ReadAsStreamAsync(
            cancellationToken);

        using var zip = new ZipArchive(
            arquivoZip,
            ZipArchiveMode.Read);

        var arquivoJson = zip.Entries.FirstOrDefault(
            arquivo => arquivo.Name.EndsWith(".json",
                StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                "O arquivo oficial não contém dados JSON.");

        await using var conteudoJson = arquivoJson.Open();

        var opcoesJson = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var arquivo = await JsonSerializer.DeserializeAsync<RespostaArquivoGastosCamara>(
     conteudoJson,
     opcoesJson,
     cancellationToken)
     ?? throw new InvalidOperationException(
         "Não foi possível ler os gastos do arquivo oficial da Câmara.");

        var quantidadeImportada = 0;
        var lote = new List<Gasto>();

        foreach (var registro in arquivo.Dados)
        {

            if (registro is null || registro.Ano != ano ||
                registro.IdDeputadoCamara == 0)
            {
                continue;
            }

            lote.Add(new Gasto
            {
                IdDeputadoCamara = registro.IdDeputadoCamara,
                Ano = registro.Ano,
                Mes = registro.Mes,
                TipoDespesa = registro.TipoDespesa ?? "Não informado",
                ValorDocumento = registro.ValorDocumento,
                ValorLiquido = registro.ValorLiquido,
                NomeFornecedor = registro.NomeFornecedor ?? "Não informado",
                DataDocumento = ConverterData(registro.DataDocumento),
                UrlDocumento = registro.UrlDocumento
            });

            if (lote.Count < 1_000)
            {
                continue;
            }

            await SalvarLoteAsync(lote, cancellationToken);
            quantidadeImportada += lote.Count;
            lote.Clear();
        }

        if (lote.Count > 0)
        {
            await SalvarLoteAsync(lote, cancellationToken);
            quantidadeImportada += lote.Count;
        }

        logger.LogInformation(
            "{Quantidade} gastos de {Ano} foram importados.",
            quantidadeImportada,
            ano);

        return quantidadeImportada;
    }

    private async Task SalvarLoteAsync(
        List<Gasto> lote,
        CancellationToken cancellationToken)
    {
        dbContext.Gastos.AddRange(lote);
        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }
}
public class RespostaArquivoGastosCamara
{
    [JsonPropertyName("dados")]
    public List<RegistroGastoArquivoCamara> Dados { get; set; } = [];
}
public class RegistroGastoArquivoCamara
{
    [JsonPropertyName("numeroDeputadoID")]
    public int IdDeputadoCamara { get; set; }

    [JsonPropertyName("ano")]
    public int Ano { get; set; }

    [JsonPropertyName("mes")]
    public int Mes { get; set; }

    [JsonPropertyName("descricao")]
    public string? TipoDespesa { get; set; }

    [JsonPropertyName("valorDocumento")]
    public decimal ValorDocumento { get; set; }

    [JsonPropertyName("valorLiquido")]
    public decimal ValorLiquido { get; set; }

    [JsonPropertyName("fornecedor")]
    public string? NomeFornecedor { get; set; }

    [JsonPropertyName("dataEmissao")]
    public string? DataDocumento { get; set; }

    [JsonPropertyName("urlDocumento")]
    public string? UrlDocumento { get; set; }
}