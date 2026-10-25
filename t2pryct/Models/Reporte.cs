using System.ComponentModel.DataAnnotations;

namespace t2pryct.Models;

public class Reporte
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    public int ModalidadExtorsionId { get; set; }
    public ModalidadExtorsion? Modalidad { get; set; }

    public int DistritoId { get; set; }
    public Distrito? Distrito { get; set; }

    public int RubroId { get; set; }
    public Rubro? Rubro { get; set; }

    [StringLength(20)]
    [Display(Name = "Número emisor")]
    public string? NumeroEmisor { get; set; }

    [Display(Name = "Medio de pago")]
    public MedioPago MedioPago { get; set; }

    [StringLength(30)]
    [Display(Name = "Cuenta / billetera destino")]
    public string? CuentaDestino { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Mensaje recibido")]
    public string Mensaje { get; set; } = string.Empty;

    [Display(Name = "Monto exigido (S/)")]
    public decimal? MontoExigido { get; set; }

    [Required, StringLength(64)]
    public string IpHash { get; set; } = string.Empty;

    [Display(Name = "Estado de verificación")]
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Pendiente;

    public int? ClusterId { get; set; }
    public Cluster? Cluster { get; set; }

    public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
    public ICollection<ReporteDenunciado> ReporteDenunciados { get; set; } = new List<ReporteDenunciado>();
}

public class Evidencia
{
    [Key]
    public int Id { get; set; }

    public int ReporteId { get; set; }
    public Reporte? Reporte { get; set; }

    [StringLength(200)]
    [Display(Name = "Nombre de archivo")]
    public string? NombreArchivo { get; set; }

    [StringLength(300)]
    [Display(Name = "Ruta del archivo")]
    public string? RutaArchivo { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Display(Name = "Registrada")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}

public class ReporteDenunciado
{
    public int ReporteId { get; set; }
    public Reporte? Reporte { get; set; }

    public int DenunciadoId { get; set; }
    public Denunciado? Denunciado { get; set; }
}
