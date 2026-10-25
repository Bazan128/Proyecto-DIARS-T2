using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ResultadoOperacion
{
    public bool Exitoso => Errores.Count == 0;
    public List<(string Campo, string Mensaje)> Errores { get; } = new();
    public int? Id { get; set; }
    public string? Codigo { get; set; }

    public ResultadoOperacion Error(string campo, string mensaje)
    {
        Errores.Add((campo, mensaje));
        return this;
    }
}
