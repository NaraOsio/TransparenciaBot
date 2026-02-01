using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Data;
using TransparenciaBot.Models;

namespace TransparenciaBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GastosController : ControllerBase
    {
         private readonly AppDbContext _context;

            public GastosController(AppDbContext context)
        {
            _context = context;
        }
          [HttpPost]
            public async Task<IActionResult> PostGasto(Gasto gasto)
           { 
            _context.Gastos.Add(gasto);
            await _context.SaveChangesAsync();
            return Ok(gasto);
        }
         [HttpGet]
             public async Task<ActionResult<IEnumerable<Gasto>>> GetGastos()
        {
            return await _context.Gastos.ToListAsync();
        }
        [HttpGet("enviar-alerta/{politicoId}")]
        public async Task<IActionResult> EnviarAlertaGasto(int politicoId)
        {
            var ultimoGasto = await _context.Gastos
                .Where(g => g.PoliticoId == politicoId)
                .OrderByDescending(g => g.Data)
                .FirstOrDefaultAsync();

            if (ultimoGasto == null)
            {
                return NotFound("Nenhum gasto encontrado.");
            }

            string mensagemFormatada = $"ALERTA DE TRANSPARÊNCIA\n\n" +
                                       $"O político ID {politicoId} registrou um novo gasto:\n" +
                                       $"Descrição: {ultimoGasto.Descricao}\n" +
                                       $"Valor: R$ {ultimoGasto.Valor}\n" +
                                       $"Data: {ultimoGasto.Data:dd/MM/yyyy}";

            return Ok(new { textoParaWhatsApp = mensagemFormatada });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGasto(int id)
        {
            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto == null) return NotFound();

            _context.Gastos.Remove(gasto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}