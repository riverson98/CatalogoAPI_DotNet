using CatalogoAPI.Models;

namespace CatalogoAPI.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    IEnumerable<Produto> BuscaProdutosPorCategoria(int id);
}
