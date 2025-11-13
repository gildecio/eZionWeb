using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Locais;

public class EditModel : PageModel
{
    private readonly ILocalEstoqueRepository _repo;

    [BindProperty]
    public LocalEstoque Item { get; set; } = new();

    public EditModel(ILocalEstoqueRepository repo)
    {
        _repo = repo;
    }

    public IActionResult OnGet(int id)
    {
        var item = _repo.GetById(id);
        if (item == null) return RedirectToPage("Index");
        Item = item;
        return Page();
    }

    public IActionResult OnPost()
    {
        _repo.Update(Item);
        return RedirectToPage("Index");
    }
}