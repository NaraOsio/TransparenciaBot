using System.Text.RegularExpressions;

namespace TransparenciaBot.Servicos;

public class InterpretadorConsultaServico
{
    public ResultadoInterpretacaoRegras Interpretar(string texto)
    {
        var mensagem = texto.Trim();

        if (string.Equals(
                mensagem,
                "AJUDA",
                StringComparison.OrdinalIgnoreCase))
        {
            return new ResultadoInterpretacaoRegras
            {
                Tipo = TipoConsultaRegras.Ajuda
            };
        }

        var dadosDeputado = Regex.Match(
            mensagem,
            @"^dados do deputado\s+(.+)$",
            RegexOptions.IgnoreCase);

        if (dadosDeputado.Success)
        {
            return new ResultadoInterpretacaoRegras
            {
                Tipo = TipoConsultaRegras.DadosDeputado,
                NomeDeputado = LimparNomeDeputado(
                    dadosDeputado.Groups[1].Value)
            };
        }

        var gastosDeputado = Regex.Match(
            mensagem,
            @"^gastos do deputado\s+(.+?)(?:\s+em\s+(\d{4}))?$",
            RegexOptions.IgnoreCase);

        if (gastosDeputado.Success)
        {
            int? ano = null;

            if (gastosDeputado.Groups[2].Success)
            {
                ano = int.Parse(gastosDeputado.Groups[2].Value);
            }

            return new ResultadoInterpretacaoRegras
            {
                Tipo = TipoConsultaRegras.GastosDeputado,
                NomeDeputado = LimparNomeDeputado(
                    gastosDeputado.Groups[1].Value),
                Ano = ano
            };
        }

        var somenteNomeDeputado = Regex.Match(
            mensagem,
            @"^[\p{L}]{2,}(?:[\s'-][\p{L}]{2,}){1,4}(?:\s*\([A-Za-z]{2,10}(?:-[A-Za-z]{2,3})?\)|\s*[-–]\s*[A-Za-z]{2,10}(?:-[A-Za-z]{2,3})?)?$",
            RegexOptions.IgnoreCase);

        if (somenteNomeDeputado.Success)
        {
            return new ResultadoInterpretacaoRegras
            {
                Tipo = TipoConsultaRegras.ResumoDeputado,
                NomeDeputado = LimparNomeDeputado(mensagem)
            };
        }

        return new ResultadoInterpretacaoRegras
        {
            Tipo = TipoConsultaRegras.NaoIdentificada
        };
    }

    private static string LimparNomeDeputado(string nome)
    {
        var nomeSemPartido = Regex.Replace(
            nome.Trim(),
            @"\s*(?:\([A-Za-z]{2,10}(?:-[A-Za-z]{2,3})?\)|[-–]\s*[A-Za-z]{2,10}(?:-[A-Za-z]{2,3})?|\s+[A-Z]{2,5})$",
            string.Empty);

        return nomeSemPartido.Trim();
    }
}

public class ResultadoInterpretacaoRegras
{
    public TipoConsultaRegras Tipo { get; init; }

    public string? NomeDeputado { get; init; }

    public int? Ano { get; init; }
}

public enum TipoConsultaRegras
{
    NaoIdentificada,
    Ajuda,
    DadosDeputado,
    GastosDeputado,
    ResumoDeputado
}