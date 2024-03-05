using CatalogoAPI.Context;
using CatalogoAPI.Models;
using CatalogoAPI.Pagination;

namespace CatalogoAPI.Repositories.Impl;

public class CategoriaRepositoryImpl : RepositoryImpl<Categoria>, ICategoriaRepository
{
    public CategoriaRepositoryImpl(AppDbContext context) : base(context)
    {
    }

    public ListaPaginada<Categoria> BuscaTodasAsCategoriasComPaginacao(ParametrosDePaginacaoDasCategorias parametrosDePaginacao)
    {
        var categorias = BuscaTodos().OrderBy(categoria => categoria.CategoriaId).AsQueryable();
        return ListaPaginada<Categoria>.ParaListaPaginada(categorias, parametrosDePaginacao.NumeroDaPagina,
                                                          parametrosDePaginacao.QuantidadeDeItensPorPagina);
    }

    public ListaPaginada<Categoria> FiltraCategoriaPorNome(CategoriasFiltroNome filtro)
    {
        var categorias = BuscaTodos().AsQueryable();

        if (!string.IsNullOrEmpty(filtro.Nome))
            categorias = categorias.Where(categoriaFiltrada => categoriaFiltrada.Nome.Contains(filtro.Nome));

        var categoriasFiltradas = ListaPaginada<Categoria>.ParaListaPaginada(categorias, filtro.NumeroDaPagina,
                                                                                filtro.QuantidadeDeItensPorPagina);
        return categoriasFiltradas;
    }
}
