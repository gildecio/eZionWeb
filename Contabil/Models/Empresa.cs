namespace eZionWeb.Contabil.Models;

public class Empresa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
}