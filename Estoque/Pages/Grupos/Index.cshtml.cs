using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Grupos;

public class IndexModel : PageModel
{
    private readonly IGrupoRepository _repo;
    public List<Linha> Itens { get; set; } = new();
    public Dictionary<int, List<Linha>> ChildrenByParent { get; set; } = new();

    public IndexModel(IGrupoRepository repo)
    {
        _repo = repo;
    }

    public void OnGet()
    {
        var all = _repo.GetAll().ToList();
        Itens = all.Select(g => new Linha
        {
            Id = g.Id,
            Nome = g.Nome,
            Codigo = g.Codigo,
            ParentId = g.ParentId,
            PaiNome = g.ParentId.HasValue ? _repo.GetById(g.ParentId.Value)?.Nome : null,
            Nivel = DepthOf(g.Id)
        }).OrderBy(x => x.Nivel).ThenBy(x => x.Nome).ToList();

        ChildrenByParent = Itens.Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public class Linha
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? PaiNome { get; set; }
        public int Nivel { get; set; }
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

    public IEnumerable<Linha> DescendantsOf(int id)
    {
        var stack = new Stack<int>();
        stack.Push(id);
        var result = new List<Linha>();
        while (stack.Count > 0)
        {
            var currentId = stack.Pop();
            if (ChildrenByParent.TryGetValue(currentId, out var kids))
            {
                foreach (var k in kids.OrderBy(x => x.Nome))
                {
                    result.Add(k);
                    stack.Push(k.Id);
                }
            }
        }
        return result;
    }
}