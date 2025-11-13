using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Contabil.Pages.Lancamentos;

public class IndexModel : PageModel
{
    private readonly ILancamentoRepository _repo;
    private readonly IPlanoContaRepository _contas;
    private readonly IEmpresaRepository _empresas;

    public List<Linha> Itens { get; set; } = new();
    public List<SelectListItem> Contas { get; set; } = new();
    public List<SelectListItem> Empresas { get; set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? De { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public DateTime? Ate { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? PlanoContaId { get; set; }
    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int? EmpresaId { get; set; }

    public IndexModel(ILancamentoRepository repo, IPlanoContaRepository contas, IEmpresaRepository empresas)
    {
        _repo = repo;
        _contas = contas;
        _empresas = empresas;
    }

    public void OnGet()
    {
        if (!EmpresaId.HasValue)
        {
            var empCookie = Request.Cookies["empresaId"];
            if (int.TryParse(empCookie, out var id)) EmpresaId = id;
        }
        var empresasDict = _empresas.GetAll().OrderBy(e => e.Nome).ToDictionary(e => e.Id, e => e.Nome);
        Empresas = _empresas.GetAll().OrderBy(e => e.Nome)
            .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nome }).ToList();

        var contasAll = _contas.GetAll().OrderBy(c => c.Codigo);
        if (EmpresaId.HasValue) contasAll = contasAll.Where(c => c.EmpresaId == EmpresaId.Value).OrderBy(c => c.Codigo);
        var contasDict = contasAll.ToDictionary(c => c.Id, c => c.Codigo + " - " + c.Nome);
        Contas = contasAll.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Codigo + " - " + c.Nome }).ToList();

        var query = _repo.GetAll().AsEnumerable();
        if (De.HasValue) query = query.Where(l => l.Data >= De.Value);
        if (Ate.HasValue) query = query.Where(l => l.Data <= Ate.Value);
        if (PlanoContaId.HasValue) query = query.Where(l => l.PlanoContaId == PlanoContaId.Value);
        if (EmpresaId.HasValue) query = query.Where(l => l.EmpresaId == EmpresaId.Value);

        Itens = query.Select(l => new Linha
        {
            Id = l.Id,
            Data = l.Data,
            EmpresaNome = empresasDict.TryGetValue(l.EmpresaId, out var en) ? en : l.EmpresaId.ToString(),
            ContaNome = contasDict.TryGetValue(l.PlanoContaId, out var cn) ? cn : l.PlanoContaId.ToString(),
            Historico = l.Historico,
            TipoTexto = l.Tipo == TipoLancamento.Debito ? "Débito" : "Crédito",
            Valor = l.Valor
        }).Take(500).ToList();
    }

    public class Linha
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string EmpresaNome { get; set; } = string.Empty;
        public string ContaNome { get; set; } = string.Empty;
        public string Historico { get; set; } = string.Empty;
        public string TipoTexto { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}