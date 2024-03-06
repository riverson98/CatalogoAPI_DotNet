using System.Linq.Expressions;

namespace CatalogoAPI.Repositories;

public interface IRepository<T>
{
    Task<IEnumerable<T>> BuscaTodosAsync();
    Task<T?> BuscaAsync(Expression<Func<T, bool>> predicate);
    T Adiciona(T entidade);
    T Atualiza(T entidade);
    T Deleta(T entidade);  
}
