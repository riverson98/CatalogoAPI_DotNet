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

    public ListaPaginada<Produto> FiltraProdutosPorPreco(ProdutosFiltroPreco filtro)
    {
        var produtos = BuscaTodos().AsQueryable();
        if(filtro.Preco.HasValue && !string.IsNullOrEmpty(filtro.PrecoCriterio))
        {
            if (filtro.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                produtos = produtos.Where(produtoFiltrado => produtoFiltrado.Preco > filtro.Preco.Value)
                                        .OrderBy(produto => produto.Preco);

            else if(filtro.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                produtos = produtos.Where(produtoFiltrado => produtoFiltrado.Preco < filtro.Preco.Value)
                                        .OrderBy(produto => produto.Preco);

            else if(filtro.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                produtos = produtos.Where(produtoFiltrado => produtoFiltrado.Preco.Equals(filtro.Preco.Value))
                                        .OrderBy(produto => produto.Preco);
        }

        var produtosFiltrados = ListaPaginada<Produto>.ParaListaPaginada(produtos, filtro.NumeroDaPagina,
                                                                                        filtro.QuantidadeDeItensPorPagina);
        return produtosFiltrados;
    }
}
