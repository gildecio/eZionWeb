using eZionWeb.Contabil.Models;

namespace eZionWeb.Contabil.Services;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly List<Lancamento> _itens = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<Lancamento> GetAll()
    {
        lock (_lock) { return _itens.Select(x => new Lancamento { Id = x.Id, Data = x.Data, EmpresaId = x.EmpresaId, PlanoContaId = x.PlanoContaId, Historico = x.Historico, Valor = x.Valor, Tipo = x.Tipo }).ToList(); }
    }

    public Lancamento? GetById(int id)
    {
        lock (_lock) { return _itens.FirstOrDefault(x => x.Id == id); }
    }

    public Lancamento Add(Lancamento l)
    {
        lock (_lock)
        {
            l.Id = _nextId++;
            _itens.Add(l);
            return l;
        }
    }

    public void Update(Lancamento l)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == l.Id);
            if (idx >= 0) _itens[idx] = l;
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