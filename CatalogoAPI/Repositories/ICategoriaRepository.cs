using CatalogoAPI.Models;
using CatalogoAPI.Pagination;

namespace CatalogoAPI.Repositories;

public interface ICategoriaRepository : IRepository<Categoria> 
{
    ListaPaginada<Categoria> BuscaTodasAsCategoriasComPaginacao(ParametrosDePaginacaoDasCategorias parametrosDePaginacao);
    ListaPaginada<Categoria> FiltraCategoriaPorNome(CategoriasFiltroNome filtro);
}
