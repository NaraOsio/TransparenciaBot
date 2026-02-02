namespace TransparenciaBot.Models;

public class Usuario
{
    public int Id { get; set; }
     public string Nome { get; set; } = null!;
     public string ChatId { get; set; } = null!;
   public int? PoliticoId { get; set; }
}