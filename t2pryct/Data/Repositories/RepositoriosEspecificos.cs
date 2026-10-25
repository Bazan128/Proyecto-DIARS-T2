using Microsoft.EntityFrameworkCore;
using t2pryct.Models;

namespace t2pryct.Data.Repositories;

public interface IReporteRepository : IRepository<Reporte>
{
    Task<Reporte?> ObtenerPorCodigoAsync(string codigo);
    Task<Reporte?> ObtenerConDetalleAsync(int id);
    Task<List<Reporte>> ListarConDetalleAsync();
    Task<int> ContarPorIpDesdeAsync(string ipHash, DateTime desde);
    Task<int> ContarDelDiaAsync(DateTime dia);
}

public class ReporteRepository : Repository<Reporte>, IReporteRepository
{
    public ReporteRepository(EscudoLibertadDbContext contexto) : base(contexto) { }

    public Task<Reporte?> ObtenerPorCodigoAsync(string codigo) =>
        _dbSet.Include(r => r.ReporteDenunciados)
              .Include(r => r.Evidencias)
              .FirstOrDefaultAsync(r => r.Codigo == codigo);

    public Task<Reporte?> ObtenerConDetalleAsync(int id) =>
        _dbSet.AsNoTracking()
              .Include(r => r.Distrito).Include(r => r.Rubro).Include(r => r.Modalidad)
              .Include(r => r.Evidencias)
              .Include(r => r.ReporteDenunciados).ThenInclude(x => x.Denunciado)
              .FirstOrDefaultAsync(r => r.Id == id);

    public Task<List<Reporte>> ListarConDetalleAsync() =>
        _dbSet.AsNoTracking()
              .Include(r => r.Distrito).Include(r => r.Rubro).Include(r => r.Modalidad)
              .Include(r => r.Evidencias)
              .Include(r => r.ReporteDenunciados)
              .OrderByDescending(r => r.Fecha)
              .ToListAsync();

    public Task<int> ContarPorIpDesdeAsync(string ipHash, DateTime desde) =>
        _dbSet.CountAsync(r => r.IpHash == ipHash && r.Fecha >= desde);

    public Task<int> ContarDelDiaAsync(DateTime dia)
    {
        var inicio = dia.Date;
        var fin = inicio.AddDays(1);
        return _dbSet.CountAsync(r => r.Fecha >= inicio && r.Fecha < fin);
    }
}

public interface IDenunciadoRepository : IRepository<Denunciado>
{
    Task<Denunciado?> ObtenerPorValorAsync(string valor);
    Task<Denunciado?> ObtenerConHistorialAsync(int id);
    Task<Denunciado?> BuscarConHistorialAsync(string valor);
    Task<List<Denunciado>> ListarConPerfilAsync();
}

public class DenunciadoRepository : Repository<Denunciado>, IDenunciadoRepository
{
    public DenunciadoRepository(EscudoLibertadDbContext contexto) : base(contexto) { }

    public Task<Denunciado?> ObtenerPorValorAsync(string valor) =>
        _dbSet.Include(d => d.PerfilRiesgo).FirstOrDefaultAsync(d => d.Valor == valor);

    public Task<Denunciado?> ObtenerConHistorialAsync(int id) =>
        _dbSet.Include(d => d.PerfilRiesgo)
              .Include(d => d.ReporteDenunciados).ThenInclude(x => x.Reporte).ThenInclude(r => r!.Evidencias)
              .FirstOrDefaultAsync(d => d.Id == id);

    public Task<Denunciado?> BuscarConHistorialAsync(string valor) =>
        _dbSet.AsNoTracking()
              .Include(d => d.PerfilRiesgo)
              .Include(d => d.Cluster)
              .Include(d => d.ReporteDenunciados).ThenInclude(x => x.Reporte).ThenInclude(r => r!.Distrito)
              .Include(d => d.ReporteDenunciados).ThenInclude(x => x.Reporte).ThenInclude(r => r!.Evidencias)
              .FirstOrDefaultAsync(d => d.Valor == valor);

    public Task<List<Denunciado>> ListarConPerfilAsync() =>
        _dbSet.AsNoTracking()
              .Include(d => d.PerfilRiesgo).Include(d => d.Cluster)
              .Include(d => d.ReporteDenunciados)
              .OrderByDescending(d => d.PerfilRiesgo!.Puntaje)
              .ToListAsync();
}

public interface IClusterRepository : IRepository<Cluster>
{
    Task<List<Cluster>> ListarConResumenAsync();

    Task<Cluster?> ObtenerConDetalleAsync(int id);

    Task<Cluster?> ObtenerConDetalleSoloLecturaAsync(int id);
    Task<string> GenerarCodigoAsync(int anio);
}

public class ClusterRepository : Repository<Cluster>, IClusterRepository
{
    public ClusterRepository(EscudoLibertadDbContext contexto) : base(contexto) { }

    public Task<List<Cluster>> ListarConResumenAsync() =>
        _dbSet.AsNoTracking()
              .Include(c => c.Distrito).Include(c => c.Rubro)
              .Include(c => c.Denunciados).Include(c => c.Reportes)
              .OrderByDescending(c => c.FechaCreacion)
              .ToListAsync();

    public Task<Cluster?> ObtenerConDetalleAsync(int id) =>
        _dbSet.Include(c => c.Distrito).Include(c => c.Rubro)
              .Include(c => c.Denunciados).ThenInclude(d => d.PerfilRiesgo)
              .Include(c => c.Reportes).ThenInclude(r => r.Evidencias)
              .FirstOrDefaultAsync(c => c.Id == id);

    public Task<Cluster?> ObtenerConDetalleSoloLecturaAsync(int id) =>
        _dbSet.AsNoTracking()
              .Include(c => c.Distrito).Include(c => c.Rubro)
              .Include(c => c.Denunciados).ThenInclude(d => d.PerfilRiesgo)
              .Include(c => c.Reportes).ThenInclude(r => r.Evidencias)
              .Include(c => c.Reportes).ThenInclude(r => r.Modalidad)
              .Include(c => c.Reportes).ThenInclude(r => r.Distrito)
              .AsSplitQuery()
              .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<string> GenerarCodigoAsync(int anio)
    {
        var prefijo = $"EXP-{anio}-TRU-";
        var existentes = await _dbSet.Where(c => c.Codigo.StartsWith(prefijo)).Select(c => c.Codigo).ToListAsync();
        var max = existentes
            .Select(c => int.TryParse(c[prefijo.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{prefijo}{max + 1:0000}";
    }
}

public interface IAlertaRepository : IRepository<Alerta>
{
    Task<List<Alerta>> ListarConDetalleAsync();
}

public class AlertaRepository : Repository<Alerta>, IAlertaRepository
{
    public AlertaRepository(EscudoLibertadDbContext contexto) : base(contexto) { }

    public Task<List<Alerta>> ListarConDetalleAsync() =>
        _dbSet.AsNoTracking().Include(a => a.Distrito).Include(a => a.Rubro)
              .OrderByDescending(a => a.FechaPublicacion).ToListAsync();
}

public interface IAuditRepository : IRepository<AuditLog>
{
    Task<List<AuditLog>> RecientesAsync(int cantidad);
}

public class AuditRepository : Repository<AuditLog>, IAuditRepository
{
    public AuditRepository(EscudoLibertadDbContext contexto) : base(contexto) { }

    public Task<List<AuditLog>> RecientesAsync(int cantidad) =>
        _dbSet.AsNoTracking().OrderByDescending(a => a.Fecha).ThenByDescending(a => a.Id).Take(cantidad).ToListAsync();
}
