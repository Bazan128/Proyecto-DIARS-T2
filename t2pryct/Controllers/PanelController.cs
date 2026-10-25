using Microsoft.AspNetCore.Mvc;
using t2pryct.Services;
using t2pryct.ViewModels;

namespace t2pryct.Controllers
{

    public class PanelController : Controller
    {
        private readonly IPanelService _panel;
        private readonly ICorrelacionService _correlacion;
        private readonly ISelectorService _selectores;

        public PanelController(IPanelService panel, ICorrelacionService correlacion, ISelectorService selectores)
        {
            _panel = panel;
            _correlacion = correlacion;
            _selectores = selectores;
        }

        public async Task<IActionResult> Index() => View(await _panel.ObtenerDashboardAsync());

        public async Task<IActionResult> Mapa(MapaCalorViewModel filtros)
        {
            var modelo = await _panel.ObtenerMapaAsync(filtros);
            modelo.Distritos = await _selectores.DistritosAsync(paraFiltro: true);
            modelo.Rubros = await _selectores.RubrosAsync(paraFiltro: true);
            return View(modelo);
        }

        public async Task<IActionResult> Correlacion() => View(await _correlacion.ObtenerSugerenciasAsync());

        public async Task<IActionResult> Auditoria() => View(await _panel.ObtenerAuditoriaAsync(100));
    }
}
