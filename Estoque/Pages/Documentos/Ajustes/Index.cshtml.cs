using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Ajustes;

public class IndexModel : PageModel
{
    private readonly IDocumentoService _docs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IUnidadeRepository _unidades;

    public List<Linha> Itens { get; set; } = new();
    public List<SelectListItem> Produtos { get; set; } = new();
    public List<SelectListItem> Locais { get; set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? De { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? Ate { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? ProdutoId { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? LocalId { get; set; }

    public IndexModel(IDocumentoService docs, IProdutoRepository produtos, ILocalEstoqueRepository locais, IUnidadeRepository unidades)
    {
        _docs = docs;
        _produtos = produtos;
        _locais = locais;
        _unidades = unidades;
    }

    public void OnGet()
    {
        var prods = _produtos.GetAll().ToDictionary(p => p.Id, p => p.Nome);
        var locs = _locais.GetAll().ToDictionary(l => l.Id, l => l.Nome);
        var uns = _unidades.GetAll().ToDictionary(u => u.Id, u => u.Sigla);

        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();

        var query = _docs.GetAjustes().AsEnumerable();
        if (ProdutoId.HasValue) query = query.Where(d => d.ProdutoId == ProdutoId.Value);
        if (LocalId.HasValue) query = query.Where(d => d.LocalId == LocalId.Value);
        if (De.HasValue) query = query.Where(d => d.Data >= De.Value);
        if (Ate.HasValue) query = query.Where(d => d.Data <= Ate.Value);

        Itens = query.Select(d => new Linha
        {
            Id = d.Id,
            Data = d.Data,
            ProdutoNome = prods.TryGetValue(d.ProdutoId, out var pn) ? pn : d.ProdutoId.ToString(),
            LocalNome = locs.TryGetValue(d.LocalId, out var ln) ? ln : d.LocalId.ToString(),
            TipoTexto = d.Entrada ? "Entrada" : "Saída",
            Quantidade = d.Quantidade,
            UnidadeSigla = uns.TryGetValue(d.UnidadeId, out var us) ? us : d.UnidadeId.ToString(),
            Observacao = d.Observacao
        }).Take(500).ToList();
    }

    public class Linha
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string LocalNome { get; set; } = string.Empty;
        public string TipoTexto { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public string UnidadeSigla { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
    }
}