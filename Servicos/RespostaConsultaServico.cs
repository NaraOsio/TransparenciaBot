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

                - dados do deputado Erika Hilton
                - gastos do deputado Erika Hilton em 2025
                - gastos do deputado Erika Hilton

                Informe o nome do deputado e, para gastos, o ano desejado.
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