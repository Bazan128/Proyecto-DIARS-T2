using System.ComponentModel.DataAnnotations;

namespace t2pryct.Models;

public class Denunciado
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Tipo")]
    public TipoDenunciado Tipo { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Número / cuenta")]
    public string Valor { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Titular aparente")]
    public string? Titular { get; set; }

    [Display(Name = "Registrado")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public int? ClusterId { get; set; }
    public Cluster? Cluster { get; set; }

    [Display(Name = "Fecha de inclusión")]
    public DateTime? FechaInclusionCluster { get; set; }

    public PerfilRiesgo? PerfilRiesgo { get; set; }

    public ICollection<ReporteDenunciado> ReporteDenunciados { get; set; } = new List<ReporteDenunciado>();
}

public class PerfilRiesgo
{
    [Key]
    public int DenunciadoId { get; set; }
    public Denunciado? Denunciado { get; set; }

    [Display(Name = "Puntaje de riesgo")]
    public int Puntaje { get; set; }

    public NivelSemaforo Semaforo { get; set; } = NivelSemaforo.Verde;

    [Display(Name = "Reportes")]
    public int TotalReportes { get; set; }

    [Display(Name = "Distritos")]
    public int TotalDistritos { get; set; }

    [Display(Name = "Calculado")]
    public DateTime FechaCalculo { get; set; } = DateTime.Now;
}
