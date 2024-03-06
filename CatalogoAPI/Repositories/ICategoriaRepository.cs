using CatalogoAPI.Models;
using CatalogoAPI.Pagination;
using X.PagedList;

namespace CatalogoAPI.Repositories;

public interface ICategoriaRepository : IRepository<Categoria> 
{
    Task<IPagedList<Categoria>> BuscaTodasAsCategoriasComPaginacaoAsync(ParametrosDePaginacaoDasCategorias parametrosDePaginacao);
    Task<IPagedList<Categoria>> FiltraCategoriaPorNomeAsync(CategoriasFiltroNome filtro);
}
