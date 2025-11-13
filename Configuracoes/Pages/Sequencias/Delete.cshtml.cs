using eZionWeb.Configuracoes.Models;
using eZionWeb.Configuracoes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Configuracoes.Pages.Sequencias;

public class DeleteModel : PageModel
{
    private readonly ISequenciaRepository _repo;

    [BindProperty]
    public Sequencia? Item { get; set; }

    public DeleteModel(ISequenciaRepository repo)
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
        if (Item != null) _repo.Delete(Item.Id);
        return RedirectToPage("Index");
    }
}