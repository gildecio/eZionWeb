namespace eZionWeb.Estoque.Models;

public enum TipoProduto
{
    MateriaPrima = 1,
    ProdutoAcabado = 2,
    ProdutoSemiacabado = 3,
    Embalagem = 4,
    Servico = 5,
    Imobilizado = 6,
    Outros = 7
}

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int? GrupoId { get; set; }
    public TipoProduto Tipo { get; set; } = TipoProduto.MateriaPrima;
    public string Codigo { get; set; } = string.Empty;
    public string CodigoAnterior { get; set; } = string.Empty;
    public int UnidadeId { get; set; }
}