using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Dados;

namespace TransparenciaBot.Servicos;

public class ConsultaGastosBancoServico(
    TransparenciaBotDbContext dbContext)
{
    public async Task<ResumoGastosDeputado?> ConsultarAsync(
        string nomeParlamentar,
        int ano,
        CancellationToken cancellationToken)
    {
        var nomeNormalizado = nomeParlamentar.Trim().ToUpper();

        var consulta = dbContext.Gastos
            .AsNoTracking()
            .Where(gasto =>
                gasto.Ano == ano &&
                gasto.NomeParlamentar.ToUpper() == nomeNormalizado);

        var quantidadeDeGastos = await consulta.CountAsync(
            cancellationToken);

        if (quantidadeDeGastos == 0)
        {
            return null;
        }

        var totalGasto = await consulta.SumAsync(
            gasto => gasto.ValorLiquido,
            cancellationToken);

        var maioresDespesas = await consulta
            .OrderByDescending(gasto => gasto.ValorLiquido)
            .ThenBy(gasto => gasto.Mes)
            .Take(10)
            .Select(gasto => new ItemGastoDeputado
            {
                Mes = gasto.Mes,
                TipoDespesa = gasto.TipoDespesa,
                Fornecedor = gasto.NomeFornecedor,
                ValorLiquido = gasto.ValorLiquido,
                DataDocumento = gasto.DataDocumento,
                UrlDocumento = gasto.UrlDocumento
            })
            .ToListAsync(cancellationToken);

        return new ResumoGastosDeputado
        {
            NomeParlamentar = nomeParlamentar,
            Ano = ano,
            QuantidadeDeGastos = quantidadeDeGastos,
            TotalGasto = totalGasto,
            MaioresDespesas = maioresDespesas
        };
    }

    public async Task<int?> ObterUltimoAnoDisponivelAsync(
        string nomeParlamentar,
        CancellationToken cancellationToken)
    {
        var nomeNormalizado = nomeParlamentar.Trim().ToUpper();

        return await dbContext.Gastos
            .AsNoTracking()
            .Where(gasto =>
                gasto.NomeParlamentar.ToUpper() == nomeNormalizado)
            .MaxAsync(
                gasto => (int?)gasto.Ano,
                cancellationToken);
    }
}

public class ResumoGastosDeputado
{
    public string NomeParlamentar { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int QuantidadeDeGastos { get; set; }
    public decimal TotalGasto { get; set; }
    public List<ItemGastoDeputado> MaioresDespesas { get; set; } = [];
}

public class ItemGastoDeputado
{
    public int Mes { get; set; }
    public string TipoDespesa { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public decimal ValorLiquido { get; set; }
    public DateTime? DataDocumento { get; set; }
    public string? UrlDocumento { get; set; }
}