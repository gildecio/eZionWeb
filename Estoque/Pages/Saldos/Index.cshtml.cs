using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Saldos;

public class IndexModel : PageModel
{
    private readonly IMovimentoRepository _movs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;

    public List<Linha> Itens { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Produtos { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Locais { get; set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? ProdutoId { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? LocalId { get; set; }

    public IndexModel(IMovimentoRepository movs, IProdutoRepository produtos, ILocalEstoqueRepository locais)
    {
        _movs = movs;
        _produtos = produtos;
        _locais = locais;
    }

    public void OnGet()
    {
        var prods = _produtos.GetAll().ToDictionary(p => p.Id, p => p.Nome);
        var locs = _locais.GetAll().ToDictionary(l => l.Id, l => l.Nome);
        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
        var itens = new List<Linha>();
        IEnumerable<int> produtosAlvo;
        if (ProdutoId.HasValue)
        {
            produtosAlvo = new[] { ProdutoId.Value };
        }
        else
        {
            produtosAlvo = prods.Keys;
        }
        foreach (var p in produtosAlvo)
        {
            var saldos = _movs.GetSaldosPorProduto(p);
            if (LocalId.HasValue) saldos = saldos.Where(x => x.localId == LocalId.Value);
            foreach (var s in saldos)
            {
                var pn = prods.TryGetValue(p, out var _pn) ? _pn : p.ToString();
                var ln = locs.TryGetValue(s.localId, out var _ln) ? _ln : s.localId.ToString();
                itens.Add(new Linha { ProdutoNome = pn, LocalNome = ln, Saldo = s.qtd });
            }
        }
        Itens = itens.OrderBy(i => i.ProdutoNome).ThenBy(i => i.LocalNome).ToList();
    }

    public class Linha
    {
        public string ProdutoNome { get; set; } = string.Empty;
        public string LocalNome { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}