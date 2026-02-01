namespace TransparenciaBot.Models;

public class Politico
{
    public int Id { get; set; }
     public string Nome { get; set; } = string.Empty;
      public string Partido { get; set; } = string.Empty;
     public string Cargo { get; set; } = string.Empty;
       public List<Gasto> Gastos { get; set; } = new List<Gasto>();
}