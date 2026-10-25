using System.Security.Cryptography;
using System.Text;
using t2pryct.Data.Repositories;
using t2pryct.Models;
using t2pryct.Models.Validaciones;
using t2pryct.ViewModels;

namespace t2pryct.Services;

public interface IReporteService
{

    Task<ResultadoOperacion> RegistrarAsync(ReporteCrearViewModel modelo, string? direccionIp);
    Task<List<Reporte>> ListarAsync();
    Task<Reporte?> ObtenerAsync(int id);
}

public class ReporteService : IReporteService
{

    public const int MaximoReportesPorHora = 3;
    private const string SalAnonimizacion = "EscudoLibertad-2026";

    private readonly IReporteRepository _reportes;
    private readonly IDenunciadoRepository _denunciados;
    private readonly IAuditRepository _auditoria;
    private readonly IRiesgoService _riesgo;
    private readonly ICorrelacionService _correlacion;
    private readonly IWebHostEnvironment _entorno;

    public ReporteService(IReporteRepository reportes, IDenunciadoRepository denunciados, IAuditRepository auditoria,
                          IRiesgoService riesgo, ICorrelacionService correlacion, IWebHostEnvironment entorno)
    {
        _reportes = reportes;
        _denunciados = denunciados;
        _auditoria = auditoria;
        _riesgo = riesgo;
        _correlacion = correlacion;
        _entorno = entorno;
    }

    public Task<List<Reporte>> ListarAsync() => _reportes.ListarConDetalleAsync();

    public Task<Reporte?> ObtenerAsync(int id) => _reportes.ObtenerConDetalleAsync(id);

    public async Task<ResultadoOperacion> RegistrarAsync(ReporteCrearViewModel modelo, string? direccionIp)
    {
        var resultado = new ResultadoOperacion();

        var ipHash = CalcularHashIp(direccionIp);

        var recientes = await _reportes.ContarPorIpDesdeAsync(ipHash, DateTime.Now.AddHours(-1));
        if (recientes >= MaximoReportesPorHora)
            return resultado.Error(string.Empty, $"Se alcanzó el máximo de {MaximoReportesPorHora} reportes por hora. Intente más tarde.");

        var reporte = new Reporte
        {
            Codigo = await GenerarCodigoAsync(),
            Fecha = DateTime.Now,
            ModalidadExtorsionId = modelo.ModalidadExtorsionId,
            DistritoId = modelo.DistritoId,
            RubroId = modelo.RubroId,
            NumeroEmisor = string.IsNullOrWhiteSpace(modelo.NumeroEmisor) ? null : ReglasDenunciado.Normalizar(modelo.NumeroEmisor),
            MedioPago = modelo.MedioPago,
            CuentaDestino = string.IsNullOrWhiteSpace(modelo.CuentaDestino) ? null : ReglasDenunciado.Normalizar(modelo.CuentaDestino),
            Mensaje = modelo.Mensaje.Trim(),
            MontoExigido = modelo.MontoExigido,
            IpHash = ipHash,
            EstadoVerificacion = EstadoVerificacion.Pendiente
        };

        if (modelo.ArchivoEvidencia != null && modelo.ArchivoEvidencia.Length > 0)
        {
            var webRoot = _entorno.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var carpetaUploads = Path.Combine(webRoot, "uploads", "evidencias");
            if (!Directory.Exists(carpetaUploads))
            {
                Directory.CreateDirectory(carpetaUploads);
            }

            var extension = Path.GetExtension(modelo.ArchivoEvidencia.FileName).ToLowerInvariant();
            var nombreFisico = $"EVID_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
            var rutaFisica = Path.Combine(carpetaUploads, nombreFisico);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await modelo.ArchivoEvidencia.CopyToAsync(stream);
            }

            var descripcion = !string.IsNullOrWhiteSpace(modelo.DescripcionEvidencia)
                ? modelo.DescripcionEvidencia.Trim()
                : $"Evidencia adjunta ({modelo.ArchivoEvidencia.FileName})";

            reporte.Evidencias.Add(new Evidencia
            {
                NombreArchivo = modelo.ArchivoEvidencia.FileName,
                RutaArchivo = $"/uploads/evidencias/{nombreFisico}",
                Descripcion = descripcion,
                FechaRegistro = DateTime.Now
            });
        }
        else if (!string.IsNullOrWhiteSpace(modelo.DescripcionEvidencia))
        {
            reporte.Evidencias.Add(new Evidencia
            {
                Descripcion = modelo.DescripcionEvidencia.Trim(),
                FechaRegistro = DateTime.Now
            });
        }

        reporte.ClusterId = await _correlacion.BuscarClusterCoincidenteAsync(reporte);

        var denunciados = new List<Denunciado>();
        if (reporte.NumeroEmisor != null)
            denunciados.Add(await ObtenerOCrearAsync(reporte.NumeroEmisor, TipoDenunciado.Celular));
        if (reporte.CuentaDestino != null && reporte.CuentaDestino != reporte.NumeroEmisor)
            denunciados.Add(await ObtenerOCrearAsync(reporte.CuentaDestino, ReglasDenunciado.DeducirTipo(reporte.CuentaDestino, reporte.MedioPago)));

        reporte.ClusterId ??= denunciados.FirstOrDefault(d => d.ClusterId != null)?.ClusterId;

        foreach (var d in denunciados)
        {
            var vinculo = new ReporteDenunciado { Reporte = reporte, Denunciado = d };
            reporte.ReporteDenunciados.Add(vinculo);
            d.ReporteDenunciados.Add(vinculo);
            _riesgo.AplicarPerfil(d);
        }

        await _reportes.AgregarAsync(reporte);
        await _auditoria.AgregarAsync(new AuditLog
        {
            Accion = "REGISTRO",
            Entidad = "Reporte",
            Detalle = $"Reporte {reporte.Codigo} registrado de forma anónima" +
                      (reporte.ClusterId != null ? " y correlacionado con un expediente existente." : ".")
        });

        await _reportes.GuardarCambiosAsync();

        resultado.Id = reporte.Id;
        resultado.Codigo = reporte.Codigo;
        return resultado;
    }

    private async Task<Denunciado> ObtenerOCrearAsync(string valor, TipoDenunciado tipo)
    {
        var existente = await _denunciados.ObtenerPorValorAsync(valor);
        if (existente != null)
        {
            return (await _denunciados.ObtenerConHistorialAsync(existente.Id))!;
        }

        var nuevo = new Denunciado { Tipo = tipo, Valor = valor, FechaRegistro = DateTime.Now };
        await _denunciados.AgregarAsync(nuevo);
        return nuevo;
    }

    private async Task<string> GenerarCodigoAsync()
    {
        var hoy = DateTime.Now;
        var correlativo = await _reportes.ContarDelDiaAsync(hoy) + 1;
        string codigo;
        do
        {
            codigo = $"REP-{hoy:yyyyMMdd}-{correlativo:0000}";
            correlativo++;
        }
        while (await _reportes.ObtenerPorCodigoAsync(codigo) != null);
        return codigo;
    }

    private static string CalcularHashIp(string? ip)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes((ip ?? "desconocida") + SalAnonimizacion));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
