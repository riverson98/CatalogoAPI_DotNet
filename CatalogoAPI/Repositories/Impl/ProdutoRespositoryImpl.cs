using CatalogoAPI.Context;
using CatalogoAPI.Models;
using CatalogoAPI.Pagination;

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

    public ListaPaginada<Produto> BuscaTodosOsProdutosComPaginacao(ParametrosDePaginacaoDosProdutos parametrosDePaginacao)
    {
        var produtos = BuscaTodos()
            .OrderBy(produto => produto.ProdutoId)
            .AsQueryable();

        return ListaPaginada<Produto>.ParaListaPaginada(produtos, parametrosDePaginacao.NumeroDaPagina,
                                                                        parametrosDePaginacao.QuantidadeDeItensPorPagina);
    }
}
