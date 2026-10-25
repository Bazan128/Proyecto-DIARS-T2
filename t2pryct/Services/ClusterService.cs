using t2pryct.Data.Repositories;
using t2pryct.Models;
using t2pryct.Models.Validaciones;
using t2pryct.ViewModels;

namespace t2pryct.Services;

public interface IClusterService
{
    Task<List<ClusterListaItemViewModel>> ListarAsync();
    Task<ClusterFormViewModel?> ObtenerFormularioAsync(int? id);
    Task<Cluster?> ObtenerDetalleAsync(int id);

    Task<(ReporteDetalleViewModel? Fila, string? Error)> ResolverReporteAsync(string? codigo, ClusterFormViewModel formulario);

    Task<ResultadoOperacion> GuardarAsync(ClusterFormViewModel formulario);
    Task<ResultadoOperacion> EliminarAsync(int id);
}

public class ClusterService : IClusterService
{
    private readonly IClusterRepository _clusters;
    private readonly IDenunciadoRepository _denunciados;
    private readonly IReporteRepository _reportes;
    private readonly IAuditRepository _auditoria;
    private readonly IRiesgoService _riesgo;

    public ClusterService(IClusterRepository clusters, IDenunciadoRepository denunciados, IReporteRepository reportes,
                          IAuditRepository auditoria, IRiesgoService riesgo)
    {
        _clusters = clusters;
        _denunciados = denunciados;
        _reportes = reportes;
        _auditoria = auditoria;
        _riesgo = riesgo;
    }

    public async Task<List<ClusterListaItemViewModel>> ListarAsync() =>
        (await _clusters.ListarConResumenAsync()).Select(c => new ClusterListaItemViewModel
        {
            Id = c.Id,
            Codigo = c.Codigo,
            NombreRed = c.NombreRed,
            Distrito = c.Distrito?.Nombre ?? "",
            Rubro = c.Rubro?.Nombre ?? "",
            Severidad = c.Severidad,
            Estado = c.Estado,
            TotalDenunciados = c.Denunciados.Count,
            TotalReportes = c.Reportes.Count
        }).ToList();

    public Task<Cluster?> ObtenerDetalleAsync(int id) => _clusters.ObtenerConDetalleSoloLecturaAsync(id);

    public async Task<ClusterFormViewModel?> ObtenerFormularioAsync(int? id)
    {
        if (id == null) return new ClusterFormViewModel { Codigo = "(se genera al guardar)" };

        var c = await _clusters.ObtenerConDetalleAsync(id.Value);
        if (c == null) return null;

        return new ClusterFormViewModel
        {
            Id = c.Id,
            Codigo = c.Codigo,
            NombreRed = c.NombreRed,
            DistritoId = c.DistritoId,
            RubroId = c.RubroId,
            Severidad = c.Severidad,
            Estado = c.Estado,
            FiscalAsignado = c.FiscalAsignado,
            Observaciones = c.Observaciones,
            Denunciados = c.Denunciados.OrderBy(d => d.FechaInclusionCluster).Select(d => new DenunciadoDetalleViewModel
            {
                Id = d.Id,
                Tipo = d.Tipo,
                Valor = d.Valor,
                Titular = d.Titular,
                Puntaje = d.PerfilRiesgo?.Puntaje ?? 0,
                FechaInclusion = d.FechaInclusionCluster
            }).ToList(),
            Reportes = c.Reportes.OrderBy(r => r.Fecha).Select(ConvertirFila).ToList()
        };
    }

    public async Task<(ReporteDetalleViewModel? Fila, string? Error)> ResolverReporteAsync(string? codigo, ClusterFormViewModel formulario)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return (null, "Ingrese el código del reporte que desea asociar (ej. REP-20260901-0001).");

        var reporte = await _reportes.ObtenerPorCodigoAsync(codigo.Trim().ToUpperInvariant());
        if (reporte == null)
            return (null, $"No existe ningún reporte con el código {codigo.Trim().ToUpperInvariant()}.");

        if (formulario.Reportes.Any(r => r.ReporteId == reporte.Id && !r.Eliminar))
            return (null, "El reporte ya está asociado a este expediente.");

        if (reporte.ClusterId != null && reporte.ClusterId != formulario.Id)
            return (null, "El reporte ya pertenece a otro expediente. Desvincúlelo primero.");

