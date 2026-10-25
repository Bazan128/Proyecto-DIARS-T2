using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class MapaCalorViewModel
{
    public int? DistritoId { get; set; }
    public int? RubroId { get; set; }
    [DataType(DataType.Date)] public DateTime? Desde { get; set; }
    [DataType(DataType.Date)] public DateTime? Hasta { get; set; }
    public List<ContadorViewModel> PorDistrito { get; set; } = new();
    public List<ContadorViewModel> PorRubro { get; set; } = new();
    public int Total { get; set; }
    public int Maximo { get; set; }
    public IEnumerable<SelectListItem>? Distritos { get; set; }
    public IEnumerable<SelectListItem>? Rubros { get; set; }
}
