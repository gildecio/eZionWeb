using eZionWeb.Estoque.Models;

namespace eZionWeb.Estoque.Services;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly List<MovimentoEstoque> _movs = new();
    private readonly Dictionary<(int produtoId, int localId), decimal> _saldos = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IEnumerable<MovimentoEstoque> GetAll(int? produtoId = null)
    {
        lock (_lock)
        {
            var q = _movs.AsEnumerable();
            if (produtoId.HasValue) q = q.Where(m => m.ProdutoId == produtoId.Value);
            return q.OrderByDescending(m => m.Data).ToList();
        }
    }

    public MovimentoEstoque Add(MovimentoEstoque mov)
    {
        lock (_lock)
        {
            mov.Id = _nextId++;
            _movs.Add(mov);
            ApplySaldo(mov);
            return mov;
        }
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            var idx = _movs.FindIndex(m => m.Id == id);
            if (idx >= 0)
            {
                var mov = _movs[idx];
                _movs.RemoveAt(idx);
                RebuildSaldos();
            }
        }
    }

    public decimal GetSaldo(int produtoId, int localId)
    {
        lock (_lock)
        {
            return _saldos.TryGetValue((produtoId, localId), out var v) ? v : 0m;
        }
    }

    public IEnumerable<(int localId, decimal qtd)> GetSaldosPorProduto(int produtoId)
    {
        lock (_lock)
        {
            return _saldos.Where(kv => kv.Key.produtoId == produtoId)
                .Select(kv => (kv.Key.localId, kv.Value))
                .OrderBy(x => x.localId)
                .ToList();
        }
    }

    private void ApplySaldo(MovimentoEstoque mov)
    {
        switch (mov.Tipo)
        {
            case TipoMovimento.Entrada:
                if (mov.LocalDestinoId.HasValue)
                    AddQty(mov.ProdutoId, mov.LocalDestinoId.Value, mov.Quantidade);
                break;
            case TipoMovimento.Saida:
                if (mov.LocalOrigemId.HasValue)
                    AddQty(mov.ProdutoId, mov.LocalOrigemId.Value, -mov.Quantidade);
                break;
            case TipoMovimento.Transferencia:
                if (mov.LocalOrigemId.HasValue)
                    AddQty(mov.ProdutoId, mov.LocalOrigemId.Value, -mov.Quantidade);
                if (mov.LocalDestinoId.HasValue)
                    AddQty(mov.ProdutoId, mov.LocalDestinoId.Value, mov.Quantidade);
                break;
            case TipoMovimento.Ajuste:
                if (mov.LocalDestinoId.HasValue)
                    AddQty(mov.ProdutoId, mov.LocalDestinoId.Value, mov.Quantidade);
                break;
        }
    }

    private void RebuildSaldos()
    {
        _saldos.Clear();
        foreach (var m in _movs)
        {
            ApplySaldo(m);
        }
    }

    private void AddQty(int produtoId, int localId, decimal qtd)
    {
        var key = (produtoId, localId);
        if (_saldos.TryGetValue(key, out var v))
        {
            _saldos[key] = v + qtd;
        }
        else
        {
            _saldos[key] = qtd;
        }
    }
}