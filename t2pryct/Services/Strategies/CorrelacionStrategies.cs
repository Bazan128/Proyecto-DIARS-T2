using t2pryct.Models;
using t2pryct.Models.Validaciones;

namespace t2pryct.Services.Strategies;

public record SugerenciaCorrelacion(string Criterio, string Descripcion, IReadOnlyList<string> CodigosReportes, double Similitud);

public interface ICorrelacionStrategy
{
    string Nombre { get; }
    IEnumerable<SugerenciaCorrelacion> Detectar(IReadOnlyList<Reporte> reportes);
}

public class CoincidenciaExactaStrategy : ICorrelacionStrategy
{
    public string Nombre => "Coincidencia exacta";

    public IEnumerable<SugerenciaCorrelacion> Detectar(IReadOnlyList<Reporte> reportes)
    {
        var pares = new List<(string Clave, string Codigo)>();
        foreach (var r in reportes)
        {
            var emisor = ReglasDenunciado.Normalizar(r.NumeroEmisor);
            var cuenta = ReglasDenunciado.Normalizar(r.CuentaDestino);
            if (emisor.Length > 0) pares.Add((emisor, r.Codigo));
            if (cuenta.Length > 0 && cuenta != emisor) pares.Add((cuenta, r.Codigo));
        }

        return pares.GroupBy(p => p.Clave)
            .Where(g => g.Select(x => x.Codigo).Distinct().Count() >= 2)
            .Select(g => new SugerenciaCorrelacion(
                Nombre,
                $"Número/cuenta {g.Key} repetido en {g.Select(x => x.Codigo).Distinct().Count()} reportes",
                g.Select(x => x.Codigo).Distinct().ToList(),
                1.0));
    }
}

public class SimilitudLevenshteinStrategy : ICorrelacionStrategy
{
    public const double UmbralSimilitud = 0.75;
    public string Nombre => "Similitud de texto (Levenshtein)";

    public IEnumerable<SugerenciaCorrelacion> Detectar(IReadOnlyList<Reporte> reportes)
    {
        for (var i = 0; i < reportes.Count; i++)
        {
            for (var j = i + 1; j < reportes.Count; j++)
            {
                var similitud = Levenshtein.Similitud(reportes[i].Mensaje, reportes[j].Mensaje);
                if (similitud >= UmbralSimilitud)
                {
                    yield return new SugerenciaCorrelacion(
                        Nombre,
                        $"Mensajes {similitud:P0} similares (posible plantilla de la misma banda)",
                        new[] { reportes[i].Codigo, reportes[j].Codigo },
                        similitud);
                }
            }
        }
    }
}

public static class Levenshtein
{
    public static int Distancia(string a, string b)
    {
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var anterior = new int[b.Length + 1];
        var actual = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++) anterior[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            actual[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var costo = a[i - 1] == b[j - 1] ? 0 : 1;
                actual[j] = Math.Min(Math.Min(actual[j - 1] + 1, anterior[j] + 1), anterior[j - 1] + costo);
            }
            (anterior, actual) = (actual, anterior);
        }
        return anterior[b.Length];
    }

    public static double Similitud(string? a, string? b)
    {
        var x = Normalizar(a);
        var y = Normalizar(b);
        var max = Math.Max(x.Length, y.Length);
        if (max == 0) return 1.0;
        return 1.0 - (double)Distancia(x, y) / max;
    }

    private static string Normalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
        var t = texto.Trim().ToLowerInvariant();
        return t.Length > 300 ? t[..300] : t;
    }
}
