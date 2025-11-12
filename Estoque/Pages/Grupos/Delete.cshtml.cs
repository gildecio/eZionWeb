using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Grupos;

public class DeleteModel : PageModel
{
    private readonly IGrupoRepository _repo;

    [BindProperty]
    public Grupo? Item { get; set; }
    public bool TemFilhos { get; set; }

    public DeleteModel(IGrupoRepository repo)
    {
        _repo = repo;
    }

    public IActionResult OnGet(int id)
    {
        Item = _repo.GetById(id);
        if (Item == null) return RedirectToPage("Index");
        TemFilhos = _repo.GetChildren(id).Any();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Item != null)
        {
            TemFilhos = _repo.GetChildren(Item.Id).Any();
            if (!TemFilhos)
            {
                _repo.Delete(Item.Id);
            }
        }
        return RedirectToPage("Index");
    }
}