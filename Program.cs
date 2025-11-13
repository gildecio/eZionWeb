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
builder.Services.AddSingleton<eZionWeb.Estoque.Services.IGrupoRepository, eZionWeb.Estoque.Services.GrupoRepository>();
builder.Services.AddSingleton<eZionWeb.Estoque.Services.ILocalEstoqueRepository, eZionWeb.Estoque.Services.LocalEstoqueRepository>();
builder.Services.AddSingleton<eZionWeb.Estoque.Services.IMovimentoRepository, eZionWeb.Estoque.Services.MovimentoRepository>();
builder.Services.AddSingleton<eZionWeb.Estoque.Services.IUnidadeRepository, eZionWeb.Estoque.Services.UnidadeRepository>();
builder.Services.AddSingleton<eZionWeb.Estoque.Services.IConversaoRepository>(sp => (eZionWeb.Estoque.Services.UnidadeRepository)sp.GetRequiredService<eZionWeb.Estoque.Services.IUnidadeRepository>());
builder.Services.AddSingleton<eZionWeb.Estoque.Services.IDocumentoService, eZionWeb.Estoque.Services.DocumentoService>();

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
    options.Conventions.AddPageRoute("/Pages/Settings", "Settings");
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
    var grupos = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.IGrupoRepository>();
    var locais = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.ILocalEstoqueRepository>();
    var unidades = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.IUnidadeRepository>();
    var convs = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.IConversaoRepository>();
    if (!grupos.GetAll().Any())
    {
        var gInfo = grupos.Add(new eZionWeb.Estoque.Models.Grupo { Nome = "Informática" });
        var gOffice = grupos.Add(new eZionWeb.Estoque.Models.Grupo { Nome = "Escritório" });
        var gPerif = grupos.Add(new eZionWeb.Estoque.Models.Grupo { Nome = "Periféricos", ParentId = gInfo.Id });
        var gMon = grupos.Add(new eZionWeb.Estoque.Models.Grupo { Nome = "Monitores", ParentId = gInfo.Id });
        grupos.Add(new eZionWeb.Estoque.Models.Grupo { Nome = "Teclados", ParentId = gPerif.Id });
        grupos.Add(new eZionWeb.Estoque.Models.Grupo { Nome = "Mouses", ParentId = gPerif.Id });
        // gOffice mantido sem filhos para demonstrar múltiplos pais
    }

    if (!repo.GetAll().Any())
    {
        var leafTeclados = grupos.GetAll().FirstOrDefault(x => x.Nome == "Teclados");
        var leafMouses = grupos.GetAll().FirstOrDefault(x => x.Nome == "Mouses");
        var leafMonitores = grupos.GetAll().FirstOrDefault(x => x.Nome == "Monitores");
        if (leafTeclados != null)
            repo.Add(new eZionWeb.Estoque.Models.Produto { Nome = "Teclado Mecânico", GrupoId = leafTeclados.Id });
        if (leafMouses != null)
            repo.Add(new eZionWeb.Estoque.Models.Produto { Nome = "Mouse Gamer", GrupoId = leafMouses.Id });
        if (leafMonitores != null)
            repo.Add(new eZionWeb.Estoque.Models.Produto { Nome = "Monitor 24\"", GrupoId = leafMonitores.Id });
    }

    if (!locais.GetAll().Any())
    {
        locais.Add(new eZionWeb.Estoque.Models.LocalEstoque { Nome = "Almoxarifado Principal", Codigo = "ALM-001" });
        locais.Add(new eZionWeb.Estoque.Models.LocalEstoque { Nome = "Depósito Externo", Codigo = "DEP-EXT" });
    }

    if (!unidades.GetAll().Any())
    {
        var un = unidades.Add(new eZionWeb.Estoque.Models.UnidadeMedida { Sigla = "UN", Nome = "Unidade" });
        var kg = unidades.Add(new eZionWeb.Estoque.Models.UnidadeMedida { Sigla = "KG", Nome = "Quilograma" });
        var g = unidades.Add(new eZionWeb.Estoque.Models.UnidadeMedida { Sigla = "G", Nome = "Grama" });
        convs.Add(new eZionWeb.Estoque.Models.ConversaoUnidade { DeUnidadeId = g.Id, ParaUnidadeId = kg.Id, Fator = 0.001m });
        convs.Add(new eZionWeb.Estoque.Models.ConversaoUnidade { DeUnidadeId = kg.Id, ParaUnidadeId = g.Id, Fator = 1000m });
    }
}

app.Run();
