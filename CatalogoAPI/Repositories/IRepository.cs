using System.Linq.Expressions;

namespace CatalogoAPI.Repositories;

public interface IRepository<T>
{
    IEnumerable<T> BuscaTodos();
    T? Busca(Expression<Func<T, bool>> predicate);
    T Adiciona(T entidade);
    T Atualiza(T entidade);
    T Deleta(T entidade);  
}
