using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public class GrupoRepository : IGrupoRepository
{
    private readonly List<Grupo> _grupos = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<Grupo> GetAll()
    {
        lock (_lock) { return _grupos.Select(g => new Grupo { Id = g.Id, Nome = g.Nome, ParentId = g.ParentId, Codigo = g.Codigo, ContaCredito = g.ContaCredito, ContaDebito = g.ContaDebito }).ToList(); }
    }

    public Grupo? GetById(int id)
    {
        lock (_lock) { return _grupos.FirstOrDefault(g => g.Id == id); }
    }

    public IEnumerable<Grupo> GetChildren(int? parentId)
    {
        lock (_lock) { return _grupos.Where(g => g.ParentId == parentId).Select(g => new Grupo { Id = g.Id, Nome = g.Nome, ParentId = g.ParentId, Codigo = g.Codigo, ContaCredito = g.ContaCredito, ContaDebito = g.ContaDebito }).ToList(); }
    }

    public Grupo Add(Grupo grupo)
    {
        lock (_lock)
        {
            grupo.Id = _nextId++;
            _grupos.Add(grupo);
            return grupo;
        }
    }

    public void Update(Grupo grupo)
    {
        lock (_lock)
        {
            var idx = _grupos.FindIndex(g => g.Id == grupo.Id);
            if (idx >= 0)
            {
                _grupos[idx] = grupo;
            }
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            if (_grupos.Any(g => g.ParentId == id)) return;
            var idx = _grupos.FindIndex(g => g.Id == id);
            if (idx >= 0) _grupos.RemoveAt(idx);
        }
    }

    public bool IsLeaf(int id)
    {
        lock (_lock) { return !_grupos.Any(g => g.ParentId == id); }
    }

    public bool IsDescendantOf(int id, int ancestorId)
    {
        lock (_lock)
        {
            var current = _grupos.FirstOrDefault(g => g.Id == id);
            while (current != null && current.ParentId.HasValue)
            {
                if (current.ParentId.Value == ancestorId) return true;
                current = _grupos.FirstOrDefault(g => g.Id == current.ParentId.Value);
            }
            return false;
        }
    }
}