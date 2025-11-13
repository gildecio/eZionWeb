using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Requisicoes;

public class ViewModel : PageModel
{
    private readonly IDocumentoService _docs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IUnidadeRepository _unidades;

    public RequisicaoEstoque? Doc { get; set; }
    public Dictionary<int, string> Prods { get; set; } = new();
    public Dictionary<int, string> Locs { get; set; } = new();
    public Dictionary<int, string> Uns { get; set; } = new();

    public int LocalOrigemId { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public List<DocumentoItem> Itens { get; set; } = new();

    public List<SelectListItem> Produtos { get; set; } = new();
    public List<SelectListItem> Locais { get; set; } = new();
    public List<SelectListItem> Unidades { get; set; } = new();

    public ViewModel(IDocumentoService docs, IProdutoRepository produtos, ILocalEstoqueRepository locais, IUnidadeRepository unidades)
    {
        _docs = docs;
        _produtos = produtos;
        _locais = locais;
        _unidades = unidades;
    }

    public IActionResult OnGet(int id)
    {
        Doc = _docs.GetRequisicoes().FirstOrDefault(d => d.Id == id);
        if (Doc == null) return RedirectToPage("Index");
        Prods = _produtos.GetAll().ToDictionary(p => p.Id, p => p.Nome);
        Locs = _locais.GetAll().ToDictionary(l => l.Id, l => l.Nome);
        Uns = _unidades.GetAll().ToDictionary(u => u.Id, u => u.Sigla);
        LocalOrigemId = Doc.LocalOrigemId;
        Observacao = Doc.Observacao ?? string.Empty;
        Itens = Doc.Itens ?? new List<DocumentoItem>();
        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
        Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
        return Page();
    }
}