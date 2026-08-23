using System.Globalization;

namespace TransparenciaBot.Servicos;

public class RespostaConsultaServico(
    InterpretadorConsultaServico interpretadorConsultaServico,
    ConsultaCamaraServico consultaCamaraServico,
    ConsultaGastosBancoServico consultaGastosBancoServico,
    FormatadorRespostaGastosServico formatadorRespostaGastosServico)
{
    public async Task<string> CriarRespostaAsync(
        string texto,
        CancellationToken cancellationToken)
    {
        var consulta = interpretadorConsultaServico.Interpretar(texto);

        if (consulta.Tipo == TipoConsultaRegras.Ajuda)
        {
            return """
                Olá! Você pode consultar:

                - Erika Hilton
                - dados do deputado Erika Hilton
                - gastos do deputado Erika Hilton em 2025
                - gastos do deputado Erika Hilton

                Envie somente o nome para ver dados e um resumo dos gastos.
                Para ver despesas detalhadas, informe "gastos" e, se desejar, o ano.
                """;
        }

        if (consulta.Tipo == TipoConsultaRegras.ResumoDeputado)
        {
            var deputado = await consultaCamaraServico.BuscarDeputadoPorNomeAsync(
                consulta.NomeDeputado!,
                cancellationToken);

            if (deputado is null)
            {
                return "Não encontrei esse deputado na fonte pública da Câmara.";
            }

            var ano = await consultaGastosBancoServico.ObterUltimoAnoDisponivelAsync(
                deputado.Nome,
                cancellationToken);

            if (ano is null)
            {
                return $"""
                    Resumo do deputado:

                    Nome: {deputado.Nome}
                    Partido: {deputado.SiglaPartido}
                    UF: {deputado.SiglaUf}

                    Não foram encontrados gastos na base disponível.

                    Fonte: API de Dados Abertos e arquivo anual oficial de cotas da Câmara dos Deputados.
                    """;
            }

            var resumo = await consultaGastosBancoServico.ConsultarAsync(
                deputado.Nome,
                ano.Value,
                cancellationToken);

            if (resumo is null)
            {
                return $"""
                    Resumo do deputado:

                    Nome: {deputado.Nome}
                    Partido: {deputado.SiglaPartido}
                    UF: {deputado.SiglaUf}

                    Não foram encontrados gastos para {ano.Value} na base disponível.

                    Fonte: API de Dados Abertos e arquivo anual oficial de cotas da Câmara dos Deputados.
                    """;
            }

            var culturaBrasileira = CultureInfo.GetCultureInfo("pt-BR");

            return $"""
                Resumo do deputado:

                Nome: {deputado.Nome}
                Partido: {deputado.SiglaPartido}
                UF: {deputado.SiglaUf}

                Gastos em {resumo.Ano}:
                Quantidade de gastos: {resumo.QuantidadeDeGastos:N0}
                Total gasto: {resumo.TotalGasto.ToString("C", culturaBrasileira)}

                Para ver as maiores despesas, envie:
                gastos do deputado {deputado.Nome} em {resumo.Ano}

                Fonte: API de Dados Abertos e arquivo anual oficial de cotas da Câmara dos Deputados.
                """;
        }

        if (consulta.Tipo == TipoConsultaRegras.DadosDeputado)
        {
            var deputado = await consultaCamaraServico.BuscarDeputadoPorNomeAsync(
                consulta.NomeDeputado!,
                cancellationToken);

            if (deputado is null)
            {
                return "Não encontrei esse deputado na fonte pública da Câmara.";
            }

            return $"""
                Dados do deputado:

                Nome: {deputado.Nome}
                Partido: {deputado.SiglaPartido}
                UF: {deputado.SiglaUf}

                Fonte: API de Dados Abertos da Câmara dos Deputados.
                """;
        }

        if (consulta.Tipo == TipoConsultaRegras.GastosDeputado)
        {
            var deputado = await consultaCamaraServico.BuscarDeputadoPorNomeAsync(
                consulta.NomeDeputado!,
                cancellationToken);

            if (deputado is null)
            {
                return "Não encontrei esse deputado na fonte pública da Câmara.";
            }

            var ano = consulta.Ano ??
                await consultaGastosBancoServico.ObterUltimoAnoDisponivelAsync(
                    deputado.Nome,
                    cancellationToken);

            if (ano is null)
            {
                return $"""
                    Não foram encontrados gastos de {deputado.Nome}
                    na base local.

                    Fonte: arquivo anual oficial de cotas da Câmara dos Deputados.
                    """;
            }

            var resumo = await consultaGastosBancoServico.ConsultarAsync(
                deputado.Nome,
                ano.Value,
                cancellationToken);

            if (resumo is null)
            {
                return $"""
                    Não foram encontrados gastos de {deputado.Nome}
                    para o ano de {ano.Value} na base local.

                    Fonte: arquivo anual oficial de cotas da Câmara dos Deputados.
                    """;
            }

            return formatadorRespostaGastosServico.CriarMensagem(resumo);
        }

        return """
            Não consegui identificar sua consulta.

            Digite AJUDA para ver os exemplos de comandos disponíveis.
            """;
    }
}