using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eZionWeb.Pages;

public class SettingsModel : PageModel
{
    [BindProperty]
    public string SelectedLayout { get; set; } = "dark-purpura";

    public List<SelectListItem> Layouts { get; set; } = new()
    {
        new SelectListItem("Dark Purpura", "dark-purpura"),
        new SelectListItem("Light Azul", "light-azul")
    };

    public void OnGet()
    {
        var current = Request.Cookies["layout"];
        if (!string.IsNullOrWhiteSpace(current)) SelectedLayout = current;
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(SelectedLayout)) SelectedLayout = "dark-purpura";
        Response.Cookies.Append("layout", SelectedLayout, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = false,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
        return RedirectToPage("/Pages/Settings");
    }
}