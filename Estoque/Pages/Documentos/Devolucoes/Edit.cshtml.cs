using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Documentos.Devolucoes;

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
    public int LocalDestinoId { get; set; }
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
        var item = _docs.GetDevolucoes().FirstOrDefault(d => d.Id == id);
        if (item == null) return RedirectToPage("Index");
        if (item.MovimentoId.HasValue || (item.MovimentoIds != null && item.MovimentoIds.Count > 0))
            return RedirectToPage("Index");
        Id = item.Id;
        ProdutoId = item.ProdutoId;
        LocalDestinoId = item.LocalDestinoId;
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
        if (ProdutoId <= 0) ModelState.AddModelError(nameof(ProdutoId), "Produto é obrigatório");
        if (LocalDestinoId <= 0) ModelState.AddModelError(nameof(LocalDestinoId), "Local de destino é obrigatório");
        if (UnidadeId <= 0) ModelState.AddModelError(nameof(UnidadeId), "Unidade é obrigatória");
        if (Quantidade <= 0) ModelState.AddModelError(nameof(Quantidade), "Quantidade deve ser maior que zero");
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
            var doc = new DevolucaoEstoque { Id = Id, ProdutoId = ProdutoId, LocalDestinoId = LocalDestinoId, UnidadeId = UnidadeId, Quantidade = Quantidade, Observacao = Observacao };
            _docs.UpdateDevolucao(doc);
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