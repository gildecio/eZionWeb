namespace eZionWeb.Estoque.Models;

public enum DocumentoStatus
{
    Rascunho = 1,
    Efetivado = 2,
    Estornado = 3,
    Cancelado = 4
}

public abstract class DocumentoBase
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string Observacao { get; set; } = string.Empty;
    public int? MovimentoId { get; set; }
    public List<int> MovimentoIds { get; set; } = new();
    public DocumentoStatus Status { get; set; } = DocumentoStatus.Rascunho;
    public DateTime? EfetivadoEm { get; set; }
}

public class AjusteEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
    public bool Entrada { get; set; } = true;
    public List<DocumentoItem> Itens { get; set; } = new();
}

public class RequisicaoEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalOrigemId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
    public List<DocumentoItem> Itens { get; set; } = new();
}

public class DevolucaoEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalDestinoId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
    public List<DocumentoItem> Itens { get; set; } = new();
}

public class TransferenciaEstoque : DocumentoBase
{
    public int ProdutoId { get; set; }
    public int LocalOrigemId { get; set; }
    public int LocalDestinoId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
    public List<DocumentoItem> Itens { get; set; } = new();
}

public class DocumentoItem
{
    public int ProdutoId { get; set; }
    public int UnidadeId { get; set; }
    public decimal Quantidade { get; set; }
}