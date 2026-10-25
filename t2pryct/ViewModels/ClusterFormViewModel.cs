using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ClusterFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Código de expediente")]
    public string? Codigo { get; set; }

    [Required(ErrorMessage = "El nombre de la red delictiva es obligatorio.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 150 caracteres.")]
    [Display(Name = "Nombre de la red delictiva")]
    public string NombreRed { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el distrito principal.")]
    [Display(Name = "Distrito principal")]
    public int DistritoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el rubro predominante.")]
    [Display(Name = "Rubro predominante")]
    public int RubroId { get; set; }

    [Display(Name = "Nivel de severidad")]
    public NivelSeveridad Severidad { get; set; } = NivelSeveridad.Medio;

    [Display(Name = "Estado procesal")]
    public EstadoExpediente Estado { get; set; } = EstadoExpediente.EnInvestigacion;

    [StringLength(120, ErrorMessage = "Máximo 120 caracteres.")]
    [Display(Name = "Fiscal / investigador asignado")]
    public string? FiscalAsignado { get; set; }

    [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres.")]
    [Display(Name = "Observaciones policiales")]
    public string? Observaciones { get; set; }

    [Display(Name = "Código del reporte")]
    public string? CodigoReporteNuevo { get; set; }

    public List<DenunciadoDetalleViewModel> Denunciados { get; set; } = new();
    public List<ReporteDetalleViewModel> Reportes { get; set; } = new();

    public IEnumerable<SelectListItem>? Distritos { get; set; }
    public IEnumerable<SelectListItem>? Rubros { get; set; }
}
