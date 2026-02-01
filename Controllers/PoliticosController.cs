using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Data;
using TransparenciaBot.Models;

namespace TransparenciaBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliticosController : ControllerBase
{
    private readonly AppDbContext _context;

       public PoliticosController(AppDbContext context)
      {
        _context = context;
    }
    [HttpGet]
    public IActionResult Get()
    {
        var lista = _context.Politicos.Include(p => p.Gastos).ToList();
        return Ok(lista);
    }
    [HttpGet("{id}/resumo")]
    public IActionResult GetResumo(int id)
    {
        var politico = _context.Politicos
                                .Include(p => p.Gastos)
                                .FirstOrDefault(p => p.Id == id);

        if (politico == null) 
            return NotFound(new { mensagem = "Político não encontrado!" });

        var totalGasto = politico.Gastos.Sum(g => g.Valor);
        return Ok(new {
            nome = politico.Nome,
            totalGasto = totalGasto,
            status = totalGasto > 5000 ? "Gasto" : "Normal"
        });
    }
    [HttpPost]
    public IActionResult Post([FromBody] Politico novoPolitico)
    {
        _context.Politicos.Add(novoPolitico);
        _context.SaveChanges();
        return Ok(novoPolitico);
    }
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Politico politicoAtualizado)
    {
        if (id != politicoAtualizado.Id) 
            return BadRequest(new { mensagem = "ID incorreto." });

        _context.Entry(politicoAtualizado).State = EntityState.Modified;
        _context.SaveChanges();
        return NoContent(); 
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var politico = _context.Politicos.Find(id);
        if (politico == null) return NotFound();

        _context.Politicos.Remove(politico);
        _context.SaveChanges();
        return Ok(new { mensagem = "Removido com sucesso!" });
    }
}