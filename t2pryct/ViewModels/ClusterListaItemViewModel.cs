using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ClusterListaItemViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string NombreRed { get; set; } = string.Empty;
    public string Distrito { get; set; } = string.Empty;
    public string Rubro { get; set; } = string.Empty;
    public NivelSeveridad Severidad { get; set; }
    public EstadoExpediente Estado { get; set; }
    public int TotalDenunciados { get; set; }
    public int TotalReportes { get; set; }
}
