using System.ComponentModel.DataAnnotations;

namespace t2pryct.Models;

public enum TipoDenunciado
{
    [Display(Name = "Celular")] Celular = 1,
    [Display(Name = "Yape")] Yape = 2,
    [Display(Name = "Plin")] Plin = 3,
    [Display(Name = "Cuenta bancaria")] CuentaBancaria = 4
}

public enum MedioPago
{
    [Display(Name = "Yape")] Yape = 1,
    [Display(Name = "Plin")] Plin = 2,
    [Display(Name = "BCP")] BCP = 3,
    [Display(Name = "BBVA")] BBVA = 4,
    [Display(Name = "Efectivo")] Efectivo = 5
}

public enum NivelSeveridad
{
    [Display(Name = "Medio")] Medio = 1,
    [Display(Name = "Alto")] Alto = 2,
    [Display(Name = "Crítico")] Critico = 3
}

public enum EstadoExpediente
{
    [Display(Name = "En investigación preliminar")] EnInvestigacion = 1,
    [Display(Name = "Derivado a Fiscalía")] DerivadoFiscalia = 2,
    [Display(Name = "Caso cerrado")] Cerrado = 3
}

public enum EstadoVerificacion
{
    [Display(Name = "Pendiente")] Pendiente = 1,
    [Display(Name = "Verificado")] Verificado = 2,
    [Display(Name = "Descartado")] Descartado = 3
}

public enum NivelSemaforo
{
    Verde = 1,
    Amarillo = 2,
    Rojo = 3
}
