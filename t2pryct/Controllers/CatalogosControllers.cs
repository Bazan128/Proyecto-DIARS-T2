using Microsoft.AspNetCore.Mvc;
using t2pryct.Models;
using t2pryct.Services;

namespace t2pryct.Controllers
{

    public abstract class CatalogoControllerBase<T> : Controller where T : class, ICatalogo, new()
    {
        private readonly ICatalogoService<T> _servicio;

        protected CatalogoControllerBase(ICatalogoService<T> servicio) => _servicio = servicio;

        protected abstract string Titulo { get; }
        protected virtual bool TieneProvincia => false;

        private IActionResult VistaCatalogo(string vista, object? modelo)
        {
            ViewData["Titulo"] = Titulo;
            ViewData["TieneProvincia"] = TieneProvincia;
            return View($"~/Views/Shared/Catalogo/{vista}.cshtml", modelo);
        }

        public async Task<IActionResult> Index() => VistaCatalogo("Index", await _servicio.ListarAsync());

        [HttpGet]
        public IActionResult Crear() => VistaCatalogo("Formulario", new T());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(T entidad)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _servicio.CrearAsync(entidad);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Registro creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(nameof(ICatalogo.Nombre), resultado.Error!);
            }
            return VistaCatalogo("Formulario", entidad);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var entidad = await _servicio.ObtenerAsync(id);
            return entidad == null ? NotFound() : VistaCatalogo("Formulario", entidad);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, T entidad)
        {
            if (id != entidad.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var resultado = await _servicio.EditarAsync(entidad);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Registro actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(nameof(ICatalogo.Nombre), resultado.Error!);
            }
            return VistaCatalogo("Formulario", entidad);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _servicio.EliminarAsync(id);
            TempData[resultado.Exitoso ? "Mensaje" : "Error"] = resultado.Exitoso ? "Registro eliminado." : resultado.Error;
            return RedirectToAction(nameof(Index));
        }
    }

    public class DistritosController : CatalogoControllerBase<Distrito>
    {
        public DistritosController(ICatalogoService<Distrito> servicio) : base(servicio) { }
        protected override string Titulo => "Distritos de La Libertad";
        protected override bool TieneProvincia => true;
    }

    public class RubrosController : CatalogoControllerBase<Rubro>
    {
        public RubrosController(ICatalogoService<Rubro> servicio) : base(servicio) { }
        protected override string Titulo => "Rubros comerciales";
    }

    public class ModalidadesController : CatalogoControllerBase<ModalidadExtorsion>
    {
        public ModalidadesController(ICatalogoService<ModalidadExtorsion> servicio) : base(servicio) { }
        protected override string Titulo => "Modalidades de extorsión";
    }
}
