using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Data;
using TransparenciaBot.Models;

namespace TransparenciaBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensagensController : ControllerBase
    {
    private readonly AppDbContext _context;

            public MensagensController(AppDbContext context)
        {
            _context = context;
        }
             [HttpPost]
             public async Task<IActionResult> PostMensagem(Mensagem mensagem)
          {
            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();
            return Ok(new { status = "Alerta registrado!", id = mensagem.Id });
        }
        [HttpGet]
            public async Task<ActionResult<IEnumerable<Mensagem>>> GetMensagens()
        {
            return await _context.Mensagens.ToListAsync();
        }
    }
}