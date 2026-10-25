using Microsoft.AspNetCore.Mvc.Rendering;
using t2pryct.Data.Repositories;
using t2pryct.Models;

namespace t2pryct.Services;

public record ResultadoCatalogo(bool Exitoso, string? Error = null);

public interface ICatalogoService<T> where T : class, ICatalogo
{
    Task<List<T>> ListarAsync();
    Task<T?> ObtenerAsync(int id);
    Task<ResultadoCatalogo> CrearAsync(T entidad);
    Task<ResultadoCatalogo> EditarAsync(T entidad);
    Task<ResultadoCatalogo> EliminarAsync(int id);
}

public class CatalogoService<T> : ICatalogoService<T> where T : class, ICatalogo
{
    private readonly IRepository<T> _repositorio;

    public CatalogoService(IRepository<T> repositorio) => _repositorio = repositorio;

    public async Task<List<T>> ListarAsync() =>
        (await _repositorio.ListarAsync()).OrderBy(x => x.Nombre).ToList();

    public Task<T?> ObtenerAsync(int id) => _repositorio.ObtenerPorIdAsync(id);

    public async Task<ResultadoCatalogo> CrearAsync(T entidad)
    {
        entidad.Nombre = entidad.Nombre.Trim();
        if (await ExisteNombreAsync(entidad.Nombre, null))
            return new ResultadoCatalogo(false, "Ya existe un registro con ese nombre.");

        await _repositorio.AgregarAsync(entidad);
        await _repositorio.GuardarCambiosAsync();
        return new ResultadoCatalogo(true);
    }

    public async Task<ResultadoCatalogo> EditarAsync(T entidad)
    {
        entidad.Nombre = entidad.Nombre.Trim();
        if (await ExisteNombreAsync(entidad.Nombre, entidad.Id))
            return new ResultadoCatalogo(false, "Ya existe otro registro con ese nombre.");

        _repositorio.Actualizar(entidad);
        await _repositorio.GuardarCambiosAsync();
        return new ResultadoCatalogo(true);
    }

    public async Task<ResultadoCatalogo> EliminarAsync(int id)
    {
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad == null) return new ResultadoCatalogo(false, "El registro no existe.");

        try
        {
            _repositorio.Eliminar(entidad);
            await _repositorio.GuardarCambiosAsync();
            return new ResultadoCatalogo(true);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return new ResultadoCatalogo(false,
                "No se puede eliminar: el registro está siendo usado por reportes, expedientes o alertas. Puede desactivarlo en su lugar.");
        }
    }

    private async Task<bool> ExisteNombreAsync(string nombre, int? idExcluido)
    {
        var todos = await _repositorio.ListarAsync();
        return todos.Any(x => x.Id != idExcluido && string.Equals(x.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
    }
}

public interface ISelectorService
{
    Task<IEnumerable<SelectListItem>> DistritosAsync(bool paraFiltro = false);
    Task<IEnumerable<SelectListItem>> RubrosAsync(bool paraFiltro = false);
    Task<IEnumerable<SelectListItem>> ModalidadesAsync();
}

public class SelectorService : ISelectorService
{
    private readonly IRepository<Distrito> _distritos;
    private readonly IRepository<Rubro> _rubros;
    private readonly IRepository<ModalidadExtorsion> _modalidades;

    public SelectorService(IRepository<Distrito> distritos, IRepository<Rubro> rubros, IRepository<ModalidadExtorsion> modalidades)
    {
        _distritos = distritos;
        _rubros = rubros;
        _modalidades = modalidades;
    }

    public async Task<IEnumerable<SelectListItem>> DistritosAsync(bool paraFiltro = false) =>
        Armar((await _distritos.ListarAsync()).Where(x => x.Activo).OrderBy(x => x.Nombre)
              .Select(x => (x.Id, x.Nombre)), paraFiltro ? "Todos los distritos" : "Seleccione el distrito...", paraFiltro);

    public async Task<IEnumerable<SelectListItem>> RubrosAsync(bool paraFiltro = false) =>
        Armar((await _rubros.ListarAsync()).Where(x => x.Activo).OrderBy(x => x.Nombre)
              .Select(x => (x.Id, x.Nombre)), paraFiltro ? "Todos los rubros" : "Seleccione el rubro...", paraFiltro);

    public async Task<IEnumerable<SelectListItem>> ModalidadesAsync() =>
        Armar((await _modalidades.ListarAsync()).Where(x => x.Activo).OrderBy(x => x.Id)
              .Select(x => (x.Id, x.Nombre)), "Seleccione la modalidad...", false);

    private static IEnumerable<SelectListItem> Armar(IEnumerable<(int Id, string Nombre)> items, string placeholder, bool paraFiltro)
    {
        var lista = new List<SelectListItem> { new(placeholder, paraFiltro ? "" : "0") };
        lista.AddRange(items.Select(i => new SelectListItem(i.Nombre, i.Id.ToString())));
        return lista;
    }
}
