using Microsoft.AspNetCore.Mvc;
using t2pryct.Models;
using t2pryct.Services;

namespace t2pryct.Controllers
{

    public class AlertaController : Controller
    {
        private readonly IAlertaService _alertas;
        private readonly ISelectorService _selectores;

        public AlertaController(IAlertaService alertas, ISelectorService selectores)
        {
            _alertas = alertas;
            _selectores = selectores;
        }

        public async Task<IActionResult> Index() => View(await _alertas.ListarAsync());

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarListasAsync();
            return View(new Alerta());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Titulo,Mensaje,DistritoId,RubroId,Activa")] Alerta alerta)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(alerta);
            }
            await _alertas.CrearAsync(alerta);
            TempData["Mensaje"] = "Alerta publicada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var alerta = await _alertas.ObtenerAsync(id);
            if (alerta == null) return NotFound();
            await CargarListasAsync();
            return View(alerta);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id,Titulo,Mensaje,DistritoId,RubroId,FechaPublicacion,Activa")] Alerta alerta)
        {
            if (id != alerta.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return View(alerta);
            }
            await _alertas.EditarAsync(alerta);
            TempData["Mensaje"] = "Alerta actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _alertas.EliminarAsync(id);
            TempData["Mensaje"] = "Alerta eliminada.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            ViewBag.Distritos = await _selectores.DistritosAsync(paraFiltro: true);
            ViewBag.Rubros = await _selectores.RubrosAsync(paraFiltro: true);
        }
    }
}
