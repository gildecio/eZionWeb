using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public class DocumentoService : IDocumentoService
{
    private readonly IMovimentoRepository _movs;
    private readonly IProdutoRepository _produtos;
    private readonly IConversaoRepository _conversor;
    private readonly List<DocumentoBase> _docs = new();
    private int _nextId = 1;

    public DocumentoService(IMovimentoRepository movs, IProdutoRepository produtos, IConversaoRepository conversor)
    {
        _movs = movs;
        _produtos = produtos;
        _conversor = conversor;
    }

    public IEnumerable<DocumentoBase> GetAll() => _docs.OrderByDescending(d => d.Data).ToList();
    public IEnumerable<AjusteEstoque> GetAjustes() => _docs.OfType<AjusteEstoque>().OrderByDescending(d => d.Data).ToList();
    public IEnumerable<RequisicaoEstoque> GetRequisicoes() => _docs.OfType<RequisicaoEstoque>().OrderByDescending(d => d.Data).ToList();
    public IEnumerable<DevolucaoEstoque> GetDevolucoes() => _docs.OfType<DevolucaoEstoque>().OrderByDescending(d => d.Data).ToList();
    public IEnumerable<TransferenciaEstoque> GetTransferencias() => _docs.OfType<TransferenciaEstoque>().OrderByDescending(d => d.Data).ToList();

    public AjusteEstoque AddAjuste(AjusteEstoque doc)
    {
        doc.Id = _nextId++;
        _docs.Add(doc);
        var prod = _produtos.GetById(doc.ProdutoId);
        if (prod == null) throw new InvalidOperationException("Produto não encontrado");
        if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
        var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
        if (!doc.Entrada)
        {
            var saldo = _movs.GetSaldo(doc.ProdutoId, doc.LocalId);
            if (qtdBase > saldo)
            {
                throw new InvalidOperationException("Saldo insuficiente para ajuste de saída");
            }
        }
        var mov = new MovimentoEstoque
        {
            ProdutoId = doc.ProdutoId,
            Tipo = doc.Entrada ? TipoMovimento.Entrada : TipoMovimento.Saida,
            LocalOrigemId = doc.Entrada ? null : doc.LocalId,
            LocalDestinoId = doc.Entrada ? doc.LocalId : null,
            Quantidade = qtdBase,
            Data = doc.Data,
            Observacao = "Ajuste: " + doc.Observacao
        };
        var added = _movs.Add(mov);
        doc.MovimentoId = added.Id;
        return doc;
    }

    public AjusteEstoque UpdateAjuste(AjusteEstoque doc)
    {
        var existenteIdx = _docs.FindIndex(d => d is AjusteEstoque a && a.Id == doc.Id);
        if (existenteIdx < 0) throw new InvalidOperationException("Documento não encontrado");
        var existente = (AjusteEstoque)_docs[existenteIdx];
        if (existente.MovimentoId.HasValue)
        {
            _movs.Delete(existente.MovimentoId.Value);
            existente.MovimentoId = null;
        }
        existente.ProdutoId = doc.ProdutoId;
        existente.LocalId = doc.LocalId;
        existente.UnidadeId = doc.UnidadeId;
        existente.Quantidade = doc.Quantidade;
        existente.Entrada = doc.Entrada;
        existente.Observacao = doc.Observacao;
        existente.Data = DateTime.UtcNow;

        var prod = _produtos.GetById(existente.ProdutoId);
        if (prod == null) throw new InvalidOperationException("Produto não encontrado");
        if (!_conversor.ExisteConversao(existente.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
        var qtdBase = _conversor.Converter(existente.UnidadeId, prod.UnidadeId, existente.Quantidade);
        if (!existente.Entrada)
        {
            var saldo = _movs.GetSaldo(existente.ProdutoId, existente.LocalId);
            if (qtdBase > saldo)
            {
                throw new InvalidOperationException("Saldo insuficiente para ajuste de saída");
            }
        }
        var mov = new MovimentoEstoque
        {
            ProdutoId = existente.ProdutoId,
            Tipo = existente.Entrada ? TipoMovimento.Entrada : TipoMovimento.Saida,
            LocalOrigemId = existente.Entrada ? null : existente.LocalId,
            LocalDestinoId = existente.Entrada ? existente.LocalId : null,
            Quantidade = qtdBase,
            Data = existente.Data,
            Observacao = "Ajuste: " + existente.Observacao
        };
        var added = _movs.Add(mov);
        existente.MovimentoId = added.Id;
        return existente;
    }

    public RequisicaoEstoque AddRequisicao(RequisicaoEstoque doc)
    {
        doc.Id = _nextId++;
        _docs.Add(doc);
        var prod = _produtos.GetById(doc.ProdutoId);
        if (prod == null) throw new InvalidOperationException("Produto não encontrado");
        if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
        var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
        var saldo = _movs.GetSaldo(doc.ProdutoId, doc.LocalOrigemId);
        if (qtdBase > saldo)
        {
            throw new InvalidOperationException("Saldo insuficiente para requisição");
        }
        var mov = new MovimentoEstoque
        {
            ProdutoId = doc.ProdutoId,
            Tipo = TipoMovimento.Saida,
            LocalOrigemId = doc.LocalOrigemId,
            Quantidade = qtdBase,
            Data = doc.Data,
            Observacao = "Requisição: " + doc.Observacao
        };
        var added = _movs.Add(mov);
        doc.MovimentoId = added.Id;
        return doc;
    }

    public DevolucaoEstoque AddDevolucao(DevolucaoEstoque doc)
    {
        doc.Id = _nextId++;
        _docs.Add(doc);
        var prod = _produtos.GetById(doc.ProdutoId);
        if (prod == null) throw new InvalidOperationException("Produto não encontrado");
        if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
        var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
        var mov = new MovimentoEstoque
        {
            ProdutoId = doc.ProdutoId,
            Tipo = TipoMovimento.Entrada,
            LocalDestinoId = doc.LocalDestinoId,
            Quantidade = qtdBase,
            Data = doc.Data,
            Observacao = "Devolução: " + doc.Observacao
        };
        var added = _movs.Add(mov);
        doc.MovimentoId = added.Id;
        return doc;
    }

    public TransferenciaEstoque AddTransferencia(TransferenciaEstoque doc)
    {
        doc.Id = _nextId++;
        _docs.Add(doc);
        var prod = _produtos.GetById(doc.ProdutoId);
        if (prod == null) throw new InvalidOperationException("Produto não encontrado");
        if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
        var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
        var saldo = _movs.GetSaldo(doc.ProdutoId, doc.LocalOrigemId);
        if (qtdBase > saldo)
        {
            throw new InvalidOperationException("Saldo insuficiente para transferência");
        }
        var mov = new MovimentoEstoque
        {
            ProdutoId = doc.ProdutoId,
            Tipo = TipoMovimento.Transferencia,
            LocalOrigemId = doc.LocalOrigemId,
            LocalDestinoId = doc.LocalDestinoId,
            Quantidade = qtdBase,
            Data = doc.Data,
            Observacao = "Transferência: " + doc.Observacao
        };
        var added = _movs.Add(mov);
        doc.MovimentoId = added.Id;
        return doc;
    }

    public void DeleteDocumento(int id)
    {
        var idx = _docs.FindIndex(d => d.Id == id);
        if (idx >= 0)
        {
            var doc = _docs[idx];
            _docs.RemoveAt(idx);
            if (doc.MovimentoId.HasValue)
            {
                _movs.Delete(doc.MovimentoId.Value);
            }
        }
    }
}