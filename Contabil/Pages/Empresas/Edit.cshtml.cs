using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Contabil.Pages.Empresas;

public class EditModel : PageModel
{
    private readonly IEmpresaRepository _repo;

    [BindProperty]
    public Empresa? Item { get; set; }

    public EditModel(IEmpresaRepository repo)
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
        if (Item == null) return RedirectToPage("Index");
        _repo.Update(Item);
        return RedirectToPage("Index");
    }
}