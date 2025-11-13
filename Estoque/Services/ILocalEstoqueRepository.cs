using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public interface ILocalEstoqueRepository
{
    IEnumerable<LocalEstoque> GetAll();
    LocalEstoque? GetById(int id);
    LocalEstoque Add(LocalEstoque item);
    void Update(LocalEstoque item);
    void Delete(int id);
}