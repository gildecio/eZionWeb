using eZionWeb.Estoque.Services;
using eZionWeb.Estoque.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eZionWeb.Configuracoes.Services;
using eZionWeb.Configuracoes.Models;

namespace eZionWeb.Estoque.Pages.Documentos.Devolucoes;

public class IndexModel : PageModel
{
    private readonly IDocumentoService _docs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IUnidadeRepository _unidades;
    private readonly ISequenciaRepository _seqs;

    public List<Linha> Itens { get; set; } = new();
    public List<SelectListItem> Produtos { get; set; } = new();
    public List<SelectListItem> Locais { get; set; } = new();
    public Dictionary<int, List<ItemLinha>> ItensPorDocumento { get; set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? De { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? Ate { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? ProdutoId { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? LocalId { get; set; }

    public IndexModel(IDocumentoService docs, IProdutoRepository produtos, ILocalEstoqueRepository locais, IUnidadeRepository unidades, ISequenciaRepository seqs)
    {
        _docs = docs;
        _produtos = produtos;
        _locais = locais;
        _unidades = unidades;
        _seqs = seqs;
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

        var query = _docs.GetDevolucoes().AsEnumerable();
        if (ProdutoId.HasValue) query = query.Where(d => d.ProdutoId == ProdutoId.Value || (d.Itens != null && d.Itens.Any(it => it.ProdutoId == ProdutoId.Value)));
        if (LocalId.HasValue) query = query.Where(d => d.LocalDestinoId == LocalId.Value);
        if (De.HasValue) query = query.Where(d => d.Data >= De.Value);
        if (Ate.HasValue) query = query.Where(d => d.Data <= Ate.Value);
        var docs = query.ToList();

        ItensPorDocumento = new Dictionary<int, List<ItemLinha>>();
        foreach (var d in docs)
        {
            var lista = new List<ItemLinha>();
            if (d.Itens != null && d.Itens.Count > 0)
            {
                foreach (var it in d.Itens)
                {
                    lista.Add(new ItemLinha
                    {
                        ProdutoNome = prods.TryGetValue(it.ProdutoId, out var pn) ? pn : it.ProdutoId.ToString(),
                        Quantidade = it.Quantidade,
                        UnidadeSigla = uns.TryGetValue(it.UnidadeId, out var us) ? us : it.UnidadeId.ToString()
                    });
                }
            }
            else
            {
                lista.Add(new ItemLinha
                {
                    ProdutoNome = prods.TryGetValue(d.ProdutoId, out var pn) ? pn : d.ProdutoId.ToString(),
                    Quantidade = d.Quantidade,
                    UnidadeSigla = uns.TryGetValue(d.UnidadeId, out var us) ? us : d.UnidadeId.ToString()
                });
            }
            ItensPorDocumento[d.Id] = lista;
        }

        Itens = docs.Select(d =>
        {
            var seq = _seqs.GetByDocSerie("Devolucao", d.Serie);
            var numeroDisplay = seq != null && seq.Tipo == TipoSequencia.Anual
                ? ($"{d.Numero}/{d.Serie}")
                : ($"{d.Serie}{d.Numero}");
            return new Linha
        {
            Id = d.Id,
            Data = d.Data,
            NumeroDisplay = numeroDisplay,
            DestinoNome = locs.TryGetValue(d.LocalDestinoId, out var ln) ? ln : d.LocalDestinoId.ToString(),
            Observacao = d.Observacao,
            Status = d.Status
            };
        }).Take(500).ToList();
    }

    public Microsoft.AspNetCore.Mvc.IActionResult OnPostEstornar(int id)
    {
        _docs.EstornarDocumento(id);
        return RedirectToPage();
    }

    

    public class Linha
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string NumeroDisplay { get; set; } = string.Empty;
        public string DestinoNome { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DocumentoStatus Status { get; set; }
    }

    public class ItemLinha
    {
        public string ProdutoNome { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public string UnidadeSigla { get; set; } = string.Empty;
    }
}