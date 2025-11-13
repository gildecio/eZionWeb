using eZionWeb.Configuracoes.Models;
using eZionWeb.Configuracoes.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Configuracoes.Pages.Sequencias;

public class IndexModel : PageModel
{
    private readonly ISequenciaRepository _repo;
    public List<Sequencia> Itens { get; set; } = new();

    public IndexModel(ISequenciaRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
        Itens = _repo.GetAll().OrderBy(s => s.Documento).ThenBy(s => s.Serie).ToList();
    }
}