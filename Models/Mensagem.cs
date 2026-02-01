using System.ComponentModel.DataAnnotations;

namespace TransparenciaBot.Models;

public class Mensagem
{
    public int Id { get; set; }

    [Required]
    public string Conteudo { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; } = DateTime.Now;

    public int UsuarioId { get; set; }
}