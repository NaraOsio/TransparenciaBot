#nullable enable
namespace TransparenciaBot.Models;

public class Gasto
{
    public int Id { get; set; }

    public string Descricao { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int PoliticoId { get; set; }
}