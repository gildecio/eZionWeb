using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public class ProdutoRepository : IProdutoRepository
{
    private readonly List<Produto> _itens = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<Produto> GetAll()
    {
        lock (_lock) { return _itens.Select(p => new Produto { Id = p.Id, Nome = p.Nome, Preco = p.Preco, Quantidade = p.Quantidade }).ToList(); }
    }

    public Produto? GetById(int id)
    {
        lock (_lock) { return _itens.FirstOrDefault(p => p.Id == id); }
    }

    public Produto Add(Produto produto)
    {
        lock (_lock)
        {
            produto.Id = _nextId++;
            _itens.Add(produto);
            return produto;
        }
    }

    public void Update(Produto produto)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(p => p.Id == produto.Id);
            if (idx >= 0)
            {
                _itens[idx] = produto;
            }
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(p => p.Id == id);
            if (idx >= 0) _itens.RemoveAt(idx);
        }
    }
}