using eZionWeb.Estoque.Services;
using eZionWeb.Estoque.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Transferencias;

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

        var query = _docs.GetTransferencias().AsEnumerable();
        if (ProdutoId.HasValue) query = query.Where(d => d.ProdutoId == ProdutoId.Value || (d.Itens != null && d.Itens.Any(it => it.ProdutoId == ProdutoId.Value)));
        if (LocalId.HasValue) query = query.Where(d => d.LocalOrigemId == LocalId.Value || d.LocalDestinoId == LocalId.Value);
        if (De.HasValue) query = query.Where(d => d.Data >= De.Value);
        if (Ate.HasValue) query = query.Where(d => d.Data <= Ate.Value);

        var linhas = new List<Linha>();
        foreach (var d in query)
        {
            if (d.Itens != null && d.Itens.Count > 0)
            {
                foreach (var it in d.Itens)
                {
                    linhas.Add(new Linha
                    {
                        Id = d.Id,
                        Data = d.Data,
                        ProdutoNome = prods.TryGetValue(it.ProdutoId, out var pn) ? pn : it.ProdutoId.ToString(),
                        OrigemNome = locs.TryGetValue(d.LocalOrigemId, out var on) ? on : d.LocalOrigemId.ToString(),
                        DestinoNome = locs.TryGetValue(d.LocalDestinoId, out var dn) ? dn : d.LocalDestinoId.ToString(),
                        Quantidade = it.Quantidade,
                        UnidadeSigla = uns.TryGetValue(it.UnidadeId, out var us) ? us : it.UnidadeId.ToString(),
                        Observacao = d.Observacao,
                        Status = d.Status
                    });
                }
            }
            else
            {
                linhas.Add(new Linha
                {
                    Id = d.Id,
                    Data = d.Data,
                    ProdutoNome = prods.TryGetValue(d.ProdutoId, out var pn) ? pn : d.ProdutoId.ToString(),
                    OrigemNome = locs.TryGetValue(d.LocalOrigemId, out var on) ? on : d.LocalOrigemId.ToString(),
                    DestinoNome = locs.TryGetValue(d.LocalDestinoId, out var dn) ? dn : d.LocalDestinoId.ToString(),
                    Quantidade = d.Quantidade,
                    UnidadeSigla = uns.TryGetValue(d.UnidadeId, out var us) ? us : d.UnidadeId.ToString(),
                    Observacao = d.Observacao,
                    Status = d.Status
                });
            }
        }
        Itens = linhas.Take(500).ToList();
    }

    public Microsoft.AspNetCore.Mvc.IActionResult OnPostEstornar(int id)
    {
        _docs.EstornarDocumento(id);
        return RedirectToPage();
    }

    public Microsoft.AspNetCore.Mvc.IActionResult OnPostEfetivar(int id)
    {
        _docs.EfetivarDocumento(id);
        return RedirectToPage();
    }

    public class Linha
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string OrigemNome { get; set; } = string.Empty;
        public string DestinoNome { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public string UnidadeSigla { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DocumentoStatus Status { get; set; }
    }
}