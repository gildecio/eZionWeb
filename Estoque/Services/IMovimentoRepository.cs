using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public interface IMovimentoRepository
{
    IEnumerable<MovimentoEstoque> GetAll(int? produtoId = null);
    MovimentoEstoque Add(MovimentoEstoque mov);
    void Delete(int id);
    decimal GetSaldo(int produtoId, int localId);
    IEnumerable<(int localId, decimal qtd)> GetSaldosPorProduto(int produtoId);
}