using eZionWeb.Configuracoes.Models;
using eZionWeb.Configuracoes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Configuracoes.Pages.Sequencias;

public class CreateModel : PageModel
{
    private readonly ISequenciaRepository _repo;

    [BindProperty]
    public string Documento { get; set; } = "Ajuste";
    [BindProperty]
    public string Serie { get; set; } = string.Empty;
    [BindProperty]
    public string Tipo { get; set; } = "Anual";
    [BindProperty]
    public int? IniciarEm { get; set; }

    public CreateModel(ISequenciaRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Documento)) ModelState.AddModelError(nameof(Documento), "Informe o documento");
        if (!ModelState.IsValid) return Page();
        var tipoEnum = Tipo == "Anual" ? TipoSequencia.Anual : TipoSequencia.Continua;
        var atual = 0;
        if (IniciarEm.HasValue && IniciarEm.Value > 0) atual = IniciarEm.Value - 1;
        _repo.Add(new Sequencia { Documento = Documento, Serie = Serie ?? string.Empty, Tipo = tipoEnum, Atual = atual });
        return RedirectToPage("Index");
    }
}