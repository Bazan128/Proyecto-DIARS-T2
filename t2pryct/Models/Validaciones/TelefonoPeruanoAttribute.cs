using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace t2pryct.Models.Validaciones;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class TelefonoPeruanoAttribute : ValidationAttribute, IClientModelValidator
{
    public TelefonoPeruanoAttribute()
    {
        ErrorMessage = "El celular debe tener 9 dígitos y empezar con 9.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success;

        return ReglasDenunciado.EsCelularValido(value.ToString())
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessageString);
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-telefonoperuano", ErrorMessageString);
    }

    private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key)) attributes.Add(key, value);
    }
}
