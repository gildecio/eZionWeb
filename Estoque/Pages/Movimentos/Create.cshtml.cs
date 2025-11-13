using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Movimentos;

public class CreateModel : PageModel
{
    private readonly IMovimentoRepository _repo;
    private readonly IProdutoRepository _produtos;
    private readonly ILocalEstoqueRepository _locais;

    [BindProperty]
    public TipoMovimento Tipo { get; set; } = TipoMovimento.Entrada;
    [BindProperty]
    public int ProdutoId { get; set; }
    [BindProperty]
    public int? LocalOrigemId { get; set; }
    [BindProperty]
    public int? LocalDestinoId { get; set; }
    [BindProperty]
    public decimal Quantidade { get; set; }
    [BindProperty]
    public string Observacao { get; set; } = string.Empty;

    public List<SelectListItem> Tipos { get; set; } = new();
    public List<SelectListItem> Produtos { get; set; } = new();
    public List<SelectListItem> Locais { get; set; } = new();

    public CreateModel(IMovimentoRepository repo, IProdutoRepository produtos, ILocalEstoqueRepository locais)
    {
        _repo = repo;
        _produtos = produtos;
        _locais = locais;
    }

    public void OnGet()
    {
        Tipos = new List<SelectListItem>
        {
            new SelectListItem("Entrada", ((int)TipoMovimento.Entrada).ToString()),
            new SelectListItem("Saída", ((int)TipoMovimento.Saida).ToString()),
            new SelectListItem("Transferência", ((int)TipoMovimento.Transferencia).ToString()),
            new SelectListItem("Ajuste", ((int)TipoMovimento.Ajuste).ToString())
        };

        Produtos = _produtos.GetAll().OrderBy(p => p.Nome)
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString())).ToList();

        Locais = _locais.GetAll().OrderBy(l => l.Nome)
            .Select(l => new SelectListItem(l.Nome, l.Id.ToString())).ToList();
    }

    public IActionResult OnPost()
    {
        var mov = new MovimentoEstoque
        {
            Tipo = Tipo,
            ProdutoId = ProdutoId,
            LocalOrigemId = LocalOrigemId,
            LocalDestinoId = LocalDestinoId,
            Quantidade = Quantidade,
            Observacao = Observacao,
            Data = DateTime.UtcNow
        };
        _repo.Add(mov);
        return RedirectToPage("Index");
    }
}