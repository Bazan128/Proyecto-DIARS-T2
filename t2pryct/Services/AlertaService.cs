using t2pryct.Data.Repositories;
using t2pryct.Models;

namespace t2pryct.Services;

public interface IAlertaService
{
    Task<List<Alerta>> ListarAsync();
    Task<Alerta?> ObtenerAsync(int id);
    Task CrearAsync(Alerta alerta);
    Task EditarAsync(Alerta alerta);
    Task EliminarAsync(int id);
}

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepository _alertas;
    private readonly IAuditRepository _auditoria;

    public AlertaService(IAlertaRepository alertas, IAuditRepository auditoria)
    {
        _alertas = alertas;
        _auditoria = auditoria;
    }

    public Task<List<Alerta>> ListarAsync() => _alertas.ListarConDetalleAsync();

    public Task<Alerta?> ObtenerAsync(int id) => _alertas.ObtenerPorIdAsync(id);

    public async Task CrearAsync(Alerta alerta)
    {
        alerta.FechaPublicacion = DateTime.Now;
        await _alertas.AgregarAsync(alerta);
        await _auditoria.AgregarAsync(new AuditLog { Accion = "CREAR", Entidad = "Alerta", Detalle = $"Alerta publicada: {alerta.Titulo}" });
        await _alertas.GuardarCambiosAsync();
    }

    public async Task EditarAsync(Alerta alerta)
    {
        _alertas.Actualizar(alerta);
        await _auditoria.AgregarAsync(new AuditLog { Accion = "MODIFICAR", Entidad = "Alerta", EntidadId = alerta.Id, Detalle = $"Alerta modificada: {alerta.Titulo}" });
        await _alertas.GuardarCambiosAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var alerta = await _alertas.ObtenerPorIdAsync(id);
        if (alerta == null) return;
        _alertas.Eliminar(alerta);
        await _auditoria.AgregarAsync(new AuditLog { Accion = "ELIMINAR", Entidad = "Alerta", EntidadId = id, Detalle = $"Alerta eliminada: {alerta.Titulo}" });
        await _alertas.GuardarCambiosAsync();
    }
}
