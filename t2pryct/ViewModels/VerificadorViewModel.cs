using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class VerificadorViewModel
{
    [Required(ErrorMessage = "Ingrese un número de teléfono o una cuenta para consultar.")]
    [StringLength(30, MinimumLength = 6, ErrorMessage = "Ingrese entre 6 y 30 caracteres.")]
    [Display(Name = "Número de teléfono, celular o cuenta Yape/Plin/Bancaria")]
    public string? Consulta { get; set; }

    public ResultadoVerificacionViewModel? Resultado { get; set; }
}
