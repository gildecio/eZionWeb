using eZionWeb.Contabil.Models;

namespace eZionWeb.Contabil.Services;

public interface ILancamentoRepository
{
    IEnumerable<Lancamento> GetAll();
    Lancamento? GetById(int id);
    Lancamento Add(Lancamento l);
    void Update(Lancamento l);
    void Delete(int id);
}