namespace eZionWeb.Estoque.Models;

public enum TipoMovimento
{
    Entrada = 1,
    Saida = 2,
    Transferencia = 3,
    Ajuste = 4
}

public class MovimentoEstoque
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int? LocalOrigemId { get; set; }
    public int? LocalDestinoId { get; set; }
    public TipoMovimento Tipo { get; set; }
    public decimal Quantidade { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string Observacao { get; set; } = string.Empty;
    public int DocumentoId { get; set; }
    public int? MovimentoOrigemId { get; set; }
}