        return (ConvertirFila(reporte), null);
    }

    public async Task<ResultadoOperacion> GuardarAsync(ClusterFormViewModel f)
    {
        var resultado = new ResultadoOperacion();
        var esNuevo = f.Id == 0;

        Cluster? cluster = null;
        EstadoExpediente? estadoAnterior = null;
        if (!esNuevo)
        {
            cluster = await _clusters.ObtenerConDetalleAsync(f.Id);
            if (cluster == null) return resultado.Error(string.Empty, "El expediente no existe.");
            estadoAnterior = cluster.Estado;
        }

        if (estadoAnterior == EstadoExpediente.Cerrado && f.Estado == EstadoExpediente.Cerrado)
            return resultado.Error(nameof(f.Estado), "El expediente está cerrado. Cambie el estado para reabrirlo antes de modificarlo.");

        var denunciadosVigentes = f.Denunciados.Where(d => !d.Eliminar).ToList();
        var reportesVigentes = f.Reportes.Where(r => !r.Eliminar).ToList();

        if (f.Estado != EstadoExpediente.EnInvestigacion && (denunciadosVigentes.Count == 0 || reportesVigentes.Count == 0))
            resultado.Error(nameof(f.Estado), "Para derivar a Fiscalía o cerrar el caso, el expediente debe tener al menos un denunciado y un reporte asociados.");

        if (f.Estado == EstadoExpediente.DerivadoFiscalia && string.IsNullOrWhiteSpace(f.FiscalAsignado))
            resultado.Error(nameof(f.FiscalAsignado), "Indique el fiscal o investigador asignado para derivar a Fiscalía.");

        var vistos = new HashSet<string>();
        for (var i = 0; i < f.Denunciados.Count; i++)
        {
            var fila = f.Denunciados[i];
            if (fila.Eliminar) continue;

            if (fila.Id == 0)
            {
                var error = ReglasDenunciado.Validar(fila.Tipo, fila.Valor);
                if (error != null) { resultado.Error($"Denunciados[{i}].Valor", error); continue; }
                fila.Valor = ReglasDenunciado.Normalizar(fila.Valor);
            }
            if (!vistos.Add(fila.Valor))
                resultado.Error($"Denunciados[{i}].Valor", "El número o cuenta está repetido en el expediente.");
        }

        if (!resultado.Exitoso) return resultado;

        if (esNuevo)
        {
            cluster = new Cluster
            {
                Codigo = await _clusters.GenerarCodigoAsync(DateTime.Now.Year),
                FechaCreacion = DateTime.Now
            };
            await _clusters.AgregarAsync(cluster);
        }

        cluster!.NombreRed = f.NombreRed.Trim();
        cluster.DistritoId = f.DistritoId;
        cluster.RubroId = f.RubroId;
        cluster.Severidad = f.Severidad;
        cluster.Estado = f.Estado;
        cluster.FiscalAsignado = string.IsNullOrWhiteSpace(f.FiscalAsignado) ? null : f.FiscalAsignado.Trim();
        cluster.Observaciones = string.IsNullOrWhiteSpace(f.Observaciones) ? null : f.Observaciones.Trim();

        var afectados = new List<Denunciado>();
        for (var i = 0; i < f.Denunciados.Count; i++)
        {
            var fila = f.Denunciados[i];

            if (fila.Id > 0)
            {
                var existente = cluster.Denunciados.FirstOrDefault(d => d.Id == fila.Id);
                if (existente == null) continue;

                if (fila.Eliminar)
                {
                    existente.ClusterId = null;
                    existente.Cluster = null;
                    existente.FechaInclusionCluster = null;
                    cluster.Denunciados.Remove(existente);
                }
                else
                {
                    existente.Titular = string.IsNullOrWhiteSpace(fila.Titular) ? null : fila.Titular.Trim();
                }
                afectados.Add(existente);
                continue;
            }

            if (fila.Eliminar) continue;

            var buscado = await _denunciados.ObtenerPorValorAsync(fila.Valor);
            Denunciado denunciado;
            if (buscado == null)
            {
                denunciado = new Denunciado { Tipo = fila.Tipo, Valor = fila.Valor, FechaRegistro = DateTime.Now };
                await _denunciados.AgregarAsync(denunciado);
            }
            else
            {
                if (buscado.ClusterId != null && buscado.ClusterId != cluster.Id)
                    return resultado.Error($"Denunciados[{i}].Valor", "Ese número o cuenta ya pertenece a otro expediente.");
                denunciado = (await _denunciados.ObtenerConHistorialAsync(buscado.Id))!;
            }

            denunciado.Titular = string.IsNullOrWhiteSpace(fila.Titular) ? denunciado.Titular : fila.Titular.Trim();
            denunciado.Cluster = cluster;
            denunciado.FechaInclusionCluster ??= DateTime.Now;
            cluster.Denunciados.Add(denunciado);
            afectados.Add(denunciado);
        }

        foreach (var fila in f.Reportes)
        {
            var reporte = await _reportes.ObtenerPorCodigoAsync(fila.Codigo);
            if (reporte == null) continue;

            if (fila.Eliminar)
            {
                if (reporte.ClusterId == cluster.Id && cluster.Id != 0) reporte.ClusterId = null;
                var asociado = cluster.Reportes.FirstOrDefault(r => r.Id == reporte.Id);
                if (asociado != null) cluster.Reportes.Remove(asociado);
                continue;
            }

            if (reporte.ClusterId != null && reporte.ClusterId != cluster.Id)
                return resultado.Error(string.Empty, $"El reporte {reporte.Codigo} pertenece a otro expediente.");

            if (!cluster.Reportes.Any(r => r.Id == reporte.Id))
                cluster.Reportes.Add(reporte);
        }

        foreach (var d in afectados.Where(x => x.Id > 0).DistinctBy(x => x.Id))
        {
            var completo = await _denunciados.ObtenerConHistorialAsync(d.Id);
            if (completo != null) _riesgo.AplicarPerfil(completo);
        }
        foreach (var d in afectados.Where(x => x.Id == 0))
            _riesgo.AplicarPerfil(d);

        await _auditoria.AgregarAsync(new AuditLog
        {
            Accion = esNuevo ? "CREAR" : "MODIFICAR",
            Entidad = "Expediente",
            EntidadId = cluster.Id == 0 ? null : cluster.Id,
            Detalle = $"{cluster.Codigo}: estado {cluster.Estado}, {cluster.Denunciados.Count} denunciado(s), {cluster.Reportes.Count} reporte(s)"
        });

        await _clusters.GuardarCambiosAsync();

        resultado.Id = cluster.Id;
        resultado.Codigo = cluster.Codigo;
        return resultado;
    }

    public async Task<ResultadoOperacion> EliminarAsync(int id)
    {
        var resultado = new ResultadoOperacion();
        var cluster = await _clusters.ObtenerConDetalleAsync(id);
        if (cluster == null) return resultado.Error(string.Empty, "El expediente no existe.");

        if (cluster.Estado == EstadoExpediente.DerivadoFiscalia)
            return resultado.Error(string.Empty, "No se puede eliminar un expediente derivado a Fiscalía.");

        var codigo = cluster.Codigo;
        var denunciados = cluster.Denunciados.ToList();

        foreach (var d in denunciados) { d.ClusterId = null; d.Cluster = null; d.FechaInclusionCluster = null; }
        foreach (var r in cluster.Reportes.ToList()) r.ClusterId = null;

        _clusters.Eliminar(cluster);
        await _auditoria.AgregarAsync(new AuditLog { Accion = "ELIMINAR", Entidad = "Expediente", EntidadId = id, Detalle = $"Expediente {codigo} eliminado" });
        await _clusters.GuardarCambiosAsync();

        foreach (var d in denunciados)
        {
            var completo = await _denunciados.ObtenerConHistorialAsync(d.Id);
            if (completo != null) _riesgo.AplicarPerfil(completo);
        }
        await _clusters.GuardarCambiosAsync();
        return resultado;
    }

    private static ReporteDetalleViewModel ConvertirFila(Reporte r) => new()
    {
        ReporteId = r.Id,
        Codigo = r.Codigo,
        Fecha = r.Fecha,
        Mensaje = r.Mensaje,
        Monto = r.MontoExigido,
        Evidencias = r.Evidencias.Count
    };
}
