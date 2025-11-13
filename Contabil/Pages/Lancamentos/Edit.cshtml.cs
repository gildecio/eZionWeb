using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Contabil.Pages.Lancamentos;

public class EditModel : PageModel
{
    private readonly ILancamentoRepository _repo;
    private readonly IPlanoContaRepository _contasRepo;

    [BindProperty]
    public Lancamento? Item { get; set; }

    public List<SelectListItem> Contas { get; set; } = new();

    public EditModel(ILancamentoRepository repo, IPlanoContaRepository contas)
    {
        _repo = repo;
        _contasRepo = contas;
    }

    public IActionResult OnGet(int id)
    {
        Item = _repo.GetById(id);
        if (Item == null) return RedirectToPage("Index");
        var contasAll = _contasRepo.GetAll().Where(c => c.EmpresaId == Item.EmpresaId).OrderBy(c => c.Codigo);
        Contas = contasAll.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Codigo + " - " + c.Nome }).ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Item == null) return RedirectToPage("Index");
        _repo.Update(Item);
        return RedirectToPage("Index");
    }
}