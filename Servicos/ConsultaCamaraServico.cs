using System.Net.Http.Json;

namespace TransparenciaBot.Servicos;

public class ConsultaCamaraServico(HttpClient httpClient)
{
    public async Task<DeputadoCamara?> BuscarDeputadoPorNomeAsync(
        string nome,
        CancellationToken cancellationToken)
    {
        var endereco =
            $"api/v2/deputados?nome={Uri.EscapeDataString(nome)}&ordem=ASC&ordenarPor=nome&itens=10";

        var resposta = await httpClient.GetFromJsonAsync<RespostaCamara<DeputadoCamara>>(
            endereco,
            cancellationToken);

        return resposta?.Dados
            .FirstOrDefault(deputado =>
                string.Equals(deputado.Nome, nome, StringComparison.OrdinalIgnoreCase))
            ?? resposta?.Dados.FirstOrDefault();
    }

    public async Task<List<DespesaCamara>> BuscarDespesasAsync(
        int deputadoId,
        int ano,
        CancellationToken cancellationToken)
    {
        var todasAsDespesas = new List<DespesaCamara>();
        var pagina = 1;

        while (true)
        {
            var endereco =
                $"api/v2/deputados/{deputadoId}/despesas?ano={ano}&ordem=ASC&ordenarPor=mes&itens=100&pagina={pagina}";

            var resposta = await httpClient.GetFromJsonAsync<RespostaCamara<DespesaCamara>>(
                endereco,
                cancellationToken);

            var despesasDaPagina = resposta?.Dados ?? [];

            if (despesasDaPagina.Count == 0)
            {
                break;
            }

            todasAsDespesas.AddRange(despesasDaPagina);

            if (despesasDaPagina.Count < 100)
            {
                break;
            }

            pagina++;
        }

        return todasAsDespesas;
    }
}

public class RespostaCamara<T>
{
    public List<T> Dados { get; set; } = [];
}

public class DeputadoCamara
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string SiglaPartido { get; set; } = string.Empty;
    public string SiglaUf { get; set; } = string.Empty;
    public string UrlFoto { get; set; } = string.Empty;
}

public class DespesaCamara
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string TipoDespesa { get; set; } = string.Empty;
    public DateTime? DataDocumento { get; set; }
    public decimal ValorDocumento { get; set; }
    public decimal ValorLiquido { get; set; }
    public string NomeFornecedor { get; set; } = string.Empty;
    public string UrlDocumento { get; set; } = string.Empty;
}