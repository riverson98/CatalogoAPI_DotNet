using CatalogoAPI.Models;
using CatalogoAPI.Pagination;

namespace CatalogoAPI.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    IEnumerable<Produto> BuscaProdutosPorCategoria(int id);
    ListaPaginada<Produto> BuscaTodosOsProdutosComPaginacao(ParametrosDePaginacaoDosProdutos parametrosDePaginacao);
    ListaPaginada<Produto> FiltraProdutosPorPreco(ProdutosFiltroPreco filtro);
}
