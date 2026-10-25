# EscudoLibertad – Plataforma web anti-extorsión digital (T2, hasta Semana 9)

ASP.NET Core MVC (.NET 10) + EF Core 10 Code First + SQL Server (localhost).

## Cómo ejecutar
```powershell
cd t2pryct
dotnet ef database update      # crea EscudoLibertadDb con datos de prueba en localhost
dotnet run
```
Cadena de conexión: `appsettings.json` → `EscudoLibertadDb` (SQL Server en `localhost`, usuario `sa`).

## Match código ↔ diagramas (demo en vivo)
| Artefacto | Código |
|---|---|
| CU-01 Registrar reporte anónimo (Prototipo 2) | `ReporteController.Crear` → `ReporteService.RegistrarAsync` → `ReporteRepository` · `Views/Reporte/Crear.cshtml` · `ReporteCrearViewModel` |
| CU-02 Verificador semafórico (Prototipo 1) | `VerificadorController.Index` → `RiesgoService.VerificarAsync` → `DenunciadoRepository` · `Views/Verificador/Index.cshtml` |
| CU-05 Catálogos (CRUD independientes, S4) | `DistritosController`, `RubrosController`, `ModalidadesController` → `CatalogoService<T>` → `Repository<T>` |
| CU-06 Motor de correlación | `PanelController.Correlacion` → `CorrelacionService` + estrategias (`Services/Strategies`) |
| CU-07 Mapa de calor (Prototipo 4) | `PanelController.Mapa` (densidad por distrito/rubro, sin librerías externas) |
| **CU-08 Maestro-detalle (Prototipo 5)** | `CasosController` → `ClusterService.GuardarAsync` → `ClusterRepository` · `Views/Casos/Formulario.cshtml` |
| CU-09 Carpeta fiscal (Prototipo 6) | `CasosController.Detalle` (vista imprimible → "Guardar como PDF" del navegador) |
| CU-10 Alertas a gremios | `AlertaController` → `AlertaService` |
| Tablero (Prototipo 3) | `PanelController.Index` → `PanelService` |
| Modelo de datos | `Models/*`, `Data/EscudoLibertadDbContext.cs`, `Migrations/` |

## Temas del sílabo aplicados
- **S4** Proyecto base + CRUD de modelos independientes (Distrito, Rubro, Modalidad, Alerta).
- **S5** Validaciones cliente y servidor: DataAnnotations, `IValidatableObject`, atributo propio `TelefonoPeruano` (`IClientModelValidator` + `wwwroot/js/validaciones-custom.js`) y reglas en servicios.
- **S7** Relaciones: **1:N** (Distrito→Reporte, Cluster→Denunciado, Cluster→Reporte, Reporte→Evidencia), **N:M** (Reporte⇄Denunciado vía `ReporteDenunciado`), **1:1** (Denunciado⇄PerfilRiesgo).
- **S8** Patrón **Repositorio** (`Data/Repositories`) + **Strategy** (`IRiesgoStrategy`, `ICorrelacionStrategy`) + **Singleton** (`CorrelationCacheManager`).
- **S9** Mantenimiento **maestro-detalle** (Cluster → Denunciados y Reportes) con guardado atómico.

## Arquitectura en capas cerradas
`Controllers → Services → Data/Repositories → DbContext/Models` (ViewModels para las vistas).

## Reglas de negocio implementadas
- RN-01 Máximo 3 reportes/hora por IP anonimizada (hash SHA-256, la IP nunca se guarda).
- Celular peruano: 9 dígitos que empiezan con 9; cuenta bancaria: 10–20 dígitos; monto S/ 0–50,000.
- Semáforo: Verde = 0 reportes · Amarillo = 1–2 reportes en un distrito · Rojo = ≥3 reportes, varios distritos o expediente activo.
- Puntaje de riesgo 0–100 (frecuencia + distritos + evidencias + expediente).
- RN-02 Expediente cerrado no se edita hasta reabrirlo · RN-03 Derivar/cerrar exige ≥1 denunciado y ≥1 reporte · RN-04 Derivar a Fiscalía exige fiscal asignado · RN-05 Un denunciado/reporte pertenece a un solo expediente · RN-06 No se elimina un expediente derivado a Fiscalía.
- Auditoría de registros, modificaciones y consultas anónimas.

## Fuera de alcance (temas posteriores a la Semana 9)
Autenticación/roles con Identity y autorización de rutas (S12–14), geolocalización en tiempo real y librerías externas no vistas en clase.

