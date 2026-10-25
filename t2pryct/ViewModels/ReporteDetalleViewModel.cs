using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ReporteDetalleViewModel
{
    public int ReporteId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public decimal? Monto { get; set; }
    public int Evidencias { get; set; }
    public bool Eliminar { get; set; }
}
