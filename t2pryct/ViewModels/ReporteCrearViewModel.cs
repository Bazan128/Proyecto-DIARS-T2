using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.ViewModels;

public class ReporteCrearViewModel : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la modalidad de extorsión.")]
    [Display(Name = "Modalidad")]
    public int ModalidadExtorsionId { get; set; }

    [TelefonoPeruano]
    [Display(Name = "Número telefónico del extorsionador")]
    public string? NumeroEmisor { get; set; }

    [Display(Name = "Medio de pago exigido")]
    public MedioPago MedioPago { get; set; } = MedioPago.Yape;

    [StringLength(30, ErrorMessage = "La cuenta no puede superar los 30 caracteres.")]
    [Display(Name = "Número de cuenta / billetera destino")]
    public string? CuentaDestino { get; set; }

    [Required(ErrorMessage = "Describa el mensaje o la amenaza recibida.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "El mensaje debe tener entre 10 y 1000 caracteres.")]
    [Display(Name = "Mensaje recibido")]
    public string Mensaje { get; set; } = string.Empty;

    [Range(0, 50000, ErrorMessage = "El monto debe estar entre S/ 0 y S/ 50,000.")]
    [Display(Name = "Monto exigido (S/)")]
    public decimal? MontoExigido { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el distrito.")]
    [Display(Name = "Distrito")]
    public int DistritoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el rubro.")]
    [Display(Name = "Rubro")]
    public int RubroId { get; set; }

    [StringLength(200, ErrorMessage = "La descripción de la evidencia no puede superar los 200 caracteres.")]
    [Display(Name = "Descripción o notas de la evidencia")]
    public string? DescripcionEvidencia { get; set; }

    [Display(Name = "Adjuntar archivo de evidencia (imagen JPG, PNG o PDF)")]
    public IFormFile? ArchivoEvidencia { get; set; }

    public IEnumerable<SelectListItem>? Modalidades { get; set; }
    public IEnumerable<SelectListItem>? Distritos { get; set; }
    public IEnumerable<SelectListItem>? Rubros { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(NumeroEmisor) && string.IsNullOrWhiteSpace(CuentaDestino))
        {
            yield return new ValidationResult(
                "Ingrese el número del extorsionador o la cuenta/billetera destino.",
                new[] { nameof(NumeroEmisor) });
        }

        if (!string.IsNullOrWhiteSpace(CuentaDestino) && MedioPago != MedioPago.Efectivo)
        {
            var tipo = MedioPago is MedioPago.Yape or MedioPago.Plin ? TipoDenunciado.Yape : TipoDenunciado.CuentaBancaria;
            var error = ReglasDenunciado.Validar(tipo, CuentaDestino);
            if (error != null)
                yield return new ValidationResult(error, new[] { nameof(CuentaDestino) });
        }

        if (ArchivoEvidencia != null)
        {
            if (ArchivoEvidencia.Length > 5 * 1024 * 1024)
            {
                yield return new ValidationResult(
                    "El archivo de evidencia no debe superar el límite de 5 MB.",
                    new[] { nameof(ArchivoEvidencia) });
            }

            var extension = Path.GetExtension(ArchivoEvidencia.FileName)?.ToLowerInvariant();
            var permitidas = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
            if (string.IsNullOrEmpty(extension) || !permitidas.Contains(extension))
            {
                yield return new ValidationResult(
                    "Solo se admiten capturas de imagen (.JPG, .PNG, .WEBP) o documentos (.PDF).",
                    new[] { nameof(ArchivoEvidencia) });
            }
        }
    }
}
