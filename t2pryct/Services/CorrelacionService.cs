using t2pryct.Data.Repositories;
using t2pryct.Models;
using t2pryct.Services.Strategies;

namespace t2pryct.Services;

public interface ICorrelacionService
{

    Task<List<SugerenciaCorrelacion>> ObtenerSugerenciasAsync();

    Task<int?> BuscarClusterCoincidenteAsync(Reporte nuevo);
}

public class CorrelacionService : ICorrelacionService
{
    private readonly IReporteRepository _reportes;
    private readonly IEnumerable<ICorrelacionStrategy> _estrategias;

    public CorrelacionService(IReporteRepository reportes, IEnumerable<ICorrelacionStrategy> estrategias)
    {
        _reportes = reportes;
        _estrategias = estrategias;
    }

    public async Task<List<SugerenciaCorrelacion>> ObtenerSugerenciasAsync()
    {
        var todos = await _reportes.ListarConDetalleAsync();
        var sinExpediente = todos.Where(r => r.ClusterId == null).Select(r => r.Codigo).ToHashSet();

        return _estrategias
            .SelectMany(e => e.Detectar(todos))
            .Where(s => s.CodigosReportes.Any(sinExpediente.Contains) && !EstanTodosEnMismoExpediente(s, todos))
            .OrderByDescending(s => s.CodigosReportes.Count)
            .ThenByDescending(s => s.Similitud)
            .ToList();
    }

    public async Task<int?> BuscarClusterCoincidenteAsync(Reporte nuevo)
    {
        var existentes = await _reportes.ListarConDetalleAsync();
        var todos = existentes.Append(nuevo).ToList();
        var porCodigo = existentes.ToDictionary(r => r.Codigo);

        foreach (var sugerencia in _estrategias.SelectMany(e => e.Detectar(todos)))
        {
            if (!sugerencia.CodigosReportes.Contains(nuevo.Codigo)) continue;

            var cluster = sugerencia.CodigosReportes
                .Where(c => c != nuevo.Codigo && porCodigo.ContainsKey(c))
                .Select(c => porCodigo[c].ClusterId)
                .FirstOrDefault(id => id != null);

            if (cluster != null) return cluster;
        }
        return null;
    }

    private static bool EstanTodosEnMismoExpediente(SugerenciaCorrelacion s, IReadOnlyList<Reporte> todos)
    {
        var clusters = todos.Where(r => s.CodigosReportes.Contains(r.Codigo)).Select(r => r.ClusterId).Distinct().ToList();
        return clusters.Count == 1 && clusters[0] != null;
    }
}
