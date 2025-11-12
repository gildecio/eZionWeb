using eZionWeb.Estoque.Models;
using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Produtos;

public class IndexModel : PageModel
{
    private readonly IProdutoRepository _repo;
    private readonly IGrupoRepository _grupos;
    public List<Linha> Itens { get; set; } = new();

    public IndexModel(IProdutoRepository repo, IGrupoRepository grupos)
    {
        _repo = repo;
        _grupos = grupos;
    }

    public void OnGet()
    {
        var all = _repo.GetAll().ToList();
        Itens = all.Select(p => new Linha
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Nome = p.Nome,
            TipoTexto = TipoToText(p.Tipo),
            GrupoNome = p.GrupoId.HasValue ? _grupos.GetById(p.GrupoId.Value)?.Nome : null
        }).OrderBy(x => x.Nome).ToList();
    }

    public class Linha
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string TipoTexto { get; set; } = string.Empty;
        public string? GrupoNome { get; set; }
    }

    private string TipoToText(TipoProduto t)
    {
        return t switch
        {
            TipoProduto.MateriaPrima => "Matéria Prima",
            TipoProduto.ProdutoAcabado => "Produto Acabado",
            TipoProduto.ProdutoSemiacabado => "Produto Semiacabado",
            TipoProduto.Embalagem => "Embalagem",
            TipoProduto.Servico => "Serviço",
            TipoProduto.Imobilizado => "Imobilizado",
            TipoProduto.Outros => "Outros",
            _ => t.ToString()
        };
    }
}