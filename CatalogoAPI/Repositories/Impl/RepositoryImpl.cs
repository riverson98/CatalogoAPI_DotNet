using CatalogoAPI.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CatalogoAPI.Repositories.Impl;

public class RepositoryImpl<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public RepositoryImpl(AppDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<T> BuscaTodos()
    {
        return _context.Set<T>().AsNoTracking().ToList();
    }

    public T Busca(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().FirstOrDefault(predicate);
    }

    public T Adiciona(T entidade)
    {
        _context.Add(entidade);
        return entidade;
    }

    public T Atualiza(T entidade)
    {
        _context.Set<T>().Update(entidade);
        return entidade;
    }

    public T Deleta(T entidade)
    {
        _context.Set<T>().Remove(entidade);
        return entidade;
    }
}
