using Microsoft.AspNetCore.Mvc;
using t2pryct.Services;
using t2pryct.ViewModels;

namespace t2pryct.Controllers
{

    public class CasosController : Controller
    {
        private readonly IClusterService _clusters;
        private readonly ISelectorService _selectores;

        public CasosController(IClusterService clusters, ISelectorService selectores)
        {
            _clusters = clusters;
            _selectores = selectores;
        }

        public async Task<IActionResult> Index() => View(await _clusters.ListarAsync());

        [HttpGet]
        public async Task<IActionResult> Crear() => await MostrarAsync((await _clusters.ObtenerFormularioAsync(null))!);

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Crear(ClusterFormViewModel modelo, string? accion) => ProcesarAsync(modelo, accion);

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var modelo = await _clusters.ObtenerFormularioAsync(id);
            return modelo == null ? NotFound() : await MostrarAsync(modelo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Editar(int id, ClusterFormViewModel modelo, string? accion)
        {
            modelo.Id = id;
            return ProcesarAsync(modelo, accion);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var cluster = await _clusters.ObtenerDetalleAsync(id);
            return cluster == null ? NotFound() : View(cluster);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _clusters.EliminarAsync(id);
            TempData[resultado.Exitoso ? "Mensaje" : "Error"] = resultado.Exitoso
                ? "Expediente eliminado. Sus denunciados y reportes quedaron desvinculados."
                : resultado.Errores.First().Mensaje;
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ProcesarAsync(ClusterFormViewModel modelo, string? accion)
        {
            accion ??= "guardar";

            if (accion == "agregarDenunciado")
            {
                modelo.Denunciados.Add(new DenunciadoDetalleViewModel());
                ModelState.Clear();
                return await MostrarAsync(modelo);
            }

            if (accion.StartsWith("quitarDenunciado:") && int.TryParse(accion.Split(':')[1], out var indice)
                && indice >= 0 && indice < modelo.Denunciados.Count)
            {
                modelo.Denunciados.RemoveAt(indice);
                ModelState.Clear();
                return await MostrarAsync(modelo);
            }

            if (accion == "agregarReporte")
            {
                var (fila, error) = await _clusters.ResolverReporteAsync(modelo.CodigoReporteNuevo, modelo);
                ModelState.Clear();
                if (fila != null)
                {
                    modelo.Reportes.Add(fila);
                    modelo.CodigoReporteNuevo = null;
                }
                else
                {
                    ViewBag.ErrorReporte = error;
                }
                return await MostrarAsync(modelo);
            }

            for (var i = 0; i < modelo.Denunciados.Count; i++)
            {
                if (modelo.Denunciados[i].Eliminar)
                {
                    ModelState.Remove($"Denunciados[{i}].Valor");
                    ModelState.Remove($"Denunciados[{i}].Titular");
                }
            }

            if (ModelState.IsValid)
            {
                var resultado = await _clusters.GuardarAsync(modelo);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = $"Expediente {resultado.Codigo} guardado correctamente (cabecera y detalles en una sola transacción).";
                    return RedirectToAction(nameof(Editar), new { id = resultado.Id });
                }
                foreach (var (campo, mensaje) in resultado.Errores)
                    ModelState.AddModelError(campo, mensaje);
            }

            return await MostrarAsync(modelo);
        }

        private async Task<IActionResult> MostrarAsync(ClusterFormViewModel modelo)
        {
            modelo.Distritos = await _selectores.DistritosAsync();
            modelo.Rubros = await _selectores.RubrosAsync();
            return View("Formulario", modelo);
        }
    }
}
