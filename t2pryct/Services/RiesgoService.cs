using t2pryct.Data.Repositories;
using t2pryct.Models;
using t2pryct.Models.Validaciones;
using t2pryct.Services.Strategies;
using t2pryct.ViewModels;

namespace t2pryct.Services;

public interface IRiesgoService
{

    Task<ResultadoVerificacionViewModel> VerificarAsync(string? consulta);

    void AplicarPerfil(Denunciado denunciado);
}

public class RiesgoService : IRiesgoService
{
    private readonly IDenunciadoRepository _denunciados;
    private readonly IAuditRepository _auditoria;
    private readonly IEnumerable<IRiesgoStrategy> _estrategias;

    public RiesgoService(IDenunciadoRepository denunciados, IAuditRepository auditoria, IEnumerable<IRiesgoStrategy> estrategias)
    {
        _denunciados = denunciados;
        _auditoria = auditoria;
        _estrategias = estrategias;
    }

    public async Task<ResultadoVerificacionViewModel> VerificarAsync(string? consulta)
    {
        var valor = ReglasDenunciado.Normalizar(consulta);
        var esCelular = ReglasDenunciado.EsCelularValido(valor);
        var esCuenta = ReglasDenunciado.EsCuentaValida(valor);

        if (!esCelular && !esCuenta)
        {
            return new ResultadoVerificacionViewModel
            {
                Consulta = consulta ?? string.Empty,
                ConsultaValida = false,
                MensajeError = "Ingrese un celular de 9 dígitos (empieza con 9) o una cuenta bancaria de 10 a 20 dígitos."
            };
        }

        if (!CorrelationCacheManager.Instancia.TryObtener(valor, out var resultado) || resultado == null)
        {
            resultado = await ConsultarBaseDeDatosAsync(valor);
            CorrelationCacheManager.Instancia.Guardar(valor, resultado);
        }

        await _auditoria.AgregarAsync(new AuditLog
        {
            Accion = "CONSULTA",
            Entidad = "Verificador",
            Detalle = $"Consulta ***{valor[^3..]} -> {resultado.Nivel}"
        });
        await _auditoria.GuardarCambiosAsync();

        return resultado;
    }

    private async Task<ResultadoVerificacionViewModel> ConsultarBaseDeDatosAsync(string valor)
    {
        var denunciado = await _denunciados.BuscarConHistorialAsync(valor);
        var reportes = denunciado?.ReporteDenunciados.Select(x => x.Reporte!).ToList() ?? new List<Reporte>();
        var distritos = reportes.Select(r => r.Distrito?.Nombre ?? "").Where(n => n != "").Distinct().OrderBy(n => n).ToList();
        var clusterActivo = denunciado?.Cluster != null && denunciado.Cluster.Estado != EstadoExpediente.Cerrado;

        var nivel = Clasificar(reportes.Count, distritos.Count, clusterActivo);
        var resultado = new ResultadoVerificacionViewModel
        {
            Consulta = valor,
            Nivel = nivel,
            TotalReportes = reportes.Count,
            Puntaje = denunciado?.PerfilRiesgo?.Puntaje ?? 0,
            Distritos = distritos,
            EnExpedienteActivo = clusterActivo
        };

        switch (nivel)
        {
            case NivelSemaforo.Verde:
                resultado.Titulo = "Sin reportes registrados";
                resultado.Mensaje = "No hay reportes de este número o cuenta. Aun así, verifique siempre la identidad del remitente antes de transferir dinero.";
                break;
            case NivelSemaforo.Amarillo:
                resultado.Titulo = "Precaución";
                resultado.Mensaje = $"Existe {reportes.Count} reporte(s) previo(s) en {string.Join(", ", distritos)}. Mantenga cautela y no realice pagos sin verificar.";
                break;
            default:
                resultado.Titulo = "ALERTA MÁXIMA";
                resultado.Mensaje = $"Este número o cuenta ha sido reportado {reportes.Count} vez/veces en {(distritos.Count > 0 ? string.Join(", ", distritos) : "La Libertad")}. NO REALICE NINGÚN PAGO.";
                break;
        }
        return resultado;
    }

    public static NivelSemaforo Clasificar(int reportes, int distritos, bool clusterActivo)
    {
        if (reportes == 0 && !clusterActivo) return NivelSemaforo.Verde;
        if (reportes >= 3 || distritos >= 2 || clusterActivo) return NivelSemaforo.Rojo;
        return NivelSemaforo.Amarillo;
    }

    public void AplicarPerfil(Denunciado denunciado)
    {
        var reportes = denunciado.ReporteDenunciados.Select(x => x.Reporte!).ToList();
        var contexto = new ContextoRiesgo(
            reportes.Count,
            reportes.Select(r => r.DistritoId).Distinct().Count(),
            reportes.Any(r => r.Evidencias.Count > 0),
            denunciado.ClusterId.HasValue || denunciado.Cluster != null);

        var puntaje = Math.Min(100, _estrategias.Sum(e => e.Calcular(contexto)));

        var perfil = denunciado.PerfilRiesgo ??= new PerfilRiesgo { DenunciadoId = denunciado.Id };
        perfil.Puntaje = puntaje;
        perfil.TotalReportes = contexto.TotalReportes;
        perfil.TotalDistritos = contexto.TotalDistritos;
        perfil.Semaforo = Clasificar(contexto.TotalReportes, contexto.TotalDistritos, contexto.EnCluster);
        perfil.FechaCalculo = DateTime.Now;

        CorrelationCacheManager.Instancia.Invalidar(denunciado.Valor);
    }
}
