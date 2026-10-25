using t2pryct.Models;

namespace t2pryct.Services.Strategies;

public record ContextoRiesgo(int TotalReportes, int TotalDistritos, bool TieneEvidencias, bool EnCluster);

public interface IRiesgoStrategy
{
    string Nombre { get; }
    int Calcular(ContextoRiesgo contexto);
}

public class FrecuenciaReportesStrategy : IRiesgoStrategy
{
    public string Nombre => "Frecuencia de reportes";
    public int Calcular(ContextoRiesgo c) => Math.Min(45, c.TotalReportes * 15);
}

public class DispersionDistritosStrategy : IRiesgoStrategy
{
    public string Nombre => "Recurrencia en distritos";
    public int Calcular(ContextoRiesgo c) => Math.Min(30, Math.Max(0, c.TotalDistritos - 1) * 15);
}

public class EvidenciasStrategy : IRiesgoStrategy
{
    public string Nombre => "Evidencias adjuntas";
    public int Calcular(ContextoRiesgo c) => c.TieneEvidencias ? 10 : 0;
}

public class PertenenciaClusterStrategy : IRiesgoStrategy
{
    public string Nombre => "Pertenencia a expediente";
    public int Calcular(ContextoRiesgo c) => c.EnCluster ? 25 : 0;
}
