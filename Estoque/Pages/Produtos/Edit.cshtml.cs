using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Produtos;

public class EditModel : PageModel
{
    private readonly IProdutoRepository _repo;
    private readonly IGrupoRepository _grupos;
    private readonly IUnidadeRepository _unidades;

    [BindProperty]
    public Produto Item { get; set; } = new();
    public List<SelectListItem> Analiticos { get; set; } = new();
    public List<SelectListItem> Tipos { get; set; } = new();
    public List<SelectListItem> Unidades { get; set; } = new();

    public EditModel(IProdutoRepository repo, IGrupoRepository grupos, IUnidadeRepository unidades)
    {
        _repo = repo;
        _grupos = grupos;
        _unidades = unidades;
    }

    public IActionResult OnGet(int id)
    {
        var item = _repo.GetById(id);
        if (item == null) return RedirectToPage("Index");
        Item = new Produto { Id = item.Id, Nome = item.Nome, GrupoId = item.GrupoId, Tipo = item.Tipo, Codigo = item.Codigo, CodigoAnterior = item.CodigoAnterior, UnidadeId = item.UnidadeId };
        Analiticos = _grupos.GetAll().Where(g => _grupos.IsLeaf(g.Id))
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome }).ToList();
        Tipos = new()
        {
            new SelectListItem { Value = ((int)TipoProduto.MateriaPrima).ToString(), Text = "Matéria Prima" },
            new SelectListItem { Value = ((int)TipoProduto.ProdutoAcabado).ToString(), Text = "Produto Acabado" },
            new SelectListItem { Value = ((int)TipoProduto.ProdutoSemiacabado).ToString(), Text = "Produto Semiacabado" },
            new SelectListItem { Value = ((int)TipoProduto.Embalagem).ToString(), Text = "Embalagem" },
            new SelectListItem { Value = ((int)TipoProduto.Servico).ToString(), Text = "Serviço" },
            new SelectListItem { Value = ((int)TipoProduto.Imobilizado).ToString(), Text = "Imobilizado" },
            new SelectListItem { Value = ((int)TipoProduto.Outros).ToString(), Text = "Outros" }
        };
        Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Analiticos = _grupos.GetAll().Where(g => _grupos.IsLeaf(g.Id))
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome }).ToList();
            Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
            return Page();
        }
        if (!Item.GrupoId.HasValue || !_grupos.IsLeaf(Item.GrupoId.Value))
        {
            ModelState.AddModelError("Item.GrupoId", "Selecione um grupo analítico válido");
            Analiticos = _grupos.GetAll().Where(g => _grupos.IsLeaf(g.Id))
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome }).ToList();
            Tipos = new()
            {
                new SelectListItem { Value = ((int)TipoProduto.MateriaPrima).ToString(), Text = "Matéria Prima" },
                new SelectListItem { Value = ((int)TipoProduto.ProdutoAcabado).ToString(), Text = "Produto Acabado" },
                new SelectListItem { Value = ((int)TipoProduto.ProdutoSemiacabado).ToString(), Text = "Produto Semiacabado" },
                new SelectListItem { Value = ((int)TipoProduto.Embalagem).ToString(), Text = "Embalagem" },
                new SelectListItem { Value = ((int)TipoProduto.Servico).ToString(), Text = "Serviço" },
                new SelectListItem { Value = ((int)TipoProduto.Imobilizado).ToString(), Text = "Imobilizado" },
                new SelectListItem { Value = ((int)TipoProduto.Outros).ToString(), Text = "Outros" }
            };
            Unidades = _unidades.GetAll().OrderBy(u => u.Nome)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome + " (" + u.Sigla + ")" }).ToList();
            return Page();
        }
        _repo.Update(Item);
        return RedirectToPage("Index");
    }
}