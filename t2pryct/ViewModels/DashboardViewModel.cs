using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class DashboardViewModel
{
    public int TotalReportes { get; set; }
    public int ExpedientesActivos { get; set; }
    public int NumerosCriticos { get; set; }
    public string DistritoMayorAlerta { get; set; } = "-";
    public List<ContadorViewModel> ReportesPorSemana { get; set; } = new();
    public List<ContadorViewModel> ReportesPorModalidad { get; set; } = new();
    public List<DenunciadoResumenViewModel> TopDenunciados { get; set; } = new();
    public int SugerenciasPendientes { get; set; }
}
