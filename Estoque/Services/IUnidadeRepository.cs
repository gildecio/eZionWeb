using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public interface IUnidadeRepository
{
    IEnumerable<UnidadeMedida> GetAll();
    UnidadeMedida? GetById(int id);
    UnidadeMedida Add(UnidadeMedida u);
}

public interface IConversaoRepository
{
    IEnumerable<ConversaoUnidade> GetAll();
    ConversaoUnidade Add(ConversaoUnidade c);
    decimal Converter(int deUnidadeId, int paraUnidadeId, decimal quantidade);
    bool ExisteConversao(int deUnidadeId, int paraUnidadeId);
}