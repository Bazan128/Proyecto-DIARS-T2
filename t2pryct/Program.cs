using Microsoft.EntityFrameworkCore;
using t2pryct.Data;
using t2pryct.Data.Repositories;
using t2pryct.Models;
using t2pryct.Services;
using t2pryct.Services.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EscudoLibertadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EscudoLibertadDb")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IDenunciadoRepository, DenunciadoRepository>();
builder.Services.AddScoped<IClusterRepository, ClusterRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

builder.Services.AddScoped<IRiesgoStrategy, FrecuenciaReportesStrategy>();
builder.Services.AddScoped<IRiesgoStrategy, DispersionDistritosStrategy>();
builder.Services.AddScoped<IRiesgoStrategy, EvidenciasStrategy>();
builder.Services.AddScoped<IRiesgoStrategy, PertenenciaClusterStrategy>();
builder.Services.AddScoped<ICorrelacionStrategy, CoincidenciaExactaStrategy>();
builder.Services.AddScoped<ICorrelacionStrategy, SimilitudLevenshteinStrategy>();

builder.Services.AddScoped<IRiesgoService, RiesgoService>();
builder.Services.AddScoped<ICorrelacionService, CorrelacionService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IClusterService, ClusterService>();
builder.Services.AddScoped<IPanelService, PanelService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<ISelectorService, SelectorService>();
builder.Services.AddScoped<ICatalogoService<Distrito>, CatalogoService<Distrito>>();
builder.Services.AddScoped<ICatalogoService<Rubro>, CatalogoService<Rubro>>();
builder.Services.AddScoped<ICatalogoService<ModalidadExtorsion>, CatalogoService<ModalidadExtorsion>>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Verificador}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
