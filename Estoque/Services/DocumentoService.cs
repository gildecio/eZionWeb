using eZionWeb.Estoque.Models;
using eZionWeb.Configuracoes.Services;

namespace eZionWeb.Estoque.Services;

public class DocumentoService : IDocumentoService
{
    private readonly IMovimentoRepository _movs;
    private readonly IProdutoRepository _produtos;
    private readonly IConversaoRepository _conversor;
    private readonly ISequenciaRepository _seqs;
    private readonly List<DocumentoBase> _docs = new();
    private int _nextId = 1;

    public DocumentoService(IMovimentoRepository movs, IProdutoRepository produtos, IConversaoRepository conversor, ISequenciaRepository seqs)
    {
        _movs = movs;
        _produtos = produtos;
        _conversor = conversor;
        _seqs = seqs;
    }

    public IEnumerable<DocumentoBase> GetAll() => _docs.OrderByDescending(d => d.Data).ToList();
    public IEnumerable<AjusteEstoque> GetAjustes() => _docs.OfType<AjusteEstoque>().OrderByDescending(d => d.Data).ToList();
    public IEnumerable<RequisicaoEstoque> GetRequisicoes() => _docs.OfType<RequisicaoEstoque>().OrderByDescending(d => d.Data).ToList();
    public IEnumerable<DevolucaoEstoque> GetDevolucoes() => _docs.OfType<DevolucaoEstoque>().OrderByDescending(d => d.Data).ToList();
    public IEnumerable<TransferenciaEstoque> GetTransferencias() => _docs.OfType<TransferenciaEstoque>().OrderByDescending(d => d.Data).ToList();

