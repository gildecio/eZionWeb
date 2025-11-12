using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Produtos;

public class CreateModel : PageModel
{
    private readonly IProdutoRepository _repo;
    private readonly IGrupoRepository _grupos;

    [BindProperty]
    public Produto Item { get; set; } = new();
    public List<SelectListItem> Analiticos { get; set; } = new();
    public List<SelectListItem> Tipos { get; set; } = new();

    public CreateModel(IProdutoRepository repo, IGrupoRepository grupos)
    {
        _repo = repo;
        _grupos = grupos;
    }

    public void OnGet()
    {
        Analiticos = _grupos.GetAll().Where(g => _grupos.IsLeaf(g.Id))
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome }).ToList();
        Tipos = new()
        {
            new SelectListItem { Value = ((int)TipoProduto.MateriaPrima).ToString(), Text = "Matéria Prima" },
            new SelectListItem { Value = ((int)TipoProduto.ProdutoAcabado).ToString(), Text = "Produto Acabado" },
            new SelectListItem { Value = ((int)TipoProduto.ProdutoSemiacabado).ToString(), Text = "Produto Semiacabado" },
            new SelectListItem { Value = ((int)TipoProduto.Embalagem).ToString(), Text = "Embalagem" },
            new SelectListItem { Value = ((int)TipoProduto.Servico).ToString(), Text = "Serviço" },
            new SelectListItem { Value = ((int)TipoProduto.Imobilizado).ToString(), Text = "Imobilizado" },
            new SelectListItem { Value = ((int)TipoProduto.Outros).ToString(), Text = "Outros" }
        };
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        if (!Item.GrupoId.HasValue || !_grupos.IsLeaf(Item.GrupoId.Value))
        {
            ModelState.AddModelError("Item.GrupoId", "Selecione um grupo analítico válido");
            OnGet();
            return Page();
        }
        _repo.Add(Item);
        return RedirectToPage("Index");
    }
}