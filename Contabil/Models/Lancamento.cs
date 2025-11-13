namespace eZionWeb.Contabil.Models;

public enum TipoLancamento
{
    Debito = 1,
    Credito = 2
}

public class Lancamento
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow.Date;
    public int EmpresaId { get; set; }
    public int PlanoContaId { get; set; }
    public string Historico { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoLancamento Tipo { get; set; } = TipoLancamento.Debito;
}