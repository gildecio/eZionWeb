using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Documentos.Requisicoes;

public class DeleteModel : PageModel
{
    private readonly IDocumentoService _docs;

    [BindProperty]
    public eZionWeb.Estoque.Models.RequisicaoEstoque? Item { get; set; }

    public DeleteModel(IDocumentoService docs)
    {
        _docs = docs;
    }

    public IActionResult OnGet(int id)
    {
        Item = _docs.GetRequisicoes().FirstOrDefault(d => d.Id == id);
        if (Item == null) return RedirectToPage("Index");
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Item != null)
        {
            _docs.DeleteDocumento(Item.Id);
        }
        return RedirectToPage("Index");
    }
}