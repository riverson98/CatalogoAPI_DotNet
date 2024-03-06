using CatalogoAPI.Models;
using CatalogoAPI.Pagination;
using X.PagedList;

namespace CatalogoAPI.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> BuscaProdutosPorCategoriaAsync(int id);
    Task<IPagedList<Produto>> BuscaTodosOsProdutosComPaginacaoAsync(ParametrosDePaginacaoDosProdutos parametrosDePaginacao);
    Task<IPagedList<Produto>> FiltraProdutosPorPrecoAsync(ProdutosFiltroPreco filtro);
}
