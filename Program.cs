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
builder.Services.AddSingleton<eZionWeb.Configuracoes.Services.ISequenciaRepository, eZionWeb.Configuracoes.Services.SequenciaRepository>();
builder.Services.AddSingleton<eZionWeb.Estoque.Services.IDocumentoService, eZionWeb.Estoque.Services.DocumentoService>();
builder.Services.AddSingleton<eZionWeb.Contabil.Services.IPlanoContaRepository, eZionWeb.Contabil.Services.PlanoContaRepository>();
builder.Services.AddSingleton<eZionWeb.Contabil.Services.ILancamentoRepository, eZionWeb.Contabil.Services.LancamentoRepository>();
builder.Services.AddSingleton<eZionWeb.Contabil.Services.IEmpresaRepository, eZionWeb.Contabil.Services.EmpresaRepository>();

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
    options.Conventions.AddFolderRouteModelConvention("/Contabil/Pages", model =>
    {
        foreach (var selector in model.Selectors.ToList())
        {
            var template = selector.AttributeRouteModel?.Template;
            if (!string.IsNullOrEmpty(template) && template.StartsWith("Contabil/Pages/"))
            {
                var newTemplate = template.Replace("Contabil/Pages/", "Contabil/");
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
    var seqs = scope.ServiceProvider.GetRequiredService<eZionWeb.Configuracoes.Services.ISequenciaRepository>();
    var contasRepo = scope.ServiceProvider.GetRequiredService<eZionWeb.Contabil.Services.IPlanoContaRepository>();
    var lancRepo = scope.ServiceProvider.GetRequiredService<eZionWeb.Contabil.Services.ILancamentoRepository>();
    var empRepo = scope.ServiceProvider.GetRequiredService<eZionWeb.Contabil.Services.IEmpresaRepository>();
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

    if (!seqs.GetAll().Any())
    {
        seqs.Add(new eZionWeb.Configuracoes.Models.Sequencia { Documento = "Ajuste", Serie = "0", Tipo = eZionWeb.Configuracoes.Models.TipoSequencia.Anual, Atual = 0 });
        seqs.Add(new eZionWeb.Configuracoes.Models.Sequencia { Documento = "Requisicao", Serie = "0", Tipo = eZionWeb.Configuracoes.Models.TipoSequencia.Anual, Atual = 0 });
        seqs.Add(new eZionWeb.Configuracoes.Models.Sequencia { Documento = "Devolucao", Serie = "0", Tipo = eZionWeb.Configuracoes.Models.TipoSequencia.Anual, Atual = 0 });
        seqs.Add(new eZionWeb.Configuracoes.Models.Sequencia { Documento = "Transferencia", Serie = "0", Tipo = eZionWeb.Configuracoes.Models.TipoSequencia.Anual, Atual = 0 });
    }

    if (!empRepo.GetAll().Any())
    {
        empRepo.Add(new eZionWeb.Contabil.Models.Empresa { Nome = "Empresa Exemplo Ltda", Cnpj = "00.000.000/0001-00" });
    }
    var empresaDefault = empRepo.GetAll().FirstOrDefault();
    if (!contasRepo.GetAll().Any())
    {
        var caixa = contasRepo.Add(new eZionWeb.Contabil.Models.PlanoConta { EmpresaId = empresaDefault?.Id ?? 1, Codigo = "1.1.1", Nome = "Caixa" });
        var banco = contasRepo.Add(new eZionWeb.Contabil.Models.PlanoConta { EmpresaId = empresaDefault?.Id ?? 1, Codigo = "1.1.2", Nome = "Banco" });
        var receita = contasRepo.Add(new eZionWeb.Contabil.Models.PlanoConta { EmpresaId = empresaDefault?.Id ?? 1, Codigo = "3.1.1", Nome = "Receitas" });
        var despesa = contasRepo.Add(new eZionWeb.Contabil.Models.PlanoConta { EmpresaId = empresaDefault?.Id ?? 1, Codigo = "4.1.1", Nome = "Despesas" });

        lancRepo.Add(new eZionWeb.Contabil.Models.Lancamento { EmpresaId = empresaDefault?.Id ?? 1, Data = DateTime.UtcNow.AddDays(-7).Date, PlanoContaId = receita.Id, Tipo = eZionWeb.Contabil.Models.TipoLancamento.Credito, Historico = "Venda serviço", Valor = 1500m });
        lancRepo.Add(new eZionWeb.Contabil.Models.Lancamento { EmpresaId = empresaDefault?.Id ?? 1, Data = DateTime.UtcNow.AddDays(-6).Date, PlanoContaId = caixa.Id, Tipo = eZionWeb.Contabil.Models.TipoLancamento.Debito, Historico = "Recebimento em caixa", Valor = 1500m });
        lancRepo.Add(new eZionWeb.Contabil.Models.Lancamento { EmpresaId = empresaDefault?.Id ?? 1, Data = DateTime.UtcNow.AddDays(-3).Date, PlanoContaId = despesa.Id, Tipo = eZionWeb.Contabil.Models.TipoLancamento.Debito, Historico = "Compra material", Valor = 200m });
        lancRepo.Add(new eZionWeb.Contabil.Models.Lancamento { EmpresaId = empresaDefault?.Id ?? 1, Data = DateTime.UtcNow.AddDays(-2).Date, PlanoContaId = banco.Id, Tipo = eZionWeb.Contabil.Models.TipoLancamento.Credito, Historico = "Depósito bancário", Valor = 1000m });
    }

    var docsSvc = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.IDocumentoService>();
    var movRepo = scope.ServiceProvider.GetRequiredService<eZionWeb.Estoque.Services.IMovimentoRepository>();
    if (!movRepo.GetAll().Any())
    {
        var prodsAll = repo.GetAll().ToList();
        var locsAll = locais.GetAll().ToList();
        var almox = locsAll.FirstOrDefault(x => x.Nome.Contains("Almoxarifado")) ?? locsAll.First();
        var deposito = locsAll.FirstOrDefault(x => x.Nome.Contains("Depósito")) ?? (locsAll.Count > 1 ? locsAll[1] : almox);
        var p1 = prodsAll.ElementAtOrDefault(0);
        var p2 = prodsAll.ElementAtOrDefault(1);
        if (p1 != null && p2 != null)
        {
        // saldo inicial
        docsSvc.AddAjuste(new eZionWeb.Estoque.Models.AjusteEstoque
        {
            Data = DateTime.UtcNow.AddDays(-14).Date,
            LocalId = almox.Id,
            Entrada = true,
            Observacao = "Saldo inicial",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 50 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 40 }
            }
        });

        // Ajustes (3 docs, >=2 itens)
        docsSvc.AddAjuste(new eZionWeb.Estoque.Models.AjusteEstoque
        {
            Data = DateTime.UtcNow.AddDays(-10).Date,
            LocalId = almox.Id,
            Entrada = true,
            Observacao = "Entrada complementar",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 10 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 8 }
            }
        });
        docsSvc.AddAjuste(new eZionWeb.Estoque.Models.AjusteEstoque
        {
            Data = DateTime.UtcNow.AddDays(-8).Date,
            LocalId = almox.Id,
            Entrada = false,
            Observacao = "Ajuste de saída",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 5 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 4 }
            }
        });
        docsSvc.AddAjuste(new eZionWeb.Estoque.Models.AjusteEstoque
        {
            Data = DateTime.UtcNow.AddDays(-6).Date,
            LocalId = almox.Id,
            Entrada = true,
            Observacao = "Reposição",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 7 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 6 }
            }
        });

        // Requisições (3 docs, >=2 itens)
        docsSvc.AddRequisicao(new eZionWeb.Estoque.Models.RequisicaoEstoque
        {
            Data = DateTime.UtcNow.AddDays(-5).Date,
            LocalOrigemId = almox.Id,
            Observacao = "Consumo operação A",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 6 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 5 }
            }
        });
        docsSvc.AddRequisicao(new eZionWeb.Estoque.Models.RequisicaoEstoque
        {
            Data = DateTime.UtcNow.AddDays(-3).Date,
            LocalOrigemId = almox.Id,
            Observacao = "Consumo operação B",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 4 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 3 }
            }
        });
        docsSvc.AddRequisicao(new eZionWeb.Estoque.Models.RequisicaoEstoque
        {
            Data = DateTime.UtcNow.AddDays(-2).Date,
            LocalOrigemId = almox.Id,
            Observacao = "Consumo operação C",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 3 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 2 }
            }
        });

        // Devoluções (3 docs, >=2 itens)
        docsSvc.AddDevolucao(new eZionWeb.Estoque.Models.DevolucaoEstoque
        {
            Data = DateTime.UtcNow.AddDays(-4).Date,
            LocalDestinoId = almox.Id,
            Observacao = "Retorno materiais X",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 2 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 1 }
            }
        });
        docsSvc.AddDevolucao(new eZionWeb.Estoque.Models.DevolucaoEstoque
        {
            Data = DateTime.UtcNow.AddDays(-2).Date,
            LocalDestinoId = almox.Id,
            Observacao = "Retorno materiais Y",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 1 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 1 }
            }
        });
        docsSvc.AddDevolucao(new eZionWeb.Estoque.Models.DevolucaoEstoque
        {
            Data = DateTime.UtcNow.AddDays(-1).Date,
            LocalDestinoId = almox.Id,
            Observacao = "Retorno materiais Z",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 2 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 2 }
            }
        });

        // Transferências (3 docs, >=2 itens)
        docsSvc.AddTransferencia(new eZionWeb.Estoque.Models.TransferenciaEstoque
        {
            Data = DateTime.UtcNow.AddDays(-7).Date,
            LocalOrigemId = almox.Id,
            LocalDestinoId = deposito.Id,
            Observacao = "Envio para depósito",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 5 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 4 }
            }
        });
        docsSvc.AddTransferencia(new eZionWeb.Estoque.Models.TransferenciaEstoque
        {
            Data = DateTime.UtcNow.AddDays(-5).Date,
            LocalOrigemId = almox.Id,
            LocalDestinoId = deposito.Id,
            Observacao = "Envio complementar",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 3 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 3 }
            }
        });
        docsSvc.AddTransferencia(new eZionWeb.Estoque.Models.TransferenciaEstoque
        {
            Data = DateTime.UtcNow.AddDays(-3).Date,
            LocalOrigemId = deposito.Id,
            LocalDestinoId = almox.Id,
            Observacao = "Retorno parcial",
            Itens = new List<eZionWeb.Estoque.Models.DocumentoItem>
            {
                new() { ProdutoId = p1.Id, UnidadeId = p1.UnidadeId, Quantidade = 2 },
                new() { ProdutoId = p2.Id, UnidadeId = p2.UnidadeId, Quantidade = 2 }
            }
        });
        }
    }
}

app.Run();
