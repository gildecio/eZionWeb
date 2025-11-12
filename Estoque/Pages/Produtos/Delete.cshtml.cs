using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Produtos;

public class DeleteModel : PageModel
{
    private readonly IProdutoRepository _repo;

    [BindProperty]
    public Produto? Item { get; set; }

    public DeleteModel(IProdutoRepository repo)
    {
        _repo = repo;
    }

    public IActionResult OnGet(int id)
    {
        Item = _repo.GetById(id);
        if (Item == null) return RedirectToPage("Index");
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Item != null)
        {
            _repo.Delete(Item.Id);
        }
        return RedirectToPage("Index");
    }
}