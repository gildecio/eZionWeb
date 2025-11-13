using eZionWeb.Estoque.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Estoque.Pages.Documentos.Devolucoes;

public class DeleteModel : PageModel
{
    private readonly IDocumentoService _docs;

    [BindProperty]
    public eZionWeb.Estoque.Models.DevolucaoEstoque? Item { get; set; }

    public DeleteModel(IDocumentoService docs)
    {
        _docs = docs;
    }

    public IActionResult OnGet(int id)
    {
        Item = _docs.GetDevolucoes().FirstOrDefault(d => d.Id == id);
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