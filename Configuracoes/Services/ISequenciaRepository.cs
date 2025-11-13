using eZionWeb.Configuracoes.Models;

namespace eZionWeb.Configuracoes.Services;

public interface ISequenciaRepository
{
    IEnumerable<Sequencia> GetAll();
    Sequencia? GetById(int id);
    Sequencia? GetByDocSerie(string documento, string serie);
    Sequencia Add(Sequencia s);
    void Update(Sequencia s);
    void Delete(int id);
    int ProximoNumero(string documento, string serie, DateTime data);
    (int numero, string serieUsada) ProximoNumeroComSerie(string documento, string serie, DateTime data);
}