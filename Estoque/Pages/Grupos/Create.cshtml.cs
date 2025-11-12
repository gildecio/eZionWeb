using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Estoque.Pages.Grupos;

public class CreateModel : PageModel
{
    private readonly IGrupoRepository _repo;

    [BindProperty]
    public Grupo Item { get; set; } = new();
    public List<SelectListItem> PossiveisPais { get; set; } = new();

    public CreateModel(IGrupoRepository repo)
    {
        _repo = repo;
    }

    public void OnGet(int? parentId)
    {
        Item.ParentId = parentId;
        PossiveisPais = _repo.GetAll()
            .Where(g => DepthOf(g.Id) < 3)
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome })
            .ToList();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            PossiveisPais = _repo.GetAll()
                .Where(g => DepthOf(g.Id) < 3)
                .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome })
                .ToList();
            return Page();
        }
        if (Item.ParentId.HasValue)
        {
            var pd = DepthOf(Item.ParentId.Value);
            if (pd >= 3)
            {
                ModelState.AddModelError("Item.ParentId", "Máximo de três níveis");
                PossiveisPais = _repo.GetAll()
                    .Where(g => DepthOf(g.Id) < 3)
                    .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nome })
                    .ToList();
                return Page();
            }
        }
        _repo.Add(Item);
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
}