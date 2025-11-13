using eZionWeb.Contabil.Models;

namespace eZionWeb.Contabil.Services;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly List<Empresa> _itens = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<Empresa> GetAll()
    {
        lock (_lock) { return _itens.Select(x => new Empresa { Id = x.Id, Nome = x.Nome, Cnpj = x.Cnpj, Ativa = x.Ativa }).ToList(); }
    }

    public Empresa? GetById(int id)
    {
        lock (_lock) { return _itens.FirstOrDefault(x => x.Id == id); }
    }

    public Empresa Add(Empresa e)
    {
        lock (_lock)
        {
            e.Id = _nextId++;
            _itens.Add(e);
            return e;
        }
    }

    public void Update(Empresa e)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == e.Id);
            if (idx >= 0) _itens[idx] = e;
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