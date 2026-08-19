using System.Globalization;
using System.Text;

namespace TransparenciaBot.Servicos;

public class FormatadorRespostaGastosServico
{
    public string CriarMensagem(ResumoGastosDeputado resumo)
    {
        var culturaBrasileira = CultureInfo.GetCultureInfo("pt-BR");

        var mensagem = new StringBuilder();

        mensagem.AppendLine($"Consulta de gastos - {resumo.Ano}");
        mensagem.AppendLine(
            $"Quantidade de gastos: {resumo.QuantidadeDeGastos:N0}");
        mensagem.AppendLine(
            $"Total gasto: {resumo.TotalGasto.ToString("C", culturaBrasileira)}");
        mensagem.AppendLine();
        mensagem.AppendLine("10 maiores despesas encontradas:");

        foreach (var despesa in resumo.MaioresDespesas)
        {
            mensagem.AppendLine(
                $"- Mês {despesa.Mes}: {despesa.TipoDespesa}");
            mensagem.AppendLine(
                $"  Fornecedor: {despesa.Fornecedor}");
            mensagem.AppendLine(
                $"  Valor: {despesa.ValorLiquido.ToString("C", culturaBrasileira)}");
        }

        mensagem.AppendLine();
        mensagem.AppendLine(
            "Fonte: Câmara dos Deputados - arquivo anual oficial de cotas parlamentares.");

        return mensagem.ToString();
    }
}