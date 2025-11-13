namespace eZionWeb.Estoque.Models;

public class UnidadeMedida
{
    public int Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

public class ConversaoUnidade
{
    public int Id { get; set; }
    public int DeUnidadeId { get; set; }
    public int ParaUnidadeId { get; set; }
    public decimal Fator { get; set; }
}