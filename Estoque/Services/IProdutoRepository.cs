using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public interface IProdutoRepository
{
    IEnumerable<Produto> GetAll();
    Produto? GetById(int id);
    Produto Add(Produto produto);
    void Update(Produto produto);
    void Delete(int id);
}