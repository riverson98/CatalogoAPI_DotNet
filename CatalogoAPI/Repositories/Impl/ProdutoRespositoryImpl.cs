using CatalogoAPI.Context;
using CatalogoAPI.Models;

namespace CatalogoAPI.Repositories.Impl;

public class ProdutoRespositoryImpl : RepositoryImpl<Produto>, IProdutoRepository
{
    public ProdutoRespositoryImpl(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Produto> BuscaProdutosPorCategoria(int id)
    {
        return BuscaTodos().Where(categoria => categoria.CategoriaId.Equals(id));
    }
}
