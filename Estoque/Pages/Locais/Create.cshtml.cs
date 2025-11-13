using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Locais;

public class CreateModel : PageModel
{
    private readonly ILocalEstoqueRepository _repo;

    [BindProperty]
    public LocalEstoque Item { get; set; } = new();

    public CreateModel(ILocalEstoqueRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        _repo.Add(Item);
        return RedirectToPage("Index");
    }
}