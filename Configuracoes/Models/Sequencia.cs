namespace eZionWeb.Configuracoes.Models;

public enum TipoSequencia
{
    Anual = 1,
    Continua = 2
}

public class Sequencia
{
    public int Id { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public int Atual { get; set; }
    public TipoSequencia Tipo { get; set; } = TipoSequencia.Continua;
}