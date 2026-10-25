using System.ComponentModel.DataAnnotations;

namespace t2pryct.Models;

public class Alerta
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 150 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El mensaje es obligatorio.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "El mensaje debe tener entre 10 y 1000 caracteres.")]
    [Display(Name = "Mensaje de prevención")]
    public string Mensaje { get; set; } = string.Empty;

    [Display(Name = "Distrito")]
    public int? DistritoId { get; set; }
    public Distrito? Distrito { get; set; }

    [Display(Name = "Rubro / gremio")]
    public int? RubroId { get; set; }
    public Rubro? Rubro { get; set; }

    [Display(Name = "Publicada")]
    public DateTime FechaPublicacion { get; set; } = DateTime.Now;

    public bool Activa { get; set; } = true;
}

public class AuditLog
{
    [Key]
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required, StringLength(40)]
    public string Accion { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Entidad { get; set; } = string.Empty;

    public int? EntidadId { get; set; }

    [StringLength(500)]
    public string? Detalle { get; set; }
}
