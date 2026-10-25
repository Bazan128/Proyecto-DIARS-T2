using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ResultadoVerificacionViewModel
{
    public string Consulta { get; set; } = string.Empty;
    public bool ConsultaValida { get; set; } = true;
    public string? MensajeError { get; set; }
    public NivelSemaforo Nivel { get; set; } = NivelSemaforo.Verde;
    public int TotalReportes { get; set; }
    public int Puntaje { get; set; }
    public List<string> Distritos { get; set; } = new();
    public bool EnExpedienteActivo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}
