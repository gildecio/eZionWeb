using eZionWeb.Configuracoes.Models;

namespace eZionWeb.Configuracoes.Services;

public class SequenciaRepository : ISequenciaRepository
{
    private readonly List<Sequencia> _itens = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<Sequencia> GetAll()
    {
        lock (_lock) { return _itens.Select(s => new Sequencia { Id = s.Id, Documento = s.Documento, Serie = s.Serie, Atual = s.Atual, Tipo = s.Tipo }).ToList(); }
    }

    public Sequencia? GetById(int id)
    {
        lock (_lock) { return _itens.FirstOrDefault(s => s.Id == id); }
    }

    public Sequencia? GetByDocSerie(string documento, string serie)
    {
        lock (_lock) { return _itens.FirstOrDefault(s => s.Documento == documento && s.Serie == (serie ?? string.Empty)); }
    }

    public Sequencia Add(Sequencia s)
    {
        lock (_lock)
        {
            s.Id = _nextId++;
            if (string.IsNullOrWhiteSpace(s.Serie)) s.Serie = "0";
            _itens.Add(s);
            return s;
        }
    }

    public void Update(Sequencia s)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == s.Id);
            if (idx >= 0) _itens[idx] = s;
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            var idx = _itens.FindIndex(x => x.Id == id);
            if (idx >= 0) _itens.RemoveAt(idx);
        }
    }

    public int ProximoNumero(string documento, string serie, DateTime data)
    {
        lock (_lock)
        {
            var s = GetByDocSerie(documento, serie ?? string.Empty);
            if (s == null)
            {
                var sDoc = _itens.FirstOrDefault(x => x.Documento == documento);
                if (sDoc != null)
                {
                    s = sDoc;
                }
                else
                {
                    var tipoNovo = string.IsNullOrWhiteSpace(serie) ? TipoSequencia.Anual : TipoSequencia.Continua;
                    var serieNova = string.IsNullOrWhiteSpace(serie) ? data.Year.ToString() : serie;
                    s = new Sequencia { Documento = documento, Serie = serieNova, Tipo = tipoNovo, Atual = 0 };
                    Add(s);
                }
            }
            if (s.Tipo == TipoSequencia.Anual)
            {
                var anoAtual = data.Year.ToString();
                if (s.Serie != anoAtual)
                {
                    s.Serie = anoAtual;
                    s.Atual = 0;
                }
            }
            else if (s.Tipo == TipoSequencia.Continua)
            {
                if (s.Atual >= 999999)
                {
                    s.Atual = 0;
                    s.Serie = IncrementaSerie(s.Serie);
                }
            }
            s.Atual += 1;
            Update(s);
            return s.Atual;
        }
    }

    public (int numero, string serieUsada) ProximoNumeroComSerie(string documento, string serie, DateTime data)
    {
        lock (_lock)
        {
            var s = GetByDocSerie(documento, serie ?? string.Empty);
            if (s == null)
            {
                var sDoc = _itens.FirstOrDefault(x => x.Documento == documento);
                if (sDoc != null)
                {
                    s = sDoc;
                }
                else
                {
                    var tipoNovo = string.IsNullOrWhiteSpace(serie) ? TipoSequencia.Anual : TipoSequencia.Continua;
                    var serieNova = string.IsNullOrWhiteSpace(serie) ? data.Year.ToString() : serie;
                    s = new Sequencia { Documento = documento, Serie = serieNova, Tipo = tipoNovo, Atual = 0 };
                    Add(s);
                }
            }
            if (s.Tipo == TipoSequencia.Anual)
            {
                var anoAtual = data.Year.ToString();
                if (s.Serie != anoAtual)
                {
                    s.Serie = anoAtual;
                    s.Atual = 0;
                }
            }
            else if (s.Tipo == TipoSequencia.Continua)
            {
                if (s.Atual >= 999999)
                {
                    s.Atual = 0;
                    s.Serie = IncrementaSerie(s.Serie);
                }
            }
            s.Atual += 1;
            Update(s);
            return (s.Atual, s.Serie);
        }
    }

    private string IncrementaSerie(string serie)
    {
        if (string.IsNullOrWhiteSpace(serie)) return "1";
        int i = serie.Length - 1;
        while (i >= 0 && char.IsDigit(serie[i])) i--;
        var prefix = serie.Substring(0, i + 1);
        var digits = serie.Substring(i + 1);
        if (digits.Length == 0) return serie + "1";
        var n = int.Parse(digits);
        var inc = (n + 1).ToString().PadLeft(digits.Length, '0');
        return prefix + inc;
    }
}