using CatalogoAPI.Context;
using CatalogoAPI.Models;

namespace CatalogoAPI.Repositories.Impl;

public class CategoriaRepositoryImpl : RepositoryImpl<Categoria>, ICategoriaRepository
{
    public CategoriaRepositoryImpl(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Categoria> BuscaTodasAsCategoriasEProdutos()
    {
        throw new NotImplementedException();
    }
}
