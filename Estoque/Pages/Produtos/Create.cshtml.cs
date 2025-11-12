using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Produtos;

public class CreateModel : PageModel
{
    private readonly IProdutoRepository _repo;

    [BindProperty]
    public Produto Item { get; set; } = new();

    public CreateModel(IProdutoRepository repo)
    {
        _repo = repo;
    }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        _repo.Add(Item);
        return RedirectToPage("Index");
    }
}