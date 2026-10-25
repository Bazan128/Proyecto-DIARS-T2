using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class DenunciadoDetalleViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tipo")]
    public TipoDenunciado Tipo { get; set; } = TipoDenunciado.Celular;

    [Required(ErrorMessage = "El número o cuenta es obligatorio.")]
    [StringLength(30, ErrorMessage = "Máximo 30 caracteres.")]
    [Display(Name = "Número / cuenta")]
    public string Valor { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "Máximo 120 caracteres.")]
    [Display(Name = "Titular")]
    public string? Titular { get; set; }

    public int Puntaje { get; set; }
    public DateTime? FechaInclusion { get; set; }
    public bool Eliminar { get; set; }
}
