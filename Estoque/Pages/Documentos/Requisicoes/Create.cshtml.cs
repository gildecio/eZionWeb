using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Requisicoes;

public class CreateModel : PageModel
{
    private readonly IDocumentoService _docs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IUnidadeRepository _unidades;

    [BindProperty]
    public int ProdutoId { get; set; }
    [BindProperty]
    public int LocalOrigemId { get; set; }
    [BindProperty]
    public int UnidadeId { get; set; }
    [BindProperty]
    public decimal Quantidade { get; set; }
    [BindProperty]
    public string Observacao { get; set; } = string.Empty;

    public List<SelectListItem> Produtos { get; set; } = new();
    public List<SelectListItem> Locais { get; set; } = new();
    public List<SelectListItem> Unidades { get; set; } = new();

    public CreateModel(IDocumentoService docs, IProdutoRepository produtos, ILocalEstoqueRepository locais, IUnidadeRepository unidades)
    {
        _docs = docs;
        _produtos = produtos;
        _locais = locais;
        _unidades = unidades;
    }

    public void OnGet()
    {
        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
        Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
    }

    public IActionResult OnPost()
    {
        var doc = new RequisicaoEstoque { ProdutoId = ProdutoId, LocalOrigemId = LocalOrigemId, UnidadeId = UnidadeId, Quantidade = Quantidade, Observacao = Observacao };
        _docs.AddRequisicao(doc);
        return RedirectToPage("Index");
    }
}