    public AjusteEstoque AddAjuste(AjusteEstoque doc)
    {
        doc.Data = doc.Data.Date;
        EnsureNumero(doc, "Ajuste");
        doc.Id = _nextId++;
        _docs.Add(doc);
        if (doc.Itens != null && doc.Itens.Count > 0)
        {
            foreach (var it in doc.Itens)
            {
                var prodIt = _produtos.GetById(it.ProdutoId);
                if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                if (!doc.Entrada)
                {
                    var saldoIt = _movs.GetSaldo(it.ProdutoId, doc.LocalId);
                    if (qtdBaseIt > saldoIt) throw new InvalidOperationException("Saldo insuficiente para ajuste de saída");
                }
                var movIt = new MovimentoEstoque
                {
                    ProdutoId = it.ProdutoId,
                    Tipo = doc.Entrada ? TipoMovimento.Entrada : TipoMovimento.Saida,
                    LocalOrigemId = doc.Entrada ? null : doc.LocalId,
                    LocalDestinoId = doc.Entrada ? doc.LocalId : null,
                    Quantidade = qtdBaseIt,
                    Data = doc.Data,
                    Observacao = "Ajuste: " + doc.Observacao,
                    DocumentoId = doc.Id
                };
                var addedIt = _movs.Add(movIt);
                doc.MovimentoIds.Add(addedIt.Id);
            }
            doc.MovimentoId = null;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
        else
        {
            var prod = _produtos.GetById(doc.ProdutoId);
            if (prod == null) throw new InvalidOperationException("Produto não encontrado");
            if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
            var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
            if (!doc.Entrada)
            {
                var saldo = _movs.GetSaldo(doc.ProdutoId, doc.LocalId);
                if (qtdBase > saldo) throw new InvalidOperationException("Saldo insuficiente para ajuste de saída");
            }
            var mov = new MovimentoEstoque
            {
                ProdutoId = doc.ProdutoId,
                Tipo = doc.Entrada ? TipoMovimento.Entrada : TipoMovimento.Saida,
                LocalOrigemId = doc.Entrada ? null : doc.LocalId,
                LocalDestinoId = doc.Entrada ? doc.LocalId : null,
                Quantidade = qtdBase,
                Data = doc.Data,
                Observacao = "Ajuste: " + doc.Observacao,
                DocumentoId = doc.Id
            };
            var added = _movs.Add(mov);
            doc.MovimentoId = added.Id;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
    }


    public RequisicaoEstoque AddRequisicao(RequisicaoEstoque doc)
    {
        doc.Data = doc.Data.Date;
        EnsureNumero(doc, "Requisicao");
        doc.Id = _nextId++;
        _docs.Add(doc);
        if (doc.Itens != null && doc.Itens.Count > 0)
        {
            foreach (var it in doc.Itens)
            {
                var prodIt = _produtos.GetById(it.ProdutoId);
                if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                var saldoIt = _movs.GetSaldo(it.ProdutoId, doc.LocalOrigemId);
                if (qtdBaseIt > saldoIt) throw new InvalidOperationException("Saldo insuficiente para requisição");
                var movIt = new MovimentoEstoque
                {
                    ProdutoId = it.ProdutoId,
                    Tipo = TipoMovimento.Saida,
                    LocalOrigemId = doc.LocalOrigemId,
                    Quantidade = qtdBaseIt,
                    Data = doc.Data,
                    Observacao = "Requisição: " + doc.Observacao,
                    DocumentoId = doc.Id
                };
                var addedIt = _movs.Add(movIt);
                doc.MovimentoIds.Add(addedIt.Id);
            }
            doc.MovimentoId = null;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
        else
        {
            var prod = _produtos.GetById(doc.ProdutoId);
            if (prod == null) throw new InvalidOperationException("Produto não encontrado");
            if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
            var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
            var saldo = _movs.GetSaldo(doc.ProdutoId, doc.LocalOrigemId);
            if (qtdBase > saldo) throw new InvalidOperationException("Saldo insuficiente para requisição");
            var mov = new MovimentoEstoque
            {
                ProdutoId = doc.ProdutoId,
                Tipo = TipoMovimento.Saida,
                LocalOrigemId = doc.LocalOrigemId,
                Quantidade = qtdBase,
                Data = doc.Data,
                Observacao = "Requisição: " + doc.Observacao,
                DocumentoId = doc.Id
            };
            var added = _movs.Add(mov);
            doc.MovimentoId = added.Id;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
    }


    public DevolucaoEstoque AddDevolucao(DevolucaoEstoque doc)
    {
        doc.Data = doc.Data.Date;
        EnsureNumero(doc, "Devolucao");
        doc.Id = _nextId++;
        _docs.Add(doc);
        if (doc.Itens != null && doc.Itens.Count > 0)
        {
            foreach (var it in doc.Itens)
            {
                var prodIt = _produtos.GetById(it.ProdutoId);
                if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                var movIt = new MovimentoEstoque
                {
                    ProdutoId = it.ProdutoId,
                    Tipo = TipoMovimento.Entrada,
                    LocalDestinoId = doc.LocalDestinoId,
                    Quantidade = qtdBaseIt,
                    Data = doc.Data,
                    Observacao = "Devolução: " + doc.Observacao,
                    DocumentoId = doc.Id
                };
                var addedIt = _movs.Add(movIt);
                doc.MovimentoIds.Add(addedIt.Id);
            }
            doc.MovimentoId = null;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
        else
        {
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
                Observacao = "Devolução: " + doc.Observacao,
                DocumentoId = doc.Id
            };
            var added = _movs.Add(mov);
            doc.MovimentoId = added.Id;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
    }


    public TransferenciaEstoque AddTransferencia(TransferenciaEstoque doc)
    {
        doc.Data = doc.Data.Date;
        EnsureNumero(doc, "Transferencia");
        doc.Id = _nextId++;
        _docs.Add(doc);
        if (doc.Itens != null && doc.Itens.Count > 0)
        {
            foreach (var it in doc.Itens)
            {
                var prodIt = _produtos.GetById(it.ProdutoId);
                if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                var saldoIt = _movs.GetSaldo(it.ProdutoId, doc.LocalOrigemId);
                if (qtdBaseIt > saldoIt) throw new InvalidOperationException("Saldo insuficiente para transferência");
                var movSaida = new MovimentoEstoque
                {
                    ProdutoId = it.ProdutoId,
                    Tipo = TipoMovimento.Saida,
                    LocalOrigemId = doc.LocalOrigemId,
                    Quantidade = qtdBaseIt,
                    Data = doc.Data,
                    Observacao = "Transferência: " + doc.Observacao,
                    DocumentoId = doc.Id
                };
                var saidaAdded = _movs.Add(movSaida);
                doc.MovimentoIds.Add(saidaAdded.Id);

                var movEntrada = new MovimentoEstoque
                {
                    ProdutoId = it.ProdutoId,
                    Tipo = TipoMovimento.Entrada,
                    LocalDestinoId = doc.LocalDestinoId,
                    Quantidade = qtdBaseIt,
                    Data = doc.Data,
                    Observacao = "Transferência: " + doc.Observacao,
                    DocumentoId = doc.Id
                };
                var entradaAdded = _movs.Add(movEntrada);
                doc.MovimentoIds.Add(entradaAdded.Id);
            }
            doc.MovimentoId = null;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
        else
        {
            var prod = _produtos.GetById(doc.ProdutoId);
            if (prod == null) throw new InvalidOperationException("Produto não encontrado");
            if (!_conversor.ExisteConversao(doc.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
            var qtdBase = _conversor.Converter(doc.UnidadeId, prod.UnidadeId, doc.Quantidade);
            var saldo = _movs.GetSaldo(doc.ProdutoId, doc.LocalOrigemId);
            if (qtdBase > saldo) throw new InvalidOperationException("Saldo insuficiente para transferência");
            var movSaida = new MovimentoEstoque
            {
                ProdutoId = doc.ProdutoId,
                Tipo = TipoMovimento.Saida,
                LocalOrigemId = doc.LocalOrigemId,
                Quantidade = qtdBase,
                Data = doc.Data,
                Observacao = "Transferência: " + doc.Observacao,
                DocumentoId = doc.Id
            };
            var saidaAdded = _movs.Add(movSaida);
            doc.MovimentoIds.Add(saidaAdded.Id);

            var movEntrada = new MovimentoEstoque
            {
                ProdutoId = doc.ProdutoId,
                Tipo = TipoMovimento.Entrada,
                LocalDestinoId = doc.LocalDestinoId,
                Quantidade = qtdBase,
                Data = doc.Data,
                Observacao = "Transferência: " + doc.Observacao,
                DocumentoId = doc.Id
            };
            var entradaAdded = _movs.Add(movEntrada);
            doc.MovimentoIds.Add(entradaAdded.Id);
            doc.MovimentoId = null;
            doc.Status = DocumentoStatus.Efetivado;
            doc.EfetivadoEm = DateTime.UtcNow;
            return doc;
        }
    }


    public void DeleteDocumento(int id)
    {
        var idx = _docs.FindIndex(d => d.Id == id);
        if (idx >= 0)
        {
            var doc = _docs[idx];
            if (doc.MovimentoId.HasValue || doc.MovimentoIds.Count > 0 || doc.Status == DocumentoStatus.Efetivado)
                throw new InvalidOperationException("Documento efetivado não pode ser excluído");
            _docs.RemoveAt(idx);
        }
    }

    public void EstornarDocumento(int id)
    {
        var doc = _docs.FirstOrDefault(d => d.Id == id);
        if (doc == null) throw new InvalidOperationException("Documento não encontrado");
        if (doc.Status != DocumentoStatus.Efetivado) throw new InvalidOperationException("Somente documentos efetivados podem ser estornados");

        switch (doc)
        {
            case AjusteEstoque aj:
                if (aj.Itens != null && aj.Itens.Count > 0)
                {
                    foreach (var it in aj.Itens)
                    {
                        var prodIt = _produtos.GetById(it.ProdutoId);
                        if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                        if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                        var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                        var movRev = new MovimentoEstoque
                        {
                            ProdutoId = it.ProdutoId,
                            Tipo = aj.Entrada ? TipoMovimento.Saida : TipoMovimento.Entrada,
                            LocalOrigemId = aj.Entrada ? aj.LocalId : null,
                            LocalDestinoId = aj.Entrada ? null : aj.LocalId,
                            Quantidade = qtdBaseIt,
                            Data = DateTime.UtcNow,
                            Observacao = "Estorno de ajuste: " + aj.Observacao,
                            DocumentoId = aj.Id
                        };
                        _movs.Add(movRev);
                    }
                }
                else
                {
                    var prod = _produtos.GetById(aj.ProdutoId);
                    if (prod == null) throw new InvalidOperationException("Produto não encontrado");
                    if (!_conversor.ExisteConversao(aj.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                    var qtdBase = _conversor.Converter(aj.UnidadeId, prod.UnidadeId, aj.Quantidade);
                    var movRev = new MovimentoEstoque
                    {
                        ProdutoId = aj.ProdutoId,
                        Tipo = aj.Entrada ? TipoMovimento.Saida : TipoMovimento.Entrada,
                        LocalOrigemId = aj.Entrada ? aj.LocalId : null,
                        LocalDestinoId = aj.Entrada ? null : aj.LocalId,
                        Quantidade = qtdBase,
                        Data = DateTime.UtcNow,
                        Observacao = "Estorno de ajuste: " + aj.Observacao,
                        DocumentoId = aj.Id
                    };
                    _movs.Add(movRev);
                }
                aj.Status = DocumentoStatus.Estornado;
                break;

            case RequisicaoEstoque req:
                if (req.Itens != null && req.Itens.Count > 0)
                {
                    foreach (var it in req.Itens)
                    {
                        var prodIt = _produtos.GetById(it.ProdutoId);
                        if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                        if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                        var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                        var movRev = new MovimentoEstoque
                        {
                            ProdutoId = it.ProdutoId,
                            Tipo = TipoMovimento.Entrada,
                            LocalDestinoId = req.LocalOrigemId,
                            Quantidade = qtdBaseIt,
                            Data = DateTime.UtcNow,
                            Observacao = "Estorno de requisição: " + req.Observacao,
                            DocumentoId = req.Id
                        };
                        _movs.Add(movRev);
                    }
                }
                else
                {
                    var prod = _produtos.GetById(req.ProdutoId);
                    if (prod == null) throw new InvalidOperationException("Produto não encontrado");
                    if (!_conversor.ExisteConversao(req.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                    var qtdBase = _conversor.Converter(req.UnidadeId, prod.UnidadeId, req.Quantidade);
                    var movRev = new MovimentoEstoque
                    {
                        ProdutoId = req.ProdutoId,
                        Tipo = TipoMovimento.Entrada,
                        LocalDestinoId = req.LocalOrigemId,
                        Quantidade = qtdBase,
                        Data = DateTime.UtcNow,
                        Observacao = "Estorno de requisição: " + req.Observacao,
                        DocumentoId = req.Id
                    };
                    _movs.Add(movRev);
                }
                req.Status = DocumentoStatus.Estornado;
                break;

            case DevolucaoEstoque dev:
                if (dev.Itens != null && dev.Itens.Count > 0)
                {
                    foreach (var it in dev.Itens)
                    {
                        var prodIt = _produtos.GetById(it.ProdutoId);
                        if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                        if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                        var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                        var movRev = new MovimentoEstoque
                        {
                            ProdutoId = it.ProdutoId,
                            Tipo = TipoMovimento.Saida,
                            LocalOrigemId = dev.LocalDestinoId,
                            Quantidade = qtdBaseIt,
                            Data = DateTime.UtcNow,
                            Observacao = "Estorno de devolução: " + dev.Observacao,
                            DocumentoId = dev.Id
                        };
                        _movs.Add(movRev);
                    }
                }
                else
                {
                    var prod = _produtos.GetById(dev.ProdutoId);
                    if (prod == null) throw new InvalidOperationException("Produto não encontrado");
                    if (!_conversor.ExisteConversao(dev.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                    var qtdBase = _conversor.Converter(dev.UnidadeId, prod.UnidadeId, dev.Quantidade);
                    var movRev = new MovimentoEstoque
                    {
                        ProdutoId = dev.ProdutoId,
                        Tipo = TipoMovimento.Saida,
                        LocalOrigemId = dev.LocalDestinoId,
                        Quantidade = qtdBase,
                        Data = DateTime.UtcNow,
                        Observacao = "Estorno de devolução: " + dev.Observacao,
                        DocumentoId = dev.Id
                    };
                    _movs.Add(movRev);
                }
                dev.Status = DocumentoStatus.Estornado;
                break;

            case TransferenciaEstoque tr:
                if (tr.Itens != null && tr.Itens.Count > 0)
                {
                    foreach (var it in tr.Itens)
                    {
                        var prodIt = _produtos.GetById(it.ProdutoId);
                        if (prodIt == null) throw new InvalidOperationException("Produto não encontrado");
                        if (!_conversor.ExisteConversao(it.UnidadeId, prodIt.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                        var qtdBaseIt = _conversor.Converter(it.UnidadeId, prodIt.UnidadeId, it.Quantidade);
                        var movEntradaOrigem = new MovimentoEstoque
                        {
                            ProdutoId = it.ProdutoId,
                            Tipo = TipoMovimento.Entrada,
                            LocalDestinoId = tr.LocalOrigemId,
                            Quantidade = qtdBaseIt,
                            Data = DateTime.UtcNow,
                            Observacao = "Estorno de transferência: " + tr.Observacao,
                            DocumentoId = tr.Id
                        };
                        _movs.Add(movEntradaOrigem);

                        var movSaidaDestino = new MovimentoEstoque
                        {
                            ProdutoId = it.ProdutoId,
                            Tipo = TipoMovimento.Saida,
                            LocalOrigemId = tr.LocalDestinoId,
                            Quantidade = qtdBaseIt,
                            Data = DateTime.UtcNow,
                            Observacao = "Estorno de transferência: " + tr.Observacao,
                            DocumentoId = tr.Id
                        };
                        _movs.Add(movSaidaDestino);
                    }
                }
                else
                {
                    var prod = _produtos.GetById(tr.ProdutoId);
                    if (prod == null) throw new InvalidOperationException("Produto não encontrado");
                    if (!_conversor.ExisteConversao(tr.UnidadeId, prod.UnidadeId)) throw new InvalidOperationException("Conversão de unidade não encontrada");
                    var qtdBase = _conversor.Converter(tr.UnidadeId, prod.UnidadeId, tr.Quantidade);
                    var movEntradaOrigem = new MovimentoEstoque
                    {
                        ProdutoId = tr.ProdutoId,
                        Tipo = TipoMovimento.Entrada,
                        LocalDestinoId = tr.LocalOrigemId,
                        Quantidade = qtdBase,
                        Data = DateTime.UtcNow,
                        Observacao = "Estorno de transferência: " + tr.Observacao,
                        DocumentoId = tr.Id
                    };
                    _movs.Add(movEntradaOrigem);

                    var movSaidaDestino = new MovimentoEstoque
                    {
                        ProdutoId = tr.ProdutoId,
                        Tipo = TipoMovimento.Saida,
                        LocalOrigemId = tr.LocalDestinoId,
                        Quantidade = qtdBase,
                        Data = DateTime.UtcNow,
                        Observacao = "Estorno de transferência: " + tr.Observacao,
                        DocumentoId = tr.Id
                    };
                    _movs.Add(movSaidaDestino);
                }
                tr.Status = DocumentoStatus.Estornado;
                break;

            default:
                throw new InvalidOperationException("Tipo de documento não suportado para estorno");
        }
    }

    private void EnsureNumero(DocumentoBase doc, string tipo)
    {
        if (doc.Numero <= 0)
        {
            var res = _seqs.ProximoNumeroComSerie(tipo, doc.Serie, doc.Data);
            doc.Numero = res.numero;
            doc.Serie = res.serieUsada;
        }
    }
}