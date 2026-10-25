using System.Text.RegularExpressions;

namespace t2pryct.Models.Validaciones;

public static class ReglasDenunciado
{
    private static readonly Regex SoloDigitos = new(@"^\d+$", RegexOptions.Compiled);

    public static string Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return string.Empty;
        var limpio = Regex.Replace(valor.Trim(), @"[\s\-\(\)]", "");
        if (limpio.StartsWith("+51")) limpio = limpio[3..];
        else if (limpio.StartsWith("0051")) limpio = limpio[4..];
        return limpio;
    }

    public static bool EsCelularValido(string? valor)
    {
        var n = Normalizar(valor);
        return n.Length == 9 && n[0] == '9' && SoloDigitos.IsMatch(n);
    }

    public static bool EsCuentaValida(string? valor)
    {
        var n = Normalizar(valor);
        return n.Length is >= 10 and <= 20 && SoloDigitos.IsMatch(n);
    }

    public static string? Validar(TipoDenunciado tipo, string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return "El número o cuenta es obligatorio.";
        return tipo switch
        {
            TipoDenunciado.CuentaBancaria => EsCuentaValida(valor) ? null : "La cuenta bancaria debe tener entre 10 y 20 dígitos.",
            _ => EsCelularValido(valor) ? null : "El celular debe tener 9 dígitos y empezar con 9."
        };
    }

    public static TipoDenunciado DeducirTipo(string? valor, MedioPago medio)
    {
        if (EsCelularValido(valor))
        {
            return medio switch
            {
                MedioPago.Yape => TipoDenunciado.Yape,
                MedioPago.Plin => TipoDenunciado.Plin,
                _ => TipoDenunciado.Celular
            };
        }
        return TipoDenunciado.CuentaBancaria;
    }
}
