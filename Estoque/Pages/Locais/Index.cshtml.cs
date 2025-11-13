using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Locais;

public class IndexModel : PageModel
{
    private readonly ILocalEstoqueRepository _repo;
    public List<LocalEstoque> Itens { get; set; } = new();

    public IndexModel(ILocalEstoqueRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
        Itens = _repo.GetAll().OrderBy(x => x.Nome).ToList();
    }
}