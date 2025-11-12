namespace eZionWeb.Estoque.Models;

public class Grupo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string ContaCredito { get; set; } = string.Empty;
    public string ContaDebito { get; set; } = string.Empty;
}