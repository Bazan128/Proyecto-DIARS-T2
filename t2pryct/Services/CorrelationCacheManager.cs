using System.Collections.Concurrent;
using t2pryct.ViewModels;

namespace t2pryct.Services;

public sealed class CorrelationCacheManager
{
    private static readonly Lazy<CorrelationCacheManager> _instancia =
        new(() => new CorrelationCacheManager());

    private readonly ConcurrentDictionary<string, (ResultadoVerificacionViewModel Resultado, DateTime Expira)> _cache = new();

    public TimeSpan Vigencia { get; } = TimeSpan.FromMinutes(5);

    private CorrelationCacheManager() { }

    public static CorrelationCacheManager Instancia => _instancia.Value;

    public bool TryObtener(string clave, out ResultadoVerificacionViewModel? resultado)
    {
        if (_cache.TryGetValue(clave, out var entrada) && entrada.Expira > DateTime.UtcNow)
        {
            resultado = entrada.Resultado;
            return true;
        }
        resultado = null;
        return false;
    }

    public void Guardar(string clave, ResultadoVerificacionViewModel resultado) =>
        _cache[clave] = (resultado, DateTime.UtcNow.Add(Vigencia));

    public void Invalidar(string clave) => _cache.TryRemove(clave, out _);

    public void LimpiarTodo() => _cache.Clear();
}
