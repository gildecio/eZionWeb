using eZionWeb.Contabil.Models;

namespace eZionWeb.Contabil.Services;

public interface IPlanoContaRepository
{
    IEnumerable<PlanoConta> GetAll();
    PlanoConta? GetById(int id);
    PlanoConta Add(PlanoConta p);
    void Update(PlanoConta p);
    void Delete(int id);
}