using Microsoft.AspNetCore.Mvc;
using TransparenciaBot.Servicos;

namespace TransparenciaBot.Controladores;

[ApiController]
[Route("api/camara")]
public class CamaraController(
    ConsultaCamaraServico consultaCamaraServico,
    ConsultaGastosBancoServico consultaGastosBancoServico,
    FormatadorRespostaGastosServico formatadorRespostaGastosServico)
    : ControllerBase
{
    [HttpGet("deputado")]
    public async Task<IActionResult> BuscarDeputado(
        [FromQuery] string nome,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return BadRequest(new
            {
                mensagem = "Informe o nome do deputado."
            });
        }

        var deputado = await consultaCamaraServico.BuscarDeputadoPorNomeAsync(
            nome,
            cancellationToken);

        if (deputado is null)
        {
            return NotFound(new
            {
                mensagem = "Deputado não encontrado na fonte pública da Câmara."
            });
        }

        return Ok(new
        {
            deputado.Id,
            deputado.Nome,
            deputado.SiglaPartido,
            deputado.SiglaUf,
            deputado.UrlFoto,
            fonte = "API de Dados Abertos da Câmara dos Deputados"
        });
    }

    [HttpGet("deputado/{id:int}/despesas")]
    public async Task<IActionResult> BuscarDespesas(
        int id,
        [FromQuery] int ano,
        CancellationToken cancellationToken)
    {
        if (ano < 2008 || ano > DateTime.UtcNow.Year)
        {
            return BadRequest(new
            {
                mensagem = "Informe um ano entre 2008 e o ano atual."
            });
        }

        var deputado = await consultaCamaraServico.BuscarDeputadoPorIdAsync(
            id,
            cancellationToken);

        if (deputado is null)
        {
            return NotFound(new
            {
                mensagem = "Deputado não encontrado na fonte pública da Câmara."
            });
        }

        var resumo = await consultaGastosBancoServico.ConsultarAsync(
            deputado.Nome,
            ano,
            cancellationToken);

        if (resumo is null)
        {
            return NotFound(new
            {
                mensagem = "Não foram encontrados gastos para este deputado nesse ano.",
                deputadoId = id,
                ano,
                fonte = "Arquivo anual oficial de cotas da Câmara dos Deputados"
            });
        }

        var mensagem = formatadorRespostaGastosServico.CriarMensagem(resumo);

        return Ok(new
        {
            mensagem,
            deputadoId = deputado.Id,
            ano = resumo.Ano,
            quantidadeDeDespesas = resumo.QuantidadeDeGastos,
            totalGasto = resumo.TotalGasto,
            despesas = resumo.MaioresDespesas.Select(despesa => new
            {
                despesa.Mes,
                despesa.TipoDespesa,
                nomeFornecedor = despesa.Fornecedor,
                despesa.ValorLiquido,
                despesa.DataDocumento,
                despesa.UrlDocumento
            }),
            fonte = "Arquivo anual oficial de cotas da Câmara dos Deputados"
        });
    }
}