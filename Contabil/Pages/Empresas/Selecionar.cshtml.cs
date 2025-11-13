using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eZionWeb.Contabil.Pages.Empresas;

public class SelecionarModel : PageModel
{
    [BindProperty]
    public int EmpresaId { get; set; }

    public IActionResult OnPost()
    {
        Response.Cookies.Append("empresaId", EmpresaId.ToString(), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = false,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrWhiteSpace(referer)) return Redirect(referer);
        return Redirect("/");
    }
}