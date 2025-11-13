using eZionWeb.Configuracoes.Models;
using eZionWeb.Configuracoes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Configuracoes.Pages.Sequencias;

public class EditModel : PageModel
{
    private readonly ISequenciaRepository _repo;

    [BindProperty]
    public Sequencia? Item { get; set; }
    [BindProperty]
    public string Tipo { get; set; } = "Anual";
    [BindProperty]
    public int? IniciarEm { get; set; }

    public EditModel(ISequenciaRepository repo)
    {
        _repo = repo;
    }

    public IActionResult OnGet(int id)
    {
        Item = _repo.GetById(id);
        if (Item == null) return RedirectToPage("Index");
        Tipo = Item.Tipo == TipoSequencia.Anual ? "Anual" : "Continua";
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Item == null) return RedirectToPage("Index");
        Item.Tipo = Tipo == "Anual" ? TipoSequencia.Anual : TipoSequencia.Continua;
        if (IniciarEm.HasValue && IniciarEm.Value > 0) Item.Atual = IniciarEm.Value - 1;
        _repo.Update(Item);
        return RedirectToPage("Index");
    }
}