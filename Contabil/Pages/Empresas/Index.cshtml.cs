using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Contabil.Pages.Empresas;

public class IndexModel : PageModel
{
    private readonly IEmpresaRepository _repo;
    public List<Empresa> Itens { get; set; } = new();

    public IndexModel(IEmpresaRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
        Itens = _repo.GetAll().OrderBy(x => x.Nome).ToList();
    }
}