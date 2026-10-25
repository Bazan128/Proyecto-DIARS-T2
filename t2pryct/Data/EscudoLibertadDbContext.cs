using Microsoft.EntityFrameworkCore;
using t2pryct.Models;

namespace t2pryct.Data;

public class EscudoLibertadDbContext : DbContext
{
    public EscudoLibertadDbContext(DbContextOptions<EscudoLibertadDbContext> options) : base(options)
    {
    }

    public DbSet<Distrito> Distritos => Set<Distrito>();
    public DbSet<Rubro> Rubros => Set<Rubro>();
    public DbSet<ModalidadExtorsion> Modalidades => Set<ModalidadExtorsion>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<Evidencia> Evidencias => Set<Evidencia>();
    public DbSet<Denunciado> Denunciados => Set<Denunciado>();
    public DbSet<PerfilRiesgo> PerfilesRiesgo => Set<PerfilRiesgo>();
    public DbSet<ReporteDenunciado> ReporteDenunciados => Set<ReporteDenunciado>();
    public DbSet<Cluster> Clusters => Set<Cluster>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reporte>().Property(r => r.MontoExigido).HasPrecision(10, 2);
        modelBuilder.Entity<Reporte>().HasIndex(r => r.Codigo).IsUnique();
        modelBuilder.Entity<Denunciado>().HasIndex(d => d.Valor).IsUnique();
        modelBuilder.Entity<Cluster>().HasIndex(c => c.Codigo).IsUnique();

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Distrito).WithMany(d => d.Reportes)
            .HasForeignKey(r => r.DistritoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Rubro).WithMany(x => x.Reportes)
            .HasForeignKey(r => r.RubroId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Modalidad).WithMany(m => m.Reportes)
            .HasForeignKey(r => r.ModalidadExtorsionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Evidencia>()
            .HasOne(e => e.Reporte).WithMany(r => r.Evidencias)
            .HasForeignKey(e => e.ReporteId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cluster>()
            .HasOne(c => c.Distrito).WithMany(d => d.Clusters)
            .HasForeignKey(c => c.DistritoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cluster>()
            .HasOne(c => c.Rubro).WithMany(r => r.Clusters)
            .HasForeignKey(c => c.RubroId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Denunciado>()
            .HasOne(d => d.Cluster).WithMany(c => c.Denunciados)
            .HasForeignKey(d => d.ClusterId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Cluster).WithMany(c => c.Reportes)
            .HasForeignKey(r => r.ClusterId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Alerta>()
            .HasOne(a => a.Distrito).WithMany().HasForeignKey(a => a.DistritoId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Alerta>()
            .HasOne(a => a.Rubro).WithMany().HasForeignKey(a => a.RubroId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReporteDenunciado>().HasKey(x => new { x.ReporteId, x.DenunciadoId });

        modelBuilder.Entity<ReporteDenunciado>()
            .HasOne(x => x.Reporte).WithMany(r => r.ReporteDenunciados)
            .HasForeignKey(x => x.ReporteId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReporteDenunciado>()
            .HasOne(x => x.Denunciado).WithMany(d => d.ReporteDenunciados)
            .HasForeignKey(x => x.DenunciadoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PerfilRiesgo>()
            .HasOne(p => p.Denunciado).WithOne(d => d.PerfilRiesgo)
            .HasForeignKey<PerfilRiesgo>(p => p.DenunciadoId).OnDelete(DeleteBehavior.Cascade);

        Sembrar(modelBuilder);
    }

    private static void Sembrar(ModelBuilder mb)
    {
        mb.Entity<Distrito>().HasData(
            new Distrito { Id = 1, Nombre = "Trujillo", Provincia = "Trujillo", Activo = true },
            new Distrito { Id = 2, Nombre = "El Porvenir", Provincia = "Trujillo", Activo = true },
            new Distrito { Id = 3, Nombre = "La Esperanza", Provincia = "Trujillo", Activo = true },
            new Distrito { Id = 4, Nombre = "Florencia de Mora", Provincia = "Trujillo", Activo = true },
            new Distrito { Id = 5, Nombre = "Alto Trujillo", Provincia = "Trujillo", Activo = true },
            new Distrito { Id = 6, Nombre = "Víctor Larco Herrera", Provincia = "Trujillo", Activo = true });

        mb.Entity<Rubro>().HasData(
            new Rubro { Id = 1, Nombre = "Bodega", Activo = true },
            new Rubro { Id = 2, Nombre = "Transporte", Activo = true },
            new Rubro { Id = 3, Nombre = "Calzado", Activo = true },
            new Rubro { Id = 4, Nombre = "Farmacia", Activo = true },
            new Rubro { Id = 5, Nombre = "Mercado", Activo = true },
            new Rubro { Id = 6, Nombre = "Otro", Activo = true });

        mb.Entity<ModalidadExtorsion>().HasData(
            new ModalidadExtorsion { Id = 1, Nombre = "Llamada amenazante", Activo = true },
            new ModalidadExtorsion { Id = 2, Nombre = "Mensaje de WhatsApp", Activo = true },
            new ModalidadExtorsion { Id = 3, Nombre = "Cobro de cupo presencial", Activo = true },
            new ModalidadExtorsion { Id = 4, Nombre = "Préstamo gota a gota", Activo = true },
            new ModalidadExtorsion { Id = 5, Nombre = "Atentado con explosivo", Activo = true });

        mb.Entity<Cluster>().HasData(new Cluster
        {
            Id = 1,
            Codigo = "EXP-2026-TRU-0001",
            NombreRed = "Red Extorsiva El Porvenir - Microempresarios del Calzado",
            DistritoId = 2,
            RubroId = 3,
            Severidad = NivelSeveridad.Alto,
            Estado = EstadoExpediente.EnInvestigacion,
            FiscalAsignado = "Fiscal Provincial de Turno - Trujillo",
            Observaciones = "Mensajes con plantilla similar y mismo número emisor en tres distritos.",
            FechaCreacion = new DateTime(2026, 9, 10, 9, 0, 0)
        });

        mb.Entity<Denunciado>().HasData(
            new Denunciado { Id = 1, Tipo = TipoDenunciado.Celular, Valor = "987654321", Titular = "Desconocido", FechaRegistro = new DateTime(2026, 9, 1, 10, 0, 0), ClusterId = 1, FechaInclusionCluster = new DateTime(2026, 9, 10, 9, 0, 0) },
            new Denunciado { Id = 2, Tipo = TipoDenunciado.Yape, Valor = "912345678", Titular = "J. R. M.", FechaRegistro = new DateTime(2026, 9, 1, 10, 0, 0), ClusterId = 1, FechaInclusionCluster = new DateTime(2026, 9, 10, 9, 0, 0) },
            new Denunciado { Id = 3, Tipo = TipoDenunciado.CuentaBancaria, Valor = "19112345678901", Titular = "Cuenta puente", FechaRegistro = new DateTime(2026, 9, 3, 15, 0, 0), ClusterId = 1, FechaInclusionCluster = new DateTime(2026, 9, 10, 9, 0, 0) },
            new Denunciado { Id = 4, Tipo = TipoDenunciado.Celular, Valor = "945123789", FechaRegistro = new DateTime(2026, 9, 12, 8, 0, 0) },
            new Denunciado { Id = 5, Tipo = TipoDenunciado.Celular, Valor = "933221100", FechaRegistro = new DateTime(2026, 9, 14, 20, 0, 0) });

        mb.Entity<PerfilRiesgo>().HasData(
            new PerfilRiesgo { DenunciadoId = 1, Puntaje = 100, Semaforo = NivelSemaforo.Rojo, TotalReportes = 3, TotalDistritos = 3, FechaCalculo = new DateTime(2026, 9, 15) },
            new PerfilRiesgo { DenunciadoId = 2, Puntaje = 80, Semaforo = NivelSemaforo.Rojo, TotalReportes = 2, TotalDistritos = 1, FechaCalculo = new DateTime(2026, 9, 15) },
            new PerfilRiesgo { DenunciadoId = 3, Puntaje = 50, Semaforo = NivelSemaforo.Rojo, TotalReportes = 1, TotalDistritos = 1, FechaCalculo = new DateTime(2026, 9, 15) },
            new PerfilRiesgo { DenunciadoId = 4, Puntaje = 45, Semaforo = NivelSemaforo.Rojo, TotalReportes = 2, TotalDistritos = 2, FechaCalculo = new DateTime(2026, 9, 15) },
            new PerfilRiesgo { DenunciadoId = 5, Puntaje = 25, Semaforo = NivelSemaforo.Amarillo, TotalReportes = 1, TotalDistritos = 1, FechaCalculo = new DateTime(2026, 9, 15) });

        const string hash = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

        mb.Entity<Reporte>().HasData(
            new Reporte { Id = 1, Codigo = "REP-20260901-0001", Fecha = new DateTime(2026, 9, 1, 10, 0, 0), ModalidadExtorsionId = 2, DistritoId = 2, RubroId = 3, NumeroEmisor = "987654321", MedioPago = MedioPago.Yape, CuentaDestino = "912345678", Mensaje = "Paga 500 soles hoy o volamos tu taller con dinamita. Sabemos donde vives", MontoExigido = 500m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Verificado, ClusterId = 1 },
            new Reporte { Id = 2, Codigo = "REP-20260903-0002", Fecha = new DateTime(2026, 9, 3, 15, 0, 0), ModalidadExtorsionId = 1, DistritoId = 4, RubroId = 3, NumeroEmisor = "987654321", MedioPago = MedioPago.BCP, CuentaDestino = "19112345678901", Mensaje = "Paga 500 soles hoy o volamos tu taller con dinamita. Sabemos donde trabajas", MontoExigido = 500m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Verificado, ClusterId = 1 },
            new Reporte { Id = 3, Codigo = "REP-20260905-0003", Fecha = new DateTime(2026, 9, 5, 18, 30, 0), ModalidadExtorsionId = 2, DistritoId = 2, RubroId = 3, NumeroEmisor = "912345678", MedioPago = MedioPago.Yape, Mensaje = "Deposita 300 soles hoy a este yape o te quemamos la fabrica", MontoExigido = 300m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Pendiente, ClusterId = 1 },
            new Reporte { Id = 4, Codigo = "REP-20260908-0004", Fecha = new DateTime(2026, 9, 8, 9, 15, 0), ModalidadExtorsionId = 2, DistritoId = 3, RubroId = 1, NumeroEmisor = "987654321", MedioPago = MedioPago.Yape, Mensaje = "Paga 300 soles hoy o volamos tu bodega con dinamita. Sabemos donde vives", MontoExigido = 300m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Pendiente, ClusterId = 1 },
            new Reporte { Id = 5, Codigo = "REP-20260912-0005", Fecha = new DateTime(2026, 9, 12, 8, 0, 0), ModalidadExtorsionId = 4, DistritoId = 5, RubroId = 2, NumeroEmisor = "945123789", MedioPago = MedioPago.Plin, Mensaje = "Entrega la cuota semanal de 200 soles o atacamos tu unidad de transporte", MontoExigido = 200m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Pendiente },
            new Reporte { Id = 6, Codigo = "REP-20260913-0006", Fecha = new DateTime(2026, 9, 13, 11, 40, 0), ModalidadExtorsionId = 1, DistritoId = 2, RubroId = 1, NumeroEmisor = "945123789", MedioPago = MedioPago.Plin, Mensaje = "Entrega la cuota semanal de 150 soles o atacamos tu bodega", MontoExigido = 150m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Pendiente },
            new Reporte { Id = 7, Codigo = "REP-20260914-0007", Fecha = new DateTime(2026, 9, 14, 20, 0, 0), ModalidadExtorsionId = 2, DistritoId = 1, RubroId = 4, NumeroEmisor = "933221100", MedioPago = MedioPago.Efectivo, Mensaje = "Si no depositas 100 soles te denunciaremos falsamente ante la policia", MontoExigido = 100m, IpHash = hash, EstadoVerificacion = EstadoVerificacion.Pendiente });

        mb.Entity<ReporteDenunciado>().HasData(
            new ReporteDenunciado { ReporteId = 1, DenunciadoId = 1 },
            new ReporteDenunciado { ReporteId = 1, DenunciadoId = 2 },
            new ReporteDenunciado { ReporteId = 2, DenunciadoId = 1 },
            new ReporteDenunciado { ReporteId = 2, DenunciadoId = 3 },
            new ReporteDenunciado { ReporteId = 3, DenunciadoId = 2 },
            new ReporteDenunciado { ReporteId = 4, DenunciadoId = 1 },
            new ReporteDenunciado { ReporteId = 5, DenunciadoId = 4 },
            new ReporteDenunciado { ReporteId = 6, DenunciadoId = 4 },
            new ReporteDenunciado { ReporteId = 7, DenunciadoId = 5 });

        mb.Entity<Evidencia>().HasData(
            new Evidencia { Id = 1, ReporteId = 1, Descripcion = "Captura del chat de WhatsApp con la amenaza", FechaRegistro = new DateTime(2026, 9, 1, 10, 5, 0) },
            new Evidencia { Id = 2, ReporteId = 2, Descripcion = "Captura del voucher de transferencia exigida", FechaRegistro = new DateTime(2026, 9, 3, 15, 5, 0) },
            new Evidencia { Id = 3, ReporteId = 7, Descripcion = "Captura del mensaje de texto recibido", FechaRegistro = new DateTime(2026, 9, 14, 20, 5, 0) });

        mb.Entity<Alerta>().HasData(new Alerta
        {
            Id = 1,
            Titulo = "Nueva modalidad: falsa denuncia como amenaza",
            Mensaje = "Se detectaron mensajes que amenazan con denunciar falsamente al comerciante si no deposita dinero. No pague, bloquee el número y reporte en la plataforma.",
            DistritoId = 1,
            RubroId = 4,
            FechaPublicacion = new DateTime(2026, 9, 15, 8, 0, 0),
            Activa = true
        });
    }
}
