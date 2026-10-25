using System.ComponentModel.DataAnnotations;

namespace t2pryct.Models;

public class Cluster
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Código de expediente")]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "Nombre de la red delictiva")]
    public string NombreRed { get; set; } = string.Empty;

    public int DistritoId { get; set; }
    public Distrito? Distrito { get; set; }

    public int RubroId { get; set; }
    public Rubro? Rubro { get; set; }

    [Display(Name = "Severidad")]
    public NivelSeveridad Severidad { get; set; } = NivelSeveridad.Medio;

    [Display(Name = "Estado procesal")]
    public EstadoExpediente Estado { get; set; } = EstadoExpediente.EnInvestigacion;

    [StringLength(120)]
    [Display(Name = "Fiscal / investigador asignado")]
    public string? FiscalAsignado { get; set; }

    [StringLength(1000)]
    [Display(Name = "Observaciones policiales")]
    public string? Observaciones { get; set; }

    [Display(Name = "Creado")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<Denunciado> Denunciados { get; set; } = new List<Denunciado>();
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
