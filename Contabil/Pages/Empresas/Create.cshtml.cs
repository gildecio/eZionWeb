using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Contabil.Pages.Empresas;

public class CreateModel : PageModel
{
    private readonly IEmpresaRepository _repo;

    [BindProperty]
    public string Nome { get; set; } = string.Empty;
    [BindProperty]
    public string Cnpj { get; set; } = string.Empty;

    public CreateModel(IEmpresaRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        _repo.Add(new Empresa { Nome = Nome ?? string.Empty, Cnpj = Cnpj ?? string.Empty });
        return RedirectToPage("Index");
    }
}