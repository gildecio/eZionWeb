using eZionWeb.Contabil.Models;

namespace eZionWeb.Contabil.Services;

public class PlanoContaRepository : IPlanoContaRepository
{
    private readonly List<PlanoConta> _itens = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<PlanoConta> GetAll()
    {
        lock (_lock) { return _itens.Select(x => new PlanoConta { Id = x.Id, EmpresaId = x.EmpresaId, Codigo = x.Codigo, Nome = x.Nome }).ToList(); }
    }

    public PlanoConta? GetById(int id)
    {
        lock (_lock) { return _itens.FirstOrDefault(x => x.Id == id); }
    }

    public PlanoConta Add(PlanoConta p)
    {
        lock (_lock)
        {
            p.Id = _nextId++;
            _itens.Add(p);
            return p;
        }
    }

    public void Update(PlanoConta p)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == p.Id);
            if (idx >= 0) _itens[idx] = p;
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == id);
            if (idx >= 0) _itens.RemoveAt(idx);
        }
    }
}