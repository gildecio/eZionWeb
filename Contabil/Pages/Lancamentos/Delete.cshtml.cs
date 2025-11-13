using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Contabil.Pages.Lancamentos;

public class DeleteModel : PageModel
{
    private readonly ILancamentoRepository _repo;

    [BindProperty]
    public Lancamento? Item { get; set; }

    public DeleteModel(ILancamentoRepository repo)
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