using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Data;
using TransparenciaBot.Models;

namespace TransparenciaBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Usuário cadastrado com sucesso!", dados = usuario });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("rastreadores/{politicoId}")]
        public async Task<IActionResult> GetUsuariosRastreando(int politicoId)
        {
            var assinantes = await _context.Usuarios
                .Where(u => u.PoliticoFavoritoId == politicoId)
                .ToListAsync();

            if (assinantes.Count == 0)
            {
                return NotFound("Nenhum usuário está rastreando este político ainda.");
            }

            return Ok(assinantes);
        }

        [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok("Usuário removido do rastreamento.");
        }
    }
}