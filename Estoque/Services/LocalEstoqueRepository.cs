using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public class LocalEstoqueRepository : ILocalEstoqueRepository
{
    private readonly List<LocalEstoque> _itens = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<LocalEstoque> GetAll()
    {
        lock (_lock) { return _itens.Select(x => new LocalEstoque { Id = x.Id, Codigo = x.Codigo, Nome = x.Nome }).ToList(); }
    }

    public LocalEstoque? GetById(int id)
    {
        lock (_lock) { return _itens.FirstOrDefault(x => x.Id == id); }
    }

    public LocalEstoque Add(LocalEstoque item)
    {
        lock (_lock)
        {
            item.Id = _nextId++;
            if (string.IsNullOrWhiteSpace(item.Codigo)) item.Codigo = item.Id.ToString();
            _itens.Add(item);
            return item;
        }
    }

    public void Update(LocalEstoque item)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == item.Id);
            if (idx >= 0)
            {
                _itens[idx] = item;
            }
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