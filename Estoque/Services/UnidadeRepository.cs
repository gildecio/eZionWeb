using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public class UnidadeRepository : IUnidadeRepository, IConversaoRepository
{
    private readonly List<UnidadeMedida> _unidades = new();
    private readonly List<ConversaoUnidade> _convs = new();
    private int _nextUniId = 1;
    private int _nextConvId = 1;

    IEnumerable<UnidadeMedida> IUnidadeRepository.GetAll() => _unidades.ToList();
    public UnidadeMedida? GetById(int id) => _unidades.FirstOrDefault(u => u.Id == id);
    public UnidadeMedida Add(UnidadeMedida u)
    {
        u.Id = _nextUniId++;
        _unidades.Add(u);
        return u;
    }

    IEnumerable<ConversaoUnidade> IConversaoRepository.GetAll() => _convs.ToList();
    public ConversaoUnidade Add(ConversaoUnidade c)
    {
        c.Id = _nextConvId++;
        _convs.Add(c);
        return c;
    }

    public decimal Converter(int deUnidadeId, int paraUnidadeId, decimal quantidade)
    {
        if (deUnidadeId == paraUnidadeId) return quantidade;
        var conv = _convs.FirstOrDefault(x => x.DeUnidadeId == deUnidadeId && x.ParaUnidadeId == paraUnidadeId);
        if (conv != null) return quantidade * conv.Fator;
        var inv = _convs.FirstOrDefault(x => x.DeUnidadeId == paraUnidadeId && x.ParaUnidadeId == deUnidadeId);
        if (inv != null && inv.Fator != 0) return quantidade / inv.Fator;
        throw new InvalidOperationException("Conversão de unidade não encontrada");
    }

    public bool ExisteConversao(int deUnidadeId, int paraUnidadeId)
    {
        if (deUnidadeId == paraUnidadeId) return true;
        var conv = _convs.Any(x => x.DeUnidadeId == deUnidadeId && x.ParaUnidadeId == paraUnidadeId);
        if (conv) return true;
        var inv = _convs.Any(x => x.DeUnidadeId == paraUnidadeId && x.ParaUnidadeId == deUnidadeId);
        return inv;
    }
}