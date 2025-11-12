using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Grupos;

public class EditModel : PageModel
{
    private readonly IGrupoRepository _repo;

    [BindProperty]
    public Grupo Item { get; set; } = new();
    public List<SelectListItem> PossiveisPais { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;

    public EditModel(IGrupoRepository repo)
    {
        _repo = repo;
    }

    public IActionResult OnGet(int id)
    {
        var item = _repo.GetById(id);
        if (item == null) return RedirectToPage("Index");
        Item = new Grupo { Id = item.Id, Nome = item.Nome, ParentId = item.ParentId, Codigo = item.Codigo, ContaCredito = item.ContaCredito, ContaDebito = item.ContaDebito };
        var altura = SubtreeHeight(id);
        PossiveisPais = _repo.GetAll()
            .Where(g => g.Id != id && DepthOf(g.Id) + altura <= 3)
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome })
            .ToList();
        return Page();
    }

    public IActionResult OnPost()
    {
        PossiveisPais = _repo.GetAll().Where(g => g.Id != Item.Id).Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome }).ToList();
        if (!ModelState.IsValid) return Page();
        if (Item.ParentId == Item.Id || (Item.ParentId.HasValue && _repo.IsDescendantOf(Item.ParentId.Value, Item.Id)))
        {
            ErrorMessage = "Seleção de pai inválida";
            return Page();
        }
        var altura = SubtreeHeight(Item.Id);
        var pd = Item.ParentId.HasValue ? DepthOf(Item.ParentId.Value) : 0;
        if (pd + altura > 3)
        {
            ErrorMessage = "Excede o máximo de três níveis";
            PossiveisPais = _repo.GetAll()
                .Where(g => g.Id != Item.Id && DepthOf(g.Id) + altura <= 3)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome })
                .ToList();
            return Page();
        }
        _repo.Update(Item);
        return RedirectToPage("Index");
    }

    private int DepthOf(int id)
    {
        var d = 1;
        var current = _repo.GetById(id);
        while (current != null && current.ParentId.HasValue)
        {
            d++;
            current = _repo.GetById(current.ParentId.Value);
        }
        return d;
    }

    private int SubtreeHeight(int id)
    {
        var children = _repo.GetChildren(id).ToList();
        if (!children.Any()) return 1;
        var maxChild = 0;
        foreach (var c in children)
        {
            var h = SubtreeHeight(c.Id);
            if (h > maxChild) maxChild = h;
        }
        return 1 + maxChild;
    }
}