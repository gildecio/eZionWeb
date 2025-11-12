using Microsoft.AspNetCore.Mvc.ApplicationModels;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
    });

builder.Services.AddSingleton<eZionWeb.Estoque.Services.IProdutoRepository, eZionWeb.Estoque.Services.ProdutoRepository>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Pages/Login");
    options.RootDirectory = "/";
}).AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Pages/Index", "");
    options.Conventions.AddPageRoute("/Pages/Login", "Login");
    options.Conventions.AddPageRoute("/Pages/Privacy", "Privacy");
    options.Conventions.AddPageRoute("/Pages/Logout", "Logout");
    options.Conventions.AddFolderRouteModelConvention("/Estoque/Pages", model =>
    {
        foreach (var selector in model.Selectors.ToList())
        {
            var template = selector.AttributeRouteModel?.Template;
            if (!string.IsNullOrEmpty(template) && template.StartsWith("Estoque/Pages/"))
            {
                var newTemplate = template.Replace("Estoque/Pages/", "Estoque/");
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = newTemplate
                    }
                });
            }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.IProdutoRepository>();
    if (!repo.GetAll().Any())
    {
        repo.Add(new eZionWeb.Estoque.Models.Produto { Nome = "Teclado Mecânico", Preco = 350.00m, Quantidade = 10 });
        repo.Add(new eZionWeb.Estoque.Models.Produto { Nome = "Mouse Gamer", Preco = 199.90m, Quantidade = 25 });
        repo.Add(new eZionWeb.Estoque.Models.Produto { Nome = "Monitor 24\"", Preco = 899.00m, Quantidade = 5 });
    }
}

app.Run();
