using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Devolucoes;

public class CreateModel : PageModel
{
    private readonly IDocumentoService _docs;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;
    private readonly IUnidadeRepository _unidades;

    [BindProperty]
    public int LocalDestinoId { get; set; }
    [BindProperty]
    public string Observacao { get; set; } = string.Empty;
    [BindProperty]
    public List<DocumentoItem> Itens { get; set; } = new();

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
        if (Itens.Count == 0) Itens.Add(new DocumentoItem());
    }

    public IActionResult OnPost()
    {
        if (LocalDestinoId <= 0) ModelState.AddModelError(nameof(LocalDestinoId), "Local de destino é obrigatório");
        if (Itens == null || Itens.Count == 0) ModelState.AddModelError("Itens", "Ao menos um item é obrigatório");
        if (Itens != null)
        {
            for (int i = 0; i < Itens.Count; i++)
            {
                if (Itens[i].ProdutoId <= 0) ModelState.AddModelError($"Itens[{i}].ProdutoId", "Produto é obrigatório");
                if (Itens[i].UnidadeId <= 0) ModelState.AddModelError($"Itens[{i}].UnidadeId", "Unidade é obrigatória");
                if (Itens[i].Quantidade <= 0) ModelState.AddModelError($"Itens[{i}].Quantidade", "Quantidade deve ser maior que zero");
            }
        }
        if (!ModelState.IsValid)
        {
            Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
            Locais = _locais.GetAll().OrderBy(l => l.Nome)
                .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
            Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
            return Page();
        }
        try
        {
            var doc = new DevolucaoEstoque { LocalDestinoId = LocalDestinoId, Observacao = Observacao ?? string.Empty, Itens = Itens ?? new List<DocumentoItem>() };
            _docs.AddDevolucao(doc);
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome }).ToList();
            Locais = _locais.GetAll().OrderBy(l => l.Nome)
                .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Nome }).ToList();
            Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
            return Page();
        }
    }
}