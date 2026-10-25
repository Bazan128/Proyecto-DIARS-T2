using Microsoft.EntityFrameworkCore;

namespace t2pryct.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<List<T>> ListarAsync();
    Task<T?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(T entidad);
    void Actualizar(T entidad);
    void Eliminar(T entidad);

    Task<int> GuardarCambiosAsync();
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly EscudoLibertadDbContext _contexto;
    protected readonly DbSet<T> _dbSet;

    public Repository(EscudoLibertadDbContext contexto)
    {
        _contexto = contexto;
        _dbSet = contexto.Set<T>();
    }

    public virtual Task<List<T>> ListarAsync() => _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task<T?> ObtenerPorIdAsync(int id) => await _dbSet.FindAsync(id);

    public virtual async Task AgregarAsync(T entidad) => await _dbSet.AddAsync(entidad);

    public virtual void Actualizar(T entidad) => _dbSet.Update(entidad);

    public virtual void Eliminar(T entidad) => _dbSet.Remove(entidad);

    public Task<int> GuardarCambiosAsync() => _contexto.SaveChangesAsync();
}
