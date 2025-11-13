using eZionWeb.Contabil.Models;
using eZionWeb.Contabil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Contabil.Pages.Lancamentos;

public class CreateModel : PageModel
{
    private readonly ILancamentoRepository _repo;
    private readonly IPlanoContaRepository _contasRepo;
    private readonly IEmpresaRepository _empresasRepo;

    [BindProperty]
    public DateTime Data { get; set; } = DateTime.UtcNow.Date;
    [BindProperty]
    public int PlanoContaId { get; set; }
    [BindProperty]
    public int EmpresaId { get; set; }
    [BindProperty]
    public TipoLancamento Tipo { get; set; } = TipoLancamento.Debito;
    [BindProperty]
    public string Historico { get; set; } = string.Empty;
    [BindProperty]
    public decimal Valor { get; set; }

    public List<SelectListItem> Contas { get; set; } = new();

    public CreateModel(ILancamentoRepository repo, IPlanoContaRepository contas, IEmpresaRepository empresas)
    {
        _repo = repo;
        _contasRepo = contas;
        _empresasRepo = empresas;
    }

    public void OnGet()
    {
        var empCookie = Request.Cookies["empresaId"];
        if (!int.TryParse(empCookie, out var empId)) empId = _empresasRepo.GetAll().FirstOrDefault()?.Id ?? 0;
        EmpresaId = empId;
        var contasAll = _contasRepo.GetAll().Where(c => c.EmpresaId == EmpresaId).OrderBy(c => c.Codigo);
        Contas = contasAll.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Codigo + " - " + c.Nome }).ToList();
        if (Contas.Count > 0) PlanoContaId = int.Parse(Contas.First().Value);
    }

    public IActionResult OnPost()
    {
        _repo.Add(new Lancamento { EmpresaId = EmpresaId, Data = Data.Date, PlanoContaId = PlanoContaId, Tipo = Tipo, Historico = Historico ?? string.Empty, Valor = Valor });
        return RedirectToPage("Index");
    }
}