using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Dados;

namespace TransparenciaBot.Servicos;

public class ConsultaGastosBancoServico(
    TransparenciaBotDbContext dbContext)
{
    public async Task<ResumoGastosDeputado?> ConsultarAsync(
        int deputadoId,
        int ano,
        CancellationToken cancellationToken)
    {
        var consulta = dbContext.Gastos
            .AsNoTracking()
            .Where(gasto =>
                gasto.IdDeputadoCamara == deputadoId &&
                gasto.Ano == ano);

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
            DeputadoId = deputadoId,
            Ano = ano,
            QuantidadeDeGastos = quantidadeDeGastos,
            TotalGasto = totalGasto,
            MaioresDespesas = maioresDespesas
        };
    }
}

public class ResumoGastosDeputado
{
    public int DeputadoId { get; set; }
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