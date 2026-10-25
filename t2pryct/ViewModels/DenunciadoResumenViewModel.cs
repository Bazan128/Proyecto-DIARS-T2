using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class DenunciadoResumenViewModel
{
    public string Valor { get; set; } = string.Empty;
    public TipoDenunciado Tipo { get; set; }
    public int Puntaje { get; set; }
    public NivelSemaforo Semaforo { get; set; }
    public int Reportes { get; set; }
    public string? Expediente { get; set; }
}
