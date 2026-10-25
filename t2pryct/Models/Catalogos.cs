using System.ComponentModel.DataAnnotations;

namespace t2pryct.Models;

public interface ICatalogo
{
    int Id { get; set; }
    string Nombre { get; set; }
    bool Activo { get; set; }
}

public class Distrito : ICatalogo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del distrito es obligatorio.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres.")]
    [Display(Name = "Distrito")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Provincia")]
    [StringLength(80, ErrorMessage = "La provincia no puede superar los 80 caracteres.")]
    public string? Provincia { get; set; } = "Trujillo";

    public bool Activo { get; set; } = true;

    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    public ICollection<Cluster> Clusters { get; set; } = new List<Cluster>();
}

public class Rubro : ICatalogo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del rubro es obligatorio.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres.")]
    [Display(Name = "Rubro")]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
    public ICollection<Cluster> Clusters { get; set; } = new List<Cluster>();
}

public class ModalidadExtorsion : ICatalogo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la modalidad es obligatorio.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres.")]
    [Display(Name = "Modalidad")]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
