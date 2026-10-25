using System.Globalization;
using t2pryct.Data.Repositories;
using t2pryct.Models;
using t2pryct.ViewModels;

namespace t2pryct.Services;

public interface IPanelService
{
    Task<DashboardViewModel> ObtenerDashboardAsync();
    Task<MapaCalorViewModel> ObtenerMapaAsync(MapaCalorViewModel filtros);
    Task<List<AuditLog>> ObtenerAuditoriaAsync(int cantidad);
}

public class PanelService : IPanelService
{
    private readonly IReporteRepository _reportes;
    private readonly IClusterRepository _clusters;
    private readonly IDenunciadoRepository _denunciados;
    private readonly IAuditRepository _auditoria;
    private readonly ICorrelacionService _correlacion;

    public PanelService(IReporteRepository reportes, IClusterRepository clusters, IDenunciadoRepository denunciados,
                        IAuditRepository auditoria, ICorrelacionService correlacion)
    {
        _reportes = reportes;
        _clusters = clusters;
        _denunciados = denunciados;
        _auditoria = auditoria;
        _correlacion = correlacion;
    }

    public async Task<DashboardViewModel> ObtenerDashboardAsync()
    {
        var reportes = await _reportes.ListarConDetalleAsync();
        var clusters = await _clusters.ListarConResumenAsync();
        var denunciados = await _denunciados.ListarConPerfilAsync();
        var sugerencias = await _correlacion.ObtenerSugerenciasAsync();

        var porDistrito = reportes.GroupBy(r => r.Distrito?.Nombre ?? "-")
                                  .Select(g => new { g.Key, Total = g.Count() })
                                  .OrderByDescending(x => x.Total).FirstOrDefault();

        var calendario = CultureInfo.CurrentCulture.Calendar;
        var semanas = reportes
            .GroupBy(r => InicioSemana(r.Fecha))
            .OrderByDescending(g => g.Key).Take(8).OrderBy(g => g.Key)
            .Select(g => new ContadorViewModel { Etiqueta = $"Semana del {g.Key:dd/MM}", Cantidad = g.Count() })
            .ToList();

        return new DashboardViewModel
        {
            TotalReportes = reportes.Count,
            ExpedientesActivos = clusters.Count(c => c.Estado != EstadoExpediente.Cerrado),
            NumerosCriticos = denunciados.Count(d => d.PerfilRiesgo?.Semaforo == NivelSemaforo.Rojo),
            DistritoMayorAlerta = porDistrito?.Key ?? "-",
            ReportesPorSemana = semanas,
            ReportesPorModalidad = reportes.GroupBy(r => r.Modalidad?.Nombre ?? "-")
                .Select(g => new ContadorViewModel { Etiqueta = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad).ToList(),
            TopDenunciados = denunciados.Take(5).Select(d => new DenunciadoResumenViewModel
            {
                Valor = d.Valor,
                Tipo = d.Tipo,
                Puntaje = d.PerfilRiesgo?.Puntaje ?? 0,
                Semaforo = d.PerfilRiesgo?.Semaforo ?? NivelSemaforo.Verde,
                Reportes = d.ReporteDenunciados.Count,
                Expediente = d.Cluster?.Codigo
            }).ToList(),
            SugerenciasPendientes = sugerencias.Count
        };
    }

    public async Task<MapaCalorViewModel> ObtenerMapaAsync(MapaCalorViewModel filtros)
    {
        var reportes = (await _reportes.ListarConDetalleAsync()).AsEnumerable();

        if (filtros.DistritoId != null) reportes = reportes.Where(r => r.DistritoId == filtros.DistritoId);
        if (filtros.RubroId != null) reportes = reportes.Where(r => r.RubroId == filtros.RubroId);
        if (filtros.Desde != null) reportes = reportes.Where(r => r.Fecha.Date >= filtros.Desde.Value.Date);
        if (filtros.Hasta != null) reportes = reportes.Where(r => r.Fecha.Date <= filtros.Hasta.Value.Date);

        var lista = reportes.ToList();
        filtros.PorDistrito = lista.GroupBy(r => r.Distrito?.Nombre ?? "-")
            .Select(g => new ContadorViewModel { Etiqueta = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad).ToList();
        filtros.PorRubro = lista.GroupBy(r => r.Rubro?.Nombre ?? "-")
            .Select(g => new ContadorViewModel { Etiqueta = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad).ToList();
        filtros.Total = lista.Count;
        filtros.Maximo = filtros.PorDistrito.Select(x => x.Cantidad).DefaultIfEmpty(0).Max();
        return filtros;
    }

    public Task<List<AuditLog>> ObtenerAuditoriaAsync(int cantidad) => _auditoria.RecientesAsync(cantidad);

    private static DateTime InicioSemana(DateTime fecha)
    {
        var dia = (int)fecha.DayOfWeek;
        var retroceso = dia == 0 ? 6 : dia - 1;
        return fecha.Date.AddDays(-retroceso);
    }
}
