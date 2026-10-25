using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ContadorViewModel
{
    public string Etiqueta { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
