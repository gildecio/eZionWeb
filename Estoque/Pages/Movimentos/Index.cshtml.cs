using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eZionWeb.Configuracoes.Services;
using eZionWeb.Configuracoes.Models;

namespace eZionWeb.Estoque.Pages.Movimentos;

public class IndexModel : PageModel
{
    private readonly IMovimentoRepository _repo;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IDocumentoService _docs;
    private readonly ISequenciaRepository _seqs;

    public List<Linha> Itens { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Produtos { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Locais { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Tipos { get; set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? De { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? Ate { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? ProdutoId { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? LocalId { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public TipoMovimento? Tipo { get; set; }

    public IndexModel(IMovimentoRepository repo, IProdutoRepository produtos, ILocalEstoqueRepository locais, IDocumentoService docs, ISequenciaRepository seqs)
    {
        _repo = repo;
        _produtos = produtos;
        _locais = locais;
        _docs = docs;
        _seqs = seqs;
    }

    public void OnGet()
    {
        var prods = _produtos.GetAll().ToDictionary(p => p.Id, p => p.Nome);
        var locs = _locais.GetAll().ToDictionary(l => l.Id, l => l.Nome);

        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
        Tipos = new()
        {
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = ((int)TipoMovimento.Entrada).ToString(), Text = "Entrada" },
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = ((int)TipoMovimento.Saida).ToString(), Text = "Saída" },
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = ((int)TipoMovimento.Transferencia).ToString(), Text = "Transferência" },
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = ((int)TipoMovimento.Ajuste).ToString(), Text = "Ajuste" }
        };

        var query = _repo.GetAll(ProdutoId);
        if (Tipo.HasValue) query = query.Where(m => m.Tipo == Tipo.Value);
        if (LocalId.HasValue) query = query.Where(m => (m.LocalOrigemId.HasValue && m.LocalOrigemId.Value == LocalId.Value) || (m.LocalDestinoId.HasValue && m.LocalDestinoId.Value == LocalId.Value));
        if (De.HasValue) query = query.Where(m => m.Data >= De.Value);
        if (Ate.HasValue) query = query.Where(m => m.Data <= Ate.Value);

        var allDocs = _docs.GetAll().ToList();
        var docTipos = allDocs.ToDictionary(d => d.Id, d => d switch
        {
            AjusteEstoque => "Ajuste",
            RequisicaoEstoque => "Requisição",
            DevolucaoEstoque => "Devolução",
            TransferenciaEstoque => "Transferência",
            _ => ""
        });

        var docNumeros = new Dictionary<int, string>();
        foreach (var d in allDocs)
        {
            var tipoDocKey = d switch
            {
                AjusteEstoque => "Ajuste",
                RequisicaoEstoque => "Requisicao",
                DevolucaoEstoque => "Devolucao",
                TransferenciaEstoque => "Transferencia",
                _ => string.Empty
            };
            var seq = !string.IsNullOrEmpty(tipoDocKey) ? _seqs.GetByDocSerie(tipoDocKey, d.Serie) : null;
            var display = seq != null && seq.Tipo == TipoSequencia.Anual
                ? ($"{d.Numero}/{d.Serie}")
                : ($"{d.Serie}{d.Numero}");
            docNumeros[d.Id] = display;
        }

        Itens = query.Select(m => new Linha
        {
            Data = m.Data,
            ProdutoNome = prods.TryGetValue(m.ProdutoId, out var pn) ? pn : m.ProdutoId.ToString(),
            TipoTexto = TipoToText(m.Tipo),
            LocalNome = m.Tipo == TipoMovimento.Transferencia
                ? string.Join(" → ",
                    new[]
                    {
                        m.LocalOrigemId.HasValue && locs.TryGetValue(m.LocalOrigemId.Value, out var on) ? on : null,
                        m.LocalDestinoId.HasValue && locs.TryGetValue(m.LocalDestinoId.Value, out var dn) ? dn : null
                    }.Where(x => !string.IsNullOrEmpty(x)))
                : m.Tipo == TipoMovimento.Entrada
                    ? (m.LocalDestinoId.HasValue && locs.TryGetValue(m.LocalDestinoId.Value, out var ld) ? ld : string.Empty)
                    : (m.LocalOrigemId.HasValue && locs.TryGetValue(m.LocalOrigemId.Value, out var lo) ? lo : string.Empty),
            DocumentoTipo = docTipos.TryGetValue(m.DocumentoId, out var dt) ? dt : string.Empty,
            DocumentoNumero = docNumeros.TryGetValue(m.DocumentoId, out var dnFmt) ? dnFmt : string.Empty,
            Quantidade = m.Quantidade
        }).Take(500).ToList();
    }

    public class Linha
    {
        public DateTime Data { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string TipoTexto { get; set; } = string.Empty;
        public string LocalNome { get; set; } = string.Empty;
        public string DocumentoTipo { get; set; } = string.Empty;
        public string DocumentoNumero { get; set; } = string.Empty;
        public string DocumentoSerie { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
    }

    private string TipoToText(TipoMovimento t)
    {
        return t switch
        {
            TipoMovimento.Entrada => "Entrada",
            TipoMovimento.Saida => "Saída",
            TipoMovimento.Transferencia => "Transferência",
            TipoMovimento.Ajuste => "Ajuste",
            _ => t.ToString()
        };
    }
}