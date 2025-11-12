using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Produtos;

public class EditModel : PageModel
{
    private readonly IProdutoRepository _repo;

    [BindProperty]
    public Produto Item { get; set; } = new();

    public EditModel(IProdutoRepository repo)
    {
        _repo = repo;
    }

    public IActionResult OnGet(int id)
    {
        var item = _repo.GetById(id);
        if (item == null) return RedirectToPage("Index");
        Item = new Produto { Id = item.Id, Nome = item.Nome, Preco = item.Preco, Quantidade = item.Quantidade };
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        _repo.Update(Item);
        return RedirectToPage("Index");
    }
}