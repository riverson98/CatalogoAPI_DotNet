using CatalogoAPI.Context;
using CatalogoAPI.Models;
using CatalogoAPI.Pagination;
using X.PagedList;

namespace CatalogoAPI.Repositories.Impl;

public class CategoriaRepositoryImpl : RepositoryImpl<Categoria>, ICategoriaRepository
{
    public CategoriaRepositoryImpl(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Categoria>> BuscaTodasAsCategoriasComPaginacaoAsync(ParametrosDePaginacaoDasCategorias parametrosDePaginacao)
    {
        var categorias = await BuscaTodosAsync();
        var categoriasOrdenada = categorias.OrderBy(categoria => categoria.CategoriaId).AsQueryable();

        //return ListaPaginada<Categoria>.ParaListaPaginada(categoriaOrdenada, parametrosDePaginacao.NumeroDaPagina,
        //                                                  parametrosDePaginacao.QuantidadeDeItensPorPagina);
        var categoriasPaginada = await categoriasOrdenada.ToPagedListAsync(parametrosDePaginacao.NumeroDaPagina,
                                                                            parametrosDePaginacao.QuantidadeDeItensPorPagina);
        return categoriasPaginada;
    }

    public async Task<IPagedList<Categoria>> FiltraCategoriaPorNomeAsync(CategoriasFiltroNome filtro)
    {
        var categorias = await BuscaTodosAsync();

        if (!string.IsNullOrEmpty(filtro.Nome))
            categorias = categorias.Where(categoriaFiltrada => categoriaFiltrada.Nome.Contains(filtro.Nome));

        var categoriasFiltradas = await categorias.ToPagedListAsync(filtro.NumeroDaPagina, filtro.QuantidadeDeItensPorPagina);
        return categoriasFiltradas;
    }
}
