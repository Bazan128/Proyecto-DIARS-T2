using Microsoft.AspNetCore.Mvc;
using t2pryct.Services;
using t2pryct.ViewModels;

namespace t2pryct.Controllers
{

    public class VerificadorController : Controller
    {
        private readonly IRiesgoService _riesgo;

        public VerificadorController(IRiesgoService riesgo) => _riesgo = riesgo;

        [HttpGet]
        public IActionResult Index() => View(new VerificadorViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VerificadorViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            modelo.Resultado = await _riesgo.VerificarAsync(modelo.Consulta);
            if (!modelo.Resultado.ConsultaValida)
            {
                ModelState.AddModelError(nameof(modelo.Consulta), modelo.Resultado.MensajeError!);
                modelo.Resultado = null;
            }
            return View(modelo);
        }
    }
}
