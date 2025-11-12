using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Produtos;

public class IndexModel : PageModel
{
    private readonly IProdutoRepository _repo;
    public List<Produto> Itens { get; set; } = new();

    public IndexModel(IProdutoRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
        Itens = _repo.GetAll().ToList();
    }
}