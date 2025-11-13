using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Ajustes;

public class EditModel : PageModel
{
    private readonly IDocumentoService _docs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IUnidadeRepository _unidades;

    [BindProperty]
    public int Id { get; set; }
    [BindProperty]
    public int ProdutoId { get; set; }
    [BindProperty]
    public int LocalId { get; set; }
    [BindProperty]
    public bool Entrada { get; set; }
    [BindProperty]
    public int UnidadeId { get; set; }
    [BindProperty]
    public decimal Quantidade { get; set; }
    [BindProperty]
    public string Observacao { get; set; } = string.Empty;

    public List<SelectListItem> Produtos { get; set; } = new();
    public List<SelectListItem> Locais { get; set; } = new();
    public List<SelectListItem> Unidades { get; set; } = new();

    public EditModel(IDocumentoService docs, IProdutoRepository produtos, ILocalEstoqueRepository locais, IUnidadeRepository unidades)
    {
        _docs = docs;
        _produtos = produtos;
        _locais = locais;
        _unidades = unidades;
    }

    public IActionResult OnGet(int id)
    {
        var item = _docs.GetAjustes().FirstOrDefault(d => d.Id == id);
        if (item == null) return RedirectToPage("Index");
        Id = item.Id;
        ProdutoId = item.ProdutoId;
        LocalId = item.LocalId;
        Entrada = item.Entrada;
        UnidadeId = item.UnidadeId;
        Quantidade = item.Quantidade;
        Observacao = item.Observacao;

        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
        Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        var doc = new AjusteEstoque { Id = Id, ProdutoId = ProdutoId, LocalId = LocalId, Entrada = Entrada, UnidadeId = UnidadeId, Quantidade = Quantidade, Observacao = Observacao };
        _docs.UpdateAjuste(doc);
        return RedirectToPage("Index");
    }
}