using eZionWeb.Contabil.Models;

namespace eZionWeb.Contabil.Services;

public interface IEmpresaRepository
{
    IEnumerable<Empresa> GetAll();
    Empresa? GetById(int id);
    Empresa Add(Empresa e);
    void Update(Empresa e);
    void Delete(int id);
}