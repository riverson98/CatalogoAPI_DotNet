using CatalogoAPI.Context;
using CatalogoAPI.Models;
using CatalogoAPI.Pagination;
using X.PagedList;

namespace CatalogoAPI.Repositories.Impl;

public class ProdutoRespositoryImpl : RepositoryImpl<Produto>, IProdutoRepository
{
    public ProdutoRespositoryImpl(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> BuscaProdutosPorCategoriaAsync(int id)
    {
        var produtos = await BuscaTodosAsync();

        return produtos.Where(categoria => categoria.CategoriaId.Equals(id));
    }

    public async Task<IPagedList<Produto>> BuscaTodosOsProdutosComPaginacaoAsync(ParametrosDePaginacaoDosProdutos parametrosDePaginacao)
    {
        var produtos = await BuscaTodosAsync();
        var produtosOrdenados = produtos.OrderBy(produto => produto.ProdutoId).AsQueryable();

        var produtosPaginados = await produtosOrdenados.ToPagedListAsync(parametrosDePaginacao.NumeroDaPagina,
                                                                    parametrosDePaginacao.QuantidadeDeItensPorPagina);
        return produtosPaginados;
    }

    public async Task<IPagedList<Produto>> FiltraProdutosPorPrecoAsync(ProdutosFiltroPreco filtro)
    {
        var produtos = await BuscaTodosAsync();
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

        var produtosPaginados = await produtos.ToPagedListAsync(filtro.NumeroDaPagina, filtro.QuantidadeDeItensPorPagina);
        return produtosPaginados;
    }
}
