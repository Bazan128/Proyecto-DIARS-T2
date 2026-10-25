using Microsoft.AspNetCore.Mvc;
using t2pryct.Services;
using t2pryct.ViewModels;

namespace t2pryct.Controllers
{

    public class ReporteController : Controller
    {
        private readonly IReporteService _reportes;
        private readonly ISelectorService _selectores;

        public ReporteController(IReporteService reportes, ISelectorService selectores)
        {
            _reportes = reportes;
            _selectores = selectores;
        }

        [HttpGet]
        public async Task<IActionResult> Crear(string? numero)
        {
            var modelo = new ReporteCrearViewModel { NumeroEmisor = numero };
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ReporteCrearViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var resultado = await _reportes.RegistrarAsync(modelo, ip);
                if (resultado.Exitoso)
                    return RedirectToAction(nameof(Confirmacion), new { codigo = resultado.Codigo });

                foreach (var (campo, mensaje) in resultado.Errores)
                    ModelState.AddModelError(campo, mensaje);
            }

            await CargarListasAsync(modelo);
            return View(modelo);
        }

        public IActionResult Confirmacion(string codigo) =>
            View(new ConfirmacionReporteViewModel { Codigo = codigo });

        public async Task<IActionResult> Index() => View(await _reportes.ListarAsync());

        public async Task<IActionResult> Detalle(int id)
        {
            var reporte = await _reportes.ObtenerAsync(id);
            return reporte == null ? NotFound() : View(reporte);
        }

        private async Task CargarListasAsync(ReporteCrearViewModel modelo)
        {
            modelo.Modalidades = await _selectores.ModalidadesAsync();
            modelo.Distritos = await _selectores.DistritosAsync();
            modelo.Rubros = await _selectores.RubrosAsync();
        }
    }
}
