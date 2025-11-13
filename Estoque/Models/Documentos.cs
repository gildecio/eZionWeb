namespace eZionWeb.Estoque.Models;

public abstract class DocumentoBase
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string Observacao { get; set; } = string.Empty;
    public int? MovimentoId { get; set; }
}

public class AjusteEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
    public bool Entrada { get; set; } = true;
}

public class RequisicaoEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalOrigemId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
}

public class DevolucaoEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalDestinoId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
}

public class TransferenciaEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalOrigemId { get; set; }
    public int LocalDestinoId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
}